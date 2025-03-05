using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace ChatServer.ShareLib
{
    public class ShareLib
    {
    }
    [Serializable]
    public class Command 
    {
        public string cmd;
    }

    [Serializable]
    public class TextMessage:Command
    {
        public string Textmsg;
    }
    [Serializable]
    public class  ReceiveTextMessage:TextMessage
    {
        public string name;
    }
    public class  UIMessageStyle
    {
        public string Name { get; set; }
        

        public string Message { get; set; }

        public bool IsMine { get; set; }
    }
    
    [Serializable]
    public class User
    {
        public string username;
        public string password;
    }
    [Serializable]
    public class Annomous:Command
    {
        public string username;
    }
    [Serializable]
    public class  RequestCondition:Command
    {
        public bool Success;
    }
    public class JsonHandle
    {
        public static string SerializeJson(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
        public static T DeserializeJson<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
    }
    public class UserInfomation
    {
        public string name;
        public int level;
    }


    [Serializable]
    public class People: Command
    {
        public string Name { get; set; }

        public string Gender { get; set; }

        public DateTime Time { get; set; }
    }
    [Serializable]
    public class SendPairRequest : People
    {

        //0: Multi , 1: Single
        public int Pairmode;

    }
    [Serializable]
    public class PairedUser
    {
        public TcpClient male { get; set; }
        public TcpClient female { get; set; }
    }
 }