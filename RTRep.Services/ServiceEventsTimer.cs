using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RTRep.Services.ClsApplicationService;

namespace RTRep.Services
{
    public class ServiceEventsTimer
    {
        protected readonly Prism.Events.IEventAggregator _eventAggregator;

        private System.Windows.Threading.DispatcherTimer RTEventsTimer;
        private System.Windows.Threading.DispatcherTimer WLEventTimer;

        private System.Windows.Threading.DispatcherTimer RTXferEventsTimer;

        private static readonly object padlock = new object();
        private static ServiceEventsTimer instance = null;

        private string RtTimeOne;
        private string RtTimeTwo =  DateTime.Now.ToString("HH:mm");

        private string WlTimeOne;
        private string WlTimeTwo = DateTime.Now.ToString("HH:mm");

      //  private string RtXferTimeOne;
        private string RtXferTimeTwo = DateTime.Now.ToString("HH:mm");

        public static ServiceEventsTimer Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ServiceEventsTimer(ClsApplicationService.Instance.EventAggregator);
                    }
                    return instance;
                }
            }
        }

        public ServiceEventsTimer(IEventAggregator EventAggregator)
        {
            _eventAggregator = EventAggregator;
        }


        #region RT_Timer

        private void InitializeRTEventsTimer(string EventTag)
        {
            if (RTEventsTimer != null) RTEventsTimer = null;
            RTEventsTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(ClsCommon.ScanRate)
            };
            RTEventsTimer.Tick += new EventHandler(RTEventsTimer_Tick);
            RTEventsTimer.Tag = EventTag;
            RTEventsTimer.Start();
        }

        private void RTEventsTimer_Tick(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => { SetRTEventActions(DateTime.Now); }));
        }
        private void SetRTEventActions(DateTime now)
        {
            RtTimeOne = now.ToString("HH:mm:ss");
            if(RtTimeOne != RtTimeTwo)
            {
                _eventAggregator.GetEvent<UpdateBaleRepTimerEvents>().Publish(now);
                RtTimeTwo = RtTimeOne;
            }
        }
        public void StartBaleReportTimer( string EventName)
        {
            InitializeRTEventsTimer(EventName);
        }
        public void StopBaleReportTimer()
        {
            if (RTEventsTimer != null)
            {
                RTEventsTimer.Stop();
                RTEventsTimer = null;
            }
        }

        #endregion RT_Timer


        #region WL_Timer

        private void InitializeWLEventsTimer(string EventTag)
        {
            if (WLEventTimer != null) WLEventTimer = null;
            WLEventTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(ClsCommon.ScanRateWl)
            };
            WLEventTimer.Tick += new EventHandler(WLEventsTimer_Tick);
            WLEventTimer.Tag = EventTag;
            WLEventTimer.Start();
        }

        private void WLEventsTimer_Tick(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => { SetWLEventActions(DateTime.Now); }));
        }
        private void SetWLEventActions(DateTime now)
        {
            WlTimeOne = now.ToString("HH:mm");
            if (WlTimeOne != WlTimeTwo)
            {
                _eventAggregator.GetEvent<UpdateWLRepTimerEvents>().Publish(now);
                WlTimeTwo = WlTimeOne;
            }  
        }

        public void StartWLReportTimer(string EventName)
        {
            InitializeWLEventsTimer(EventName);
        }
        public void StopWLReportTimer()
        {
            if (WLEventTimer != null)
            {
                WLEventTimer.Stop();
                WLEventTimer = null;
            }
        }

        #endregion WL_Timer


        #region RTXferEventsTimer

        private void InitializeXferRTEventsTimer(string EventTag)
        {
            if (RTXferEventsTimer != null) RTXferEventsTimer = null;
            RTXferEventsTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(6)
            };
            RTXferEventsTimer.Tick += new EventHandler(RTXferEventsTimer_Tick);
            RTXferEventsTimer.Tag = EventTag;
            RTXferEventsTimer.Start();
        }

        private void RTXferEventsTimer_Tick(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => { SetRTXferEventActions(DateTime.Now); }));
        }
        private void SetRTXferEventActions(DateTime now)
        {
          _eventAggregator.GetEvent<UpdateRtXferTimerEvents>().Publish(now);   
        }
        public void StartXferRtTimer(string EventName)
        {
            InitializeXferRTEventsTimer(EventName);
        }
        public void StopXferRtTimer()
        {
            if (RTXferEventsTimer != null)
            {
                RTXferEventsTimer.Stop();
                RTXferEventsTimer = null;
            }
        }

        #endregion RTXferEventsTimer

    }
}
