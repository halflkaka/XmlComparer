using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using XmlCompare.Infrastructure.Entity;
using XmlCompare.UseControl.Services;

namespace XmlCompare.UseControl.ViewModels
{
    [Export(typeof(pnlFileCompareListPanelViewModel))]
    public class pnlFileCompareListPanelViewModel : BindableBase
    {
        IEventAggregator _ea;
        public ObservableCollection<CFilePare> itemsList = new ObservableCollection<CFilePare>();
        public CFilePare iFilePare = new CFilePare();
        private CFilePare selectfile = new CFilePare();
        public ObservableCollection<CFilePare> ItemsList
        {
            get { return itemsList; }
            set { itemsList = value;RaisePropertyChanged("ItemsList"); }
        }
        public CFilePare selectFile
        {
            get { return selectfile; }
            set { selectfile = value;RaisePropertyChanged("selectFile"); }
        }
        public void SetFileList(ObservableCollection<CFilePare> iFileList)
        {
            itemsList.Clear();
            foreach (CFilePare tempFilePare in iFileList)
            {
                ItemsList.Add(tempFilePare);
                iFilePare = tempFilePare;
            }
        }
        public ObservableCollection<CFilePare> GetFilePareList()
        {
            ObservableCollection<CFilePare> resultList = new ObservableCollection<CFilePare>();
            foreach (var FilePare in ItemsList)
            {
                if (FilePare.OldFile != "" && FilePare.NewFile != "")
                {
                    resultList.Add(new CFilePare()
                    {
                        OldFile = FilePare.OldFile,
                        NewFile = FilePare.NewFile
                    }
                    );
                }
            }
            return resultList;
        }
        public ICommand PathChanged
        {
            get
            {
                return new MyCommand<CFilePare>(OnPathChanged);
            }
        }
        public void OnPathChanged(CFilePare obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Browse original version file.";
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Xml|*.xml|All Documents|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                ObservableCollection<CFilePare> NewList = new ObservableCollection<CFilePare>();
                foreach (var item in ItemsList)
                {
                    if (item.OldFile == obj.OldFile && item.NewFile == obj.NewFile)
                    {
                        item.OldFile = openFileDialog.FileName;
                    }
                    NewList.Add(item);
                }
                ItemsList = null;
                ItemsList = NewList;
            }
        }
        [ImportingConstructor]
        public pnlFileCompareListPanelViewModel(IEventAggregator ea)
        {
            _ea = ea;
            _ea.GetEvent<FilePareEvent>().Subscribe(SetFileList);
        }
    }
}
