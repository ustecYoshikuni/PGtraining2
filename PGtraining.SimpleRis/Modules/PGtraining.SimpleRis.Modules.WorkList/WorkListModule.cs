using PGtraining.SimpleRis.Core;
using PGtraining.SimpleRis.Modules.WorkList.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace PGtraining.SimpleRis.Modules.WorkList
{
    public class WorkListModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public WorkListModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, "WorkList");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.WorkList>();
        }
    }
}