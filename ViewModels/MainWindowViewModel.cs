using ForteReports.Model;
using FortéRTREP.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using RTRep.Services;
using System;
using System.Threading;
using System.Windows;
using Unity;
using static RTRep.Services.ClsApplicationService;
using Application = System.Windows.Forms.Application;

namespace FortéRTREP.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
       protected readonly IEventAggregator _eventAggregator;

        private readonly IUnityContainer _container;
        private IModuleManager moduleManager;
        private readonly IRegionManager _regionManager;

        private readonly ForteReportModel ReportModel;
        private readonly ClsSQLhandler Sqlhandler;

        private string _mainWindowTitle = "Forté RealTime and Layers Reports from ->";
        public string MainWindowTitle
        {
            get => _mainWindowTitle;
            set => SetProperty(ref _mainWindowTitle, value);
        }

        private string _title = "Forté RealTime Reports";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private WindowState _curWindowState;
        public WindowState CurWindowState
        {
            get => _curWindowState;
            set => SetProperty(ref _curWindowState, value);
        }

        private string _tabRtHeight = "0";
        public string TabRtHeight
        {
            get => _tabRtHeight;
            set => SetProperty(ref _tabRtHeight, value);
        }

        private string _tabWLHeight = "0";
        public string TabWLHeight
        {
            get => _tabWLHeight;
            set => SetProperty(ref _tabWLHeight, value);
        }

        private string _tabXFerHeight = "0";
        public string TabXFerHeight
        {
            get => _tabXFerHeight;
            set => SetProperty(ref _tabXFerHeight, value);
        }


        private bool _wLExcist;
        public bool WLExcist
        {
            get => _wLExcist; 
            set => SetProperty(ref _wLExcist,value); 
        }

        private string _strStatus;
        public string StrStatus
        {
            get => _strStatus;
            set => SetProperty(ref _strStatus, value);
        }

        private bool _baleActive;
        public bool BaleActive
        {
            get => _baleActive;
            set 
            { 
                SetProperty(ref _baleActive, value);
                ShowRT = (value) ? Visibility.Visible : Visibility.Hidden;
            }
        }
        private Visibility _showRT;
        public Visibility ShowRT
        {
            get => _showRT;
            set => SetProperty(ref _showRT, value); 
        }

        private bool _wLActive;
        public bool WLActive
        {
            get => _wLActive;
            set 
            { 
                SetProperty(ref _wLActive, value);
                ShowWL = (value) ? Visibility.Visible : Visibility.Hidden;
            }
        }
        private Visibility _showWL;
        public Visibility ShowWL
        {
            get => _showWL;
            set => SetProperty(ref _showWL, value);
        }

        private bool _xFerActive;
        public bool XFerActive
        {
            get => _xFerActive;
            set
            {
                SetProperty(ref _xFerActive, value);
                ShowXFer = (value) ? Visibility.Visible : Visibility.Hidden;
            }
        }
        private Visibility _showXFer;
        public Visibility ShowXFer
        {
            get => _showXFer;
            set => SetProperty(ref _showXFer, value);
        }

        private bool _selectBale;
        public bool SelectBale
        {
            get => _selectBale;
            set => SetProperty(ref _selectBale, value);
        }

        private bool _selectWl;
        public bool SelectWl
        {
            get => _selectWl;
            set => SetProperty(ref _selectWl, value);
        }

        private bool _selectXFer;
        public bool SelectXFer
        {
            get => _selectXFer;
            set => SetProperty(ref _selectXFer, value);
        }


        private bool _selectSetup;
        public bool SelectSetup
        {
            get => _selectSetup;
            set => SetProperty(ref _selectSetup, value);
        }

        private string _curTime;
        public string CurTime
        {
            get => _curTime;
            set => SetProperty(ref _curTime, value);
        }

        private bool bSqlConections;
        public bool BSqlConections
        {
            get { return bSqlConections; }
            set { bSqlConections = value; }
        }

        public string ProgramVersion
        {
            get { return GetLastModTime();  }  
        }

        public DelegateCommand ClosedPageICommand { get; set; }

        public DelegateCommand _appExitCommand;
        public DelegateCommand AppExitCommand =>
        _appExitCommand ?? (_appExitCommand =
          new DelegateCommand(AppExitExecute));
        private void AppExitExecute()
        {
            if (System.Windows.MessageBox.Show("Are you Sure, you want to Exit ?", "Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ClsSerilog.LogMessage(ClsSerilog.Info, $"Exit Program");
                MainWindow.AppWindows.Close();
            }
        }
        public DelegateCommand _hideScrCommand;
        public DelegateCommand HideScrCommand =>
        _hideScrCommand ?? (_hideScrCommand =
          new DelegateCommand(HideScrCommandExecute));
        private void HideScrCommandExecute()
        {
            MainWindow.AppWindows.WindowState = WindowState.Minimized;
        }

        public DelegateCommand _minScrCommand;
        public DelegateCommand MinScrCommand =>
        _minScrCommand ?? (_minScrCommand =
          new DelegateCommand(MinScrCommandExecute));
        private void MinScrCommandExecute()
        {
            CurWindowState = WindowState.Normal;
        }

        public DelegateCommand _fullScrCommand;
        public DelegateCommand FullScrCommand =>
        _fullScrCommand ?? (_fullScrCommand =
          new DelegateCommand(FullScrCommandExecute));
        private void FullScrCommandExecute()
        {
            CurWindowState = WindowState.Maximized;
        }

        private DelegateCommand _ViewLogCommand;
        public DelegateCommand ViewLogCommand =>
        _ViewLogCommand ?? (_ViewLogCommand =
            new DelegateCommand(ViewLogExecute));
        private void ViewLogExecute()
        {
          
        }

        private string GetLastModTime()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(assembly.Location);
            DateTime lastModified = fileInfo.LastWriteTime;
            return $" SW.Ver: {lastModified.ToString()}";
        }

        public MainWindowViewModel(IUnityContainer container, IRegionManager regionManager, IEventAggregator EventAggregator)
        {
            _container = container;
            _regionManager = regionManager;

            this._eventAggregator = EventAggregator;

           

            // ClsCommon.MyInfoLog.SetTitle("ForteReportApp");
            //  ClsCommon.MyInfoLog.LogMessage(MsgTypes.INFO, MsgSources.APPSTART, "..................................");
            //  ClsCommon.MyInfoLog.LogMessage(MsgTypes.INFO, MsgSources.APPSTART, ".... StartUp Forté Report App ....");
            //  ClsCommon.MyInfoLog.LogMessage(MsgTypes.INFO, MsgSources.APPSTART, "..................................");

            Sqlhandler = ClsSQLhandler.Instance;
            ReportModel = new ForteReportModel(_eventAggregator);

            StrStatus = string.Empty;
            _eventAggregator.GetEvent<CloseAppEvent>().Subscribe(ShutDownEvent);

           // _eventAggregator.GetEvent<UpdateWLRepTimerEvents>().Subscribe(UpdateWLTimerEvent, ThreadOption.UIThread);
          //  _eventAggregator.GetEvent<UpdateBaleRepTimerEvents>().Subscribe(UpdateWLTimerEvent);

            this.moduleManager = _container.Resolve<IModuleManager>();
            this.moduleManager.LoadModule("RepSetupModule"); //run Initialize() of the other module 



            if(Sqlhandler.CheckWorkStationTarget())
            {
                BaleActive = (Sqlhandler.FindSqlDatabase("ForteData"));
                WLActive = (Sqlhandler.FindSqlDatabase("ForteLayer"));

                ClsCommon.BaleActive = BaleActive;
                ClsCommon.WLActive = WLActive;

                // XFerActive = ((ClsCommon.Host == Environment.MachineName) & (ClsCommon.SQLTransferOn)) ? true : false;
                XFerActive = false; // for debug only
                ClsCommon.SQLXferValid = XFerActive;

                if (BaleActive)
                {
                    if (!WLActive) SelectBale = true;
                    StrStatus = "Realtime Database Active ";
                    MainWindowTitle = "Forté Realtime Reports  SQL Server ->  " + ClsCommon.Host;
                    this.moduleManager = _container.Resolve<IModuleManager>();
                    this.moduleManager.LoadModule("BaleReportsModule"); //run Initialize() of the other module 
                    TabRtHeight = "60";
                    ClsSerilog.LogMessage(ClsSerilog.Info, $"BaleActive Active");
                    
                }
                else
                {
                    TabRtHeight = "0";
                    ClsSerilog.LogMessage(ClsSerilog.Info, $"BaleActive Not Active");
                }
                    
                if (WLActive)
                {
                    if (!BaleActive) SelectWl = true;
                    StrStatus += " and Wet Layers Database Active";
                    this.moduleManager = _container.Resolve<IModuleManager>();
                    this.moduleManager.LoadModule("WLReportsModule"); //run Initialize() of the other module 
                    TabWLHeight = "60";
                    ClsSerilog.LogMessage(ClsSerilog.Info, $"Wetlayer Active");
                   
                }
                else
                {
                    TabWLHeight = "0";
                    ClsSerilog.LogMessage(ClsSerilog.Info, $"WetLayer not Active");
                }
                   
                if((WLActive) & (BaleActive))
                {
                    MainWindowTitle = "Forté Realtime & Layers Reports from SQL Server ->  " + ClsCommon.Host;
                }
                
                if (XFerActive)
                {
                    TabXFerHeight = "60";
                    this.moduleManager = _container.Resolve<IModuleManager>();
                    this.moduleManager.LoadModule("SQLDataTansferModule"); //run Initialize() of the other module
                }else
                    TabXFerHeight = "0";

                SelectSetup = true;

                StrStatus = "Target Workstation is set";
                ClsSerilog.LogMessage(ClsSerilog.Info, $"{StrStatus}");
            }
            else
            {
                StrStatus = "Target Workstation is not set !";
                ClsSerilog.LogMessage(ClsSerilog.Info, $"{StrStatus}");
            }
            CurTime = DateTime.Now.ToString("d");
        }

        private void UpdateWLTimerEvent(DateTime obj)
        {
            CurTime = obj.ToString("d");
        }

        private void UpdateRTTimerEvent(DateTime obj)
        {
            CurTime = obj.ToString("HH:mm:ss");
        }

        private void ShutDownEvent(bool obj)
        {
            MessageBox.Show("Program will Exit and Restart !");
            MainWindow.AppWindows.Close();
            Thread.Sleep(1000);
            Application.Restart();
        }
    }
}
