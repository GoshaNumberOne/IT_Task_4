using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChessApp.ViewModels
{
    public class ParameterInputViewModel : INotifyPropertyChanged
    {
        private string _value;

        public string Name { get; set; }
        public Type ParameterType { get; set; }
        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}