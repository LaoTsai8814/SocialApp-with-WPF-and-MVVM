using ChatApp_WPF.MainTabs;
using ChatServer.ShareLib;
using SocialApp.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SocialApp.Command;
using ChatApp_WPF.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;
using ChatApp_WPF.View;

namespace ChatApp_WPF.Viewmodel.TabVM
{
    internal class ChatTabVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        ClientNetwork clientNetwork = new ClientNetwork();
        public ObservableCollection<UIMessageStyle> ChatHistory { get; set; } = new ObservableCollection<UIMessageStyle>();

        private string _message;

        public string Message
        {
            get { return _message; }
            set 
            {
                _message = value;
                OnPropertyChanged();
            }
        }
        public ICommand SendMessageCommand { get; set; }
        public ICommand Quit {  get; set; }
        public ChatTabVM() 
        {
            SendMessageCommand = new RelayCommand(SendMessage);
            Quit = new RelayCommand(QuitCommand);
            ClientNetwork.MessageReceived -= OnClientNetworkMessageReceived;
            ClientNetwork.MessageReceived += OnClientNetworkMessageReceived;
        }

        private void QuitCommand(object obj)
        {
            Command command = new Command
            {
                cmd = "quit"
            };
            clientNetwork.SendMessageAsync(JsonHandle.SerializeJson(command));

            //throw new NotImplementedException();
        }

        private async void SendMessage(object obj)
        {

            TextMessage message = new TextMessage
            {
                cmd = "chatmessage",
                Textmsg = Message

            };
            
            await clientNetwork.SendMessageAsync(JsonHandle.SerializeJson(message));
            
            //throw new NotImplementedException();
        }
        private void OnClientNetworkMessageReceived(string obj)
        {
           
            switch (JsonHandle.DeserializeJson<Command>(obj).cmd)
            {
                case "sendmsg":
                    RequestCondition condition = JsonHandle.DeserializeJson<RequestCondition>(obj);
                    if (condition.Success)
                    {
                        UIMessageStyle messageStyle = new UIMessageStyle
                        {
                            Name = UserData.GetUserName(),
                            Message = Message,
                            IsMine = true

                        };
                        ChatHistory.Add(messageStyle);
                        Message = "";
                    }
                    else
                    {
                        Trace.WriteLine("Send Fail");
                        
                    }
                    break;
                case "sendto":
                    ReceiveTextMessage textMessage = JsonHandle.DeserializeJson<ReceiveTextMessage>(obj);
                    UIMessageStyle receivemessageStyle = new UIMessageStyle
                    {
                        Name = textMessage.name,
                        Message = textMessage.Textmsg,
                        IsMine = false
                    };
                    ChatHistory.Add(receivemessageStyle);
                    break;
                case "quit":
                    RequestCondition condition1 = JsonHandle.DeserializeJson<RequestCondition>(obj);
                    if (condition1.Success == true)
                    {
                        MainWindow.vm.SetCurrentView<HomeView>();
                        ClientNetwork.MessageReceived -= OnClientNetworkMessageReceived;
                    }
                    break;

            }
            
        }
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class BoolToColumnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? 1 : 0; // 自己的消息放右侧（Grid.Column=1），对方消息放左侧（Grid.Column=0）
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
    public class MessageAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
    public class MessageBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Brushes.LightBlue : Brushes.LightGray; // 自己的消息是蓝色
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }


}
