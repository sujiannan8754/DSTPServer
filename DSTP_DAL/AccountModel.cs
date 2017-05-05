using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSTP_DAL
{
    /// <summary>
    /// Account模型
    /// </summary>
    public class AccountModel
    {
        /// <summary>
        /// 账号主键，GUID
        /// </summary>
        public string Account_ID { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Account_Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Account_Password { get; set; }
        /// <summary>
        /// 注册时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public DateTime Account_Time { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public string TEL { get; set; }
        /// <summary>
        /// 施工单位名称
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// 头像本地地址
        /// </summary>
        public string Account_Picture { get; set; }
        /// <summary>
        /// 是否使用；0已使用，1未使用
        /// </summary>
        public int Account_IsUse { get; set; }
        /// <summary>
        /// 账号权限；0为APP管理员权限，1为场地录入人员权限，2为内网使用者权限
        /// </summary>
        public int Account_Permission { get; set; }
        /// <summary>
        /// 标记是否已上传至内网；0已上传，1未上传
        /// </summary>
        public int Sign { get; set; }
        /// <summary>
        /// 备注字段1，用于后期使用，格式设置为int
        /// </summary>
        public int Remark1 { get; set; }
        /// <summary>
        /// 备注字段2，用于后期使用
        /// </summary>
        public string Remark2 { get; set; }
        /// <summary>
        /// 最后操作时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public DateTime Last_Operate_Date { get; set; }
        /// <summary>
        /// 最后操作；0新增、1修改、2删除
        /// </summary>
        public int Last_Operate_Type { get; set; }
    }
}
