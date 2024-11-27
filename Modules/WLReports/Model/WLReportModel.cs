using ClosedXML.Excel;
using Prism.Events;
using Prism.Mvvm;
using RTRep.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using WLReports.Properties;
using static RTRep.Services.ClsApplicationService;


namespace WLReports.Model
{
    public class WLReportModel
    {
       // private bool BDebug = true;

        private IEventAggregator _eventAggregator;
        private ClsSQLhandler SqlHandler = ClsSQLhandler.Instance;
        private ServiceEventsTimer WLReportTimer;
        private CsvFileService FileService = CsvFileService.Instance;

        public int LayerCount = 0;
        public int RepSelect;
     
        private string DayEndHrMn;
        private DateTime PrevProdDayEnd;
        private DateTime CurrProdDayEnd;
        private DateTime NextProdDayEnd;
        private DateTime ShiftOneEnd;
        private DateTime ShiftTwoEnd;
        private DateTime ShiftThreeEnd;
        private DateTime ShiftTwotoThreeEnd;
        private int DefaultCount = 16;
        private string strLayer;
        public DateTime DayEndTime { get; set; }
        private string CurrWlBaleTable { get; set; }

        private string _hourStart;
        public string HourStart
        {
            get => _hourStart; 
            set => _hourStart = value; 
        }

        private string _hourEnd;
        public string HourEnd
        {
            get => _hourEnd; 
            set => _hourEnd = value; 
        }

        private string _txtWLStatus;
        public string txtWLStatus
        {
            get => _txtWLStatus; 
            set => _txtWLStatus = value; 
        }

        private string _strTableName;
        public string StrTableName
        {
            get => _strTableName;
            set => _strTableName = value;
        }

        private DataTable _wetlayerDataTable;
        public DataTable WetLayerDataTable
        {
            get => _wetlayerDataTable; 
            set => _wetlayerDataTable = value; 
        }

        private string _CSVTextMsg;
        public string CSVTextMsg
        {
            get => _CSVTextMsg; 
            set => _CSVTextMsg = value; 
        }

       

        public string strFileLocation
        {
            get { return ClsCommon.ExCsvFileLocation; }
        }

        private string _strFileName;
        public string StrFileName
        {
            get => _strFileName;
            set => _strFileName = value;
        }

        private bool _bExcelOut;
        public bool BExcelOut
        {
            get => _bExcelOut;
            set => _bExcelOut = value;
        }

        public bool WLReportScanOn
        {
            get { return Settings.Default.WLScanTimerOn; }
            set
            {
                Settings.Default.WLScanTimerOn = value;
                Settings.Default.Save();

                if (value)
                {
                    SetReportTimeEvents();
                    WLReportTimer.StartWLReportTimer("WLReportEvents");
                    ClsSerilog.LogMessage(ClsSerilog.Info, $"WL Event Timer on ");
                }

                else
                {
                    WLReportTimer.StopBaleReportTimer();
                    ClsSerilog.LogMessage(ClsSerilog.Info, $"WL Event Timer off ");
                }
            }
        }

        #region Day Report ----------------------------------------------------------

        public bool DayRepAuto
        {
            get => Settings.Default.bDayRepCheck; 
            set
            {
                Settings.Default.bDayRepCheck = value;
                Settings.Default.Save();
            }
        }

        private DateTime _dayStart;
        public DateTime DayStart
        {
            get => _dayStart; 
            set => _dayStart = value; 
        }

        private DateTime _dayEnd;
        public DateTime DayEnd
        {
            get => _dayEnd; 
            set => _dayEnd = value; 
        }

        public DateTime ProdDayEnd
        {
            get => ClsCommon.ProdDayEnd; 
            set => ClsCommon.ProdDayEnd = value; 
        }
        private string dayReportString;
        public string DayReportString
        {
            get => dayReportString; 
            set => dayReportString = value; 
        }

        #endregion  Day Report


        #region Hour Report -------------------------------------------------

        public bool HourRepAuto
        {
            get => Settings.Default.bHourRepCheck; 
            set
            {
                Settings.Default.bHourRepCheck = value;
                Settings.Default.Save();
            }
        }

        #endregion Hour Report

        private string ShiftOneHrMn;
        private string ShiftTwoHrMn;
        private string ShiftThreeHrMn;

        #region Shift One ----------------------------------------------------

        public bool ShiftOneRepAuto
        {
            get => Settings.Default.bShiftOneCheck; 
            set
            {
                Settings.Default.bShiftOneCheck = value;
                Settings.Default.Save();
            }
        }

        #endregion Shift One

        #region Shift Two

