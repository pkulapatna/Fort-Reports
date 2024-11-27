using BaleReports.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace BaleReports
{
    public class BaleReportsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion2", typeof(BaleReportView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}