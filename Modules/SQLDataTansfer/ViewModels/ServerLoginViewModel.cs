using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SQLDataTansfer.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static RTRep.Services.ClsApplicationService;

namespace SQLDataTansfer.ViewModels
{
    public class ServerLoginViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;

        private string _host;
        public string Host
        {
            get => _host;
            set => SetProperty(ref _host, value);
        }

        private string _userid;
        public string Userid
        {
            get => _userid;
            set => SetProperty(ref _userid, value);
        }
        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private bool _winAuChecked = true;
        public bool WinAuChecked
        {
            get => _winAuChecked; 
            set { SetProperty(ref _winAuChecked, value); }
        }

        private bool _sQLAuChecked = false;
        public bool SQLAuChecked
        {
            get => _sQLAuChecked;
            set { SetProperty(ref _sQLAuChecked, value); }
        }


        private DelegateCommand _connectCommand;
        public DelegateCommand ConnectCommand =>
        _connectCommand ?? (_connectCommand = new DelegateCommand(ConnectCommandExecute));
        private void ConnectCommandExecute()
        {
            bool bTestRet = false;

            if(WinAuChecked)
            {
                //Do the checking 





               
            }
            else
            {
                //Do the checking 

                Settings.Default.xferRtUserId = Userid;
                Settings.Default.xferRtUserPwd = Password;
                Settings.Default.Save();

            }


            _eventAggregator.GetEvent<TestSqlConnection>().Publish(bTestRet);

            CloseChildWindows();
        }

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
        _cancelCommand ?? (_cancelCommand = new DelegateCommand(CancelCommandExecute));
        private void CancelCommandExecute()
        {
            CloseChildWindows();
        }

       
        private void CloseChildWindows()
        {
            _eventAggregator.GetEvent<CloseSqlConnectWindow>().Publish(true);
        }

        public ServerLoginViewModel(IEventAggregator eventAggregator, string serverName)
        {
            this._eventAggregator = eventAggregator;

            Host = serverName;
            Userid = Settings.Default.xferRtUserId;
            Password = Settings.Default.xferRtUserPwd;
        }

    }
}
