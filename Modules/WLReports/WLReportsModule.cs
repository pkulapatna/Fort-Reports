using WLReports.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace WLReports
{
    public class WLReportsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion3", typeof(WLReportView));

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}