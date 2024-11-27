using Prism.Commands;
using Prism.Mvvm;
using RTRep.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WLReports.Model;
using WLReports.Properties;
using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Globalization;
using Prism.Events;
using static RTRep.Services.ClsApplicationService;

namespace WLReports.ViewModels
{
    public class WLReportViewModel : BindableBase
    {
        protected readonly Prism.Events.IEventAggregator _eventAggregator;
        private WLReportModel ReportModel;
        private string _curTime;
        public string CurTime
        {
            get => _curTime;
            set => SetProperty(ref _curTime, value);
        }

        private bool _periodRepCheck = Settings.Default.PeriodRepCheck;
        public bool PeriodRepCheck
        {
            get { return _periodRepCheck; }
            set
            {
                SetProperty(ref _periodRepCheck, value);
                PeriodRepShow = (value) ? Visibility.Visible : Visibility.Hidden;
                //ReportModel.PeriodRepCheck = value;
                //PeriodRepStatus = (value) ? "ON": "OFF";
               // HidePeriod = (value) ? "1" : ".1";
                //CanScannerOn();
            }
        }

        private Visibility _periodRepShow;
        public Visibility PeriodRepShow
        {
            get => _periodRepShow;
            set => SetProperty(ref _periodRepShow, value);
        }

        private string _periodRepStatus;
        public string PeriodRepStatus
        {
            get { return _periodRepStatus; }
            set { SetProperty(ref _periodRepStatus, value); }
        }

        private string _strPeriod;
        public string StrPeriod
        {
            get { return _strPeriod; }
            set { _strPeriod = value; }
        }

        private bool _bDemandMode = false;
        public bool BDemandMode
        {
            get { return _bDemandMode; }
            set { SetProperty(ref _bDemandMode, value); }
        }

        private Brush _GraphHiColor;
        public Brush GraphHiColor
        {
            get { return _GraphHiColor; }
            set { SetProperty(ref _GraphHiColor, value); }
        }
        private Brush _GraphLowColor;
        public Brush GraphLowColor
        {
            get { return _GraphLowColor; }
            set { SetProperty(ref _GraphLowColor, value); }
        }
        private Brush _GraphNormColor;
        public Brush GraphNormColor
        {
            get { return _GraphNormColor; }
            set { SetProperty(ref _GraphNormColor, value); }
        }

        private int _MinLimit;
        public int MinLimit
        {
            get { return _MinLimit; }
            set { SetProperty(ref _MinLimit, value); }
        }
        private int _MaxLimit;
        public int MaxLimit
        {
            get { return _MaxLimit; }
            set { SetProperty(ref _MaxLimit, value); }
        }
        private string _NormLimit;
        public string NormLimit
        {
            get { return _NormLimit; }
            set { SetProperty(ref _NormLimit, value); }
        }

