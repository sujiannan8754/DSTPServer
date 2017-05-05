using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DSTP_BLL.ClassToRtu
{
    /// <summary>
    /// RTU连接实体类
    /// </summary>
    public class RTU
    {
        public TcpClient client { get; private set; }
        public BinaryReader br { get; private set; }
        public BinaryWriter bw { get; private set; }
        public int RTU_No { get; set; }

        /// <summary>
        /// 指示是否存储了RTU设备编号
        /// </summary>
        public bool bRTU_No { get; set; }

        /// <summary>
        /// 标识是否第一次接收数据，是的话将第一次接收数据的RTU编号放入list中，否则与list中RTU编号进行验证
        /// </summary>
        public bool bCheck { get; set; }

        /// <summary>
        /// 指示是否发送心跳包
        /// </summary>
        public bool bHeartBeat { get; set; }

        /// <summary>
        /// 最后发送数据的时间
        /// </summary>
        public DateTime LaseSendDT { get; set; }


        /// <summary>
        /// 收到的server端发来的上传图像文件命令的队列
        /// </summary>
        public ClassSafeEventQueue qStringMsgFileImage;

        /// <summary>
        /// 收到的server端发来的ACK应答的队列
        /// </summary>
        public ClassSafeEventQueue qStringMsgACK;

        /// <summary>
        /// 收到的server端发来的上传实时数据命令的队列
        /// </summary>
        public ClassSafeEventQueue qStringMsgRealDatas;

        /// <summary>
        /// RTU当前实体数据
        /// </summary>
        public ClassTableEntity myEntity;

        /// <summary>
        /// 进行发送时使用的互斥体
        /// </summary>
        public Mutex mutForSockTx;

        /// <summary>
        /// RTU启用状态 1正常使用，2停用，3检修
        /// </summary>
        public int RTU_ParaMeter_No { get; set; }

        /// <summary>
        /// 要求RTU触发一次照相
        /// </summary>
        public AutoResetEvent autoEventTriggerCam { get; set; }

        /// <summary>
        /// 设置RTU参数
        /// </summary>
        public AutoResetEvent autoEventTriggerPara { get; set; }

        /// <summary>
        /// RTU连接设备主进程
        /// </summary>
        public Thread MasterThread { get; set; }

        /// <summary>
        /// 用于接收最新接收到的字符串
        /// </summary>
        public StringBuilder strReceived { get; set; }

        /// <summary>
        /// 命令序号
        /// </summary>
        public byte SnOfCmd;

        /// <summary>
        /// RTU数据上传周期
        /// </summary>
        public int UploadCycle { get; set; }

        /// <summary>
        /// 内网操作RTU启用状态时，是否将数据存入数据库（不关闭Socket连接，接收数据，但是不将数据存入数据库中）
        /// </summary>
        public bool bStartReceive { get; set; }

        /// <summary>
        /// 判断是否发送过了时间校对
        /// </summary>
        public bool bSendUpdateTime { get; set; }

        /// <summary>
        /// RTU最后一次接收数据的时间
        /// </summary>
        public DateTime RTUReceivedTime { get; set; }

        /// <summary>
        /// RTU连接是否正常
        /// </summary>
        public bool bRTUConnected { get; set; }

        public RTU(TcpClient client)
        {
            this.client = client;
            NetworkStream networkStream = client.GetStream();
            br = new BinaryReader(networkStream);
            bw = new BinaryWriter(networkStream);

            autoEventTriggerCam = new AutoResetEvent(false);
            autoEventTriggerPara = new AutoResetEvent(false);

            bStartReceive = true;
            bSendUpdateTime = false;
            bRTUConnected = true;

            RTUReceivedTime = DateTime.Now;//先初始化
        }

        public void Close()
        {
            client.Client.Shutdown(SocketShutdown.Both);
            br.Close();
            bw.Close();
            Thread.Sleep(50);
            client.Close();

        }

    }
}
