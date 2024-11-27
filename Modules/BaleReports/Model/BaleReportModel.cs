using BaleReports.Graphs.GraphViews;
using BaleReports.Properties;
using Prism.Events;
using RTRep.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using static RTRep.Services.ClsApplicationService;
using System.Collections.ObjectModel;
using ClosedXML.Excel;
using System.IO;
using ClosedXML;
using System.Linq;
using System.Globalization;
using System.ComponentModel;
using System.Threading;

namespace BaleReports.Model
{
    public class BaleReportModel
    {
        protected readonly IEventAggregator _eventAggregator;


        private Thread newWindowThread;
        private LoadingWIndow tempWindow;

        //    private bool BDebug = false;
        private static bool Debuggy = true;
        private Xmlhandler MyXml;

        private ServiceEventsTimer BaleReportTimer;
        private ClsSQLhandler SqlHandler;
        private Window GraphWindow;
        private string DayEndHrMn;
        private CsvFileService FileService;

        private string ShiftOneHrMn;
        private string ShiftTwoHrMn;
        private string ShiftThreeHrMn;

        private DateTime PrevProdDayEnd;
        private DateTime CurrProdDayEnd;
        private DateTime NextProdDayEnd;
        private DateTime ShiftOneEnd;
        private DateTime ShiftTwoEnd;
        private DateTime ShiftThreeEnd;
        private DateTime ShiftTwotoThreeEnd;
        public DateTime DayEndTime { get; set; }
        private string CurrBaleTable { get; set; }

        public const int Daily = 0;
        public const int Hourly = 1;
        public const int Monthly = 2;
        public const int ShiftOne = 3;
        public const int ShiftTwo = 4;
        public const int ShiftThree = 5;
        public const int PeriodRep = 6;

        private DateTime DayStarted;
        private DateTime DayEnded;


        private List<string> HdrCol = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", 
            "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        public string[] RepEvents = new string[] { "Daily", "Hourly", "Monthly", "ShiftOne", "ShiftTwo", "ShiftThree", "PeriodRep" };

        public const int Moisture = 0;
        public const int Weight = 1;
        public string[] GaphType = new string[] { "Moisture", "Weight" };
        public List<int> ColOrdinal = new List<int>();

        private string baleQueryString = string.Empty;
        private string baleQueryHdrName = string.Empty;
        private ObservableCollection<SqlReportField> XMLColumnList = new ObservableCollection<SqlReportField>();
        private int BlankFieldOne { get; set; }
        private int BlankFieldTwo { get; set; }
        private int BlankFieldThree { get; set; }
        public string SqlReportitems { get; set; }
        public string OutputReportitems { get; set; }

        private string _exCsvTextMsg;
        public string ExCsvTextMsg
        {
            get => _exCsvTextMsg;
            set => _exCsvTextMsg = value;
        }

        private string _strPathFile;
        public string StrFilePatch
        {
            get => _strPathFile;
            set => _strPathFile = value;
        }

        private List<string> _shiftLst;
        public List<string> ShiftLst
        {
            get => _shiftLst;
            set => _shiftLst = value;
        }

        private string _shiftName;
        public string ShiftName
        {
            get => _shiftName;
            set => _shiftName = value;
        }

        private int _selectedShift;
        public int SelectedShift
        {
            get => _selectedShift;
            set => _selectedShift = value;
        }

        private List<string> _lineList;
        public List<string> LineList
        {
            get => _lineList;
            set => _lineList = value;
        }

        private bool _lineActive = false;
        public bool LineActive
        {
            get => _lineActive;
            set => _lineActive = value;
        }

        private List<string> _sourceList;
        public List<string> SourceList
        {
            get => _sourceList;
            set => _sourceList = value;
        }

        private bool _sourceActive = false;
        public bool SourceActive
        {
            get => _sourceActive;
            set => _sourceActive = value;
        }

        private DataTable _baleDataTable;
        public DataTable BaleDataTable
        {
            get => _baleDataTable;
            set => _baleDataTable = value;
        }

        private string _strFileName;
        public string StrFileName
        {
            get => _strFileName;
            set => _strFileName = value;
        }

        private bool RtScannerOn
        {
            set
            {
                if (value)
                {
                    BaleReportTimer.StartBaleReportTimer("BaleRealTime");
                    ClsSerilog.LogMessage(ClsSerilog.Info, $"RT Event Timer on");
                }
                else
                {
                    BaleReportTimer.StopBaleReportTimer();
                    ClsSerilog.LogMessage(ClsSerilog.Info, $"RT Event Timer off");
                }
            }
        }


