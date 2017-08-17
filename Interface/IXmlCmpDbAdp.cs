using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlCompare.Infrastructure.Entity;

namespace XmlCompare.Infrastructure.Interface
{
    public interface IXmlCmpDbAdp
    {
        bool SaveUser(CUser user);
        CUser GetUserById(string id);
        List<CUser> GetUserList();
        List<CUser> GetUsetListForFunction(string functionName, bool flag);
        bool IsUserExist(string userId);

        List<CFunction> GetFunctionList();
        CFunction GetFunctionById(int id);
        CFunction GetFunctionByName(string name);

        bool SavePrivilege(CPrivilege privilege);
        bool SaveAllPrivelege(List<CPrivilege> privilegeList);
        bool DeletePrivilege(CPrivilege privilege);
        bool DeleteAllPrivelegeForFunciton(string functionName);
        CPrivilege GetPrivilege(string functionName, string userId);
        bool IsPrivilegeExist(string functionName, string userId);

        CSetting GetSetting(string setingName);
        List<CSetting> GetSettingList();
        bool SaveSetting(CSetting setting);

    }
}
