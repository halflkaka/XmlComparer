using ServiceStack.OrmLite;
using ServiceStack.OrmLite.MySql;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlCompare.Infrastructure.Entity;
using XmlCompare.Infrastructure.Interface;

namespace XmlCompare.DAL.Services
{
    [Export(typeof(IXmlCmpDbAdp))]
    public class CXmlCmpDbAdpImp : IXmlCmpDbAdp
    {
        private static string Provider = "MySql.Data.MySqlClient";
        private static string Server = "em.saifei.corp";
        private static string Database = "engineering_tools";
        private static string User = "ProjectManager";
        private static string Password = "Dead99beef";
        private string GetConnectionString()
        {
            return "Data Source=" + Server
                + ";User ID=" + User
                + ";Password =" + Password
                + "; Database = " + Database;
        }
        public bool SaveUser(CUser user)
        {
            var dbFactory = new OrmLiteConnectionFactory(
                GetConnectionString(),
                MySqlDialectProvider.Instance);
            using (var db = dbFactory.Open())
            {
                return db.Save<CUser>(user);
            }
        }
        public CUser GetUserById(string id)
        {
            var dbFactory = new OrmLiteConnectionFactory(
               GetConnectionString(),
               MySqlDialectProvider.Instance);
            using (var db = dbFactory.Open())
            {
                return db.SingleById<CUser>(id);
            }
        }
        public List<CUser> GetUserList()
        {
            var dbFactory = new OrmLiteConnectionFactory(
                GetConnectionString(),
                MySqlDialectProvider.Instance);
            using (var db = dbFactory.Open())
            {
                return db.Select<CUser>();
            }
        }
        public List<CUser> GetUsetListForFunction(string functionName, bool flag)
        {
            var dbFactory = new OrmLiteConnectionFactory(
                GetConnectionString(),
                MySqlDialectProvider.Instance);
            using (var db = dbFactory.Open())
            {
                var sqlExp = db.From<CUser>()
                    .LeftJoin<CUser, CPrivilege>((user, privilege) => user.Id == privilege.CUserId)
                    .LeftJoin<CPrivilege, CFunction>((privilege, func) => privilege.CFunctionId == func.Id
                        && func.Name == functionName);
                if (flag)
                {
                    return db.Select<CUser>(sqlExp.Where<CFunction>(func => func == null));
                }
                else
                {
                    return db.Select<CUser>(sqlExp.Where<CFunction>(func => func != null));
                }
            }
        }
        public bool IsUserExist(string userId)
        {
            var dbFactory = new OrmLiteConnectionFactory(
               GetConnectionString(),
               MySqlDialectProvider.Instance);
            using (var db = dbFactory.Open())
            {
                return db.Exists<CUser>(new { Id = userId });
            }
        }

        public List<CFunction> GetFunctionList()
        {
            var dbFactory = new OrmLiteConnectionFactory(
               GetConnectionString(),
               MySqlDialectProvider.Instance);
            using (var db = dbFactory.Open())
            {
                return db.Select<CFunction>();
            }
        }
        public CFunction GetFunctionById(int id)
        {
            var dbFactory = new OrmLiteConnectionFactory(
               GetConnectionString(),
               MySqlDialectProvider.Instance);
            using (var db = dbFactory.Open())
            {
                return db.SingleById<CFunction>(id);
            }
        }
        public CFunction GetFunctionByName(string name)
        {
            var dbFactory = new OrmLiteConnectionFactory(
               GetConnectionString(),
               MySqlDialectProvider.Instance);
            using (var db = dbFactory.Open())
            {
                return db.Single<CFunction>(new { Name = name });
            }
        }

        public bool SavePrivilege(CPrivilege privilege)
        {
            var dbFactory = new OrmLiteConnectionFactory(
                GetConnectionString(),
                MySqlDialectProvider.Instance);
            using (var db = dbFactory.Open())
            {
                return db.Save<CPrivilege>(privilege);
            }
        }
        public bool SaveAllPrivelege(List<CPrivilege> privilegeList)
        {
            var dbFactory = new OrmLiteConnectionFactory(
               GetConnectionString(),
               MySqlDialectProvider.Instance);
            using (var db = dbFactory.Open())
            {
                int flag = db.SaveAll<CPrivilege>(privilegeList);
                if (flag > 0) return true;
                else return false;
            }
        }
        public bool DeletePrivilege(CPrivilege privilege)
        {
            var dbFactory = new OrmLiteConnectionFactory(
                GetConnectionString(),
                MySqlDialectProvider.Instance);
            using (var db = dbFactory.Open())
            {
                int flag = db.Delete<CPrivilege>(privilege);
                if (flag > 0) return true;
                else return false;
            }
        }
        public bool DeleteAllPrivelegeForFunciton(string functionName)
        {
            var dbFactory = new OrmLiteConnectionFactory(
                GetConnectionString(),
                MySqlDialectProvider.Instance);
            using (var db = dbFactory.Open())
            {
                int flag = db.Delete<CPrivilege>(new { CFunctionId = functionName });
                if (flag > 0) return true;
                else return false;
            }
        }
        public CPrivilege GetPrivilege(string functionName, string userId)
        {
            var dbFactory = new OrmLiteConnectionFactory(
                GetConnectionString(),
                MySqlDialectProvider.Instance);
            using (var db = dbFactory.Open())
            {
                var sqlExp = db.From<CPrivilege>()
                    .Join<CPrivilege, CUser>((p, u) => u.Id == p.CUserId)
                    .Join<CPrivilege, CFunction>((p, f) => p.CFunctionId == f.Id)
                    .Where<CUser>(u => u.Id == userId)
                    .And<CFunction>(f => f.Name == functionName);

                return db.Single<CPrivilege>(sqlExp);
            }
        }
        public bool IsPrivilegeExist(string functionName, string userId)
        {
            var dbFactory = new OrmLiteConnectionFactory(
                GetConnectionString(),
                MySqlDialectProvider.Instance);
            using (var db = dbFactory.Open())
            {
                var sqlExp = db.From<CPrivilege>()
                    .Join<CPrivilege, CUser>((p, u) => u.Id == p.CUserId)
                    .Join<CPrivilege, CFunction>((p, f) => p.CFunctionId == f.Id)
                    .Where<CUser>(u => u.Id == userId)
                    .And<CFunction>(f => f.Name == functionName);

                return db.Exists<CPrivilege>(sqlExp);
            }

        }

        public CSetting GetSetting(string setingName)
        {
            var dbFactory = new OrmLiteConnectionFactory(
                GetConnectionString(),
                MySqlDialectProvider.Instance);
            using (var db = dbFactory.Open())
            {
                return db.Single<CSetting>(new { Name = setingName });
            }
        }
        public bool SaveSetting(CSetting setting)
        {
            var dbFactory = new OrmLiteConnectionFactory(
                GetConnectionString(),
                MySqlDialectProvider.Instance);
            using (var db = dbFactory.Open())
            {
                return db.Save<CSetting>(setting);
            }
        }
        public List<CSetting> GetSettingList()
        {
            var dbFactory = new OrmLiteConnectionFactory(
                GetConnectionString(),
                MySqlDialectProvider.Instance);
            using (var db = dbFactory.Open())
            {
                return db.Select<CSetting>();
            }
        }

    }
}
