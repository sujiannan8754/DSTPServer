using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSTP_BLL.ClassToLan
{
    public class ClassConnectToLan
    {
        /// <summary>
        /// 消息帧头尾枚举
        /// </summary>
        public enum StateRxOneString
        {
            /// <summary>
            /// 什么都没收到
            /// </summary>
            NoSTX,
            /// <summary>
            /// 已收到STX
            /// </summary>
            WAIT_ETX
        }
    }
}
