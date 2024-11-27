using BaleReports.Views;
using FieldsColumnSelect.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using RepSetup.Properties;
using RTRep.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WLReports.Views;
using static RTRep.Services.ClsApplicationService;


namespace RepSetup.ViewModels
{
    public class SetupViewModel : BindableBase
    {
        protected readonly Prism.Events.IEventAggregator _eventAggregator;

        private ClsSQLhandler SqlConfigModel = ClsSQLhandler.Instance;


        public UserControl BaleSetupView
        {
            get { return new BaleSetupView(_eventAggregator); }
            set { }
        }

        public UserControl WLSetupView
        {
            get { return new WLSetupView(_eventAggregator); }
            set { }
        }


        #region SQL Transfer

        private bool _sQLXferValid;
        public bool SQLXferValid
        {
            get { return _sQLXferValid; }
            set 
            { 
                SetProperty(ref _sQLXferValid, value);
                ShowXfer = (value) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private bool _rtTransferEnable;
        public bool RtTransferEnable
        {
            get { return _rtTransferEnable; }
            set { SetProperty(ref _rtTransferEnable, value); }
        }


        private bool _wlTransferEnable;
        public bool WlTransferEnable
        {
            get { return _wlTransferEnable; }
            set { SetProperty(ref _wlTransferEnable, value); }
        }

        private string _xferStatus;
        public string XferStatus
        {
            get { return _xferStatus; }
            set { SetProperty(ref _xferStatus, value); }
        }

        private bool _sQLTransferOn;
        public bool SQLTransferOn
        {
            get { return _sQLTransferOn; }
            set
            {
                SetProperty(ref _sQLTransferOn, value);
                if (value)
                {
                    XferStatus = "ON";
                    XFerOpc = "1";
                }
                else
                {
                    XferStatus = "OFF";
                    XFerOpc = ".2";
                    RtTransferEnable = false;
                    WlTransferEnable = false;
                }
            }
        }
        private string _xFerOpc;
        public string XFerOpc
        {
            get { return _xFerOpc; }
            set { SetProperty(ref _xFerOpc, value); }
        }


        #endregion SQL Transfer



       


        private string _rTRepOpc;
        public string RTRepOpc
        {
            get { return _rTRepOpc; }
            set { SetProperty(ref _rTRepOpc, value); }
        }

        private string _wlTRepOpc;
        public string WlRepOpc
        {
            get { return _wlTRepOpc; }
            set { SetProperty(ref _wlTRepOpc, value); }
        }

        private bool _bModify = false;
        public bool BModify
        {
            get { return _bModify; }
            set { SetProperty(ref _bModify, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private string _LocalHost = Environment.MachineName;
        public string LocalHost
        {
            get { return _LocalHost; }
            set { SetProperty(ref _LocalHost, value); }
        }

        private Visibility _showXfer;
        public Visibility ShowXfer
        {
            get { return _showXfer; }
            set { SetProperty(ref _showXfer, value); }
        }


        private Visibility _wetLayerVisible;
        public Visibility WetLayerVisible
        {
            get { return _wetLayerVisible; }
            set { SetProperty(ref _wetLayerVisible, value); }
        }

        private Visibility showsql;
        public Visibility Showsql
        {
            get => showsql;
            set => SetProperty(ref showsql, value);
        }



        private string _host;
        public string Host
        {
            get { return _host; }
            set { SetProperty(ref _host, value); }
        }

        private string _instance;
        public string Instant
        {
            get { return _instance; }
            set { SetProperty(ref _instance, value); }
        }

        private string _userid;
        public string Userid
        {
            get { return _userid; }
            set {SetProperty(ref _userid, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private string _database;
        public string Database
        {
            get { return _database; }
            set { SetProperty(ref _database, value); }
        }

        private string _sqlStatus;
        public string SqlStatus
        {
            get { return _sqlStatus; }
            set { SetProperty(ref _sqlStatus, value); }
        }
        private List<string> _servercomboList;
        public List<string> ServercomboList
        {
            get { return _servercomboList; }
            set { SetProperty(ref _servercomboList, value); }
        }

        private int _selectedServerCombo;
        public int SelectedServerCombo
        {
            get
            {
                return _selectedServerCombo;
            }
            set
            {
                SetProperty(ref _selectedServerCombo, value);
                char[] separators = { '\\' };  //Host\\Instant
                string strNewHost = ServercomboList[SelectedServerCombo].ToString();
                string[] words = strNewHost.Split(separators);
                Host = words[0];
                Instant = words[1];
            }
        }

        private bool _bfoundSQL;
        public bool BfoundSQL
        {
            get { return _bfoundSQL; }
            set { SetProperty(ref _bfoundSQL, value); }
        }

        private bool _bTesting = false;
        public bool BTesting
        {
            get { return _bTesting; }
            set { SetProperty(ref _bTesting, value); }
        }

        private bool _bSaved = false;
        public bool BSaved
        {
            get { return _bSaved; }
            set { SetProperty(ref _bSaved, value); }
        }

        private bool _SearchDone = false;
        public bool SearchDone
        {
            get { return _SearchDone; }
            set { SetProperty(ref _SearchDone, value); }
        }

        private DateTime _hourStart = ClsCommon.HourStart;
        public DateTime HourStart
        {
            get { return _hourStart; }
            set 
            { 
                SetProperty(ref _hourStart, value);
                ClsCommon.HourStart = value;
            }
        }

        private DateTime _hourEnd = ClsCommon.HourEnd;
        public DateTime HourEnd
        {
            get { return _hourEnd; }
            set 
            { 
                SetProperty(ref _hourEnd, value);
                ClsCommon.HourEnd = value;
            }
        }

        private DateTime _prodDayEnd = ClsCommon.ProdDayEnd;
        public DateTime ProdDayEnd
        {
            get { return _prodDayEnd; }
            set
            {
                SetProperty(ref _prodDayEnd, value);
                ClsCommon.ProdDayEnd = value;
            }
        }

        private DateTime _shiftoneTime = ClsCommon.ShiftOneEnd;
        public DateTime ShiftOneTime
        {
            get { return _shiftoneTime; }
            set
            {
                SetProperty(ref _shiftoneTime, value);
                ClsCommon.ShiftOneEnd = value;
            }
        }

        private DateTime _shiftTwoTime = ClsCommon.ShiftTwoEnd;
        public DateTime ShiftTwoTime
        {
            get { return _shiftTwoTime; }
            set
            {
                SetProperty(ref _shiftTwoTime, value);
                ClsCommon.ShiftTwoEnd = value;
            }
        }
        private DateTime _shiftThreeTime = ClsCommon.ShiftThreeEnd;
        public DateTime ShiftThreeTime
        {
            get { return _shiftThreeTime; }
            set
            {
                SetProperty(ref _shiftThreeTime, value);
                ClsCommon.ShiftThreeEnd = value;
            }
        }

        private int _scanrateRt = ClsCommon.ScanRate;
        public int ScanRateRt
        {
            get { return _scanrateRt; }
            set
            {
                if ((value > 0) & (value < 601))
                {
                    SetProperty(ref _scanrateRt, value);
                    ClsCommon.ScanRate = value;
                }
                else
                {
                    SetProperty(ref _scanrateRt, 10);
                    ClsCommon.ScanRate = 10;
                }
            }
        }


        private int _scanrateWl = ClsCommon.ScanRateWl;
        public int ScanRateWl
        {
            get { return _scanrateWl; }
            set
            {
                if ((value > 0) & (value < 601))
                {
                    SetProperty(ref _scanrateWl, value);
                    ClsCommon.ScanRateWl = value;
                }
                else
                {
                    SetProperty(ref _scanrateWl, 10);
                    ClsCommon.ScanRateWl = 10;
                }
            }
        }

      

        private bool _kgChecked = true;
        public bool KGChecked
        {
            get { return _kgChecked; }
            set {SetProperty(ref _kgChecked, value);}
        }

        private bool _lbChecked;
        public bool LBChecked
        {
            get { return _lbChecked; }
            set {SetProperty(ref _lbChecked, value); }
        }


        private string _rTStatus;
        public string RTStatus
        {
            get { return _rTStatus; }
            set { SetProperty(ref _rTStatus, value); }
        }
        private bool _rtReportOn = ClsCommon.RtReportOn;
        public bool RtReportOn
        {
            get { return _rtReportOn; }
            set 
            { 
                SetProperty(ref _rtReportOn, value);
                ClsCommon.RtReportOn = value;
                Settings.Default.RtReportOn = value;
                Settings.Default.Save();
                if (value)
                {
                    RTStatus = "ON";
                    RTRepOpc = "1";
                }
                else
                {
                    RTStatus = "OFF";
                    RTRepOpc = ".2";
                }
            }
        }


        private string _wlStatus;
        public string WLStatus
        {
            get { return _wlStatus; }
            set { SetProperty(ref _wlStatus, value); }
        }
        private bool _wlReportOn  = ClsCommon.WlReportOn;
        public bool WlReportOn
        {
            get { return _wlReportOn; }
            set
            {
                SetProperty(ref _wlReportOn, value);
                ClsCommon.WlReportOn = value;
                Settings.Default.WlReportOn = value;
                Settings.Default.Save();
                if (value)
                {
                    WLStatus = "ON";
                    WlRepOpc = "1";
                }
                else
                {
                    WLStatus = "OFF";
                    WlRepOpc = ".2";
                }
            }
        }


        

        private bool _wetLayerExcist;
        public bool WetLayerExcist
        {
            get { return _wetLayerExcist; }
            set { SetProperty(ref _wetLayerExcist, value); }
        }

        private bool _wLCSVChecked;
        public bool WLCSVChecked
        {
            get => _wLCSVChecked;
            set {SetProperty(ref _wLCSVChecked, value);}
        }
        private bool _wLExcelChecked = ClsCommon.BExcelOut;
        public bool WLExcelChecked
        {
            get => _wLExcelChecked;
            set
            {
                SetProperty(ref _wLExcelChecked, value);
                ClsCommon.BExcelOut = value;
                if (ClsCommon.BExcelOut == true) WLCSVChecked = false;
                else WLCSVChecked = true;
            }
        }

        private bool _selectRealtime;
        public bool SelectRealtime
        {
            get => _selectRealtime;
            set
            {
                SetProperty(ref _selectRealtime, value);
            }
        }

        private bool _selectProdtime;
        public bool SelectProdtime
        {
            get => _selectProdtime;
            set
            {
                SetProperty(ref _selectProdtime, value);
                ClsCommon.UsedProdTime = value;
            }
        }

        private bool _cSVChecked;
        public bool CSVChecked
        {
            get => _cSVChecked;
            set 
            { 
                SetProperty(ref _cSVChecked, value);
                ClsCommon.BRtCsvOut = value;
            }
        }
        private bool _ExcelChecked;
        public bool ExcelChecked
        {
            get => _ExcelChecked;
            set
            {
                SetProperty(ref _ExcelChecked, value);
                ClsCommon.BRtExcelOut = value;
                if (ClsCommon.BRtExcelOut == true) CSVChecked = false;
                else CSVChecked = true;
            }
        }

        private string _strFileLocation = ClsCommon.ExCsvFileLocation;
        public string strFileLocation
        {
            get { return _strFileLocation; }
            set
            {
                SetProperty(ref _strFileLocation, value);
                ClsCommon.ExCsvFileLocation = value;
            }
        }

        #region Custom Realtime field

        private List<string> _customList;
        public List<string> CustomList
        {
            get { return _customList; }
            set { SetProperty(ref _customList, value); }
        }
        private int _customLstIndex;
        public int CustomLstIndex
        {
            get { return _customLstIndex; }
            set 
            { 
                SetProperty(ref _customLstIndex, value);

                CustomFldsel = (value == 1) ? true : false;
                ShowFldSel = (value == 1) ? "1" : "0.1";
            }
        }
        private bool _customFldsel;
        public bool CustomFldsel
        {
            get => _customFldsel;
            set
            {
                SetProperty(ref _customFldsel, value);
                ClsCommon.UsedCusField = value;
            }
        }
        private string _showFldSel = "1";
        public string ShowFldSel
        {
            get => _showFldSel;
            set => SetProperty(ref _showFldSel, value);
        }

        private Window RtFieldSelWindow;

        private DelegateCommand _columnConfigCommand;
        public DelegateCommand ColumnConfigCommand =>
       _columnConfigCommand ?? (_columnConfigCommand =
            new DelegateCommand(ColumnConfigCommandExecute).ObservesCanExecute(() => BModify));
        private void ColumnConfigCommandExecute()
        {
            if (RtFieldSelWindow != null) RtFieldSelWindow = null;
            RtFieldSelWindow = new Window
            {
                Title = "RealTime Report Column Selection",
                Width = 1120,
                Height = 520,
                Content = new FieldSelectView(_eventAggregator)
            };
            if (RtFieldSelWindow != null)
            {
                RtFieldSelWindow.ResizeMode = ResizeMode.NoResize;
                RtFieldSelWindow.ShowDialog();
            }
        }

        private void CloseRtFieldSelWindow(bool obj)
        {
            if (obj)
            {
                if (RtFieldSelWindow != null)
                {
                    RtFieldSelWindow.Close();
                    //    RtFieldSelWindow = null;
                }
            }
        }


        #endregion Custom Realtime field



        private DelegateCommand _searchCommand;
        public DelegateCommand SearchCommand =>
       _searchCommand ?? (_searchCommand = new DelegateCommand(SearchCommandExecute).ObservesCanExecute(() => BModify));
        private void SearchCommandExecute()
        {
            ServercomboList = SqlConfigModel.GetServerList();

            if (ServercomboList.Count > 0)
            {
                SelectedServerCombo = 0; //Always goes to the top
                BfoundSQL = true;
                SearchDone = true;
                SqlStatus = "SQL Server Found";
            }
            else
            {
                SqlStatus = "No SQL Server!";
                BfoundSQL = false;
                SearchDone = false;
            }
        }

        private DelegateCommand _testCommand;
        public DelegateCommand TestCommand =>
       _testCommand ?? (_testCommand = 
            new DelegateCommand(TestCommandExecute).ObservesCanExecute(() => BModify).ObservesCanExecute(() => BfoundSQL));
        private void TestCommandExecute()
        {
            BTesting = SqlConfigModel.TestSqlConnection(Host, Instant, Database, Userid, Password);
        }


        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand =>
       _saveCommand ?? (_saveCommand = 
            new DelegateCommand(SaveCommandExecute).ObservesCanExecute(() => BTesting));
        private void SaveCommandExecute()
        {
            BSaved = SaveSetting();
           
            ClsSerilog.LogMessage(ClsSerilog.Info, $"Change Connections to ... -> {ClsCommon.Host}");

            //_eventAggregator.GetEvent<CloseAppEvent>().Publish(true);
        }

        private bool SaveSetting()
        {
            ClsCommon.Host = Host;
            ClsCommon.Instance = Instant;
            ClsCommon.UserName =  Userid;
            ClsCommon.PassWord = Password;
            ClsCommon.RTDatabase =  Database;

            return true;
        }

        
        private DelegateCommand _modifyCommand;
        public DelegateCommand ModifyCommand =>
       _modifyCommand ?? (_modifyCommand = new DelegateCommand(ModifyCommandExecute));
        private void ModifyCommandExecute()
        {
            BModify = true;
        }

        private DelegateCommand _applyCommand;
        public DelegateCommand ApplyCommand =>
       _applyCommand ?? (_applyCommand = 
            new DelegateCommand(ApplyCommandExecute).ObservesCanExecute(() => BModify));
        private void ApplyCommandExecute()
        {
            if (LBChecked)
            {
                ClsCommon.WeightType = ClsCommon.WtEnglish;
                ClsCommon.WeightUnit = ClsCommon.WtUnit[ClsCommon.WtEnglish];
            }
            else
            {
                ClsCommon.WeightType = ClsCommon.WtMetric;
                ClsCommon.WeightUnit = ClsCommon.WtUnit[ClsCommon.WtMetric];
            }
            
            

            ClsCommon.SQLTransferOn = SQLTransferOn;
            ClsCommon.RtTransferEnable = RtTransferEnable;
            ClsCommon.WlTransferEnable = WlTransferEnable;

            BModify = false;

            BSaved = SaveSetting();

            ClsSerilog.LogMessage(ClsSerilog.Info, $"Modified Parameters -> {ClsCommon.Host}");

            _eventAggregator.GetEvent<CloseAppEvent>().Publish(true);
        }

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
       _cancelCommand ?? (_cancelCommand = 
            new DelegateCommand(CancelCommandExecute).ObservesCanExecute(() => BModify));
        private void CancelCommandExecute()
        {
            BModify = false;

            Host = ClsCommon.Host;
            Instant = ClsCommon.Instance;
            Userid = ClsCommon.UserName;
            Password = ClsCommon.PassWord;
            Database = ClsCommon.RTDatabase;
            BTesting = false;
        }

        private DelegateCommand _browseCommand;
        public DelegateCommand BrowseCommand =>
        _browseCommand ?? (_browseCommand = new DelegateCommand(BrowseCommandExecute));
        private void BrowseCommandExecute()
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                strFileLocation = dlg.SelectedPath;
            }
            dlg = null;
            findCreateDir(strFileLocation);
        }

        private void findCreateDir(string dirname)
        {
            try
            {
                if (!Directory.Exists(dirname))
                {
                    DirectoryInfo Di = Directory.CreateDirectory(dirname);
                    Di.Attributes = FileAttributes.ReadOnly;
                    Di.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in findCreateDir " + ex);
            }
        }

        public SetupViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;


            showsql = Visibility.Hidden;

            Host = ClsCommon.Host;
            Instant = ClsCommon.Instance;
            Userid = ClsCommon.UserName;
            Password = ClsCommon.PassWord;
            Database = ClsCommon.RTDatabase;

            _eventAggregator.GetEvent<CloseFieldSelWindow>().Subscribe(CloseRtFieldSelWindow);

            //    SqlStatus = "Not Connected!";
            Message = "View from SETUP Module";

            WetLayerVisible = (ClsCommon.WLActive) ? Visibility.Visible : Visibility.Hidden;

            WlReportOn = ClsCommon.WlReportOn;
            RtReportOn = ClsCommon.RtReportOn;

            SQLTransferOn = ClsCommon.SQLTransferOn;
            RtTransferEnable = ClsCommon.RtTransferEnable;
            WlTransferEnable = ClsCommon.WlTransferEnable;

            CustomList = new List<string>() { "Pre-Defined", "Manual" };
            CustomLstIndex = (ClsCommon.UsedCusField) ? 1 : 0;
            //howFldSel = (ClsCommon.UsedCusField) ? "0.1" : "1";

            BModify = false;
          
            if (ClsCommon.UsedProdTime) SelectProdtime = true;
            else SelectRealtime = true;

          //  SelectProdtime = true;

            if (ClsCommon.WeightType == ClsCommon.WtMetric) KGChecked = true;
            else LBChecked = true;

            SearchDone = false;

            if (ClsCommon.BExcelOut == true) WLCSVChecked = false;
            else WLCSVChecked = true;

            if (ClsCommon.BRtExcelOut == true) ExcelChecked = true;
            else CSVChecked = true;

            SQLXferValid = ClsCommon.SQLXferValid;

        }
    }
}
