using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QC_Toray_App_v3
{
    public class GlobalState : INotifyPropertyChanged
    {
        private static GlobalState _instance;
        public static GlobalState Instance => _instance ??= new GlobalState();

        private bool _isFeatureEnabled = false;
        public bool IsFeatureEnabled
        {
            get { return _isFeatureEnabled; }
            set
            {
                if (_isFeatureEnabled != value)
                {
                    _isFeatureEnabled = value;
                    OnPropertyChanged(nameof(IsFeatureEnabled));
                    OnPropertyChanged(nameof(NegativeIsFeatureEnabled));
                }
            }
        }
        public bool NegativeIsFeatureEnabled { get { return !_isFeatureEnabled; } }


        private string _serverIP = "192.168.0.123";
        public string ServerIP
        {
            get { return _serverIP; }
            set
            {
                if (_serverIP != value)
                {
                    _serverIP = value;
                    OnPropertyChanged(nameof(ServerIP));
                }
            }
        }

        private int _serverPort = 7930;
        public int ServerPort
        {
            get { return _serverPort; }
            set
            {
                if (_serverPort != value)
                {
                    _serverPort = value;
                    OnPropertyChanged(nameof(ServerPort));
                }
            }
        }

        private string _userName = "admin";
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BooleanNegationConverter : IValueConverter
    {
        // ✅ Static singleton instance for easy use in XAML
        public static readonly BooleanNegationConverter Instance = new BooleanNegationConverter();
        private BooleanNegationConverter() { }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return false;
        }
    }
}
