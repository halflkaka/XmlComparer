using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlCompare.Infrastructure.Interface;
using XmlCompare.Infrastructure.Entity;
using System.ComponentModel.Composition;

namespace XmlCompare.Core.Services
{
    [Export(typeof(IXmlFileNode))]
    public class CXmlFileNodeImp : IXmlFileNode
    {
        public Dictionary<string, string> m_attributes { get; set; }
        public Dictionary<string, IXmlFileNode> m_childNodes { get; set; }
        public Dictionary<string,IXmlFileNode> m_parentNodes { get; set; }
        public string text { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string xPath { get; set; }

        public CXmlFileNodeImp(string strName, string strXPath)
        {
            this.key = "";
            this.name = strName;
            this.m_childNodes = new Dictionary<string, IXmlFileNode>();
            this.m_parentNodes = new Dictionary<string, IXmlFileNode>();
            this.m_attributes = new Dictionary<string, string>();
            this.text = "";
            this.xPath = strXPath;
        }

        public static bool operator ==(CXmlFileNodeImp node1, CXmlFileNodeImp node2)
        {
            bool mattributesFlag, textFlag, childNodesFlag;

            #region compair the text of the node
            /**********************************************************************************
      * compair the text of the node
      **********************************************************************************/
            textFlag = (node1.text == node2.text) ? true : false;
            if (!textFlag)
            {
                CLog loger = CLog.CreateInstance();
                loger.Append(XmlSettingRptDicCategory.ChangedNodeText.ToString(),
                    node2.name,
                    node2.key,
                    node2.xPath,
                    "Inner Xml Text",
                    node1.text,
                    node2.text);
            }
            #endregion

            #region compair the attributes of the nodes
            /**********************************************************************************
      * compair the attributes of the node
      **********************************************************************************/
            mattributesFlag = true;
            // Union the two Node m_mattributes by key, 
            //    if the value is different for the key, keep the first Node attribute
            IEnumerable<KeyValuePair<string, string>> attrUnion = node1.m_attributes.
                Union<KeyValuePair<string, string>>(node2.m_attributes,
                   ProjectionEqualityComparer<KeyValuePair<string, string>>.Create(a => a.Key));
            foreach (KeyValuePair<string, string> tempKeyValPair in attrUnion)
            {
                // find the new added attributes
                if (!node1.m_attributes.Keys.Any<string>((string temKey) => temKey == tempKeyValPair.Key))
                {
                    CLog loger = CLog.CreateInstance();
                    loger.Append(XmlSettingRptDicCategory.AddedAttribute.ToString(),
                        node2.name,
                        node2.key,
                        node2.xPath,
                        tempKeyValPair.Key,
                        "[New Added]",
                        tempKeyValPair.Value);
                    mattributesFlag = false;
                }

                // find the deleted attribute and changed attributes
                if (!node2.m_attributes.Keys.Any<string>((string temKey) => temKey == tempKeyValPair.Key))
                {
                    //deleted attribute
                    CLog loger = CLog.CreateInstance();
                    loger.Append(XmlSettingRptDicCategory.DeletedAttribute.ToString(),
                        node2.name,
                        node2.key,
                        node2.xPath,
                        tempKeyValPair.Key,
                        tempKeyValPair.Value,
                        "[Deleted]");
                    mattributesFlag = false;
                }
                else if (node2.m_attributes[tempKeyValPair.Key] != tempKeyValPair.Value)
                {
                    //changed attribute
                    CLog loger = CLog.CreateInstance();
                    loger.Append(XmlSettingRptDicCategory.ChangedAttribute.ToString(),
                        node2.name,
                        node2.key,
                        node2.xPath,
                        tempKeyValPair.Key,
                        node1.m_attributes[tempKeyValPair.Key],
                        node2.m_attributes[tempKeyValPair.Key]);
                    mattributesFlag = false;

                }
            }
            #endregion

            #region compair the child nodes of the nodes
            childNodesFlag = true;

            IEnumerable<KeyValuePair<string, IXmlFileNode>> childUnion = node1.m_childNodes.
                Union<KeyValuePair<string, IXmlFileNode>>(node2.m_childNodes,
                   ProjectionEqualityComparer<KeyValuePair<string, IXmlFileNode>>.Create(a => a.Key));
            foreach (KeyValuePair<string, IXmlFileNode> tempKeyValPair in childUnion)
            {
                // find the new added child nodes
                if (!node1.m_childNodes.Keys.Any<string>((string temKey) => temKey == tempKeyValPair.Key))
                {
                    // new nodes
                    CLog loger = CLog.CreateInstance();
                    loger.Append(XmlSettingRptDicCategory.AddedChildNode.ToString(),
                        node2.name,
                        node2.key,
                        node2.xPath,
                        "[Child Notes]",
                        "[New Added]",
                        tempKeyValPair.Key);
                    childNodesFlag = false;
                }
                // find the deleted child nodes and changed child nodes
                if (!node2.m_childNodes.Keys.Any<string>((string temKey) => temKey == tempKeyValPair.Key))
                {
                    // deleted nodes
                    CLog loger = CLog.CreateInstance();
                    loger.Append(XmlSettingRptDicCategory.DeletedChildNode.ToString(),
                        node2.name,
                        node2.key,
                        node2.xPath,
                        "[Child Notes]",
                        tempKeyValPair.Key,
                        "[Deleted]");
                    childNodesFlag = false;
                }
                else
                {
                    CXmlFileNodeImp tmpNode1 = (CXmlFileNodeImp)tempKeyValPair.Value;
                    CXmlFileNodeImp tmpNode2 = (CXmlFileNodeImp)node2.m_childNodes[tempKeyValPair.Key];
                    if (!(tmpNode1 == tmpNode2))
                    {
                        childNodesFlag = false;
                    }
                }
            }
            #endregion

            if (mattributesFlag && textFlag && childNodesFlag)
                return true;
            else
                return false;
        }

        public static bool operator !=(CXmlFileNodeImp node1, CXmlFileNodeImp node2)
        {
            return (node1 == node2) ? false : true;
        }

    }
}
