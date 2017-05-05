using DSTP_DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DSTP_BLL.ClassToLan
{
    /// <summary>
    /// 内网连接实体类
    /// </summary>
    public class Lan
    {
        public TcpClient client { get; set; }
        public BinaryReader br { get; set; }
        public BinaryWriter bw { get; set; }

        /// <summary>
        /// 存储该连接的用户名信息
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 最后发送数据的时间
        /// </summary>
        public DateTime LaseSendDT { get; set; }

        /// <summary>
        /// 建立连接的时间
        /// </summary>
        public DateTime StartConnectDate { get; set; }

        /// <summary>
        /// 指示是否开始发送心跳包
        /// </summary>
        public bool bHeartBeat { get; set; }

        /// <summary>
        /// 连接是否正常
        /// </summary>
        public bool isNormalExit { get; set; }

        /// <summary>
        /// 用于接收最新接收到的字符串
        /// </summary>
        public StringBuilder strReceived;

        /// <summary>
        /// 数据浏览已经发送了的数据总条数
        /// </summary>
        public int ListCountSum { get; set; }

        /// <summary>
        /// APP账号信息读取数据浏览的结果集数据
        /// </summary>
        public List<AccountModel> SaveAccountModel;
        /// <summary>
        /// 工程信息读取数据浏览的结果集数据
        /// </summary>
        public List<ProjectModel> SaveProjectModel;

        /// <summary>
        /// 工程日志读取数据浏览的结果集数据
        /// </summary>
        public List<Project_LogModels> SaveLogModel;

        /// <summary>
        /// RTU数据浏览的结果集数据
        /// </summary>
        public List<RtuDataModel> SaveRtuDataModel;

        /// <summary>
        /// RTU图片数据浏览的结果集数据
        /// </summary>
        public List<RtuPicModel> SaveRtuPicModel;

        public List<Project_pic> SaveProjectPicList;

        /// <summary>
        /// 最后接收数据的时间
        /// </summary>
        public DateTime LanReceivedTime;

        public Lan(TcpClient client)
        {
            SaveAccountModel = new List<AccountModel>();
            SaveProjectModel = new List<ProjectModel>();
            SaveLogModel = new List<Project_LogModels>();
            SaveRtuDataModel = new List<RtuDataModel>();
            SaveRtuPicModel = new List<RtuPicModel>();
            SaveProjectPicList = new List<Project_pic>();

            this.client = client;
            NetworkStream networkStream = client.GetStream();
            br = new BinaryReader(networkStream);
            bw = new BinaryWriter(networkStream);

            isNormalExit = false;

            LanReceivedTime = DateTime.Now;//先对数据进行初始化
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
