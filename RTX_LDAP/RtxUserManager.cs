using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTXSAPILib;
using System.Windows.Forms;

namespace RTX_LDAP
{
    public class RtxUserManager
    {
        RTXSAPILib.RTXSAPIRootObj RootObj;  //声明一个根对象
        RTXSAPILib.IRTXSAPIUserManager UserManager;

        public RtxUserManager()
        {
            RootObj = new RTXSAPIRootObj();     //创建根对象            
            RootObj.ServerIP = "127.0.0.1";
            RootObj.ServerPort = Convert.ToInt16("8006");
            UserManager = RootObj.UserManager;
        }

        public bool AddRtxUser(string bstrUserName, int IAuthType)
        {
            try
            {
                UserManager.AddUser(bstrUserName, IAuthType);
                return true;
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                return false;
            }
        }
        public bool SetBasicRtxUser(string bstrUserName, string bstrName = "RTX_NULL", int gender = -1, string bstrMobile = "RTX_NULL", string bstrEMail = "RTX_NULL", string bstrPhone = "RTX_NULL", int IAuthType = -1)
        {
            try
            {
                UserManager.SetUserBasicInfo(bstrUserName,bstrName,gender,bstrMobile,bstrEMail,bstrPhone,IAuthType);
                
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                return false;
            }
        }
        public bool SetUserDeptPosition(string bstrUserName, string bstrDept, string bstrExPostion)
        {
            try
            {
                UserManager.SetUserDeptExPosition(bstrUserName,bstrDept,bstrExPostion);
                return true;
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                return false;
            }
        }        
    }
}
