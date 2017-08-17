using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlCompare.Infrastructure.Entity;
using XmlCompare.Infrastructure.Interface;

namespace XmlCompare.ToolSetting.Services
{
    [Export(typeof(IXmlCompareToolSetting))]
    public class CXmlCmpToolSettingImp : IXmlCompareToolSetting
    {
        private IXmlCmpDbAdp m_Dba;

        [ImportingConstructor]
        public CXmlCmpToolSettingImp(IXmlCmpDbAdp dba)
        {
            m_Dba = dba;
        }

        public List<CSetting> GetSettingList()
        {
            return m_Dba.GetSettingList();
        }
        public CSetting GetXmlSettingFilePath()
        {
            return new CSetting()
            {
                Name = "XmlSettingFilePath",
                Value = System.IO.Directory.GetCurrentDirectory() + "\\New folder\\Xml_Compare_Setting.xml" 
            };
            //return m_Dba.GetSetting("XmlSettingFilePath");
        }
        public void SaveXmlSettingFilePath(string filePath)
        {
            m_Dba.SaveSetting(new CSetting() { Name = "XmlSettingFilePath", Value = filePath });
        }
        public void SaveXmlSettingFilePath(CSetting setting)
        {
            m_Dba.SaveSetting(setting);
        }
    }
}