        private object GetRtProdDate(DateTime dateTime)
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


        internal string GetstrPeriod()
        {
            HourStart = ClsCommon.HourStart.ToString("HH:mm");
            HourEnd = ClsCommon.HourEnd.ToString("HH:mm");

            return HourStart + "-" + HourEnd;
        }

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

        private DateTime _selectHourTimeVal;
        public DateTime SelectHourTimeVal
        {
            get => _selectHourTimeVal;
            set => _selectHourTimeVal = value;
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

        internal List<string> Getsourcelist()
        {
            return SqlHandler.Getsourcelist();
        }

        #endregion Shift One

        #region Shift Two
        public bool ShiftTwoRepAuto
        {
            get => Settings.Default.bShiftTwoCheck;
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

        #endregion Shift Three

        public bool PeriodRepCheck
        {
            get => Settings.Default.PeriodRepCheck;
            set
            {
                Settings.Default.PeriodRepCheck = value;
                Settings.Default.Save();
            }
        }

        public string strFileLocation
        {
            get { return ClsCommon.ExCsvFileLocation; }
        }

        private ObservableCollection<SqlReportField> _reportColumnList;
        public ObservableCollection<SqlReportField> ReportColumnList
        {
            get => _reportColumnList;
            set => _reportColumnList = value;
        }

        public BaleReportModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            MyXml = Xmlhandler.Instance;

            ClsCommon.ScanRate = 60; //Set Scan Timer to 60 Sec.
            BaleReportTimer = new ServiceEventsTimer(_eventAggregator);
            SqlHandler = ClsSQLhandler.Instance;
            FileService = new CsvFileService(_eventAggregator);

            RtScannerOn = ClsCommon.RtScannerOn;

            _eventAggregator.GetEvent<CloseGraphWindowEvent>().Subscribe(CloseGraphWindows);
            _eventAggregator.GetEvent<UpdateBaleRepTimerEvents>().Subscribe(UpdateRTTimerEventAsync);

            HourRepAuto = Settings.Default.bHourRepCheck;
            DayRepAuto = Settings.Default.bDayRepCheck;

            if (SqlHandler.TestSqlConnections())
            {
                CurrBaleTable = SqlHandler.GetCurrentBaleTable();
                ShiftLst = SqlHandler.GetUniqueStrItemlist("ShiftName");
                LineList = SqlHandler.GetUniquIntitemlist("LineID");
                SourceList = SqlHandler.GetUniquIntitemlist("SourceID");

                LineActive = (LineList.Count > 1) ? true : false;
                SourceActive = (SourceList.Count > 1) ? true : false;
            }

            XMLColumnList = GetXMLColumnList();
            SqlReportitems = GetReportItems();
            OutputReportitems = GetOutputReportitems();
        }

        private void CloseGraphWindows(bool obj)
        {
            if (obj)
            {
                if (GraphWindow != null)
                {
                    GraphWindow.Close();
                    GraphWindow = null;
                }
            }
        }


        /// <summary>
        /// On Demand Events ////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="selectedMode"></param>
        /// <param name="dateOrHour"></param>
        /// <param name="hourSelect"></param>
        /// <param name="outPutType"></param>
        internal void GetDataForDemandMode(int selectedMode, DateTime? dateOrHour, DateTime hourSelect, int outPutType,string source)
        {
            var usCulture = "en-US";
            DateTime timeNow = DateTime.Now;
            DateTime Datesel = (DateTime)dateOrHour;
            var SelDay = Datesel.ToString("dd");
            CurrBaleTable = $"BaleArchive{Datesel.ToString("MMMy")}";
            var PreBaleTable = $"BaleArchive{Datesel.AddMonths(-1).ToString("MMMy")}";

            var MidNight = new DateTime(Datesel.Year, Datesel.Month, Datesel.Day, 00, 00, 00);

            string MinNow = timeNow.ToString("mm");
            string HourNow = timeNow.ToString("HH:mm");
            string TimeFrame = string.Empty;
            string ReportTitle = string.Empty;
            string ArchiveTable = SqlHandler.GetCurrentBaleTable();

            string selectsource = (source != "ALL") ? $"AND SourceID = {source}":string.Empty;

            DayEndTime = (ClsCommon.UsedProdTime) ? ClsCommon.ProdDayEnd : MidNight; 
            DayEndHrMn = DayEndTime.ToString("HH:mm");

            ShiftOneHrMn = ClsCommon.ShiftOneEnd.ToString("HH:mm");
            ShiftTwoHrMn = ClsCommon.ShiftTwoEnd.ToString("HH:mm");
            ShiftThreeHrMn = ClsCommon.ShiftThreeEnd.ToString("HH:mm");

            //Start should be before End
            if (ClsCommon.HourStart > ClsCommon.HourEnd)
            {
                ClsCommon.HourStart = ClsCommon.HourEnd.AddHours(-1);
            }

            var PerHrStart = new DateTime(dateOrHour.Value.Year, dateOrHour.Value.Month,
                dateOrHour.Value.Day, ClsCommon.HourStart.Hour, ClsCommon.HourStart.Minute, 00);

            var PerHrEnd = new DateTime(dateOrHour.Value.Year, dateOrHour.Value.Month,
                dateOrHour.Value.Day, ClsCommon.HourEnd.Hour, ClsCommon.HourEnd.Minute, 00);

            HourStart = ClsCommon.HourStart.ToString("HH:mm");
            HourEnd = ClsCommon.HourEnd.ToString("HH:mm");

            var d3 = new DateTime(Datesel.Year, Datesel.Month, Datesel.Day, DayEndTime.Hour, DayEndTime.Minute, 00);
            CurrProdDayEnd = d3;
            PrevProdDayEnd = d3.AddDays(-1);
            NextProdDayEnd = d3.AddDays(1);

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
                        DayStarted = DateTime.Parse(PrevProdDayEnd.AddDays(1).AddSeconds(1).ToString(), new CultureInfo(usCulture, false));
                        DayEnded = DateTime.Parse(CurrProdDayEnd.AddDays(1).AddSeconds(1).ToString(), new CultureInfo(usCulture, false));

                        StrFileName = $"RT Day Report {Datesel.ToString(" MM_dd_yyyy")}";
                        TimeFrame = " WHERE (TimeComplete BETWEEN '" + DayStarted.ToString("MM/dd/yyyy HH:mm") +
                                        "' and '" + DayEnded.ToString("MM/dd/yyyy HH:mm") + "') ";
                        ReportTitle = "RT Day Report";
                        break;

                    case 1: //Hour
                        var SelectedTime = new DateTime(Datesel.Year, Datesel.Month, Datesel.Day, SelectHourTimeVal.Hour, SelectHourTimeVal.Minute, 00);
                        DayStarted = DateTime.Parse(SelectedTime.AddHours(-1).AddSeconds(1).ToString(), new CultureInfo(usCulture, false));
                        DayEnded = DateTime.Parse(SelectedTime.AddSeconds(1).ToString(), new CultureInfo(usCulture, false));

                        StrFileName = $"RT Hour Report { SelectedTime.ToString(" MM_dd_yyyy_HH")}";
                        TimeFrame = " WHERE (TimeComplete BETWEEN '" +
                        DayStarted.ToString("MM/dd/yyyy HH:mm") +
                                       "' and '" + DayEnded.ToString("MM/dd/yyyy HH:mm") + "') ";
                        ReportTitle = "RT Hour Report";
                        break;

                    case 2: //ShiftOne
                        DayStarted = DateTime.Parse(ShiftThreeEnd.AddSeconds(1).ToString(), new CultureInfo(usCulture, false));
                        DayEnded = DateTime.Parse(ShiftOneEnd.AddSeconds(1).ToString(), new CultureInfo(usCulture, false));

                        StrFileName = "RT ShiftOne Report " + Datesel.ToString(" MM_dd_yyyy");
                        TimeFrame = " WHERE (TimeComplete BETWEEN '" + DayStarted.ToString("MM/dd/yyyy HH:mm") +
                                        "' and '" + DayEnded.ToString("MM/dd/yyyy HH:mm") + "') ";
                        ReportTitle = "RT Shift One Report";
                        break;

                    case 3: //ShiftTwo from shift 1 to shift 2
                        DayStarted = DateTime.Parse(ShiftOneEnd.AddSeconds(1).ToString(), new CultureInfo(usCulture, false));
                        DayEnded = DateTime.Parse(ShiftTwoEnd.AddSeconds(1).ToString(), new CultureInfo(usCulture, false));

                        StrFileName = "RT ShiftTwo Report " + Datesel.ToString(" MM_dd_yyyy");
                        TimeFrame = " WHERE (TimeComplete BETWEEN '" + DayStarted.ToString("MM/dd/yyyy HH:mm") +
                                        "' and '" + DayEnded.ToString("MM/dd/yyyy HH:mm") + "') ";
                        ReportTitle = "RT Shift Two Report";
                        break;

                    case 4: //ShiftThree from shift 2 of previous day to shift 3 of current day
                        DayStarted = DateTime.Parse(ShiftTwoEnd.AddSeconds(1).ToString(), new CultureInfo(usCulture, false));
                        DayEnded = DateTime.Parse(ShiftThreeEnd.AddDays(1).AddSeconds(1).ToString(), new CultureInfo(usCulture, false));

                        StrFileName = "RT ShiftThree Report " + Datesel.ToString(" MM_dd_yyyy");
                        TimeFrame = " WHERE (TimeComplete BETWEEN '" + DayStarted.ToString("MM/dd/yyyy HH:mm") +
                                        "' and '" + DayEnded.ToString("MM/dd/yyyy HH:mm") + "') ";
                        ReportTitle = "RT Shift Three Report ";
                        break;

                    case 5: //Period
                        DayStarted = DateTime.Parse(PerHrStart.ToString(), new CultureInfo(usCulture, false));
                        DayEnded = DateTime.Parse(PerHrEnd.ToString(), new CultureInfo(usCulture, false));

                        StrFileName = "RT Period Report " + Datesel.ToString(" MM_dd_yyyy");
                        TimeFrame = " WHERE (TimeComplete BETWEEN '" + DayStarted.ToString("MM/dd/yyyy HH:mm") +
                                       "' and '" + DayEnded.ToString("MM/dd/yyyy HH:mm") + "') ";
                        ReportTitle = "RT Period Report ";
                        break;
                }

                string strquery = $"SELECT {SqlReportitems} From {CurrBaleTable} with (NOLOCK) {TimeFrame}  {selectsource} ORDER by UID ASC ";

                ClsCommon.RtRep = false;

                try
                {
                    using (DataTable RtDataTable = SqlHandler.GetForteDataTable(strquery, XMLColumnList, selectedMode, DayStarted, DayEnded))
                    {
                        if (RtDataTable.Rows.Count > 0)
                        {
                            if (BaleDataTable != null) BaleDataTable = null;
                            BaleDataTable = new DataTable();
                            BaleDataTable = RtDataTable;

                            if (outPutType == ClsCommon.ExcelCSVGraph)
                            {
                                GraphOneView MyGraph = new GraphOneView(ReportTitle +
                                    SelectHourTimeVal.ToString(" MM_dd_yyyy  HH") + " Hr.", _eventAggregator);
                                MyGraph.GraphViewModel.SetupGraph(RtDataTable);

                                GraphWindow = new Window
                                {
                                    Width = 1100,
                                    Height = 700,
                                    Topmost = true,
                                    Title = ReportTitle,
                                    Content = MyGraph
                                };
                                GraphWindow.WindowStyle = WindowStyle.ThreeDBorderWindow;
                                GraphWindow.ResizeMode = ResizeMode.NoResize;
                                GraphWindow.ShowDialog();
                            }
                            else if (outPutType == ClsCommon.ExcelCSVReport)
                            {
                                _ = PrintReportOnDemandAsync(StrFileName, RtDataTable);
                            }
                        }
                        else
                        {
                            ClsSerilog.LogMessage(ClsSerilog.Info, $"No Record Found for this Report!");
                            MessageBox.Show("No Record Found for this Report!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"ERROR in GetDataForDemandMode {ex.Message}");
                }
            }
            else
                MessageBox.Show("No Report Selection Selected!");
        }

        private string GetPreviousBaleTable(DateTime? dateOrHour)
        {
            return (dateOrHour.Value.Month == 1) ? ClsCommon.BaleArchMonth[11] + (dateOrHour.Value.Year - 2001).ToString() :
                ClsCommon.BaleArchMonth[dateOrHour.Value.Month - 2] + (dateOrHour.Value.Year - 2000).ToString();
        }

        private string GetReportItems()
        {
            char[] charsToTrim = { ',' };
            string returnList = string.Empty;

            List<int> blanklist = new List<int>();

            for (int i = 0; i < XMLColumnList.Count; i++)
            {
                if ((XMLColumnList[i].FieldExp.ToString() == "BlankField1") | (XMLColumnList[i].FieldExp.ToString() == "BlankField2") | (XMLColumnList[i].FieldExp.ToString() == "BlankField3"))
                {
                    blanklist.Add(i);
                }
                else
                    returnList += XMLColumnList[i].FieldExp + ',';
            }
            returnList = returnList.Trim(charsToTrim);
            return returnList;
        }

        private string GetOutputReportitems()
        {
            char[] charsToTrim = { ',' };
            string returnList = string.Empty;
            for (int i = 0; i < XMLColumnList.Count; i++)
            {
                returnList += XMLColumnList[i].FieldExp + ",";
            }
            returnList = returnList.Trim(charsToTrim);
            return returnList;
        }

        public ObservableCollection<SqlReportField> GetXMLColumnList()
        {
            Xmlhandler MyXml = Xmlhandler.Instance;
            return MyXml.ReadXMlRepColumn(ClsCommon.ReportXmlFilePath);
        }

        /// <summary>
        /// Realtime Events ----------------------------------------------------------------------------------------------------------------------------------
        /// trigger from ServiceEventsTimer
        /// more than one if events can happens at the same time.
        /// </summary>
        /// <param name="timeNow"></param>
        private void UpdateRTTimerEventAsync(DateTime timeNow)
        {
            var usCulture = "en-US";

            string MinNow = timeNow.ToString(@"mm");
            string HourNow = timeNow.ToString(@"HH:mm");
            string DayNow = timeNow.ToString("dd");
            string TimeFrame = string.Empty;
            string ReportTitle = string.Empty;

            var MidNight = new DateTime(timeNow.Year, timeNow.Month, timeNow.Day, 00, 00, 00);

            //Start should be before End
            if (ClsCommon.HourStart > ClsCommon.HourEnd)
                ClsCommon.HourStart = ClsCommon.HourEnd.AddHours(-1);

            HourStart = ClsCommon.HourStart.ToString(@"HH:mm");
            HourEnd = ClsCommon.HourEnd.ToString(@"HH:mm");
    
            ShiftOneHrMn = ClsCommon.ShiftOneEnd.ToString(@"HH:mm");
            ShiftTwoHrMn = ClsCommon.ShiftTwoEnd.ToString(@"HH:mm");
            ShiftThreeHrMn = ClsCommon.ShiftThreeEnd.ToString(@"HH:mm");

            var d3 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ClsCommon.ProdDayEnd.Hour, ClsCommon.ProdDayEnd.Minute, 00);
            CurrProdDayEnd = d3;
            PrevProdDayEnd = d3.AddDays(-1);
            NextProdDayEnd = d3.AddDays(1);

            ShiftOneEnd = new DateTime(d3.Year, d3.Month, d3.Day, ClsCommon.ShiftOneEnd.Hour, ClsCommon.ShiftOneEnd.Minute, 00);
            ShiftTwoEnd = new DateTime(d3.Year, d3.Month, d3.Day, ClsCommon.ShiftTwoEnd.Hour, ClsCommon.ShiftTwoEnd.Minute, 00);

            ShiftTwotoThreeEnd = new DateTime(PrevProdDayEnd.Year, PrevProdDayEnd.Month, PrevProdDayEnd.Day, ClsCommon.ShiftTwoEnd.Hour, ClsCommon.ShiftTwoEnd.Minute, 00);
            ShiftThreeEnd = new DateTime(d3.Year, d3.Month, d3.Day, ClsCommon.ShiftThreeEnd.Hour, ClsCommon.ShiftThreeEnd.Minute, 00);

            DayEndHrMn = (ClsCommon.UsedProdTime) ? ClsCommon.ProdDayEnd.ToString(@"HH:mm") : MidNight.ToString(@"HH:mm");

            //Day Report AND Report from this day end to previous day //////////////////////////////////////////////////////////////////////////////////
            if ((DayRepAuto) & (HourNow == DayEndHrMn))
            {
                DayStarted = DateTime.Parse(PrevProdDayEnd.AddSeconds(1).ToString(), new CultureInfo(usCulture, false));
                DayEnded = DateTime.Parse(CurrProdDayEnd.AddSeconds(1).ToString(), new CultureInfo(usCulture, false));

                TimeFrame = " WHERE (TimeComplete BETWEEN '" +
                    DayStarted.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") + "' and '" + DayEnded.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") + "') ";
                ReportTitle = $"RT Day Report {DayStarted.ToString(" MM_dd_yyyy")}";

                //if (Debuggy) ClsSerilog.LogMessage(ClsSerilog.Debug, $"Title= {ReportTitle} Query= <{TimeFrame}>  ");

                _ = PrintReportAsync(SqlReportitems, TimeFrame, ReportTitle,0);
            }

            //Hour Report////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //
            if ((HourRepAuto) & (MinNow == "00"))
            {
                DayStarted = DateTime.Parse(timeNow.AddHours(-1).ToString(), new CultureInfo(usCulture, false));
                DayEnded = DateTime.Parse(timeNow.ToString(), new CultureInfo(usCulture, false));

                TimeFrame = " WHERE (TimeComplete BETWEEN '" +
                    timeNow.AddHours(-1).AddSeconds(1).ToString("MM/dd/yyyy HH:mm") + "' and '" + timeNow.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") + "') ";
                ReportTitle = $"RT Hour Report {DateTime.Now.ToString("MM_dd_yyyy HH")}";
           
                _ = PrintReportAsync(SqlReportitems, TimeFrame, ReportTitle,1);
            }

            //Shift one ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //
            if ((ShiftOneRepAuto) & (HourNow == ShiftOneHrMn))
            {
                DayStarted = DateTime.Parse(ShiftThreeEnd.ToString(), new CultureInfo(usCulture, false));
                DayEnded = DateTime.Parse(ShiftOneEnd.ToString(), new CultureInfo(usCulture, false));

                TimeFrame = " WHERE (TimeComplete BETWEEN '" + ShiftThreeEnd.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") +
                                  "' and '" + ShiftOneEnd.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") + "') ";
                ReportTitle = $"RT Shift One Report {DateTime.Now.ToString("MM_dd_yyyy")}";

                _ = PrintReportAsync(SqlReportitems, TimeFrame, ReportTitle,2);
            }

            //Shift two/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //
            if ((ShiftTwoRepAuto) & (HourNow == ShiftTwoHrMn))
            {

                DayStarted = DateTime.Parse(ShiftOneEnd.AddHours(-1).ToString(), new CultureInfo(usCulture, false));
                DayEnded = DateTime.Parse(ShiftTwoEnd.ToString(), new CultureInfo(usCulture, false));

                TimeFrame = " WHERE (TimeComplete BETWEEN '" +
                    ShiftOneEnd.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") + "' and '" + ShiftTwoEnd.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") + "') ";
                ReportTitle = $"RT Shift Two Report {DateTime.Now.ToString("MM_dd_yyyy")}";

                _ = PrintReportAsync(SqlReportitems, TimeFrame, ReportTitle,3);
            }

