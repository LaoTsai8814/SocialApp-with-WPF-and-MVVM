using ChatApp_WPF.MainTabs;
using ChatApp_WPF.Models;
using ChatServer.ShareLib;
using SocialApp.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp_WPF.Viewmodel.TabVM
{
    internal class HomeTabVM: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand Pair { get; set; }

        public HomeTabVM()
        {
            Pair = new RelayCommand(PairRequest);
        }

        private void PairRequest(object obj)
        {
            
            if (obj is string PairType)
            {
                SendPairRequest(PairType);
            }
            
        }
        SendPairRequest SetUserData(string Pairtype)
        {
            SendPairRequest pairRequest = new SendPairRequest();
            pairRequest.Name = UserData.GetUserName();
            pairRequest.Gender = UserData.GetGender();
            pairRequest.Time = DateTime.Now;
            pairRequest.cmd = "pair";
            switch (Pairtype)
            {
                case "Multi":
                    pairRequest.Pairmode = 0;
                    break;
                case "Single":
                    pairRequest.Pairmode = 1;
                    break;
                default:
                    break;
            }
            return pairRequest;
        }
        public void SendPairRequest(string Pairtype)
        {
            try
            {
                ClientNetwork clientNetwork = new ClientNetwork();
                ClientNetwork.MessageReceived -= OnClientNetworkMessageReceived;
                ClientNetwork.MessageReceived += OnClientNetworkMessageReceived;
                _ = clientNetwork.SendMessageAsync(JsonHandle.SerializeJson(SetUserData(Pairtype)));
                
                
                MainWindow.vm.SetCurrentView<PairingTab>();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }

        }
        private void OnClientNetworkMessageReceived(string obj)
        {
            Trace.WriteLine("paired success");
            RequestCondition Condition = JsonHandle.DeserializeJson<RequestCondition>(obj);
            if (Condition.Success)
            {
                ClientNetwork.MessageReceived -= OnClientNetworkMessageReceived;
                MainWindow.vm.SetCurrentView<ChatTab>();
               
                Trace.WriteLine($"paired");
            }
            else
            {
                Trace.WriteLine("Pair Failed");
            }
            
            //throw new NotImplementedException();
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}
