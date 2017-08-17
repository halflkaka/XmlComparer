using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlCompare.Infrastructure.Interface;

namespace XmlCompare.Core.Services
{
    [Export(typeof(IXmlSettingNode))]
    [Serializable]
    public class CXmlSettingNodeImp : IXmlSettingNode
    {
        //member
        private List<CXmlSettingAttrImp> m_Attributes = new List<CXmlSettingAttrImp>();
        private List<CXmlSettingRptDicImp> m_Dictionarys = new List<CXmlSettingRptDicImp>();
        //property
        public string name { get; set; }
        public bool compare_flag { get; set; }
        public string CompareImage { get; set; }
        public string Compare_flag { get; set; }
        public string NameImage { get; set; }
        public string IdtAttributes { get; set; }
        //public CXmlSettingNodeImp
        public List<IXmlSettingAttribute> GetAttributes
        {
            get
            {
                return m_Attributes.ToList<IXmlSettingAttribute>();
            }
        }
        public List<IXmlSettingReportDictionary> GetDictionarys
        {
            get
            {
                return m_Dictionarys.ToList<IXmlSettingReportDictionary>();
            }
        }
        //method
        public string identifier()
        {
            return String.Join(",", (from IXmlSettingAttribute tmp in m_Attributes
                                     where tmp.identifier_flag == true
                                     select tmp.name).ToArray());
        }
        public string attributesToCompare()
        {
            return String.Join(",", (from IXmlSettingAttribute tmp in m_Attributes
                                     where tmp.compare_flag == true
                                     select tmp.name).ToArray());
        }

        public IXmlSettingAttribute GetAttribute(string name)
        {
            return m_Attributes.FirstOrDefault(m => m.name == name);
        }

        public void AppendAttribute(IXmlSettingAttribute attribute)
        {
            CXmlSettingAttrImp value = (CXmlSettingAttrImp)attribute;
            if (m_Attributes.Exists(m => m.name == value.name))
            {
                throw new Exception("duplicate attribute name[" + value.name + "] exist in node, append failed");
            }
            else
            {
                m_Attributes.Add(value);
            }
        }

        public void RemoveAttribute(IXmlSettingAttribute attribute)
        {
            CXmlSettingAttrImp value = (CXmlSettingAttrImp)attribute;
            m_Attributes.Remove(value);
        }

        public IXmlSettingReportDictionary GetDictionary(XmlSettingRptDicCategory category)
        {
            return m_Dictionarys.FirstOrDefault(m => m.issue_category == category.ToString());
        }

        public void AppendDictionary(IXmlSettingReportDictionary dictionary)
        {
            CXmlSettingRptDicImp value = (CXmlSettingRptDicImp)dictionary;
            if (m_Dictionarys.Exists(m => m.issue_category == value.issue_category))
            {
                throw new Exception("duplicate category[" + value.issue_category + "] exist in dictionarys, append failed.");
            }
            else
            {
                m_Dictionarys.Add(value);
            }
        }

        public void RemoveDictionary(IXmlSettingReportDictionary dictionary)
        {
            CXmlSettingRptDicImp value = (CXmlSettingRptDicImp)dictionary;
            m_Dictionarys.Remove(value);
        }
    }
}
