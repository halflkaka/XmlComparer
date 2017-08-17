using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.ComponentModel.Composition;
using XmlCompare.Core.Views;

namespace XmlCompare.Core
{
    [ModuleExport(typeof(CoreModule))]
    public class CoreModule : IModule
    {
        IRegionManager _regionManager;

        [ImportingConstructor]
        public CoreModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("ContentRegion", typeof(CoreView));
            //_regionManager.RegisterViewWithRegion("ContentRegion", typeof(ExcelView));
        }
    }
}