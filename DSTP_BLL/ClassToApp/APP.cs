using DSTP_DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DSTP_BLL.ClassToApp
{
    /// <summary>
    /// APP连接实体类
    /// </summary>
    public class APP
    {

        public TcpClient client { get; private set; }
        public BinaryReader br { get; private set; }
        public BinaryWriter bw { get; private set; }

        /// <summary>
        /// 存储该连接的用户名信息
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// 最后发送数据的时间
        /// </summary>
        public DateTime LaseSendDT { get; set; }

        /// <summary>
        /// 连接是否正常
        /// </summary>
        public bool isNormalExit { get; set; }

        /// <summary>
        /// 用于接收最新接收到的字符串
        /// </summary>
        public StringBuilder strReceived;

        public byte[] byteReceived;

        /// <summary>
        /// 接收数据存储要发送的工程信息数据
        /// </summary>
        public List<ProjectModel> SaveProject;
        /// <summary>
        /// 接收数据存储要发送的工程日志数据
        /// </summary>
        public List<Project_LogModels> SaveProject_Log;

        /// <summary>
        /// 指示是否完成了一个流程操作
        /// </summary>
        public bool bFinish { get; set; }

        /// <summary>
        /// 数据浏览已经发送了的数据总条数
        /// </summary>
        public int ListCountSum { get; set; }

        public Mutex AppMutex;

        public SaveTableID SaveID;

        /// <summary>
        /// 数据分表标识
        /// </summary>
        public AutoResetEvent autoEventTriggerData { get; set; }

        public APP(TcpClient client)
        {
            this.client = client;
            NetworkStream networkStream = client.GetStream();
            br = new BinaryReader(networkStream);
            bw = new BinaryWriter(networkStream);

            autoEventTriggerData = new AutoResetEvent(false);

            SaveID = new SaveTableID();

            isNormalExit = false;
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


    public class SaveTableID
    {
        /// <summary>
        /// 工程信息ID
        /// </summary>
        public string Project_ID { get; set; }

        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
    }
}
