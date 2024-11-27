using Prism.Commands;
using RTRep.Services.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTRep.Services
{
    public static class ClsCommon
    {
        public static int WtMetric = 0;
        public static int WtEnglish = 1;

        // MoistureType.MContent = 0
        public static int MContent = 0;
        public static int MRegain = 1;
        public static int MAirDry = 2;
        public static int MBoneDry = 3;

        public static int PreviewReport = 0;
        public static int PreviewGraph = 1;

        public static int DayReport = 0;
        public static int HourReport = 1;
        public static int ShiftOneReport = 3;
        public static int ShiftTwoReport = 4;
        public static int ShiftThreeReport = 5;
        public static int PeriodReport = 6;

        public static bool RtRep;

        public static int ExcelCSVReport = 0;
        public static int ExcelCSVGraph = 1;

        public static string[] RepEvents = new string[] { "Daily", "Hourly", "Monthly", "ShiftOne", "ShiftTwo", "ShiftThree", "Period" };
        
        public static string[] MtType = new string[] { "MContent", "MRegain", "MAirDry", "MBoneDry" };
        public static string[] MtUnit = new string[] { "%MC","% MR","%AD","%BD" };

        public static string[] WtType = new string[] { "WtMetric", "WtEnglish" };
        public static string[] WtUnit = new string[] { "Kg.", "Lb." };

        public static string path = Path.Combine(Directory.GetCurrentDirectory(), "ASCIILog", "two");

        public static string[] BaleArchMonth = new string[] 
        { "BaleArchiveJan", 
          "BaleArchiveFeb",
          "BaleArchiveMar",
          "BaleArchiveApr",
          "BaleArchiveMay",
          "BaleArchiveJun",
          "BaleArchiveJul",
          "BaleArchiveAug",
          "BaleArchiveSep",
          "BaleArchiveOct",
          "BaleArchiveNov",
          "BaleArchiveDec"};


        public static string[] WlArchMonth = new string[]
        { "FValueReadingsJan",
          "FValueReadingsFeb",
          "FValueReadingsMar",
          "FValueReadingsApr",
          "FValueReadingsMay",
          "FValueReadingsJun",
          "FValueReadingsJul",
          "FValueReadingsAug",
          "FValueReadingsSep",
          "FValueReadingsOct",
          "FValueReadingsNov",
          "FValueReadingsDec"};

        public static class GlobalCommands
        {
            public static CompositeCommand MyCompositeCommand = new CompositeCommand();
        }

        /// <summary>
        /// 
        /// </summary>
        public static string ReportFieldsCustomer1 
        {
            get => "TimeComplete, SourceName,SerialNumber, Weight, TareWeight, Forte," +
                "NetWeight, Moisture, BDWeight*(1+SR/100), StockLabel4, SpareSngFld3, StockName, FC_IdentString";
        }

        public static  string ReportFieldsManual { get; set; }

        public static string BaseDirPath
        {
            get => System.AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string ReportXmlFilePath
        {
            get => System.AppDomain.CurrentDomain.BaseDirectory + "GridviewItems.xml";
        }

        public static string XferRtItemsXmlFilePath
        {
            get => System.AppDomain.CurrentDomain.BaseDirectory + "XferRtItems.xml";
        }

        public static string XferWlItemsXmlFilePath
        {
            get => System.AppDomain.CurrentDomain.BaseDirectory + "XferWlItems.xml";
        }

        public static string ReportFields
        {
            get => Settings.Default.ReportFields;
            set
            {
                Settings.Default.ReportFields = value;
                Settings.Default.Save();
            }
        }

        private static ObservableCollection<SqlReportField> _reportTable;
        public static ObservableCollection<SqlReportField> ReportTable
        {
            get => _reportTable;
            set => _reportTable = value;
        }



        public static List<Tuple<string, char>> Asciilist = new List<Tuple<string, char>>() {
            new Tuple<string, char>("<NULL>", '\x0'),
            new Tuple<string, char>("<SOH>", '\x1'),
            new Tuple<string, char>("<STX>", '\x2'),
            new Tuple<string, char>("<ETX>", '\x3'),
            new Tuple<string, char>("<EOT>", '\x4'),
            new Tuple<string, char>("<ENQ>", '\x5'),
            new Tuple<string, char>("<ACK>", '\x6'),
            new Tuple<string, char>("<TAB>", '\x9'),
            new Tuple<string, char>("<LF>", '\xA'),
            new Tuple<string, char>("<VT>", '\xB'),
            new Tuple<string, char>("<FF>", '\xC'),
            new Tuple<string, char>("<CR>", '\xD'),
            new Tuple<string, char>("<SO>", '\xF'),
            new Tuple<string, char>("<ETB>", '\x17'),
            new Tuple<string, char>("<ESC>", '\x1B'),
            new Tuple<string, char>("<Space>", '\x20'),
            new Tuple<string, char>(",", '\x2C'),
            new Tuple<string, char>("-", '\x2D'),
            new Tuple<string, char>(".", '\x2E'),
            new Tuple<string, char>("#", '\x23'),
            new Tuple<string, char>(":", '\x2A'),
            new Tuple<string, char>(";", '\x2B'),
            new Tuple<string, char>("[", '\x5B'),
            new Tuple<string, char>("]", '\x5D'),
            new Tuple<string, char>("_", '\x5F')};


        public static bool BaleActive
        {
            get => Settings.Default.BaleActive;
            set
            {
                Settings.Default.BaleActive = value;
                Settings.Default.Save();
            }
        }

        public static bool WLActive
        {
            get => Settings.Default.WLActive;
            set
            {
                Settings.Default.WLActive = value;
                Settings.Default.Save();
            }
        }



        public static string MoistureUnit
        {
            get => Settings.Default.MoistureUnit;  
            set 
            {
                Settings.Default.MoistureUnit = value;
                Settings.Default.Save();
            }
        }

        public static int MoistureType
        {
            get => Settings.Default.MoistureType; 
            set
            {
                Settings.Default.MoistureType = value;
                Settings.Default.Save();
            }
        }

        public static int WeightType
        {
            get => Settings.Default.WeightType; 
            set
            {
                Settings.Default.WeightType = value;
                Settings.Default.Save();
            }
        }

        public static string WeightUnit
        {
            get => Settings.Default.WeightUnit; 
            set
            {
                Settings.Default.WeightUnit = value;
                Settings.Default.Save();
            }
        }

        public static string Host
        {
            get => Settings.Default.Host; 
            set
            {
                Settings.Default.Host = value;
                Settings.Default.Save();
            }
        }

        public static string LocalHost
        {
            get => Settings.Default.LocalHost; 
            set
            {
                Settings.Default.LocalHost = value;
                Settings.Default.Save();
            }
        }

        public static string Instance
        {
            get => Settings.Default.Instance; 
            set
            {
                Settings.Default.Instance = value;
                Settings.Default.Save();
            }
        }

        public static string UserName
        {
            get => Settings.Default.UserName; 
            set
            {
                Settings.Default.UserName = value;
                Settings.Default.Save();
            }
        }

        public static string PassWord
        {
            get => Settings.Default.PassWord; 
            set
            {
                Settings.Default.PassWord = value;
                Settings.Default.Save();
            }
        }

        public static string RTDatabase
        {
            get => Settings.Default.RTDatabase; 
            set
            {
                Settings.Default.RTDatabase = value;
                Settings.Default.Save();
            }
        }

        public static DateTime ProdDayEnd
        {
            get => Settings.Default.DayEndTime; 
            set
            {
                Settings.Default.DayEndTime = value;
                Settings.Default.Save();
            }
        }

        public static DateTime HourStart
        {
            get => Settings.Default.HourStart; 
            set
            {
                Settings.Default.HourStart = value;
                Settings.Default.Save();
            }
        }
        public static DateTime HourEnd
        {
            get => Settings.Default.HourEnd; 
            set
            {
                Settings.Default.HourEnd = value;
                Settings.Default.Save();
            }
        }


        public static DateTime ShiftOneEnd
        {
            get { return Settings.Default.ShiftOneEnd; }
            set
            {
                Settings.Default.ShiftOneEnd = value;
                Settings.Default.Save();
            }
        }

        public static DateTime ShiftTwoEnd
        {
            get => Settings.Default.ShiftTwoEnd;
            set
            {
                Settings.Default.ShiftTwoEnd = value;
                Settings.Default.Save();
            }
        }

        public static DateTime ShiftThreeEnd
        {
            get => Settings.Default.ShiftThreeEnd;
            set
            {
                Settings.Default.ShiftThreeEnd = value;
                Settings.Default.Save();
            }
        }

        public static int ScanRate
        {
            get => Settings.Default.ScanRate;
            set
            {
                Settings.Default.ScanRate = value;
                Settings.Default.Save();
            }
        }

        public static int ScanRateWl
        {
            get => Settings.Default.ScanRateWl;
            set
            {
                Settings.Default.ScanRateWl = value;
                Settings.Default.Save();
            }
        }


        public static DateTime CurrProdDayEnd
        {
            get => Settings.Default.CurrProdDayEnd;
            set
            {
                Settings.Default.CurrProdDayEnd = value;
                Settings.Default.Save();
            }
        }
        public static DateTime PrevProdDayEnd
        {
            get => Settings.Default.PrevProdDayEnd;
            set
            {
                Settings.Default.PrevProdDayEnd = value;
                Settings.Default.Save();
            }
        }
        public static DateTime NextProdDayEnd
        {
            get => Settings.Default.NextProdDayEnd;
            set
            {
                Settings.Default.NextProdDayEnd = value;
                Settings.Default.Save();
            }
        }

        public static string DayEndHrMn
        {
            get => Settings.Default.DayEndHrMn;
            set
            {
                Settings.Default.DayEndHrMn = value;
                Settings.Default.Save();
            }
        }

        public static bool RTRepPrn
        {
            get => Settings.Default.RTRepPrn;
            set
            {
                Settings.Default.RTRepPrn = value;
                Settings.Default.Save();
            }
        }

        public static bool ExCsvCheck
        {
            get => Settings.Default.ExCsvOn;
            set
            {
                Settings.Default.ExCsvOn = value;
                Settings.Default.Save();
            }
        }

        public static bool WLCSV
        {
            get => Settings.Default.WLCSV;
            set
            {
                Settings.Default.WLCSV = value;
                Settings.Default.Save();
            }
        }

        public static bool RtScannerOn
        {
            get => Settings.Default.RtScannerOn;
            set
            {
                Settings.Default.RtScannerOn = value;
                Settings.Default.Save();
            }
        }

        public static bool RtReportOn
        {
            get => Settings.Default.RtReportOn;
            set
            {
                Settings.Default.RtReportOn = value;
                Settings.Default.Save();
            }
        }

        public static bool WlReportOn
        {
            get => Settings.Default.WlReportOn;
            set
            {
                Settings.Default.WlReportOn = value;
                Settings.Default.Save();
            }
        }

        //--------------------------------------------------------------------
        public static bool SQLTransferOn
        {
            get => Settings.Default.SQLTransferOn;
            set
            {
                Settings.Default.SQLTransferOn = value;
                Settings.Default.Save();
            }
        }
        public static bool RtTransferEnable
        {
            get => Settings.Default.RtTransferEnable;
            set
            {
                Settings.Default.RtTransferEnable = value;
                Settings.Default.Save();
            }
        }
        public static bool WlTransferEnable
        {
            get => Settings.Default.WlTransferEnable;
            set
            {
                Settings.Default.WlTransferEnable = value;
                Settings.Default.Save();
            }
        }

        public static int XferScanRateRt
        {
            get => Settings.Default.XferScanRateRt;
            set
            {
                Settings.Default.XferScanRateRt = value;
                Settings.Default.Save();
            }
        }
        //----------------------------------------------------------------------
        public static bool BExcelOut
        {
            get => Settings.Default.BExcelOut;
            set
            {
                Settings.Default.BExcelOut = value;
                Settings.Default.Save();
            }
        }

        public static bool BRtExcelOut
        {
            get => Settings.Default.BRtExcelOut;
            set
            {
                Settings.Default.BRtExcelOut = value;
                Settings.Default.Save();
            }
        }

        public static bool BRtCsvOut
        {
            get => Settings.Default.BRtCsvOut;
            set
            {
                Settings.Default.BRtCsvOut = value;
                Settings.Default.Save();
            }
        }

        public static string ExCsvFileLocation
        {
            get => Settings.Default.ExCsvFileLocation; 
            set
            {
                Settings.Default.ExCsvFileLocation = value;
                Settings.Default.Save();
            }
        }

        public static string ExCsvFileLocationWL
        {
            get => Settings.Default.ExCsvFileLocationWL; 
            set
            {
                Settings.Default.ExCsvFileLocationWL = value;
                Settings.Default.Save();
            }
        }

        public static bool UsedProdTime
        {
            get => Settings.Default.UsedProdTime;
            set
            {
                Settings.Default.UsedProdTime = value;
                Settings.Default.Save();
            }
        }

        public static bool UsedCusField
        {
            get => Settings.Default.UsedCusField;
            set
            {
                Settings.Default.UsedCusField = value;
                Settings.Default.Save();
            }
        }

        public static bool SQLXferValid
        {
            get => Settings.Default.SQLXferValid;
            set
            {
                Settings.Default.SQLXferValid = value;
                Settings.Default.Save();
            }
        }


        public static List<Tuple<string, string, string>> BaleQueryItemList;

        //-------------------------------------------------------




        //--------------------------------------------------------

        public static ObservableCollection<SqlReportField> ReportGridView { get; set; }

        public static ObservableCollection<SqlOutFields> RemoteSqlMapfields { get; set; }

        /// <summary>
        /// Moisture data type = float = Single
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mType"></param>
        /// <returns></returns>
        public static string CalulateMoisture(string data, int mType)
        {
            string Newdata = string.Empty;
            float ftMoisture = 0;

            switch (mType)
            {
                case 0: // %MC = moisture from Sql database
                    ftMoisture = Convert.ToSingle(data);
                    break;

                case 1: // %MR = Moisture / ( 1- Moisture / 100)
                    ftMoisture = Convert.ToSingle(data) / (1 - Convert.ToSingle(data) / 100);
                    break;

                case 2: // %AD = (100 - moisture) / 0.9
                    ftMoisture = (float)((100 - Convert.ToSingle(data)) / 0.9);

                    break;

                case 3: // %BD = 100 - moisture
                    ftMoisture = 100 - Convert.ToSingle(data);
                    break;
            }
            return ftMoisture.ToString("0.##");
        }

    }
}
