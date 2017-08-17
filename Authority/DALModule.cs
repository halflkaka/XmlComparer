using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using System;
using XmlCompare.DAL.Views;
using System.ComponentModel.Composition;

namespace XmlCompare.DAL
{
    [ModuleExport(typeof(DALModule))]
    public class DALModule : IModule
    {
        IRegionManager _regionManager;

        [ImportingConstructor]
        public DALModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("ContentRegion", typeof(DALView));
        }
    }
}