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

namespace XmlCompare.UseControl.ViewModels
{
    [Export(typeof(frmCmpFileListViewModel))]
    public class frmCmpFileListViewModel : BindableBase
    {
        IEventAggregator _ea;
        public DelegateCommand CancelCommand { get; private set; }
        public DelegateCommand OKCommand { get; private set; }
        private IRegionManager _regionManager;
        private string m_OutputDir;
        private string m_XmlType;
        private pnlFileCompareListPanelViewModel pnlFileCompare { get; set; }
        private IXmlCompareCore core;
        public string Okimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\OK.ico";
        public string Cancelimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\cancel.ico";
        public string OutputDir
        {
            get { return m_OutputDir; }
            set { m_OutputDir = value; RaisePropertyChanged("OutputDir"); }
        }
        public string XmlType
        {
            get { return m_XmlType; }
            set { m_XmlType = value; RaisePropertyChanged("XmlType"); }
        }
        public void SetFileList(List<CFilePare> iFileList)
        {
            pnlFileCompare.SetFileList(new ObservableCollection<CFilePare>(iFileList));
        }
        public void SetOutputInformation(string iXmlType, string iOutputDir)
        {
            OutputDir = iOutputDir;
            XmlType = iXmlType;
        }
        public void CancelCommandExecute()
        {
            _regionManager.RequestNavigate("ContentRegion", "CoreView");
            _regionManager.RequestNavigate("DictionRegion", "EmptyView");
        }
        public void Getfiletype(ObservableCollection<string> type)
        {
            XmlType = type[0];
            OutputDir = type[1];
        }
        public void OKCommandExecute()
        {
            if (OutputDir.Length == 0)
            {
                MessageBox.Show("m_OutputDir is not set. could not execute batch compare function");
                return;
            }
            if (XmlType.Length == 0)
            {
                MessageBox.Show("m_XmlType is not set. could not execute batch compare function");
                return;
            }
            List<CFilePare> listFilePare = pnlFileCompare.GetFilePareList().ToList();
            core.BatchCompare(listFilePare, XmlType, OutputDir);
            MessageBox.Show("Batch Compare Done, Please check " + m_OutputDir + "\\compare_log.txt" + " for detail");
            _regionManager.RequestNavigate("ContentRegion", "CoreView");
            _regionManager.RequestNavigate("DictionRegion", "EmptyView");
        }
        [ImportingConstructor]
        public frmCmpFileListViewModel( IEventAggregator ea,IXmlCompareCore _core, IRegionManager regionManager, pnlFileCompareListPanelViewModel _pnlFileCompare)
        {
            core = _core;
            _ea = ea;
            _ea.GetEvent<InfoEvent>().Subscribe(Getfiletype);
            pnlFileCompare = _pnlFileCompare;
            _regionManager = regionManager;
            CancelCommand = new DelegateCommand(CancelCommandExecute);
            OKCommand = new DelegateCommand(OKCommandExecute);
        }

    }
}
