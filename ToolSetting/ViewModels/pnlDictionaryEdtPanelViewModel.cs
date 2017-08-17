using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using XmlCompare.Core.Services;
using XmlCompare.Infrastructure.Entity;
using XmlCompare.Infrastructure.Interface;
using XmlCompare.ToolSetting.Services;
using XmlCompare.UseControl.Services;

namespace XmlCompare.ToolSetting.ViewModels
{
    [Export(typeof(pnlDictionaryEdtPanelViewModel))]
    public class pnlDictionaryEdtPanelViewModel : BindableBase
    {
        private IEventAggregator _ea;
        private IXmlCompareToolSetting _ToolSetting = null;
        private IXmlCompareUserControl _UserControl = null;
        private string nodetype;

        public IXmlSettingNode Currentnode = null;
        public IXmlSetting _CurrentSetting = null;
        public IXmlSettingReportDictionary _CurrentReportDictionary = null;
        [Import]
        public IXmlSettingReportDictionary _currentReportDictionary = null;
        public bool _editable = false;
        public bool NodeNameEnable { get; set; }
        public bool SourceEditable { get; set; }
        public bool CategoryEditable { get; set; }
        public bool InstructionEnable { get; set; }
        public bool AddEnable { get; set; }
        public bool DeleteEnable { get; set; }
        public bool UpdateEnable { get; set; }
        public bool UpdateGeneralEnable { get; set; }
        public bool GeneralNotesEnable { get; set; }
        public DelegateCommand UpdateCommand { get; set; }
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand UpdateGeneralCommand { get; set; }
        public string Okimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\OK.ico";
        public string Addimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\add.ico";
        public string Deleteimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\delete.ico";

        private string rpt_issueSource;
        private string rpt_issueCategory;
        private string rpt_issueInstruction;
        private string generalNotesSetting;
        private string sourceSelected;
        private ObservableCollection<string> issueSourceList = new ObservableCollection<string>();
        private ObservableCollection<string> issue_SourceList = new ObservableCollection<string>();
        private ObservableCollection<IXmlSettingReportDictionary> sourcelist = new ObservableCollection<IXmlSettingReportDictionary>();
        private IXmlSettingReportDictionary selecteditems;
        private ObservableCollection<string> rptIssueCategoryList = new ObservableCollection<string>();
        private ObservableCollection<string> _IssueCategoryList_1 = new ObservableCollection<string>();
        private ObservableCollection<string> _IssueCategoryList_2 = new ObservableCollection<string>();

