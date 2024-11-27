using Prism.Events;
using System;

namespace RTRep.Services
{
    public sealed class ClsApplicationService
   {
        private ClsApplicationService() { }
        private static readonly ClsApplicationService _instance = new ClsApplicationService();
        public static ClsApplicationService Instance { get { return _instance; } }

        private Prism.Events.IEventAggregator _eventAggregator;

        public Prism.Events.IEventAggregator EventAggregator
        {
            get
            {
                if (_eventAggregator == null)
                    _eventAggregator = new Prism.Events.EventAggregator();
                return _eventAggregator;
            }
        }

        public class UpdatedEvent : PubSubEvent<bool> { }
        
        public class UpdatedEventShutdown : PubSubEvent<bool> { }

        public class UpdatedSqlTableEvent : PubSubEvent<int> { }

        public class UpdatedWLEvent : PubSubEvent<bool> { }

        public class UpdateBaleRepTimerEvents : PubSubEvent<DateTime> { }

        public class UpdateWLRepTimerEvents : PubSubEvent<DateTime> { }

        public class UpdateWLDataEvents : PubSubEvent<bool> { }

        public class ReportPrintEvent : PubSubEvent<string> { }

        public class PrintScheduleEvent : PubSubEvent<int> { }

        public class CloseAppEvent : PubSubEvent<bool> { }

        public class CloseGraphWindowEvent : PubSubEvent<bool> { }

        public class CloseFieldSelWindow : PubSubEvent<bool> { }

        public class CloseFieldModWindow : PubSubEvent<bool> { }

        public class ListViewHdrClickEvent : PubSubEvent<string> { }

        public class LoadListViewDataEvent : PubSubEvent<bool> { }

        public class CloseSqlConnectWindow : PubSubEvent<bool> { }

        public class TestSqlConnection : PubSubEvent<bool> { }

        public class CloseFieldMapWindow : PubSubEvent<string> { }

        public class UpdateRtXferTimerEvents : PubSubEvent<DateTime> { }

        public class UpdateXferStatus : PubSubEvent<string> { }
    }
}
