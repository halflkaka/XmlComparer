using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCompare.Infrastructure.Interface
{
    public interface IXmlSettingFactorty
    {
        IXmlSettingCollection ReadSettingCollection(string filePath);
        void WriteSettingCollection(IXmlSettingCollection instance, string filePath);

        IXmlSetting InitFromXmlFile(string xmlType, string xmlFilePath, string userId);
    }
}
