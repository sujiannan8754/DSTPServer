using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using DSTP_DAL;

namespace DSTP_BLL.ClassToRtu
{


    /// <summary>
    /// 解帧后的消息结构
    /// </summary>
    public struct MessageReceived
    {
        /// <summary>
        /// 数组长度
        /// </summary>
        public int CountBytes;  //数组长度
        /// <summary>
        /// ASCII解码后的帧数据
        /// </summary>
        public byte[] recvData; //ASCII解码后的帧数据
    }

    /// <summary>
    /// DTU发送的注册包
    /// </summary>
    public struct ServiceCode
    {
        public int CountBytes;
        public byte[] recvData;
        public DateTime LastSendDT;
    }

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
