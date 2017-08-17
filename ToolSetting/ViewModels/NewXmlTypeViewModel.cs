using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using XmlCompare.Infrastructure.Entity;
using XmlCompare.Infrastructure.Interface;

namespace XmlCompare.ToolSetting.ViewModels
{
    [Export(typeof(NewXmlTypeViewModel))]
    public class NewXmlTypeViewModel : BindableBase
    {
        private IEventAggregator _ea;
        private bool m_XmlTypeStatusFlag = false;
        private bool m_XmlSampleStatusFlag = false;
        private bool sampleEnable;
        private IXmlCompareToolSetting ToolSetting;
        private IXmlSettingCollection m_XmlSettingCollection;
        private string samplePath;
        private string xmlType;
        private IRegionManager _regionManager;
        private string checkimage;
        public string Lightimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\checkXmlType.ico";
        public string Browseimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\Browser.ico";
        public string Okimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\OK.ico";
        public string Cancelimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\cancel.ico";
        public string Checkimage
        {
            get { return checkimage; }
            set { checkimage = value;RaisePropertyChanged("Checkimage"); }
        }
        public bool SampleEnable
        {
            get { return sampleEnable; }
            set { sampleEnable = value; RaisePropertyChanged("SampleEnable"); }
        }
        public string XmlType
        {
            get { return xmlType; }
            set { xmlType = value; RaisePropertyChanged("XmlType"); }
        }
        public string SamplePath
        {
            get { return samplePath; }
            set { samplePath = value; RaisePropertyChanged("SamplePath"); }
        }
        public DelegateCommand UploadSampleCommand { get; private set; }
        public DelegateCommand<IXmlSettingCollection> OKCommand { get; private set; }
        public DelegateCommand CancelCommand{get;private set;}
        public DelegateCommand CheckCommand { get; private set; }
        private IXmlSettingFactorty xmlFact { get; set; }
        private IXmlCompareUserControl xmlUserControl { get; set; }
        public bool XmlTypeStatusFlag
        {
            get { return m_XmlTypeStatusFlag; }
            set { m_XmlTypeStatusFlag = value; RaisePropertyChanged("XmlTypeStatusFlag"); }
        }
        public bool XmlSampleStatusFlag
        {
            get { return m_XmlSampleStatusFlag; }
            set { m_XmlSampleStatusFlag = value;RaisePropertyChanged("XmlSampleStatusFlag"); }
        }
        public void UploadSampleCommandExecute()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a Xml file as a Sample";
            openFileDialog.Filter = "Xml|*.xml|All Documents|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                SamplePath = openFileDialog.FileName;
                XmlSampleStatusFlag = true;
            }
        }
        public void OKCommandExecute(IXmlSettingCollection collection)
        {
            if (!XmlTypeStatusFlag)
            {
                MessageBox.Show("Please check your xml type name first!");
                return;
            }
            else if (!XmlSampleStatusFlag)
            {
                MessageBox.Show("Please upload a xml file example to analyze the Nodes and Attribues for this type xml file");
                return;
            }
            else
            {
                //write Nodes and attribute list to the xml setting
                IXmlSetting xmlSetting = xmlFact.InitFromXmlFile(XmlType,
                    SamplePath,
                    xmlUserControl.GetCurrentUser().Id);
                m_XmlSettingCollection =
                xmlFact.ReadSettingCollection(ToolSetting.GetXmlSettingFilePath().Value);
                m_XmlSettingCollection.AppendSetting(xmlSetting);
                _ea.GetEvent<CollectionEvent>().Publish(m_XmlSettingCollection);
                XmlType = null;
                SamplePath = null;
                XmlTypeStatusFlag = false;
                Checkimage = null;
                _regionManager.RequestNavigate("ContentRegion", "ToolSettingView");
            }
        }
        public void Check()
        {
            m_XmlSettingCollection =
                xmlFact.ReadSettingCollection(ToolSetting.GetXmlSettingFilePath().Value);
            IXmlSetting tempSetting = m_XmlSettingCollection.GetSetting(XmlType);
            if (tempSetting == null && XmlType != null)
            {
                m_XmlTypeStatusFlag = true;
                Checkimage = System.IO.Directory.GetCurrentDirectory() + "\\Images\\xmlTypeOK.ico";
            }
            else
            {
                m_XmlTypeStatusFlag = false;
                Checkimage = System.IO.Directory.GetCurrentDirectory() + "\\Images\\xmlTypeKO.ico";
            }
        }
        public void Cancel()
        {
            XmlType = null;
            SamplePath = null;
            XmlTypeStatusFlag = false;
            Checkimage = null;
            _regionManager.RequestNavigate("ContentRegion", "ToolSettingView");
        }
        [ImportingConstructor] 
        public NewXmlTypeViewModel(IEventAggregator ea,IXmlCompareToolSetting _ToolSetting,IXmlSettingCollection xmlSettingCollection,IRegionManager regionManager, IXmlSettingFactorty _xmlFact, IXmlCompareUserControl _xmlUserControl)
        {
            _regionManager = regionManager;
            xmlFact = _xmlFact;
            ToolSetting = _ToolSetting;
            xmlUserControl = _xmlUserControl;
            SampleEnable = false;
            m_XmlSettingCollection = xmlSettingCollection;
            UploadSampleCommand = new DelegateCommand(UploadSampleCommandExecute);
            OKCommand = new DelegateCommand<IXmlSettingCollection>(OKCommandExecute);
            CancelCommand = new DelegateCommand(Cancel);
            CheckCommand = new DelegateCommand(Check);
            _ea = ea;
        }
    }
}
