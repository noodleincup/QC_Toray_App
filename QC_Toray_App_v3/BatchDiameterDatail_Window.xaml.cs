using QC_Toray_App_v3.UserControl;
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
using System.Windows.Shapes;

namespace QC_Toray_App_v3
{
    /// <summary>
    /// Interaction logic for BatchDiameterDatail_Window.xaml
    /// </summary>
    /// 

    public partial class BatchDiameterDatail_Window : Window
    {
        public string ResultString { get; private set; }
        private const int BATCH_ROW_COUNT = 30;

        // Private fields to hold the initial data
        private readonly string _initialGrade;
        private readonly string _initialLot;
        private readonly string _initialBtachNo;

        // Parameterized constructor to receive the three strings
        public BatchDiameterDatail_Window(string grade, string lot, string batchNo)
        {
            InitializeComponent();

            // Store the data
            _initialGrade = grade;
            _initialLot = lot;
            _initialBtachNo = batchNo;

            // Display the data in the TextBlocks
            txbGrade.Text = _initialGrade;
            txbLot.Text = _initialLot;
            txbBatchNumber.Text = _initialBtachNo;

            InitializeBatchDiameterTable();
        }

        public void InitializeBatchDiameterTable()
        {


            // *** NEW: Use the Grid and create RowDefinitions ***
            // 1. Clear any existing content and row definitions
            grdDiameterTable.Children.Clear();
            grdDiameterTable.RowDefinitions.Clear();

            // 2. Define the row count: 1 (Header) + BATCH_ROW_COUNT (30) + 1 (Average) + 1 (Judgement) = 33 rows (based on your code logic)
            // Header (1) + Loop (30) + Average (1) + Judgement (1) = 33 rows total.
            int totalRowCount = BATCH_ROW_COUNT + 3; // Assuming BATCH_ROW_COUNT is 30.

            // 3. Create a RowDefinition for each row and set the Height to '*' for proportional distribution.
            for (int i = 0; i < totalRowCount; i++)
            {
                // '*' is proportional sizing. It tells the Grid to give this row an equal share of the available vertical space.
                grdDiameterTable.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }

            int rowIndex = 0;

            // 4) Define and place the header row (Row 0)
            BatchCell_UserControl header = new BatchCell_UserControl();
            header.cols0.Text = "";
            header.cols1.Text = "A";
            header.cols2.Text = "B";
            header.cols3.Text = "L";

            header.cols1.IsReadOnly = true;
            header.cols2.IsReadOnly = true;
            header.cols3.IsReadOnly = true;

            header.cols1.IsEnabled = false;
            header.cols2.IsEnabled = false;
            header.cols3.IsEnabled = false;

            header.cols1.FontSize = 10;
            header.cols2.FontSize = 10;
            header.cols3.FontSize = 10;
            Grid.SetRow(header, rowIndex++);
            grdDiameterTable.Children.Add(header);

            // 5) Create 30 batch rows (Row 1 to Row 30)
            for (int i = 1; i <= BATCH_ROW_COUNT; i++)
            {
                BatchCell_UserControl batchRow = new BatchCell_UserControl();
                batchRow.cols0.Text = i.ToString(); // Set the row number
                Grid.SetRow(batchRow, rowIndex++);
                grdDiameterTable.Children.Add(batchRow);
            }

            // 6) Add the average row (Row 31)
            BatchCell_UserControl averageRow = new BatchCell_UserControl();
            averageRow.cols0.Text = "Average";
            Grid.SetRow(averageRow, rowIndex++);
            grdDiameterTable.Children.Add(averageRow);

            // 7) Add the judgement row (Row 32)
            BatchCell_UserControl judgementRow = new BatchCell_UserControl();
            judgementRow.cols0.Text = "Judgement";
            Grid.SetRow(judgementRow, rowIndex++);
            grdDiameterTable.Children.Add(judgementRow);

            //// 8) Add buttons at the bottom
            //StackPanel buttonPanel = new StackPanel
            //{
            //    Orientation = Orientation.Horizontal,
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    Margin = new Thickness(0, 0, 0, 0)
            //};
            //Button btnOK = new Button
            //{
            //    Content = "OK",
            //    Width = 80,
            //    Margin = new Thickness(10, 0, 10, 0)
            //};
            //btnOK.Click += btnOK_Clicked;
            //Button btnCancel = new Button
            //{
            //    Content = "Cancel",
            //    Width = 80,
            //    Margin = new Thickness(10, 0, 10, 0)
            //};
            //btnCancel.Click += btnCancel_Clicked;


            //buttonPanel.Children.Add(btnOK);
            //buttonPanel.Children.Add(btnCancel);

            //Grid.SetRow(buttonPanel, rowIndex++);
            //grdDiameterTable.Children.Add(buttonPanel);
        }

        private void btnSaveDiameterData_Clicked(object sender, RoutedEventArgs e)
        {
            // Collect data from the table and form the ResultString
            StringBuilder resultBuilder = new StringBuilder();
            foreach (UIElement element in grdDiameterTable.Children)
            {
                if (element is BatchCell_UserControl batchCell)
                {
                    // Skip header and buttons
                    if (batchCell.cols0.Text == "" || batchCell.cols0.Text == "Average" || batchCell.cols0.Text == "Judgement")
                        continue;
                    resultBuilder.AppendLine($"{batchCell.cols0.Text},{batchCell.cols1.Text},{batchCell.cols2.Text},{batchCell.cols3.Text}");
                }
            }
            ResultString = resultBuilder.ToString();
            this.DialogResult = true; // Indicate success
            this.Close();
        }

        private void btnCancelDiameterData_Clicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // Indicate cancellation
            this.Close();
        }
    }
}