        public string Nodetype
        {
            get { return nodetype; }
            set
            {
                nodetype = value;
                if(nodetype != null)
                {
                    if(CurrentNode != null)
                    {
                        Sourcelist.Clear();
                    }
                    if(_CurrentSetting != null)
                    {
                        RefreshGeneralNodes();
                    }
                }
                RaisePropertyChanged("Nodetype");
            }
        }
        public ObservableCollection<string> Issue_SourceList
        {
            get { return issue_SourceList; }
            set { issue_SourceList = value;RaisePropertyChanged("Issue_SourceList"); }
        }
        public ObservableCollection<string> IssueSourceList
        {
            get { return issueSourceList; }
            set { issueSourceList = value;RaisePropertyChanged("IssueSourceList"); }
        }
        public IXmlSettingNode CurrentNode
        {
            get { return Currentnode; }
            set
            {
                Currentnode = value;
                if(Currentnode != null)
                {
                    Rpt_issueInstruction = "";
                    RefreshTreeListView();
                    if(_CurrentSetting != null)
                    {
                        RefreshGeneralNodes();
                    }
                }
                RaisePropertyChanged("CurrentNode");
            }
        }
        public string SourceSelected
        {
            get { return sourceSelected; }
            set
            {
                sourceSelected = value;
                RptIssueCategoryList = null;
                if (sourceSelected == "Parent Node <" + CurrentNode.name + "_...>")
                {
                    RptIssueCategoryList = _IssueCategoryList_2;
                }
                else if (sourceSelected == "<" + CurrentNode.name + "_...>")
                {
                    RptIssueCategoryList = _IssueCategoryList_1;
                }
                RaisePropertyChanged("SourceSelected");
            }
        }
        public ObservableCollection<string> RptIssueCategoryList
        {
            get { return rptIssueCategoryList; }
            set
            {
                rptIssueCategoryList = value;
                RaisePropertyChanged("RptIssueCategoryList");
            }
        }
        public IXmlSettingReportDictionary Selecteditems
        {
            get { return selecteditems; }
            set
            {
                selecteditems = value;
                _CurrentReportDictionary = null;
                //When select an item
                if (selecteditems != null)
                {
                    var tlvSelectedItem = selecteditems;
                    _CurrentReportDictionary = CurrentNode.GetDictionary(
                        (XmlSettingRptDicCategory)Enum.Parse(typeof(XmlSettingRptDicCategory),
                        tlvSelectedItem.issue_category));
                }
                if (_CurrentReportDictionary != null)
                {
                    Rpt_issueSource = _CurrentReportDictionary.issue_source;
                    Rpt_issueCategory = _CurrentReportDictionary.issue_category;
                    Rpt_issueInstruction = _CurrentReportDictionary.issue_instruction;
                }
                else
                {
                    Rpt_issueSource = "";
                    Rpt_issueCategory = "";
                    Rpt_issueInstruction = "";
                }
                RaisePropertyChanged("Selecteditems");
            }
        }
        public ObservableCollection<IXmlSettingReportDictionary> Sourcelist
        {
            get { return sourcelist; }
            set { sourcelist = value;RaisePropertyChanged("Sourcelist"); }
        }
        public string Rpt_issueSource
        {
            get { return rpt_issueSource; }
            set { rpt_issueSource = value;RaisePropertyChanged("Rpt_issueSource"); }
        }
        public string Rpt_issueCategory
        {
            get { return rpt_issueCategory; }
            set { rpt_issueCategory = value;
                RaisePropertyChanged("Rpt_issueCategory"); }
        } 
        public string Rpt_issueInstruction
        {
            get { return rpt_issueInstruction; }
            set { rpt_issueInstruction = value;RaisePropertyChanged("Rpt_issueInstruction"); }
        }
        public string GeneralNotesSetting
        {
            get { return generalNotesSetting; }
            set { generalNotesSetting = value;RaisePropertyChanged("GeneralNotesSetting"); }
        }
        public bool Editable
        {
            get
            {
                return _editable;
            }
            set
            {
                if (value == true)
                {
                    NodeNameEnable = true;
                    SourceEditable = true;
                    CategoryEditable = true;
                    InstructionEnable = true;
                    AddEnable = true;
                    DeleteEnable = true;
                    UpdateEnable = true;
                    UpdateGeneralEnable = true;
                    GeneralNotesEnable = true;
                    _editable = true;
                }
                else
                {
                    NodeNameEnable = false;
                    SourceEditable = false;
                    CategoryEditable = false;
                    InstructionEnable = false;
                    AddEnable = false;
                    DeleteEnable = false;
                    UpdateEnable = false;
                    UpdateGeneralEnable = false;
                    GeneralNotesEnable = false;
                    _editable = false;
                }
            }
        }
        public void BindDictionary(ref IXmlSettingNode iNode, ref IXmlSetting iSetting)
        {
            CurrentNode = iNode;
            _CurrentSetting = iSetting;
        }