            //Shift three From before midnight of the previous day to AM of the next day./////////////////////////////////////////////////////////////////
            //
            if ((ShiftThreeRepAuto) & (HourNow == ShiftThreeHrMn))
            {
                DayStarted = DateTime.Parse(ShiftTwotoThreeEnd.ToString(), new CultureInfo(usCulture, false));
                DayEnded = DateTime.Parse(ShiftThreeEnd.ToString(), new CultureInfo(usCulture, false));

                TimeFrame = " WHERE (TimeComplete BETWEEN '" +
                    ShiftTwotoThreeEnd.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") + "' and '" + ShiftThreeEnd.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") + "') ";
                ReportTitle = $"RT Shift Three Report {DateTime.Now.ToString("MM_dd_yyyy")}";

                _ = PrintReportAsync(SqlReportitems, TimeFrame, ReportTitle, 4);
            }


            // Period report /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //
            if ((Settings.Default.PeriodRepCheck) & (HourNow == HourEnd))
            {
                var PerHrStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ClsCommon.HourStart.Hour, ClsCommon.HourStart.Minute, 00);
                var PerHrEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ClsCommon.HourEnd.Hour, ClsCommon.HourEnd.Minute, 00);

                DayStarted = DateTime.Parse(PerHrStart.ToString(), new CultureInfo(usCulture, false));
                DayEnded = DateTime.Parse(PerHrEnd.ToString(), new CultureInfo(usCulture, false));

                TimeFrame = " WHERE (TimeComplete BETWEEN '" +
                    PerHrStart.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") + "' and '" + PerHrEnd.AddSeconds(1).ToString("MM/dd/yyyy HH:mm") + "') ";
                ReportTitle = $"RT Period Report {DateTime.Now.ToString("MM_dd_yyyy")}";
            
                _ = PrintReportAsync(SqlReportitems, TimeFrame, ReportTitle,5);
            }
        }

        /// <summary>
        /// Print report for realtime events
        /// </summary>
        /// <param name="ItemList"></param>
        /// <param name="TimeFrame"></param>
        /// <param name="ReportTitle"></param>
        /// <param name="BaleTable"></param>
        /// <returns></returns>
        private async Task PrintReportAsync(string ItemList, string TimeFrame, string ReportTitle, int repmode)
        {
            ClsCommon.RtRep = true;

            string DayNow = DateTime.Now.ToString("dd");
            CurrBaleTable = (DayNow == "01") ? SqlHandler.GetPreviousBaleTable() : SqlHandler.GetCurrentBaleTable();


            try
            {
                string strquery = $"SELECT {ItemList} From {CurrBaleTable} {TimeFrame} ORDER by UID ASC";

                await Task.Run(async () =>
                {
                    using (DataTable ReportTable = SqlHandler.GetForteDataTable(strquery, XMLColumnList, repmode, DayStarted, DayEnded))
                    {
                        if (ReportTable.Rows.Count > 0)
                        {
                            if (BaleDataTable != null) BaleDataTable = null;
                            BaleDataTable = new DataTable();
                            BaleDataTable = ReportTable;

                            if (ClsCommon.BRtCsvOut)
                            {
                                await FileService.WriteRtCSVFileAsync(ReportTable, ReportTitle);
                                ExCsvTextMsg = FileService.ExCsvTextMsg;
                                if(Debuggy)ClsSerilog.LogMessage(ClsSerilog.Debug, $" {ExCsvTextMsg}");
                            }
                            else
                            {
                                await WriteRtExcelFileAsync(ReportTable, ReportTitle);
                               
                            }
                
                            _eventAggregator.GetEvent<ReportPrintEvent>().Publish(ReportTitle);
                        }
                        else
                        {
                            ClsSerilog.LogMessage(ClsSerilog.Info, $"No Record Found for this Report! {ReportTitle}");
                            ClsSerilog.LogMessage(ClsSerilog.Info, $"String Query {strquery}");
                        }
                    }
                });

                if (Debuggy) ClsSerilog.LogMessage(ClsSerilog.Debug, $"Print Report Title= {ReportTitle}  <{DateTime.Now}>  ");
            }
            catch (Exception ex)
            {
                ClsSerilog.LogMessage(ClsSerilog.Error, $"ERROR in BaleReportModel PrintReportAsync! {ex.Message}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ReportTitle"></param>
        /// <param name="ReportTable"></param>
        /// <returns></returns>
        private async Task PrintReportOnDemandAsync(string ReportTitle, DataTable ReportTable)
        {
            try
            {
                newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint));
                newWindowThread.SetApartmentState(ApartmentState.STA);
                newWindowThread.IsBackground = true;
                newWindowThread.Start();


                await Task.Run(async () =>
                 {
                     if (ClsCommon.BRtCsvOut)
                     {
                         await FileService.WriteRtCSVFileAsync(ReportTable, ReportTitle);
                         if (Debuggy) ClsSerilog.LogMessage(ClsSerilog.Debug, $" {FileService.ExCsvTextMsg}");
                     }
                     else
                     {
                         await WriteRtExcelFileAsync(ReportTable, ReportTitle);
                         if (Debuggy) ClsSerilog.LogMessage(ClsSerilog.Debug, $" Print Excel File Done!");
                     }
                     _eventAggregator.GetEvent<ReportPrintEvent>().Publish(ReportTitle);
                 });

                newWindowThread.Abort();
                if (newWindowThread != null) newWindowThread = null;
            }
            catch (Exception ex)
            {
                ClsSerilog.LogMessage(ClsSerilog.Error, $"ERROR in  BaleReportModel PrintReportOnDemandAsync! {ex.Message}");
            }
        }

        private void ThreadStartingPoint()
        {
            try
            {
                tempWindow = new LoadingWIndow();
                tempWindow.Show();
                System.Windows.Threading.Dispatcher.Run();
            }
            catch (ThreadAbortException)
            {
                tempWindow = null;
                //System.Windows.Threading.Dispatcher.InvokeShutdown();
            }
        }

        private async Task WriteRtExcelFileAsync(DataTable mydatatable, string fileName)
        {
            _eventAggregator.GetEvent<ReportPrintEvent>().Publish($"Printing {fileName}  Please Wait! ");

           // DataTable ModTable = ModifyAddField(mydatatable);

            try
            {
                string StrFilePatch = ClsCommon.ExCsvFileLocation + "\\" + fileName + ".xlsx";

                //Get and Set Colum Header Names
                ObservableCollection<SqlReportField> ReportXmlFile = MyXml.ReadXMlRepColumn(ClsCommon.ReportXmlFilePath);

                if (Debuggy)
                {
                    for( int i = 0; i < mydatatable.Columns.Count; i++ )
                    {
                        Console.WriteLine($" DataType {i} = {mydatatable.Columns[i].DataType.ToString()}, Name=  {mydatatable.Columns[i].ToString()}");
                        ClsSerilog.LogMessage(ClsSerilog.Debug, $" DataType {i} = {mydatatable.Columns[i].DataType.ToString()}, Name=  {mydatatable.Columns[i].ToString()}");
                    }
                }

                //Fix the header's column title from the XML file
                await Task.Run(() =>
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var ws = workbook.Worksheets.Add("ForteData");

                        //Header Row 1 top
                        for (int i = 0; i < mydatatable.Columns.Count; i++)
                        {
                            ws.Cell(HdrCol[i]+1).Value = ReportXmlFile[i].FieldName; //mydatatable.Columns[i].ColumnName;
                            ws.Cell(HdrCol[i]+1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(HdrCol[i] + 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            ws.Cell(HdrCol[i] + 1).Style.Font.SetBold();
                            mydatatable.Columns[i].ColumnName = ReportXmlFile[i].FieldName;

                            //Console.WriteLine($" ColumnName {i} = {ReportXmlFile[i].FieldName}");
                        }
                        mydatatable.AcceptChanges();
                   
                        // Row 2+
                        for (int y = 0; y < mydatatable.Columns.Count; y++)
                        {
                            for (int i = 2; i < mydatatable.Rows.Count+2; i++)
                            {
                                if (mydatatable.Columns[y].DataType == typeof(float))
                                {
                                    ws.Cell(HdrCol[y] + i).SetValue((float)(mydatatable.Rows[i - 2][y]));
                                    ws.Cell(HdrCol[y] + i).Style.NumberFormat.Format = "#.0#";
                                    ws.Cell(HdrCol[y] + i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                }
                                else if (mydatatable.Columns[y].DataType == typeof(string))
                                {
                                    ws.Cell(HdrCol[y] + i).SetValue(Convert.ToString(mydatatable.Rows[i - 2][y]));
                                    ws.Cell(HdrCol[y] + i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    
                                }
                                else if (mydatatable.Columns[y].DataType == typeof(Int32))
                                {
                                    ws.Cell(HdrCol[y] + i).SetValue(Convert.ToInt32(mydatatable.Rows[i - 2][y]));
                                    ws.Cell(HdrCol[y] + i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;   
                                }
                                else if (mydatatable.Columns[y].DataType == typeof(DateTime))
                                {
                                    ws.Cell(HdrCol[y] + i).SetValue(Convert.ToDateTime(mydatatable.Rows[i - 2][y]));
                                    ws.Cell(HdrCol[y] + i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                                    for(int x = 0; x < ReportXmlFile.Count; x++ )
                                    {
                                        if(ReportXmlFile[x].FieldName == mydatatable.Columns[y].ColumnName)
                                        {
                                            ws.Cell(HdrCol[y] + i).Style.NumberFormat.Format = ReportXmlFile[x].Format;
                                        }
                                    }
                                }
                                else
                                {
                                    ws.Cell(HdrCol[y] + i).SetValue(mydatatable.Rows[i - 2][y].ToString());   
                                }
                            }
                            ws.Column(HdrCol[y]).AdjustToContents();
                        }
                        workbook.SaveAs(StrFilePatch);

                        /*  auto
                         var ws = workbook.Worksheets.Add(mydatatable, "ForteData");
                         if (!System.IO.File.Exists(StrFilePatch))
                         {
                             workbook.SaveAs(StrFilePatch);
                             ExCsvTextMsg = "Write RT Excel " + StrFilePatch + " Done ";
                         }
                         else
                         {
                             ExCsvTextMsg = "Can not Write RT Excel " + StrFilePatch + "  " + DateTime.Now.Date.ToString("MM/dd/yyyy");
                         }
                        */
                    }
                });
                if (Debuggy) ClsSerilog.LogMessage(ClsSerilog.Debug, $"Write Excel <{fileName}>");
            }
            catch (Exception ex)
            {
                ExCsvTextMsg = $"ERROR in  BaleReportModel WriteRtExcelFileAsync! {ex.Message}";
                ClsSerilog.LogMessage(ClsSerilog.Error, $"{ExCsvTextMsg}");
            }
        }

        private DataTable ModifyAddField(DataTable mydatatable)
        {
            DataTable ModTable = new DataTable();
            DataColumnCollection columns = mydatatable.Columns;

            try
            {
                for(int i =0; i < columns.Count; i++)
                {
                    if (columns[i].Caption == "Moisture")
                    {
                        Console.WriteLine(columns[i].ColumnName + "i = "+ i);
                        DataColumn NewCol = mydatatable.Columns.Add("xxxx", typeof(float));
                    }
                }
            }
            catch (Exception ex )
            {
                MessageBox.Show("ERROR inModifyAddField" + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.Error, $"ERROR inModifyAddField {ex.Message}");
            }

            return ModTable;
        }
        //------------------------------------------------
    }

}

