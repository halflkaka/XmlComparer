using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCompare.Infrastructure.Interface
{
    public interface IXmlSetting
    {
        //properties
        string file_type { get; set; }
        string update_time { get; set; }
        string updated_by { get; set; }
        List<IXmlSettingNode> GetNodes { get; }
        List<string> GeneralNotes { get; set; }

        //methord
        IXmlSettingNode GetNode(string strNodeName);
        void AppendNode(IXmlSettingNode node);
        void RemoveNode(IXmlSettingNode node);
    }
}
