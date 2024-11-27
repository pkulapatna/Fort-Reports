using FieldsColumnSelect.Model;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using RTRep.Services;
using SQLDataTansfer.Model;
using SQLDataTansfer.Properties;
using SQLDataTansfer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static RTRep.Services.ClsApplicationService;

namespace SQLDataTansfer.ViewModels
{
    public class DataTransferViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;
        private readonly DataTransferModel DataXferModel;
        private ClsSQLhandler SqlConfigModel = ClsSQLhandler.Instance;
        private Xmlhandler MyXml = Xmlhandler.Instance;

        private Window FldMapWindow;

        private ObservableCollection<SqlOutFields> _sqlOutputFields;
        public ObservableCollection<SqlOutFields> SqlOutputFields
        {
            get => _sqlOutputFields;
            set => SetProperty(ref _sqlOutputFields, value);
        }

        private bool _rtTabEnable;
        public bool RtTabEnable
        {
            get => _rtTabEnable;
            set
            {
                SetProperty(ref _rtTabEnable, value);
                RtTabWidth = (value) ? 220 : 0;
            }
        }
        private int _rtTabWidth;
        public int RtTabWidth
        {
            get => _rtTabWidth;
            set => SetProperty(ref _rtTabWidth, value);
        }

        private int _selectedIndex;
        public int SelectedLstViewIndex
        {
            get => _selectedIndex;
            set{SetProperty(ref _selectedIndex, value);}
        }

        private string _selectedLstViewVal;
        public string SelectedLstViewVal
        {
            get => _selectedLstViewVal;
            set { SetProperty(ref _selectedLstViewVal, value); }
        }


        private string _textStatus = "Idle";
        public string TextStatus
        {
            get => _textStatus;
            set { SetProperty(ref _textStatus, value); }
        }
        internal void ListViewClickHandler(string hdrName)
        {

            if ((BModify) & (hdrName == "Forte Fields") & (SelectedLstViewIndex > -1))
            {
                if (SqlOutputFields[SelectedLstViewIndex].ClientFieldName.Length > -1)
                {
                    if (FldMapWindow != null) FldMapWindow = null;
                    FldMapWindow = new Window
                    {
                        Width = 560,
                        Height = 300,
                        Content = new SqlFieldsMap(_eventAggregator, SqlOutputFields[SelectedLstViewIndex].ClientFieldName)
                    };
                    FldMapWindow.ResizeMode = ResizeMode.NoResize;
                    FldMapWindow.ShowDialog();
                }
            }   
        }

        private List<Tuple<string, string>> _sqlFieldList;
        public List<Tuple<string, string>> SqlFieldList
        {
            get { return _sqlFieldList; }
            set { SetProperty(ref _sqlFieldList, value); }
        }

        private List<Tuple<string, string>> _clientFieldList;
        public List<Tuple<string, string>> ClientFieldList
        {
            get { return _clientFieldList; }
            set { SetProperty(ref _clientFieldList, value); }
        }


        private bool _rtAutoOn;
        public bool RtAutoOn
        {
            get => _rtAutoOn;
            set => SetProperty(ref _rtAutoOn, value);
        }


        private bool _rtXferScannerOn;
        public bool RtXferScannerOn
        {
            get => _rtXferScannerOn;
            set
            {
                SetProperty(ref _rtXferScannerOn, value);
                BaleXferStatus = (value) ? "ON" : "OFF";
                ShowMe = (value) ? "1" : ".1";
                IdleMode = (value) ? false : true;
                DataXferModel.RtXferScannerOn = value;
            }

        }
        private string _baleXferStatus;
        public string BaleXferStatus
        {
            get => _baleXferStatus;
            set => SetProperty(ref _baleXferStatus, value);
        }

        private bool _idleMode;
        public bool IdleMode
        {
            get => _idleMode; 
            set => SetProperty(ref _idleMode, value); 
        }

        #region WetLayer