        public bool ShiftTwoRepAuto
        {
            get { return Settings.Default.bShiftTwoCheck; }
            set
            {
                Settings.Default.bShiftTwoCheck = value;
                Settings.Default.Save();
            }
        }

        #endregion Shift Two

        #region Shift Three

        public bool ShiftThreeRepAuto
        {
            get => Settings.Default.bShiftThreeCheck; 
            set
            {
                Settings.Default.bShiftThreeCheck = value;
                Settings.Default.Save();
            }
        }

        public bool PeriodRepCheck
        {
            get { return Settings.Default.PeriodRepCheck; }
            set
            {
                Settings.Default.PeriodRepCheck = value;
                Settings.Default.Save();
            }
        }

        private DateTime _selectHourTimeVal;
        public DateTime SelectHourTimeVal
        {
            get => _selectHourTimeVal;
            set => _selectHourTimeVal = value;
        }


        #endregion Shift Three


        public struct CALC_RESULTS
        {
            public long BaleID;
            public int iBalerID;
            public string strBaler; //*10
            public double dDeviation;
            public double dAverage;
            public double dMaxValue;
            public double dMinValue;
            public int iNumbOfSpots;
            public string strResult; //*10
            public int[] iVals;
            public int iSize;
            public double[] dCalcResults;
            public List<double> dLayers;
            public int iLayers;
            public double dMoisture;
            public bool bAlarm;
            public bool bTCStampsAssigned;
        };

        public WLReportModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;

            ClsCommon.ScanRateWl = 60; //Set Scan Timer to 60 Sec.
            WLReportTimer = new ServiceEventsTimer(_eventAggregator);
            SetReportTimeEvents();
            PeriodRepCheck = Settings.Default.PeriodRepCheck;

            _eventAggregator.GetEvent<UpdateWLRepTimerEvents>().Subscribe(UpdateWLRealTimeEvent);
        }


