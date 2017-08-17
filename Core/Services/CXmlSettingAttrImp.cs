using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlCompare.Infrastructure.Interface;

namespace XmlCompare.Core.Services
{
    [Export(typeof(IXmlSettingAttribute))]
    [Serializable]
    public class CXmlSettingAttrImp : IXmlSettingAttribute
    {
        public string name { get; set; }
        public bool identifier_flag { get; set; }
        public bool compare_flag { get; set; }
        public string Idtflag { get; set; }
        public string IdtflagImage { get; set; }
        public string Compare_flag { get; set; }
        public string compareImage { get; set; }
        public string AttributeImage { get; set; }
    }
}
