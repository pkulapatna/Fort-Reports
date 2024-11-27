using BaleReports.Model;
using BaleReports.Properties;
using FieldsColumnSelect.Views;
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

namespace BaleReports.ViewModels
{
    public class BaleSetupViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;
     
        private string _scanRate = ClsCommon.ScanRate.ToString();
        public string ScanRate
        {
            get => _scanRate;
            set => SetProperty(ref _scanRate, value);
        }

        private bool _dayRepChecked;
        public bool DayRepChecked
        {
            get => _dayRepChecked;
            set
            {
                SetProperty(ref _dayRepChecked, value);
                Settings.Default.bDayRepCheck = value;
                Settings.Default.Save();
                DayRepStatus = (value) ? "ON" : "OFF";
                CanScannerOn();

                ClsSerilog.LogMessage(ClsSerilog.Info, $" Rt Day Report {value}");
            }
        }
        private string _dayRepStatus;
        public string DayRepStatus
        {
            get => _dayRepStatus;
            set => SetProperty(ref _dayRepStatus, value);
        }

      
        private bool _shiftOneCheck;
        public bool ShiftOneCheck
        {
            get => _shiftOneCheck;
            set
            {
                SetProperty(ref _shiftOneCheck, value);
                Settings.Default.bShiftOneCheck = value;
                Settings.Default.Save();
                ShiftOneRepStatus = (value) ? "ON" : "OFF";
                CanScannerOn();
                ClsSerilog.LogMessage(ClsSerilog.Info, $" Rt ShiftOne Report {value}");
            }
        }

        private string _shiftOneRepStatus; // = GetShiftOneStatus();
        public string ShiftOneRepStatus
        {
            get => _shiftOneRepStatus;
            set => SetProperty(ref _shiftOneRepStatus, value);
        }



        private bool _shiftTwoCheck;
        public bool ShiftTwoCheck
        {
            get => _shiftTwoCheck;
            set
            {
                SetProperty(ref _shiftTwoCheck, value);
                Settings.Default.bShiftTwoCheck = value;
                Settings.Default.Save();
                ShiftTwoRepStatus = (value) ? "ON" : "OFF";
                CanScannerOn();
                ClsSerilog.LogMessage(ClsSerilog.Info, $" Rt ShiftTwo Report {value}");
            }
        }
        private string _shiftTwoRepStatus; // = GetShiftTwoStatus();
        public string ShiftTwoRepStatus
        {
            get => _shiftTwoRepStatus;
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
                ClsSerilog.LogMessage(ClsSerilog.Info, $" Rt ShiftThree Report {value}");
            }
        }
        private string _shiftThreeRepStatus; // = GetShiftThreeStatus();
        public string ShiftThreeRepStatus
        {
            get => _shiftThreeRepStatus;
            set => SetProperty(ref _shiftThreeRepStatus, value);
        }

        private bool _periodRepCheck;
        public bool PeriodRepCheck
        {
            get => _periodRepCheck;
            set 
            {
                SetProperty(ref _periodRepCheck, value);
                Settings.Default.PeriodRepCheck = value;
                Settings.Default.Save();

                PeriodRepStatus = (value) ? "ON" : "OFF";
                CanScannerOn();
                ClsSerilog.LogMessage(ClsSerilog.Info, $" Rt Priod Report {value}");
            }
        }
        private string _periodRepStatus;
        public string PeriodRepStatus
        {
            get => _periodRepStatus;
            set => SetProperty(ref _periodRepStatus, value);
        }


        private bool _hourRepChecked;
        public bool HourRepChecked
        {
            get => _hourRepChecked;
            set
            {
                SetProperty(ref _hourRepChecked, value);
                HourRepStatus = (value) ? "ON" : "OFF";
                CanScannerOn();
                Settings.Default.bHourRepCheck = value;
                Settings.Default.Save();
                ClsSerilog.LogMessage(ClsSerilog.Info, $" Rt Hour Report {value}");
            }
        }
        private string _hourRepStatus;
        public string HourRepStatus
        {
            get => _hourRepStatus;
            set => SetProperty(ref _hourRepStatus, value);
        }

        private bool _rtCanScannerOn;
        public bool RtCanScannerOn
        {
            get => _rtCanScannerOn;
            set => SetProperty(ref _rtCanScannerOn, value);
        }
        private void CanScannerOn()
        {
            RtCanScannerOn = (DayRepChecked | HourRepChecked | ShiftOneCheck | ShiftTwoCheck | ShiftThreeCheck | PeriodRepCheck);
            RtScannerOn = (RtCanScannerOn) ? true:false;

        }

        private bool _rtScannerOn;
        public bool RtScannerOn
        {
            get => _rtScannerOn;
            set
            {
                SetProperty(ref _rtScannerOn, value);
                ClsCommon.RtScannerOn = value;
                ScanStatus = (value) ? "ON" : "OFF";
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

        private string _scanStatus = "OFF";
        public string ScanStatus
        {
            get => _scanStatus;
            set => SetProperty(ref _scanStatus, value);
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

        private Window RtFieldSelWindow;

        private DelegateCommand _columnConfigCommand;
        public DelegateCommand ColumnConfigCommand =>
       _columnConfigCommand ?? (_columnConfigCommand =
            new DelegateCommand(ColumnConfigCommandExecute));
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



        public BaleSetupViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            DayRepChecked = Settings.Default.bDayRepCheck;
            HourRepChecked = Settings.Default.bHourRepCheck;
            ShiftOneCheck = Settings.Default.bShiftOneCheck;
            ShiftTwoCheck = Settings.Default.bShiftTwoCheck;
            ShiftThreeCheck = Settings.Default.bShiftThreeCheck;
            PeriodRepCheck = Settings.Default.PeriodRepCheck;

           
            CSVChecked = ClsCommon.BRtCsvOut;
            RtScannerOn = ClsCommon.RtScannerOn;
            ExcelChecked = (ClsCommon.BRtExcelOut) ? true : false;

        }
    }
}
