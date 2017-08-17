using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.ComponentModel.Composition;
using XmlCompare.UseControl.Views;

namespace XmlCompare.UseControl
{
    [ModuleExport(typeof(UseControlModule))]
    public class UseControlModule : IModule
    {
        IRegionManager _regionManager;

        [ImportingConstructor]
        public UseControlModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("DictionRegion", typeof(frmCmpFileListView));
            _regionManager.RegisterViewWithRegion("ContentRegion", typeof(pnlFileCompareListPanelView));
        }
    }
}