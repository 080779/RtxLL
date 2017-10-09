using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using RTX_LDAP.Jobs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTX_LDAP
{
    public partial class FrmRtxLdapPlugin : Form
    {
        static public string dc = string.Empty;
        static public string rtxip = string.Empty;
        static public string rtxport = string.Empty;

        public FrmRtxLdapPlugin()
        {
            InitializeComponent();
        }

        private void btn_Set_Click(object sender, EventArgs e)
        {
            dc = txt_dc.Text.Trim();
            rtxip = txt_rtxip.Text.Trim();
            rtxport = txt_rtxport.Text.Trim();
            if (dc == string.Empty || rtxip == string.Empty || rtxport == string.Empty)
            {
                MessageBox.Show("DC、RTX服务器IP及APP端口不能为空");
            }
            else
            {
                btn_Set.Enabled = false;
                btn_start.Enabled = true;
            }
        }


        private void btn_start_Click(object sender, EventArgs e)
        {
            RTX rtx = new RTX();
            rtx.RegApp();
            rtx.StartApp();
            btn_start.Enabled = false;
            btn_Cancel.Enabled = true;
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            RTX rtx = new RTX();
            rtx.StopApp();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            RTX rtx = new RTX();
            rtx.UnregApp();
            btn_start.Enabled = true;
            btn_Set.Enabled = true;
            btn_Cancel.Enabled = false;
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            RtxDeptManager rdm = new RtxDeptManager();
            RtxUserManager rum = new RtxUserManager();
            OperateAD oad = new OperateAD();
            oad.DomainNamePath = "LDAP://192.168.31.134/OU=中迅,DC=test,DC=com";
            oad.AdminUser = "hzy";
            oad.AdminPwd = "Asd123456";
            oad.OUEntryToRTX(oad.DoMainConnected(), "objectclass=organizationalUnit", rdm);
            oad.UserNameToRTX(oad.DoMainConnected(), "(&(objectCategory=person)(objectClass=user))", rum, rdm);
            rdm.DelDept("已删除用户");
            MessageBox.Show("同步成功");
        }

        private void FrmRtxLdapPlugin_Load(object sender, EventArgs e)
        {
            IScheduler sched = new StdSchedulerFactory().GetScheduler();
            JobDetailImpl syncDeptUser = new JobDetailImpl("syncDeptUser", typeof(SyncDeptUserJob));
            //IMutableTrigger triggerSyncDeptUser = CronScheduleBuilder.DailyAtHourAndMinute(16,03).Build();//每天 23:45 执行一次
            CalendarIntervalScheduleBuilder triggerSyncDeptUserBuilder = CalendarIntervalScheduleBuilder.Create();
            IMutableTrigger triggerSyncDeptUser=triggerSyncDeptUserBuilder.WithInterval(60, IntervalUnit.Second).Build();//每 3 秒钟执行一次 
            triggerSyncDeptUser.Key = new TriggerKey("triggerSyncDeptUser");
            sched.ScheduleJob(syncDeptUser, triggerSyncDeptUser);
            sched.Start();
        }
    }
}