        /// <summary>
        /// RealTime Events
        /// </summary>
        /// <param name="timeNow"></param>
        private void UpdateWLRealTimeEvent(DateTime timeNow)
        {
            string MinNow = timeNow.ToString("mm");
            string HourNow = timeNow.ToString("HH:mm");
            string DateNow = timeNow.ToString("dd");
            string LiveRepQuery = string.Empty;
            string ReportTitle = string.Empty;
            string CheckItems = string.Empty;
            //bool PrintReady = false;

            //Start should be before End
            if (ClsCommon.HourStart > ClsCommon.HourEnd)
            {
                ClsCommon.HourStart = ClsCommon.HourEnd.AddHours(-1);
            }

            var PerHrStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ClsCommon.HourStart.Hour, ClsCommon.HourStart.Minute, 00);
            var PerHrEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ClsCommon.HourEnd.Hour, ClsCommon.HourEnd.Minute, 00);

            HourStart = ClsCommon.HourStart.ToString("HH:mm");
            HourEnd = ClsCommon.HourEnd.ToString("HH:mm");

            Console.WriteLine("WL Time =  " + timeNow.ToString("HH:mm") + " MinNow = " + MinNow);

            try
            {
                //Day Report AND Report from yesterday to today
                if ((DayRepAuto) & (HourNow == DayEndHrMn))
                {
                    StrFileName = "WL Day Report " + DateTime.Now.ToString("M_dd_yyyy_HH_mm");
                    RepSelect = ClsCommon.DayReport;

                    WetLayerDataTable = GetWlDataTableByDay(timeNow.ToString());
                    _eventAggregator.GetEvent<UpdateWLDataEvents>().Publish(true);
                }
                //Hour Report
                if ((HourRepAuto) & (MinNow == "00"))
                {
                    StrFileName = "WL Hour Report " + DateTime.Now.ToString("M_dd_yyyy_HH_mm");
                    RepSelect = ClsCommon.HourReport;
                    WetLayerDataTable = GetWlDataTableByHour(timeNow);
                    _eventAggregator.GetEvent<UpdateWLDataEvents>().Publish(true);
                }
                //Shift one
                if ((ShiftOneRepAuto) & (HourNow == ShiftOneHrMn))
                {
                    StrFileName = "WL ShiftOne Report " + DateTime.Now.ToString("M_dd_yyyy_HH_mm");
                    RepSelect = ClsCommon.ShiftOneReport;
                    WetLayerDataTable = GetWlDataTableByShift(2);
                    _eventAggregator.GetEvent<UpdateWLDataEvents>().Publish(true);
                }
                //Shift two
                if ((ShiftTwoRepAuto) & (HourNow == ShiftTwoHrMn))
                {
                    StrFileName = "WL ShiftTwo_Report " + DateTime.Now.ToString("M_dd_yyyy_HH_mm");
                    RepSelect = ClsCommon.ShiftTwoReport;
                    WetLayerDataTable = GetWlDataTableByShift(3);
                    _eventAggregator.GetEvent<UpdateWLDataEvents>().Publish(true);
                }
                //Shift three
                if ((ShiftThreeRepAuto) & (HourNow == ShiftThreeHrMn))
                {
                    StrFileName = "WL ShiftThree Report " + DateTime.Now.ToString("M_dd_yyyy_HH_mm");
                    RepSelect = ClsCommon.ShiftThreeReport;
                    WetLayerDataTable = GetWlDataTableByShift(4);
                    _eventAggregator.GetEvent<UpdateWLDataEvents>().Publish(true);
                }

                //PeriodReport
                if ((Settings.Default.PeriodRepCheck) & (HourNow == HourEnd))
                {
                    StrFileName = "WL Period Report " + DateTime.Now.ToString("M_dd_yyyy_HH_mm");
                    RepSelect = ClsCommon.PeriodReport;
                    WetLayerDataTable = GetWlDataTableByPeriod(PerHrStart, PerHrEnd);
                    _eventAggregator.GetEvent<UpdateWLDataEvents>().Publish(true);
                }
            }
            catch (Exception ex )
            {
                System.Windows.MessageBox.Show($"ERROR in WLReportModel UpdateWLRealTimeEvent {ex.Message}");
            }
        }

        /// <summary>
        /// Demand Mode
        /// date is selectable via Day picker daySelect
        /// </summary>
        /// <param name="selectedMode"></param>
        /// <param name="daySelect"></param>
        /// <param name="now"></param>
        /// <param name="excelCSVReport"></param>
        internal void GetDataForWlDemandMode(int selectedMode, DateTime? daySelect, DateTime now, int excelCSVReport)
        {
            DateTime timeNow = DateTime.Now;
            DateTime Datesel = (DateTime)daySelect;
            var SelDay = Datesel.ToString("dd");

            CurrWlBaleTable = $"FValueReadings{Datesel.ToString("MMMy")}";
            var PreWlBaleTable = $"FValueReadings{Datesel.AddMonths(-1).ToString("MMMy")}";

            string MinNow = timeNow.ToString("mm");
            string HourNow = timeNow.ToString("HH:mm");
            string TimeFrame = string.Empty;
            string ReportTitle = string.Empty;

            string ArchiveTable = SqlHandler.GetCurrentBaleTable();

            DayEndTime = ClsCommon.ProdDayEnd;
            DayEndHrMn = DayEndTime.ToString("HH:mm");

            ShiftOneHrMn = ClsCommon.ShiftOneEnd.ToString("HH:mm");
            ShiftTwoHrMn = ClsCommon.ShiftTwoEnd.ToString("HH:mm");
            ShiftThreeHrMn = ClsCommon.ShiftThreeEnd.ToString("HH:mm");

            var PerHrStart = new DateTime(daySelect.Value.Year, daySelect.Value.Month,
               daySelect.Value.Day, ClsCommon.HourStart.Hour, ClsCommon.HourStart.Minute, 00);

            var PerHrEnd = new DateTime(daySelect.Value.Year, daySelect.Value.Month,
                daySelect.Value.Day, ClsCommon.HourEnd.Hour, ClsCommon.HourEnd.Minute, 00);

            HourStart = ClsCommon.HourStart.ToString("HH:mm");
            HourEnd = ClsCommon.HourEnd.ToString("HH:mm");

            var d3 = new DateTime(Datesel.Year, Datesel.Month, Datesel.Day, DayEndTime.Hour, DayEndTime.Minute, 00);
            CurrProdDayEnd = d3;
            PrevProdDayEnd = d3.AddDays(-1);
            NextProdDayEnd = d3.AddDays(1);

            DayStart = PrevProdDayEnd.AddSeconds(1); //Not include the first bale on the hour
            DayEnd = CurrProdDayEnd.AddSeconds(1);   //include the last bale on the hour

            ShiftOneEnd = new DateTime(d3.Year, d3.Month, d3.Day, ClsCommon.ShiftOneEnd.Hour, ClsCommon.ShiftOneEnd.Minute, 00);
            ShiftTwoEnd = new DateTime(d3.Year, d3.Month, d3.Day, ClsCommon.ShiftTwoEnd.Hour, ClsCommon.ShiftTwoEnd.Minute, 00);

            ShiftTwotoThreeEnd = new DateTime(PrevProdDayEnd.Year, PrevProdDayEnd.Month,
                PrevProdDayEnd.Day, ClsCommon.ShiftTwoEnd.Hour, ClsCommon.ShiftTwoEnd.Minute, 00);
            ShiftThreeEnd = new DateTime(d3.Year, d3.Month, d3.Day, ClsCommon.ShiftThreeEnd.Hour, ClsCommon.ShiftThreeEnd.Minute, 00);

            if (selectedMode > -1)
            {
                switch (selectedMode)
                {
                    case 0: //Day
                        RepSelect = ClsCommon.DayReport;
                        StrFileName = $"WL Day Report {Datesel.ToString(" MM_dd_yyyy")}";
                        ReportTitle = "Wl Day Report";
                        TimeFrame = " WHERE (ReadTime BETWEEN '" + DayStart.AddDays(1).AddSeconds(1).ToString("MM/dd/yyyy HH:mm") +
                                    "' and '" + DayEnd.AddDays(1).AddSeconds(1).ToString("MM/dd/yyyy HH:mm") + "') "; 
                        break;

                    case 1: //Hour
                        RepSelect = ClsCommon.HourReport;
                        ReportTitle = "WL Hour Report";
                        var SelectedTime = new DateTime(Datesel.Year, Datesel.Month, 
                            Datesel.Day, SelectHourTimeVal.Hour, SelectHourTimeVal.Minute, 00);
                        StrFileName = $"WL Hour Report {SelectedTime.ToString(" MM_dd_yyyy HH")}";
                        TimeFrame = " WHERE (ReadTime BETWEEN '" +
                                        SelectedTime.AddHours(-1).AddSeconds(1).ToString("MM/dd/yyyy HH:mm") +
                                       "' and '" + SelectedTime.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") + "') ";
                        break;

                    case 2: //ShiftOne
                        RepSelect = ClsCommon.ShiftOneReport;
                        StrFileName = $"WL ShiftOne {Datesel.ToString(" MM_dd_yyyy")}";
                        ReportTitle = "WL ShiftOne Report";
                        TimeFrame = " WHERE (ReadTime BETWEEN '" + ShiftThreeEnd.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") +
                                       "' and '" + ShiftOneEnd.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") + "') ";
                        break;

                    case 3: //ShiftTwo
                        RepSelect = ClsCommon.ShiftTwoReport;
                        StrFileName = $"WL ShiftTwo {Datesel.ToString(" MM_dd_yyyy")}";
                        ReportTitle = "WL ShiftTwo Report";
                        TimeFrame = " WHERE (ReadTime BETWEEN '" + ShiftOneEnd.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") +
                                        "' and '" + ShiftTwoEnd.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") + "') ";
                        break;

                    case 4: //ShiftThree
                        RepSelect = ClsCommon.ShiftThreeReport;
                        StrFileName = $"WL ShiftThree {Datesel.ToString(" MM_dd_yyyy")}"; 
                        ReportTitle = "WL ShiftThree Report";
                        TimeFrame = " WHERE (ReadTime BETWEEN '" + ShiftTwoEnd.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") +
                                        "' and '" + ShiftThreeEnd.AddDays(1).AddSeconds(1).ToString("MM/dd/yyyy HH:mm") + "') ";
                        break;

                    case 5: //Period
                        RepSelect = ClsCommon.ShiftThreeReport;
                        StrFileName = $"WL Period Report {Datesel.ToString(" MM_dd_yyyy")}"; 
                        ReportTitle = "WL Period Report";
                        TimeFrame = " WHERE (ReadTime BETWEEN '" + PerHrStart.ToString("MM/dd/yyyy HH:mm") +
                                       "' and '" + PerHrEnd.ToString("MM/dd/yyyy HH:mm") + "') ";
                        break;
                }

                string strquery = $"SELECT * From [ForteLayer].[dbo].[{CurrWlBaleTable}] with (NOLOCK) {TimeFrame} And Samples > 0;";
                WetLayerDataTable = GetWlDataTable(strquery);
            }
        }

        internal DataTable GetWlDataTableByDay(string TimeString)
        {
            string Today = DateTime.Now.Date.ToString("dd");

            DayEndTime = ClsCommon.ProdDayEnd;
            DayEndHrMn = DayEndTime.ToString("HH:mm");
            var d3 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DayEndTime.Hour, DayEndTime.Minute, 00);
            CurrProdDayEnd = d3;
            PrevProdDayEnd = d3.AddDays(-1);
            NextProdDayEnd = d3.AddDays(1);

            DayStart = PrevProdDayEnd.AddSeconds(1);
            DayEnd = CurrProdDayEnd.AddSeconds(1);

            StrTableName = (Today == "01") ? SqlHandler.GetPreMonth() : SqlHandler.GetWLCurrMonth();

            string strQuery = "SELECT * FROM " + StrTableName + " with (NOLOCK) WHERE (ReadTime BETWEEN '" +
                                DayStart.ToString("MM/dd/yyyy HH:mm") +
                                "' AND '" + DayEnd.ToString("MM/dd/yyyy HH:mm") + "')";
            return GetWlDataTable(strQuery);
        }

        private DataTable GetWlDataTableByPeriod(DateTime perHrStart, DateTime perHrEnd)
        {
            string TableName = SqlHandler.GetWLCurrMonth();
            string strQuery = "SELECT * FROM " + TableName + " with (NOLOCK) WHERE (ReadTime BETWEEN '" +
                perHrStart.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") + "' AND '" + perHrEnd.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") + "')";
            return GetWlDataTable(strQuery);
        }

        internal DataTable GetWlDataTableByHour(DateTime HourStart)
        {
            DayStart = HourStart.AddHours(-1).AddSeconds(1);
            DayEnd = HourStart.AddSeconds(1);
            string TableName = SqlHandler.GetWLCurrMonth();
            string strQuery = "SELECT * FROM " + TableName + " with (NOLOCK) WHERE (ReadTime BETWEEN '" +
                DayStart.ToString("MM/dd/yyyy HH:mm") + "' AND '" + DayEnd.ToString("MM/dd/yyyy HH:mm") + "')";
            return GetWlDataTable(strQuery);
        }

        internal DataTable GetWlDataTableByShift(int selectedMode)
        {
            var d3 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DayEndTime.Hour, DayEndTime.Minute, 00);
            CurrProdDayEnd = d3;
            PrevProdDayEnd = d3.AddDays(-1);
            NextProdDayEnd = d3.AddDays(1);

            var ShiftOneEnd = new DateTime(d3.Year, d3.Month, d3.Day, ClsCommon.ShiftOneEnd.Hour, ClsCommon.ShiftOneEnd.Minute, 00);
            var ShiftTwoEnd = new DateTime(d3.Year, d3.Month, d3.Day, ClsCommon.ShiftTwoEnd.Hour, ClsCommon.ShiftTwoEnd.Minute, 00);
            var ShiftThreeEnd = new DateTime(d3.Year, d3.Month, d3.Day, ClsCommon.ShiftThreeEnd.Hour, ClsCommon.ShiftThreeEnd.Minute, 00);
            ShiftTwotoThreeEnd = new DateTime(PrevProdDayEnd.Year, PrevProdDayEnd.Month, PrevProdDayEnd.Day, ClsCommon.ShiftTwoEnd.Hour, 
                ClsCommon.ShiftTwoEnd.Minute, 00);

            switch (selectedMode)
            {
                case 2://ShiftOne
                    DayStart = ShiftThreeEnd.AddSeconds(1);
                    DayEnd = ShiftOneEnd.AddSeconds(1);
                   // if (BDebug) ClsCommon.MyInfoLog.LogMessage(MsgTypes.INFO, MsgSources.EVENTNOTIFY, "WL Shift One Report ...");
                    break;

                case 3://ShiftTwo
                    DayStart = ShiftOneEnd.AddSeconds(1);
                    DayEnd = ShiftTwoEnd.AddSeconds(1);
                    //if (BDebug) ClsCommon.MyInfoLog.LogMessage(MsgTypes.INFO, MsgSources.EVENTNOTIFY, "WL Shift Two Report ...");
                    break;

                case 4://ShiftThree
                    DayStart = ShiftTwotoThreeEnd.AddSeconds(1);
                    DayEnd = ShiftThreeEnd.AddSeconds(1);
                    //if (BDebug) ClsCommon.MyInfoLog.LogMessage(MsgTypes.INFO, MsgSources.EVENTNOTIFY, "WL Shift Three Report ...");
                    break;
            }
            string TableName = SqlHandler.GetWLCurrMonth();
            string strQuery = "SELECT * FROM " + TableName + " with (NOLOCK) WHERE (ReadTime BETWEEN '" + 
                DayStart.ToString("MM/dd/yyyy HH:mm") + "' AND '" + DayEnd.ToString("MM/dd/yyyy HH:mm") + "')";
            return GetWlDataTable(strQuery);
        }

        private DataTable GetWlDataTable(string strQuery)
        {

            DataTable MyWLDat = new DataTable();
            try
            {
                MyWLDat = SqlHandler.GetNewWLDataTable(StrTableName, strQuery);

                if (MyWLDat.Rows.Count > 0)
                {
                    ProccessDataByDay(MyWLDat); //Produce WetLayerDataTable
                    if (MyWLDat.Columns.Contains("Title"))
                        MyWLDat.Columns.Remove("Title");

                    if(ClsCommon.BExcelOut)
                    {
                        //WriteWLExcelFile(WetLayerDataTable, StrFileName);
                        _=WriteWLExcelFileAsync(WetLayerDataTable, StrFileName);
                        CSVTextMsg = $"Wrote Excel File {StrFileName}";
                    }
                    else
                    {
                        FileService.WriteWlCSVFile(WetLayerDataTable, StrFileName);
                        CSVTextMsg = $"Wrote CSV File {StrFileName}";// FileService.ExCsvTextMsg;
                    }
                }
                else
                {
                    CSVTextMsg = $"No Data Found!";
                    _eventAggregator.GetEvent<UpdateWLDataEvents>().Publish(false);
                }

                //incase using the Production time, will change date after midnight to dayend time to previous day.
                /*
                if(ClsCommon.UsedProdTime)
                {
                    for (int i = 0; i < MyWLDat.Rows.Count; i++)
                    {
                        MyWLDat.Rows[i]["ReadTime"] = GetWlProdDate(MyWLDat.Rows[i].Field<System.DateTime>("ReadTime"));
                    }
                }*/
                
            }
            catch (Exception ex)
            {
                CSVTextMsg = $"ERROR GetWlDataTable {ex}";
                //System.Windows.MessageBox.Show($"ERROR in WLReportModel GetWlDataTable  {ex.Message}");
            }
            return MyWLDat;
        }

        public void WriteWLExcelFile(System.Data.DataTable Mydatatable, string StrFileName)
        {

            int rowcount = Mydatatable.Rows.Count;
            List<string> hdrList = new List<string>();
            string ExCsvTextMsg;

            string StrPathFile = ClsCommon.ExCsvFileLocationWL + "\\" + StrFileName + ".xlsx";
            foreach (DataColumn column in Mydatatable.Columns)
            {
                if (column.ColumnName == "Deviation")
                    column.ColumnName = "%CV";
                else if (column.ColumnName == "Param1")
                    column.ColumnName = "MAX";
                else if (column.ColumnName == "Param2")
                    column.ColumnName = "MIN";
                else if (column.ColumnName == "Moisture")
                    column.ColumnName = "%MR";
                Mydatatable.AcceptChanges();
            }
            try
            {
                using (XLWorkbook wlworkbook = new XLWorkbook())
                {
                    wlworkbook.Worksheets.Add(Mydatatable, "ForteLayer");
                    if (!System.IO.File.Exists(StrPathFile))
                    {
                        wlworkbook.SaveAs(StrPathFile);
                    }
                    else
                    {
                        ExCsvTextMsg = "Can not Write RT Excel " + StrPathFile + "  " + DateTime.Now.Date.ToString("MM/dd/yyyy");
                    }
                }
                ExCsvTextMsg = "Can not Write RT Excel, No Data for" + StrPathFile + "  " + DateTime.Now.Date.ToString("MM/dd/yyyy");
            }
            catch (Exception ex)
            {
                ClsSerilog.LogMessage(ClsSerilog.Error, $" ERROR in CsvFileService WriteWLExcelFile {ex.Message}");
            }
        }



        public async Task WriteWLExcelFileAsync(System.Data.DataTable Mydatatable, string StrFileName)
        {

            int rowcount = Mydatatable.Rows.Count;
            List<string> hdrList = new List<string>();
            string ExCsvTextMsg;

            string StrPathFile = ClsCommon.ExCsvFileLocationWL + "\\" + StrFileName + ".xlsx";
            foreach (DataColumn column in Mydatatable.Columns)
            {
                if (column.ColumnName == "Deviation")
                    column.ColumnName = "%CV";
                else if (column.ColumnName == "Param1")
                    column.ColumnName = "MAX";
                else if (column.ColumnName == "Param2")
                    column.ColumnName = "MIN";
                else if (column.ColumnName == "Moisture")
                    column.ColumnName = "%MR";
                Mydatatable.AcceptChanges();
            }
            try
            {
                await Task.Run(() =>
                {
                    using (XLWorkbook wlworkbook = new XLWorkbook())
                    {
                        wlworkbook.Worksheets.Add(Mydatatable, "ForteLayer");
                        if (!System.IO.File.Exists(StrPathFile))
                        {
                            wlworkbook.SaveAs(StrPathFile);
                        }
                        else
                        {
                            ExCsvTextMsg = "Can not Write RT Excel " + StrPathFile + "  " + DateTime.Now.Date.ToString("MM/dd/yyyy");
                            ClsSerilog.LogMessage(ClsSerilog.Error, $" {ExCsvTextMsg}");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ClsSerilog.LogMessage(ClsSerilog.Error, $" ERROR in CsvFileService WriteWLExcelFile {ex.Message}");
            }
        }

        private object GetWlProdDate(DateTime dateTime)
        {
            DateTime RetDate = dateTime;
            var MidNight = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 00, 00, 00);
            var DatEnd = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, ClsCommon.ProdDayEnd.Hour, ClsCommon.ProdDayEnd.Minute, 00);

            var AfterMidNight = new DateTime(dateTime.Year, dateTime.Month, dateTime.AddDays(-1).Day, dateTime.Hour, dateTime.Minute, dateTime.Second);

            if (ClsCommon.UsedProdTime)
            {
                if ((dateTime >= MidNight) & (dateTime <= DatEnd))
                {
                    RetDate = AfterMidNight;
                }
            }
            return RetDate;
        }

        private void ProccessDataByDay(DataTable MyTable)
        {
            List<double> fLayerVal;
            List<Tuple<int, string, double>> MyCVListX;

            try
            {
                if (MyTable.Rows.Count > 0)
                {
                    MyCVListX = new List<Tuple<int, string, double>>();

                    if (MyTable.Rows[0]["Layers"] == null)
                        LayerCount = DefaultCount;
                    else
                        LayerCount = 16; // MyTable.Rows[0].Field<int>("Layers");

                    WetLayerDataTable = new DataTable();
                    //Added for graph title
                    MyTable.Columns.Add("Title", typeof(string));

                    foreach (DataRow dtRow in MyTable.Rows)
                    {
                        CALC_RESULTS StructLast = new CALC_RESULTS();
                        fLayerVal = new List<double>();

                        for (int i = 1; i < LayerCount + 1; i++)
                        {
                            strLayer = "Layer" + i.ToString();
                            var field1 = dtRow[strLayer].ToString();
                            if (ClsCommon.MoistureType == 1)
                                fLayerVal.Add(ConvToMR(Convert.ToDouble(dtRow[strLayer].ToString())));
                            else
                                fLayerVal.Add(Convert.ToDouble(dtRow[strLayer].ToString()));
                        }

                        CalCVMinMax(fLayerVal, LayerCount, out StructLast);

                        if (ClsCommon.MoistureType == 1)
                            dtRow["Moisture"] = ConvToMR(Convert.ToDouble(dtRow["Moisture"])).ToString("#0.00");
                        else
                            dtRow["Moisture"] = Convert.ToDouble(dtRow["Moisture"]).ToString("#0.00");
                        dtRow["Deviation"] = StructLast.dDeviation.ToString("#0.00");
                        dtRow["Param1"] = StructLast.dMaxValue.ToString("#0.00");
                        dtRow["Param2"] = StructLast.dMinValue.ToString("#0.00");

                        //For CV Graph data list of Tuple<int, string, double>
                        MyCVListX.Add(new Tuple<int, string, double>(Convert.ToInt32(dtRow["ID"].ToString()),
                            dtRow["ReadTime"].ToString(), Convert.ToDouble(dtRow["Deviation"].ToString())));

                        for (int i = 1; i < LayerCount + 1; i++)
                        {
                            strLayer = "Layer" + i.ToString();
                            dtRow[strLayer] = StructLast.dLayers[i - 1].ToString("#0.00");
                        }

                        //Graph Title
                        dtRow["Title"] = dtRow["ReadTime"] + " - Baler " + dtRow["BalerID"] + ", Number - " + dtRow["ID"];
                    }

                    MyTable.Columns.Remove("Time1");
                    MyTable.Columns.Remove("Time2");
                    MyTable.Columns.Remove("Time3");
                    MyTable.Columns.Remove("MaximumPrc");
                    MyTable.Columns.Remove("Layer17");
                    MyTable.Columns.Remove("Layer18");
                    MyTable.Columns.Remove("Layer19");
                    MyTable.Columns.Remove("Layer20");
                    MyTable.Columns.Remove("Layer21");
                    MyTable.Columns.Remove("Layer22");
                    MyTable.Columns.Remove("Layer23");
                    MyTable.Columns.Remove("Layer24");
                    MyTable.Columns.Remove("Layer25");
                    MyTable.Columns.Remove("Layer26");
                    MyTable.Columns.Remove("Layer27");
                    MyTable.Columns.Remove("Layer28");
                    MyTable.Columns.Remove("Layer29");
                    MyTable.Columns.Remove("Layer30");
                    MyTable.Columns.Remove("Layers");


                    if (MyTable.Columns.Contains("Moisture"))
                    {
                        if (ClsCommon.MoistureType == 0)
                        {
                            MyTable.Columns["Moisture"].ColumnName = "MC %";
                        }
                        else if (ClsCommon.MoistureType == 1)
                        {
                            MyTable.Columns["Moisture"].ColumnName = "MR %";
                        }
                        MyTable.AcceptChanges();
                    }
                    WetLayerDataTable = MyTable;
                }
                else
                    txtWLStatus = "! - No Data Found for this table - !";

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"ERROR in WLReportModel ProccessDataByDay {ex.Message}");
            }
        }

        internal string GetWlCurTableName()
        {
            return SqlHandler.GetCurrentBaleTable();
        }

        internal DataTable GetWlDataTableByDay()
        {
            return WetLayerDataTable;
        }

        internal string GetstrPeriod()
        {
            HourStart = ClsCommon.HourStart.ToString("HH:mm");
            HourEnd = ClsCommon.HourEnd.ToString("HH:mm");
            return HourStart + "-" + HourEnd;
        }

        private string QuoteValue(string value)
        {
            return string.Concat("" + value + "");
        }

        private double ConvToMR(double fMoisture)
        {
            return (fMoisture / (1 - fMoisture / 100));
        }

        public void SetReportTimeEvents()
        {
            DayEndTime = ClsCommon.ProdDayEnd;
            ShiftOneHrMn = ClsCommon.ShiftOneEnd.ToString("HH:mm");
            ShiftTwoHrMn = ClsCommon.ShiftTwoEnd.ToString("HH:mm");
            ShiftThreeHrMn = ClsCommon.ShiftThreeEnd.ToString("HH:mm");

            DayEndHrMn = DayEndTime.ToString("HH:mm");
            var d3 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DayEndTime.Hour, DayEndTime.Minute, 00);
            CurrProdDayEnd = d3;
            PrevProdDayEnd = d3.AddDays(-1);
            NextProdDayEnd = d3.AddDays(1);

            DayStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, Convert.ToInt32(DayStart.ToString("dd")), DayEndTime.Hour, DayEndTime.Minute, 00);
            var dEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, Convert.ToInt32(DayEnd.ToString("dd")), DayEndTime.Hour, DayEndTime.Minute, 00);
            DayEnd = dEnd.AddDays(1);

            ClsCommon.DayEndHrMn = DayEndHrMn;

            ClsCommon.CurrProdDayEnd = CurrProdDayEnd;
            ClsCommon.PrevProdDayEnd = PrevProdDayEnd;
            ClsCommon.NextProdDayEnd = NextProdDayEnd;

        }

        internal void SetDayReport(DateTime? daySelect1, DateTime? daySelect2)
        {
            DayStart = (DateTime)daySelect1;
            DayEnd = (DateTime)daySelect2;
        }

        private void CalCVMinMax(List<double> SampleList, int iLayers, out CALC_RESULTS tResults)
        {
            tResults = new CALC_RESULTS();
            double sumOfDerivation = 0;

            //Average
            tResults.dAverage = SampleList.Average();

            //Min Max
            tResults.dMinValue = SampleList.Min();
            tResults.dMaxValue = SampleList.Max();

            //MaxYAxis = SampleList.Max() + 5;

            //layers
            tResults.dLayers = new List<Double>();
            tResults.dLayers = SampleList;

            //Deviation
            tResults.bAlarm = false;
            foreach (var value in SampleList)
            {
                sumOfDerivation += (value - tResults.dAverage) * (value - tResults.dAverage);
            }

            //STD
            double Variance = sumOfDerivation / (SampleList.Count - 1);
            double StandardDeviation = Math.Sqrt(Variance);

            //%CV (Coefficient of Variation = Standard Deviation / Mean)
            tResults.dDeviation = (StandardDeviation / tResults.dAverage) * 100;
            tResults.bAlarm = false;
        }
    }

    /// <summary>
    /// Select different colors for Bar Graph
    /// </summary>
    public class ChartData : BindableBase
    {
        private int _Index;
        public int Index
        {
            get { return _Index; }
            set { SetProperty(ref _Index, value); }
        }

        private double _value;
        public double Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        private Brush _color;
        public Brush ChartColor
        {
            get { return _color; }
            set { SetProperty(ref _color, value); }
        }

    }
}
