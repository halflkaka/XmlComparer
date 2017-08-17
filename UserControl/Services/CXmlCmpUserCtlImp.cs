using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using XmlCompare.Infrastructure.Entity;
using XmlCompare.Infrastructure.Interface;

namespace XmlCompare.UseControl.Services
{
        [Export(typeof(IXmlCompareUserControl))]
        public class CXmlCmpUserCtlImp : IXmlCompareUserControl
        {
            private IXmlCmpDbAdp m_Dba;
            private IXmlCompareToolSetting m_Setting;
            
            [ImportingConstructor]
            public CXmlCmpUserCtlImp(IXmlCmpDbAdp dba,
                IXmlCompareToolSetting setting)
            {
                m_Dba = dba;
                m_Setting = setting;
            }
            public CUser GetUser(string id)
            {
                return m_Dba.GetUserById(id);
            }
            public CUser GetCurrentUser()
            {
                string userName = UserPrincipal.Current.SamAccountName;
                string userFullName = UserPrincipal.Current.DisplayName;
                return new CUser() { Id = userName, Name = userFullName };
            }
            public List<CUser> GetUsers()
            {
                return m_Dba.GetUserList();
            }

            public CUser CreateUser(string id, string name)
            {
                CUser neoUser = new CUser { Id = id, Name = name };
                if (m_Dba.SaveUser(neoUser))
                {
                    return neoUser;
                }
                else
                {
                    return null;
                }
            }
            public bool CheckUser(string id)
            {
                return m_Dba.IsUserExist(id);
            }

            public List<CFunction> GetFunctions()
            {
                return m_Dba.GetFunctionList();
            }

            public List<CUser> GetFunctionUsers(string functionName, bool flag)
            {
                List<CUser> result = new List<CUser>();
                if (functionName != "XmlSettingFilePath")
                {
                    result = m_Dba.GetUsetListForFunction(functionName, flag);
                }
                else
                {
                    CSetting tmpSetting = m_Setting.GetXmlSettingFilePath();
                    if (tmpSetting != null)
                    {
                        List<CUser> lstUser = m_Dba.GetUserList();
                        List<string> lstUserID = new List<string>();
                        string tmpDirectoryName = (new System.IO.FileInfo(tmpSetting.Value)).DirectoryName;
                        GetDirectorySecurity(tmpDirectoryName,
                            FileSystemRights.FullControl,
                            AccessControlType.Allow,
                            ref lstUserID);
                        if (flag)
                        {
                            result = (from CUser tmp in lstUser
                                      where lstUserID.Exists(id => id == tmp.Id)
                                      select tmp).ToList();
                        }
                        else
                        {
                            result = (from CUser tmp in lstUser
                                      where !lstUserID.Exists(id => id == tmp.Id)
                                      select tmp).ToList();
                        }

                    }
                }
                return result;
            }
            public void SetFunctionUsers(string functionName, List<CUser> userList)
            {
                if (functionName != "XmlSettingFilePath")
                {
                    m_Dba.DeleteAllPrivelegeForFunciton(functionName);
                    List<CPrivilege> lstPrivilege = new List<CPrivilege>();
                    CFunction tmpFunc = m_Dba.GetFunctionByName(functionName);
                    foreach (CUser tmpUser in userList)
                    {
                        lstPrivilege.Add(new CPrivilege() { CFunctionId = tmpFunc.Id, CUserId = tmpUser.Id });
                    }
                    m_Dba.SaveAllPrivelege(lstPrivilege);
                }
                else
                {
                    CSetting tmpSetting = m_Setting.GetXmlSettingFilePath();
                    if (tmpSetting != null)
                    {
                        List<string> lstUserID = new List<string>();
                        string tmpDirectoryName = (new System.IO.FileInfo(tmpSetting.Value)).DirectoryName;
                        GetDirectorySecurity(tmpDirectoryName,
                            FileSystemRights.FullControl,
                            AccessControlType.Allow,
                            ref lstUserID);
                        // add new fullcontrol access permision
                        foreach (CUser tmpUser in userList)
                        {
                            if (!lstUserID.Exists(tmp => tmp == tmpUser.Id))
                            {
                                AddDirectorySecurity(tmpDirectoryName,
                                    "SF\\" + tmpUser.Id,
                                    FileSystemRights.FullControl,
                                    AccessControlType.Allow);
                            }
                        }
                        // remove useless fullcontrol access permision
                        foreach (string tmpUserId in lstUserID)
                        {
                            if (!userList.Exists(tmp => tmp.Id == tmpUserId))
                            {
                                RemoveDirectorySecurity(tmpDirectoryName,
                                    "SF\\" + tmpUserId,
                                    FileSystemRights.FullControl,
                                    AccessControlType.Allow);
                            }
                        }
                    }
                }
            }
            public bool IsUserHasPriviledge(CUser user, string functionName)
            {
                if (functionName != "XmlSettingFilePath")
                {
                    return m_Dba.IsPrivilegeExist(functionName, user.Id);
                }
                else
                {
                    CSetting tmpSetting = m_Setting.GetXmlSettingFilePath();
                    string tmpDirectoryName = (new System.IO.FileInfo(tmpSetting.Value)).DirectoryName;
                    List<string> lstAuthUserID = new List<string>();
                    GetDirectorySecurity(tmpDirectoryName,
                                    FileSystemRights.FullControl,
                                    AccessControlType.Allow,
                                    ref lstAuthUserID);
                    return lstAuthUserID.Exists(userId => userId == user.Id);
                }
            }

