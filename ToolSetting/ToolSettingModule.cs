using Prism.Modularity;
using Prism.Mef.Modularity;
using Prism.Regions;
using XmlCompare.ToolSetting.Views;
using System.ComponentModel.Composition;

namespace XmlCompare.ToolSetting
{
    [ModuleExport(typeof(ToolSettingModule))]
    public class ToolSettingModule : IModule
    {
        IRegionManager _regionManager;

        [ImportingConstructor]
        public ToolSettingModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("ContentRegion", typeof(ToolSettingView));
            _regionManager.RegisterViewWithRegion("ContentRegion", typeof(NewXmlTypeView));
            _regionManager.RegisterViewWithRegion("ContentRegion", typeof(SetAddressView));
            _regionManager.RegisterViewWithRegion("DictionRegion", typeof(EmptyView));
            _regionManager.RegisterViewWithRegion("DictionRegion", typeof(pnlDictionaryEdtPanelView));
        }
    }
}