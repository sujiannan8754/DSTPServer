using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DSTP_BLL.ClassToRtu
{
    /// <summary>
    /// 实时数据的实体模型
    /// </summary>
    public class ClassTableEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Int32 ID;
        /// <summary>
        /// 告警
        /// </summary>
        public UInt32 Alarm;
        /// <summary>
        /// 压力
        /// </summary>
        public Int32 Pressure;
        /// <summary>
        /// 当前流量
        /// </summary>
        public Int64 FlowCurrent;
        /// <summary>
        /// 累计流量
        /// </summary>
        public Int64 FlowTotal;
        /// <summary>
        /// 流体电导比
        /// </summary>
        public Int64 ConductanceRatio;
        /// <summary>
        /// 环境温度
        /// </summary>
        public Int32 TemperatureInBox;
        /// <summary>
        /// 机箱温度
        /// </summary>
        public Int32 TemperatureOutside;
        /// <summary>
        /// 测试时间
        /// </summary>
        public string CaptureTime;
        /// <summary>
        /// 
        /// </summary>
        public Int32 TransmitOK;
        public Int32 Year;
        public Int32 Month;
        public Int32 Day;
        public byte hour;
        public byte minute;
        public byte second;
        public Mutex mutAccessMe;
    }

    class ClassDbOperationExtend
    {
    }
}
