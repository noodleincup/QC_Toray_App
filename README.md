# QC_Toray_App — Project Documentation


## Table of Contents
- Project overview
- Solution layout
- Module-by-module documentation (template)


---

## Project overview
For this project be a middleware for user can interface with camera software throught this application and handling database SQL server

---

## Solution layout
There are 3 namespaces
- QC_Toray_App_v3
  - /image
  - /library
    - ConsoleHelper.cs
    - HandleDatabase.dll
    - System.Data.SqlClient.dll
    - TcpClientService.cs
    - TCPClientViewModel.cs
    - TcpConfig.cs
  - /UserConrtrol
    - BatchCell_UserControl.xaml
    - LoginUserControl.xaml
    - LotOverviewUserControl.xaml
    - MainTable_UserControl.xaml
    - ManagePattern_UserControl.xaml
    - Master_UserControl.xaml
    - OperationUserControl.xaml
    - Report1_UserControl.xaml
    - Report2_UserControl.xaml
    - SetPattern_UserControl.xaml
    - SetSampleType_UserControl.xaml
    - UniformGrid_Master_UserControl
  - /Windows
    - AddLot_Window.xaml
    - BatchDiameterDatail_Window.xaml
    - MasterSampleLimit_Window.xaml
  - AppVar.cs
  - Batch_Class.cs
  - MainWindow.xaml

---

## Module-by-file documentation (template)

Module MainWindow
  - Path: `QC_Toray_App_v3/MainWindow.cs` 
  - Purpose: Main Window
  - Public API (classes/functions)
    - Class MainWindow
      - Properties:
        - string LotData
        - string GradeData
        - string OrderNo
        - string BatchNum
      - Private method 
        - ListViewMenu_SelectionChanged
          - Summary: Change page
        - OpenConsoleWindows
          - Summary: Open console for debugging
        - OnUpdateLotAndGradeData
          - Summary: Update all properties function
        - OnChangeUserName
          - Summary: Update username function

---        
### Library

Module: ConsoleHelper
- Path: `QC_Toray_App_v3/library/ConsoleHelper.cs`
- Purpose: Display Console for debugging is used MainWindow.cs
- Public API (classes/functions)
  - Class: ConsoleHelper
    - Summary: Display console for debugging  

Module: HandleDatabase
- Path: `QC_Toray_App_v3/library/HandleDatabase.dll`
- Purpose: Handling data in app. with database table
- Public API (class/functions)
  - Class: DatabaseHandler
    - Summary: Instance for database handling 
    - Constructiors:
      - DatabaseHandler(string connectionString) -> use connectionString from SQL server
    - Public methods:
      - GetTableDatabaseAsDataTable(string tableName)
        - Summary: `select * from tableName`
        - Parameters: DataTable dt
        - Returns: DataTable dt
      - GetTableDatabaseAsDataTableWithCondition(string tableName, string condition)
        - Summary: `select * from tableName where condition`
        - Parameters: DataTable dt
        - Returns: DataTable dt
      - UpdateDataInTableById(Dictionary<string, object> updateValues, string tableName, string idColumnName, int idValue)
        - Summary: `update set updateValues.Key = updateValues.Value from tableName where idColumnName = idValue`
        - Parameters Dictionary<string, object> updateValues {string columnName: value}
        - Returns: true if success
      - ExecuteStoredProcedure(string procedureName, Dictionary<string, object> parameters, bool outputExisted = false, string outputParamName = null)
        - Summary: execute procedure
        - Parameters: Dictionary<string, object> parameters {string parameterName, value}
        - return DataSet of procedure output

Module: DatabaseConfig
  - Path: `QC_Toray_App_v3/library/DatabaseConfig.cs`
  - Purpose: Collect Table name, ConnectionString and Stored Procedure Name
  -  Public API (class/functions)
     - Class DatabaseConfig
        - Summary: Instance for DatabaseConfig
        - Constructiors:
          - None
        - Public methods: Returns string value from Setting.setting
          - ConnectionString1
          - ConnectionString2
          - LotOverviewTableName
          - SampleGroupTableName
          - MasterSampleLimitTableName
          - MasterSampleItemTableName
          - MasterPatternTableName
          - MasterGradePatternTableName
          - MasterDiameterTableName 
          - AddLotStoredProcedure
          - UpdateOrInsertMasterSampleItemProcedure
          - InserOrUpdateMasterPatternProcedure
          - TimeoutMs {Returns int}
      
