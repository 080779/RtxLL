using System;
using System.Collections.Generic;
using System.Text;

namespace RTX_LDAP
{
    /// <summary>
    /// 域名用户模型
    /// </summary>
    public class DomainUser
    {        
        /// <summary>
        /// 姓
        /// </summary>
        public string SN{ get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description{ get; set; }
        /// <summary>
        /// 办公室名
        /// </summary>
        public string PhysicalDeliveryOfficeName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string TelephoneNumber { get; set; }
        /// <summary>
        /// 名
        /// </summary>
        public string GivenName { get; set; }
        /// <summary>
        /// 英文缩写
        /// </summary>
        public string Initials { get; set; }
        /// <summary>
        /// 标识用户在域树上的唯一位置 
        /// </summary>
        public string DistinguishedName { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 公司
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 用户登录名(Windows 2000 以前版本)
        /// </summary>
        public string SAMAccountName { get; set; }
        /// <summary>
        /// 用户登录名
        /// </summary>
        public string UserPrincipalName { get; set; }
        /// <summary>
        /// 用户登录密码
        /// </summary>
        public string UserPwd { get; set; }
        /// <summary>
        /// 电子邮件
        /// </summary>
        public string Mail { get; set; }
    }
}