        private bool _wlTabEnable;
        public bool WlTabEnable
        {
            get => _wlTabEnable;
            set
            {
                SetProperty(ref _wlTabEnable, value);
                WlTabWidth = (value) ? 220 : 0;
            }
        }
        private int _wlTabWidth;
        public int WlTabWidth
        {
            get => _wlTabWidth;
            set => SetProperty(ref _wlTabWidth, value);
        }

        private ObservableCollection<SqlOutFields> _sqlWlOutputFields;
        public ObservableCollection<SqlOutFields> SqlWlOutputFields
        {
            get => _sqlWlOutputFields;
            set => SetProperty(ref _sqlWlOutputFields, value);
        }


        private bool _wlXferScannerOn;
        public bool WlXferScannerOn
        {
            get => _wlXferScannerOn;
            set 
            {
                SetProperty(ref _wlXferScannerOn, value);
                WLXferStatus = (value) ? "ON" : "OFF";
                ShowWLXfer = (value) ? "1" : "0.1";
                WlIdleMode = (value) ? false : true;
                DataXferModel.WlXferScannerOn = value;
            }
        }

        private bool _wlIdleMode;
        public bool WlIdleMode
        {
            get => _wlIdleMode;
            set => SetProperty(ref _wlIdleMode, value);
        }


        private string _wLXferStatus;
        public string WLXferStatus
        {
            get => _wLXferStatus; 
            set => SetProperty(ref _wLXferStatus, value); 
        }
        private string _showWLXfer = ".1";
        public string ShowWLXfer
        {
            get => _showWLXfer;
            set => SetProperty(ref _showWLXfer, value);
        }

        private bool _bWlTestCon = false;
        public bool BWlConGood
        {
            get { return _bWlTestCon; }
            set { SetProperty(ref _bWlTestCon, value); }
        }


        #endregion WetLayer

        private bool _SearchDone = false;
        public bool SearchDone
        {
            get => _SearchDone;
            set => SetProperty(ref _SearchDone, value);
        }

        private List<string> _servercomboList;
        public List<string> ServercomboList
        {
            get => _servercomboList; 
            set => SetProperty(ref _servercomboList, value); 
        }
        private int _selectedServerIdx;
        public int SelectedServerIdx
        {
            get => _selectedServerIdx; 
            set
            {
                SetProperty(ref _selectedServerIdx, value);
                char[] separators = { '\\' };  //Host\\Instant
                string strNewHost = ServercomboList[SelectedServerIdx].ToString();
                string[] words = strNewHost.Split(separators);
                XferHost = words[0];
                XferInstant = words[1];
            }
        }

        private List<string> _forteItemList;
        public  List<string> ForteItemList
        {
            get => _forteItemList;
            set => SetProperty(ref _forteItemList, value);
        }

        private bool _bModify = false;
        public bool BModify
        {
            get => _bModify;
            set => SetProperty(ref _bModify, value);
        }

        private bool _bManualCon = false;
        public bool BManualCon
        {
            get => _bManualCon;
            set => SetProperty(ref _bManualCon, value);
        }

        private string _showme = "1";
        public string ShowMe
        {
            get => _showme;
            set => SetProperty(ref _showme, value);
        }

        private bool _bTesting = false;
        public bool BTesting
        {
            get { return _bTesting; }
            set { SetProperty(ref _bTesting, value); }
        }


        private bool _bTestCon = false;
        public bool BTestCon
        {
            get { return _bTestCon; }
            set { SetProperty(ref _bTestCon, value); }
        }


        private bool _bFoundDb = false;
        public bool BFoundDb
        {
            get { return _bFoundDb; }
            set { SetProperty(ref _bFoundDb, value); }
        }


        #region Buttons Handler

        //-----------------------------------------------------------------------------------Rt