Module: TCPClientViewModel
  - Path: `QC_Toray_App_v3/library/TCPClientViewModel.cs`
  - Purpose : Be a model to handle tcp communication
  - Public API (class/functions)
    - Class TCPClientViewModel
      - Public methods:
        - ConnectToServerAsync()
          - Summary: Connect to TCP server
        - DisconnectFromServerAsync()
          - Summary: Disconnect from TCP server
        - SendDataAsync(string data)
          - Summary: `Send data to TCP Server`
      - Public events:
        - Action<string> MessageReceived
          - Summary: `for add event triger when message receive`
        - Action<bool> ConnectionStatusChanged
          - Summary: `for add event triger when connection status change`
Module: TcpConfig
  - Path: `QC_Toray_App_v3/library/TCPClientViewModel.cs` 
  - Purpose: Collect TCP config that get from setting.setting
  - Public API (class/functions)
    - Class TcpConfig
      - Public methods:
        - string TcpServerIp
        - int TcpServerPort

Module LoginUserControl
  - Path: `QC_Toray_App_v3/UserControl/LoginUserControl.xaml`
  - Purpose: keep function for Login Page
  - Public API (class/function)
    - Class LoginUserControl
      - Public method:
        - Login_Button_Clicked 
          - Summary: Click to trigger LoginProcess event
        - Enter_Pressed
          - Summary: Press Enter trigger LoginProcess event
        - LoginProcess(string username, string password)
          - Summary: function when LoginProcess event happened for verify username, pasword and change page and username
        - VerifyLogin(string user, string pass)
          - Summary: verify username, pasword and change page and username function
          - Returns: login result 
        - ChangePageRequested
          - Summary: Trigger this function to change page
        - ChangeUserName
          - Summary: Trigger this function to change current username
---
### Window
Module AddLot_Window
  - Path: `QC_Toray_App_v3/Windows/AddLot_Window.xaml`
  - Purpose: use when disire to add sample type in Set Sample Group Page
  - Public API (class/function)
    - Class AddLot_Window
      - Properties:
        - Result -> string
        - Message -> string
      - Private method:
        - btnAddLot_Click
          - Summary: excecute procedure and send bool result to Set Sample Group Page

Module BatchDiameterDatail_Window
  - Path: `QC_Toray_App_v3/Windows/BatchDiameterDatail_Window.xaml`
  - Purpose: be pop up in lot overview page to measure diameter AB and L
  - Public API (class/function)
    - Class BatchDiameterDatail_Window
      - Properties:
        - Result -> string
        - Message -> string
      - Private method:
        - BatchDiameterDetail_Winodws_Loaded
          - Summary: load tcp event to viewTcpModel
        - UpdateBatchDiameterTable
          - Summary: a function to call message processing function and load to UI 
        - btnSaveDiameterData_Clicked
          - Summary: Save data to somewhere?
        - btnTcpStatus_Clicked
          - Summary: for toggling connection tcp status
        - btnMeasureL_Clicked
          - Summary: Click to trigger measure L diameter
        - btnMeasureAB_Clicked
          - Summary: Click to trigger measure Ab diameter

