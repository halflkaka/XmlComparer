using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCompare.Infrastructure.Interface
{
    public interface IXmlSettingCollection
    {
        //properties
        List<IXmlSetting> GetSettings { get; }
        //methord
        List<string> GetAllSettingTypes();
        IXmlSetting GetSetting(string settingType);
        void AppendSetting(IXmlSetting setting);
        void RemoveSetting(IXmlSetting setting);
    }
}