        private DelegateCommand _searchRtCommand;
        public DelegateCommand SearchRtCommand =>
       _searchRtCommand ?? (_searchRtCommand = new DelegateCommand(SearchCommandExecute));
        private void SearchCommandExecute()
        {

            ServercomboList = SqlConfigModel.GetServerList();
            if (ServercomboList.Count > 0)
            {
                SelectedServerIdx = 0;
                SearchDone = true;
                TextStatus = "Found SQL Server";
                BManualCon = false;
            }
            else
            {
                SearchDone = false;
                TextStatus = "No SQL Server found!";
                BManualCon = true;
            }
        }
        private DelegateCommand _selectRtDbCommand;
        public DelegateCommand SelectRtDbCommand =>
       _selectRtDbCommand ?? (_selectRtDbCommand = new DelegateCommand(SelectDbCommandExecute).ObservesCanExecute(() => SearchDone));
        private void SelectDbCommandExecute()
        {
            if (XferRtServerName != null)
            {
                DbcomboList = SqlConfigModel.getDbList(XferRtServerName, SQLAuChecked, XferUserId, XferUserPwd);
                SearchDbDone = (DbcomboList.Count > 0) ? true : false;
            }
        }

        private DelegateCommand _selectRtTableCommand;
        public DelegateCommand SelectRtTableCommand =>
       _selectRtTableCommand ?? (_selectRtTableCommand = new DelegateCommand(SelectTableCommandExecute).ObservesCanExecute(() => SearchDbDone));
        private void SelectTableCommandExecute()
        {
            TablecomboList = SqlConfigModel.GetTableList(XferRtServerName, XferRtDbName);
            SearchTableDone = (TablecomboList.Count > 0) ? true : false;
        }

        /// <summary>
        ///----------------------------------------------------SQL
        /// </summary>
        private DelegateCommand _testRtSqlCommand;
        public DelegateCommand TestRtSqlCommand =>
       _testRtSqlCommand ?? (_testRtSqlCommand = new DelegateCommand(TestSqlCommandExecute).ObservesCanExecute(() => SearchDbDone));  //SearchDbDone
        private void TestSqlCommandExecute()
        {
            BTestCon = DataXferModel.TestSqlConnection(XferHost, XferInstant, XferRtDbName, XferUserId, XferUserPwd, SQLAuChecked);
            TextStatus = (BTestCon) ? "SQL connections Pass" : "SQL connections Fail";

            
        }
        private DelegateCommand _saveRtSqlCommand;
        public DelegateCommand SaveRtSqlCommand =>
       _saveRtSqlCommand ?? (_saveRtSqlCommand = new DelegateCommand(SaveSqlCommandExecute).ObservesCanExecute(() => BTestCon));
        private void SaveSqlCommandExecute()
        {
            SaveRtSQLSettings();
        }

        private DelegateCommand _cancelRtSqlCommand;
        public DelegateCommand CancelRtSqlCommand =>
       _cancelRtSqlCommand ?? (_cancelRtSqlCommand = new DelegateCommand(CancelSqlCommandExecute).ObservesCanExecute(() => BTestCon));
        private void CancelSqlCommandExecute()
        {
            BModify = false;
            TextStatus = "Cancel";

        }

        //--------------------------------------------------------SQL

        //------------------------------------------------------------------------------------------



        private DelegateCommand _modifyCommand;
        public DelegateCommand ModifyCommand =>
       _modifyCommand ?? (_modifyCommand = new DelegateCommand(ModifyCommandExecute).ObservesCanExecute(()=> IdleMode));
        private void ModifyCommandExecute()
        {
            BModify = true;
            TextStatus = "Modify Mode";
        }

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
       _cancelCommand ?? (_cancelCommand = new DelegateCommand(CancelCommandExecute).ObservesCanExecute(() => BModify));
        private void CancelCommandExecute()
        {
            BModify = false;
            TextStatus = "Cancel";
        }

        private DelegateCommand _saveCommand;
        public DelegateCommand SaveRtTableCommand =>
       _saveCommand ?? (_saveCommand = new DelegateCommand(SaveCommandExecute).ObservesCanExecute(() => BModify));
        private void SaveCommandExecute()
        {
            BModify = false;
            TextStatus = "Save Mapping Table";
            SaveOutputTable();
        }

        private string _xferRtServerName;
        public string XferRtServerName
        {
            get => _xferRtServerName;
            set  { SetProperty(ref _xferRtServerName, value);}
        }

