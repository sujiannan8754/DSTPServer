using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSTP_BLL.ClassToRtu;
using System.Net.Sockets;
using System.Threading;
using DSTP_DAL;
using System.Net;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DSTP_BLL.ClassToOther;

namespace DSTPServer
{

    //定义写主窗口的debug Text的代理类型
    public delegate void deleWriteDebugBox(string strText, int writeMode);

    /// <summary>
    /// RTU连接管理
    /// </summary>
    public class ConnectToRTU
    {
        #region 字段

        /// <summary>
        /// 保存所有连接的RTU设备
        /// </summary>
        public static List<RTU> RTULlist = new List<RTU>();

        /// <summary>
        /// 服务器IP地址
        /// </summary>;
        private string ServerIP;

        /// <summary>
        /// 监听端口
        /// </summary>
        private string port;
        private TcpListener myListener;


        /// <summary>
        /// 是否正常退出子线程
        /// </summary>
        bool isNormalExit = false;


        /// <summary>
        /// socket连接正常标志
        /// </summary>
        public bool bConnectionOk;

        /// <summary>
        /// DTU注册包
        /// </summary>
        ServiceCode sc;

        /// <summary>
        /// 存储的Dtu密码
        /// </summary>
        string DtuSVRPWD;

        string bDtuSVRPWD;

        /// <summary>
        /// 心跳包开关
        /// </summary>
        bool bHeartBeat;

        /// <summary>
        /// 指示开始是否发送心跳包
        /// </summary>
        bool bStartHeartBeat;

        /// <summary>
        /// 心跳包间隔（秒）
        /// </summary>
        int HeartBeatTime;

        /// <summary>
        /// RTU默认接收数据超时断开连接时间(分钟)
        /// </summary>
        int RTUReceivedTime;


        /// <summary>
        /// 等待图片文件分包的最长超时时间
        /// </summary>
        public int imageFileTimeout;

        RtuDAL RtuDAL = new RtuDAL();
        /// <summary>
        /// RTU数据模型
        /// </summary>
        RtuDataModel rdm;
        /// <summary>
        /// RTU图片模型
        /// </summary>
        RtuPicModel rpm;

        DSTP dstp;

        deleWriteDebugBox writeDebug;

        #endregion


        #region 开始、结束监听
        /// <summary>
        /// 开始监听RTU设备
        /// </summary>
        public void Begin_Listen(string ServerIP, int port, string DtuSVRPWD1, string bDtuSVRPWD1, string bHeartBeat, string bStartHeartBeat, string HeartBeatTime,string RTUReceivedTime, int ImageFileTimeOut1, DSTP dstp)
        {
            try
            {
                this.dstp = dstp;
                this.imageFileTimeout = ImageFileTimeOut1;
                this.DtuSVRPWD = DtuSVRPWD1;
                this.bDtuSVRPWD = bDtuSVRPWD1;

                this.bHeartBeat = Convert.ToBoolean(bHeartBeat);
                this.bStartHeartBeat = Convert.ToBoolean(bStartHeartBeat);
                this.HeartBeatTime = Convert.ToInt32(HeartBeatTime);
                this.RTUReceivedTime = Convert.ToInt32(RTUReceivedTime);

                writeDebug = dstp.writeDebugBox;
                myListener = new TcpListener(IPAddress.Parse(ServerIP), Convert.ToInt32(port));
                myListener.Start();

                //这里窗体输出开始监听

                //创建一个线程开始监听客户端的连接请求
                Thread RtuThread = new Thread(ListenClientConnect);
                RtuThread.IsBackground = true;//设置为后台进程
                RtuThread.Start();

            }
            catch (Exception ex)
            {
                MessageBox.Show("RTU连接失败，检查服务器IP是否正确");

                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " Begin_Listen " + ex.Message.ToString());
            }
        }

