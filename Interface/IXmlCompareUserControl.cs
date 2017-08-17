using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlCompare.Infrastructure.Entity;

namespace XmlCompare.Infrastructure.Interface
{
    public interface IXmlCompareUserControl
    {
        CUser GetUser(string id);
        List<CUser> GetUsers();
        CUser CreateUser(string id, string name);
        CUser GetCurrentUser();
        bool CheckUser(string id);

        List<CFunction> GetFunctions();

        List<CUser> GetFunctionUsers(string functionName, bool flag);
        void SetFunctionUsers(string funcitonName, List<CUser> userList);
        bool IsUserHasPriviledge(CUser user, string functionName);
    }
}
