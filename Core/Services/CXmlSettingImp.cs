using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using XmlCompare.Infrastructure.Interface;

namespace XmlCompare.Core.Services
{
    [Export(typeof(IXmlSetting))]
    [Serializable]
    public class CXmlSettingImp : IXmlSetting
    {
        //member 
        private List<CXmlSettingNodeImp> m_Nodes = new List<CXmlSettingNodeImp>();
        private List<string> m_GeneralNotes = new List<string>();
        //public Dictionary<string, List<string>> Children = new Dictionary<string, List<string>>();
        //public Dictionary<string, List<string>> Parent = new Dictionary<string, List<string>>();
        public Dictionary<IXmlSettingNode, List<IXmlSettingNode>> Children = new Dictionary<IXmlSettingNode, List<IXmlSettingNode>>();
        public Dictionary<IXmlSettingNode, List<IXmlSettingNode>> Parent = new Dictionary<IXmlSettingNode, List<IXmlSettingNode>>();

        //properties
        public string file_type { get; set; }
        public string update_time { get; set; }
        public string updated_by { get; set; }
        public List<IXmlSettingNode> GetNodes
        {
            get
            {
                return m_Nodes.ToList<IXmlSettingNode>();
            }
        }
        public List<string> GeneralNotes
        {
            get
            {
                return m_GeneralNotes;
            }
            set
            {
                m_GeneralNotes = value;
            }
        }
        //method
        public IXmlSettingNode GetNode(string strNodeName)
        {
            /*return (from IXmlSettingNode tmp in m_Nodes
                    where tmp.name == strNodeName
                    select tmp).FirstOrDefault();*/
            IXmlSettingNode x = (from IXmlSettingNode tmp in m_Nodes
                                 where tmp.name == strNodeName
                                 select tmp).First();
            return x;
        }

        public void AppendNode(IXmlSettingNode node)
        {
            CXmlSettingNodeImp value = (CXmlSettingNodeImp)node;
            if (m_Nodes.Exists(m => m.name == value.name))
            {
                throw new Exception("duplicate node name[" + value.name + "] exist in setting, append failed");
            }
            else
            {
                m_Nodes.Add(value);
            }
        }
        public void RemoveNode(IXmlSettingNode node)
        {
            CXmlSettingNodeImp value = (CXmlSettingNodeImp)node;
            m_Nodes.Remove(value);
        }
        public Dictionary<IXmlSettingNode, List<IXmlSettingNode>> GetChildRelation(string xmlFilePath)
        {
            XmlDocument NodeSettingDoc = new XmlDocument();
            NodeSettingDoc.Load(xmlFilePath);
            XmlNode root = NodeSettingDoc.DocumentElement;
            XmlNodeList listNode = root.SelectNodes("//*");
            XElement xele = XElement.Load(xmlFilePath);
            List<string> lstDisctinctNodeNames = (from XmlNode tempNode in listNode select tempNode.Name).Distinct().ToList();
            foreach (var item in lstDisctinctNodeNames)
            {
                if (item == root.Name)
                {
                    List<IXmlSettingNode> relation = new List<IXmlSettingNode>();
                    foreach (var sub in xele.Descendants())
                    {
                        if (!relation.Exists(r => r.name == sub.Name))// == sub.Name.ToString()))
                        {
                            if (GetNode(sub.Name.ToString()) != null)
                            {
                                IXmlSettingNode x = GetNode(sub.Name.ToString());
                                //relation.Add(sub.Name.ToString());
                                relation.Add(x);
                            }
                        }
                    }
                    if(GetNode(item) != null) { IXmlSettingNode y = GetNode(item);Children.Add(y, relation);}
                    
                }
                else
                {
                    var nodes = xele.Descendants(item).Select(r => r);
                    if (nodes != null)
                    {
                        List<IXmlSettingNode> relation = new List<IXmlSettingNode>();
                        foreach (var par in nodes.Descendants())
                        {
                            if (!relation.Exists(r => r.name == par.Name.ToString()))
                            {
                                if (GetNode(par.Name.ToString()) != null)
                                {
                                    IXmlSettingNode x = GetNode(par.Name.ToString());
                                    relation.Add(x);
                                }
                            }
                        }
                        if(GetNode(item) != null)
                        {
                            IXmlSettingNode y = GetNode(item);
                            Children.Add(y, relation);
                        }
                    }
                }
            }
            return Children;
        }
        public Dictionary<IXmlSettingNode,List<IXmlSettingNode>> GetParentRelation(string xmlFilePath)
        {
            XmlDocument NodeSettingDoc = new XmlDocument();
            NodeSettingDoc.Load(xmlFilePath);
            XmlNode root = NodeSettingDoc.DocumentElement;
            XmlNodeList listNode = root.SelectNodes("//*");
            XElement xele = XElement.Load(xmlFilePath);
            List<string> lstDisctinctNodeNames = (from XmlNode tempNode in listNode select tempNode.Name).Distinct().ToList();
            foreach (var item in lstDisctinctNodeNames)
            {
                if (item == root.Name)
                {
                    if(GetNode(item) != null)
                    {
                        IXmlSettingNode x = GetNode(item);
                        Parent.Add(x, null);
                    }
                }
                else
                {
                    var nodes = xele.Descendants(item).Select(r => r);
                    if (nodes != null)
                    {
                        List<IXmlSettingNode> relation = new List<IXmlSettingNode>();
                        foreach (var par in nodes.Ancestors())
                        {
                            if (!relation.Exists(r => r.name == par.Name.ToString()))
                            {
                                if (GetNode(par.Name.ToString()) != null)
                                {
                                    IXmlSettingNode x = GetNode(par.Name.ToString());
                                    relation.Add(x);
                                }
                            }
                        }
                        if (GetNode(item) != null)
                        {
                            IXmlSettingNode y = GetNode(item);
                            Parent.Add(y, relation);
                        }
                    }
                }
            }
            return Parent;
        }
    }
}
