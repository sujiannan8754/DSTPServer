using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSTP_DAL
{
    /// <summary>
    /// RTU数据模型
    /// </summary>
    public class RtuDataModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string RTU_Data_ID { get; set; }
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 压力
        /// </summary>
        public float Pressure { get; set; }
        /// <summary>
        /// 当前流量
        /// </summary>
        public float Displacement { get; set; }
        /// <summary>
        /// 累计流量
        /// </summary>
        public float Displacement_Acc { get; set; }
        /// <summary>
        /// 流体电导比
        /// </summary>
        public float ConductanceRatio { get; set; }
        /// <summary>
        /// 环境温度
        /// </summary>
        public float TemperatureInBox { get; set; }
        /// <summary>
        /// 机箱温度
        /// </summary>
        public float TemperatureOutside { get; set; }
        /// <summary>
        /// 测试时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public DateTime Detect_Time { get; set; }
        /// <summary>
        /// 上传时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public DateTime Upload_Time { get; set; }
        /// <summary>
        /// 标记  标记是否已上传至内网；0已上传，1未上传
        /// </summary>
        public int Sign { get; set; }
    }

    /// <summary>
    /// RTU图片模型
    /// </summary>
    public class RtuPicModel
    {
        /// <summary>
        /// 主键，GUID
        /// </summary>
        public string RTU_Pic_ID { get; set; }
        /// <summary>
        /// 图片编号；RTU编号+上传时间
        /// </summary>
        public string Pic_name { get; set; }
        /// <summary>
        /// RTU设备编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string RTU_Pic_Address { get; set; }
        /// <summary>
        /// 检测时间
        /// </summary>
        public DateTime Detect_Time { get; set; }
        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime Upload_Time { get; set; }
        /// <summary>
        /// 标记是否已上传至内网；0已上传，1未上传
        /// </summary>
        public int Sign { get; set; }
    }

    /// <summary>
    /// RTU参数模型
    /// </summary>
    public class RtuParameterModel
    {
        /// <summary>
        /// RTU参数主键，GUID表示
        /// </summary>
        public string RTU_ParaMeter_ID { get; set; }
        /// <summary>
        /// RTU设备编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 启用状态 ；1正常使用，2停用，3检修
        /// </summary>
        public int RTU_ParaMeter_No { get; set; }
        /// <summary>
        /// RTU类型
        /// </summary>
        public int RTU_Type { get; set; }
        /// <summary>
        /// RTU分组
        /// </summary>
        public string RTU_Group { get; set; }
    }
}
