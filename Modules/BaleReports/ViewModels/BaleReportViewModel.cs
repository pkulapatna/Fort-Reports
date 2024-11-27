using BaleReports.Graphs.GraphViews;
using BaleReports.Model;
using BaleReports.Properties;
using FieldsColumnSelect.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using RTRep.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static RTRep.Services.ClsApplicationService;

namespace BaleReports.ViewModels
{
    public class BaleReportViewModel : BindableBase
    {       
        protected readonly IEventAggregator _eventAggregator;
        private readonly BaleReportModel ReportsModel;

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

        private string _curTime;
        public string CurTime
        {
            get => _curTime;
            set => SetProperty(ref _curTime, value);
        }

        private string _strStatus;
        public string StrStatus
        {
            get => _strStatus;
            set => SetProperty(ref _strStatus, value);
        }

        private string _message;
        public string Message
        {
            get => _message; 
            set => SetProperty(ref _message, value); 
        }

        private string _showme = "1";
        public string ShowMe
        {
            get => _showme; 
            set => SetProperty(ref _showme, value); 
        }

        private string _hideMe = ".1";
        public string HideMe
        {
            get => _hideMe; 
            set => SetProperty(ref _hideMe, value); 
        }

        private string _hidePeriod = ".1";
        public string HidePeriod
        {
            get => _hidePeriod;
            set => SetProperty(ref _hidePeriod, value);
        }

        private double _realTimeOpc = 1;
        public double RealTimeOpc
        {
            get { return _realTimeOpc; }
            set { SetProperty(ref _realTimeOpc, value); }
        }

        private string _showFldSel = "1";
        public string ShowFldSel
        {
            get => _showFldSel; 
            set => SetProperty(ref _showFldSel, value); 
        }


        private string _printStatus = "Not Printing !";
        public string PrintStatus
        {
            get => _printStatus; 
            set => SetProperty(ref _printStatus, value); 
        }

        /// <summary>
        /// Baler selections -----------------------------------------------------
        /// </summary>
        ///
        private string m_Source;
        //
        private List<string> sourceList;
        public List<string> SourceList
        {
            get => sourceList;
            set => SetProperty(ref sourceList, value);
        }
        private int selectSourceIndex;
        public int SelectSourceIndex
        {
            get => selectSourceIndex;
            set
            {
                if (value > -1)
                {
                    SetProperty(ref selectSourceIndex, value);
                    m_Source = SourceList[value];
                }
            }
        }
        //-------------------------------------------------------------------------


        private bool _sourceSelected;
        public bool SourceSelected
        {
            get => _sourceSelected; 
            set => SetProperty(ref _sourceSelected, value); 
        }

        private bool[] _ReportArray = new bool[] { false, false, false, false, false, false };
        public bool[] ReportArray
        {
            get => _ReportArray; 
            set => SetProperty(ref _ReportArray, value); 
        }
        public int SelectedMode
        {
            get { return Array.IndexOf(_ReportArray, true); }
        }

