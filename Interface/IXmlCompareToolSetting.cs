using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlCompare.Infrastructure.Entity;

namespace XmlCompare.Infrastructure.Interface
{
    public interface IXmlCompareToolSetting
    {
        List<CSetting> GetSettingList();
        CSetting GetXmlSettingFilePath();
        void SaveXmlSettingFilePath(string filePath);
        void SaveXmlSettingFilePath(CSetting setting);
    }
}
