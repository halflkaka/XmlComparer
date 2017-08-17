using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XmlCompare.Infrastructure.Interface;

namespace XmlCompare.Core.Services
{
    [Export(typeof(IXmlSettingCollection))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Serializable]
    public class CXmlSettingCollectionImp : IXmlSettingCollection
    {
        //member
        private List<CXmlSettingImp> m_NodeSettingCollection = new List<CXmlSettingImp>();
        //properties
        public List<IXmlSetting> GetSettings
        {
            get
            {
                return m_NodeSettingCollection.ToList<IXmlSetting>();
            }
        }
        //method
        public List<string> GetAllSettingTypes()
        {
            return (from IXmlSetting tmp in m_NodeSettingCollection
                    select tmp.file_type).ToList();
        }
        public IXmlSetting GetSetting(string settingType)
        {
            return (from IXmlSetting tmp in m_NodeSettingCollection
                    where tmp.file_type == settingType
                    select tmp).FirstOrDefault();
        }
        public void AppendSetting(IXmlSetting setting)
        {
            CXmlSettingImp value = (CXmlSettingImp)setting;
            if (m_NodeSettingCollection.Exists(m => m.file_type == value.file_type))
            {
                //throw new Exception("diplicate file type[" + value.file_type + "] exist in setting collcetion.append failed");
                MessageBox.Show("Type already exists!");
            }
            else
            {
                m_NodeSettingCollection.Add(value);
            }
        }
        public void RemoveSetting(IXmlSetting setting)
        {
            CXmlSettingImp value = (CXmlSettingImp)setting;
            m_NodeSettingCollection.Remove(value);
        }
    }
}
