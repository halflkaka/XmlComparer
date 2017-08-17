using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using XmlCompare.Core.Services;
using XmlCompare.Infrastructure.Entity;
using XmlCompare.Infrastructure.Interface;
using XmlCompare.UseControl.ViewModels;

namespace XmlCompare.Core.ViewModels
{
    [Export(typeof(CoreViewModel))]
    public class CoreViewModel:BindableBase
    {
        private IEventAggregator _ea;
        public DelegateCommand OriginalPathCommand { get; private set; }
        public DelegateCommand NewPathCommand { get; private set; }
        public DelegateCommand OutputPathCommand { get; private set; }
        public DelegateCommand<string> CompareReportCommand { get; private set; }
        public DelegateCommand ChangetextCommand { get; private set; }
        public string Compareimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\Compute.ico";
        public string Browseimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\Browser.ico";
        public string Outimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\FolderBrowser_s.ico";
        private IRegionManager _regionManager;
        private IXmlSettingCollection settingCollection = null;
        private string originalPath;
        private string newPath;
        private string outputPath;
        private bool compareFile = true;
        private bool compareFolder = false;
        private string selectedtype;
        private string oldFolderOrFile = "Original File:";
        private string newFolderOrFile = "New File:";
        private ObservableCollection<string> filetype;
        private pnlFileCompareListPanelViewModel _pnlFileCompare { get; }
        public string select { get; set; }
        public string Selectedtype
        {
            get { return selectedtype; }
            set { selectedtype = value;RaisePropertyChanged("Selectedtype"); }
        }
        public ObservableCollection<string> Filetype
        {
            get { return filetype; }
            set { filetype = value;RaisePropertyChanged("Filetype"); }
        }
        public string OldFolderOrFile
        {
            get { return oldFolderOrFile; }
            set
            {
                oldFolderOrFile = value;
                RaisePropertyChanged("OldFolderOrFile");
            }
        }
        public string NewFolderOrFile
        {
            get { return newFolderOrFile; }
            set
            {
                newFolderOrFile = value;
                RaisePropertyChanged("NewFolderOrFile");
            }
        }
        public bool CompareFile
        {
            get { return compareFile; }
            set
            {
                compareFile = value;
                RaisePropertyChanged("CompareFile");
            }
        }
        public bool CompareFolder
        {
            get { return compareFolder; }
            set
            {
                compareFolder = value;
                RaisePropertyChanged("CompareFolder");
            }
        }
        public string OriginalPath
        {
            get { return originalPath; }
            set
            {
                originalPath = value;
                RaisePropertyChanged("OriginalPath");
            }
        }
        public string NewPath
        {
            get { return newPath; }
            set
            {
                newPath = value;
                RaisePropertyChanged("NewPath");
            }
        }
        public string OutputPath
        {
            get { return outputPath; }
            set
            {
                outputPath = value;
                RaisePropertyChanged("OutputPath");
            }
        }
        public void OriginalPathCommandExecute()
        {
            if(CompareFile == true)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Xml|*.xml|All Documents|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    OriginalPath = openFileDialog.FileName;
                }
            }
            else if (CompareFolder == true)
            {
                System.Windows.Forms.FolderBrowserDialog folderbrowser = new System.Windows.Forms.FolderBrowserDialog();
                folderbrowser.ShowNewFolderButton = false;
                System.Windows.Forms.DialogResult result = folderbrowser.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    OriginalPath = folderbrowser.SelectedPath;
                }
            }
        }
        public void ChangetextCommandExecute()
        {
            if (CompareFolder == true)
            {
                NewFolderOrFile = "New Files Folder:";
                OldFolderOrFile = "Original Files Folder:";
            }
            else
            {
                NewFolderOrFile = "New File:";
                OldFolderOrFile = "Original File:";
            }
        }
        public void NewPathCommandExecute()
        {
            if(CompareFile == true)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Please select a xml file as origin file";
                openFileDialog.Multiselect = false;
                openFileDialog.Filter = "Xml|*.xml|All Documents|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    NewPath = openFileDialog.FileName;
                }
            }
            else if(CompareFolder == true)
            {
                System.Windows.Forms.FolderBrowserDialog folderbrowser = new System.Windows.Forms.FolderBrowserDialog();
                folderbrowser.ShowNewFolderButton = false;
                System.Windows.Forms.DialogResult result = folderbrowser.ShowDialog();
                if(result == System.Windows.Forms.DialogResult.OK)
                {
                    NewPath = folderbrowser.SelectedPath;
                }
            }
        }
        public void OutputPathCommandExecute()
        {
            System.Windows.Forms.FolderBrowserDialog folderbrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderbrowser.ShowNewFolderButton = false;
            System.Windows.Forms.DialogResult result = folderbrowser.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                OutputPath = folderbrowser.SelectedPath;
            }
        }
        public IXmlCompareCore core;
        public void CompareReportCommandExecute(string navigatePath)
        {
            if(OutputPath != null)
            {
                ObservableCollection<string> ll = new ObservableCollection<string>();
                ll.Add(Selectedtype);
                ll.Add(OutputPath);
                _ea.GetEvent<InfoEvent>().Publish(ll);
                if (CompareFile == true)
                {
                    if(Selectedtype != "" && NewPath != "" && OriginalPath != "" && OutputPath != "")
                    {
                        core.Compare(OriginalPath, NewPath, Selectedtype, OutputPath);
                        MessageBox.Show("Compare function execution finished.");
                    }
                    else { MessageBox.Show("Input Error!"); }
                }
                else if(CompareFolder == true)
                {
                    List<CFilePare> listOfFilePare = core.GetFilePare(OriginalPath, NewPath);
                    _ea.GetEvent<FilePareEvent>().Publish(new ObservableCollection<CFilePare>(listOfFilePare));
                    _regionManager.RequestNavigate("ContentRegion", "pnlFileCompareListPanelView");
                    _regionManager.RequestNavigate("DictionRegion", "frmCmpFileListView");
                }
            }
            else
            {
                MessageBox.Show("Please Select an output path!");
            }
        }
        private void GetCollection(IXmlSettingCollection Collection)
        {
            settingCollection = Collection;
            Filetype = new ObservableCollection<string>(settingCollection.GetAllSettingTypes());
            Selectedtype = Filetype[0];
        }
        [ImportingConstructor]
        public CoreViewModel(IXmlCompareCore _core, IRegionManager regionManager,IEventAggregator ea)
        {
            core = _core;
            _ea = ea;
            _ea.GetEvent<CollectionEvent>().Subscribe(GetCollection);
            _regionManager = regionManager;
            OriginalPathCommand = new DelegateCommand(OriginalPathCommandExecute);
            NewPathCommand = new DelegateCommand(NewPathCommandExecute);
            OutputPathCommand = new DelegateCommand(OutputPathCommandExecute);
            CompareReportCommand = new DelegateCommand<string>(CompareReportCommandExecute);
            ChangetextCommand = new DelegateCommand(ChangetextCommandExecute);
            if (core.Initialize(out settingCollection))
            {
                Filetype = new ObservableCollection<string>(settingCollection.GetAllSettingTypes());
                Selectedtype = Filetype[0];
                select = Filetype[0];
                _ea.GetEvent<SourceEvent>().Publish(select);
            }
        }
    }
}
