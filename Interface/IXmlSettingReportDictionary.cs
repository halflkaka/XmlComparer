using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCompare.Infrastructure.Interface
{
    public interface IXmlSettingReportDictionary
    {
        string id { get; set; }
        string node_name { get; set; }
        string issue_source { get; set; }
        string issue_category { get; set; }
        string issue_instruction { get; set; }
        string ImageSource { get; set; }
    }
    public enum XmlSettingRptDicCategory
    {
        AddedAttribute = 1,
        DeletedAttribute = 2,
        ChangedAttribute = 3,

        AddedChildNode = 4,
        DeletedChildNode = 5,

        ChangedNodeText = 6
    };
}
