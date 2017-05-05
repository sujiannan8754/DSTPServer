using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSTP_BLL.ClassToApp;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using static DSTP_BLL.ClassToApp.ClassConnectToApp;
using DSTP_BLL.ClassToOther;
using DSTP_DAL;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DSTPServer
{
    //定义写主窗口的debug Text的代理类型
    public delegate void deleWriteDebugBox1(string strText, int writeMode);

    /// <summary>
    /// APP连接管理
    /// </summary>
    class ConnectToAPP
    {

        #region 字段
        /// <summary>
        /// 保存所有连接的RTU设备
        /// </summary>
        private List<APP> APPLlist = new List<APP>();

        /// <summary>
        /// 服务器IP地址
        /// </summary>;
        private string ServerIP;

        /// <summary>
        /// 监听端口
        /// </summary>
        private int port;
        private TcpListener myListener;

        DSTP dstp;

        AppDAL AppDAL = new AppDAL();

        deleWriteDebugBox1 writeDebug;

        /// <summary>
        /// 关闭线程的信号
        /// </summary>
        AutoResetEvent CloseEvent;
        #endregion

        #region 开始结束监听客户端

        /// <summary>
        /// 开始监听APP设备
        /// </summary>
        public void Begin_Listen(string ServerIP, int port, DSTP dstp)
        {
            try
            {
                this.dstp = dstp;
                writeDebug = dstp.writeDebugBox;
                myListener = new TcpListener(IPAddress.Parse(ServerIP), Convert.ToInt32(port));
                myListener.Start();
                
                //创建一个线程开始监听客户端的连接请求
                Thread AppThread = new Thread(ListenClientConnect);
                AppThread.IsBackground = true;//设置为后台线程
                AppThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("APP连接失败，检查服务器IP是否正确");

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
                for (int i = APPLlist.Count - 1; i >= 0; i--)
                {
                    APPLlist[i].isNormalExit = true;
                    RemoveAPP(APPLlist[i]);
                }
                myListener.Stop();
            }
            catch (Exception ex)
            {
                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " Stop_Listen " + ex.Message.ToString());
            }
        }


        /// <summary>
        /// 移除RTU设备（同时关闭Socket）
        /// </summary>
        /// <param name="rtu">指定要移除的用户</param>
        private void RemoveAPP(APP app)
        {
            try
            {
                app.isNormalExit = true;
                APPLlist.Remove(app);
                app.Close();
            }
            catch (Exception ex)
            {
                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " RemoveAPP " + ex.Message.ToString());
            }
        }
        #endregion

        #region APP监听线程
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

                    //线程等待100毫秒，让Socket建立完成连接(之后写成信号的形式,这里需要修改)
                    Thread.Sleep(100);

                    //每接收一个客户端连接，就创建一个对应的线程循环接收该客户端发来的信息
                    APP app = new APP(newClient);
                    app.strReceived = new StringBuilder();
                    app.LaseSendDT = DateTime.Now;

                    bool b = false;
                    int Count = 0;
                    for (int i = 0; i < APPLlist.Count; i++) //判断相同IP地址是否进行过连接
                    {
                        string a = app.client.Client.RemoteEndPoint.ToString().Split(':')[0];
                        if (app.client.Client.RemoteEndPoint.ToString().Split(':')[0] == APPLlist[i].client.Client.RemoteEndPoint.ToString().Split(':')[0])
                        {
                            b = true;
                            Count = i;
                        }
                    }
                    if (b == true)
                    {
                        RemoveAPP(APPLlist[Count]);
                    }
                    APPLlist.Add(app);


                    Thread threadReceive = new Thread(Child_Thread);
                    threadReceive.IsBackground = true;
                    threadReceive.Start(app);

                    string str = string.Format("   {0}   连入系统", app.client.Client.RemoteEndPoint.ToString());
                    dstp.Invoke(writeDebug, new object[] { str, 2 });
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

        #region APP子线程(接收数据，空闲线程)
        private void Child_Thread(object userState)
        {
            APP app = (APP)userState;
            TcpClient client = app.client;

            Thread Operation_Thread;
            Operation_Thread = new Thread(thread_receiving_datas); //接收数据
            Operation_Thread.IsBackground = true;
            Operation_Thread.Start(app);

            Operation_Thread = new Thread(thread_free); //空闲线程
            Operation_Thread.IsBackground = true;
            Operation_Thread.Start(app);
        }
        #endregion

        #region 接收数据，空闲操作
        /// <summary>
        /// 接收客户端消息
        /// </summary>
        /// <param name="userState">客户端信息</param>
        private void thread_receiving_datas(object userState)
        {
            APP app = (APP)userState;
            TcpClient client = app.client;

            StateRxOneString oneStringRxState = StateRxOneString.NoSTX;

            while (true)
            {
                Thread.Sleep(200);
                if (app.isNormalExit == true)
                {
                    break;
                }

                try
                {
                    while (client.Available > 0)
                    {
                        app.bFinish = false;
                        app.LaseSendDT = DateTime.Now;
                        byte[] receiveBytes = null;
                        int Available = 0;
                        receiveBytes = app.br.ReadBytes(client.Available);
                        Available = receiveBytes.Count();

                        app.byteReceived = new byte[Available - 2];
                        //首先判断帧的完整性

                        for (int i = 0; i < Available; i++)
                        {
                            receiveOneByte(app, ref oneStringRxState, receiveBytes[i]);
                        }

                        #region 这里是UTF8格式发送的尝试,现在不用这个方法了
                        //for (int i = 0; i < Available - 2; i++)
                        //{
                        //    app.byteReceived[i] = receiveBytes[i+1];
                        //}
                        //checkMyFrameType(app, Encoding.UTF8.GetString(app.byteReceived));
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    if (app.isNormalExit == false)
                    {
                        //string ErrorMessage = client.Client.RemoteEndPoint.ToString();//错误信息
                        app.isNormalExit = true;
                    }

                    DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " thread_receiving_datas " + ex.Message.ToString());

                    break;
                }
            }
        }

        /// <summary>
        /// 空闲线程
        /// </summary> 
        void thread_free(object userState)
        {
            APP app = (APP)userState;
            TcpClient client = app.client;
            while (true)
            {
                Thread.Sleep(200);
                if (app.isNormalExit == true)
                {
                    break;
                }
                try
                {
                    DateTime NowDate = DateTime.Now;
                    TimeSpan TS = NowDate - app.LaseSendDT;
                    if (TS.TotalMinutes > 2 && NowDate > app.LaseSendDT && app.LaseSendDT != DateTime.MinValue) //APP操作完一次数据，2分钟后关闭连接
                    {
                        //先看看还有没有这个链接
                        for (int i = 0; i < APPLlist.Count; i++)
                        {
                            if (APPLlist[i].client == app.client) //存在的情况下，已经超过了2分钟，直接关闭这个链接
                            {
                                string str = string.Format("   {0}   退出系统", app.client.Client.RemoteEndPoint.ToString());
                                dstp.Invoke(writeDebug, new object[] { str, 2 });
                                RemoveAPP(app);
                                break;
                            }
                        }
                    }

                    if (app.bFinish) //正常情况下除了有关数据浏览的接口操作完都会直接关闭连接
                    {
                        string str = string.Format("   {0}   退出系统", app.client.Client.RemoteEndPoint.ToString());
                        dstp.Invoke(writeDebug, new object[] { str, 2 });
                        RemoveAPP(app);
                        break;
                    }


                    //bool bRst = app.autoEventTriggerData.WaitOne(10, false);
                    //if (bRst)
                    //{
                    //    //工程结束，开始转移数据
                    //    AppDAL.SaveTable(app.SaveID.Project_ID, app.SaveID.RTU_No);
                    //}
                }
                catch (Exception ex)
                {
                    DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " thread_free " + ex.Message.ToString());
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
            APP app = (APP)userState;
            TcpClient client = app.client;
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
                        app.strReceived.Length = 0;

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
                        checkMyFrameType(app, app.strReceived.ToString());
                        myState = StateRxOneString.NoSTX;
                        #endregion
                    }
                    else //STX和ETX中间的字符的接收
                    {
                        #region

                        byte[] tmpArr = new byte[1];
                        tmpArr[0] = oneByte;
                        app.strReceived.Append(System.Text.Encoding.ASCII.GetString(tmpArr));
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

        #region 判断消息命令,命令分发，完成操作并回应消息
        /// <summary>
        /// 检查收到的帧结构
        /// 然后进行命令分发
        /// </summary>
        /// <param name="strRx">ASCII编码后的数据帧</param>
        void checkMyFrameType(APP app, string strRx)
        {
            int Error = 0;
            string ErrorMessage = "";
            string Res = "";
            AppModel Command = new AppModel();
            bool re = false;
            try
            {
                //将接收到的数据转化为JSON格式,接收消息类型与命令序号
                Command = JsonHelper.DeserializeJsonToObject<AppModel>(strRx);

                #region  下面进行消息分发
                switch (Command.Command_Type)
                {
                    case 11:
                        #region 登录验证
                        //接收登录信息
                        Login login = JsonHelper.DeserializeJsonToObject<Login>(strRx);

                        //查询数据库
                        AccountModel AccountModel = new AccountModel();
                        AppDAL.SelectLogin(login.Data1, login.Data2, out AccountModel, out Error, out ErrorMessage);

                        //回应消息
                        if (Error == 0)
                        {
                            Res1 res1 = new Res1();
                            res1.Command_Type = Command.Command_Type;
                            res1.Command_SN = Command.Command_SN;
                            res1.Name = AccountModel.Name;
                            res1.TEL = AccountModel.TEL;
                            res1.Company = AccountModel.Company;
                            res1.Account_Permission = AccountModel.Account_Permission;
                            res1.Error = Error;
                            res1.Error_Message = ErrorMessage;
                            Res = JsonHelper.SerializeObject(res1);
                        }
                        else
                        {
                            Res2 res2 = new Res2();
                            res2.Command_Type = Command.Command_Type;
                            res2.Command_SN = Command.Command_SN;
                            res2.Error = Error;
                            res2.Error_Message = ErrorMessage;
                            Res = JsonHelper.SerializeObject(res2);
                        }
                        re = sendStringFrame(app, Res);
                        #endregion
                        break;
                    case 21:
                        #region 工程信息新增
                        //接收信息
                        AddProject addproject = JsonHelper.DeserializeJsonToObject<AddProject>(strRx);

                        if(addproject.Start_Date=="")
                        {
                            addproject.Start_Date = null;
                        }

                        //操作数据库
                        dstp.ConDBM.WriteProjectMutex.WaitOne();
                        AppDAL.AddProject(addproject.RTU_No, Convert.ToDateTime(addproject.Start_Date), addproject.Construction_Well_ID, addproject.Team_Leader_Name, addproject.Team_Leader_Tel, addproject.Company_Name, addproject.Data1, out Error, out ErrorMessage);
                        dstp.ConDBM.WriteProjectMutex.ReleaseMutex();


                        //应答
                        Res2 res = new Res2();
                        res.Command_Type = Command.Command_Type;
                        res.Command_SN = Command.Command_SN;
                        res.Error = Error;
                        res.Error_Message = ErrorMessage;
                        Res = JsonHelper.SerializeObject(res);

                        re = sendStringFrame(app, Res);
                        #endregion
                        break;
                    case 22:
                        #region 工程信息上传

                        //接收数据
                        UpdateProject up = JsonHelper.DeserializeJsonToObject<UpdateProject>(strRx);

                        //#region 处理图片
                        //up.Project.Team_Leader_Picture = SavePics("Team_Leader_Picture", up.Project.Team_Leader_Picture, up.Project.RTU_No, up.Project.Start_Date);
                        //up.Project.Team_Worker_Picture = SavePics("Team_Worker_Picture", up.Project.Team_Worker_Picture, up.Project.RTU_No, up.Project.Start_Date);
                        //up.Project.Con_Begin_Pic1 = SavePics("Con_Begin_Pic1", up.Project.Con_Begin_Pic1, up.Project.RTU_No, up.Project.Start_Date);
                        //up.Project.Con_Begin_Pic2 = SavePics("Con_Begin_Pic2", up.Project.Con_Begin_Pic2, up.Project.RTU_No, up.Project.Start_Date);
                        //up.Project.Con_Begin_Pic3 = SavePics("Con_Begin_Pic3", up.Project.Con_Begin_Pic3, up.Project.RTU_No, up.Project.Start_Date);
                        //up.Project.Con_Begin_Pic4 = SavePics("Con_Begin_Pic4", up.Project.Con_Begin_Pic4, up.Project.RTU_No, up.Project.Start_Date);
                        //up.Project.Con_Begin_Pic5 = SavePics("Con_Begin_Pic5", up.Project.Con_Begin_Pic5, up.Project.RTU_No, up.Project.Start_Date);
                        //up.Project.Con_Begin_Pic6 = SavePics("Con_Begin_Pic6", up.Project.Con_Begin_Pic6, up.Project.RTU_No, up.Project.Start_Date);
                        //up.Project.Con_Begin_Pic7 = SavePics("Con_Begin_Pic7", up.Project.Con_Begin_Pic7, up.Project.RTU_No, up.Project.Start_Date);
                        //up.Project.Con_Begin_Pic8 = SavePics("Con_Begin_Pic8", up.Project.Con_Begin_Pic8, up.Project.RTU_No, up.Project.Start_Date);
                        //up.Project.Con_Begin_Pic9 = SavePics("Con_Begin_Pic9", up.Project.Con_Begin_Pic9, up.Project.RTU_No, up.Project.Start_Date);
                        //up.Project.Con_End_Pic1 = SavePics("Con_End_Pic1", up.Project.Con_End_Pic1, up.Project.RTU_No, up.Project.Start_Date);
                        //#endregion

                        //操作数据库
                        ProjectModel PM = new ProjectModel();
                        List<JamModel> Jam = new List<JamModel>();

                        #region 接收数据工程信息数据
                        PM.Start_Date = Convert.ToDateTime(up.Project.Start_Date);
                        PM.Jar_Shape = up.Project.Jar_Shape;
                        PM.Jar_Size = up.Project.Jar_Size;
                        PM.Jar_Volume = up.Project.Jar_Volume;
                        PM.RTU_No = up.Project.RTU_No;
                        PM.Construction_Name = up.Project.Construction_Name;
                        PM.Construction_Well_ID = up.Project.Construction_Well_ID;
                        PM.Oil_Factory = up.Project.Oil_Factory;
                        PM.Area = up.Project.Area;
                        PM.Place = up.Project.Place;
                        PM.Team_Leader_Name = up.Project.Team_Leader_Name;
                        PM.Team_Leader_Tel = up.Project.Team_Leader_Tel;
                        //PM.Team_Leader_Picture = up.Project.Team_Leader_Picture;
                        PM.Team_Worker_Name = up.Project.Team_Worker_Name;
                        PM.Team_Worker_Tel = up.Project.Team_Worker_Tel;
                        //PM.Team_Worker_Picture = up.Project.Team_Worker_Picture;
                        PM.Company_Name = up.Project.Company_Name;
                        //PM.Con_Begin_Pic1 = up.Project.Con_Begin_Pic1;
                        //PM.Con_Begin_Pic2 = up.Project.Con_Begin_Pic2;
                        //PM.Con_Begin_Pic3 = up.Project.Con_Begin_Pic3;
                        //PM.Con_Begin_Pic4 = up.Project.Con_Begin_Pic4;
                        //PM.Con_Begin_Pic5 = up.Project.Con_Begin_Pic5;
                        //PM.Con_Begin_Pic6 = up.Project.Con_Begin_Pic6;
                        //PM.Con_Begin_Pic7 = up.Project.Con_Begin_Pic7;
                        //PM.Con_Begin_Pic8 = up.Project.Con_Begin_Pic8;
                        //PM.Con_Begin_Pic9 = up.Project.Con_Begin_Pic9;
                        PM.Con_Begin_OilPressure = up.Project.Con_Begin_OilPressure;
                        PM.Con_Begin_CasingPressure = up.Project.Con_Begin_CasingPressure;
                        PM.Con_Begin_Dayinflow = up.Project.Con_Begin_Dayinflow;
                        PM.Con_Begin_IsSeparate = up.Project.Con_Begin_IsSeparate;
                        PM.Con_Begin_SepPresure = up.Project.Con_Begin_SepPresure;
                        if (up.Project.End_Date == "" || up.Project.End_Date==null)
                        {
                            PM.End_Date = DateTime.MinValue;
                        }
                        else
                        {
                            PM.End_Date = Convert.ToDateTime(up.Project.End_Date);
                        }
                        PM.Con_End_OilPressure = up.Project.Con_End_OilPressure;
                        PM.Con_End_CasingPressure = up.Project.Con_End_CasingPressure;
                        PM.Con_End_Dayinflow = up.Project.Con_End_Dayinflow;
                        PM.CasPre_Cause = up.Project.CasPre_Cause;
                        //PM.Con_End_Pic1 = up.Project.Con_End_Pic1;
                        PM.Project_State = up.Project.Project_State;
                        #endregion

                        #region 接收堵剂数量数据
                        //先查出要修改的工程信息的主键
                        string Project_ID = AppDAL.SelProject_ID(Convert.ToDateTime(up.Project.Start_Date), up.Project.RTU_No);

                        for (int i = 0; i < up.Project.Jam.Count; i++)
                        {
                            JamModel jamModel = new JamModel();
                            jamModel.Jam_ID = Guid.NewGuid().ToString();
                            jamModel.Project_ID = Project_ID;
                            jamModel.Jam_Name = up.Project.Jam[i].Jam_Name;
                            jamModel.Jam_Num = up.Project.Jam[i].Jam_Num;
                            jamModel.Sign = 1;
                            jamModel.Last_Operate_People = up.Data1;
                            jamModel.Last_Operate_Date = DateTime.Now;
                            jamModel.Last_Operate_Type = 0;
                            Jam.Add(jamModel);
                        }
                        #endregion

                        dstp.ConDBM.WriteProjectMutex.WaitOne();
                        if(PM.End_Date>PM.Start_Date || PM.End_Date==DateTime.MinValue) //只有施工结束日期大于施工开始日期或者施工结束日期为最小（没有填写）时才进行操作
                        {
                            AppDAL.UpdateProject(PM, Jam, up.Data1, out Error, out ErrorMessage);
                        }
                        else
                        {
                            Error = 1;
                            ErrorMessage = "施工结束日期填写有误";
                        }
                        dstp.ConDBM.WriteProjectMutex.ReleaseMutex();

                        //应答
                        Res2 res22 = new Res2();
                        res22.Command_Type = Command.Command_Type;
                        res22.Command_SN = Command.Command_SN;
                        res22.Error = Error;
                        res22.Error_Message = ErrorMessage;
                        Res = JsonHelper.SerializeObject(res22);

                        re = sendStringFrame(app, Res);

                        //查看是否是已经完结的工程信息，是则将该条完结的工程信息所有数据存放到历史表中
                        if(PM.Project_State == 1)
                        {
                            app.SaveID.Project_ID = Project_ID;
                            app.SaveID.RTU_No = PM.RTU_No;

                            try
                            {
                                AppDAL.SaveTable(app.SaveID.Project_ID, app.SaveID.RTU_No);
                            }
                            catch (Exception ex)
                            {
                                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " checkMyFrameType " + ex.Message.ToString());
                            }
                        }
                        #endregion
                        break;
                    case 23:
                        #region 工程概要信息查询
                        SelectGYProject selgy = JsonHelper.DeserializeJsonToObject<SelectGYProject>(strRx);
                        List<ProjectModel> ListProject = new List<ProjectModel>();
                        if(selgy.Start_Time=="")
                        {
                            selgy.Start_Time = DateTime.MinValue.ToString();
                        }
                        if(selgy.End_Time=="")
                        {
                            selgy.End_Time = DateTime.MaxValue.ToString();
                        }
                        DateTime Start_Time = Convert.ToDateTime(selgy.Start_Time);
                        DateTime End_Time = Convert.ToDateTime(selgy.End_Time);

                        dstp.ConDBM.WriteProjectMutex.WaitOne();
                        AppDAL.SelectOutLineProject(selgy.RTU_No, selgy.Construction_Well_ID, selgy.Company_Name, Start_Time, End_Time, out Error, out ErrorMessage, out ListProject);
                        dstp.ConDBM.WriteProjectMutex.ReleaseMutex();

                        //将查询的信息存起来
                        app.SaveProject = new List<ProjectModel>();
                        app.SaveProject = ListProject;

                        //应答
                        if (Error == 0)
                        {
                            Res3 ResOutLine = new Res3();
                            ResOutLine.Command_Type = Command.Command_Type;
                            ResOutLine.Command_SN = Command.Command_SN;
                            ResOutLine.ResultSet_Name = "Project";
                            ResOutLine.Rows_Num = ListProject.Count;
                            ResOutLine.Error = Error;
                            ResOutLine.Error_Message = ErrorMessage;
                            Res = JsonHelper.SerializeObject(ResOutLine);

                            if(ListProject.Count==0)//如果查询出数据条数为0,则直接退出连接
                            {
                                app.bFinish = true;
                            }
                        }
                        else
                        {
                            Res2 FailResOutLine = new Res2();
                            FailResOutLine.Command_Type = Command.Command_Type;
                            FailResOutLine.Command_SN = Command.Command_SN;
                            FailResOutLine.Error = Error;
                            FailResOutLine.Error_Message = ErrorMessage;
                            Res = JsonHelper.SerializeObject(FailResOutLine);
                        }
                        re = sendStringFrame(app, Res);
                        #endregion
                        break;
                    case 25:
                        #region 工程详细信息查询
                        SelectXXProject selxx = JsonHelper.DeserializeJsonToObject<SelectXXProject>(strRx);

                        ProjectModel ProjectModel = new ProjectModel();
                        List<JamModel> ListJam = new List<JamModel>();

                        dstp.ConDBM.WriteProjectMutex.WaitOne();
                        AppDAL.SelectDetailedProject(selxx.RTU_No, Convert.ToDateTime(selxx.Start_Date), out Error, out ErrorMessage, out ProjectModel, out ListJam);
                        dstp.ConDBM.WriteProjectMutex.ReleaseMutex();

                        if (Error == 0)
                        {
                            Res4 ResDetaile = new Res4();
                            ResDetaile.Jam = new List<DSTP_BLL.ClassToApp.Jam>();
                            JamModel JamModel = new JamModel();

                            #region 接收数据
                            ResDetaile.Command_Type = Command.Command_Type;
                            ResDetaile.Command_SN = Command.Command_SN;
                            ResDetaile.Error = Error;
                            ResDetaile.Error_Message = ErrorMessage;
                            ResDetaile.Project_ID = ProjectModel.Project_ID;
                            ResDetaile.Start_Date = ProjectModel.Start_Date.ToString("yyyy-MM-dd HH:mm:ss");
                            ResDetaile.Jar_Shape = ProjectModel.Jar_Shape;
                            ResDetaile.Jar_Size = ProjectModel.Jar_Size;
                            ResDetaile.Jar_Volume = ProjectModel.Jar_Volume;
                            ResDetaile.RTU_No = ProjectModel.RTU_No;
                            ResDetaile.Construction_Name = ProjectModel.Construction_Name;
                            ResDetaile.Construction_Well_ID = ProjectModel.Construction_Well_ID;
                            ResDetaile.Oil_Factory = ProjectModel.Oil_Factory;
                            ResDetaile.Area = ProjectModel.Area;
                            ResDetaile.Place = ProjectModel.Place;
                            ResDetaile.Team_Leader_Name = ProjectModel.Team_Leader_Name;
                            ResDetaile.Team_Leader_Tel = ProjectModel.Team_Leader_Tel;
                            //ResDetaile.Team_Leader_Picture = PicToStr(ProjectModel.Team_Leader_Picture);
                            ResDetaile.Team_Worker_Name = ProjectModel.Team_Worker_Name;
                            ResDetaile.Team_Worker_Tel = ProjectModel.Team_Worker_Tel;
                            //ResDetaile.Team_Worker_Picture = PicToStr(ProjectModel.Team_Worker_Picture);
                            ResDetaile.Company_Name = ProjectModel.Company_Name;
                            //ResDetaile.Con_Begin_Pic1 = PicToStr(ProjectModel.Con_Begin_Pic1);
                            //ResDetaile.Con_Begin_Pic2 = PicToStr(ProjectModel.Con_Begin_Pic2);
                            //ResDetaile.Con_Begin_Pic3 = PicToStr(ProjectModel.Con_Begin_Pic3);
                            //ResDetaile.Con_Begin_Pic4 = PicToStr(ProjectModel.Con_Begin_Pic4);
                            //ResDetaile.Con_Begin_Pic5 = PicToStr(ProjectModel.Con_Begin_Pic5);
                            //ResDetaile.Con_Begin_Pic6 = PicToStr(ProjectModel.Con_Begin_Pic6);
                            //ResDetaile.Con_Begin_Pic7 = PicToStr(ProjectModel.Con_Begin_Pic7);
                            //ResDetaile.Con_Begin_Pic8 = PicToStr(ProjectModel.Con_Begin_Pic8);
                            //ResDetaile.Con_Begin_Pic9 = PicToStr(ProjectModel.Con_Begin_Pic9);
                            ResDetaile.Con_Begin_OilPressure = ProjectModel.Con_Begin_OilPressure;
                            ResDetaile.Con_Begin_CasingPressure = ProjectModel.Con_Begin_CasingPressure;
                            ResDetaile.Con_Begin_Dayinflow = ProjectModel.Con_Begin_Dayinflow;
                            ResDetaile.Con_Begin_IsSeparate = ProjectModel.Con_Begin_IsSeparate;
                            ResDetaile.Con_Begin_SepPresure = ProjectModel.Con_Begin_SepPresure;
                            ResDetaile.End_Date = ProjectModel.End_Date.ToString("yyyy-MM-dd HH:mm:ss");
                            ResDetaile.Con_End_CasingPressure = ProjectModel.Con_End_CasingPressure;
                            ResDetaile.Con_End_Dayinflow = ProjectModel.Con_End_Dayinflow;
                            ResDetaile.CasPre_Cause = ProjectModel.CasPre_Cause;
                            //ResDetaile.Con_End_Pic1 = PicToStr(ProjectModel.Con_End_Pic1);
                            ResDetaile.Project_State = ProjectModel.Project_State;

                            for (int i = 0; i < ListJam.Count; i++)
                            {
                                Jam JamInfo = new DSTP_BLL.ClassToApp.Jam();
                                JamInfo.Jam_Name = ListJam[i].Jam_Name;
                                JamInfo.Jam_Num = ListJam[i].Jam_Num;
                                ResDetaile.Jam.Add(JamInfo);
                            }
                            #endregion

                            Res = JsonHelper.SerializeObject(ResDetaile);
                        }
                        else
                        {
                            Res2 FailResDetaile = new Res2();
                            FailResDetaile.Command_Type = Command.Command_Type;
                            FailResDetaile.Command_SN = Command.Command_SN;
                            FailResDetaile.Error = Error;
                            FailResDetaile.Error_Message = ErrorMessage;
                            Res = JsonHelper.SerializeObject(FailResDetaile);
                        }
                        re = sendStringFrame(app, Res);
                        #endregion
                        break;
                    case 26:
                        #region 工程信息图片查询
                        SelectProjectPic JsonToProjectPic = JsonHelper.DeserializeJsonToObject<SelectProjectPic>(strRx);

                        string Project_Pic_Content = "";

                        dstp.ConDBM.WriteProjectMutex.WaitOne();
                        AppDAL.SelectProject_Pic(JsonToProjectPic.RTU_No, JsonToProjectPic.Start_Date, JsonToProjectPic.Project_Pic_Name, out Project_Pic_Content, out Error, out ErrorMessage);
                        dstp.ConDBM.WriteProjectMutex.ReleaseMutex();

                        if (Error == 0)
                        {
                            Res8 ResSelectProjectPic = new Res8();
                            ResSelectProjectPic.Command_Type = Command.Command_Type;
                            ResSelectProjectPic.Command_SN = Command.Command_SN;
                            ResSelectProjectPic.Error = Error;
                            ResSelectProjectPic.Error_Message = ErrorMessage;
                            ResSelectProjectPic.Project_Pic_Name = JsonToProjectPic.Project_Pic_Name;
                            ResSelectProjectPic.Project_Pic_Content = PicToStr(Project_Pic_Content);

                            Res = JsonHelper.SerializeObject(ResSelectProjectPic);
                        }
                        else
                        {
                            Res2 FailSelectProjectPic = new Res2();
                            FailSelectProjectPic.Command_Type = Command.Command_Type;
                            FailSelectProjectPic.Command_SN = Command.Command_SN;
                            FailSelectProjectPic.Error = Error;
                            FailSelectProjectPic.Error_Message = ErrorMessage;
                            Res = JsonHelper.SerializeObject(FailSelectProjectPic);
                        }
                        re = sendStringFrame(app, Res);
                        #endregion
                        break;
                    case 24:
                        #region 工程信息删除
                        DeleteProject delProject = JsonHelper.DeserializeJsonToObject<DeleteProject>(strRx);

                        dstp.ConDBM.WriteProjectMutex.WaitOne();
                        AppDAL.DeleteProject(delProject.RTU_No, Convert.ToDateTime(delProject.Start_Date), delProject.Project_ID, delProject.Data1, out Error, out ErrorMessage);
                        dstp.ConDBM.WriteProjectMutex.ReleaseMutex();

                        //回应
                        Res2 ResDelProject = new Res2();
                        ResDelProject.Command_Type = Command.Command_Type;
                        ResDelProject.Command_SN = Command.Command_SN;
                        ResDelProject.Error = Error;
                        ResDelProject.Error_Message = ErrorMessage;
                        Res = JsonHelper.SerializeObject(ResDelProject);

                        re = sendStringFrame(app, Res);
                        #endregion
                        break;
                    case 31:
                        #region 工程日志上传
                        AddProject_Log JSONtoAddLog = JsonHelper.DeserializeJsonToObject<AddProject_Log>(strRx);

                        //接收消息
                        List<Project_LogModels> AddList = new List<Project_LogModels>();
                        List<Project_LogModels> UpdateList = new List<Project_LogModels>();
                        if (JSONtoAddLog.Project_Log_Add.Count == 0 && JSONtoAddLog.Project_Log_Update.Count == 0)
                        {
                            //没上传数据
                            Error = 1;
                            ErrorMessage += "添加失败，未上传数据";
                        }
                        else
                        {
                            #region 遍历接收数据
                            for (int i = 0; i < JSONtoAddLog.Project_Log_Add.Count; i++)
                            {
                                //接收时看看工程日志的施工时间正确性
                                if (JSONtoAddLog.Project_Log_Add[i].Construction_Time == "" || JSONtoAddLog.Project_Log_Add[i].Construction_Time == null)
                                {
                                    Error = 1;
                                    ErrorMessage += "添加失败，检查施工时间正确性";
                                }
                                else
                                {
                                    if (Convert.ToDateTime(JSONtoAddLog.Project_Log_Add[i].Construction_Time) < Convert.ToDateTime(JSONtoAddLog.Project_Log_Add[i].Start_Date))
                                    {
                                        Error = 1;
                                        ErrorMessage += "添加失败，检查施工时间正确性";
                                    }
                                    else
                                    {
                                        string SelProject_ID = AppDAL.SelProject_ID(Convert.ToDateTime(JSONtoAddLog.Project_Log_Add[i].Start_Date), JSONtoAddLog.Project_Log_Add[i].RTU_No);
                                        if (SelProject_ID == "" || SelProject_ID == null)
                                        {
                                            //没查到工程信息
                                            Error = 1;
                                            ErrorMessage += "添加失败，未查询到工程信息" + JSONtoAddLog.Project_Log_Add[i].Start_Date + "  " + JSONtoAddLog.Project_Log_Add[i].RTU_No;
                                        }
                                        else
                                        {
                                            Project_LogModels AddLog = new Project_LogModels();
                                            AddLog.Project_Log_ID = Guid.NewGuid().ToString();
                                            AddLog.Project_ID = SelProject_ID;
                                            AddLog.Project_Log_No = JSONtoAddLog.Project_Log_Add[i].Project_Log_No;
                                            if (JSONtoAddLog.Project_Log_Add[i].Construction_Time == "")
                                            {
                                                AddLog.Construction_Time = DateTime.MinValue;
                                            }
                                            else
                                            {
                                                AddLog.Construction_Time = Convert.ToDateTime(JSONtoAddLog.Project_Log_Add[i].Construction_Time);
                                            }
                                            AddLog.Pressure = JSONtoAddLog.Project_Log_Add[i].Pressure;
                                            AddLog.Displacement = JSONtoAddLog.Project_Log_Add[i].Displacement;
                                            AddLog.Displacement_Acc = JSONtoAddLog.Project_Log_Add[i].Displacement_Acc;
                                            AddLog.Slug_Name = JSONtoAddLog.Project_Log_Add[i].Slug_Name;
                                            AddLog.Formula = JSONtoAddLog.Project_Log_Add[i].Formula;
                                            AddLog.Events = JSONtoAddLog.Project_Log_Add[i].Events;
                                            AddLog.Sign = 1;
                                            AddLog.Last_Operate_People = JSONtoAddLog.Data1;
                                            AddLog.Last_Operate_Date = DateTime.Now;
                                            AddLog.Last_Operate_Type = 0;
                                            AddList.Add(AddLog);
                                        }
                                    }
                                }
                            }


                            for (int n = 0; n < JSONtoAddLog.Project_Log_Update.Count; n++)
                            {
                                Project_LogModels UpdateLog = new Project_LogModels();
                                UpdateLog.Project_ID = AppDAL.SelProject_ID(Convert.ToDateTime(JSONtoAddLog.Project_Log_Update[n].Start_Date), JSONtoAddLog.Project_Log_Update[n].RTU_No);
                                UpdateLog.Project_Log_No = JSONtoAddLog.Project_Log_Update[n].Project_Log_No;
                                if (JSONtoAddLog.Project_Log_Update[n].Construction_Time == "")
                                {
                                    UpdateLog.Construction_Time = DateTime.MinValue;
                                }
                                else
                                {
                                    UpdateLog.Construction_Time = Convert.ToDateTime(JSONtoAddLog.Project_Log_Update[n].Construction_Time);
                                }
                                UpdateLog.Pressure = JSONtoAddLog.Project_Log_Update[n].Pressure;
                                UpdateLog.Displacement = JSONtoAddLog.Project_Log_Update[n].Displacement;
                                UpdateLog.Displacement_Acc = JSONtoAddLog.Project_Log_Update[n].Displacement_Acc;
                                UpdateLog.Slug_Name = JSONtoAddLog.Project_Log_Update[n].Slug_Name;
                                UpdateLog.Formula = JSONtoAddLog.Project_Log_Update[n].Formula;
                                UpdateLog.Events = JSONtoAddLog.Project_Log_Update[n].Events;
                                UpdateLog.Sign = 1;
                                UpdateLog.Last_Operate_People = JSONtoAddLog.Data1;
                                UpdateLog.Last_Operate_Date = DateTime.Now;
                                UpdateLog.Last_Operate_Type = 1;
                                UpdateList.Add(UpdateLog);
                            }
                            #endregion
                        }

                        if (Error == 0)
                        {
                            //操作数据库
                            dstp.ConDBM.WriteLogMutex.WaitOne();
                            AppDAL.UpdateProject_Log(AddList, UpdateList, out Error, out ErrorMessage);
                            dstp.ConDBM.WriteLogMutex.ReleaseMutex();
                        }

                        //回应
                        Res2 ResUpdateProject_Log = new Res2();
                        ResUpdateProject_Log.Command_Type = Command.Command_Type;
                        ResUpdateProject_Log.Command_SN = Command.Command_SN;
                        ResUpdateProject_Log.Error = Error;
                        ResUpdateProject_Log.Error_Message = ErrorMessage;
                        Res = JsonHelper.SerializeObject(ResUpdateProject_Log);

                        re = sendStringFrame(app, Res);
                        #endregion
                        break;
                    case 33:
                        #region 工程日志查询
                        SelectProject_log sellog = JsonHelper.DeserializeJsonToObject<SelectProject_log>(strRx);
                        if (sellog.Start_Time == "")
                        {
                            sellog.Start_Time = DateTime.MinValue.ToString();
                        }
                        if (sellog.End_Time == "")
                        {
                            sellog.End_Time = DateTime.MaxValue.ToString();
                        }
                        List<Project_LogModels> ListLog = new List<Project_LogModels>();

                        dstp.ConDBM.WriteLogMutex.WaitOne();
                        AppDAL.SelectProject_Log(sellog.RTU_No, Convert.ToDateTime(sellog.Start_Date), Convert.ToDateTime(sellog.Start_Time), Convert.ToDateTime(sellog.End_Time), out Error, out ErrorMessage, out ListLog);
                        dstp.ConDBM.WriteLogMutex.ReleaseMutex();

                        //将查询到的数据存起来
                        app.SaveProject_Log = new List<Project_LogModels>();
                        app.SaveProject_Log = ListLog;

                        //应答
                        if (Error == 0)
                        {
                            Res3 ResOutLineLog = new Res3();
                            ResOutLineLog.Command_Type = Command.Command_Type;
                            ResOutLineLog.Command_SN = Command.Command_SN;
                            ResOutLineLog.ResultSet_Name = "Project_Log";
                            ResOutLineLog.Rows_Num = ListLog.Count;
                            ResOutLineLog.Error = Error;
                            ResOutLineLog.Error_Message = ErrorMessage;
                            Res = JsonHelper.SerializeObject(ResOutLineLog);

                            if (ListLog.Count == 0)//如果查询出数据条数为0,则直接退出连接
                            {
                                app.bFinish = true;
                            }
                        }
                        else
                        {
                            Res2 FailResOutLineLog = new Res2();
                            FailResOutLineLog.Command_Type = Command.Command_Type;
                            FailResOutLineLog.Command_SN = Command.Command_SN;
                            FailResOutLineLog.Error = Error;
                            FailResOutLineLog.Error_Message = ErrorMessage;
                            Res = JsonHelper.SerializeObject(FailResOutLineLog);
                        }
                        re = sendStringFrame(app, Res);
                        #endregion
                        break;
                    case 34:
                        #region 工程日志删除
                        DeleteProject_log dellog = JsonHelper.DeserializeJsonToObject<DeleteProject_log>(strRx);


                        dstp.ConDBM.WriteLogMutex.WaitOne();
                        AppDAL.DeleteProject_Log(dellog.RTU_No, Convert.ToDateTime(dellog.Start_Date), dellog.Project_Log_No, dellog.Data1, out Error, out ErrorMessage);
                        dstp.ConDBM.WriteLogMutex.ReleaseMutex();

                        //回应
                        Res2 ResDelProject_Log = new Res2();
                        ResDelProject_Log.Command_Type = Command.Command_Type;
                        ResDelProject_Log.Command_SN = Command.Command_SN;
                        ResDelProject_Log.Error = Error;
                        ResDelProject_Log.Error_Message = ErrorMessage;
                        Res = JsonHelper.SerializeObject(ResDelProject_Log);

                        re = sendStringFrame(app, Res);
                        #endregion
                        break;
                    case 41:
                        #region 图片上传
                        AddPic addpic = JsonHelper.DeserializeJsonToObject<AddPic>(strRx);
                        for (int i = 0; i < addpic.Project_Pic.Count; i++)
                        {
                            string Pic_Name = addpic.RTU_No.ToString();
                            Pic_Name += Convert.ToDateTime(addpic.Start_Date).ToString("yyyyMMddHHmmss");
                            Pic_Name += addpic.Project_Pic[i].Pic_Name;
                            Pic_Name += ".jpg";
                            try
                            {
                                //首先查询是否有该工程信息
                                string StrProject_ID = AppDAL.SelProject_ID(Convert.ToDateTime(addpic.Start_Date), addpic.RTU_No);
                                if (StrProject_ID == "")
                                {
                                    Error = 1;
                                    ErrorMessage = "不存在该工程信息，先创建工程信息再上传图片";
                                }
                                else
                                {
                                    //先将图片保存到本地
                                    SavePic(Pic_Name, addpic.Project_Pic[i].Pic_Content);
                                    //更新数据库
                                    AppDAL.UpdateProject_Pic(addpic.Project_Pic[i].Pic_Name, Pic_Name, StrProject_ID, out Error, out ErrorMessage);
                                }

                            }
                            catch (Exception ex)
                            {
                                Error = 3;
                                ErrorMessage = "数据处理异常";

                                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " checkMyFrameType " + ex.Message.ToString());
                            }
                        }

                        //回应
                        Res2 ResUpdateProject_Pic = new Res2();
                        ResUpdateProject_Pic.Command_Type = Command.Command_Type;
                        ResUpdateProject_Pic.Command_SN = Command.Command_SN;
                        ResUpdateProject_Pic.Error = Error;
                        ResUpdateProject_Pic.Error_Message = ErrorMessage;
                        Res = JsonHelper.SerializeObject(ResUpdateProject_Pic);

                        re = sendStringFrame(app, Res);
                        #endregion
                        break;
                    case 61:
                        #region RTU数据查询
                        SelectRTUData selrtu = JsonHelper.DeserializeJsonToObject<SelectRTUData>(strRx);

                        //查询数据库
                        RtuDataModel RtuData = new RtuDataModel();
                        dstp.ConDBM.WriteRTUDataMutex.WaitOne();
                        AppDAL.SelectRTU_Data(selrtu.RTU_No, out Error, out ErrorMessage, out RtuData);
                        dstp.ConDBM.WriteRTUDataMutex.ReleaseMutex();

                        //回应
                        if (Error == 0)
                        {
                            Res5 ResSelRtuData = new Res5();
                            ResSelRtuData.Command_Type = Command.Command_Type;
                            ResSelRtuData.Command_SN = Command.Command_SN;
                            ResSelRtuData.RTU_No = RtuData.RTU_No;
                            ResSelRtuData.Displacement = RtuData.Displacement;
                            ResSelRtuData.Displacement_Acc = RtuData.Displacement_Acc;
                            ResSelRtuData.Pressure = RtuData.Pressure;
                            ResSelRtuData.Detect_Time = RtuData.Detect_Time.ToString();
                            bool IsTrue = false;
                            for(int i=0;i<ConnectToRTU.RTULlist.Count;i++)
                            {
                                if(selrtu.RTU_No== ConnectToRTU.RTULlist[i].RTU_No)
                                {
                                    IsTrue = true;
                                    if(ConnectToRTU.RTULlist[i].RTU_ParaMeter_No==1)
                                    {
                                        ResSelRtuData.RTU_EnableState = 1;
                                    }
                                    else
                                    {
                                        ResSelRtuData.RTU_EnableState = 0;
                                    }
                                }
                            }
                            if(IsTrue)
                            {
                                ResSelRtuData.RTU_Connected = 1;
                            }
                            else
                            {
                                ResSelRtuData.RTU_Connected = 0;
                                ResSelRtuData.RTU_EnableState = 0;
                            }
                            ResSelRtuData.Error = Error;
                            ResSelRtuData.Error_Message = ErrorMessage;
                            Res = JsonHelper.SerializeObject(ResSelRtuData);
                        }
                        else
                        {
                            Res2 FailResSelRtuData = new Res2();
                            FailResSelRtuData.Command_Type = Command.Command_Type;
                            FailResSelRtuData.Command_SN = Command.Command_SN;
                            FailResSelRtuData.Error = Error;
                            FailResSelRtuData.Error_Message = ErrorMessage;
                            Res = JsonHelper.SerializeObject(FailResSelRtuData);
                        }
                        re = sendStringFrame(app, Res);
                        #endregion
                        break;
                    case 52:
                        #region 数据浏览
                        SelectData seldata = JsonHelper.DeserializeJsonToObject<SelectData>(strRx);
                        if (seldata.ResultSet_Name == "Project")//工程概要信息查询
                        {
                            try
                            {
                                List<ProjectModel> NewListProject = new List<ProjectModel>();
                                NewListProject = app.SaveProject.GetRange(seldata.Row_First, seldata.Row_Last - seldata.Row_First + 1);
                                //回应
                                Res6 ResSelectProjectData = new Res6();
                                ResSelectProjectData.Command_Type = Command.Command_Type;
                                ResSelectProjectData.Command_SN = Command.Command_SN;
                                ResSelectProjectData.Error = 0;
                                ResSelectProjectData.Error_Message = "";
                                ResSelectProjectData.ResultSet_Name = seldata.ResultSet_Name;
                                ResSelectProjectData.Rows_Num = NewListProject.Count;
                                ResSelectProjectData.ResultSet_Data = new List<ResultSet_Data1>();
                                for (int i = 0; i < NewListProject.Count; i++)
                                {
                                    ResultSet_Data1 ResultSet_Data1 = new ResultSet_Data1();
                                    ResultSet_Data1.Start_Date = NewListProject[i].Start_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    ResultSet_Data1.RTU_No = NewListProject[i].RTU_No;
                                    ResultSet_Data1.Construction_Well_ID = NewListProject[i].Construction_Well_ID;
                                    if(NewListProject[i].End_Date==DateTime.MinValue)
                                    {
                                        ResultSet_Data1.End_Date = null;
                                    }
                                    else
                                    {
                                        ResultSet_Data1.End_Date = NewListProject[i].End_Date.ToString("yyyy-MM-dd HH:mm:ss");
                                    }
                                    ResSelectProjectData.ResultSet_Data.Add(ResultSet_Data1);
                                    app.ListCountSum++;
                                }
                                Res = JsonHelper.SerializeObject(ResSelectProjectData);
                            }
                            catch (Exception ex)
                            {
                                Res2 FailResSelProjectData = new Res2();
                                FailResSelProjectData.Command_Type = Command.Command_Type;
                                FailResSelProjectData.Command_SN = Command.Command_SN;
                                FailResSelProjectData.Error = 1;
                                FailResSelProjectData.Error_Message = "数据处理异常";
                                Res = JsonHelper.SerializeObject(FailResSelProjectData);

                                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " checkMyFrameType " + ex.Message.ToString());
                            }
                            finally
                            {
                                re = sendStringFrame(app, Res);
                                if (re == true)
                                {
                                    if (app.ListCountSum == app.SaveProject.Count)
                                    {
                                        app.SaveProject = null;
                                        app.ListCountSum = 0;
                                        app.bFinish = true;
                                    }
                                }
                            }
                        }
                        if (seldata.ResultSet_Name == "Project_Log")//工程日志查询
                        {
                            try
                            {
                                List<Project_LogModels> NewListProject_Log = new List<Project_LogModels>();
                                NewListProject_Log = app.SaveProject_Log.GetRange(seldata.Row_First, seldata.Row_Last - seldata.Row_First + 1);

                                //回应
                                Res7 ResSelectLogData = new Res7();
                                ResSelectLogData.Command_Type = Command.Command_Type;
                                ResSelectLogData.Command_SN = Command.Command_SN;
                                ResSelectLogData.Error = 0;
                                ResSelectLogData.Error_Message = "";
                                ResSelectLogData.ResultSet_Name = seldata.ResultSet_Name;
                                ResSelectLogData.Rows_Num = NewListProject_Log.Count;
                                ResSelectLogData.ResultSet_Data = new List<ResultSet_Data2>();
                                for (int i = 0; i < NewListProject_Log.Count; i++)
                                {
                                    ResultSet_Data2 ResultSet_Data2 = new ResultSet_Data2();
                                    ResultSet_Data2.Project_Log_ID = NewListProject_Log[i].Project_Log_ID;
                                    ResultSet_Data2.Project_Log_No = NewListProject_Log[i].Project_Log_No;
                                    ResultSet_Data2.Construction_Time = NewListProject_Log[i].Construction_Time.ToString("yyyy-MM-dd HH:mm:ss");
                                    ResultSet_Data2.Pressure = NewListProject_Log[i].Pressure;
                                    ResultSet_Data2.Displacement = NewListProject_Log[i].Displacement;
                                    ResultSet_Data2.Displacement_Acc = NewListProject_Log[i].Displacement_Acc;
                                    ResultSet_Data2.Slug_Name = NewListProject_Log[i].Slug_Name;
                                    ResultSet_Data2.Formula = NewListProject_Log[i].Formula;
                                    ResultSet_Data2.Events = NewListProject_Log[i].Events;
                                    ResultSet_Data2.Last_Operate_Type = NewListProject_Log[i].Last_Operate_Type;
                                    ResSelectLogData.ResultSet_Data.Add(ResultSet_Data2);
                                    app.ListCountSum++;
                                }
                                Res = JsonHelper.SerializeObject(ResSelectLogData);
                            }
                            catch (Exception ex)
                            {
                                Res2 FailResSelLogData = new Res2();
                                FailResSelLogData.Command_Type = Command.Command_Type;
                                FailResSelLogData.Command_SN = Command.Command_SN;
                                FailResSelLogData.Error = 1;
                                FailResSelLogData.Error_Message = "数据处理异常";
                                Res = JsonHelper.SerializeObject(FailResSelLogData);

                                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " checkMyFrameType " + ex.Message.ToString());
                            }
                            finally
                            {
                                re = sendStringFrame(app, Res);
                                if (re == true)
                                {
                                    if (app.ListCountSum == app.SaveProject_Log.Count)
                                    {
                                        app.SaveProject_Log = null;
                                        app.ListCountSum = 0;
                                        app.bFinish = true;
                                    }
                                }
                            }
                        }
                        #endregion
                        break;
                    default:
                        #region 命令号错误
                        Res2 FailResType = new Res2();
                        FailResType.Command_Type = Command.Command_Type;
                        FailResType.Command_SN = Command.Command_SN;
                        FailResType.Error = 5;
                        FailResType.Error_Message = "消息帧命令号错误";
                        Res = JsonHelper.SerializeObject(FailResType);
                        re = sendStringFrame(app, Res);
                        #endregion
                        break;
                }
                #endregion

                //显示到界面
                if (Error == 0)
                {
                    ErrorMessage = "Success";
                }
                else
                {
                    
                }
                string str = string.Format("   {0}   {1}   {2}", app.client.Client.RemoteEndPoint.ToString(), Command.Command_Type, ErrorMessage);
                dstp.Invoke(writeDebug, new object[] { str, 2 });

            }
            catch (System.Exception ex)
            {
                Res2 FailResType = new Res2();
                FailResType.Command_Type = Command.Command_Type;
                FailResType.Command_SN = Command.Command_SN;
                FailResType.Error = 6;
                FailResType.Error_Message = "消息帧解析失败";
                Res = JsonHelper.SerializeObject(FailResType);
                re = sendStringFrame(app, Res);

                //显示到界面
                string str = string.Format("   {0}   {1}   {2}", app.client.Client.RemoteEndPoint.ToString(), Command.Command_Type, "Fail");
                dstp.Invoke(writeDebug, new object[] { str, 2 });

                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " checkMyFrameType " + ex.Message.ToString());
            }
            finally
            {
                app.LaseSendDT = DateTime.Now;
                if ((Command.Command_Type != 52 && Command.Command_Type != 23 && Command.Command_Type != 33) || Error!=0 )
                {
                    app.bFinish = true;
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
        bool sendStringFrame(APP app, string strCMD)
        {
            bool bRst = false;
            try
            {
                byte[] buffer = new byte[1];
                byte[] msg = Encoding.UTF8.GetBytes(strCMD);

                buffer[0] = 0x02;
                SendToClient(app, buffer);
                SendToClient(app, msg);
                buffer[0] = 0x03;
                SendToClient(app, buffer);
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
        private void SendToClient(APP app, byte[] message)
        {
            try
            {
                //将字符串写入网络流，此方法会自动附加字符串长度前缀
                app.bw.Write(message);
                app.bw.Flush();
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

        ///// <summary>
        ///// 保存图片
        ///// </summary>
        ///// <param name="Pic_Name">图片名称</param>
        ///// <param name="Pic_Content">图片内容</param>
        //public void SavePic(string Pic_Name, string Pic_Content)
        //{
        //    try
        //    {
        //        byte[] imageBytes = Convert.FromBase64String(Pic_Content);
        //        //读入MemoryStream对象
        //        MemoryStream memoryStream = new MemoryStream(imageBytes, 0, imageBytes.Length);
        //        memoryStream.Write(imageBytes, 0, imageBytes.Length);
        //        //转成图片
        //        Image image = Image.FromStream(memoryStream);
        //        image.Save("AppPic//" + Pic_Name, System.Drawing.Imaging.ImageFormat.Jpeg);
        //        //byte[] bytePic_Content = StringToByteArray(Pic_Content);
        //        //Image myImg = BytesToImg2(bytePic_Content);

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}


        /// <summary>
        /// 工程详细信息上传，保存图片到本地AppPic文件夹下
        /// </summary>
        /// <param name="PicName">图片数据库名</param>
        /// <param name="PicContent">图片内容（数据）</param>
        /// <param name="RTU_No">RTU编号</param>
        /// <param name="Start_Date">施工开始时间</param>
        /// <returns>要保存的数据库信息</returns>
        public string SavePics(string PicName, string PicContent, int RTU_No, string Start_Date)
        {
            string Pic_Name = "";//图片名称，eg：123420170323170000Con_Begin_Pic1.jpg
            if (PicContent != "" && PicContent != null)
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
        /// 将图片数据转化为String格式
        /// </summary>
        /// <param name="PicName">图片名称</param>
        /// <returns></returns>
        public string PicToStr(string PicName)
        {
            string TmpPicStr = "";
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
                    DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " PicToStr " + ex.Message.ToString());
                }
                //TmpPicStr = binaryArrayToString(ImageBytes, ImageBytes.Length); //此方法在转化时效率较低，已使用下面的方法
                TmpPicStr = ByteToString(ImageBytes);
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
                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " BytesToImg2 " + ex.Message.ToString());

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
                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " ImgToBytes2 " + ex.Message.ToString());

                return null;
            }

        }
        #endregion

        #region 测试连接是否正常
        // 检查一个Socket是否可连接
        private bool IsSocketConnected(APP app)
        {
            bool blockingState = app.client.Client.Blocking;
            try
            {
                byte[] tmp = new byte[1];
                app.client.Client.Blocking = false;
                app.client.Client.Send(tmp, 0, 0);
                return true;
            }
            catch (SocketException e)
            {
                bool RETURN = true;
                // 产生 10035 == WSAEWOULDBLOCK 错误，说明被阻止了，但是还是连接的
                // 产生 10053 == WSAEWOULDBLOCK 错误，说明由于超时或者其它失败而中止连接
                if (e.NativeErrorCode.Equals(10035) || e.NativeErrorCode.Equals(10053))
                {
                    RETURN = false;
                }

                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " IsSocketConnected " + e.Message.ToString());

                return RETURN;
            }
            finally
            {
                app.client.Client.Blocking = blockingState;    // 恢复状态
            }
        }
        #endregion
    }
}
