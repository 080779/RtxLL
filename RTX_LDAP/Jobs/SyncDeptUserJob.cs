using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTX_LDAP.Jobs
{
    public class SyncDeptUserJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            RtxDeptManager rdm = new RtxDeptManager();
            RtxUserManager rum = new RtxUserManager();
            OperateAD oad = new OperateAD();
            rdm.DelDept("已删除用户");
            oad.DomainNamePath = "LDAP://192.168.31.134/OU=中迅,DC=test,DC=com";
            oad.AdminUser = "hzy";
            oad.AdminPwd = "Asd123456";
            oad.OUEntryToRTX(oad.DoMainConnected(), "objectclass=organizationalUnit", rdm);

            oad.UserNameToRTX(oad.DoMainConnected(), "(&(objectCategory=person)(objectClass=user))", rum, rdm);
            rdm.DelDept("已删除用户");
            File.WriteAllText("c:/sync.txt","同步成功"+DateTime.Now);
        }
    }
}
