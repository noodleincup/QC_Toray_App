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

namespace QC_Toray_App_v3
{
    /// <summary>
    /// Interaction logic for UniformGrid_Master_UserControl.xaml
    /// </summary>
    public partial class UniformGrid_Master_UserControl : UserControl
    {
        public UniformGrid_Master_UserControl()
        {
            InitializeComponent();
        }

        // Define a Dependency Property to receive data
        public static readonly DependencyProperty HeadLabelProperty =
            DependencyProperty.Register("HeadLabel", typeof(string), typeof(UniformGrid_Master_UserControl), new PropertyMetadata());

        public string HeadLabel
        {
            get { return (string)GetValue(HeadLabelProperty); }
            set { SetValue(HeadLabelProperty, value); }
        }

        // Define a Dependency Property to receive data
        public static readonly DependencyProperty SubLabelProperty =
            DependencyProperty.Register("SubLabel", typeof(string), typeof(UniformGrid_Master_UserControl), new PropertyMetadata("Default Label"));

        public string SubLabel
        {
            get { return (string)GetValue(SubLabelProperty); }
            set { SetValue(SubLabelProperty, value); }
        }


        public static readonly DependencyProperty InputValueProperty =
            DependencyProperty.Register("InputValue", typeof(string), typeof(UniformGrid_Master_UserControl), new PropertyMetadata(""));

        public string InputValue
        {
            get { return (string)GetValue(InputValueProperty); }
            set { SetValue(InputValueProperty, value); }
        }
    }
}