        /// <summary>
        /// 停止监听所有RTU设备
        /// </summary>
        public void Stop_Listen()
        {
            try
            {
                isNormalExit = true;
                for (int i = RTULlist.Count - 1; i >= 0; i--)
                {
                    RemoveRTU(RTULlist[i]);
                }
            }
            catch (Exception ex)
            {
                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " Stop_Listen " + ex.Message.ToString());
            }
        }
        #endregion


        #region RTU设备监听线程
        /// <summary>
        /// 接收客户端连接
        /// </summary>
        private void ListenClientConnect()
        {
            TcpClient newClient = null;
            while (true)
            {
                try
                {
                    newClient = myListener.AcceptTcpClient();

                    Thread.Sleep(100);

                    #region 每接收一个客户端连接，就创建三个对应的子线程循环接收该客户端发来的数据、图片,空闲操作

                    RTU rtu = new RTU(newClient);

                    rtu.qStringMsgRealDatas = new ClassSafeEventQueue(100);
                    rtu.qStringMsgACK = new ClassSafeEventQueue(30);
                    rtu.qStringMsgFileImage = new ClassSafeEventQueue(30);

                    rtu.mutForSockTx = new Mutex();

                    rtu.myEntity = new ClassTableEntity();
                    rtu.myEntity.mutAccessMe = new Mutex();


                    rtu.strReceived = new StringBuilder();

                    bool b = false;
                    int Count = 0;
                    for (int i = 0; i < RTULlist.Count; i++) //判断相同IP地址是否进行过连接
                    {
                        string a = rtu.client.Client.RemoteEndPoint.ToString().Split(':')[0];
                        if (rtu.client.Client.RemoteEndPoint.ToString().Split(':')[0] == RTULlist[i].client.Client.RemoteEndPoint.ToString().Split(':')[0])
                        {
                            b = true;
                            Count = i;
                        }
                    }
                    if (b == true)
                    {
                        RTULlist[Count].MasterThread.Abort();
                        bool c = RTULlist[Count].MasterThread.IsAlive;
                        RemoveRTU(RTULlist[Count]);
                    }
                    RTULlist.Add(rtu);

                    rtu.RTU_ParaMeter_No = 1;

                    rtu.MasterThread = new Thread(Child_Thread);
                    rtu.MasterThread.IsBackground = true;
                    rtu.MasterThread.Start(rtu);
                    #endregion

                    #region 输出信息到界面
                    //写信息
                    string str = string.Format("   {0}   连入系统", rtu.client.Client.RemoteEndPoint.ToString());
                    dstp.Invoke(writeDebug, new object[] { str, 0 });

                    //更新RTU列表
                    dstp.Invoke(writeDebug, new object[] { null, 4 });
                    for (int Num = 0; Num < RTULlist.Count; Num++)
                    {
                        if (RTULlist[Num].RTU_No == 0)
                        {
                            string strNo = string.Format("   {0}   ", RTULlist[Num].client.Client.RemoteEndPoint.ToString());
                            dstp.Invoke(writeDebug, new object[] { strNo, 1 });
                        }
                        else
                        {
                            string strNo = string.Format("   {0}   ", RTULlist[Num].RTU_No.ToString());
                            dstp.Invoke(writeDebug, new object[] { strNo, 1 });
                        }
                    }
                    #endregion

                }
                catch (Exception ex)
                {
                    //当单击‘停止监听’或者退出此窗体时 AcceptTcpClient() 会产生异常
                    //因此可以利用此异常退出循环
                    break;
                }

            }
        }

        /// <summary>
        /// 移除RTU设备（同时关闭Socket）
        /// </summary>
        /// <param name="rtu">指定要移除的用户</param>
        private void RemoveRTU(RTU rtu)
        {
            try
            {
                rtu.bRTUConnected = false;
                //在关闭连接之前把数据库该RTU改为关闭状态
                if (rtu.RTU_No != 0)
                {
                    RtuDAL.RtuParaMeter(rtu.RTU_No, 2);
                }
                RTULlist.Remove(rtu);
                rtu.Close();
            }
            catch (Exception ex)
            {
                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " RemoveRTU " + ex.Message.ToString());
            }
        }
        #endregion


        #region RTU子线程
        private void Child_Thread(object userState)
        {
            RTU rtu = (RTU)userState;
            TcpClient client = rtu.client;

            Thread Operation_Thread;
            Operation_Thread = new Thread(thread_receiving_datas);
            Operation_Thread.IsBackground = true;
            Operation_Thread.Start(rtu);

            Operation_Thread = new Thread(thread_free);
            Operation_Thread.IsBackground = true;
            Operation_Thread.Start(rtu);

            Operation_Thread = new Thread(thread_for_image_file);
            Operation_Thread.IsBackground = true;
            Operation_Thread.Start(rtu);
        }

        #endregion


        #region RTU操作线程
        /// <summary>
        /// 接收客户端消息
        /// </summary>
        /// <param name="userState">客户端信息</param>
        private void thread_receiving_datas(object userState)
        {
            RTU rtu = (RTU)userState;
            TcpClient client = rtu.client;

            rtu.bRTU_No = false;

            StateRxOneString oneStringRxState = StateRxOneString.NoSTX;

            bool bStart = false;//标识注册包密码是否开始接收数据
            bool bDTU = true;//第一次接收数据，接收注册包
            rtu.bCheck = true;

            sc = new ServiceCode();
            sc.recvData = new byte[52];
            sc.CountBytes = 0;
            sc.LastSendDT = DateTime.Now;

            rtu.strReceived = new StringBuilder();

            rtu.bHeartBeat = bStartHeartBeat;

            while (true)
            {
                if (rtu.bRTUConnected == false)
                {
                    break;
                }
                Thread.Sleep(100);
                try
                {
                    if (bool.Parse(bDtuSVRPWD) == true)//是否使用注册包密码
                    {
                        while (client.Available > 0)
                        {
                            if (bDTU == true)
                            {
                                if (sc.CountBytes <= 52)
                                {
                                    TimeSpan ts = DateTime.Now - sc.LastSendDT;
                                    if (ts.TotalMilliseconds < 2000) //接收的数据在2000毫秒以内，证明还是注册包
                                    {
                                        byte[] receiveBytes = null;
                                        receiveBytes = rtu.br.ReadBytes(1);
                                        sc.recvData[sc.CountBytes] = receiveBytes[0];
                                        sc.CountBytes++;
                                        sc.LastSendDT = DateTime.Now;
                                        if (sc.CountBytes == 52)
                                        {
                                            bStart = checkSVRPWD();
                                            if (bStart == true)
                                            {
                                                bDTU = false;

                                                string str = string.Format("   DTU密码验证成功");
                                                dstp.Invoke(writeDebug, new object[] { str, 0 });
                                            }
                                            else //DTU密码验证错误，断开连接
                                            {
                                                string str = string.Format("   DTU密码验证失败");
                                                dstp.Invoke(writeDebug, new object[] { str, 0 });

                                                RemoveRTU(rtu);
                                                bDTU = false;
                                            }
                                        }
                                    }
                                    else //接收超时，断开连接
                                    {
                                        RemoveRTU(rtu);
                                    }
                                }
                            }
                            else
                            {
                                //接收到了数据,更新最后收到数据的时间
                                rtu.RTUReceivedTime = DateTime.Now;

                                //数据接收开始时不发送心跳包
                                rtu.bHeartBeat = false;
                                byte[] receiveBytes = null;
                                //从网络流中读出字符串，此方法会自动判断字符串长度前缀
                                int Available = 0;
                                receiveBytes = rtu.br.ReadBytes(client.Available);
                                Available = receiveBytes.Count();
                                for (int i = 0; i < Available; i++)
                                {
                                    receiveOneByte(rtu, ref oneStringRxState, receiveBytes[i]);
                                }
                            }
                        }
                    }
                    else
                    {
                        while (client.Available > 0)
                        {
                            //接收到了数据,更新最后收到数据的时间
                            rtu.RTUReceivedTime = DateTime.Now;

                            //数据接收开始时不发送心跳包
                            rtu.bHeartBeat = false;

                            byte[] receiveBytes = null;
                            //从网络流中读出字符串，此方法会自动判断字符串长度前缀
                            int Available = 0;
                            receiveBytes = rtu.br.ReadBytes(client.Available);
                            Available = receiveBytes.Count();
                            for (int i = 0; i < Available; i++)
                            {
                                receiveOneByte(rtu, ref oneStringRxState, receiveBytes[i]);
                            }
                        }
                    }

                    #region 老版本
                    //while (client.Available > 0)
                    //{
                    //    byte[] receiveBytes = null;
                    //    //从网络流中读出字符串，此方法会自动判断字符串长度前缀
                    //    int Available = 0;
                    //    receiveBytes = rtu.br.ReadBytes(client.Available);
                    //    Available = receiveBytes.Count();
                    //    if (bool.Parse(bDtuSVRPWD) == true)//是否使用注册包密码
                    //    {
                    //        if (Available == 52) //证明该消息帧为DTU注册包
                    //        {
                    //            sc = new ServiceCode();

                    //            sc.recvData = receiveBytes;
                    //            sc.CountBytes = 52;
                    //            //解析注册包验证密码
                    //            bStart = checkSVRPWD();
                    //        }
                    //        else
                    //        {
                    //            if (bStart == true)
                    //            {

                    //                for (int i = 0; i < Available; i++)
                    //                {
                    //                    receiveOneByte(rtu, ref oneStringRxState, receiveBytes[i]);
                    //                }
                    //            }
                    //            else
                    //            {
                    //                Thread.Sleep(1000);
                    //                RemoveRTU(rtu);
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        for (int i = 0; i < Available; i++)
                    //        {
                    //            receiveOneByte(rtu, ref oneStringRxState, receiveBytes[i]);
                    //        }
                    //    }
                    //}
                    #endregion


                }
                catch (Exception ex)
                {
                    if (isNormalExit == false)
                    {
                        //string ErrorMessage = client.Client.RemoteEndPoint.ToString();//错误信息
                        //RemoveRTU(rtu);

                        DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " thread_receiving_datas " + ex.Message.ToString());
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 空闲杂物线程
        /// </summary>
        /// <param name="userState">客户端信息</param>
        private void thread_free(object userState)
        {
            RTU rtu = (RTU)userState;
            TcpClient client = rtu.client;


            while (true)
            {
                if (rtu.bRTUConnected == false)
                {
                    break;
                }
                Thread.Sleep(100);
                try
                {
                    //查看触发照相事件
                    bool bRst = rtu.autoEventTriggerCam.WaitOne(10, false);
                    if (bRst)
                    {
                        byte[] tmpArr;
                        int length;
                        makeBinarySetOptionCmd(rtu, ref rtu.SnOfCmd, Convert.ToUInt16(rtu.RTU_No), out tmpArr, out length); //成命令帧
                        string tmpStr = binaryArrayToString(tmpArr, length); //进行ASCII编码

                        rtu.mutForSockTx.WaitOne();

                        sendStringFrame(rtu, tmpStr); //加帧头帧尾，选择信道发送
                                                      //应答在接收数据线程接收
                        rtu.mutForSockTx.ReleaseMutex();

                        string str = string.Format("   {0}   触发照相", rtu.RTU_No.ToString());
                        dstp.Invoke(writeDebug, new object[] { str, 0 });
                    }

                    //查看设置RTU的上传周期事件
                    bRst = rtu.autoEventTriggerPara.WaitOne(10, false);
                    if (bRst)
                    {
                        byte[] tmpArr;
                        int length;
                        makeBinarySetOptionPara(rtu, ref rtu.SnOfCmd, Convert.ToUInt16(rtu.RTU_No), out tmpArr, out length); //成命令帧
                        string tmpStr = binaryArrayToString(tmpArr, length); //进行ASCII编码

                        rtu.mutForSockTx.WaitOne();
                        sendStringFrame(rtu, tmpStr); //加帧头帧尾，选择信道发送,先不管应答了
                        rtu.mutForSockTx.ReleaseMutex();

                        string str = string.Format("   {0}   设置上传周期", rtu.RTU_No.ToString());
                        dstp.Invoke(writeDebug, new object[] { str, 0 });
                    }

                    //查看内网是否改变了RTU的状态
                    if (rtu.bRTU_No == true)
                    {
                        if (rtu.RTU_ParaMeter_No == 1)
                        {
                            //正常使用,开始接收数据
                            rtu.bStartReceive = true;
                        }
                        if (rtu.RTU_ParaMeter_No == 2 || rtu.RTU_ParaMeter_No == 3)
                        {
                            //内网关闭了RTU,停止接收数据
                            rtu.bStartReceive = false;
                        }

                        //查看是否发送时间校对事件
                        if (rtu.bSendUpdateTime)
                        {
                            //在改变了RTU编号的情况下,首先发送一次时间校对
                            byte[] tmpArr;
                            int length;
                            makeBinarySetUpdateTime(rtu, ref rtu.SnOfCmd, Convert.ToUInt16(rtu.RTU_No), out tmpArr, out length); //成命令帧
                            string tmpStr = binaryArrayToString(tmpArr, length); //进行ASCII编码

                            rtu.mutForSockTx.WaitOne();
                            sendStringFrame(rtu, tmpStr); //加帧头帧尾，选择信道发送,先不管应答了
                            rtu.mutForSockTx.ReleaseMutex();

                            rtu.bSendUpdateTime = false;

                            string str = string.Format("   {0}   时间校对", rtu.RTU_No.ToString());
                            dstp.Invoke(writeDebug, new object[] { str, 0 });
                        }
                    }

                    //发送心跳包，判定是否在传输数据
                    if (bHeartBeat == true)
                    {
                        if (rtu.bHeartBeat == true)
                        {
                            TimeSpan ts = DateTime.Now - rtu.LaseSendDT;
                            if (ts.TotalSeconds > HeartBeatTime)//设定心跳包传输时间间隔为10分钟
                            {
                                rtu.mutForSockTx.WaitOne();
                                SendHeartBeat(rtu);
                                rtu.LaseSendDT = DateTime.Now;

                                string str = string.Format("   {0}   发送心跳包", rtu.RTU_No.ToString());
                                dstp.Invoke(writeDebug, new object[] { str, 0 });
                                rtu.mutForSockTx.ReleaseMutex();
                            }
                        }
                    }

                    //若设置的时间内没有再收到RTU的数据,默认该RTU已关闭,关闭该RTU连接
                    if((DateTime.Now - rtu.RTUReceivedTime).TotalMinutes> RTUReceivedTime)
                    {
                        RemoveRTU(rtu);

                        string str = string.Format("   {0}   连接超时,断开连接", rtu.RTU_No.ToString());
                        dstp.Invoke(writeDebug, new object[] { str, 0 });

                        //更新RTU列表
                        dstp.Invoke(writeDebug, new object[] { null, 4 });
                        for (int Num = 0; Num < RTULlist.Count; Num++)
                        {
                            if (RTULlist[Num].RTU_No == 0)
                            {
                                string strNo = string.Format("   {0}   ", RTULlist[Num].client.Client.RemoteEndPoint.ToString());
                                dstp.Invoke(writeDebug, new object[] { strNo, 1 });
                            }
                            else
                            {
                                string strNo = string.Format("   {0}   ", RTULlist[Num].RTU_No.ToString());
                                dstp.Invoke(writeDebug, new object[] { strNo, 1 });
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " thread_free " + ex.Message.ToString());
                }
            }
        }

        /// <summary>
        /// 发送心跳包
        /// </summary>
        /// <param name="obj"></param>
        private void SendHeartBeat(object obj)
        {
            RTU rtu = (RTU)obj;
            TcpClient client = rtu.client;

            //发送心跳包
            byte[] tmpArr;
            int length;
            makeBinarySetHB(Convert.ToUInt16(rtu.RTU_No), out tmpArr, out length); //成命令帧
            string tmpStr = binaryArrayToString(tmpArr, length); //进行ASCII编码
            rtu.mutForSockTx.WaitOne();
            try
            {
                sendStringFrame(rtu, tmpStr);
            }
            catch (Exception ex)
            {
                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " SendHeartBeat " + ex.Message.ToString());
            }
            rtu.mutForSockTx.ReleaseMutex();
        }


        /// <summary>
        /// 图像文件的接收线程
        /// </summary>
        void thread_for_image_file(object userState)
        {
            RTU rtu = (RTU)userState;
            TcpClient client = rtu.client;

            bool bInReceiving = false;//正在接收一个图片的标志
            int pagesTotal = 0;       //当前图片总分包数
            int countPagesReceived = 0;    //当前接收到的图片分包数量
            DateTime dtLastPageReceived = DateTime.Now; //最后一个分包接收的时间
            long imageMaxSize = 10000000;
            byte[] imageArr = new byte[imageMaxSize]; //图片数据缓冲区  10M字节大小
            int countBytesReceived = 0;  //缓冲区中接收到的图片字节数量   
            while (true)
            {
                #region  while
                Thread.Sleep(100);
                object tmpObj = new object();
                bool bGetMsg = rtu.qStringMsgFileImage.GetFromQ(ref tmpObj);

                if (bInReceiving) //正在接收过程中
                {
                    #region  后续分包接收过程

                    Double dT = (DateTime.Now - dtLastPageReceived).TotalMilliseconds;
                    if (dT > imageFileTimeout) //等待超时
                    {
                        #region  先判断是否超时
                        bInReceiving = false;
                        //imageArr = null;
                        pagesTotal = 0;
                        countPagesReceived = 0;
                        countBytesReceived = 0;
                        #endregion

                        string str = string.Format("   {0}   图片接收超时", rtu.RTU_No.ToString());
                        dstp.Invoke(writeDebug, new object[] { str, 0 });

                        rtu.bHeartBeat = true;
                    }
                    else
                    {
                        #region 正常接收
                        if (bGetMsg)
                        {

                            try
                            {
                                #region   tryAction  
                                dtLastPageReceived = DateTime.Now;
                                MessageReceived myMsg = (MessageReceived)tmpObj;

                                UInt16 u16Tmp;
                                u16Tmp = myMsg.recvData[13];
                                u16Tmp = (ushort)(u16Tmp * 256 + myMsg.recvData[14]);
                                if (u16Tmp == pagesTotal) //分包总数检查正确
                                {
                                    u16Tmp = myMsg.recvData[15];
                                    u16Tmp = (ushort)(u16Tmp * 256 + myMsg.recvData[16]);
                                    if (u16Tmp == countPagesReceived) //分包序号检查正确
                                    {//收下这个分包
                                        #region  收下这个分包

                                        try
                                        {
                                            string str = string.Format("   {0}   继续接收分包{1}", rtu.RTU_No.ToString(), u16Tmp.ToString());
                                            dstp.Invoke(writeDebug, new object[] { str, 0 });

                                            u16Tmp = myMsg.recvData[17];
                                            u16Tmp = (ushort)(u16Tmp * 256 + myMsg.recvData[18]); //分包内的数据长度
                                            Array.Copy(myMsg.recvData, 19, imageArr, countBytesReceived, u16Tmp);
                                            countPagesReceived++;
                                            countBytesReceived += u16Tmp;
                                            #region 应答
                                            byte[] myBuffer;
                                            makeAckBinary(myMsg, out myBuffer);
                                            string strCmd = binaryArrayToString(myBuffer, myBuffer.Length);
                                            rtu.mutForSockTx.WaitOne();
                                            sendStringFrame(rtu, strCmd);

                                            //写信息
                                            string strWrite = string.Format("   {0}   应答分包完成", rtu.RTU_No.ToString(), u16Tmp.ToString());
                                            dstp.Invoke(writeDebug, new object[] { strWrite, 0 });
                                            rtu.mutForSockTx.ReleaseMutex();
                                            #endregion
                                            if (countPagesReceived == pagesTotal) //收完所有分包数据
                                            {
                                                if (rtu.bStartReceive)
                                                {
                                                    //保存，显示
                                                    #region 保存，显示

                                                    byte[] tmpArr = new byte[countBytesReceived];
                                                    Array.Copy(imageArr, tmpArr, countBytesReceived);
                                                    Image myImg = BytesToImg2(tmpArr);
                                                    ClassTableEntity myEntity = new ClassTableEntity();
                                                    myEntity.Year = myMsg.recvData[6] * 256 + myMsg.recvData[7];
                                                    myEntity.Month = myMsg.recvData[8];
                                                    myEntity.Day = myMsg.recvData[9];
                                                    myEntity.hour = myMsg.recvData[10];
                                                    myEntity.minute = myMsg.recvData[11];
                                                    myEntity.second = myMsg.recvData[12];
                                                    string strFileName = rtu.RTU_No.ToString() + "-";
                                                    strFileName += myEntity.Year.ToString("d4");
                                                    strFileName += myEntity.Month.ToString("d2");
                                                    strFileName += myEntity.Day.ToString("d2");
                                                    strFileName += myEntity.hour.ToString("d2");
                                                    strFileName += myEntity.minute.ToString("d2");
                                                    strFileName += myEntity.second.ToString("d2");
                                                    strFileName += ".jpg";

                                                    string RTUAddress = "cap1/RTU-" + rtu.RTU_No.ToString() + "/" + myEntity.Year.ToString("d4") + myEntity.Month.ToString("d2");
                                                    if (Directory.Exists(RTUAddress) == false)//判断RTU图片文件夹是否存在
                                                    {
                                                        Directory.CreateDirectory(@RTUAddress);
                                                    }

                                                    myImg.Save(@RTUAddress + "//" + strFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                                    bInReceiving = false;
                                                    //imageArr = null;
                                                    pagesTotal = 0;
                                                    countPagesReceived = 0;
                                                    countBytesReceived = 0;

                                                    //将图片文件以及地址保存到数据库中(当前未使用互斥锁，以后加上)
                                                    rpm = new RtuPicModel();
                                                    rpm.RTU_Pic_ID = Guid.NewGuid().ToString();
                                                    rpm.Pic_name = strFileName;
                                                    rpm.RTU_No = rtu.RTU_No;
                                                    rpm.RTU_Pic_Address = RTUAddress + "/" + strFileName;
                                                    rpm.Detect_Time = DateTime.Parse(myEntity.Year.ToString("d4") + "-" + myEntity.Month.ToString("d2") + "-" + myEntity.Day.ToString("d2") + " " + myEntity.hour.ToString("d2") + ":" + myEntity.minute.ToString("d2") + ":" + myEntity.second.ToString("d2"));
                                                    rpm.Upload_Time = DateTime.Now;
                                                    rpm.Sign = 1; //RTU新上传过来的图片，固标记为1

                                                    dstp.ConDBM.WriteRTUpicMutex.WaitOne();
                                                    bool b = RtuDAL.AddRtuPic(rpm);
                                                    dstp.ConDBM.WriteRTUpicMutex.ReleaseMutex();
                                                    if (b == true)
                                                    {
                                                        //写信息
                                                        strWrite = string.Format("   {0}   图像存储成功", rtu.RTU_No.ToString());
                                                        dstp.Invoke(writeDebug, new object[] { strWrite, 0 });
                                                    }
                                                    else
                                                    {
                                                        //写信息
                                                        strWrite = string.Format("   {0}   图像存储失败", rtu.RTU_No.ToString());
                                                        dstp.Invoke(writeDebug, new object[] { strWrite, 0 });
                                                    }

                                                    //数据接收完成开始发送心跳包
                                                    rtu.bHeartBeat = true;
                                                    rtu.LaseSendDT = DateTime.Now;
                                                    #endregion
                                                }
                                            }
                                        }
                                        catch (System.Exception ex)
                                        {
                                            bInReceiving = false;
                                            //imageArr = null;
                                            pagesTotal = 0;
                                            countPagesReceived = 0;
                                            countBytesReceived = 0;

                                            DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " thread_for_image_file " + ex.Message.ToString());
                                        }
                                        #endregion
                                    }
                                }
                                #endregion
                            }
                            catch (System.Exception ex)
                            {
                                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " thread_for_image_file " + ex.Message.ToString());
                            }

                        }
                        #endregion
                    }


                    #endregion
                }
                else  //还没有接收到首个分包
                {
                    #region 首个分包接收过程
                    if (bGetMsg)
                    {
                        try
                        {
                            #region tryAciton
                            dtLastPageReceived = DateTime.Now;
                            MessageReceived myMsg = (MessageReceived)tmpObj;

                            UInt16 u16Tmp;
                            u16Tmp = myMsg.recvData[13];
                            u16Tmp = (ushort)(u16Tmp * 256 + myMsg.recvData[14]); //分包总数
                            pagesTotal = u16Tmp;
                            if ((u16Tmp > 1) && (u16Tmp < 5000)) //图像最小也要大于一个分包
                            {
                                u16Tmp = myMsg.recvData[15];
                                u16Tmp = (ushort)(u16Tmp * 256 + myMsg.recvData[16]); //分包序号
                                if (u16Tmp == 0) //分包序号检查正确
                                {
                                    #region  收下这个分包

                                    try
                                    {

                                        u16Tmp = myMsg.recvData[17];
                                        u16Tmp = (ushort)(u16Tmp * 256 + myMsg.recvData[18]); //分包内的数据长度
                                        Array.Copy(myMsg.recvData, 19, imageArr, countBytesReceived, u16Tmp);
                                        countPagesReceived++;
                                        countBytesReceived += u16Tmp;
                                        bInReceiving = true;

                                        //应答
                                        #region 
                                        byte[] myBuffer;
                                        makeAckBinary(myMsg, out myBuffer);
                                        string strCmd = binaryArrayToString(myBuffer, myBuffer.Length);
                                        rtu.mutForSockTx.WaitOne();
                                        sendStringFrame(rtu, strCmd);
                                        rtu.mutForSockTx.ReleaseMutex();

                                        //写信息
                                        string strWrite = string.Format("   {0}   应答首个分包", rtu.RTU_No.ToString());
                                        dstp.Invoke(writeDebug, new object[] { strWrite, 0 });
                                        #endregion

                                    }
                                    catch (System.Exception ex)
                                    {
                                        bInReceiving = false;
                                        //imageArr = null;
                                        pagesTotal = 0;
                                        countPagesReceived = 0;
                                        countBytesReceived = 0;

                                        DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " thread_for_image_file " + ex.Message.ToString());
                                    }
                                    #endregion

                                    string str = string.Format("   {0}   接收首个图片分包", rtu.RTU_No.ToString());
                                    dstp.Invoke(writeDebug, new object[] { str, 0 });
                                }
                            }
                            else
                            {
                                pagesTotal = 0;
                            }
                            #endregion
                        }
                        catch (System.Exception ex)
                        {
                            bInReceiving = false;
                            //imageArr = null;
                            pagesTotal = 0;
                            countPagesReceived = 0;
                            countBytesReceived = 0;

                            DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " thread_for_image_file " + ex.Message.ToString());
                        }

                    }
                    #endregion
                }

                #endregion
            }
        }



        /// <summary>
        /// 发送操作命令给指定RTU
        /// </summary>
        /// <param name="rtu"></param>
        /// <param name="message"></param>
        private void SendToClient(RTU rtu, byte[] message)
        {
            try
            {
                //将字符串写入网络流，此方法会自动附加字符串长度前缀
                rtu.bw.Write(message);
                rtu.bw.Flush();
            }
            catch(Exception ex)
            {
                ////发送失败，验证一下Socket连接
                //if(!IsSocketConnected(rtu,message))
                //{
                //    RemoveRTU(rtu);
                //}
                //编写失败原因，输出至界面并写入日志
            }
        }
        #endregion


        #region 格式转化
        /// <summary>
        /// 成操作命令帧(Ascii编码前)(触发照相)
        /// </summary>
        /// <param name="snCmd">帧序号</param>
        /// /// <param name="cmdBuff">输出字节数组</param>
        /// /// <param name="countBytes">有效字节数量</param>
        void makeBinarySetOptionCmd(RTU rtu, ref byte snCmd, UInt16 deviceId, out byte[] cmdBuff, out int countBytes)
        {
            UInt16 myCount = 0;
            cmdBuff = new byte[100];
            //cmdBuff[0] = 0xF7;
            // string tmpStr = cmdBuff[0].ToString("X"); // 255.ToString("X")--> FF
            //byte[] msg = Encoding.UTF8.GetBytes(strTmp);
            //mainHwnd.myClient.sendDataInBuffer(msg, strTmp.Length);



            cmdBuff[myCount++] = 0; //帧长高字节先设定为0
            cmdBuff[myCount++] = 0; //帧长低字节先设定为0

            cmdBuff[myCount++] = Convert.ToByte(deviceId >> 8);
            cmdBuff[myCount++] = Convert.ToByte(deviceId & 0x00ff);
            cmdBuff[myCount++] = 4; //消息类型为4
            cmdBuff[myCount++] = rtu.SnOfCmd; //消息序号


            if (rtu.SnOfCmd < 255)
            {
                rtu.SnOfCmd++;
            }
            else
            {
                rtu.SnOfCmd = 0;
            }
            cmdBuff[myCount++] = 1; //设置码为1

            cmdBuff[0] = Convert.ToByte((myCount + 1) >> 8);      //帧长高字节
            cmdBuff[1] = Convert.ToByte((myCount + 1) & 0x00ff);  //帧长低字节


            //下面进行异或和校验
            byte tmpB1 = cmdBuff[0];
            for (int i = 1; i < myCount; i++)
            {
                tmpB1 = (byte)(tmpB1 ^ cmdBuff[i]);
            }

            cmdBuff[myCount] = tmpB1;

            countBytes = myCount + 1;
            return;
        }

        /// <summary>
        /// 心跳包成帧(Ascii编码前)
        /// </summary>
        /// <param name="snCmd">帧序号</param>
        /// /// <param name="cmdBuff">输出字节数组</param>
        /// /// <param name="countBytes">有效字节数量</param>
        void makeBinarySetHB(UInt16 deviceId, out byte[] cmdBuff, out int countBytes)
        {
            UInt16 myCount = 0;
            cmdBuff = new byte[100];



            cmdBuff[myCount++] = 0; //帧长高字节先设定为0
            cmdBuff[myCount++] = 0; //帧长低字节先设定为0

            cmdBuff[myCount++] = Convert.ToByte(deviceId >> 8);
            cmdBuff[myCount++] = Convert.ToByte(deviceId & 0x00ff);
            cmdBuff[myCount++] = 100; //消息类型为100

            //无序号，无数据

            //计算帧长
            cmdBuff[0] = Convert.ToByte((myCount + 1) >> 8);      //帧长高字节
            cmdBuff[1] = Convert.ToByte((myCount + 1) & 0x00ff);  //帧长低字节

            //下面进行异或和校验
            byte tmpB1 = cmdBuff[0];
            for (int i = 1; i < myCount; i++)
            {
                tmpB1 = (byte)(tmpB1 ^ cmdBuff[i]);
            }

            cmdBuff[myCount] = tmpB1;

            countBytes = myCount + 1;
            return;



        }

        /// <summary>
        /// 成操作命令帧(Ascii编码前)(设置上传周期)
        /// </summary>
        /// <param name="snCmd">帧序号</param>
        /// /// <param name="cmdBuff">输出字节数组</param>
        /// /// <param name="countBytes">有效字节数量</param>
        void makeBinarySetOptionPara(RTU rtu, ref byte snCmd, UInt16 deviceId, out byte[] cmdBuff, out int countBytes)
        {
            UInt16 myCount = 0;
            cmdBuff = new byte[100];



            cmdBuff[myCount++] = 0; //帧长高字节先设定为0
            cmdBuff[myCount++] = 0; //帧长低字节先设定为0

            cmdBuff[myCount++] = Convert.ToByte(deviceId >> 8);
            cmdBuff[myCount++] = Convert.ToByte(deviceId & 0x00ff);
            cmdBuff[myCount++] = 4; //消息类型为4
            cmdBuff[myCount++] = rtu.SnOfCmd; //消息序号


            if (rtu.SnOfCmd < 255)
            {
                rtu.SnOfCmd++;
            }
            else
            {
                rtu.SnOfCmd = 0;
            }
            cmdBuff[myCount++] = 5; //设置码为5,设置数据采集周期
            cmdBuff[myCount++] = Convert.ToByte(rtu.UploadCycle); //设置周期


            cmdBuff[0] = Convert.ToByte((myCount + 1) >> 8);      //帧长高字节
            cmdBuff[1] = Convert.ToByte((myCount + 1) & 0x00ff);  //帧长低字节


            //下面进行异或和校验
            byte tmpB1 = cmdBuff[0];
            for (int i = 1; i < myCount; i++)
            {
                tmpB1 = (byte)(tmpB1 ^ cmdBuff[i]);
            }

            cmdBuff[myCount] = tmpB1;

            countBytes = myCount + 1;
            return;
        }

        /// <summary>
        /// 成操作命令帧(Ascii编码前)(更新RTU时间)
        /// </summary>
        /// <param name="rtu">RTU</param>
        /// <param name="snCmd">帧序号</param>
        /// <param name="deviceId">RTU编号</param>
        /// <param name="cmdBuff">输出字节数组</param>
        /// <param name="countBytes">有效字节数量</param>
        void makeBinarySetUpdateTime(RTU rtu, ref byte snCmd, UInt16 deviceId, out byte[] cmdBuff, out int countBytes)
        {
            UInt16 myCount = 0;
            cmdBuff = new byte[100];



            cmdBuff[myCount++] = 0; //帧长高字节先设定为0
            cmdBuff[myCount++] = 0; //帧长低字节先设定为0

            cmdBuff[myCount++] = Convert.ToByte(deviceId >> 8);
            cmdBuff[myCount++] = Convert.ToByte(deviceId & 0x00ff);
            cmdBuff[myCount++] = 4; //消息类型为4
            cmdBuff[myCount++] = rtu.SnOfCmd; //消息序号


            if (rtu.SnOfCmd < 255)
            {
                rtu.SnOfCmd++;
            }
            else
            {
                rtu.SnOfCmd = 0;
            }
            cmdBuff[myCount++] = 4; //设置码为4,设置RTU时间

            #region 数据区 七字节，年2字节其他1字节
            DateTime dt = DateTime.Now;
            cmdBuff[myCount++] = Convert.ToByte(dt.Year >> 8);
            cmdBuff[myCount++] = Convert.ToByte(dt.Year & 0x00ff);
            cmdBuff[myCount++] = Convert.ToByte(dt.Month);
            cmdBuff[myCount++] = Convert.ToByte(dt.Day);
            cmdBuff[myCount++] = Convert.ToByte(dt.Hour);
            cmdBuff[myCount++] = Convert.ToByte(dt.Minute);
            cmdBuff[myCount++] = Convert.ToByte(dt.Second);

            cmdBuff[0] = Convert.ToByte((myCount + 1) >> 8);      //帧长高字节
            cmdBuff[1] = Convert.ToByte((myCount + 1) & 0x00ff);  //帧长低字节
            #endregion

            //下面进行异或和校验
            byte tmpB1 = cmdBuff[0];
            for (int i = 1; i < myCount; i++)
            {
                tmpB1 = (byte)(tmpB1 ^ cmdBuff[i]);
            }

            cmdBuff[myCount] = tmpB1;

            countBytes = myCount + 1;
            return;
        }


        /// <summary>
        /// 收到的命令帧进行编码成帧
        /// </summary>
        /// <param name="recvMsg">收到的命令帧结构</param>
        /// <param name="msgArr">存储ASCII编码前的应答帧数组</param>
        public void makeAckBinary(MessageReceived recvMsg, out byte[] msgArr)
        {

            UInt16 myCount = 0;
            byte[] cmdBuff = new byte[100];
            //cmdBuff[0] = 0xF7;
            // string tmpStr = cmdBuff[0].ToString("X"); // 255.ToString("X")--> FF
            //byte[] msg = Encoding.UTF8.GetBytes(strTmp);
            //mainHwnd.myClient.sendDataInBuffer(msg, strTmp.Length);



            cmdBuff[myCount++] = 0; //帧长高字节先设定为0
            cmdBuff[myCount++] = 0; //帧长低字节先设定为0

            cmdBuff[myCount++] = recvMsg.recvData[myCount - 1];  //ID high
            cmdBuff[myCount++] = recvMsg.recvData[myCount - 1];  //ID low
            cmdBuff[myCount++] = 0; //消息类型为ACK应答帧
            cmdBuff[myCount++] = recvMsg.recvData[myCount - 1]; ; //消息序号
            cmdBuff[0] = Convert.ToByte((myCount + 1) >> 8);      //帧长高字节
            cmdBuff[1] = Convert.ToByte((myCount + 1) & 0x00ff);  //帧长低字节

            //下面进行异或和校验
            byte tmpB1 = cmdBuff[0];
            for (int i = 1; i < myCount; i++)
            {
                tmpB1 = (byte)(tmpB1 ^ cmdBuff[i]);
            }

            cmdBuff[myCount] = tmpB1;
            msgArr = new byte[myCount + 1];
            Array.Copy(cmdBuff, msgArr, myCount + 1);

        }

        /// <summary>
        /// 将字节数组转换成16进制的字符串
        /// </summary>
        /// <param name="cmdBuff">字节数组</param>
        /// <param name="countBytes">数组长度</param>
        /// <returns>转换后的字符串</returns>
        string binaryArrayToString(byte[] cmdBuff, int countBytes)
        {
            string tmpStr = null;
            for (int i = 0; i < countBytes; i++)
            {
                tmpStr += cmdBuff[i].ToString("X2");//注意这里的2:如果不使用，则单字节数值1就会转换为1，不会转换为01
            }
            // string tmpStr = cmdBuff[0].ToString("X"); // 255.ToString("X")--> FF
            return tmpStr;
        }


        /// <summary>
        /// 发送格式化的命令帧(加STX和 ETX)
        /// </summary>
        /// <param name="strCMD">ASCII编码后的帧数据</param>
        bool sendStringFrame(RTU rtu, string strCMD)
        {
            bool bRst = false;
            try
            {

                byte[] buffer = new byte[strCMD.Length + 2];
                byte[] msg = Encoding.UTF8.GetBytes(strCMD);
                buffer[0] = 0x02;
                // Array.Copy(msg, buffer, strCMD.Length);
                Array.Copy(msg, 0, buffer, 1, strCMD.Length);
                buffer[strCMD.Length + 1] = 0x03;

                SendToClient(rtu, buffer);
                bRst = true;
            }
            catch (System.Exception ex)
            {
                bRst = false;

                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " sendStringFrame " + ex.Message.ToString());
            }
            return bRst;
        }

        /// <summary>
        /// 这个方法将内存图像以Png格式反序列化，数据量大大的降低，目测效率也是比前一种要好一点，代码更加简单易读。
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public Image BytesToImg2(byte[] bytes)
        {
            try
            {
                return Image.FromStream(new MemoryStream(bytes));
            }
            catch (System.Exception ex)
            {
                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " BytesToImg2 " + ex.Message.ToString());

                return null;
            }

        }


        /// <summary>
        ///  string 转换成 字节数组
        ///  "1213" 转换成 byte[]={0x12,0x13}
        /// </summary>
        /// <param name="hex">带转换的16进制字符串</param>
        /// <returns>转换后的byte数组</returns>
        public static byte[] StringToByteArray(string hex)
        {
            hex = hex.Replace(" ", "");
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// 将字节数组转换成16进制的字符串
        /// </summary>
        /// <param name="bytes">转换前字节数组</param>
        /// <returns></returns>
        public string ByteToString(byte[] bytes)
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (byte bt in bytes)
            {
                strBuilder.AppendFormat("{0:X2}", bt);
            }
            return strBuilder.ToString();
        }
        #endregion


        #region 检查是否完整帧
        /// <summary>
        /// 接收一字节
        /// 收到一个完整帧后进行调用checkMyFrameType()函数进行判帧
        /// </summary>
        /// <param name="myState">当前的命令字符串接收状态</param>
        /// <param name="oneByte">接收到字节</param>
        void receiveOneByte(object userState, ref StateRxOneString myState, byte oneByte)
        {
            RTU rtu = (RTU)userState;
            TcpClient client = rtu.client;
            try
            {
                if (myState == StateRxOneString.NoSTX)//等待STX过程
                {
                    #region

                    if (oneByte != 0x02)
                    {
                        //丢弃
                        return;
                    }
                    else
                    {
                        rtu.strReceived.Length = 0;

                        myState = StateRxOneString.WAIT_ETX;

                        string str = string.Format("   {0}   收到STX", rtu.RTU_No.ToString());
                        dstp.Invoke(writeDebug, new object[] { str, 0 });
                    }
                    #endregion
                }
                else //等待ETX过程
                {
                    #region

                    if (rtu.strReceived.Length > 5000) //限制最大长度
                    {
                        rtu.strReceived.Length = 0;
                        myState = StateRxOneString.NoSTX;

                        string str = string.Format("   {0}   数据超过5000", rtu.RTU_No.ToString());
                        dstp.Invoke(writeDebug, new object[] { str, 0 });
                        return;
                    }
                    if (oneByte == 0x03) //找到ETX
                    {
                        #region
                        if (rtu.strReceived.Length % 2 != 0) //不是偶数字节长度
                        {
                            //错误，丢弃
                            string str = string.Format("   {0}   不是偶数字节:{1}", rtu.RTU_No.ToString(), rtu.strReceived.Length);
                            dstp.Invoke(writeDebug, new object[] { str, 0 });

                            rtu.strReceived.Length = 0;

                            myState = StateRxOneString.NoSTX;

                            rtu.bHeartBeat = true;
                            return;
                        }
                        else
                        {
                            string str = string.Format("   {0}   接收到ETX,数据长度：{1}", rtu.RTU_No.ToString(), rtu.strReceived.Length);
                            dstp.Invoke(writeDebug, new object[] { str, 0 });

                            checkMyFrameType(rtu, rtu.strReceived.ToString());
                            myState = StateRxOneString.NoSTX;
                        }
                        #endregion
                    }
                    else //STX和ETX中间的字符的接收
                    {
                        #region

                        byte[] tmpArr = new byte[1];
                        tmpArr[0] = oneByte;
                        rtu.strReceived.Append(System.Text.Encoding.ASCII.GetString(tmpArr));
                        #endregion
                    }
                    #endregion

                }
            }
            catch (System.Exception ex)
            {
                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " receiveOneByte " + ex.Message.ToString());
            }
        }

        /// <summary>
        /// 检查注册包密码
        /// </summary>
        /// <param name="bytes">接收的注册包</param>
        /// <returns></returns>
        private bool checkSVRPWD()
        {
            bool b = false;
            byte[] SVRPWD = new byte[9];
            int n = 32;//标识注册包密码位开始位
            for (int i = 0; i < 9; i++)
            {
                SVRPWD[i] = sc.recvData[n];
                n++;
            }
            string strRx = System.Text.Encoding.ASCII.GetString(SVRPWD).Replace("\0", "");
            if (strRx == DtuSVRPWD)
            {
                b = true;
            }
            return b;
        }
        #endregion


        #region 判断消息命令
        /// <summary>
        /// 检查收到的帧结构
        /// 然后进行命令分发
        /// </summary>
        /// <param name="strRx">ASCII编码后的数据帧</param>
        void checkMyFrameType(RTU rtu, string strRx)
        {
            //指示为第一次接收RTU数据时,回应完成后发送时间校对
            bool bFirstSendData=false;
            UInt16 u16Tmp;
            try
            {
                byte[] tmpArr = StringToByteArray(strRx);
                if (tmpArr.Length > 6) //达到最小帧长
                {
                    #region 先检查数据长度
                    u16Tmp = tmpArr[0];
                    u16Tmp = (UInt16)((short)u16Tmp * 256 + tmpArr[1]);
                    if (u16Tmp != tmpArr.Length)
                    {
                        return;
                    }
                    #endregion
                    #region 再检查设备ID
                    u16Tmp = tmpArr[2];
                    u16Tmp = (UInt16)(u16Tmp * 256 + tmpArr[3]);
                    if (rtu.RTU_No == 0)
                    {
                        rtu.RTU_No = u16Tmp;

                        bFirstSendData = true;

                        //将数据库该RTU参数设置为启用状态
                        bool bRtuParaMeter = false;
                        try
                        {
                            bRtuParaMeter = RtuDAL.RtuParaMeter(rtu.RTU_No, 1);
                            if (bRtuParaMeter)
                            {
                                //更新RTU列表
                                dstp.Invoke(writeDebug, new object[] { null, 4 });
                                for (int Num = 0; Num < RTULlist.Count; Num++)
                                {
                                    if (RTULlist[Num].RTU_No == 0)
                                    {
                                        string strNo = string.Format("   {0}   ", RTULlist[Num].client.Client.RemoteEndPoint.ToString());
                                        dstp.Invoke(writeDebug, new object[] { strNo, 1 });
                                    }
                                    else
                                    {
                                        string strNo = string.Format("   {0}   ", RTULlist[Num].RTU_No.ToString());
                                        dstp.Invoke(writeDebug, new object[] { strNo, 1 });
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " checkMyFrameType " + ex.Message.ToString());
                        }
                        rtu.bCheck = false;
                        rtu.bRTU_No = true;
                    }
                    else
                    {
                        if (u16Tmp != rtu.RTU_No)
                        {
                            return;
                        }
                    }
                    #endregion
                    #region 再检查异或校验和

                    byte tmp = tmpArr[0];
                    for (int i = 1; i < tmpArr.Length - 1; i++)
                    {
                        tmp ^= tmpArr[i];
                    }
                    if (tmp != tmpArr[tmpArr.Length - 1])
                    {
                        return;
                    }
                    #endregion
                    #region  下面进行消息分发

                    //经过多重检查，没有错误，则是一个合格的帧，下面看一下是什么类型的
                    if (tmpArr[4] == 0) //应答帧
                    {//发到应答帧队列
                        ;
                        MessageReceived myMsg = new MessageReceived();
                        myMsg.CountBytes = tmpArr.Length;
                        myMsg.recvData = new byte[myMsg.CountBytes];
                        Array.Copy(tmpArr, myMsg.recvData, myMsg.CountBytes);
                        rtu.qStringMsgACK.PutToQ((object)myMsg); //放入消息队列
                    }

                    if (tmpArr[4] == 1) //RTU到中心的历史数据上传命令
                    {
                        //发到命令帧队列
                        string str = string.Format("   {0}   收到RTU数据", rtu.RTU_No.ToString());
                        dstp.Invoke(writeDebug, new object[] { str, 0 });


                        MessageReceived myMsg = new MessageReceived();
                        myMsg.CountBytes = tmpArr.Length;
                        myMsg.recvData = new byte[myMsg.CountBytes];
                        Array.Copy(tmpArr, myMsg.recvData, myMsg.CountBytes);
                        rtu.qStringMsgRealDatas.PutToQ((object)myMsg); //放入消息队列
                        Thread.Sleep(50);

                        if (rtu.bStartReceive)
                        {
                            processCmdRealDatas(rtu);
                        }

                        //应答
                        byte[] myBuffer;
                        makeAckBinary(myMsg, out myBuffer);
                        string strCmd = binaryArrayToString(myBuffer, myBuffer.Length);
                        rtu.mutForSockTx.WaitOne();
                        bool bRstTx = sendStringFrame(rtu, strCmd);
                        if (!bRstTx)
                        {
                            bConnectionOk = false;
                        }

                        rtu.bHeartBeat = true;
                        rtu.LaseSendDT = DateTime.Now;

                        if (bFirstSendData == true)
                        {
                            rtu.bSendUpdateTime = true;
                            bFirstSendData = false;
                        }

                        rtu.mutForSockTx.ReleaseMutex();
                    }
                    if (tmpArr[4] == 3) //RTU到中心的告警图片上传命令
                    {//发到命令帧队列
                        ;
                        MessageReceived myMsg = new MessageReceived();
                        myMsg.CountBytes = tmpArr.Length;
                        myMsg.recvData = new byte[myMsg.CountBytes];
                        Array.Copy(tmpArr, myMsg.recvData, myMsg.CountBytes);
                        rtu.qStringMsgFileImage.PutToQ((object)myMsg); //放入消息队列

                        string str = string.Format("   {0}   收到RTU图片", rtu.RTU_No.ToString());
                        dstp.Invoke(writeDebug, new object[] { str, 0 });
                    }
                    if (tmpArr[4] == 100) //心跳包
                    {
                        //无动作
                        ;
                    }
                    #endregion

                }
            }
            catch (System.Exception ex)
            {
                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " checkMyFrameType " + ex.Message.ToString());
            }

        }
        #endregion


        #region 数据保存
        /// <summary>
        /// 处理一条历史数据上传命令
        /// </summary>
        void processCmdRealDatas(RTU rtu)
        {
            object tmpObj = new object();
            UInt16 u16Tmp = 0;
            UInt64 u64Tmp = 0;
            bool bEntityOk = false;

            bool bGetMsg = rtu.qStringMsgRealDatas.GetFromQ(ref tmpObj);
            if (bGetMsg)
            {
                try
                {
                    #region 获取数据实体
                    MessageReceived myMsg = (MessageReceived)tmpObj;
                    rtu.myEntity.mutAccessMe.WaitOne();
                    rtu.myEntity.Year = myMsg.recvData[6] * 256 + myMsg.recvData[7];
                    rtu.myEntity.Month = myMsg.recvData[8];
                    rtu.myEntity.Day = myMsg.recvData[9];
                    rtu.myEntity.hour = myMsg.recvData[10];
                    rtu.myEntity.minute = myMsg.recvData[11];
                    rtu.myEntity.second = myMsg.recvData[12];
                    //数据库中的采集日期的存储格式 2016-12-22T16:43:16
                    rtu.myEntity.CaptureTime = rtu.myEntity.Year.ToString("D4") + "-" + rtu.myEntity.Month.ToString("D2") + "-";
                    rtu.myEntity.CaptureTime += rtu.myEntity.Day.ToString("D2") + " ";
                    rtu.myEntity.CaptureTime += rtu.myEntity.hour.ToString("D2") + ":" + rtu.myEntity.minute.ToString("D2") + ":";
                    rtu.myEntity.CaptureTime += rtu.myEntity.second.ToString("D2");

                    u16Tmp = myMsg.recvData[15];
                    u16Tmp = (ushort)(u16Tmp * 256 + myMsg.recvData[16]);
                    rtu.myEntity.Alarm = u16Tmp;
                    u16Tmp = myMsg.recvData[19];
                    u16Tmp = (ushort)(u16Tmp * 256 + myMsg.recvData[20]);
                    rtu.myEntity.Pressure = (int)u16Tmp;

                    u64Tmp = (UInt64)((myMsg.recvData[23] << 24) | (myMsg.recvData[24] << 16) | (myMsg.recvData[25] << 8) | myMsg.recvData[26]);
                    rtu.myEntity.FlowCurrent = (Int64)u64Tmp;

                    u64Tmp = (UInt64)((myMsg.recvData[29] << 24) | (myMsg.recvData[30] << 16) | (myMsg.recvData[31] << 8) | myMsg.recvData[32]);
                    rtu.myEntity.FlowTotal = (Int64)u64Tmp;
                    u64Tmp = (UInt64)((myMsg.recvData[35] << 24) | (myMsg.recvData[36] << 16) | (myMsg.recvData[37] << 8) | myMsg.recvData[38]);
                    rtu.myEntity.ConductanceRatio = (Int64)u64Tmp;

                    rtu.myEntity.mutAccessMe.ReleaseMutex();
                    bEntityOk = true;
                    #endregion
                }
                catch (System.Exception ex)
                {
                    DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " processCmdRealDatas " + ex.Message.ToString());
                }
                if (bEntityOk) //实体获取成功，应该更新数据库
                {
                    //将获取的值转化到Model中
                    rdm = new RtuDataModel();
                    rdm.RTU_Data_ID = Guid.NewGuid().ToString();
                    rdm.RTU_No = rtu.RTU_No;
                    rdm.Displacement = (float)((rtu.myEntity.FlowCurrent) * 0.01);
                    rdm.Displacement_Acc = (float)((rtu.myEntity.FlowTotal) * 0.01);
                    rdm.Pressure = (float)((rtu.myEntity.Pressure) * 0.01);
                    rdm.ConductanceRatio = rtu.myEntity.ConductanceRatio;
                    rdm.TemperatureInBox = rtu.myEntity.TemperatureInBox;
                    rdm.TemperatureOutside = rtu.myEntity.TemperatureOutside;
                    rdm.Detect_Time = DateTime.Parse(rtu.myEntity.CaptureTime);
                    rdm.Upload_Time = DateTime.Now;
                    rdm.Sign = 1; //第一次上传新数据所以标记为1
                    //更新数据库操作,使用互斥锁（当前没使用互斥锁，以后加上）

                    dstp.ConDBM.WriteRTUDataMutex.WaitOne();
                    try
                    {
                        bool b = RtuDAL.AddRtuDate(rdm);
                        if (b == true)
                        {
                            //写信息
                            string str = string.Format("   {0}   数据存储成功", rtu.RTU_No.ToString());
                            dstp.Invoke(writeDebug, new object[] { str, 0 });
                        }
                        else
                        {
                            //写信息
                            string str = string.Format("   {0}   数据存储失败", rtu.RTU_No.ToString());
                            dstp.Invoke(writeDebug, new object[] { str, 0 });
                        }
                    }
                    catch (Exception ex)
                    {
                        DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " processCmdRealDatas " + ex.Message.ToString());
                    }
                    dstp.ConDBM.WriteRTUDataMutex.ReleaseMutex();
                }




            }
        }
        #endregion


        #region 测试连接是否正常
        // 检查一个Socket是否可连接(此方法当前没使用,当RTU重连后,内网会根据IP地址先把原来的RTU连接断开)
        private bool IsSocketConnected(RTU rtu, byte[] message)
        {
            bool RETURN = true;

            try
            {
                if (!((rtu.client.Client.Poll(1000, SelectMode.SelectRead) && (rtu.client.Client.Available == 0)) || !rtu.client.Client.Connected))
                {
                    RETURN = true;
                }
                else
                {
                    RETURN = false;
                }
            }
            catch (Exception ex)
            {
                RETURN = false;
            }
            if (RETURN == true)
            {
                bool blockingState = rtu.client.Client.Blocking;
                try
                {
                    rtu.client.Client.Blocking = false;
                    rtu.client.Client.Send(message, 0, 0);
                    RETURN = true;
                }
                catch (SocketException e)
                {
                    // 产生 10035 == WSAEWOULDBLOCK 错误，说明被阻止了，但是还是连接的
                    // 产生 10053 == WSAEWOULDBLOCK 错误，说明由于超时或者其它失败而中止连接
                    // 产生 10054 == WSAEWOULDBLOCK 错误，远程主机强迫关闭了Socket连接
                    if (e.NativeErrorCode.Equals(10035) || e.NativeErrorCode.Equals(10053) || e.NativeErrorCode.Equals(10054))
                    {
                        RETURN = false;
                    }

                    //DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " IsSocketConnected " + e.Message.ToString());
                }
                finally
                {
                    rtu.client.Client.Blocking = blockingState;    // 恢复状态
                }
            }

            return RETURN;
        }


        #endregion

        #region 更新RTU列表
        public void UpdateRTUListBox()
        {
            //更新RTU列表
            dstp.Invoke(writeDebug, new object[] { null, 4 });
            for (int Num = 0; Num < RTULlist.Count; Num++)
            {
                if (RTULlist[Num].RTU_No == 0)
                {
                    string strNo = string.Format("   {0}   ", RTULlist[Num].client.Client.RemoteEndPoint.ToString());
                    dstp.Invoke(writeDebug, new object[] { strNo, 1 });
                }
                else
                {
                    string strNo = string.Format("   {0}   ", RTULlist[Num].RTU_No.ToString());
                    dstp.Invoke(writeDebug, new object[] { strNo, 1 });
                }
            }
        }
        #endregion
    }
}
