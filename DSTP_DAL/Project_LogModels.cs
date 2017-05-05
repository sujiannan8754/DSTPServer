using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSTP_DAL
{
    /// <summary>
    /// 工程日志模型
    /// </summary>
    public class Project_LogModels
    {
        /// <summary>
        /// 主键，GUID生成
        /// </summary>
        public string Project_Log_ID { get; set; }
        /// <summary>
        /// 工程信息表主键
        /// </summary>
        public string Project_ID { get; set; }
        /// <summary>
        /// 日志所在工程的日志编号
        /// </summary>
        public int Project_Log_No { get; set; }
        /// <summary>
        /// 施工时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public DateTime Construction_Time { get; set; }
        /// <summary>
        /// 压力MPa
        /// </summary>
        public float Pressure { get; set; }
        /// <summary>
        /// 排量m3/h
        /// </summary>
        public float Displacement { get; set; }
        /// <summary>
        /// 累计注入量m3
        /// </summary>
        public float Displacement_Acc { get; set; }
        /// <summary>
        /// 段塞名
        /// </summary>
        public string Slug_Name { get; set; }
        /// <summary>
        /// 堵剂配方
        /// </summary>
        public string Formula { get; set; }
        /// <summary>
        /// 事件
        /// </summary>
        public string Events { get; set; }
        /// <summary>
        /// 标记是否已上传至内网；0已上传，1未上传
        /// </summary>
        public int Sign { get; set; }
        /// <summary>
        /// 备注字段1，用于后期使用
        /// </summary>
        public int Remark1 { get; set; }
        /// <summary>
        /// 备注字段2，用于后期使用
        /// </summary>
        public string Remark2 { get; set; }
        /// <summary>
        /// 最后操作人
        /// </summary>
        public string Last_Operate_People { get; set; }
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
