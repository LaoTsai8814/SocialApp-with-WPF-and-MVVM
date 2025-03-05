using ChatApp_WPF.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp_WPF.Viewmodel
{
    public class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private object _currentview;

        public object CurrentView
        {
            get { return _currentview; }
            set {
                
                _currentview = value;
                OnPropertyChanged();
            }
        }

        public static List<object> ExistingTab;

        public MainVM()
        {
            CurrentView = new LoginView();
        }
        public void SetCurrentView<T>() where T : new()
        {
            CurrentView = new T();
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
