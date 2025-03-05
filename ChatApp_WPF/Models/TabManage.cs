using ChatApp_WPF.MainTabs;
using ChatApp_WPF.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ChatApp_WPF.Models
{
    internal class TabManage
    {
        
        private static readonly Dictionary<string, UserControl> _tabMappings = new();
        public static object ChangeTabRequest(object obj)
        {
            if (obj is string tab)
            {
                if (!_tabMappings.ContainsKey(tab))
                {
                    _tabMappings[tab] = tab switch
                    {
                        "Home" => new HomeTab(),
                        "Pair" => new PairingTab(),
                        //"Messages" => new MessagesTab(),
                        "Profile" => new ProfileTab(),
                        //"Settings" => new SettingsView(),
                        //"Help" => new HelpView(),
                        //"About" => new AboutView(),
                        //"Logout" => new LogoutView(),
                        _ => null
                    };
                }

                return _tabMappings[tab];
            }
            return null;
        }

        public static List<object> ExistingTab;

        
    }
}