        private int _MinYAxis =0;
        public int MinYAxis
        {
            get { return _MinYAxis; }
            set { SetProperty(ref _MinYAxis, value); }
        }
        private double _MaxYAxis = 40;
        public double MaxYAxis
        {
            get { return _MaxYAxis; }
            set { SetProperty(ref _MaxYAxis, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private bool _bWindowLogon =true;
        public bool bWindowLogon
        {
            get { return _bWindowLogon; }
            set { SetProperty(ref _bWindowLogon, value); }
        }

        private bool _bModify = false;
        public bool BModify
        {
            get { return _bModify; }
            set { SetProperty(ref _bModify, value); }
        }

        private int _scanrate = ClsCommon.ScanRateWl;
        public int ScanRate
        {
            get { return _scanrate; }
            set{ SetProperty(ref _scanrate, value);}
        }

        private DataTable _wetlayerDataTable;
        public DataTable WetLayerDataTable
        {
            get { return _wetlayerDataTable; }
            set { SetProperty(ref _wetlayerDataTable, value); }
        }

        private int _selecttableindex;
        public int SelectTableIndex
        {
            get { return _selecttableindex; }
            set { SetProperty(ref _selecttableindex, value); }
        }


        private string _StrFileName;
        public string StrFileName
        {
            get => _StrFileName; 
            set => SetProperty(ref _StrFileName, value);
        }

        private string _strPathFile;
        public string strPathFile
        {
            get { return _strPathFile; }
            set { SetProperty(ref _strPathFile, value); }
        }

        private string _CVScanInterval = "40";
        public string CVScanInterval
        {
            get { return _CVScanInterval; }
            set { SetProperty(ref _CVScanInterval, value); }
        }

        private string _CSVTextMsg;
        public string CSVTextMsg
        {
            get { return _CSVTextMsg; }
            set { SetProperty(ref _CSVTextMsg, value); }
        }
        private string _strStatus;
        public string StrStatus
        {
            get { return _strStatus; }
            set { SetProperty(ref _strStatus, value); }
        }

        //StrStatus

        private string _scanStatus = "OFF";
        public string ScanStatus
        {
            get { return _scanStatus; }
            set { SetProperty(ref _scanStatus, value); }
        }

        private string _dayRepStatus = "OFF";
        public string DayRepStatus
        {
            get { return _dayRepStatus; }
            set { SetProperty(ref _dayRepStatus, value); }
        }

        private string _hourRepStatus = "OFF";
        public string HourRepStatus
        {
            get { return _hourRepStatus; }
            set { SetProperty(ref _hourRepStatus, value); }
        }


        private string _shiftOneRepStatus;
        public string ShiftOneRepStatus
        {
            get { return _shiftOneRepStatus; }
            set { SetProperty(ref _shiftOneRepStatus, value); }
        }
        private static string GetShiftOneStatus()
        {
            return (Settings.Default.bShiftOneCheck) ? "ON": "OFF"; 
        }
        private static string GetShiftThreeStatus()
        {
            return (Settings.Default.bShiftThreeCheck) ? "ON": "OFF";
        }


        private string _shiftTwoRepStatus; 
        public string ShiftTwoRepStatus
        {
            get { return _shiftTwoRepStatus; }
            set { SetProperty(ref _shiftTwoRepStatus, value); }
        }
        private static string GetShiftTwoStatus()
        {
            return (Settings.Default.bShiftTwoCheck) ? "ON" : "OFF";
        }


        private string _shiftThreeRepStatus;
        public string ShiftThreeRepStatus
        {
            get { return _shiftThreeRepStatus; }
            set { SetProperty(ref _shiftThreeRepStatus, value); }
        }

        private bool _wlscanTimerOn;
        public bool WLScanTimerOn
        {
            get { return _wlscanTimerOn; }
            set
            {
                SetProperty(ref _wlscanTimerOn, value);
                ReportModel.WLReportScanOn = value;
                WetLayerOpc = (value) ? .2 : 1;
                WlScannerOff = (value) ? false : true;
                if (value)
                {
                    ScanStatus = "ON";
                    BDemandMode = false;
                    CurTime = DateTime.Now.ToString("HH:mm:ss");
                    ShowMe = 1;
                    HideMe = 1;
                    StrStatus = "WL Timer On";
                }
                else
                {
                    ScanStatus = "OFF";
                    BDemandMode = true;
                    CurTime = "OFF";
                    ShowMe = .1;
                    HideMe = 1;
                    StrStatus = "WL Timer Off";
                }
            }
        }
        private bool _wlCanScannerOn;
        public bool WlCanScannerOn
        {
            get => _wlCanScannerOn;
            set => SetProperty(ref _wlCanScannerOn, value);
        }
        private void CanScannerOn()
        {
            WlCanScannerOn = (DayRepChecked | HourRepChecked | ShiftOneCheck | ShiftTwoCheck | ShiftThreeCheck | PeriodRepCheck);
        }

        private bool _wlScannerOff;
        public bool WlScannerOff
        {
            get => _wlScannerOff;
            set => SetProperty(ref _wlScannerOff, value);
        }



        /// <summary>
        /// ---------------------Report on Demand Selects
        /// </summary>

        private bool[] _ReportArray = new bool[] { false, false, false, false, false, false };
        public bool[] ReportArray
        {
            get { return _ReportArray; }
            set { SetProperty(ref _ReportArray, value); }
        }

        public int SelectedMode
        {
            get { return Array.IndexOf(_ReportArray, true); }
        }

        private Nullable<DateTime> _daySelect = null;
        public Nullable<DateTime> DaySelect
        {
            get
            {
                if (_daySelect == null)
                    _daySelect = DateTime.Today;
                return _daySelect;
            }
            set { SetProperty(ref _daySelect, value); }
        }

        private string _prodDayEnd;
        public string ProdDayEnd
        {
            get => _prodDayEnd; 
            set { SetProperty(ref _prodDayEnd, value); }
        }

        private string _shiftOneEnd = ClsCommon.ShiftOneEnd.ToString("HH:mm");
        public string ShiftOneEnd
        {
            get => _shiftOneEnd;
            set { SetProperty(ref _shiftOneEnd, value); }
        }

        private string _shiftTwoEnd = ClsCommon.ShiftTwoEnd.ToString("HH:mm");
        public string ShiftTwoEnd
        {
            get => _shiftTwoEnd;
            set { SetProperty(ref _shiftTwoEnd, value); }
        }

        private string _shiftThreeEnd = ClsCommon.ShiftThreeEnd.ToString("HH:mm");
        public string ShiftThreeEnd
        {
            get => _shiftThreeEnd;
            set { SetProperty(ref _shiftThreeEnd, value); }
        }


        private DelegateCommand _browseCommand;
        public DelegateCommand BrowseCommand =>
        _browseCommand ?? (_browseCommand = new DelegateCommand(BrowseCommandExecute).ObservesCanExecute(() => WlScannerOff));
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

        private DelegateCommand _cSVTestCommand;
        public DelegateCommand CSVTestCommand =>
        _cSVTestCommand ?? (_cSVTestCommand = new DelegateCommand(CsvTestExecute));
        private void CsvTestExecute()
        {
            if (SelectedMode > -1)
            {
                ReportModel.GetDataForWlDemandMode(SelectedMode, DaySelect, DateTime.Now, ClsCommon.ExcelCSVReport);

                WetLayerDataTable = ReportModel.WetLayerDataTable;
                if (WetLayerDataTable.Rows.Count > 0)
                {
                    StrStatus = "Bales count = " + WetLayerDataTable.Rows.Count.ToString();
                    CSVTextMsg = ReportModel.CSVTextMsg;
                    StrFileName = ReportModel.StrFileName;
                }
            }
            else
                MessageBox.Show("No Report type Selected!");

        }

        private bool _dayRepChecked;
        public bool DayRepChecked
        {
            get { return _dayRepChecked; }
            set
            {
                SetProperty(ref _dayRepChecked, value);
                DayRepShow = (value) ? Visibility.Visible : Visibility.Hidden;
                // ReportModel.DayRepAuto = value;
                //  DayRepStatus = (value) ? "ON" : "OFF";


            }
        }
        private Visibility _dayRepShow;
        public Visibility DayRepShow
        {
            get => _dayRepShow;
            set => SetProperty(ref _dayRepShow, value);
        }


        private bool _hourRepChecked;
        public bool HourRepChecked
        {
            get { return _hourRepChecked; }
            set
            {
                SetProperty(ref _hourRepChecked, value);
                HourRepShow = (value) ? Visibility.Visible : Visibility.Hidden;
                // ReportModel.HourRepAuto = value;
                //HourRepStatus = (value) ? "ON": "OFF";
            }
        }
        private Visibility _hourRepShow;
        public Visibility HourRepShow
        {
            get => _hourRepShow;
            set => SetProperty(ref _hourRepShow, value);
        }


        private bool _shiftOneCheck;
        public bool ShiftOneCheck
        {
            get { return _shiftOneCheck; }
            set
            {
                SetProperty(ref _shiftOneCheck, value);
                //ReportModel.ShiftOneRepAuto = value;
               // ShiftOneRepStatus = (value) ? "ON": "OFF";

                ShiftOneRepShow = (value) ? Visibility.Visible : Visibility.Hidden;

                // CanScannerOn();
            }
        }

        private Visibility _shiftOneRepShow;
        public Visibility ShiftOneRepShow
        {
            get => _shiftOneRepShow;
            set => SetProperty(ref _shiftOneRepShow, value);
        }


        private bool _shiftTwoCheck;
        public bool ShiftTwoCheck
        {
            get { return _shiftTwoCheck; }
            set
            {
                SetProperty(ref _shiftTwoCheck, value);
               // ReportModel.ShiftTwoRepAuto = value;
               // ShiftTwoRepStatus = (value) ? "ON": "OFF";

                ShiftTwoRepShow = (value) ? Visibility.Visible : Visibility.Hidden;

                // CanScannerOn();
                //ClsSerilog.LogMessage(ClsSerilog.Info, $"WL Shift Two Report {value}");
            }
        }
        private Visibility _shiftTwoRepShow;
        public Visibility ShiftTwoRepShow
        {
            get => _shiftTwoRepShow;
            set => SetProperty(ref _shiftTwoRepShow, value);
        }




        private bool _shiftThreeCheck;
        public bool ShiftThreeCheck
        {
            get { return _shiftThreeCheck; }
            set
            {
                SetProperty(ref _shiftThreeCheck, value);
                ShiftThreeRepShow = (value) ? Visibility.Visible : Visibility.Hidden;


                //ReportModel.ShiftThreeRepAuto = value;
                // ShiftThreeRepStatus = (value) ? "ON": "OFF";
                // CanScannerOn();
                //ClsSerilog.LogMessage(ClsSerilog.Info, $"WL Shift Three Report {value}");
            }
        }

        private Visibility _shiftThreeRepShow;
        public Visibility ShiftThreeRepShow
        {
            get => _shiftThreeRepShow;
            set => SetProperty(ref _shiftThreeRepShow, value);
        }


        /// <summary>
        /// -------------------------------------------------------------------------------
        /// </summary>

        private double _showme = 1;
        public double ShowMe
        {
            get { return _showme; }
            set { SetProperty(ref _showme, value); }
        }

        private double _hideMe = 1;
        public double HideMe
        {
            get { return _hideMe; }
            set { SetProperty(ref _hideMe, value); }
        }

        private string _hidePeriod = ".1";
        public string HidePeriod
        {
            get => _hidePeriod;
            set => SetProperty(ref _hidePeriod, value);
        }


        private double _wetLayerOpc = 1;
        public double WetLayerOpc
        {
            get { return _wetLayerOpc; }
            set { SetProperty(ref _wetLayerOpc, value); }
        }

        //WetLayerOpc

        private int _selectWlData;
        public int SelectWlData
        {
            get { return _selectWlData; }
            set
            {
                SetProperty(ref _selectWlData, value);
                if (value > 0) DrawWetLayerChart(value);
            }
        }

        private List<string> _DayEndList;
        public List<string> DayEndList
        {
            get
            {
                _DayEndList = new List<string>();
                DateTime date = new DateTime();

                var result = Enumerable.Repeat(date, 24)
                                       .Select((x, i) => x.AddHours(i).ToString("HH:MM"));

                var hours2 = Enumerable.Range(00, 24).Select(i => (DateTime.MinValue.AddHours(i)).ToString("hh.mm"));

                var hours = from i in Enumerable.Range(0, 24)
                            let h = new DateTime(2019, 1, 1, i, 0, 0)
                            select h.ToString("t", CultureInfo.InvariantCulture);

                foreach (var item in hours)
                {
                    _DayEndList.Add(item.ToString());
                }
                return _DayEndList;
            }
        }
        private int _selectHourIndex;
        public int SelectHourIndex
        {
            get { return _selectHourIndex; }
            set{ SetProperty(ref _selectHourIndex, value);}
        }
        private DateTime _selectHourTimeVal;


        public DateTime SelectHourTimeVal
        {
            get => _selectHourTimeVal; 
            set
            { 
                SetProperty(ref _selectHourTimeVal, value);
                ReportModel.SelectHourTimeVal = value;
            }
        }

        private string _strFileLocation = ClsCommon.ExCsvFileLocationWL;
        public string strFileLocation
        {
            get { return _strFileLocation; }
            set
            {
                SetProperty(ref _strFileLocation, value);
                ClsCommon.ExCsvFileLocationWL = value;
            }
        }

        private ObservableCollection<ChartData> _wlChartList;
        public ObservableCollection<ChartData> WlChartList
        {
            get { return _wlChartList; }
            set { SetProperty(ref _wlChartList, value); }
        }

        private DelegateCommand _loadedPageICommand;
        public DelegateCommand LoadedPageICommand =>
        _loadedPageICommand ?? (_loadedPageICommand = new DelegateCommand(LoadedPageICommandExecute));
        private void LoadedPageICommandExecute()
        {
            SelectTableIndex = 0;
            StrFileName = string.Empty;// ReportModel.GetWlCurTableName();
            ProdDayEnd = ClsCommon.DayEndHrMn;
        }

        private DelegateCommand _closedPageICommand;
        public DelegateCommand ClosedPageICommand =>
        _closedPageICommand ?? (_closedPageICommand = new DelegateCommand(ClosedPageICommandExecute));
        private void ClosedPageICommandExecute()
        {
         
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
                MessageBox.Show($"Error in findCreateDir {ex.Message}");
            }
        }

     
        public WLReportViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;

            ClsSerilog.LogMessage(ClsSerilog.Info, $"Load Wet Layer Module......");

            ReportModel = new WLReportModel(_eventAggregator);

            _eventAggregator.GetEvent<UpdateWLDataEvents>().Subscribe(UpdateWLTable);
            _eventAggregator.GetEvent<UpdateWLRepTimerEvents>().Subscribe(UpdateWLEvent);

            DayRepChecked = Settings.Default.bDayRepCheck;
            HourRepChecked = Settings.Default.bHourRepCheck;
            ShiftOneCheck = Settings.Default.bShiftOneCheck;
            ShiftTwoCheck = Settings.Default.bShiftTwoCheck;
            ShiftThreeCheck = Settings.Default.bShiftThreeCheck;
            PeriodRepCheck = Settings.Default.PeriodRepCheck;

            StrPeriod = ReportModel.GetstrPeriod();
            //WLScanTimerOn = (ClsCommon.WlReportOn) ? Settings.Default.WLScanTimerOn : false;
            WLScanTimerOn = ClsCommon.WlReportOn;

            ShiftOneRepStatus = GetShiftOneStatus();
            ShiftTwoRepStatus = GetShiftTwoStatus();
            ShiftThreeRepStatus = GetShiftThreeStatus();

            ScanStatus = (ClsCommon.WlReportOn) ? "ON" : "OFF";

        }

        private void UpdateWLEvent(DateTime obj)
        {
            CurTime = obj.ToString("HH:mm:ss");
            StrStatus = CurTime;
        }

        private void UpdateWLTable(bool obj)
        {
            if (obj)
            {
                WetLayerDataTable = ReportModel.GetWlDataTableByDay();
                StrStatus = "Bales count = " + WetLayerDataTable.Rows.Count.ToString();
                CSVTextMsg = "Write File " + DateTime.Now.ToString("d");
                StrFileName = ReportModel.StrFileName;
            }
            else
            {
                StrStatus = "No SQL Data found!";
                StrFileName = string.Empty;
            }
        }

        private void DrawWetLayerChart(int selectedrow)
        {
            if (WlChartList != null) WlChartList = null;
            WlChartList = new ObservableCollection<ChartData>();
            WlChartList.Clear();
            //bOutofLimitAlarmOn = false;

            try
            {
                //Set maximum to 16 Layers only
                for (int x = 1; x < ReportModel.LayerCount + 1; x++)
                {
                    if ((WetLayerDataTable.Rows[selectedrow]["Layer" + x].ToString() != string.Empty) & (WetLayerDataTable != null))
                    {
                        double iDouble = Convert.ToDouble(WetLayerDataTable.Rows[selectedrow]["Layer" + x]);
                        WlChartList.Add(new ChartData() { Index = x, Value = iDouble, ChartColor = SetChartColors(iDouble) });
                    }
                }

               // MaxYAxis = 40; // Settings.Default.WLMaxYAxis;
               // MinYAxis = 0; // Settings.Default.WLMinYAxis;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in DrawWetLayerChart {ex.Message}");
                
            }
        }

        private Brush SetChartColors(double DBaleValue)
        {
            Brush crtColor;

            if (DBaleValue > MaxLimit)
            {
                crtColor = Brushes.Red;// GraphHiColor;
                //bOutofLimitAlarmOn = true;
            }
            else if (DBaleValue < MinLimit)
            {
                crtColor = Brushes.Yellow;// GraphLowColor;
                //bOutofLimitAlarmOn = true;
            }
            else
            {
                crtColor = Brushes.Green; ; // GraphNormColor;
            }
            return crtColor;
        }

    }
}
