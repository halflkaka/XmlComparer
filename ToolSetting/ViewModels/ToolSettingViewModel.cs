using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;
using XmlCompare.Infrastructure.Entity;
using XmlCompare.Infrastructure.Interface;
using XmlCompare.ToolSetting.Services;
using XmlCompare.UseControl.Services;

namespace XmlCompare.ToolSetting.ViewModels
{
    [Export]
    public class ToolSettingViewModel : BindableBase
    {
        private IEventAggregator _ea;
        private IXmlSettingCollection m_CurrentSettingCollection = null;
        private IXmlCompareToolSetting m_ToolSetting = null;
        private IXmlCompareUserControl m_UserControl = null;
        private IXmlSettingFactorty m_SettingFact = null;
        private IXmlSettingNode currentNode;
        private ObservableCollection<string> issueSourceList = new ObservableCollection<string>();
        private IXmlSettingNode Node;
        private IXmlSetting CurrentSetting;
        private IXmlSetting currentXmlSetting = null;
        public IXmlSettingReportDictionary _CurrentReportDictionary;
        private pnlDictionaryEdtPanelViewModel m_DictionaryEdtPanel = null;
        private IRegionManager _regionManager;

        private string xmlFileType;
        private string chkContent;
        private string selectType;
        private bool chkMoreSelected;

        private ObservableCollection<string> xmlTypeList = new ObservableCollection<string>();
        private ObservableCollection<IXmlSettingNode> nodesList = new ObservableCollection<IXmlSettingNode>();
        private IXmlSettingNode nodeSelected;
        private IXmlSettingAttribute attrSelected;
        private ObservableCollection<IXmlSettingAttribute> attributeList = new ObservableCollection<IXmlSettingAttribute>();
        private bool _flag;