        private DelegateCommand _testSendRtCommand;
        public DelegateCommand TestSendRtCommand =>
       _testSendRtCommand ?? (_testSendRtCommand = new DelegateCommand(TestSendRtCommandExecute).ObservesCanExecute(() => IdleMode));  //SearchDbDone
        /// <summary>
        /// Send the newest bale data from the Forte's Archive table.
        /// </summary>
        private void TestSendRtCommandExecute()
        {
            bool SendDone =   DataXferModel.TestSendRtRemote(SqlOutputFields, XferRtDbName, XferRtTableName, true);
            TextStatus = (SendDone) ? $"Data Send to SQL Server{XferRtTableName}" : "Data Not Send to Client SQL Server";
        }


        


        #endregion Buttons Handler

        //-------------------------------------------------------------

        private bool _SearchDbDone = false;
        public bool SearchDbDone
        {
            get => _SearchDbDone;
            set => SetProperty(ref _SearchDbDone, value);
        }

       
        private List<string> _dbcomboList;
        public List<string> DbcomboList
        {
            get => _dbcomboList;
            set => SetProperty(ref _dbcomboList, value);
        }
       
        private bool _winAuChecked;
        public bool WinAuChecked
        {
            get => _winAuChecked;
            set 
            { 
                SetProperty(ref _winAuChecked, value);
                Settings.Default.WinAuCheck = value;
                Settings.Default.Save();
            }
        }
        private bool _sQLAuChecked;
        public bool SQLAuChecked
        {
            get => _sQLAuChecked;
            set { SetProperty(ref _sQLAuChecked, value); }
        }

       
        
        //----------------------------------------------------------------

        private bool _SearchTableDone = false;
        public bool SearchTableDone
        {
            get => _SearchTableDone;
            set => SetProperty(ref _SearchTableDone, value);
        }

        private string _dbTableName;
        public string DbTableName
        {
            get => _dbTableName;
            set => SetProperty(ref _dbTableName, value);
        }
        private List<string> _tablecomboList;
        public List<string> TablecomboList
        {
            get => _tablecomboList;
            set => SetProperty(ref _tablecomboList, value);
        }
        

        //--------------------------------------------------------------------------
        #region Xfer params

        private string _xferHost;
        public string XferHost
        {
            get => _xferHost;
            set => SetProperty(ref _xferHost, value);
        }

        private string _xferInstant;
        public string XferInstant
        {
            get => _xferInstant;
            set => SetProperty(ref _xferInstant, value);
        }

        private string _xferUserId;
        public string XferUserId
        {
            get => _xferUserId;
            set => SetProperty(ref _xferUserId, value);
        }

        private string _xferUserPwd;
        public string XferUserPwd
        {
            get => _xferUserPwd;
            set => SetProperty(ref _xferUserPwd, value);
        }

        private string _xferRtDbName;
        public string XferRtDbName
        {
            get => _xferRtDbName;
            set => SetProperty(ref _xferRtDbName, value);
        }

        private string _xferRtTableName;
        public string XferRtTableName
        {
            get => _xferRtTableName;
            set
            {
                SetProperty(ref _xferRtTableName, value);
                if (value != null)
                {
                    List<Tuple<string, string>> MyFieldList = new List<Tuple<string, string>>();
                    MyFieldList = DataXferModel.GetColumnList(XferRtDbName, value);

                    if(Settings.Default.XferRtTableName == value)
                    {
                        SqlOutputFields = MyXml.ReadRtXfertable();
                    }
                    else
                    {
                        if (SqlOutputFields != null) SqlOutputFields = null;
                        SqlOutputFields = new ObservableCollection<SqlOutFields>();
                        SqlOutputFields.Clear();

                        ClientFieldList = new List<Tuple<string, string>>();

                        for (int i = 0; i < MyFieldList.Count; i++)
                        {
                            SqlOutputFields.Add(new SqlOutFields(MyFieldList[i].Item1, MyFieldList[i].Item2, "", ""));
                            ClientFieldList.Add(new Tuple<string, string>(MyFieldList[i].Item1, "hello"));
                        }
                    }
                }
            }
        }


