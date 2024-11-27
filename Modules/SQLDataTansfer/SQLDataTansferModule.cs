using SQLDataTansfer.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace SQLDataTansfer
{
    public class SQLDataTansferModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegionX", typeof(DataTransferView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}