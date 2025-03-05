using ChatServer.CommandHandle;
using ChatServer.UserInfo;

namespace ChatServer
{
    internal class Program
    {
        async static Task Main(string[] args)
        {
            Network network = new Network();
            _ = ServerUserData.Pairing();

            
            await Task.Delay(-1);
        }
    }
}
