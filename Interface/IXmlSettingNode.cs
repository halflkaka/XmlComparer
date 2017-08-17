using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCompare.Infrastructure.Interface
{
    public interface IXmlSettingNode
    {
        //properties
        string name { get; set; }
        bool compare_flag { get; set; }
        string CompareImage { get; set; }
        string Compare_flag { get; set; }
        string NameImage { get; set; }
        string IdtAttributes { get; set; }
        List<IXmlSettingAttribute> GetAttributes { get; }
        List<IXmlSettingReportDictionary> GetDictionarys { get; }
        //method
        string identifier();
        string attributesToCompare();
        IXmlSettingAttribute GetAttribute(string name);
        void AppendAttribute(IXmlSettingAttribute attribute);
        void RemoveAttribute(IXmlSettingAttribute attribute);
        IXmlSettingReportDictionary GetDictionary(XmlSettingRptDicCategory category);
        void AppendDictionary(IXmlSettingReportDictionary dictionary);
        void RemoveDictionary(IXmlSettingReportDictionary dictionary);
    }
}