        #endregion Xfer params



        #region WetLayer
  
        /// <summary>
        /// Wet Layer Tab-----------------------------------------------------------------------------------
        /// </summary>
        /// 
        private DelegateCommand _searchWlCommand;
        public DelegateCommand SearchWlCommand =>
       _searchWlCommand ?? (_searchWlCommand = new DelegateCommand(SearchWlCommandExecute));
        private void SearchWlCommandExecute()
        {
            WlServercomboList = SqlConfigModel.GetServerList();
            SearchWlDone = (WlServercomboList.Count > 0) ? true : false;
            WlTextStatus = (WlServercomboList.Count > 0) ? "Found SQL SERVER " : "NO SQL SERVER ";
        }

        private DelegateCommand _selectWlDbCommand;
        public DelegateCommand SelectWlDbCommand =>
       _selectWlDbCommand ?? (_selectWlDbCommand = new DelegateCommand(SelectWlDbCommandExecute).ObservesCanExecute(() => SearchWlDone));
        private void SelectWlDbCommandExecute()
        {
            if (XferWlServerName != null)
            {
                DbWlcomboList = SqlConfigModel.getDbList(XferWlServerName, SQLAuChecked, XferUserId, XferUserPwd);
                SearchWlDbDone = (DbWlcomboList.Count > 0) ? true : false;
                WlTextStatus = (DbWlcomboList.Count > 0) ? "Found SQL Database " : "NO SQL Database ";
            }
        }

        private DelegateCommand _selectWlTableCommand;
        public DelegateCommand SelectWlTableCommand =>
       _selectWlTableCommand ?? (_selectWlTableCommand = new DelegateCommand(SelectWlTableCommandExecute).ObservesCanExecute(() => SearchWlDbDone));
        private void SelectWlTableCommandExecute()
        {
            TableWlcomboList = SqlConfigModel.GetTableList(XferWlServerName, XferWlDbName);
            SearchWlTableDone = (TableWlcomboList.Count > 0) ? true : false;
            WlTextStatus = (TableWlcomboList.Count > 0) ? "Found SQL Table " : "NO SQL Table ";
        }

        private DelegateCommand _testWlSqlCommand;
        public DelegateCommand TestWlSqlCommand =>
       _testWlSqlCommand ?? (_testWlSqlCommand = new DelegateCommand(TestWlSqlCommandExecute));
        private void TestWlSqlCommandExecute()
        {
            BWlConGood = DataXferModel.TestSqlConnection(XferWlHost, XferWlInstant, XferWlDbName, XferWlUserId, XferWlUserPwd, WlSQLAuChecked);
            WlTextStatus = (BWlConGood) ? "SQL connections Pass" : "SQL connections Fail";
        }

        private DelegateCommand _saveWlSqlCommand;
        public DelegateCommand SaveWlSqlCommand =>
       _saveWlSqlCommand ?? (_saveWlSqlCommand = new DelegateCommand(SaveWlSqlCommandExecute).ObservesCanExecute(() => BWlConGood));
        private void SaveWlSqlCommandExecute()
        {
            SaveWlSQLSettings();
        }

        private DelegateCommand _cancelWlSqlCommand;
        public DelegateCommand CancelWlSqlCommand =>
       _cancelWlSqlCommand ?? (_cancelWlSqlCommand = new DelegateCommand(CancelWlSqlCommandExecute).ObservesCanExecute(() => BWlConGood));
        private void CancelWlSqlCommandExecute()
        {
            WlTextStatus = "Does not save SQL Parameters";
            BWlConGood = false;
        }

        private DelegateCommand _cancelWlCommand;
        public DelegateCommand CancelWlCommand =>
        _cancelWlCommand ?? (_cancelWlCommand = new DelegateCommand(CancelWlCommandExecute).ObservesCanExecute(() => BWlModify));
        private void CancelWlCommandExecute()
        {
            BWlModify = false;
            WlTextStatus = "Cancel";
        }

