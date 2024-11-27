using RepSetup.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace RepSetup
{
    public class RepSetupModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion1", typeof(SetupView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}