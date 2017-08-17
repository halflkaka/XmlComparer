using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlCompare.Infrastructure.Entity;

namespace XmlCompare.Infrastructure.Interface
{
    public interface IXmlCompareCore
    {
        void Compare(string iOriginalPath,
            string iNewPath,
            string iFileType,
            string iOutputPath,
            bool iShowAlert = false);
        List<CFilePare> GetFilePare(string OldFileFolderDir, string NewFileFolderDir);
        bool Initialize(out IXmlSettingCollection xmlSettingCollection);
        void BatchCompare(List<CFilePare> iFilePares, string iFileType, string iOutputPath);

    }
}