        private DelegateCommand _modifyWlCommand;
        public DelegateCommand ModifyWlCommand =>
       _modifyWlCommand ?? (_modifyWlCommand = new DelegateCommand(ModifyWlCommandExecute).ObservesCanExecute(() => IdleMode));
        private void ModifyWlCommandExecute()
        {
            BWlModify = true;
            WlTextStatus = "Modify Mode";
        }

        private DelegateCommand _saveWlCommand;
        public DelegateCommand SaveWlTableCommand =>
       _saveWlCommand ?? (_saveWlCommand = new DelegateCommand(SaveWlTableCommandExecute).ObservesCanExecute(() => BWlModify));
        private void SaveWlTableCommandExecute()
        {
            BWlModify = false;
            WlTextStatus = SaveWetLayerOutputTable() ? "Fields Mapping Saved" : "Fields Mapping Not Saved";
        }


        private DelegateCommand _testSendWlCommand;
        public DelegateCommand TestSendWlCommand =>
       _testSendWlCommand ?? (_testSendWlCommand = new DelegateCommand(TestSendWlCommandExecute).ObservesCanExecute(() => IdleMode));  //SearchDbDone
        /// <summary>
        /// Send the newest bale data from the Forte's Archive table.
        /// </summary>
        private void TestSendWlCommandExecute()
        {
            WlTextStatus = "Test Send Wet Layer Data to Client";

            //    bool SendDone = DataXferModel.TestSendRtRemote(SqlOutputFields, XferRtDbName, XferRtTableName, true);
            //   WlTextStatus = (SendDone) ? $"Data Send to SQL Server{XferRtTableName}" : "Data Not Send to Client SQL Server";
        }


        // End WetLayer command ------------------------------------------

        private bool SaveWetLayerOutputTable()
        {
            return MyXml.SaveWlXfertable(WlSqlOutputFields);
        }



        private ObservableCollection<SqlOutFields> _wlSqlOutputFields;
        public ObservableCollection<SqlOutFields> WlSqlOutputFields
        {
            get => _wlSqlOutputFields;
            set => SetProperty(ref _wlSqlOutputFields, value);
        }


        private bool _searchWlDbDone = false;
        public bool SearchWlDbDone
        {
            get => _searchWlDbDone;
            set => SetProperty(ref _searchWlDbDone, value);
        }


        private bool _bWlModify = false;
        public bool BWlModify
        {
            get => _bWlModify;
            set => SetProperty(ref _bWlModify, value);
        }

        private int _selectedWlLstViewIndex;
        public int SelectedWlLstViewIndex
        {
            get => _selectedWlLstViewIndex;
            set { SetProperty(ref _selectedWlLstViewIndex, value); }
        }

        private string _selectedWlLstViewVal;
        public string SelectedWlLstViewVal
        {
            get => _selectedWlLstViewVal;
            set { SetProperty(ref _selectedWlLstViewVal, value); }
        }

        private bool _SearchWlDone = false;
        public bool SearchWlDone
        {
            get => _SearchWlDone;
            set => SetProperty(ref _SearchWlDone, value);
        }

      
        private int _wlDbIndex = 0;
        public int WlDbIndex
        {
            get => _wlDbIndex;
            set => SetProperty(ref _wlDbIndex, value);
        }

        private bool _SearchWlTableDone = false;
        public bool SearchWlTableDone
        {
            get => _SearchWlTableDone;
            set => SetProperty(ref _SearchWlTableDone, value);
        }

        private int _wlTableIndex = 0;
        public int WlTableIndex
        {
            get => _wlTableIndex;
            set => SetProperty(ref _wlTableIndex, value);
        }

        private List<string> _wlServercomboList;
        public List<string> WlServercomboList
        {
            get => _wlServercomboList;
            set => SetProperty(ref _wlServercomboList, value);
        }

        private int _selectedWlServerIdx;
        public int SelectedWlServerIdx
        {
            get => _selectedWlServerIdx;
            set
            {
                SetProperty(ref _selectedWlServerIdx, value);
                char[] separators = { '\\' };  //Host\\Instant
                string strNewHost = WlServercomboList[SelectedWlServerIdx].ToString();
                string[] words = strNewHost.Split(separators);
                XferWlHost = words[0];
                XferWlInstant = words[1];
            }
        }

