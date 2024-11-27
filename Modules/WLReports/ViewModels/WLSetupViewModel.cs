using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using RTRep.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WLReports.Model;
using WLReports.Properties;
using static RTRep.Services.ClsApplicationService;

namespace WLReports.ViewModels
{
    public class WLSetupViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;
    
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

        private bool _wlscanTimerOn;
        public bool WLScanTimerOn
        {
            get { return _wlscanTimerOn; }
            set
            {
                SetProperty(ref _wlscanTimerOn, value);
                ClsCommon.WlReportOn = value;
                ScanStatus = (value) ? "ON" : "OFF";
            }
        }

        private string _scanStatus = "OFF";
        public string ScanStatus
        {
            get { return _scanStatus; }
            set { SetProperty(ref _scanStatus, value); }
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
            WLScanTimerOn = (WlCanScannerOn) ? true : false; 
        }

        private bool _periodRepCheck = Settings.Default.PeriodRepCheck;
        public bool PeriodRepCheck
        {
            get { return _periodRepCheck; }
            set
            {
                SetProperty(ref _periodRepCheck, value);
               
                Settings.Default.PeriodRepCheck = value;
                Settings.Default.Save();

                PeriodRepStatus = (value) ? "ON" : "OFF";
                HidePeriod = (value) ? "1" : ".1";
                CanScannerOn();
            }
        }
        private string _strPeriod;
        public string StrPeriod
        {
            get { return _strPeriod; }
            set { _strPeriod = value; }
        }
        private string _periodRepStatus;
        public string PeriodRepStatus
        {
            get { return _periodRepStatus; }
            set { SetProperty(ref _periodRepStatus, value); }
        }
        private string _hidePeriod = ".1";
        public string HidePeriod
        {
            get => _hidePeriod;
            set => SetProperty(ref _hidePeriod, value);
        }

   
        private bool _dayRepChecked;
        public bool DayRepChecked
        {
            get { return _dayRepChecked; }
            set
            {
                SetProperty(ref _dayRepChecked, value);
                Settings.Default.bDayRepCheck = value;
                Settings.Default.Save();
                DayRepStatus = (value) ? "ON" : "OFF";
                CanScannerOn();
               ClsSerilog.LogMessage(ClsSerilog.Info, $" WL DayReport {value}");
            }
        }
        private string _dayRepStatus = "OFF";
        public string DayRepStatus
        {
            get { return _dayRepStatus; }
            set { SetProperty(ref _dayRepStatus, value); }
        }

        private bool _hourRepChecked;
        public bool HourRepChecked
        {
            get { return _hourRepChecked; }
            set
            {
                SetProperty(ref _hourRepChecked, value);
                Settings.Default.bHourRepCheck = value;
                Settings.Default.Save();
                HourRepStatus = (value) ? "ON" : "OFF";
                CanScannerOn();
                ClsSerilog.LogMessage(ClsSerilog.Info, $" WL Hour Report {value}");
            
            }
        }
        private string _hourRepStatus = "OFF";
        public string HourRepStatus
        {
            get { return _hourRepStatus; }
            set { SetProperty(ref _hourRepStatus, value); }
        }


        private bool _shiftOneCheck;
        public bool ShiftOneCheck
        {
            get { return _shiftOneCheck; }
            set
            {
                SetProperty(ref _shiftOneCheck, value);
                Settings.Default.bShiftOneCheck = value;
                Settings.Default.Save();
                ShiftOneRepStatus = (value) ? "ON" : "OFF";
                CanScannerOn();
            }
        }
        private string _shiftOneRepStatus;
        public string ShiftOneRepStatus
        {
            get { return _shiftOneRepStatus; }
            set { SetProperty(ref _shiftOneRepStatus, value); }
        }

        private bool _shiftTwoCheck;
        public bool ShiftTwoCheck
        {
            get { return _shiftTwoCheck; }
            set
            {
                SetProperty(ref _shiftTwoCheck, value);
                Settings.Default.bShiftTwoCheck = value;
                Settings.Default.Save();
                ShiftTwoRepStatus = (value) ? "ON" : "OFF";
                CanScannerOn();
                ClsSerilog.LogMessage(ClsSerilog.Info, $" WL Shift Two Report {value}");
            }
        }
        private string _shiftTwoRepStatus;
        public string ShiftTwoRepStatus
        {
            get { return _shiftTwoRepStatus; }
            set { SetProperty(ref _shiftTwoRepStatus, value); }
        }


        private bool _shiftThreeCheck;
        public bool ShiftThreeCheck
        {
            get { return _shiftThreeCheck; }
            set
            {
                SetProperty(ref _shiftThreeCheck, value);
                Settings.Default.bShiftThreeCheck = value;
                Settings.Default.Save();
                ShiftThreeRepStatus = (value) ? "ON" : "OFF";
                CanScannerOn();
                ClsSerilog.LogMessage(ClsSerilog.Info, $" WL Shift Three Report {value}");
            }
        }
        private string _shiftThreeRepStatus;
        public string ShiftThreeRepStatus
        {
            get { return _shiftThreeRepStatus; }
            set { SetProperty(ref _shiftThreeRepStatus, value); }
        }

        private bool _wLCSVChecked;
        public bool WLCSVChecked
        {
            get => _wLCSVChecked;
            set { SetProperty(ref _wLCSVChecked, value); }
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

        // Moisture Type for WL --------------------------------------------------
        private bool _mCChecked;
        public bool MCChecked
        {
            get => _mCChecked;
            set 
            { 
                SetProperty(ref _mCChecked, value);
                if(value)
                {
                    ClsCommon.MoistureType = ClsCommon.MContent;
                    ClsCommon.MoistureUnit = ClsCommon.MtUnit[ClsCommon.MContent];
                }
            
            }
        }
        private bool _mRChecked;
        public bool MRChecked
        {
            get => _mRChecked;
            set 
            { 
                SetProperty(ref _mRChecked, value); 
                if(value)
                {
                    ClsCommon.MoistureType = ClsCommon.MRegain;
                    ClsCommon.MoistureUnit = ClsCommon.MtUnit[ClsCommon.MRegain];
                }
            }
        }
        //-------------------------------------------------------------------------

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
                MessageBox.Show($"Error in findCreateDir {ex.Message}");
            }
        }

        public WLSetupViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;

            DayRepChecked = Settings.Default.bDayRepCheck;
            HourRepChecked = Settings.Default.bHourRepCheck;
            ShiftOneCheck = Settings.Default.bShiftOneCheck;
            ShiftTwoCheck = Settings.Default.bShiftTwoCheck;
            ShiftThreeCheck = Settings.Default.bShiftThreeCheck;
            PeriodRepCheck = Settings.Default.PeriodRepCheck;
            StrPeriod = GetstrPeriod();

            WLScanTimerOn = ClsCommon.WlReportOn;
            WLCSVChecked = (ClsCommon.BExcelOut) ? false : true;

            switch ((int)ClsCommon.MoistureType)
            {
                case 0:
                    MCChecked = true;
                    break;
                case 1:
                    MRChecked = true;
                    break;
            }
        }

       

        internal string GetstrPeriod()
        {
            string HourStart = string.Empty;
            string HourEnd= string.Empty;

            HourStart = ClsCommon.HourStart.ToString("HH:mm");
            HourEnd = ClsCommon.HourEnd.ToString("HH:mm");
            return HourStart + "-" + HourEnd;
        }

    }
}
