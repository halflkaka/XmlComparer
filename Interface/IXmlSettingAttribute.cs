using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCompare.Infrastructure.Interface
{
    public interface IXmlSettingAttribute
    {
        string name { get; set; }
        bool identifier_flag { get; set; }
        bool compare_flag { get; set; }
        string Idtflag { get; set; }
        string IdtflagImage { get; set; }
        string Compare_flag { get; set; }
        string compareImage { get; set; }
        string AttributeImage { get; set; }
    }
}
