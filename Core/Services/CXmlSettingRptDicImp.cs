using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlCompare.Infrastructure.Interface;

namespace XmlCompare.Core.Services
{
    [Export(typeof(IXmlSettingReportDictionary))]
    [Serializable]
    public class CXmlSettingRptDicImp : IXmlSettingReportDictionary
    {
        public string id { get; set; }
        public string node_name { get; set; }
        public string issue_source { get; set; }
        public string issue_category { get; set; }
        public string issue_instruction { get; set; }
        public string ImageSource { get; set; }
    }
}
