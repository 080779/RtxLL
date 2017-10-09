using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;

namespace RTX_LDAP
{
    public class OperateAD
    {
        public string DomainNamePath { get; set; }
        public string AdminUser { get; set; }
        public string AdminPwd { get; set; }

        /// <summary>
        /// 是否连接到域
        /// </summary>
        /// <param name="domainName">域地址</param>
        /// <param name="adminUser">域管理员账户名</param>
        /// <param name="adminPwd">域管理员密码</param>
        /// <returns>返回域节点</returns>
        public DirectoryEntry DoMainConnected()
        {
            DirectoryEntry domain = new DirectoryEntry();
            try
            {
                domain.Path = DomainNamePath;
                domain.Username = AdminUser;
                domain.Password = AdminPwd;
                domain.AuthenticationType = AuthenticationTypes.Secure;
                domain.RefreshCache();
                return domain;
            }
            catch (Exception ex)
            {
                throw new Exception("[DoMainConnected方法]错误信息：" + ex.Message);                
            }
        }

        public bool CheckADUser(string domainPath, string userName, string password)
        {
            try
            {
                DirectoryEntry domain = new DirectoryEntry(domainPath, userName, password);
                domain.AuthenticationType = AuthenticationTypes.Secure;
                domain.RefreshCache();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        public bool IsADUserExist(DirectoryEntry entry, string userName)
        {
            using (DirectorySearcher search = new DirectorySearcher(entry))
            {
                search.Filter = "(sAMAccountName=" + userName + ")";
                //search.PropertiesToLoad.Add("cn"); //不指定加载查询属性，不会把属性查出来
                SearchResult result = search.FindOne();
                if (result == null)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 通过组织单位名找到组织单位地址
        /// </summary>
        /// <param name="objDE">域节点，对象</param>
        /// <param name="ouName">组织单位名</param>
        /// <returns>返回组织单位地址</returns>
        public string OUEntryPath(DirectoryEntry objDE, string ouName)
        {
            using (DirectorySearcher objSearcher = new DirectorySearcher(objDE, "ou=" + ouName))
            {
                SearchResult src = objSearcher.FindOne();
                return src.Path;
            }                    
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objDE"></param>
        /// <param name="ouName"></param>
        /// <param name="rootOUName"></param>
        /// <returns></returns>
        public string OUEntryPathForFilter(DirectoryEntry objDE, string filter,string rootOUName)
        {
            using (DirectorySearcher objSearcher = new DirectorySearcher(objDE, filter))
            {
                SearchResultCollection srcs = objSearcher.FindAll();
                string path = null;
                foreach(SearchResult src in srcs)
                {
                    if (src.Path.Contains(rootOUName))
                    {
                        path = src.Path;
                    }
                }
                return path;
            }
        }

        public void OUEntryToRTX(DirectoryEntry objDE, string filter, RtxDeptManager rdm)
        {
            using (DirectorySearcher objSearcher = new DirectorySearcher(objDE, filter))
            {
                objSearcher.PropertiesToLoad.Add("distinguishedName");
                SearchResultCollection srcs = objSearcher.FindAll();
                foreach (SearchResult src in srcs)
                {
                    string[] strs = src.Properties["distinguishedName"][0].ToString().Split(',');
                    if (!strs[1].Contains("OU="))
                    {
                        rdm.AddDept(strs[0].Replace("OU=", ""), "");
                    }
                    else
                    {
                        rdm.AddDept(strs[0].Replace("OU=", ""), strs[1].Replace("OU=", ""));
                        
                    }
                }
            }
        }

        public void UserNameToRTX(DirectoryEntry objDE, string filter, RtxUserManager rum,RtxDeptManager rdm)
        {
            //(&(objectCategory=person)(objectClass=user))
            using (DirectorySearcher objSearcher = new DirectorySearcher(objDE, filter))
            {
                objSearcher.PropertiesToLoad.Add("distinguishedName");
                objSearcher.PropertiesToLoad.Add("name");
                objSearcher.PropertiesToLoad.Add("displayName");
                objSearcher.PropertiesToLoad.Add("mail");
                objSearcher.PropertiesToLoad.Add("telephoneNumber");
                SearchResultCollection srcs = objSearcher.FindAll();
                foreach (SearchResult src in srcs)
                {
                    string[] paths= src.Properties["distinguishedName"][0].ToString().Replace("DC=","").Replace("OU=","").Replace("CN=","").Split(',');
                    string deptName = paths[1];
                    StringBuilder builder = new StringBuilder();
                    for(int i=paths.Length-1;i>0;i--)
                    {
                        builder.Append(paths[i]).Append(@"\");
                    }
                    string path = builder.ToString().Replace(@"com\test\","");
                    string userName = src.Properties["name"][0].ToString();
                    string displayName = "RTX_NULL";
                    int gender = -1;
                    string mobile = "RTX_NULL";
                    string email = "RTX_NULL";
                    string phone = "RTX_NULL";
                    if(src.Properties["displayName"].Count == 1)
                    {
                        displayName = src.Properties["displayName"][0].ToString();
                    }
                    if (src.Properties["mail"].Count == 1)
                    {
                        email = src.Properties["mail"][0].ToString();
                    }
                    if (src.Properties["telephoneNumber"].Count == 1)
                    {
                        mobile = src.Properties["telephoneNumber"][0].ToString();
                    }
                    if(rdm.GetUserDeptsName(userName)==null)
                    {
                        rum.AddRtxUser(userName, 1);
                        rum.SetBasicRtxUser(userName, displayName, gender, mobile, email, phone, 1);
                        //rum.SetUserDeptPosition(userName, deptName, path);
                        rdm.AddUserToDept(userName, null, path, false);
                    }
                    else
                    {
                        rdm.AddUserToDept(userName, rdm.GetUserDeptsName(userName), path, false);
                    }
                }
            }
        }

        public string GetAllOUEntry(DirectoryEntry objDE, string filter)
        {
            using (DirectorySearcher objSearcher = new DirectorySearcher(objDE, filter))
            {
                SearchResultCollection srcs = objSearcher.FindAll();
                DirectoryEntry entry = null;
                string values = null;
                for (int i = 0; i < srcs.Count; i++)
                {
                    entry = srcs[i].GetDirectoryEntry();
                    foreach (string pro in entry.Properties.PropertyNames)
                    {
                        values += entry.Properties[pro].PropertyName + ".........." + entry.Properties[pro].Value.ToString() + " \r\n";
                    }
                }
                return values;
            }
        }

        public Dictionary<string, string> GetAllOUEntryToDir(DirectoryEntry objDE,string filter, Dictionary<string, string> dir)
        {
            using (DirectorySearcher objSearcher = new DirectorySearcher(objDE, filter))
            {
                objSearcher.PropertiesToLoad.Add("distinguishedName");
                SearchResultCollection srcs = objSearcher.FindAll();
                //DirectoryEntry entry = null;
                //string values = null;
                //for (int i = 0; i < srcs.Count; i++)
                //{
                //    //entry = srcs[i].GetDirectoryEntry();
                //    foreach (string pro in entry.Properties.PropertyNames)
                //    {
                //        values += entry.Properties[pro].PropertyName + ".........." + entry.Properties[pro].Value.ToString() + " \r\n";
                //    }
                //}
                foreach(SearchResult src in srcs)
                {
                    //dir.Add(src.Properties["parent"][0].ToString(), src.Properties["ou"][0].ToString());
                    string[] strs=src.Properties["distinguishedName"][0].ToString().Split(',');
                    if(!strs[1].Contains("OU="))
                    {
                        dir.Add(strs[0].Replace("OU=", ""), "");
                    }
                    else
                    {
                        dir.Add(strs[0].Replace("OU=", ""), strs[1].Replace("OU=", ""));
                    }                    
                }
                return dir;
            }
        }

        public void GetChildOUEntry(DirectoryEntry objDE, RtxDeptManager rdm)
        {            
            foreach(DirectoryEntry entry in objDE.Children)
            {
                rdm.AddDept(entry.Properties["ou"].Value.ToString(), objDE.Properties["ou"].Value.ToString());
                if(entry.Children!=null)
                {
                    GetChildOUEntry(entry, rdm);
                }
            }
        }

        public Dictionary<string, string> GetChildOUEntryName(DirectoryEntry objDE,Dictionary<string,string> dir)
        {
            foreach (DirectoryEntry entry in objDE.Children)
            {
                //rdm.AddDept(entry.Properties["ou"].Value.ToString(), objDE.Properties["ou"].Value.ToString());
                if(entry.Children!=null)
                {                    
                    if (entry != null)
                    {
                        dir.Add(entry.Properties["ou"].Value.ToString(), objDE.Properties["ou"].Value.ToString());
                        GetChildOUEntryName(entry, dir);
                    }
                    else
                    {
                        break;
                    }
                }                
                else
                {
                    break;
                }
            }
            return dir;
        }

        /// <summary>
        /// 通过组名查找组地址
        /// </summary>
        /// <param name="objDE">域节点，对象</param>
        /// <param name="groupName">组名</param>
        /// <returns>返回组地址</returns>
        public string GroupEntryPath(DirectoryEntry objDE, string groupName)
        {
            using (DirectorySearcher objSearcher = new DirectorySearcher(objDE, "cn=" + groupName))
            {
                SearchResult src = objSearcher.FindOne();
                return src.Path;
            }
        }

        public bool AddAccount(DirectoryEntry entry,DomainUser user)
        {
            try
            {
                DirectoryEntry NewUser = entry.Children.Add("CN=" + user.Name, "user");
                NewUser.Properties["sAMAccountName"].Add(user.Name); //account
                NewUser.Properties["userPrincipalName"].Value = user.UserPrincipalName; //user logon name,xxx@bdxy.com
                if (!string.IsNullOrEmpty(user.Company))
                {
                    NewUser.Properties["company"].Value = user.Company;
                }
                if (!string.IsNullOrEmpty(user.Department))
                {
                    NewUser.Properties["department"].Value = user.Department;
                }
                if (!string.IsNullOrEmpty(user.Description))
                {
                    NewUser.Properties["description"].Value = user.Description;
                }
                if (!string.IsNullOrEmpty(user.DisplayName))
                {
                    NewUser.Properties["displayName"].Value = user.DisplayName;
                }
                if (!string.IsNullOrEmpty(user.GivenName))
                {
                    NewUser.Properties["givenName"].Value = user.GivenName;
                }
                if (!string.IsNullOrEmpty(user.Initials))
                {
                    NewUser.Properties["initials"].Value = user.Initials;
                }
                if (!string.IsNullOrEmpty(user.Mail))
                {
                    NewUser.Properties["mail"].Value = user.Mail;
                }
                if (!string.IsNullOrEmpty(user.Name))
                {
                    NewUser.Properties["name"].Value = user.Name;
                }
                if (!string.IsNullOrEmpty(user.PhysicalDeliveryOfficeName))
                {
                    NewUser.Properties["physicalDeliveryOfficeName"].Value = user.PhysicalDeliveryOfficeName;
                }
                if (!string.IsNullOrEmpty(user.SN))
                {
                    NewUser.Properties["sn"].Value = user.SN;
                }
                if (!string.IsNullOrEmpty(user.TelephoneNumber))
                {
                    NewUser.Properties["telephoneNumber"].Value = user.TelephoneNumber;
                }
                NewUser.CommitChanges();
                //设置密码
                //反射调用修改密码的方法（注意端口号的问题  端口号会引起方法调用异常）
                NewUser.Invoke("SetPassword", new object[] { user.UserPwd });
                //默认设置新增账户启用
                NewUser.Properties["userAccountControl"].Value = 0x200;
                NewUser.CommitChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public List<string> AccsesADQuery(string userName)
        {
            //定义de进入AD架构
            //DirectoryEntry de = OUEntry("tiantianwang");
            DirectoryEntry de = new DirectoryEntry();
            //定义ds查找AD
            DirectorySearcher ds = new DirectorySearcher(de);
            string value = string.Empty;
            List<string> domainList = new List<string>();
            try
            {
                //3.定义查询
                ds.Filter = "(SAMAccountName=" + userName + ")";
                ds.PropertiesToLoad.Add("SAMAccountName");//account
                ds.PropertiesToLoad.Add("Name");//full name
                ds.PropertiesToLoad.Add("displayName");
                ds.PropertiesToLoad.Add("mail");
                ds.PropertiesToLoad.Add("sn");
                ds.PropertiesToLoad.Add("description");
                ds.PropertiesToLoad.Add("Department");
                ds.PropertiesToLoad.Add("userPrincipalName");//user logon name,xxx@bdxy.com
                ds.PropertiesToLoad.Add("physicalDeliveryOfficeName");
                ds.PropertiesToLoad.Add("telephoneNumber");
                //查找一个
                SearchResult sr = ds.FindOne();
                if (sr != null)
                {
                    //列出值
                    foreach (string key in sr.Properties.PropertyNames)
                    {
                        foreach (object obj in de.Properties[key])
                        {
                            value += key + " = " + obj + Environment.NewLine;
                            domainList.Add(value);
                        }
                    }
                    return domainList;
                }
                else
                {
                    return domainList;
                }
            }
            catch (Exception ex)
            {
                //DomainUser.Failed = ex.Message.ToString();
                return domainList;
            }
            finally
            {
                if (ds != null)
                {
                    ds.Dispose();
                }
                if (de != null)
                {
                    de.Dispose();
                }
            }
        }

    }
}