Module MasterSampleLimit_Window.xaml
  - Path: `QC_Toray_App_v3/Windows/MasterSampleLimit_Window.xaml.xaml`
  - Purpose: be pop up in set sample type page for adding and updating master sample limit
  - Public API (class/function)
    - Class MasterSampleLimit_Window.xaml
      - Properties:
        - Result -> string
        - Message -> string
      - Private method:
        - MasterSampleLimit_Window_Loaded
          - Summary: call LoadDataGrid function when application loaded
        - LoadDataGrid
          - Summary: a function to call function that getting data from DB and load to data grid 
        - LoadRowDataToItemList
          - Summary: Load data to ItemList that be ItemSource to data grid
        - LoadSampleItemName
          - Summary: Update item name to list
        - LoadMasterSampleLimitFromDatabase
          - Summary: Get master sample limit from tb_master_sample_limit
        - btnSave_Click
          - Summary: Click to trigger to save data in data grid to tb_master_sample_limit
        - SaveAllRowToDatabase
          - Summary: Update or insert data grid for all row with loop
        - SaveEachRowDataToDatabase
          - Summary: Update or insert data grid each row by usp_UpdateOrInsertMasterSampleLimit 
        - GetParameterInputDictionary -> Dictionary<string, object>
          - Summary: for preparing parameter to procedure
          - Returns: itemDataDict
    - Class ItemData
      - Summary: for be element to binding per row
      - Properties:
        - Items -> string
        - MaxOKQty -> string
        - RangeMin -> string
        - RangeMax -> string
        - SampleId -> int


---
### UserControl
Module MainTable_UserControl
  - Path: `QC_Toray_App_v3/UserControl/MainTable_UserControl.xaml`
  - Purpose: keep function for MainTable Page
  - Public API (class/function)
    - Class MainTable_UserControl
      - Public method:
        - tableGrid_SelectionCellChanged
          - Summary: Click cell to trigger change page and grade, lot data
        - LoadDataFromDatabase
          - Summary: Get data from uv_lot_onprocess and load to data grid 
        - ConfigureDataGrid()
          - Summary: Set data grid configuration
        - btnAddLot_Click
          - Summary: Click button to trigger this event to add new data to database and renew load data grid 
        - FilterTableBySearch(string query)
          - Summary: function for filtering data in data grid
        - txbSearch_TextChanged
          - Summary: event for filtering data in data grid
        - ChangePageRequested
          - Summary: Trigger this function to change page
        - UpdateLotAndGradeData
          - Summary: Trigger this function to change lot and grade data

Module ManagePattern_UserControl
  - Path: `QC_Toray_App_v3/UserControl/ManagePattern_UserControl.xaml`
  - Purpose: keep function for ManagePattern Page
  - Public API (class/function)
    - Class ManagePattern_UserControl
      - Public method:
        - ManagePattern_UserControl_Loaded
          - Summary: event for getting data from tb_master_pattern and load to data grid
        - LoadDataToDataGrid()
          - Summary: function to get data from tb_master_pattern and load to data grid 
        - AddPatternDataToList
          - Summary: function to load DataTable to datagrid
        - btnAdd_Click
          - Summary: Click button to trigger this event to add new data data grid 
        - btnSave_Click
          - Summary: Click button to trigger this event to Insert and update database by procedure
        - txbSearch_TextChanged
          - Summary: event for filtering data in data grid
        - ChangePageRequested
          - Summary: Trigger this function to change page
    - Class PatternItem
      - Summary: class for binding data to data grid 
      - Properties:
        - No -> int 
        - Name -> string
        - Description -> string

Module SetPattern_UserControl
  - Path: `QC_Toray_App_v3/UserControl/SetPattern_UserControl.xaml`
  - Purpose: keep function for SetPattern Page
  - Public API (class/function)
    - Class SetPattern_UserControl
      - Public method:
        - UserControl_Loaded
          - Summary: event for getting data from tb_master_grade_pattern and load to data grid
        - LoadGradeItemsFromDatabaseAsync()
          - Summary: event to get data from tb_master_grade_pattern and load to data grid 
        - GetDataTableFromDatabaseAsync
          - Summary: function get data from tb_master_pattern make convert to dictionary
        - ProcessAndBindData
          - Summary: Binding data to GradeItems that be ItemSource to data grid 
        - PatternComboBox_SelectionChanged
          - Summary: trigger this event when combo box selectiong change selected in GradeItem and update database
        - UpdatePatternIdToDatabaseAsync
          - Summary: update database function at tb_master_grade_pattern
        
    - Class GradeItem
      - Summary: class for binding data to data grid 
      - Properties:
        - No -> int 
        - Grade -> string 
        - GradeCode -> string 
        - UpdateDate -> DataTime 
        - UpdateBy -> string 
        - Pattern -> string 
        - Description -> string 

