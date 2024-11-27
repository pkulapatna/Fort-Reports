using BaleReports;
using Prism.Events;
using RTRep.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RTRep.Services.ClsApplicationService;


namespace ForteReports.Model
{
    public class ForteReportModel
    {
        protected readonly IEventAggregator _eventAggregator;

        private ClsSQLhandler Sqlhandler;

        public ForteReportModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
            Sqlhandler = ClsSQLhandler.Instance;

           // _= QuartzSched.DayEndEvent();
        }

        internal bool CheckWlSystem()
        {
            return Sqlhandler.FindSqlDatabase("ForteLayer");
        }

    }
}
