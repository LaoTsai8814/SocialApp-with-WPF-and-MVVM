using ChatApp_WPF.Viewmodel.TabVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatApp_WPF.MainTabs
{
    /// <summary>
    /// ChatTab.xaml 的互動邏輯
    /// </summary>
    public partial class ChatTab : UserControl
    {
        ChatTabVM ChatTabVM = new ChatTabVM();
        public ChatTab()
        {
            InitializeComponent();
            DataContext = ChatTabVM;
        }
    }
}
