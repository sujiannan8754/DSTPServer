using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSTP_DAL
{
    /// <summary>
    /// 堵剂数量表模型
    /// </summary>
    public class JamModel
    {
        /// <summary>
        /// 主键，GUID
        /// </summary>
        public string Jam_ID { get; set; }
        /// <summary>
        /// 工程信息表主键
        /// </summary>
        public string Project_ID { get; set; }
        /// <summary>
        /// 堵剂名称（化学名称）
        /// </summary>
        public string Jam_Name { get; set; }
        /// <summary>
        /// 数量（袋或桶）
        /// </summary>
        public float Jam_Num { get; set; }
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