        public string Homeimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\XmlCompare.ico";
        public string Addimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\add.ico";
        public string Deleteimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\delete.ico";
        public string Saveimage { get; set; } = System.IO.Directory.GetCurrentDirectory() + "\\Images\\save.ico";
        public ObservableCollection<string> IssueSourceList
        {
            get { return issueSourceList; }
            set { issueSourceList = value;RaisePropertyChanged("IssueSourceList"); }
        }
        public IXmlSettingNode CurrentNode
        {
            get { return currentNode; }
            set
            {
                if(NodeSelected != null)
                {
                    currentNode = NodeSelected;
                }
                RaisePropertyChanged("CurrentNode");
            }
        }
        public IXmlSettingCollection CurrentSettingCollection
        {
            get { return m_CurrentSettingCollection; }
            set {
                m_CurrentSettingCollection = value;
                RaisePropertyChanged("CurrentSettingCollection");
            }
        }
        public bool Flag
        {
            get { return _flag; }
            set { _flag = value;RaisePropertyChanged("flag"); }
        }
        public ObservableCollection<IXmlSettingAttribute> AttributeList
        {
            get { return attributeList; }
            set { attributeList = value;RaisePropertyChanged("AttributeList"); }
        }
        public IXmlSettingNode NodeSelected
        {
            get { return nodeSelected; }
            set {
                nodeSelected = value;
                if (nodeSelected != null)
                {
                    CurrentNode = nodeSelected;
                    CurrentSetting = m_CurrentSettingCollection
                                                .GetSetting(XmlFileType);
                    _ea.GetEvent<SettingEvent>().Publish(CurrentSetting);
                    _ea.GetEvent<NodeEvent>().Publish(CurrentNode);
                    var tlvSelectedItem = nodeSelected;
                    List<IXmlSettingAttribute> lstAttribute = m_CurrentSettingCollection
                                                            .GetSetting(SelectType)
                                                            .GetNode(tlvSelectedItem.name).GetAttributes;
                    // update tlvAttribute View
                    AttributeList.Clear();
                    foreach (IXmlSettingAttribute tmpAttribute in lstAttribute)
                    {
                        if (tmpAttribute.identifier_flag)
                        {
                            tmpAttribute.Idtflag = "1";
                            tmpAttribute.IdtflagImage = System.IO.Directory.GetCurrentDirectory() + "\\Images\\key_flag_true.png";
                        }
                        else
                        {
                            tmpAttribute.Idtflag = "0";
                            tmpAttribute.IdtflagImage = System.IO.Directory.GetCurrentDirectory() + "\\Images\\key_flag_false.png";
                        }
                        if (tmpAttribute.compare_flag)
                        {
                            tmpAttribute.Compare_flag = "1";
                            tmpAttribute.compareImage = System.IO.Directory.GetCurrentDirectory() + "\\Images\\compare_flag_true.png";
                        }
                        else
                        {
                            tmpAttribute.Compare_flag = "0";
                            tmpAttribute.compareImage = System.IO.Directory.GetCurrentDirectory() + "\\Images\\compare_flag_false.png";
                        }
                        tmpAttribute.AttributeImage = System.IO.Directory.GetCurrentDirectory() + "\\Images\\attribute.png";
                        AttributeList.Add(tmpAttribute);
                    }
                    // update ufmDictionaryEdtPanel View
                    Node = m_CurrentSettingCollection
                                                .GetSetting(SelectType)
                                                .GetNode(tlvSelectedItem.name);
                    IssueSourceList.Clear();
                    IssueSourceList.Add("<" + Node.name + "_...>");
                    IssueSourceList.Add("Parent Node <" + Node.name + "_...>");
                    _ea.GetEvent<SourceListEvent>().Publish(IssueSourceList);
                }
                RaisePropertyChanged("NodeSelected"); }
        }
        public IXmlSettingAttribute AttrSelected
        {
            get { return attrSelected;}
            set { attrSelected = value;RaisePropertyChanged("AttrSelected"); }
        }
        public ObservableCollection<IXmlSettingNode> NodesList
        {
            get { return nodesList; }
            set { nodesList = value;RaisePropertyChanged("NodesList"); }
        }
        public ObservableCollection<string> XmlTypeList
        {
            get { return xmlTypeList; }
            set {
                xmlTypeList = value;
                RaisePropertyChanged("XmlTypeList");
                }
        }
        public string SelectType
        {
            get { return selectType; }
            set
            {
                selectType = value;
                if(selectType != null)
                {
                    _ea.GetEvent<SourceEvent>().Publish(selectType);
                    currentXmlSetting = m_CurrentSettingCollection.GetSetting(SelectType);
                    BindgingSettingNodeToNodeTlv(currentXmlSetting, "");
                }
                RaisePropertyChanged("SelectType");
            }
        }
        public string ChkContent
        {
            get { return chkContent; }
            set { chkContent = value;RaisePropertyChanged("ChkContent"); }
        }
        public string XmlFileType
        {
            get { return xmlFileType; }
            set { xmlFileType = value;RaisePropertyChanged("XmlFileType"); }
        }
        public bool ChkMoreSelected
        {
            get { return chkMoreSelected; }
            set { chkMoreSelected = value;RaisePropertyChanged("ChkMoreSelected"); }
        }
        public DelegateCommand<IXmlSettingCollection> AddNewTypeCommand { get; set; }
        public DelegateCommand DeleteTypeCommand { get; set; }
        public DelegateCommand KeyFlagTrueCommand { get; set; }
        public DelegateCommand KeyFlagFalseCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand chkMoreCommand { get; set; }
        public DelegateCommand BackCommand { get; set; }
        public DelegateCommand AddNodeCompareFlagCommand { get; set; }
        public DelegateCommand RemoveNodeCompareFlagCommand { get; set; }
        public DelegateCommand AddAttrCompareFlagCommand { get; set; }
        public DelegateCommand RemoveAttrCompareFlagCommand { get; set; }
        public void AddNodeCompareFlag()
        {
            if (NodeSelected != null)
            {
                NodeSelected.Compare_flag = "1";
                AlertNodeSetting();
                BindgingSettingNodeToNodeTlv(m_CurrentSettingCollection.GetSetting(SelectType),
                                            NodeSelected.name);
            }
        }
        public void RemoveNodeCompareFlag()
        {
            if (NodeSelected != null)
            {
                NodeSelected.Compare_flag = "0";
                AlertNodeSetting();
                BindgingSettingNodeToNodeTlv(m_CurrentSettingCollection.GetSetting(SelectType),
                                            NodeSelected.name);
            }
        }
        public void AddAttrCompareFlag()
        {
            if (AttrSelected != null)
            {
                AttrSelected.Compare_flag = "1";
                AttrSelected.compareImage = "C:\\Users\\LTC00471\\Desktop\\XMLMain\\XmlCompare\\Images\\compare_flag_true.png";
                AlertNodeAttributeSetting();
                BindgingSettingNodeToNodeTlv(m_CurrentSettingCollection.GetSetting(SelectType),
                                            CurrentNode.name);
                NodeSelected = CurrentNode;
            }
        }
        public void RemoveAttrCompareFlag()
        {
            if (AttrSelected != null)
            {
                AttrSelected.Compare_flag = "0";
                AttrSelected.compareImage = "C:\\Users\\LTC00471\\Desktop\\XMLMain\\XmlCompare\\Images\\compare_flag_false.png";
                AlertNodeAttributeSetting();
                BindgingSettingNodeToNodeTlv(m_CurrentSettingCollection.GetSetting(SelectType),
                                            CurrentNode.name);
                NodeSelected = CurrentNode;
            }
        }
        private void AlertNodeSetting()
        {
            List<string> lstComparedNodes = new List<string>();
            foreach (var tlvItem in NodesList)
            {
                if (tlvItem.Compare_flag == "1")
                    lstComparedNodes.Add(tlvItem.name);
            }
            foreach (IXmlSettingNode tmp in m_CurrentSettingCollection
                .GetSetting(SelectType).GetNodes)
            {
                tmp.compare_flag = lstComparedNodes.Exists(x => x == tmp.name);
            }
        }
        public void Back()
        {
            _regionManager.RequestNavigate("ContentRegion", "CoreView");
            _regionManager.RequestNavigate("DictionRegion", "EmptyView");
            ChkMoreSelected = false;
            ChkContent = "Show Report Dictionary Setting";
        }
       
