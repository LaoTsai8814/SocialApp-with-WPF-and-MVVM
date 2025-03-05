using ChatApp_WPF.Models;
using ChatApp_WPF.View;
using ChatServer.ShareLib;
using SocialApp.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
namespace ChatApp_WPF.Viewmodel
{
    public class LoginVM:INotifyPropertyChanged
    {
        public ICommand LoginCommand { get; set; }

        public ObservableCollection<string> GenderSelect { get;set; }
        public event PropertyChangedEventHandler? PropertyChanged;

        //Binding UI UserName
        private string _username;
        public string UserName
		{
			get { return _username; }
			set {
				
				_username = value; 
				OnPropertyChanged();

            }
		}

        private string _gender;

        public string Gender
        {
            get { return _gender; }
            set {                 
                _gender = value;
                Trace.WriteLine(_gender);
                OnPropertyChanged();
            }
        }
        public LoginVM()
        {
            LoginCommand = new RelayCommand(LoginRequest);
            GenderSelect=new ObservableCollection<string> { "Male", "Female", "Helicopter" };
        }
        private async void LoginRequest(object obj)
        {
            if (UserName==null )
            {
                MessageBox.Show("Name Can't be empty");
                return;
            }
            if (Gender == null)
            {
                MessageBox.Show("Name Can't be empty");
                return;
            }
            People annomous = new People
            {
                cmd = "login",
                Name = UserName,
                Gender = Gender,
                Time = DateTime.Now,
            };
            ClientNetwork? clientNetwork = new ClientNetwork();
            await clientNetwork.ConnectAsync();

            ClientNetwork.MessageReceived += OnClientNetworkMessageReceived;
            await clientNetwork.SendMessageAsync(JsonHandle.SerializeJson(annomous));
        }

        private void OnClientNetworkMessageReceived(string obj)
        {
            bool Condition = JsonHandle.DeserializeJson<RequestCondition>(obj).Success;
            if(Condition)
            {
                UserData.SetUserName(UserName);
                UserData.SetGender(Gender);
                
                
                MainWindow.vm.SetCurrentView<HomeView>();
                ClientNetwork.MessageReceived -= OnClientNetworkMessageReceived;
            }
            else
            {
                Trace.WriteLine("Login Failed");
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
