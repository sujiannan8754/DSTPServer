using DSTP_BLL.ClassToLan;
using DSTP_BLL.ClassToOther;
using DSTP_BLL.ClassToRtu;
using DSTP_DAL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DSTPServer
{
    //定义写主窗口的debug Text的代理类型
    public delegate void deleWriteDebugBox2(string strText, int writeMode);

    /// <summary>
    /// 内网连接管理
    /// </summary>
    public class ConnectToLan
    {

        #region 字段
        /// <summary>
        /// 保存内网连接，之所以存成List格式，是为了重新连接之后，断开之前的连接（与内网时1对1的长连接形式）
        /// </summary>
        private List<Lan> LanList = new List<Lan>();

        private TcpListener myListener;

        DSTP dstp;

        LanDAL LanDAL = new LanDAL();

        deleWriteDebugBox2 writeDebug;

        /// <summary>
        /// 标识是否开启事务结束修改Sign字段为已上传
        /// </summary>
        public bool bSelDataEnd;


        /// <summary>
        /// 指示是否开启心跳包发送
        /// </summary>
        bool bHeartBeat;
        /// <summary>
        /// 心跳包周期（秒）
        /// </summary>
        int HeartBeatTime;

        /// <summary>
        /// 内网超时时间
        /// </summary>
        int LanReceivedTime;

        public object RTULlist { get; private set; }

        #endregion

        #region 开始、结束监听客户端
        /// <summary>
        /// 开始监听内网
        /// </summary>
        public void Begin_Listen(string bHeartBeat, string HeartBeatTime,string LanReceivedTime, string ServerIP, int port, DSTP dstp)
        {
            try
            {
                this.dstp = dstp;
                writeDebug = dstp.writeDebugBox;

                this.bHeartBeat = Convert.ToBoolean(bHeartBeat);
                this.HeartBeatTime = Convert.ToInt32(HeartBeatTime);
                this.LanReceivedTime = Convert.ToInt32(LanReceivedTime);

                myListener = new TcpListener(IPAddress.Parse(ServerIP), Convert.ToInt32(port));
                myListener.Start();

                //创建一个线程开始监听客户端的连接请求
                Thread LanThread = new Thread(ListenClientConnect);
                LanThread.IsBackground = true;//设置为后台线程
                LanThread.Start();

                bSelDataEnd = true;
            }
            catch (Exception ex)
            {
                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " Begin_Listen " + ex.Message.ToString());
                MessageBox.Show("内网连接失败，检查服务器IP是否正确");
            }
        }

        /// <summary>
        /// 停止监听内网连接
        /// </summary>
        public void Stop_Listen()
        {
            try
            {
                for (int i = LanList.Count - 1; i >= 0; i--)
                {
                    LanList[i].isNormalExit = true;
                    RemoveLan(LanList[i]);
                }
                myListener.Stop();
            }
            catch (Exception ex)
            {
                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " Stop_Listen " + ex.Message.ToString());
            }
        }

        /// <summary>
        /// 移除内网连接（同时关闭Socket）
        /// </summary>
        /// <param name="rtu">指定要移除的用户</param>
        private void RemoveLan(Lan Lan)
        {
            try
            {
                Lan.isNormalExit = true;
                LanList.Remove(Lan);
                Lan.Close();
            }
            catch (Exception ex)
            {
                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " RemoveLan " + ex.Message.ToString());
            }
        }
        #endregion

        #region 内网监听线程操作
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

                    //接收到连接后，建立连接，如果之前的连接没有断开，先断开之前的连接，保证LanList中只存在一个连接
                    Lan Lan = new Lan(newClient);
                    Lan.strReceived = new StringBuilder();
                    Lan.LaseSendDT = DateTime.Now;

                    Lan.StartConnectDate = DateTime.Now;
                    Lan.UserName = "";

                    bool b = false;
                    int Count = 0;
                    for (int i = 0; i < LanList.Count; i++) //判断之前的连接是否断开了，没断开则先断开之前的连接
                    {
                        string a = Lan.client.Client.RemoteEndPoint.ToString().Split(':')[0];
                        if (Lan.client.Client.RemoteEndPoint.ToString().Split(':')[0] == LanList[i].client.Client.RemoteEndPoint.ToString().Split(':')[0])
                        {
                            b = true;
                            Count = i;
                        }
                    }
                    if (b == true)
                    {
                        RemoveLan(LanList[Count]);
                    }
                    LanList.Add(Lan);

                    //创建子线程开始准备接收数据等操作
                    Thread threadReceive = new Thread(Child_Thread);
                    threadReceive.IsBackground = true;
                    threadReceive.Start(Lan);

                    string str = string.Format("    {0}    连入系统", Lan.client.Client.RemoteEndPoint.ToString());
                    dstp.Invoke(writeDebug, new object[] { str, 3 });
                }
                catch
                {
                    //当单击‘停止监听’或者退出此窗体时 AcceptTcpClient() 会产生异常
                    //因此可以利用此异常退出循环
                    break;
                }
            }
        }
        #endregion

        #region 内网子线程(建立接收数据，空闲线程)
        private void Child_Thread(object userState)
        {
            Lan Lan = (Lan)userState;
            TcpClient client = Lan.client;

            Thread Operation_Thread;
            Operation_Thread = new Thread(thread_receiving_datas); //接收数据线程
            Operation_Thread.IsBackground = true;
            Operation_Thread.Start(Lan);

            Operation_Thread = new Thread(thread_free); //空闲线程
            Operation_Thread.IsBackground = true;
            Operation_Thread.Start(Lan);

            Lan.bHeartBeat = true;
        }
        #endregion

        #region 接收数据，空闲操作
        /// <summary>
        /// 接收客户端消息
        /// </summary>
        /// <param name="userState">客户端信息</param>
        private void thread_receiving_datas(object userState)
        {
            Lan Lan = (Lan)userState;
            TcpClient client = Lan.client;

            StateRxOneString oneStringRxState = StateRxOneString.NoSTX;

            while (true)
            {
                Thread.Sleep(200);
                if (Lan.isNormalExit == true)
                {
                    break;
                }
                try
                {
                    while (client.Available > 0)
                    {
                        Lan.LanReceivedTime = DateTime.Now; //接收到了数据,对最后接收数据时间进行更新
                        Lan.LaseSendDT = DateTime.Now;
                        Lan.bHeartBeat = false;
                        byte[] receiveBytes = null;
                        int Available = 0;
                        receiveBytes = Lan.br.ReadBytes(client.Available);
                        Available = receiveBytes.Count();
                        //首先判断帧的完整性
                        for (int i = 0; i < Available; i++)
                        {
                            receiveOneByte(Lan, ref oneStringRxState, receiveBytes[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (Lan.isNormalExit == false)
                    {
                        //string ErrorMessage = client.Client.RemoteEndPoint.ToString();//错误信息(此处有错)
                        Lan.isNormalExit = true;
                    }

                    DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " thread_receiving_datas " + ex.Message.ToString());

                    break;
                }
            }
        }

        /// <summary>
        /// 空闲线程,主要发送心跳包，验证与内网的连通性
        /// </summary>
        void thread_free(object userState)
        {
            Lan Lan = (Lan)userState;
            TcpClient client = Lan.client;
            while (true)
            {
                Thread.Sleep(200);
                if (Lan.isNormalExit == true)
                {
                    break;
                }
                //发送心跳包，判定是否在传输数据
                if (bHeartBeat == true)
                {
                    if (Lan.bHeartBeat == true)
                    {
                        TimeSpan ts = DateTime.Now - Lan.LaseSendDT;
                        if (ts.TotalSeconds > HeartBeatTime)//设定心跳包传输时间间隔为HeartBeatTime(Config中读取)秒
                        {
                            try
                            {
                                HeartbBeatData HBD = new HeartbBeatData();
                                HBD.Command_Type = 100;
                                string Res = Res = JsonHelper.SerializeObject(HBD);
                                sendStringFrame(Lan, Res);//发送心跳包
                                Lan.LaseSendDT = DateTime.Now;

                                string str = string.Format("   {0}   发送心跳包", Lan.client.Client.RemoteEndPoint.ToString());
                                dstp.Invoke(writeDebug, new object[] { str, 3 });
                            }
                            catch (Exception ex)
                            {
                                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " thread_free " + ex.Message.ToString());
                            }
                        }
                    }
                }
                //建立完连接如果5分钟之内不登录，直接断开连接
                if (Lan.UserName == "")
                {
                    if ((DateTime.Now - Lan.StartConnectDate).TotalMinutes >= 5)
                    {
                        string str = string.Format("   {0}   没有登录,断开连接", Lan.client.Client.RemoteEndPoint.ToString());
                        dstp.Invoke(writeDebug, new object[] { str, 3 });

                        RemoveLan(Lan);
                        break;
                    }
                }
                //检测超时连接，内网断开连接指定时间之前没有再发送数据默认该连接已断开,断开该连接
                if((DateTime.Now - Lan.LanReceivedTime).TotalMinutes > LanReceivedTime)
                {
                    string str = string.Format("   {0}   连接超时,断开连接", Lan.client.Client.RemoteEndPoint.ToString());
                    dstp.Invoke(writeDebug, new object[] { str, 3 });

                    //已经超过了超时时间没有接收到数据了,默认内网已断开连接
                    RemoveLan(Lan);
                }
            }
        }
        #endregion

        #region 判断帧完整性
        /// <summary>
        /// 接收一字节
        /// 收到一个完整帧后进行调用checkMyFrameType()函数进行判帧
        /// </summary>
        /// <param name="myState">当前的命令字符串接收状态</param>
        /// <param name="oneByte">接收到字节</param>
        void receiveOneByte(object userState, ref StateRxOneString myState, byte oneByte)
        {
            Lan Lan = (Lan)userState;
            TcpClient client = Lan.client;
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
                        Lan.strReceived.Length = 0;

                        myState = StateRxOneString.WAIT_ETX;
                    }
                    #endregion
                }
                else //等待ETX过程
                {
                    #region
                    if (oneByte == 0x03) //找到ETX
                    {
                        #region 接收数据完毕，并验证为完整帧，开始判断帧类型
                        checkMyFrameType(Lan, Lan.strReceived.ToString());
                        Lan.strReceived.Length = 0;
                        myState = StateRxOneString.NoSTX;
                        #endregion
                    }
                    else //STX和ETX中间的字符的接收
                    {
                        #region

                        byte[] tmpArr = new byte[1];
                        tmpArr[0] = oneByte;
                        Lan.strReceived.Append(System.Text.Encoding.ASCII.GetString(tmpArr));
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
        #endregion

        #region 判断消息命令,命令分发，完成操作
        /// <summary>
        /// 检查收到的帧结构
        /// 然后进行命令分发
        /// </summary>
        /// <param name="strRx">ASCII编码后的数据帧</param>
        void checkMyFrameType(Lan Lan, string strRx)
        {
            int Error = 0;
            string Error_Message = "";
            string Res = "";
            LanModel Command = new LanModel();
            bool re = false;
            try
            {
                //将接收到的数据转化为JSON格式,接收消息类型与命令序号
                Command = JsonHelper.DeserializeJsonToObject<LanModel>(strRx);

                #region  下面进行消息分发
                switch (Command.Command_Type)
                {
                    case 0:
                        #region 登录验证
                        //接收JSON命令
                        Login JsonToLogin = JsonHelper.DeserializeJsonToObject<Login>(strRx);

                        LanDAL.SelectLogin(JsonToLogin.Data1, JsonToLogin.Data2, out Error, out Error_Message);

                        //回应消息
                        Res1 ResLogin = new Res1();
                        ResLogin.Command_Type = Command.Command_Type;
                        ResLogin.Command_SN = Command.Command_SN;
                        ResLogin.Error = Error;
                        ResLogin.Error_Message = Error_Message;
                        Res = JsonHelper.SerializeObject(ResLogin);

                        re = sendStringFrame(Lan, Res);

                        if (Error == 0)
                        {
                            //登录成功了,将登录的用户名存起来
                            Lan.UserName = JsonToLogin.Data1;
                        }
                        JsonToLogin = null;
                        ResLogin = null;
                        #endregion
                        break;
                    case 1:
                        #region APP账号信息读取
                        ReadAppAccount JsonToReadAccount = JsonHelper.DeserializeJsonToObject<ReadAppAccount>(strRx);

                        LanDAL.SelectAccount(JsonToReadAccount.Data_Tag, out Error, out Error_Message, out Lan.SaveAccountModel);

                        //应答
                        if (Error == 0)
                        {
                            Res2 ResReadAccount = new Res2();
                            ResReadAccount.Command_Type = Command.Command_Type;
                            ResReadAccount.Command_SN = Command.Command_SN;
                            ResReadAccount.ResultSet_Name = "Account";
                            ResReadAccount.Rows_Num = Lan.SaveAccountModel.Count;
                            ResReadAccount.Error = Error;
                            Res = JsonHelper.SerializeObject(ResReadAccount);
                            ResReadAccount = null;
                        }
                        else
                        {
                            Res3 FailResReadAccount = new Res3();
                            FailResReadAccount.Command_Type = Command.Command_Type;
                            FailResReadAccount.Command_SN = Command.Command_SN;
                            FailResReadAccount.Error = Error;
                            Res = JsonHelper.SerializeObject(FailResReadAccount);
                            FailResReadAccount = null;
                        }
                        re = sendStringFrame(Lan, Res);

                        JsonToReadAccount = null;
                        #endregion
                        break;
                    case 3:
                        #region 工程信息读取
                        ReadProject JsonToReadProject = JsonHelper.DeserializeJsonToObject<ReadProject>(strRx);
                        if (JsonToReadProject.Start_Time == "")
                        {
                            JsonToReadProject.Start_Time = null;
                        }
                        if (JsonToReadProject.End_Time == "")
                        {
                            JsonToReadProject.End_Time = null;
                        }

                        dstp.ConDBM.WriteProjectMutex.WaitOne();
                        LanDAL.SelectProject(JsonToReadProject.RTU_No, JsonToReadProject.Construction_Well_ID, Convert.ToDateTime(JsonToReadProject.Start_Time), Convert.ToDateTime(JsonToReadProject.End_Time), JsonToReadProject.Data_Tag, out Error, out Error_Message, out Lan.SaveProjectModel);
                        dstp.ConDBM.WriteProjectMutex.ReleaseMutex();

                        //应答
                        if (Error == 0)
                        {
                            //先将查询到的图片存到数组中,并且给数组赋值
                            for (int i = 0; i < Lan.SaveProjectModel.Count; i++)
                            {
                                if (Lan.SaveProjectModel[i].Team_Leader_Picture != null && Lan.SaveProjectModel[i].Team_Leader_Picture != "")
                                {
                                    Project_pic ProjectPic = new Project_pic();
                                    ProjectPic.Project_ID = Lan.SaveProjectModel[i].Project_ID;
                                    ProjectPic.RTU_No = Lan.SaveProjectModel[i].RTU_No;
                                    ProjectPic.Start_Date = Lan.SaveProjectModel[i].Start_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    ProjectPic.Project_Pic_Name = "Team_Leader_Picture";
                                    ProjectPic.Project_Pic_Content = PicToStr(Lan.SaveProjectModel[i].Team_Leader_Picture);
                                    Lan.SaveProjectPicList.Add(ProjectPic);
                                    ProjectPic = null;
                                }
                                if (Lan.SaveProjectModel[i].Team_Worker_Picture != null && Lan.SaveProjectModel[i].Team_Worker_Picture != "")
                                {
                                    Project_pic ProjectPic = new Project_pic();
                                    ProjectPic.Project_ID = Lan.SaveProjectModel[i].Project_ID;
                                    ProjectPic.RTU_No = Lan.SaveProjectModel[i].RTU_No;
                                    ProjectPic.Start_Date = Lan.SaveProjectModel[i].Start_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    ProjectPic.Project_Pic_Name = "Team_Worker_Picture";
                                    ProjectPic.Project_Pic_Content = PicToStr(Lan.SaveProjectModel[i].Team_Worker_Picture);
                                    Lan.SaveProjectPicList.Add(ProjectPic);
                                    ProjectPic = null;
                                }
                                if (Lan.SaveProjectModel[i].Con_Begin_Pic1 != null && Lan.SaveProjectModel[i].Con_Begin_Pic1 != "")
                                {
                                    Project_pic ProjectPic = new Project_pic();
                                    ProjectPic.Project_ID = Lan.SaveProjectModel[i].Project_ID;
                                    ProjectPic.RTU_No = Lan.SaveProjectModel[i].RTU_No;
                                    ProjectPic.Start_Date = Lan.SaveProjectModel[i].Start_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    ProjectPic.Project_Pic_Name = "Con_Begin_Pic1";
                                    ProjectPic.Project_Pic_Content = PicToStr(Lan.SaveProjectModel[i].Con_Begin_Pic1);
                                    Lan.SaveProjectPicList.Add(ProjectPic);
                                    ProjectPic = null;
                                }
                                if (Lan.SaveProjectModel[i].Con_Begin_Pic2 != null && Lan.SaveProjectModel[i].Con_Begin_Pic2 != "")
                                {
                                    Project_pic ProjectPic = new Project_pic();
                                    ProjectPic.Project_ID = Lan.SaveProjectModel[i].Project_ID;
                                    ProjectPic.RTU_No = Lan.SaveProjectModel[i].RTU_No;
                                    ProjectPic.Start_Date = Lan.SaveProjectModel[i].Start_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    ProjectPic.Project_Pic_Name = "Con_Begin_Pic2";
                                    ProjectPic.Project_Pic_Content = PicToStr(Lan.SaveProjectModel[i].Con_Begin_Pic2);
                                    Lan.SaveProjectPicList.Add(ProjectPic);
                                    ProjectPic = null;
                                }
                                if (Lan.SaveProjectModel[i].Con_Begin_Pic3 != null && Lan.SaveProjectModel[i].Con_Begin_Pic3 != "")
                                {
                                    Project_pic ProjectPic = new Project_pic();
                                    ProjectPic.Project_ID = Lan.SaveProjectModel[i].Project_ID;
                                    ProjectPic.RTU_No = Lan.SaveProjectModel[i].RTU_No;
                                    ProjectPic.Start_Date = Lan.SaveProjectModel[i].Start_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    ProjectPic.Project_Pic_Name = "Con_Begin_Pic3";
                                    ProjectPic.Project_Pic_Content = PicToStr(Lan.SaveProjectModel[i].Con_Begin_Pic3);
                                    Lan.SaveProjectPicList.Add(ProjectPic);
                                    ProjectPic = null;
                                }
                                if (Lan.SaveProjectModel[i].Con_Begin_Pic4 != null && Lan.SaveProjectModel[i].Con_Begin_Pic4 != "")
                                {
                                    Project_pic ProjectPic = new Project_pic();
                                    ProjectPic.Project_ID = Lan.SaveProjectModel[i].Project_ID;
                                    ProjectPic.RTU_No = Lan.SaveProjectModel[i].RTU_No;
                                    ProjectPic.Start_Date = Lan.SaveProjectModel[i].Start_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    ProjectPic.Project_Pic_Name = "Con_Begin_Pic4";
                                    ProjectPic.Project_Pic_Content = PicToStr(Lan.SaveProjectModel[i].Con_Begin_Pic4);
                                    Lan.SaveProjectPicList.Add(ProjectPic);
                                    ProjectPic = null;
                                }
                                if (Lan.SaveProjectModel[i].Con_Begin_Pic5 != null && Lan.SaveProjectModel[i].Con_Begin_Pic5 != "")
                                {
                                    Project_pic ProjectPic = new Project_pic();
                                    ProjectPic.Project_ID = Lan.SaveProjectModel[i].Project_ID;
                                    ProjectPic.RTU_No = Lan.SaveProjectModel[i].RTU_No;
                                    ProjectPic.Start_Date = Lan.SaveProjectModel[i].Start_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    ProjectPic.Project_Pic_Name = "Con_Begin_Pic5";
                                    ProjectPic.Project_Pic_Content = PicToStr(Lan.SaveProjectModel[i].Con_Begin_Pic5);
                                    Lan.SaveProjectPicList.Add(ProjectPic);
                                    ProjectPic = null;
                                }
                                if (Lan.SaveProjectModel[i].Con_Begin_Pic6 != null && Lan.SaveProjectModel[i].Con_Begin_Pic6 != "")
                                {
                                    Project_pic ProjectPic = new Project_pic();
                                    ProjectPic.Project_ID = Lan.SaveProjectModel[i].Project_ID;
                                    ProjectPic.RTU_No = Lan.SaveProjectModel[i].RTU_No;
                                    ProjectPic.Start_Date = Lan.SaveProjectModel[i].Start_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    ProjectPic.Project_Pic_Name = "Con_Begin_Pic6";
                                    ProjectPic.Project_Pic_Content = PicToStr(Lan.SaveProjectModel[i].Con_Begin_Pic6);
                                    Lan.SaveProjectPicList.Add(ProjectPic);
                                    ProjectPic = null;
                                }
                                if (Lan.SaveProjectModel[i].Con_Begin_Pic7 != null && Lan.SaveProjectModel[i].Con_Begin_Pic7 != "")
                                {
                                    Project_pic ProjectPic = new Project_pic();
                                    ProjectPic.Project_ID = Lan.SaveProjectModel[i].Project_ID;
                                    ProjectPic.RTU_No = Lan.SaveProjectModel[i].RTU_No;
                                    ProjectPic.Start_Date = Lan.SaveProjectModel[i].Start_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    ProjectPic.Project_Pic_Name = "Con_Begin_Pic7";
                                    ProjectPic.Project_Pic_Content = PicToStr(Lan.SaveProjectModel[i].Con_Begin_Pic7);
                                    Lan.SaveProjectPicList.Add(ProjectPic);
                                    ProjectPic = null;
                                }
                                if (Lan.SaveProjectModel[i].Con_Begin_Pic8 != null && Lan.SaveProjectModel[i].Con_Begin_Pic8 != "")
                                {
                                    Project_pic ProjectPic = new Project_pic();
                                    ProjectPic.Project_ID = Lan.SaveProjectModel[i].Project_ID;
                                    ProjectPic.RTU_No = Lan.SaveProjectModel[i].RTU_No;
                                    ProjectPic.Start_Date = Lan.SaveProjectModel[i].Start_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    ProjectPic.Project_Pic_Name = "Con_Begin_Pic8";
                                    ProjectPic.Project_Pic_Content = PicToStr(Lan.SaveProjectModel[i].Con_Begin_Pic8);
                                    Lan.SaveProjectPicList.Add(ProjectPic);
                                    ProjectPic = null;
                                }
                                if (Lan.SaveProjectModel[i].Con_Begin_Pic9 != null && Lan.SaveProjectModel[i].Con_Begin_Pic9 != "")
                                {
                                    Project_pic ProjectPic = new Project_pic();
                                    ProjectPic.Project_ID = Lan.SaveProjectModel[i].Project_ID;
                                    ProjectPic.RTU_No = Lan.SaveProjectModel[i].RTU_No;
                                    ProjectPic.Start_Date = Lan.SaveProjectModel[i].Start_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    ProjectPic.Project_Pic_Name = "Con_Begin_Pic9";
                                    ProjectPic.Project_Pic_Content = PicToStr(Lan.SaveProjectModel[i].Con_Begin_Pic9);
                                    Lan.SaveProjectPicList.Add(ProjectPic);
                                    ProjectPic = null;
                                }
                                if (Lan.SaveProjectModel[i].Con_End_Pic1 != null && Lan.SaveProjectModel[i].Con_End_Pic1 != "")
                                {
                                    Project_pic ProjectPic = new Project_pic();
                                    ProjectPic.Project_ID = Lan.SaveProjectModel[i].Project_ID;
                                    ProjectPic.RTU_No = Lan.SaveProjectModel[i].RTU_No;
                                    ProjectPic.Start_Date = Lan.SaveProjectModel[i].Start_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    ProjectPic.Project_Pic_Name = "Con_End_Pic1";
                                    ProjectPic.Project_Pic_Content = PicToStr(Lan.SaveProjectModel[i].Con_End_Pic1);
                                    Lan.SaveProjectPicList.Add(ProjectPic);
                                    ProjectPic = null;
                                }
                            }



                            Res2 ResReadProject = new Res2();
                            ResReadProject.Command_Type = Command.Command_Type;
                            ResReadProject.Command_SN = Command.Command_SN;
                            ResReadProject.ResultSet_Name = "Project";
                            ResReadProject.Rows_Num = Lan.SaveProjectModel.Count;
                            ResReadProject.Error = Error;
                            Res = JsonHelper.SerializeObject(ResReadProject);
                            ResReadProject = null;
                        }
                        else
                        {
                            Res3 FailResReadProject = new Res3();
                            FailResReadProject.Command_Type = Command.Command_Type;
                            FailResReadProject.Command_SN = Command.Command_SN;
                            FailResReadProject.Error = Error;
                            Res = JsonHelper.SerializeObject(FailResReadProject);
                            FailResReadProject = null;
                        }
                        re = sendStringFrame(Lan, Res);
                        JsonToReadProject = null;
                        #endregion
                        break;
                    case 4:
                        #region 工程日志读取
                        ReadProject_Log JsonToReadLog = JsonHelper.DeserializeJsonToObject<ReadProject_Log>(strRx);
                        if (JsonToReadLog.Start_Time == "")
                        {
                            JsonToReadLog.Start_Time = null;
                        }
                        if (JsonToReadLog.End_Time == "")
                        {
                            JsonToReadLog.End_Time = null;
                        }
                        if (JsonToReadLog.Start_Date == "")
                        {
                            JsonToReadLog.Start_Date = null;
                        }

                        dstp.ConDBM.WriteLogMutex.WaitOne();
                        LanDAL.SelectProject_Log(JsonToReadLog.RTU_No, JsonToReadLog.Construction_Well_ID, Convert.ToDateTime(JsonToReadLog.Start_Date), Convert.ToDateTime(JsonToReadLog.Start_Time), Convert.ToDateTime(JsonToReadLog.End_Time), JsonToReadLog.Data_Tag, out Error, out Error_Message, out Lan.SaveLogModel);
                        dstp.ConDBM.WriteLogMutex.ReleaseMutex();

                        //应答
                        if (Error == 0)
                        {
                            Res2 ResReadLog = new Res2();
                            ResReadLog.Command_Type = Command.Command_Type;
                            ResReadLog.Command_SN = Command.Command_SN;
                            ResReadLog.ResultSet_Name = "Project_Log";
                            ResReadLog.Rows_Num = Lan.SaveLogModel.Count;
                            ResReadLog.Error = Error;
                            Res = JsonHelper.SerializeObject(ResReadLog);
                            ResReadLog = null;
                        }
                        else
                        {
                            Res3 FailResReadLog = new Res3();
                            FailResReadLog.Command_Type = Command.Command_Type;
                            FailResReadLog.Command_SN = Command.Command_SN;
                            FailResReadLog.Error = Error;
                            Res = JsonHelper.SerializeObject(FailResReadLog);
                            FailResReadLog = null;
                        }
                        re = sendStringFrame(Lan, Res);
                        JsonToReadLog = null;
                        #endregion
                        break;
                    case 5:
                        #region RTU数据读取
                        ReadRTU_Data JsonToReadRTUData = JsonHelper.DeserializeJsonToObject<ReadRTU_Data>(strRx);
                        if (JsonToReadRTUData.Start_Time == "")
                        {
                            JsonToReadRTUData.Start_Time = null;
                        }
                        if (JsonToReadRTUData.End_Time == "")
                        {
                            JsonToReadRTUData.End_Time = null;
                        }

                        dstp.ConDBM.WriteRTUDataMutex.WaitOne();
                        LanDAL.SelRtuData(JsonToReadRTUData.RTU_No, Convert.ToDateTime(JsonToReadRTUData.Start_Time), Convert.ToDateTime(JsonToReadRTUData.End_Time), JsonToReadRTUData.Data_Tag, out Error, out Error_Message, out Lan.SaveRtuDataModel);
                        dstp.ConDBM.WriteRTUDataMutex.ReleaseMutex();
                        //应答
                        if (Error == 0)
                        {
                            Res2 ResReadRtuData = new Res2();
                            ResReadRtuData.Command_Type = Command.Command_Type;
                            ResReadRtuData.Command_SN = Command.Command_SN;
                            ResReadRtuData.ResultSet_Name = "RTU_Data";
                            ResReadRtuData.Rows_Num = Lan.SaveRtuDataModel.Count;
                            ResReadRtuData.Error = Error;
                            Res = JsonHelper.SerializeObject(ResReadRtuData);
                            ResReadRtuData = null;
                        }
                        else
                        {
                            Res3 FailResReadRtuData = new Res3();
                            FailResReadRtuData.Command_Type = Command.Command_Type;
                            FailResReadRtuData.Command_SN = Command.Command_SN;
                            FailResReadRtuData.Error = Error;
                            Res = JsonHelper.SerializeObject(FailResReadRtuData);
                            FailResReadRtuData = null;
                        }
                        re = sendStringFrame(Lan, Res);
                        JsonToReadRTUData = null;
                        #endregion
                        break;
                    case 7:
                        #region RTU图片读取
                        ReadRTU_Pic JsonToReadRTUPic = JsonHelper.DeserializeJsonToObject<ReadRTU_Pic>(strRx);
                        if (JsonToReadRTUPic.Start_Time == "")
                        {
                            JsonToReadRTUPic.Start_Time = null;
                        }
                        if (JsonToReadRTUPic.End_Time == "")
                        {
                            JsonToReadRTUPic.End_Time = null;
                        }

                        dstp.ConDBM.WriteRTUpicMutex.WaitOne();
                        LanDAL.SelRtuPic(JsonToReadRTUPic.RTU_No, Convert.ToDateTime(JsonToReadRTUPic.Start_Time), Convert.ToDateTime(JsonToReadRTUPic.End_Time), JsonToReadRTUPic.Data_Tag, out Error, out Error_Message, out Lan.SaveRtuPicModel);
                        dstp.ConDBM.WriteRTUpicMutex.ReleaseMutex();


                        //应答
                        if (Error == 0)
                        {
                            Res2 ResReadRtuData = new Res2();
                            ResReadRtuData.Command_Type = Command.Command_Type;
                            ResReadRtuData.Command_SN = Command.Command_SN;
                            ResReadRtuData.ResultSet_Name = "RTU_Pic";
                            ResReadRtuData.Rows_Num = Lan.SaveRtuPicModel.Count;
                            ResReadRtuData.Error = Error;
                            Res = JsonHelper.SerializeObject(ResReadRtuData);
                            ResReadRtuData = null;
                        }
                        else
                        {
                            Res3 FailResReadRtuData = new Res3();
                            FailResReadRtuData.Command_Type = Command.Command_Type;
                            FailResReadRtuData.Command_SN = Command.Command_SN;
                            FailResReadRtuData.Error = Error;
                            Res = JsonHelper.SerializeObject(FailResReadRtuData);
                            FailResReadRtuData = null;
                        }
                        re = sendStringFrame(Lan, Res);
                        JsonToReadRTUPic = null;
                        #endregion
                        break;
                    case 11:
                        #region 工程信息图片读取
                        SelectProjectPic JsonToSelectProjectPic = JsonHelper.DeserializeJsonToObject<SelectProjectPic>(strRx);

                        Res2 ResReadProjectPic = new Res2();
                        ResReadProjectPic.Command_Type = Command.Command_Type;
                        ResReadProjectPic.Command_SN = Command.Command_SN;
                        ResReadProjectPic.ResultSet_Name = "Project_Pic";
                        ResReadProjectPic.Rows_Num = Lan.SaveProjectPicList.Count;
                        ResReadProjectPic.Error = 0;
                        Res = JsonHelper.SerializeObject(ResReadProjectPic);
                        ResReadProjectPic = null;

                        re = sendStringFrame(Lan, Res);
                        JsonToSelectProjectPic = null;
                        #endregion
                        break;
                    case 2:
                        #region APP账号修改
                        UpdateAPPAccount JsonToUpDateAPPData = JsonHelper.DeserializeJsonToObject<UpdateAPPAccount>(strRx);

                        try
                        {
                            for (int i = 0; i < JsonToUpDateAPPData.Account.Count; i++)
                            {
                                LanDAL.UpdateAccount(JsonToUpDateAPPData.Account[i].Account_ID, JsonToUpDateAPPData.Account[i].Account_Name, JsonToUpDateAPPData.Account[i].Account_Password, JsonToUpDateAPPData.Account[i].Account_Time, JsonToUpDateAPPData.Account[i].Name, JsonToUpDateAPPData.Account[i].TEL, JsonToUpDateAPPData.Account[i].Company, JsonToUpDateAPPData.Account[i].Account_Picture, JsonToUpDateAPPData.Account[i].Account_Permission, out Error, out Error_Message);
                            }

                            Res3 ResUpdateAccountData = new Res3();
                            ResUpdateAccountData.Command_Type = Command.Command_Type;
                            ResUpdateAccountData.Command_SN = Command.Command_SN;
                            ResUpdateAccountData.Error = 0;
                            Res = JsonHelper.SerializeObject(ResUpdateAccountData);
                            re = sendStringFrame(Lan, Res);
                            ResUpdateAccountData = null;
                        }
                        catch (Exception ex)
                        {
                            Res3 FailResUpdateAccountData = new Res3();
                            FailResUpdateAccountData.Command_Type = Command.Command_Type;
                            FailResUpdateAccountData.Command_SN = Command.Command_SN;
                            FailResUpdateAccountData.Error = 1;
                            Res = JsonHelper.SerializeObject(FailResUpdateAccountData);
                            re = sendStringFrame(Lan, Res);
                            FailResUpdateAccountData = null;

                            DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " receiveOneByte " + ex.Message.ToString());
                        }
                        finally
                        {
                            JsonToUpDateAPPData = null;
                        }
                        #endregion
                        break;
                    case 6:
                        #region RTU参数设置
                        UpdateRTUPara JsonToUpdateRtuPara = JsonHelper.DeserializeJsonToObject<UpdateRTUPara>(strRx);

                        re = false;
                        try
                        {
                            //先看看到底是设置什么参数，再进行相应操作
                            if (JsonToUpdateRtuPara.RTU_IsTrue != 0)
                            {
                                //更新数据库RTU状态
                                LanDAL.UpdateRTUPara(JsonToUpdateRtuPara.RTU_No, JsonToUpdateRtuPara.RTU_IsTrue, out Error, out Error_Message);
                                //修改RTUList中状态
                                for (int i = 0; i < ConnectToRTU.RTULlist.Count; i++)
                                {
                                    if (ConnectToRTU.RTULlist[i].RTU_No == JsonToUpdateRtuPara.RTU_No)
                                    {
                                        ConnectToRTU.RTULlist[i].RTU_ParaMeter_No = JsonToUpdateRtuPara.RTU_IsTrue;
                                    }
                                }
                            }
                            if (JsonToUpdateRtuPara.RTU_Cycle_Seconds != 0)
                            {
                                bool bRtuOpen = false;
                                for (int i = 0; i < ConnectToRTU.RTULlist.Count; i++)
                                {
                                    if (ConnectToRTU.RTULlist[i].RTU_No == JsonToUpdateRtuPara.RTU_No)
                                    {
                                        if (JsonToUpdateRtuPara.RTU_Cycle_Seconds != 0)
                                        {
                                            ConnectToRTU.RTULlist[i].UploadCycle = JsonToUpdateRtuPara.RTU_Cycle_Seconds;
                                            ConnectToRTU.RTULlist[i].autoEventTriggerPara.Set();
                                            //这里没管应答，直接返回正确了
                                            bRtuOpen = true;
                                            Error = 0;
                                        }
                                    }
                                }
                                if (bRtuOpen == false)
                                {
                                    Error = 1;//表明该RTU没有开启或者不存在该RTU
                                }
                            }
                            //应答内网
                            if (Error == 0)
                            {
                                Res3 ResRtuPara = new Res3();
                                ResRtuPara.Command_Type = Command.Command_Type;
                                ResRtuPara.Command_SN = Command.Command_SN;
                                ResRtuPara.Error = 0;
                                Res = JsonHelper.SerializeObject(ResRtuPara);
                                re = sendStringFrame(Lan, Res);
                                ResRtuPara = null;
                            }
                            else
                            {
                                //返回错误号1(不存在RTU设备或该RTU没进行连接)
                                Res3 FailResRtuPara = new Res3();
                                FailResRtuPara.Command_Type = Command.Command_Type;
                                FailResRtuPara.Command_SN = Command.Command_SN;
                                FailResRtuPara.Error = 1;
                                Res = JsonHelper.SerializeObject(FailResRtuPara);
                                re = sendStringFrame(Lan, Res);
                                FailResRtuPara = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            //返回错误号2(操作异常)
                            Res3 FailResRtuPara = new Res3();
                            FailResRtuPara.Command_Type = Command.Command_Type;
                            FailResRtuPara.Command_SN = Command.Command_SN;
                            FailResRtuPara.Error = 2;
                            Res = JsonHelper.SerializeObject(FailResRtuPara);
                            re = sendStringFrame(Lan, Res);
                            FailResRtuPara = null;

                            DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " RTU参数设置 " + ex.Message.ToString());
                        }
                        finally
                        {
                            JsonToUpdateRtuPara = null;
                        }
                        #endregion
                        break;
                    case 8:
                        #region 触发RTU拍照
                        re = false;

                        RTUNowPic JsonToRtuPicNow = JsonHelper.DeserializeJsonToObject<RTUNowPic>(strRx);

                        List<RTU> RTU = new List<RTU>();
                        RTU = ConnectToRTU.RTULlist;
                        for (int i = 0; i < RTU.Count; i++)
                        {
                            if (RTU[i].RTU_No == JsonToRtuPicNow.RTU_No)
                            {
                                try
                                {
                                    RTU[i].autoEventTriggerCam.Set();

                                    Res3 ResRtuPicNow = new Res3();
                                    ResRtuPicNow.Command_Type = Command.Command_Type;
                                    ResRtuPicNow.Command_SN = Command.Command_SN;
                                    ResRtuPicNow.Error = 0;
                                    Res = JsonHelper.SerializeObject(ResRtuPicNow);
                                    re = sendStringFrame(Lan, Res);
                                    ResRtuPicNow = null;
                                }
                                catch (Exception ex)
                                {
                                    Res3 FailResRtuPicNow = new Res3();
                                    FailResRtuPicNow.Command_Type = Command.Command_Type;
                                    FailResRtuPicNow.Command_SN = Command.Command_SN;
                                    FailResRtuPicNow.Error = 1;
                                    Res = JsonHelper.SerializeObject(FailResRtuPicNow);
                                    re = sendStringFrame(Lan, Res);
                                    FailResRtuPicNow = null;

                                    DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " 触发RTU拍照 " + ex.Message.ToString());
                                }
                            }
                        }
                        if (re == false) //返回错误号2（没有这个RTU或者RTU没有开启）
                        {
                            Res3 FailResRtuPicNow = new Res3();
                            FailResRtuPicNow.Command_Type = Command.Command_Type;
                            FailResRtuPicNow.Command_SN = Command.Command_SN;
                            FailResRtuPicNow.Error = 2;
                            Res = JsonHelper.SerializeObject(FailResRtuPicNow);
                            re = sendStringFrame(Lan, Res);
                            FailResRtuPicNow = null;
                        }
                        JsonToRtuPicNow = null;
                        RTU = null;
                        #endregion
                        break;
                    case 52:
                        #region 数据浏览
                        SelectData JsonToSelectData = JsonHelper.DeserializeJsonToObject<SelectData>(strRx);

                        //初始化发送数据条数为0条
                        Lan.ListCountSum = 0;

                        try
                        {
                            if (JsonToSelectData.ResultSet_Name == "Account")//APP账号信息数据浏览，最大上传1000条
                            {
                                List<AccountModel> NewListAccount = new List<AccountModel>();
                                NewListAccount = Lan.SaveAccountModel.GetRange(JsonToSelectData.Row_First, JsonToSelectData.Row_Last - JsonToSelectData.Row_First + 1);
                                Res4 ResAccountData = new Res4();
                                ResAccountData.Command_Type = Command.Command_Type;
                                ResAccountData.Command_SN = Command.Command_SN;
                                ResAccountData.Error = 0;
                                ResAccountData.ResultSet_Name = "Account";
                                ResAccountData.Rows_Num = NewListAccount.Count;
                                ResAccountData.ResultSet_Data = new List<Account>();
                                for (int i = 0; i < NewListAccount.Count; i++)
                                {
                                    Account AccountData = new Account();
                                    AccountData.Account_ID = NewListAccount[i].Account_ID;
                                    AccountData.Account_Name = NewListAccount[i].Account_Name;
                                    AccountData.Account_Password = NewListAccount[i].Account_Password;
                                    AccountData.Account_Time = NewListAccount[i].Account_Time.ToString("yyyy-MM-dd");
                                    AccountData.Name = NewListAccount[i].Name;
                                    AccountData.TEL = NewListAccount[i].TEL;
                                    AccountData.Company = NewListAccount[i].Company;
                                    AccountData.Account_Picture = PicToStrbyAccount(NewListAccount[i].Account_Picture);
                                    AccountData.Account_Permission = NewListAccount[i].Account_Permission;
                                    AccountData.Last_Operate_Date = NewListAccount[i].Last_Operate_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    AccountData.Last_Operate_Type = NewListAccount[i].Last_Operate_Type;
                                    AccountData.Account_IsUse = NewListAccount[i].Account_IsUse;
                                    ResAccountData.ResultSet_Data.Add(AccountData);
                                    Lan.ListCountSum++;
                                }
                                Res = JsonHelper.SerializeObject(ResAccountData);
                                re = sendStringFrame(Lan, Res);
                                NewListAccount = null;
                            }
                            if (JsonToSelectData.ResultSet_Name == "Project")//工程信息数据浏览,最大上传数1000(接口已修改，图片另外上传，详情查看接口说明)
                            {
                                List<ProjectModel> NewListProject = new List<ProjectModel>();
                                NewListProject = Lan.SaveProjectModel.GetRange(JsonToSelectData.Row_First, JsonToSelectData.Row_Last - JsonToSelectData.Row_First + 1);
                                Res5 ResProjectData = new Res5();
                                ResProjectData.Command_Type = Command.Command_Type;
                                ResProjectData.Command_SN = Command.Command_SN;
                                ResProjectData.Error = 0;
                                ResProjectData.ResultSet_Name = "Project";
                                ResProjectData.Rows_Num = NewListProject.Count;
                                ResProjectData.ResultSet_Data = new List<Project>();

                                #region ResultSet_Data赋值
                                for (int i = 0; i < NewListProject.Count; i++)
                                {
                                    Project ProjectData = new Project();
                                    ProjectData.Project_ID = NewListProject[i].Project_ID;
                                    ProjectData.Account_ID = NewListProject[i].Account_ID;
                                    ProjectData.Start_Date = NewListProject[i].Start_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    ProjectData.Jar_Shape = NewListProject[i].Jar_Shape;
                                    ProjectData.Jar_Size = NewListProject[i].Jar_Size;
                                    ProjectData.Jar_Volume = NewListProject[i].Jar_Volume;
                                    ProjectData.RTU_No = NewListProject[i].RTU_No;
                                    ProjectData.Construction_Name = NewListProject[i].Construction_Name;
                                    ProjectData.Construction_Well_ID = NewListProject[i].Construction_Well_ID;
                                    ProjectData.Oil_Factory = NewListProject[i].Oil_Factory;
                                    ProjectData.Area = NewListProject[i].Area;
                                    ProjectData.Place = NewListProject[i].Place;
                                    ProjectData.Team_Leader_Name = NewListProject[i].Team_Leader_Name;
                                    ProjectData.Team_Leader_Tel = NewListProject[i].Team_Leader_Tel;
                                    //ProjectData.Team_Leader_Picture = PicToStr(NewListProject[i].Team_Leader_Picture);
                                    ProjectData.Team_Worker_Name = NewListProject[i].Team_Worker_Name;
                                    ProjectData.Team_Worker_Tel = NewListProject[i].Team_Worker_Tel;
                                    //ProjectData.Team_Worker_Picture = PicToStr(NewListProject[i].Team_Worker_Picture);
                                    ProjectData.Company_Name = NewListProject[i].Company_Name;
                                    //ProjectData.Con_Begin_Pic1 = PicToStr(NewListProject[i].Con_Begin_Pic1);
                                    //ProjectData.Con_Begin_Pic2 = PicToStr(NewListProject[i].Con_Begin_Pic2);
                                    //ProjectData.Con_Begin_Pic3 = PicToStr(NewListProject[i].Con_Begin_Pic3);
                                    //ProjectData.Con_Begin_Pic4 = PicToStr(NewListProject[i].Con_Begin_Pic4);
                                    //ProjectData.Con_Begin_Pic5 = PicToStr(NewListProject[i].Con_Begin_Pic5);
                                    //ProjectData.Con_Begin_Pic6 = PicToStr(NewListProject[i].Con_Begin_Pic6);
                                    //ProjectData.Con_Begin_Pic7 = PicToStr(NewListProject[i].Con_Begin_Pic7);
                                    //ProjectData.Con_Begin_Pic8 = PicToStr(NewListProject[i].Con_Begin_Pic8);
                                    //ProjectData.Con_Begin_Pic9 = PicToStr(NewListProject[i].Con_Begin_Pic9);
                                    ProjectData.Con_Begin_OilPressure = NewListProject[i].Con_Begin_OilPressure;
                                    ProjectData.Con_Begin_CasingPressure = NewListProject[i].Con_Begin_CasingPressure;
                                    ProjectData.Con_Begin_Dayinflow = NewListProject[i].Con_Begin_Dayinflow;
                                    ProjectData.Con_Begin_IsSeparate = NewListProject[i].Con_Begin_IsSeparate;
                                    ProjectData.Con_Begin_SepPresure = NewListProject[i].Con_Begin_SepPresure;
                                    if (NewListProject[i].End_Date == DateTime.MinValue)//判断施工结束日期是不是空
                                    {
                                        ProjectData.End_Date = null;
                                    }
                                    else
                                    {
                                        ProjectData.End_Date = NewListProject[i].End_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    }
                                    ProjectData.Con_End_CasingPressure = NewListProject[i].Con_End_CasingPressure;
                                    ProjectData.Con_End_OilPressure = NewListProject[i].Con_End_OilPressure;
                                    ProjectData.Con_End_Dayinflow = NewListProject[i].Con_End_Dayinflow;
                                    ProjectData.CasPre_Cause = NewListProject[i].CasPre_Cause;
                                    //ProjectData.Con_End_Pic1 = PicToStr(NewListProject[i].Con_End_Pic1);
                                    ProjectData.Last_Operate_Type = NewListProject[i].Last_Operate_Type;
                                    ProjectData.Last_Operate_Date = NewListProject[i].Last_Operate_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    ProjectData.Jam = new List<Jam>();
                                    //查出本工程信息所有的堵剂信息
                                    List<JamModel> JamModelData = new List<JamModel>();
                                    LanDAL.SelectJam(ProjectData.Project_ID, out JamModelData);
                                    for (int n = 0; n < JamModelData.Count; n++)
                                    {
                                        Jam JamData = new Jam();
                                        JamData.Jam_ID = JamModelData[n].Jam_ID;
                                        JamData.Jam_Name = JamModelData[n].Jam_Name;
                                        JamData.Jam_Num = JamModelData[n].Jam_Num;
                                        ProjectData.Jam.Add(JamData);
                                    }
                                    ResProjectData.ResultSet_Data.Add(ProjectData);
                                    Lan.ListCountSum++;
                                }
                                #endregion

                                Res = JsonHelper.SerializeObject(ResProjectData);
                                re = sendStringFrame(Lan, Res);
                                NewListProject = null;
                            }
                            if (JsonToSelectData.ResultSet_Name == "Project_Log")//工程日志数据浏览，最大上传1000条
                            {
                                List<Project_LogModels> NewListLog = new List<Project_LogModels>();
                                NewListLog = Lan.SaveLogModel.GetRange(JsonToSelectData.Row_First, JsonToSelectData.Row_Last - JsonToSelectData.Row_First + 1);
                                Res6 ResLogData = new Res6();
                                ResLogData.Command_Type = Command.Command_Type;
                                ResLogData.Command_SN = Command.Command_SN;
                                ResLogData.Error = 0;
                                ResLogData.ResultSet_Name = "Project_Log";
                                ResLogData.Rows_Num = NewListLog.Count;
                                ResLogData.ResultSet_Data = new List<Project_Log>();
                                for (int i = 0; i < NewListLog.Count; i++)
                                {
                                    Project_Log LogData = new Project_Log();
                                    LogData.Project_Log_ID = NewListLog[i].Project_Log_ID;
                                    LogData.Project_ID = NewListLog[i].Project_ID;
                                    LogData.Construction_Time = NewListLog[i].Construction_Time.ToString("yyyy-MM-dd HH:mm:ss");
                                    LogData.Pressure = NewListLog[i].Pressure;
                                    LogData.Displacement = NewListLog[i].Displacement;
                                    LogData.Displacement_Acc = NewListLog[i].Displacement_Acc;
                                    LogData.Slug_Name = NewListLog[i].Slug_Name;
                                    LogData.Formula = NewListLog[i].Formula;
                                    LogData.Events = NewListLog[i].Events;
                                    LogData.Last_Operate_Type = NewListLog[i].Last_Operate_Type;
                                    LogData.Last_Operate_Date = NewListLog[i].Last_Operate_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    ResLogData.ResultSet_Data.Add(LogData);
                                    Lan.ListCountSum++;
                                }
                                Res = JsonHelper.SerializeObject(ResLogData);
                                re = sendStringFrame(Lan, Res);
                            }
                            if (JsonToSelectData.ResultSet_Name == "RTU_Data")//RTU数据数据浏览，最大上传1000条
                            {
                                List<RtuDataModel> NewListRtuData = new List<RtuDataModel>();
                                NewListRtuData = Lan.SaveRtuDataModel.GetRange(JsonToSelectData.Row_First, JsonToSelectData.Row_Last - JsonToSelectData.Row_First + 1);
                                Res7 ResRtuData = new Res7();
                                ResRtuData.Command_Type = Command.Command_Type;
                                ResRtuData.Command_SN = Command.Command_SN;
                                ResRtuData.Error = 0;
                                ResRtuData.ResultSet_Name = "RTU_Data";
                                ResRtuData.Rows_Num = NewListRtuData.Count;
                                ResRtuData.ResultSet_Data = new List<RTU_Data>();
                                for (int i = 0; i < NewListRtuData.Count; i++)
                                {
                                    RTU_Data RtuData = new RTU_Data();
                                    RtuData.RTU_Data_ID = NewListRtuData[i].RTU_Data_ID;
                                    RtuData.RTU_No = NewListRtuData[i].RTU_No;
                                    RtuData.Displacement = NewListRtuData[i].Displacement;
                                    RtuData.Displacement_Acc = NewListRtuData[i].Displacement_Acc;
                                    RtuData.Pressure = NewListRtuData[i].Pressure;
                                    RtuData.Detect_Time = NewListRtuData[i].Detect_Time.ToString("yyyy-MM-dd HH:mm:ss");
                                    RtuData.Upload_Time = NewListRtuData[i].Upload_Time.ToString("yyyy-MM-dd HH:mm:ss");
                                    ResRtuData.ResultSet_Data.Add(RtuData);
                                    Lan.ListCountSum++;
                                }
                                Res = JsonHelper.SerializeObject(ResRtuData);
                                re = sendStringFrame(Lan, Res);
                                NewListRtuData = null;
                            }
                            if (JsonToSelectData.ResultSet_Name == "RTU_Pic")//RTU图片数据浏览，最大上传20张
                            {
                                List<RtuPicModel> NewListRtuPic = new List<RtuPicModel>();
                                NewListRtuPic = Lan.SaveRtuPicModel.GetRange(JsonToSelectData.Row_First, JsonToSelectData.Row_Last - JsonToSelectData.Row_First + 1);
                                Res8 ResRtuPic = new Res8();
                                ResRtuPic.Command_Type = Command.Command_Type;
                                ResRtuPic.Command_SN = Command.Command_SN;
                                ResRtuPic.Error = 0;
                                ResRtuPic.ResultSet_Name = "RTU_Pic";
                                ResRtuPic.Rows_Num = NewListRtuPic.Count;
                                ResRtuPic.ResultSet_Data = new List<RTU_Pic>();
                                for (int i = 0; i < NewListRtuPic.Count; i++)
                                {
                                    RTU_Pic RtuPic = new RTU_Pic();
                                    RtuPic.RTU_Pic_ID = NewListRtuPic[i].RTU_Pic_ID;
                                    RtuPic.Pic_Name = NewListRtuPic[i].Pic_name;
                                    RtuPic.RTU_No = NewListRtuPic[i].RTU_No;
                                    RtuPic.RTU_Pic_Content = PicToStrbyRTU(NewListRtuPic[i].RTU_Pic_Address);
                                    RtuPic.Detect_Time = NewListRtuPic[i].Detect_Time.ToString("yyyy-MM-dd HH:mm:ss");
                                    RtuPic.Upload_Time = NewListRtuPic[i].Upload_Time.ToString("yyyy-MM-dd HH:mm:ss");
                                    ResRtuPic.ResultSet_Data.Add(RtuPic);
                                    Lan.ListCountSum++;
                                }
                                Res = JsonHelper.SerializeObject(ResRtuPic);
                                re = sendStringFrame(Lan, Res);
                                NewListRtuPic = null;
                            }
                            if (JsonToSelectData.ResultSet_Name == "Project_Pic")//工程信息图片上传，最好上传2张
                            {
                                List<Project_pic> NewListProjectPic = new List<Project_pic>();
                                NewListProjectPic = Lan.SaveProjectPicList.GetRange(JsonToSelectData.Row_First, JsonToSelectData.Row_Last - JsonToSelectData.Row_First + 1);

                                Res9 ResUploadProjectPic = new Res9();
                                ResUploadProjectPic.Command_Type = Command.Command_Type;
                                ResUploadProjectPic.Command_SN = Command.Command_SN;
                                ResUploadProjectPic.Error = 0;
                                ResUploadProjectPic.ResultSet_Name = "Project_Pic";
                                ResUploadProjectPic.Rows_Num = NewListProjectPic.Count;
                                ResUploadProjectPic.ResultSet_Data = new List<Project_pic>();
                                ResUploadProjectPic.ResultSet_Data = NewListProjectPic;
                                Res = JsonHelper.SerializeObject(ResUploadProjectPic);
                                re = sendStringFrame(Lan, Res);
                                ResUploadProjectPic = null;
                            }
                        }
                        catch (Exception ex)//处理失败时发送失败应答
                        {
                            Res3 FailResData = new Res3();
                            FailResData.Command_Type = Command.Command_Type;
                            FailResData.Command_SN = Command.Command_SN;
                            FailResData.Error = 1;
                            Res = JsonHelper.SerializeObject(FailResData);
                            re = sendStringFrame(Lan, Res);
                            FailResData = null;

                            DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " 数据浏览 " + ex.Message.ToString());
                        }
                        finally
                        {
                            JsonToSelectData = null;
                        }
                        #endregion
                        break;
                    case 9:
                        #region 事务结束
                        SelectDataEnd JsonToSelDataEnd = JsonHelper.DeserializeJsonToObject<SelectDataEnd>(strRx);

                        if (bSelDataEnd)//指示是否开启事务结束指令,用于测试时不修改Sign值(正式环境可不添加)
                        {
                            Error_Message = "";
                            try
                            {
                                if (Lan.SaveAccountModel.Count != 0)
                                {
                                    for (int i = 0; i < Lan.SaveAccountModel.Count; i++)
                                    {
                                        LanDAL.UpdateAccountSign(Lan.SaveAccountModel[i].Account_ID);
                                    }
                                    Lan.SaveAccountModel = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                if (Lan.SaveAccountModel != null)
                                {
                                    Error_Message = "更新标记失败";
                                    DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " 事务结束(SaveAccountModel) " + ex.Message.ToString());
                                }
                            }
                            try
                            {
                                if (Lan.SaveProjectModel.Count != 0)
                                {
                                    for (int i = 0; i < Lan.SaveProjectModel.Count; i++)
                                    {
                                        LanDAL.UpdateProjectSign(Lan.SaveProjectModel[i].Project_ID);
                                        LanDAL.UpdateJamSign(Lan.SaveProjectModel[i].Project_ID);
                                    }
                                    Lan.SaveProjectModel = null;
                                    Lan.SaveProjectPicList = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                if (Lan.SaveProjectModel != null)
                                {
                                    Error_Message = "更新标记失败";
                                    DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " 事务结束(SaveProjectModel) " + ex.Message.ToString());
                                }
                            }
                            try
                            {
                                if (Lan.SaveLogModel.Count != 0)
                                {
                                    for (int i = 0; i < Lan.SaveLogModel.Count; i++)
                                    {
                                        LanDAL.UpdateProjectLogSign(Lan.SaveLogModel[i].Project_Log_ID);
                                    }
                                    Lan.SaveLogModel = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                if (Lan.SaveLogModel != null)
                                {
                                    Error_Message = "更新标记失败";
                                    DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " 事务结束(SaveLogModel) " + ex.Message.ToString());
                                }
                            }
                            try
                            {
                                if (Lan.SaveRtuDataModel.Count != 0)
                                {
                                    for (int i = 0; i < Lan.SaveRtuDataModel.Count; i++)
                                    {
                                        LanDAL.UpdateRTUDataSign(Lan.SaveRtuDataModel[i].RTU_Data_ID);
                                    }
                                    Lan.SaveRtuDataModel = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                if (Lan.SaveRtuDataModel != null)
                                {
                                    Error_Message = "更新标记失败";
                                    DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " 事务结束(SaveRtuDataModel) " + ex.Message.ToString());
                                }
                            }
                            try
                            {
                                if (Lan.SaveRtuPicModel.Count != 0)
                                {
                                    for (int i = 0; i < Lan.SaveRtuPicModel.Count; i++)
                                    {
                                        LanDAL.UpdateRTUPicSign(Lan.SaveRtuPicModel[i].RTU_Pic_ID);
                                    }
                                    Lan.SaveRtuPicModel = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                if (Lan.SaveRtuPicModel != null)
                                {
                                    Error_Message = "更新标记失败";
                                    DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " 事务结束(SaveRtuPicModel) " + ex.Message.ToString());
                                }
                            }

                        }
                        #endregion
                        break;
                    case 100://心跳包发送(这里先留着，心跳包由云平台发送，内网可能需要返回数据)

                        break;
                    default:
                        #region 命令号错误
                        Res3 FailResType = new Res3();
                        FailResType.Command_Type = Command.Command_Type;
                        FailResType.Command_SN = Command.Command_SN;
                        FailResType.Error = 5;
                        Res = JsonHelper.SerializeObject(FailResType);
                        re = sendStringFrame(Lan, Res);
                        FailResType = null;

                        Lan.LaseSendDT = DateTime.Now;
                        Lan.bHeartBeat = true;
                        #endregion
                        break;
                }
                #endregion

                //显示到界面
                if (Error == 0)
                {
                    Error_Message = "Success";
                }
                else
                {
                    Error_Message = "Fail";
                }
                string str = string.Format("   {0}   {1}   {2}", Lan.client.Client.RemoteEndPoint.ToString(), Command.Command_Type, Error_Message);
                dstp.Invoke(writeDebug, new object[] { str, 3 });

            }
            catch (System.Exception ex)
            {
                Res3 FailResType = new Res3();
                FailResType.Command_Type = Command.Command_Type;
                FailResType.Command_SN = Command.Command_SN;
                FailResType.Error = 6;
                Res = JsonHelper.SerializeObject(FailResType);
                re = sendStringFrame(Lan, Res);
                FailResType = null;

                Lan.LaseSendDT = DateTime.Now;
                Lan.bHeartBeat = true;

                //显示到界面
                string str = string.Format("   {0}   {1}   {2}", Lan.client.Client.RemoteEndPoint.ToString(), Command.Command_Type, "Fail");
                dstp.Invoke(writeDebug, new object[] { str, 3 });

                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " 命令号接收失败 " + ex.Message.ToString());
            }
            finally
            {
                int TypeNo = Command.Command_Type;
                if (TypeNo == 0 || TypeNo == 2 || TypeNo == 6 || TypeNo == 8 || TypeNo == 9)//一个流程操作结束后空闲线程开始按固定时间发送心跳包
                {
                    Lan.LaseSendDT = DateTime.Now;
                    Lan.bHeartBeat = true;
                }
            }
        }
        #endregion

        #region 格式转化
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
        /// <param name="cmdBuff">转化前Byte数组</param>
        /// <param name="countBytes">Byte数组字节数</param>
        /// <returns></returns>
        public string binaryArrayToString(byte[] cmdBuff, int countBytes)
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

        #region 发送消息
        /// <summary>
        /// 发送格式化的命令帧(加STX和 ETX)
        /// </summary>
        /// <param name="strCMD">帧数据</param>
        bool sendStringFrame(Lan Lan, string strCMD)
        {
            bool bRst = false;
            try
            {
                byte[] buffer = new byte[1];
                byte[] msg = Encoding.UTF8.GetBytes(strCMD);

                buffer[0] = 0x02;
                SendToClient(Lan, buffer);
                SendToClient(Lan, msg);
                buffer[0] = 0x03;
                SendToClient(Lan, buffer);
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
        /// 发送操作命令给指定app
        /// </summary>
        /// <param name="rtu"></param>
        /// <param name="message"></param>
        private void SendToClient(Lan Lan, byte[] message)
        {
            try
            {
                //将字符串写入网络流，此方法会自动附加字符串长度前缀
                Lan.bw.Write(message);
                Lan.bw.Flush();
            }
            catch
            {
                //编写失败原因，输出至界面并写入日志
            }
        }
        #endregion

        #region 图片操作(数据化与反数据化)
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="Pic_Name">图片名称</param>
        /// <param name="Pic_Content">图片内容</param>
        public void SavePic(string Pic_Name, string Pic_Content)
        {
            try
            {
                byte[] bytePic_Content = StringToByteArray(Pic_Content);
                Image myImg = BytesToImg2(bytePic_Content);
                myImg.Save("AppPic//" + Pic_Name, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " SavePic " + ex.Message.ToString());
            }
        }

        /// <summary>
        /// 保存图片到本地AppPic文件夹下
        /// </summary>
        /// <param name="PicName">图片数据库名</param>
        /// <param name="PicContent">图片内容（数据）</param>
        /// <param name="RTU_No">RTU编号</param>
        /// <param name="Start_Date">施工开始时间</param>
        /// <returns>要保存的数据库信息</returns>
        public string SavePics(string PicName, string PicContent, int RTU_No, string Start_Date)
        {
            string Pic_Name = "";//图片名称，eg：123420170323170000Con_Begin_Pic1.jpg
            if (PicContent != "")
            {
                Pic_Name = RTU_No.ToString();
                Pic_Name += Convert.ToDateTime(Start_Date).ToString("yyyyMMddHHmmss");
                Pic_Name += PicName;
                Pic_Name += ".jpg";
                SavePic(Pic_Name, PicContent);
            }
            return Pic_Name;
        }

        /// <summary>
        /// 将图片数据化为String格式(APP)
        /// </summary>
        /// <param name="PicName">图片名称</param>
        /// <returns></returns>
        public string PicToStr(string PicName)
        {
            string TmpPicStr = "";
            try
            {
                if (PicName != "")
                {
                    string StrDir = @"AppPic/";
                    byte[] ImageBytes = { 0 };
                    Bitmap IMageBit = new Bitmap(StrDir + PicName);
                    ImageBytes = ImgToBytes2(IMageBit);
                    try
                    {
                        IMageBit.Dispose();
                    }
                    catch (Exception ex)
                    {
                        DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, ex.Message.ToString());
                    }
                    //TmpPicStr = binaryArrayToString(ImageBytes, ImageBytes.Length); //此方法在转化时效率较低，已使用下面的方法
                    TmpPicStr = ByteToString(ImageBytes);
                }
            }
            catch (Exception ex)
            {
                TmpPicStr = "";

                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " PicToStr " + ex.Message.ToString());
            }
            return TmpPicStr;
        }

        /// <summary>
        /// 将图片数据化为String格式(RTU)
        /// </summary>
        /// <param name="PicName">图片名称</param>
        /// <returns></returns>
        public string PicToStrbyRTU(string PicAddress)
        {
            string TmpPicStr = "";
            try
            {

                if (PicAddress != "")
                {
                    byte[] ImageBytes = { 0 };
                    Bitmap IMageBit = new Bitmap(@PicAddress);
                    ImageBytes = ImgToBytes2(IMageBit);
                    try
                    {
                        IMageBit.Dispose();
                    }
                    catch (Exception ex)
                    {
                        DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, ex.Message.ToString());
                    }
                    //TmpPicStr = binaryArrayToString(ImageBytes, ImageBytes.Length); //此方法在转化时效率较低，已使用下面的方法
                    TmpPicStr = ByteToString(ImageBytes);
                }
            }
            catch (Exception ex)
            {
                TmpPicStr = "";

                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " PicToStrbyRTU " + ex.Message.ToString() + " " + PicAddress);
            }
            return TmpPicStr;
        }

        /// <summary>
        /// 将图片数据化为String格式(RTU)
        /// </summary>
        /// <param name="PicName">图片名称</param>
        /// <returns></returns>
        public string PicToStrbyAccount(string PicName)
        {
            string TmpPicStr = "";
            try
            {

                if (PicName != "")
                {
                    string StrDir = @"AccountPic/";
                    byte[] ImageBytes = { 0 };
                    Bitmap IMageBit = new Bitmap(StrDir + PicName);
                    ImageBytes = ImgToBytes2(IMageBit);
                    try
                    {
                        IMageBit.Dispose();
                    }
                    catch (Exception ex)
                    {
                        DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, ex.Message.ToString());
                    }
                    //TmpPicStr = binaryArrayToString(ImageBytes, ImageBytes.Length); //此方法在转化时效率较低，已使用下面的方法
                    TmpPicStr = ByteToString(ImageBytes);
                }
            }
            catch (Exception ex)
            {
                TmpPicStr = "";

                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, ex.Message.ToString());
            }
            return TmpPicStr;
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

                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, ex.Message.ToString());
                return null;
            }

        }

        /// <summary>
        /// 这个方法将内存图像以Png格式序列化，数据量大大的降低，目测效率也是比前一种要好一点，代码更加简单易读。
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public byte[] ImgToBytes2(Bitmap bmp)
        {
            try
            {
                MemoryStream sr = new MemoryStream();
                //bmp.Save(sr, System.Drawing.Imaging.ImageFormat.Png);
                bmp.Save(sr, System.Drawing.Imaging.ImageFormat.Jpeg);
                int len = (int)sr.Position;
                byte[] ret = new byte[sr.Position];
                sr.Seek(0, SeekOrigin.Begin);
                sr.Read(ret, 0, len);
                return ret;
            }
            catch (System.Exception ex)
            {

                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, ex.Message.ToString());
                return null;
            }

        }
        #endregion
    }
}
