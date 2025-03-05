using ChatApp_WPF.MainTabs;
using ChatApp_WPF.Models;
using ChatApp_WPF.View;
using SocialApp.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChatApp_WPF.Viewmodel
{
    public class HomeVM: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>
        /// Handle the tab to not create the same tab again
        /// </summary>
        public static Dictionary<string, object> ExistTabs = new Dictionary<string, object>();
        /// <summary>
        /// current tab
        /// The current tab that is displayed on right side of your screen
        /// </summary>
        private static object _currenttab;
        public object CurrentTab
        {
            get { return _currenttab; }
            set 
            {
                _currenttab = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// UI command that requests the tab change
        /// </summary>
        public ICommand ChangeTab { get; set; }
        /// <summary>
        ///    HomeVM God damn constructor
        /// </summary>
        public HomeVM()
        {
            ChangeTab = new RelayCommand(ChangeTabRequest);
            OndefaultTab();
        }
        /// <summary>
        /// Default tab
        /// The First Tab you get after the login
        /// </summary>
        private void  OndefaultTab()
        {
            CurrentTab = TabManage.ChangeTabRequest("Home");
        }
        /// <summary>
        /// the method that changes the tab
        /// </summary>
        /// <param name="obj">
        /// parameter should be string type
        /// </param>
        public void ChangeTabRequest(object obj)
        {
            CurrentTab =  TabManage.ChangeTabRequest(obj);
        }
        /// <summary>
        /// --------------------------------
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}