        private string _xferWlServerName;
        public string XferWlServerName
        {
            get => _xferWlServerName;
            set { SetProperty(ref _xferWlServerName, value); }
        }

        private List<string> _dbWlcomboList;
        public List<string> DbWlcomboList
        {
            get => _dbWlcomboList;
            set => SetProperty(ref _dbWlcomboList, value);
        }
        private List<string> _tableWlcomboList;
        public List<string> TableWlcomboList
        {
            get => _tableWlcomboList;
            set => SetProperty(ref _tableWlcomboList, value);
        }
        private string _xferWlHost;
        public string XferWlHost
        {
            get => _xferWlHost;
            set => SetProperty(ref _xferWlHost, value);
        }
        private string _xferWlInstant;
        public string XferWlInstant
        {
            get => _xferWlInstant;
            set => SetProperty(ref _xferWlInstant, value);
        }

        private string _xferWlUserId;
        public string XferWlUserId
        {
            get => _xferWlUserId;
            set => SetProperty(ref _xferWlUserId, value);
        }
        private string _xferWlUserPwd;
        public string XferWlUserPwd
        {
            get => _xferWlUserPwd;
            set => SetProperty(ref _xferWlUserPwd, value);
        }
        private bool _winWLAuChecked = false;
        public bool WinWLAuChecked
        {
            get => _winWLAuChecked;
            set => SetProperty(ref _winWLAuChecked, value);
        }

        private bool _sQLWLAuChecked = true;
        public bool SQLWLAuChecked
        {
            get => _sQLWLAuChecked;
            set => SetProperty(ref _sQLWLAuChecked, value);
        }
        private string _xferWlDbName;
        public string XferWlDbName
        {
            get => _xferWlDbName;
            set => SetProperty(ref _xferWlDbName, value);
        }
        private string _xferWlTableName;
        public string XferWlTableName
        {
            get => _xferWlTableName;
            set 
            { 
                SetProperty(ref _xferWlTableName, value);
                if (value != null)
                {
                    List<Tuple<string, string>> MyWlFieldList = new List<Tuple<string, string>>();

                    MyWlFieldList = DataXferModel.GetColumnList(XferWlDbName, value);
                    if (Settings.Default.xferWlTableName == value)
                    {
                        SqlWlOutputFields = MyXml.ReadWlXfertable();
                    }
                    else
                    {


                    }


                }
            }
        }

        private bool _wlWinAuChecked;
        public bool WlWinAuChecked
        {
            get => _wlWinAuChecked;
            set
            {
                SetProperty(ref _wlWinAuChecked, value);
                Settings.Default.WinAuCheck = value;
                Settings.Default.Save();
            }
        }
        private bool _wlSQLAuChecked;
        public bool WlSQLAuChecked
        {
            get => _wlSQLAuChecked;
            set { SetProperty(ref _wlSQLAuChecked, value); }
        }

        private string _wlTextStatus = "Idle";
        public string WlTextStatus
        {
            get => _wlTextStatus;
            set { SetProperty(ref _wlTextStatus, value); }
        }

        // End Wet Layer------------------------------------------------------------------------
        #endregion WetLayer

        //--------------------------------------------------------------------------------------
        public DataTransferViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            
            DataXferModel = new DataTransferModel(_eventAggregator);
            ClsCommon.RemoteSqlMapfields = new ObservableCollection<SqlOutFields>();

            MyXml.CheckandCreateXMLFiles(ClsCommon.XferRtItemsXmlFilePath, "XferRtItems");

            _eventAggregator.GetEvent<CloseSqlConnectWindow>().Subscribe(CloseLoginDialog);
            _eventAggregator.GetEvent<CloseFieldMapWindow>().Subscribe(CloseFldMapWindow);
            _eventAggregator.GetEvent<UpdateXferStatus>().Subscribe(UpdateXferStatustext);

