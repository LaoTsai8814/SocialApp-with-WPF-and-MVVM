using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChatServer.CommandHandle
{
    internal class Network
    {
        public Network()
        {
            _ = StartListenConnection();


        }
        public async Task StartListenConnection()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            Console.WriteLine("Server Started，Waiting For Connection...");

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                Console.WriteLine("Client Connected！");
                _ = HandleClient(client);
            }
        }
        static async Task HandleClient(TcpClient client)
        {
            using (client)
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];

                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break; // 断开连接

                    string jsonMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    ClientCommandHandle clientCommandHandle = new ClientCommandHandle();
                    //Receive Message and Send Response
                    clientCommandHandle.ClientActionHandle += async (response) =>
                    {
                        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    };
                    clientCommandHandle.HandleClientCommand(client,jsonMessage);
                    //--------------------------------
                    //var message = JsonConvert.DeserializeObject<ChatMessage>(jsonMessage);
                    Console.WriteLine($"Receive Client Message: {jsonMessage}");
                }
            }
        }
        
    }
}