Module SetSampleType_UserControl
  - Path: `QC_Toray_App_v3/UserControl/SetSampleType_UserControl.xaml`
  - Purpose: keep function for SetSampleType Page
  - Public API (class/function)
    - Class SetSampleType_UserControl
      - Public method:
        - SetSampleType_UserControl_Loaded
          - Summary: event for getting data from tb_master_grade_pattern and load to data grid
        - LoadSampleType()
          - Summary: function to get data from tb_master_grade_pattern and load to data grid 
        - btnAdd_Click
          - Summary: Click button to trigger pop up MasterSampleLimit window 
        

Module OperationUserControl
  - Path: `QC_Toray_App_v3/UserControl/OperationUserControl.xaml`
  - Purpose: keep function for Operation Pop up
  - Public API (class/function)
    - Class OperationUserControl
      - Public method:
        - SetSampleType_UserControl_Loaded
          - Summary: event for getting data from tb_master_grade_pattern and load to data grid
        - LoadSampleType()
          - Summary: function to get data from tb_master_grade_pattern and load to data grid 
        - AddPatternDataToList
          - Summary: function to load DataTable to datagrid
        - btnAdd_Click
          - Summary: Click button to trigger this event to add new data data grid 
        - btnSave_Click
          - Summary: Click button to trigger this event to Insert and update database by procedure
        - txbSearch_TextChanged
          - Summary: event for filtering data in data grid
        - ChangePageRequested
          - Summary: Trigger this function to change page
 
Module LotOverviewUserControl
  - Path: `QC_Toray_App_v3/UserControl/LotOverviewUserControl.xaml`
  - Purpose: keep function for LotOverview Page
  - Public API (class/function)
    - Class LotOverviewUserControl
      - Constructors:
        - string orderNo
        - string lotData
        - string gradeData
      - Public method:
        - LotOverviewUserControl_Loaded
          - Summary: event for call get database and binding data 
        - txbBatchNumber_MouseDoubleClick
          - Summary: event for pop out BatchDiameterDatail_Window for measurement
        - btnInitial_Click
          - Summary: before be next camera opeation need to click initialize button 
        - txbBatchStart_KeyDown
          - Summary: press enter at textbox to get start batch number 
        - txbSearch_TextChanged
          - Summary: event for filtering data in data grid
        - ToggleIsEnableBatchItem
          - Summary: Toggle enable some UI
    - Class LotOverviewViewModel
      - Summary: class for binding data to data grid 
      - Properties:
        [for combo box]
        - SampleGroups -> SampleGroup 
        - SelectedSampleGroup -> SampleGroup  
        - Patterns -> Pattern 
        - SelectedPattern -> Pattern 
        - ItemDiameters -> ItemDiameter 
        - SelectedItemDiameter -> ItemDiameter 
      - Public method:
        - LoadSampleGroupAsync
          - Summary: event to get database table and binding to combo box
        - LoadPatternAsync
          - Summary: event to get database table and binding to combo box
        - LoadItemDiameter
          - Summary: event to get database table and binding to combo box

Module OperationUserControl
  - Path: `QC_Toray_App_v3/UserControl/OperationUserControl.xaml`
  - Purpose: keep function for Operation Page
  - Public API (class/function)
    - Class OperationUserControl
      - Constructors:
        - orderNo -> string
        - lotData -> string
        - gradeData -> string
      - Public method:
        - OperationUserControl_Loaded
          - Summary: event for binding function event to TCP viewModel 
        - OnMessageReceivedFromServer
          - Summary: event for hamdling whem messgae recieve from tcp server
        - InitialSampleImageButtonLoading
          - Summary: Add button to display image 
        - Window_PreviewKeyDown
          - Summary: press spacebar to move to next sample process 
        - SummaryDefecAllType
          - Summary: when process move to summary it will automically sum data from all sample
        

---