            // Add sharing permission method
            private void AddDirectorySecurity(string FolderName, string Account, FileSystemRights Rights, AccessControlType ControlType)
            {

                try
                {
                    // Create a new DirectoryInfo object.
                    DirectoryInfo dInfo = new DirectoryInfo(FolderName);

                    // Get a DirectorySecurity object that represents the  
                    // current security settings.
                    DirectorySecurity dSecurity = dInfo.GetAccessControl();

                    // Add the FileSystemAccessRule to the security settings. 
                    dSecurity.AddAccessRule(new FileSystemAccessRule(Account, Rights, ControlType));

                    // Set the new access settings.
                    dInfo.SetAccessControl(dSecurity);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }

            // remove sharing permission method
            private void RemoveDirectorySecurity(string FolderName, string Account, FileSystemRights Rights, AccessControlType ControlType)
            {
                // Create a new DirectoryInfo object.
                DirectoryInfo dInfo = new DirectoryInfo(FolderName);

                // Get a DirectorySecurity object that represents the  
                // current security settings.
                DirectorySecurity dSecurity = dInfo.GetAccessControl();

                // Add the FileSystemAccessRule to the security settings. 
                dSecurity.RemoveAccessRule(new FileSystemAccessRule(Account, Rights, ControlType));

                // Set the new access settings.
                dInfo.SetAccessControl(dSecurity);

            }

            // read sharing permission method 
            public void GetDirectorySecurity(string FolderName, FileSystemRights Rights, AccessControlType ControlType, ref List<string> UserList)
            {
                // Create a new DirectoryInfo object.
                DirectoryInfo dInfo = new DirectoryInfo(FolderName);

                // Get a DirectorySecurity object that represents the  
                // current security settings.
                DirectorySecurity dSecurity = dInfo.GetAccessControl();

                foreach (FileSystemAccessRule rule in
                    dSecurity.GetAccessRules(true, true,
                    typeof(System.Security.Principal.NTAccount)))
                {
                    if (rule.FileSystemRights == Rights && rule.AccessControlType == ControlType)
                    {
                        string tmp = rule.IdentityReference.Value;
                        if (tmp.IndexOf("SF\\") >= 0)
                        {
                            UserList.Add(tmp.Split('\\')[1].ToUpper());
                        }
                    }
                }
            }
        }
    
}