        public void AddNewTypeCommandExecute(IXmlSettingCollection collection)
        {
            //Navigation to NewXmlTypeView
            _regionManager.RequestNavigate("ContentRegion", "NewXmlTypeView");
        }
        public void DeleteTypeCommandExecute()
        {
            DialogResult flag = MessageBox.Show("Do you want to delete this xml setting from Setting File?",
                "Setting File Change Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation);
            if (flag == DialogResult.Yes)
            {
                IXmlSetting currentXmlSetting = m_CurrentSettingCollection.GetSetting(SelectType);
                m_CurrentSettingCollection.RemoveSetting(currentXmlSetting);
                _ea.GetEvent<CollectionEvent>().Publish(m_CurrentSettingCollection);
                XmlTypeList = null;
                XmlTypeList = new ObservableCollection<string>(m_CurrentSettingCollection.GetAllSettingTypes());
                SelectType = null;
                AttributeList.Clear();
            }
        }
        public void KeyFlagTrueCommandExecute()
        {
            if (AttrSelected != null)
            {
                AttrSelected.Idtflag = "1";
                AttrSelected.IdtflagImage = System.IO.Directory.GetCurrentDirectory() + "\\Images\\key_flag_true.png";
                AlertNodeAttributeSetting();
                BindgingSettingNodeToNodeTlv(m_CurrentSettingCollection.GetSetting(SelectType),
                                            CurrentNode.name);
                NodeSelected = CurrentNode;
            }
        }
        public void KeyFlagFalseCommandExecute()
        {
            if (AttrSelected != null)
            {
                AttrSelected.Idtflag = "0";
                AttrSelected.IdtflagImage = System.IO.Directory.GetCurrentDirectory() + "\\Images\\key_flag_false.png";
                AlertNodeAttributeSetting();
                BindgingSettingNodeToNodeTlv(m_CurrentSettingCollection.GetSetting(SelectType),
                                            CurrentNode.name);
                NodeSelected = CurrentNode;
            }
        }
        private void AlertNodeAttributeSetting()
        {
            List<string> lstKeyAttributes = new List<string>();
            List<string> lstComparedAttributes = new List<string>();
            foreach (var tlvItem in AttributeList)
            {
                if (tlvItem.Idtflag == "1")
                {
                    lstKeyAttributes.Add(tlvItem.name);
                }
                if (tlvItem.Compare_flag == "1")
                {
                    lstComparedAttributes.Add(tlvItem.name);
                }
            }
            foreach (IXmlSettingAttribute tmpAttr
                in m_CurrentSettingCollection
                .GetSetting(SelectType)
                .GetNode(CurrentNode.name)
                .GetAttributes)
            {
                tmpAttr.identifier_flag = lstKeyAttributes.Exists(x => x == tmpAttr.name);
                tmpAttr.compare_flag = lstComparedAttributes.Exists(x => x == tmpAttr.name);
            }
        }
        private void BindgingSettingNodeToNodeTlv(IXmlSetting currentXmlSetting, string strSelectedItemText)
        {
            if (SelectType != "")
            {
                NodesList.Clear();
                foreach (IXmlSettingNode tempNode in currentXmlSetting.GetNodes)
                {
                    tempNode.IdtAttributes = tempNode.identifier();
                    if (tempNode.compare_flag)
                    {
                        tempNode.Compare_flag = "1";
                        tempNode.CompareImage = System.IO.Directory.GetCurrentDirectory() + "\\Images\\compare_flag_true.png";
                    }
                    else
                    {
                        tempNode.Compare_flag = "0";
                        tempNode.CompareImage = System.IO.Directory.GetCurrentDirectory() + "\\Images\\compare_flag_false.png";
                    }
                    tempNode.NameImage = System.IO.Directory.GetCurrentDirectory() + "\\Images\\node.png";   
                    NodesList.Add(tempNode);
                    if (tempNode.name == strSelectedItemText)
                    {
                       // neoItem.Selected = true;
                    }
                }
            }
        }
        public void SaveCommandExecute()
        {
            if (SelectType != "")
            {
                string strSelectedXmlType = SelectType;
                m_SettingFact.WriteSettingCollection(m_CurrentSettingCollection,
                    m_ToolSetting.GetXmlSettingFilePath().Value);
                MessageBox.Show("Save successed!");
            }
        }
        private void chkMoreCommandExecute()
        {
            if (ChkMoreSelected == true)
            {
                ChkContent = "Hide Report Dictionary Setting";
                _regionManager.RequestNavigate("DictionRegion", "pnlDictionaryEdtPanelView");
            }
            else
            {
                ChkContent = "Show Report Dictionary Setting";
                _regionManager.RequestNavigate("DictionRegion", "EmptyView");
            }
        }
        private void GetCollection(IXmlSettingCollection Collection)
        {
            m_CurrentSettingCollection = Collection;
            XmlTypeList = new ObservableCollection<string>(m_CurrentSettingCollection.GetAllSettingTypes());
            SelectType = XmlTypeList.First();
        }
        [ImportingConstructor]
        public ToolSettingViewModel(IEventAggregator ea, IXmlCompareToolSetting ToolSetting, IXmlSettingCollection CurrentSettingCollection, pnlDictionaryEdtPanelViewModel DictionaryEdtPanel, IXmlCompareUserControl UserControl, IXmlSettingReportDictionary CurrentReportDictionary, IXmlSettingFactorty SettingFact, IRegionManager regionManager, IXmlSettingNode _CurrentNode, IXmlSetting _CurrentSetting)
        {
            AddNewTypeCommand = new DelegateCommand<IXmlSettingCollection>(AddNewTypeCommandExecute);
            DeleteTypeCommand = new DelegateCommand(DeleteTypeCommandExecute);
            KeyFlagTrueCommand = new DelegateCommand(KeyFlagTrueCommandExecute);
            KeyFlagFalseCommand = new DelegateCommand(KeyFlagFalseCommandExecute);
            SaveCommand = new DelegateCommand(SaveCommandExecute);
            chkMoreCommand = new DelegateCommand(chkMoreCommandExecute);
            BackCommand = new DelegateCommand(Back);
            AddAttrCompareFlagCommand = new DelegateCommand(AddAttrCompareFlag);
            RemoveAttrCompareFlagCommand = new DelegateCommand(RemoveAttrCompareFlag);
            AddNodeCompareFlagCommand = new DelegateCommand(AddNodeCompareFlag);
            RemoveNodeCompareFlagCommand = new DelegateCommand(RemoveNodeCompareFlag);
            CurrentNode = _CurrentNode;
            _CurrentReportDictionary = CurrentReportDictionary;
            CurrentSetting = _CurrentSetting;
            _regionManager = regionManager;
            m_ToolSetting = ToolSetting;
            m_UserControl = UserControl;
            m_SettingFact = SettingFact;
            m_DictionaryEdtPanel = DictionaryEdtPanel;
            m_DictionaryEdtPanel = new pnlDictionaryEdtPanelViewModel(ea,m_ToolSetting,m_UserControl,_CurrentReportDictionary,CurrentNode,CurrentSetting);
            ChkMoreSelected = false;
            m_CurrentSettingCollection = CurrentSettingCollection;
            m_CurrentSettingCollection =
                m_SettingFact.ReadSettingCollection(m_ToolSetting.GetXmlSettingFilePath().Value);
            _ea = ea;
            _ea.GetEvent<CollectionEvent>().Subscribe(GetCollection);
            Flag = m_UserControl.IsUserHasPriviledge(m_UserControl.GetCurrentUser(), "XmlSettingFilePath");
            XmlTypeList = new ObservableCollection<string>(m_CurrentSettingCollection.GetAllSettingTypes());
            SelectType = null;
            ChkContent = "Show Report Dictionary Setting";
        }

    }
}
