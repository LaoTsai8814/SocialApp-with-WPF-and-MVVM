using ChatServer.ShareLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.UserInfo
{
    internal class ServerUserData
    {
        static ConcurrentDictionary<string, People>? UserList = new ConcurrentDictionary<string, People>();


        static ConcurrentQueue<TcpClient>? PendingUserMale = new ConcurrentQueue<TcpClient>();

        static ConcurrentQueue<TcpClient>? PendingUserFemale = new ConcurrentQueue<TcpClient>();

        static ConcurrentDictionary<string,PairedUser> PairedList = new ConcurrentDictionary<string, PairedUser>();

        public static bool AddUser(TcpClient client,People user)
        {
            Console.WriteLine(user.Gender);
            user.cmd = string.Empty;
            if (UserList.ContainsKey(((IPEndPoint)client.Client.RemoteEndPoint).ToString()))
            {
                return false;
            }

            UserList.TryAdd(((IPEndPoint)client.Client.RemoteEndPoint).ToString(), user);
            return true;
        }

        public static bool RemoveUser(TcpClient client)
        {
            if (client?.Client?.RemoteEndPoint is not IPEndPoint endPoint)
            {
                return false;
            }

            string clientKey = endPoint.ToString();
            
            PairedUser user ;
            
            bool removedFromPairedList = PairedList.TryRemove(clientKey, out user);
            var response = new RequestCondition
            {
                cmd = "quit",
                Success = true,

            };
            NetworkStream stream;
            if (user.female == client)
            {
                
                stream = user.male.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(JsonHandle.SerializeJson(response));
                stream.WriteAsync(data, 0, data.Length);
            }
            else
            {
                stream = user.female.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(JsonHandle.SerializeJson(response));
                stream.WriteAsync(data, 0, data.Length);
            }
            return removedFromPairedList;
        }

        public static bool RemoveUserFully(TcpClient client)
        {

            return UserList.TryRemove(((IPEndPoint)client.Client.RemoteEndPoint).ToString(), out _)&& RemoveUser(client);

        }

        public static async Task<bool> PairUser(TcpClient client)
        {
            //Console.WriteLine("Entry");
            if (UserList.ContainsKey(((IPEndPoint)client.Client.RemoteEndPoint).ToString()))
            {
                Console.WriteLine("Contain");
                Console.WriteLine(UserList[((IPEndPoint)client.Client.RemoteEndPoint).ToString()].Gender);
                
            }
            else
            {
                Console.WriteLine("Dont Contain");
            }
            if(UserList[((IPEndPoint)client.Client.RemoteEndPoint).ToString()].Gender == "Male")
            {
                
                PendingUserMale.Enqueue(client);
            }
            else if (UserList[((IPEndPoint)client.Client.RemoteEndPoint).ToString()].Gender == "Female")
            {
                PendingUserFemale.Enqueue(client);
            }

            return await WaitingForPair(client);
  
        }
        static async Task<bool> WaitingForPair(TcpClient client)
        {
            while (!PairedList.ContainsKey(((IPEndPoint)client.Client.RemoteEndPoint).ToString())) 
            {
                await Task.Delay(100);
            }
            return true;
        }

        public static async Task Pairing()
        {
            Console.WriteLine("Pairing Function Opened");
            while (true)
            {
                if (!PendingUserMale.IsEmpty && !PendingUserFemale.IsEmpty)
                {
                    TcpClient femaleresult;
                    TcpClient maleresult;
                    
                    if (PendingUserFemale.TryDequeue(out femaleresult) && PendingUserMale.TryDequeue(out maleresult))
                    {
                        while (!IsConnected(maleresult)|| !IsConnected(femaleresult)) 
                        {
                            if (!IsConnected(maleresult))
                            {
                                PendingUserMale.TryDequeue(out _);
                            }
                            else if (!IsConnected(femaleresult))
                            {
                                PendingUserFemale.TryDequeue(out _);
                            }
                        
                        }
                        PairedUser paired = new PairedUser
                        {
                            female = femaleresult,
                            male = maleresult
                        };
                        PairedList.TryAdd(((IPEndPoint)femaleresult.Client.RemoteEndPoint).ToString(), paired);
                        PairedList.TryAdd(((IPEndPoint)maleresult.Client.RemoteEndPoint).ToString(), paired);
                        Console.WriteLine("Paired Success");
                    }
                    else
                    {
                        Console.WriteLine("Error Occur During Dequeue");
                    }
                }
            }
            
        }

        public static async Task<bool> SendDataSingleMode(TcpClient client,TextMessage text)
        {
            try
            {
                PairedUser pairclient = PairedList[((IPEndPoint)client.Client.RemoteEndPoint).ToString()];
                if (pairclient != null)
                {
                    if (IsConnected(pairclient.male) && IsConnected(pairclient.female))
                    {
                        NetworkStream stream;
                        ReceiveTextMessage textmsg;
                        if (((IPEndPoint)client.Client.RemoteEndPoint).ToString() == ((IPEndPoint)pairclient.male.Client.RemoteEndPoint).ToString())
                        {

                            stream = pairclient.female.GetStream();
                            textmsg = new ReceiveTextMessage
                            {
                                cmd = "sendto",
                                Textmsg = text.Textmsg,
                                name = UserList[((IPEndPoint)pairclient.male.Client.RemoteEndPoint).ToString()].Name

                            };

                        }
                        else
                        {
                            stream = pairclient.male.GetStream();
                            textmsg = new ReceiveTextMessage
                            {
                                cmd = "sendto",
                                Textmsg = text.Textmsg,
                                name = UserList[((IPEndPoint)pairclient.female.Client.RemoteEndPoint).ToString()].Name

                            };
                        }
                        

                        byte[] responseBytes = Encoding.UTF8.GetBytes(JsonHandle.SerializeJson(textmsg));
                        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);

                        return true;
                    }
                }
            }
            catch(Exception ex)  
            {
                return false;
            }
            return false;
            
        }
        public static bool IsConnected(TcpClient client)
        {
            if (client == null || !client.Connected)
                return false;

            try
            {
                return !(client.Client.Poll(1, SelectMode.SelectRead) && client.Client.Available == 0);
            }
            catch (SocketException)
            {
                return false;
            }
        }
        
    }
}