        private void RefreshTreeListView()
        {
            if(Sourcelist != null) {Sourcelist.Clear(); }
            foreach (IXmlSettingReportDictionary item in CurrentNode.GetDictionarys)
            {
                item.ImageSource = "C:\\Users\\LTC00471\\Desktop\\XMLMain\\XmlCompare\\Images\\dictionary.ico";
                Sourcelist.Add(item);
            }
        }
        private void RefreshGeneralNodes()
        {
            GeneralNotesSetting = string.Join("\r\n", _CurrentSetting.GeneralNotes.ToArray());
        }
        public void UpdateCommandExecute()
        {
            if (_CurrentReportDictionary != null)
            {
                if(Rpt_issueSource == null) { MessageBox.Show("Error!"); }
                else
                {
                    _CurrentReportDictionary.issue_source =
                    System.Text.RegularExpressions.Regex.Replace(Rpt_issueSource, "[\\r\\v\\n\\f\\t]", "");
                    _CurrentReportDictionary.issue_category =
                        System.Text.RegularExpressions.Regex.Replace(Rpt_issueCategory, "[\\r\\v\\n\\f\\t]", "");
                    _CurrentReportDictionary.issue_instruction =
                        System.Text.RegularExpressions.Regex.Replace(Rpt_issueInstruction, "[\\r\\v\\n\\f\\t]", "");
                    RefreshTreeListView();
                    MessageBox.Show("Report Issue Dictionary Updateded!");
                }
            }
        }
        //Import???
        public void AddCommandExecute()
        {
            CXmlSettingRptDicImp dic = new CXmlSettingRptDicImp();
            dic.node_name = CurrentNode.name;
            if(Rpt_issueSource == null) { MessageBox.Show("Error!"); }
            else
            {
                dic.issue_source =
                System.Text.RegularExpressions.Regex.Replace(Rpt_issueSource, "[\\r\\v\\n\\f\\t]", "");
                dic.issue_category =
                    System.Text.RegularExpressions.Regex.Replace(Rpt_issueCategory, "[\\r\\v\\n\\f\\t]", "");
                dic.issue_instruction =
                    System.Text.RegularExpressions.Regex.Replace(Rpt_issueInstruction, "[\\r\\v\\n\\f\\t]", "");
                CurrentNode.AppendDictionary(dic);
                RefreshTreeListView();
                MessageBox.Show("Report Issue Dictionary Added!");
            }
            
        }
        public void DeleteCommandExecute()
        {
            if (this._CurrentReportDictionary != null)
            {
                Rpt_issueSource = "";
                Rpt_issueCategory = "";
                Rpt_issueInstruction = "";
                CurrentNode.RemoveDictionary(_CurrentReportDictionary);
                _CurrentReportDictionary = null;
                RefreshTreeListView();
            }
            MessageBox.Show("Report Issue Dictionary Removed!");
        }
        public void UpdateGeneralCommandExecute()
        {
            if(GeneralNotesSetting == null) { MessageBox.Show("Error!"); }
            else
            {
                string[] tmpArray = GeneralNotesSetting.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                _CurrentSetting.GeneralNotes.Clear();
                foreach (string tmpStr in tmpArray)
                {
                    _CurrentSetting.GeneralNotes.Add(System.Text.RegularExpressions.Regex.Replace(tmpStr, "[\\r\\v\\n\\f\\t]", ""));
                }
                RefreshGeneralNodes();
                MessageBox.Show("General Notes Updateded!");
            }
        }
        public void GetNode(IXmlSettingNode node)
        {
            CurrentNode = node;
        }
        public void GetSource(ObservableCollection<string> source)
        {
            IssueSourceList = source;
            Rpt_issueSource = IssueSourceList[0];
            if (Rpt_issueSource == "Parent Node <" + CurrentNode.name + "_...>")
            {
                rptIssueCategoryList = _IssueCategoryList_2;
            }
            else if (Rpt_issueSource == "<" + CurrentNode.name + "_...>")
            {
                rptIssueCategoryList = _IssueCategoryList_1;
            }
        }
        public void GetSetting(IXmlSetting setting)
        {
            _CurrentSetting = setting;
        }
        public void GetNodetype(string type)
        {
            Nodetype = type;
        }
        [ImportingConstructor]
        public pnlDictionaryEdtPanelViewModel(IEventAggregator ea,IXmlCompareToolSetting ToolSetting, IXmlCompareUserControl UserControl,IXmlSettingReportDictionary CurrentReportDictionary, IXmlSettingNode _CurrentNode, IXmlSetting CurrentSetting)
        {
            _ea = ea;
            _ea.GetEvent<NodeEvent>().Subscribe(GetNode);
            _ea.GetEvent<SourceListEvent>().Subscribe(GetSource);
            _ea.GetEvent<SettingEvent>().Subscribe(GetSetting);
            _ea.GetEvent<SourceEvent>().Subscribe(GetNodetype);

            _ToolSetting = ToolSetting;
            _UserControl = UserControl;
            CurrentNode = _CurrentNode;
            _CurrentReportDictionary = CurrentReportDictionary;

            UpdateCommand = new DelegateCommand(UpdateCommandExecute);
            DeleteCommand = new DelegateCommand(DeleteCommandExecute);
            AddCommand = new DelegateCommand(AddCommandExecute);
            UpdateGeneralCommand = new DelegateCommand(UpdateGeneralCommandExecute);

            Editable = _UserControl.IsUserHasPriviledge(_UserControl.GetCurrentUser(), "XmlSettingFilePath");

            _IssueCategoryList_1.Add(XmlSettingRptDicCategory.ChangedAttribute.ToString());
            _IssueCategoryList_1.Add(XmlSettingRptDicCategory.DeletedAttribute.ToString());
            _IssueCategoryList_1.Add(XmlSettingRptDicCategory.AddedAttribute.ToString());

            _IssueCategoryList_2.Add(XmlSettingRptDicCategory.DeletedChildNode.ToString());
            _IssueCategoryList_2.Add(XmlSettingRptDicCategory.AddedChildNode.ToString());
        }
    }
}