            RtTabEnable = ClsCommon.RtTransferEnable;
            if (RtTabEnable)
            {
                XferRtServerName = Settings.Default.XferRtServerName;
                if (Settings.Default.XferRtServerName != string.Empty)
                {
                    XferHost = Settings.Default.xferRtHost;
                    XferInstant = Settings.Default.xferRtInstant;
                    XferUserId = Settings.Default.xferRtUserId;
                    XferUserPwd = Settings.Default.xferRtUserPwd;
                    XferRtDbName = Settings.Default.XferRtDbName;
                    XferRtTableName = Settings.Default.XferRtTableName;

                    SqlOutputFields = new ObservableCollection<SqlOutFields>();
                    SqlOutputFields = MyXml.ReadRtXfertable();
                }
                RtXferScannerOn = false;// Settings.Default.RtXferScannerOn;

                WinAuChecked = Settings.Default.WinAuCheck;
                if (!WinAuChecked) SQLAuChecked = true;
             
                SearchDbDone = false;
                BModify = false;
            }
            
            WlTabEnable = ClsCommon.WlTransferEnable;
            if(WlTabEnable)
            {
                WLXferStatus = "OFF";
                XferWlServerName = Settings.Default.xferWlServerName;

                if (Settings.Default.xferWlServerName != string.Empty)
                {
                    XferWlHost = Settings.Default.xferWlHost;
                    XferWlInstant = Settings.Default.xferWlInstant;
                    XferWlUserId = Settings.Default.xferWlUserId;
                    XferWlUserPwd = Settings.Default.xferRtUserPwd;
                    XferWlDbName = Settings.Default.xferWlDbName;
                    XferWlTableName = Settings.Default.xferWlTableName;

                    SqlWlOutputFields = new ObservableCollection<SqlOutFields>();
                    SqlWlOutputFields = MyXml.ReadWlXfertable();
                }

                WlXferScannerOn = Settings.Default.WlXferScannerOn;

                WlWinAuChecked = Settings.Default.wlWinAuCheck;
                if (!WlWinAuChecked) WlSQLAuChecked = true;

  
            }
        }

        private void UpdateXferStatustext(string obj)
        {
            TextStatus = obj;
        }

        private void SaveWlSQLSettings()
        {
            Settings.Default.xferWlServerName = XferWlServerName;
            Settings.Default.xferWlHost = XferWlHost;
            Settings.Default.xferWlInstant = XferWlInstant;
            Settings.Default.xferWlUserId = XferWlUserId;
            Settings.Default.xferWlUserPwd = XferWlUserPwd;
            Settings.Default.xferWlDbName = XferWlDbName;
            Settings.Default.xferWlTableName = XferWlTableName;
            Settings.Default.Save();
            WlTextStatus = "Saved WetLayer SQL Parameters";
        }

        private void SaveRtSQLSettings()
        {
            Settings.Default.XferRtServerName = XferRtServerName;
            Settings.Default.xferRtHost = XferHost;
            Settings.Default.xferRtInstant = XferInstant;
            Settings.Default.xferWlUserId = XferUserId;
            Settings.Default.xferRtUserPwd = XferUserPwd;
            Settings.Default.XferRtDbName = XferRtDbName;
            Settings.Default.XferRtTableName = XferRtTableName;
            Settings.Default.Save();
            TextStatus = "Saved RealTime SQL Parameters";
        }

        private void SaveOutputTable()
        {
          bool Saved =   MyXml.SaveRtXfertable(SqlOutputFields);
        }

        private void CloseFldMapWindow(string obj)
        {
            if(obj != "cancel")
                SqlOutputFields[SelectedLstViewIndex].ForteFieldName = obj;
           
            if (FldMapWindow != null)
            {
                FldMapWindow.Close();
                FldMapWindow = null;
            }
        }

        private void CloseLoginDialog(bool obj)
        {
            /*
            if (obj)
            {
                if (LoginWindow != null)
                {
                    LoginWindow.Close();
                    LoginWindow = null;
                }  
            }
            */
        }
    }
}
