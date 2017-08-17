using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using XmlCompare.Infrastructure.Interface;

namespace XmlCompare.ToolSetting.ViewModels
{
    [Export(typeof(SetAddressViewModel))]
    public class SetAddressViewModel : BindableBase
    {
        private IXmlCompareToolSetting m_setting;
        private IRegionManager _regionmanager;
        private string setFileFullAddress;
        public DelegateCommand<string> CancelCommand { get; set; }
        public DelegateCommand SetFileFullPathCommand { get; set; }
        public DelegateCommand<string> OKCommand { get; set; }
        public string Browseimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\Browser.ico";
        public string Okimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\OK.ico";
        public string Cancelimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\cancel.ico";
        public string SetFileFullAddress
        {
            get { return setFileFullAddress; }
            set { setFileFullAddress = value;RaisePropertyChanged("SetFileFullAddress"); }
        }
        public void CancelCommandExecute(string navigatePath)
        {
            if(navigatePath != null)
            {
                _regionmanager.RequestNavigate("ContentRegion", "CoreView");
            }
        }
        public void SetFileFullPathCommandExecute()
        {
            System.Windows.Forms.FolderBrowserDialog folderbrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderbrowser.ShowNewFolderButton = false;
            System.Windows.Forms.DialogResult result = folderbrowser.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SetFileFullAddress = folderbrowser.SelectedPath;
            }
            
        }
        public void OKCommandExecute(string navigatePath)
        {
            if (SetFileFullAddress != m_setting.GetXmlSettingFilePath().Value)
            {
                DialogResult flag = System.Windows.Forms.MessageBox.Show("Do you want to change XmlSettingFilePath setting?",
                    "Setting Change Confirm",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning);
                if (flag == DialogResult.OK)
                {
                    m_setting.SaveXmlSettingFilePath(SetFileFullAddress);
                    System.Windows.MessageBox.Show("Setting has been submitted.");
                }
                if(navigatePath != null)
                {
                    _regionmanager.RequestNavigate("ContentRegion", "CoreView");
                }
            }
        }
        [ImportingConstructor]
        public SetAddressViewModel(IRegionManager regionManager, IXmlCompareToolSetting setting)
        {
            _regionmanager = regionManager;
            m_setting = setting;
            SetFileFullAddress = m_setting.GetXmlSettingFilePath().Value;
            CancelCommand = new DelegateCommand<string>(CancelCommandExecute);
            SetFileFullPathCommand = new DelegateCommand(SetFileFullPathCommandExecute);
            OKCommand = new DelegateCommand<string>(OKCommandExecute);
        }
    }
}
