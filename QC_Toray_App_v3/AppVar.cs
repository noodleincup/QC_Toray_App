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
