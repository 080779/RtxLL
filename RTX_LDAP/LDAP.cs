using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;
using System.Runtime.InteropServices;


namespace RTX_LDAP
{
    class LDAP
    {
        const int LOGON32_LOGON_INTERACTIVE = 2; //通过网络验证账户合法性
        const int LOGON32_PROVIDER_DEFAULT = 0; //使用默认的Windows 2000/NT NTLM验证方
        [DllImport("advapi32.dll")]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, uint dwLogonType, uint dwLogonProvider, out IntPtr phToken);

        public bool IsAuthenticated(string server,string userName, string password)
        {
            string sAMAccountName = userName;
            IntPtr tokenHandle = new IntPtr(0);
            tokenHandle = IntPtr.Zero;
            bool checkok = LogonUser(sAMAccountName, "LDAP://127.0.0.1/DC=test,DC=com", password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out tokenHandle);
            return checkok;

        }

        public bool CheckADUser(string domainPath, string userName, string password)
        {
            try
            {
                DirectoryEntry domain = new DirectoryEntry("LDAP://127.0.0.1/DC=test,DC=com", userName, password);
                domain.AuthenticationType = AuthenticationTypes.Secure;
                domain.RefreshCache();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string GetLoginName(string server, string userName)
        {
            string returnStr = string.Empty;
            SearchResultCollection results = null;
            string filter = "(&(objectCategory=user)(objectClass=person)(cn=" + userName + "))";
            string connectionPrefix = string.Format("LDAP://{0}", server);
            using (DirectoryEntry root = new DirectoryEntry(connectionPrefix, "zqs", "Asd123456"))
            {
                using (DirectorySearcher searcher = new DirectorySearcher(root))
                {
                    searcher.ReferralChasing = ReferralChasingOption.All;
                    searcher.SearchScope = SearchScope.Subtree;
                    searcher.Filter = filter;
                    results = searcher.FindAll();
                }
            }
            foreach (SearchResult sr in results)
            {
                DirectoryEntry entry = sr.GetDirectoryEntry();
                PropertyValueCollection pg = entry.Properties["sAMAccountName"];
                returnStr = (string)pg.Value;
            }
            return returnStr;
        }
    }
}
