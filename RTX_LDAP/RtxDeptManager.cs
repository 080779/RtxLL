using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTXSAPILib;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RTX_LDAP
{
    public class RtxDeptManager
    {
        RTXSAPILib.RTXSAPIRootObj RootObj;  //声明一个根对象
        RTXSAPILib.RTXSAPIDeptManager DeptManager;
        
        //RTXSAPILib.RTXSAPIUserManager UserManager;

        public RtxDeptManager()
        {
            RootObj = new RTXSAPIRootObj();     //创建根对象            
            RootObj.ServerIP = "127.0.0.1";
            RootObj.ServerPort = Convert.ToInt16("8006");
            DeptManager = RootObj.DeptManager;
        }

        public bool AddUserToDept(string bstrUserName, string bstrSrcDeptName, string bstrDestDeptName, bool bIsCopy)
        {
            try
            {
                DeptManager.AddUserToDept(bstrUserName, bstrSrcDeptName, bstrDestDeptName, bIsCopy);
                return true;
            }

            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                return false;
            }
        }     
        
        //public bool XmlAdd()
        //{
        //    RTXServerApi.RTXObjectClass RTXObj = new RTXObjectClass();  //创建一个业务逻辑对象
        //    RTXServerApi.RTXCollectionClass RTXParams = new RTXCollectionClass();// 创建一个集合对象

        //    RTXObj.Name = "USERSYNC";  //业务逻辑对象名称为用户同步类别
        //    RTXParams.Add("MODIFYMODE", 1);
        //    RTXParams.Add("DATA", "");

        //    try
        //    {
        //        RTXObj.Call2(enumCommand_.PRO_SYNC_TO_RTX, RTXParams);
        //    }
        //    catch (COMException ex)
        //    {

        //    }
        //}   

        public bool AddDept(string bstrDeptName,string bstrParentDept)
        {
            try
            {                
                DeptManager.AddDept(bstrDeptName, bstrParentDept);
                return true;
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                return false;
            }
        }

        public void DelDept(string bstrDeptName)
        {
            try
            {
                DeptManager.DelDept(bstrDeptName, true);
            }
            catch(Exception ex)
            {

            }
        }

        public string GetUserDeptsName(string userName)
        {
            try
            {
                string[] paths= DeptManager.GetUserDepts(userName).Split('"');
                return paths[1];
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                return null;
            }
        }
    }
}
