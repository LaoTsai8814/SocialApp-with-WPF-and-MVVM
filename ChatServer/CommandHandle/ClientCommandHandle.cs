using ChatServer.ShareLib;
using ChatServer.UserInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.CommandHandle
{
    internal class ClientCommandHandle
    {
        public event Action<string> ClientActionHandle;  // 事件：收到消息
        public ClientCommandHandle()
        {

        }
        public void HandleClientCommand(TcpClient client,string jsonMessage)
        {
            //
            SwitchCase(client,jsonMessage);
        }
        private async void SwitchCase(TcpClient client, string jsonMessage)
        {
            Command command = JsonHandle.DeserializeJson<Command>(jsonMessage);
            //Console.WriteLine(jsonMessage);
            switch (command.cmd)
            {
                case "login":
                    People annomous = JsonHandle.DeserializeJson<People>(jsonMessage);
                    #region //Check If User Exist
                    if (ServerUserData.AddUser(client,annomous))
                    {
                        ClientActionHandle?.Invoke(JsonHandle.SerializeJson(new RequestCondition { cmd = "login", Success = true }));
                    }
                    else
                    {
                        ClientActionHandle?.Invoke(JsonHandle.SerializeJson(new RequestCondition { cmd = "login", Success = false }));
                    }
                    #endregion
                    break;
                case "pair":
                    //Console.WriteLine("receive pair");
                    bool complete = await ServerUserData.PairUser(client);
                    if (complete)
                    {
                        Console.WriteLine("Pair Completed");
                        ClientActionHandle?.Invoke(JsonHandle.SerializeJson(new RequestCondition { cmd = "pair", Success = true }));
                    }
                    else
                    {
                        ClientActionHandle?.Invoke(JsonHandle.SerializeJson(new RequestCondition { cmd = "pair", Success = false }));
                    }
                    break ;
                case "chatmessage":
                    if(await ServerUserData.SendDataSingleMode(client, JsonHandle.DeserializeJson<TextMessage>(jsonMessage)))
                    {
                        Console.WriteLine("SendMessage Completed");
                        ClientActionHandle?.Invoke(JsonHandle.SerializeJson(new RequestCondition { cmd = "sendmsg", Success = true }));
                    }
                    else
                    {
                        ClientActionHandle?.Invoke(JsonHandle.SerializeJson(new RequestCondition { cmd = "sendmsg", Success = false }));
                    }
                    break;
                case "quit":

                    if (ServerUserData.RemoveUser(client))
                    {
                        ClientActionHandle?.Invoke(JsonHandle.SerializeJson(new RequestCondition { cmd = "quit", Success = true }));
                    }
                    else
                    {
                        ClientActionHandle?.Invoke(JsonHandle.SerializeJson(new RequestCondition { cmd = "quit", Success = false }));
                    }

                    break;
                case "quitapp":

                    if (ServerUserData.RemoveUserFully(client))
                    {
                        ClientActionHandle?.Invoke(JsonHandle.SerializeJson(new RequestCondition { cmd = "quit", Success = true }));
                    }
                    else
                    {
                        ClientActionHandle?.Invoke(JsonHandle.SerializeJson(new RequestCondition { cmd = "quit", Success = false }));
                    }

                    break;

            }
        }
    }
}