        private DataTable _baleDataTable;
        public DataTable BaleDataTable
        {
            get => _baleDataTable; 
            set => SetProperty(ref _baleDataTable, value); 
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

        private string _strFileLocation = ClsCommon.ExCsvFileLocation;
        public string strFileLocation
        {
            get => _strFileLocation; 
            set
            {
                SetProperty(ref _strFileLocation, value);
                ClsCommon.ExCsvFileLocation = value;
            }
        }

        private string _strFileName;
        public string StrFileName
        {
            get => _strFileName; 
            set => SetProperty(ref _strFileName, value); 
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

        private DelegateCommand _cSVTestCommand;
        public DelegateCommand CSVTestCommand =>
        _cSVTestCommand ?? (_cSVTestCommand = new DelegateCommand(CsvTestExecute));
        private void CsvTestExecute() => 
            ReportsModel.GetDataForDemandMode(SelectedMode, DaySelect, DateTime.Now, ClsCommon.ExcelCSVReport, m_Source);

        private DelegateCommand _graphCommand;
        public DelegateCommand GraphCommand =>
        _graphCommand ?? (_graphCommand = new DelegateCommand(GraphCommandExecute));
        private void GraphCommandExecute() => 
            ReportsModel.GetDataForDemandMode(SelectedMode, DaySelect, DateTime.Now, ClsCommon.ExcelCSVGraph,m_Source);

        public BaleReportViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            ClsSerilog.LogMessage(ClsSerilog.Info, $"Load RealTime Module ... -> {DateTime.Now}");

            ReportsModel = new BaleReportModel(_eventAggregator);
            
            DayRepChecked = Settings.Default.bDayRepCheck; 
            HourRepChecked = Settings.Default.bHourRepCheck;
            ShiftOneCheck = Settings.Default.bShiftOneCheck;
            ShiftTwoCheck = Settings.Default.bShiftTwoCheck;
            ShiftThreeCheck = Settings.Default.bShiftThreeCheck;
            PeriodRepCheck = Settings.Default.PeriodRepCheck;
           
            LineActive = ReportsModel.LineActive;
            SourceActive = ReportsModel.SourceActive;

            ShiftLst = ReportsModel.ShiftLst;
            ShiftActive = (ShiftLst.Count > 1) ? true : false;

            setRtScaner(ClsCommon.RtScannerOn);

            StrPeriod = ReportsModel.GetstrPeriod();

            sourceList = new List<string>();
            sourceList = ReportsModel.Getsourcelist();
            if (sourceList.Count > 0) sourceList.Add("ALL");
            

            _eventAggregator.GetEvent<UpdateBaleRepTimerEvents>().Subscribe(UpdateMainTimerEvent);
            _eventAggregator.GetEvent<ReportPrintEvent>().Subscribe(ReportPrintingEvent);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void ReportPrintingEvent(string obj)
        {
            PrintStatus = "Printing " + obj + "Done";
            StrFileName = obj;

            if(obj.Contains("Printing"))
            {
                PrintStatus = obj;
            }
            else
            {
                if(ReportsModel.BaleDataTable.Rows.Count > 0 )
                {
                    if (BaleDataTable != null) BaleDataTable = null;
                    BaleDataTable = new DataTable();
                    BaleDataTable = ReportsModel.BaleDataTable;
                }
            }
        }

        private void UpdateMainTimerEvent(DateTime obj)
        {
            CurTime = obj.ToString("HH:mm:ss");
            StrStatus = CurTime;
        }

        private DelegateCommand _loadedPageICommand;
        public DelegateCommand LoadedPageICommand =>
        _loadedPageICommand ?? (_loadedPageICommand = new DelegateCommand(LoadPageExecute));
        private void LoadPageExecute()
        {
            DayRepChecked = Settings.Default.bDayRepCheck;
            HourRepChecked = Settings.Default.bHourRepCheck;

            ShiftOneCheck = Settings.Default.bShiftOneCheck;
            ShiftTwoCheck = Settings.Default.bShiftTwoCheck;
            ShiftThreeCheck = Settings.Default.bShiftThreeCheck;

            SelectSourceIndex = sourceList.Count - 1;

        }

        private bool _SelectBaleTab;
        public bool SelectBaleTab
        {
            get => _SelectBaleTab; 
            set => SetProperty(ref _SelectBaleTab, value); 
        }

        private DelegateCommand _closedPageICommand;
        public DelegateCommand ClosedPageICommand =>
        _closedPageICommand ?? (_closedPageICommand = new DelegateCommand(ClosePageExecute));
        private void ClosePageExecute()
        {
            //Close page
        }

        private void setRtScaner( bool scanState)
        {
            ScanStatus = (scanState) ? "ON" : "OFF"; 

            if(scanState)
            {
                ScanStatus = "ON";
                EventStatus = "Timer On";
                CurTime = DateTime.Now.ToString("HH:mm:ss");
                StrStatus = EventStatus;
            }
            else
            {
                ScanStatus = "OFF";
                EventStatus = "Timer Off";
            }

        }

        

        private string _showExcelRep = ".1";
        public string ShowExcelRep
        {
            get => _showExcelRep; 
            set => SetProperty(ref _showExcelRep, value); 
        }


        private bool _scanTimerOff;
        public bool ScanTimerOff
        {
            get => _scanTimerOff; 
            set 
            { 
                SetProperty(ref _scanTimerOff, value);
                ShowExcelRep = (value) ? "1" : ".1";
            }
        }

        private string _dayEndTime = ClsCommon.ProdDayEnd.ToString("HH:mm");
        public string DayEndTime
        {
            get => _dayEndTime; 
            set => SetProperty(ref _dayEndTime, value); 
        }
        
        private string _shiftOneEnd = ClsCommon.ShiftOneEnd.ToString("HH:mm");
        public string ShiftOneEnd
        {
            get => _shiftOneEnd; 
            set => SetProperty(ref _shiftOneEnd, value); 
        }
       


        private string _shiftTwoEnd = ClsCommon.ShiftTwoEnd.ToString("HH:mm");
        public string ShiftTwoEnd
        {
            get => _shiftTwoEnd; 
            set => SetProperty(ref _shiftTwoEnd, value); 
        }
       
        private string _shiftThreeEnd = ClsCommon.ShiftThreeEnd.ToString("HH:mm");
        public string ShiftThreeEnd
        {
            get => _shiftThreeEnd; 
            set => SetProperty(ref _shiftThreeEnd, value); 
        }
       

        private string _strPeriod;
        public string StrPeriod
        {
            get => _strPeriod; 
            set => _strPeriod = value; 
        }
      


        private string _scanStatus = "OFF";
        public string ScanStatus
        {
            get => _scanStatus; 
            set => SetProperty(ref _scanStatus, value); 
        }

        private string _eventStatus = "OFF LINE !";
        public string EventStatus
        {
            get => _eventStatus; 
            set => SetProperty(ref _eventStatus, value); 
        }

        #region Day Report

        private bool _dayRepoffline;
        public bool DayRepoffline
        {
            get => _dayRepoffline; 
            set => SetProperty(ref _dayRepoffline, value); 
        }

        private bool _dayRepGraphActive;
        public bool DayRepGraphActive
        {
            get => _dayRepGraphActive; 
            set => SetProperty(ref _dayRepGraphActive, value); 
        }


        private string _dayRepStatus;
        public string DayRepStatus
        {
            get => _dayRepStatus; 
            set => SetProperty(ref _dayRepStatus, value); 
        }

        private Visibility _dayRepShow;
        public Visibility DayRepShow
        {
            get => _dayRepShow;
            set => SetProperty(ref _dayRepShow, value);
        }


        private bool _dayRepChecked;
        public bool DayRepChecked
        {
            get => _dayRepChecked;
            set
            {
                SetProperty(ref _dayRepChecked, value);
                DayRepShow = (value) ? Visibility.Visible : Visibility.Hidden;
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
            get => _selectHourIndex; 
            set =>  SetProperty(ref _selectHourIndex, value);
        }

        private DateTime _selectHourTimeVal;
        public DateTime SelectHourTimeVal
        {
            get => _selectHourTimeVal; 
            set
            {
                SetProperty(ref _selectHourTimeVal, value);
                ReportsModel.SelectHourTimeVal = value;
            }
        }

        #endregion Day Report



        #region Hour Report
       
        private Visibility _hourRepShow;
        public Visibility HourRepShow
        {
            get => _hourRepShow;
            set => SetProperty(ref _hourRepShow, value);
        }


        private bool _hourRepGraphActive;
        public bool HourRepGraphActive
        {
            get => _hourRepGraphActive; 
            set => SetProperty(ref _hourRepGraphActive, value); 
        }

        private bool _hourRepChecked;
        public bool HourRepChecked
        {
            get => _hourRepChecked; 
            set
            {
                SetProperty(ref _hourRepChecked, value);
                HourRepShow = (value) ? Visibility.Visible : Visibility.Hidden;
            }
        }

       
        #endregion Hour Report

        //---------------------------------------------

        private bool _lineActive;
        public bool LineActive
        {
            get => _lineActive; 
            set
            {
                SetProperty(ref _lineActive, value);
                LineVisble = (value) ? "1": ".1";
            }
        }
        private string _lineVisble;
        public string LineVisble
        {
            get => _lineVisble; 
            set => SetProperty(ref _lineVisble, value); 
        }

        private List<string> _lineList;
        public List<string> LineList
        {
            get => _lineList; 
            set => SetProperty(ref _lineList, value); 
        }

     
        //---------------------------------------------------------------------------

        private bool _sourceActive;
        public bool SourceActive
        {
            get => _sourceActive; 
            set
            {
                SetProperty(ref _sourceActive, value);
                SourceVisble = (value) ? "1": ".1";
            }
        }

        private string _SourceVisble;
        public string SourceVisble
        {
            get => _SourceVisble; 
            set => SetProperty(ref _SourceVisble, value); 
        }

        //-----------------------------------------------------------------------------

        private List<string> _shiftLst;
        public List<string> ShiftLst
        {
            get => _shiftLst; 
            set => SetProperty(ref _shiftLst, value); 
        }

        private int _shiftIndex = 0;
        public int ShiftIndex
        {
            get => _shiftIndex; 
            set 
            { 
                SetProperty(ref _shiftIndex, value);
                if(value > -1)
                    ReportsModel.SelectedShift = value;
            }
        }

        
        private bool _shiftActive;
        public bool ShiftActive
        {
            get => _shiftActive; 
            set
            {
                SetProperty(ref _shiftActive, value);
                ShiftsVisible = (value) ? "1" : ".1";
            }
        }

        private string _shiftsVisible;
        public string ShiftsVisible
        {
            get => _shiftsVisible; 
            set => SetProperty(ref _shiftsVisible, value); 
        }

        #region Shift One 

        private static string GetShiftOneStatus()
        { 
            return (Settings.Default.bShiftOneCheck)? "ON": "OFF"; 
        }

        private bool _shiftOneRepGraphActive;
        public bool ShiftOneRepGraphActive
        {
            get => _shiftOneRepGraphActive; 
            set => SetProperty(ref _shiftOneRepGraphActive, value); 
        }


        private string _shiftOneRepStatus = GetShiftOneStatus();
        public string ShiftOneRepStatus
        {
            get => _shiftOneRepStatus; 
            set => SetProperty(ref _shiftOneRepStatus, value); 
        }

        private bool _shiftOneCheck;
        public bool ShiftOneCheck
        {
            get => _shiftOneCheck; 
            set
            {
                SetProperty(ref _shiftOneCheck, value);
                ShiftOneRepShow = (value) ? Visibility.Visible : Visibility.Hidden; 
            }
        }

        private Visibility _shiftOneRepShow;
        public Visibility ShiftOneRepShow
        {
            get => _shiftOneRepShow;
            set => SetProperty(ref _shiftOneRepShow, value);
        }



        #endregion Shift One

        //---------------------------------------------

        #region Shift Two

        

        private bool _shiftTwoRepGraphActive;
        public bool ShiftTwoRepGraphActive
        {
            get => _shiftTwoRepGraphActive; 
            set => SetProperty(ref _shiftTwoRepGraphActive, value); 
        }
       

      
        private bool _shiftTwoCheck;
        public bool ShiftTwoCheck
        {
            get => _shiftTwoCheck; 
            set
            {
                SetProperty(ref _shiftTwoCheck, value);
                ShiftTwoRepShow = (value) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private Visibility _shiftTwoRepShow;
        public Visibility ShiftTwoRepShow
        {
            get => _shiftTwoRepShow;
            set => SetProperty(ref _shiftTwoRepShow, value);
        }

        #endregion Shift Two

        //---------------------------------------------

        #region Shift Three

        private static string GetShiftThreeStatus()
        {
            return (Settings.Default.bShiftThreeCheck) ? "ON": "OFF"; 
        }

        private bool _shiftThreeRepGraphActive;
        public bool ShiftThreeRepGraphActive
        {
            get => _shiftThreeRepGraphActive; 
            set => SetProperty(ref _shiftThreeRepGraphActive, value); 
        }
        

        private bool _shiftThreeCheck;
        public bool ShiftThreeCheck
        {
            get { return _shiftThreeCheck; }
            set
            {
                SetProperty(ref _shiftThreeCheck, value);
                ShiftThreeRepShow = (value) ? Visibility.Visible : Visibility.Hidden;
            }
        }
        private Visibility _shiftThreeRepShow;
        public Visibility ShiftThreeRepShow
        {
            get => _shiftThreeRepShow;
            set => SetProperty(ref _shiftThreeRepShow, value);
        }



        #endregion  Shift Three

        private bool _periodRepCheck;
        public bool PeriodRepCheck
        {
            get => _periodRepCheck; 
            set 
            {
                SetProperty(ref _periodRepCheck, value);
                PeriodRepShow = (value) ? Visibility.Visible : Visibility.Hidden;
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
            get => _periodRepStatus; 
            set => SetProperty(ref _periodRepStatus, value); 
        }

    }
}
