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
    /// Interaction logic for LotOverviewUserControl.xaml
    /// </summary>
    public partial class LotOverviewUserControl : UserControl
    {
        public event EventHandler<string> ChangePageRequested;
        private const int defaultBatchNum = 20; // Default number of BatchDetailItem controls

        StackPanel _buttonPanel;
        public LotOverviewUserControl()
        {
            InitializeComponent();
            initializeBatchDetail();
        }

        public void initializeBatchDetail()
        {

            // 0. Create default BatchDetailItem controls
            for (int i = 0; i < defaultBatchNum; i++)
            {
                BatchDetailItem item = new BatchDetailItem();
                item.ItemValue = (i + 1).ToString();
                wrpBatchDetail.Children.Add(item);
            }

            // 1. Create the StackPanel (the container)
            _buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 10, 0, 0), // Optional: add some top margin
                Width = 300 // Optional: match the width of the UserControls
            };

            // 2. Create the 'Add' Button
            Button addButton = new Button
            {
                Content = "Add",
                Width = 100,
                Height = 30,
                Margin = new Thickness(20,5,5,5)
            };
            // Attach click event handler
            addButton.Click += AddDetail_Click;

            // 3. Create the 'Remove' Button
            Button removeButton = new Button
            {
                Content = "Remove",
                Width = 100,
                Height = 30,
                Margin = new Thickness(20, 5, 5, 5),
                Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC864C8"),
                BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC864C8")
            };
            // Attach click event handler
            removeButton.Click += RemoveDetail_Click;

            // 4. Add the buttons to the StackPanel
            _buttonPanel.Children.Add(addButton);
            _buttonPanel.Children.Add(removeButton);

            // 5. Add the StackPanel to the WrapPanel as the last child
            // This ensures the button group is always at the end, even 
            // if the WrapPanel wraps to a new line.
            wrpBatchDetail.Children.Add(_buttonPanel);
        }

        #region Button Click Handlers
        private void AddDetail_Click(object sender, RoutedEventArgs e)
        {
            // 1. Temporarily remove the button panel
            wrpBatchDetail.Children.Remove(_buttonPanel);

            // 2. Create and add the new UserControl
            BatchDetailItem newItem = new BatchDetailItem();
            newItem.ItemValue = (wrpBatchDetail.Children.Count + 1).ToString();
            wrpBatchDetail.Children.Add(newItem);

            // 3. Re-add the button panel to ensure it is the last child
            wrpBatchDetail.Children.Add(_buttonPanel);
        }

        private void RemoveDetail_Click(object sender, RoutedEventArgs e)
        {
            // 1. Temporarily remove the button panel
            wrpBatchDetail.Children.Remove(_buttonPanel);

            // 2. Find and remove the last BatchDetailItem (assuming it's not the button panel itself)
            // We iterate backwards to easily find the last *actual* item
            for (int i = wrpBatchDetail.Children.Count - 1; i >= 0; i--)
            {
                if (wrpBatchDetail.Children[i] is BatchDetailItem)
                {
                    wrpBatchDetail.Children.RemoveAt(i);
                    break; // Stop after removing one item
                }
            }

            // 3. Re-add the button panel to ensure it is the last child
            // This is safe even if no BatchDetailItem was found and removed.
            wrpBatchDetail.Children.Add(_buttonPanel);
        }

        private void btnRecord_Clicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Record button clicked!");
        }
        private void btnSendData_Clicked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Send Data button clicked!");
        }
        #endregion
    }
}
