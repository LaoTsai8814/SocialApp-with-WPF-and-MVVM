using ChatApp_WPF.Models;
using ChatServer.ShareLib;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ChatApp_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected async override void OnExit(ExitEventArgs e)
        {
            ClientNetwork client = new ClientNetwork();
            
            await client.SendMessageAsync(JsonHandle.SerializeJson(new Command { cmd = "quitapp" }));

            

            base.OnExit(e);
        }
        
    }

}
