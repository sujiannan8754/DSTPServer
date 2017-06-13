using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DSTP_DAL
{
    /// <summary>
    /// APP数据库操作
    /// </summary>
    public class AppDAL
    {
        MySQLHelper MySQLHelper = new MySQLHelper();

        /// <summary>
        /// 查询登录信息
        /// </summary>
        /// <param name="Account_Name">用户名</param>
        /// <param name="Account_Password">密码</param>
        /// <param name="AccountModel">账号模型</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        public void SelectLogin(string Account_Name, string Account_Password, out AccountModel AccountModel, out int Error, out string ErrorMessage)
        {
            AccountModel = new AccountModel();
            Error = 0;
            ErrorMessage = "";
            //先查是否存在用户名
            string SQLString = "select Account_Name,Account_Password,Account_Permission from  Account where Account_Name=@Account_Name";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@Account_Name",Account_Name)
             };
            try
            {
                DataRow Login = MySQLHelper.ExecuteDataTableRow(SQLString, comParamerer);
                if (Login == null)
                {
                    Error = 1;
                    ErrorMessage = "账号不存在";
                }
                else
                {
                    string Name = Login[0].ToString();
                    string PassWord = Login[1].ToString();
                    int Permission = int.Parse(Login[2].ToString());
                    if (PassWord != Account_Password)
                    {
                        Error = 2;
                        ErrorMessage = "密码错误";
                    }
                    else if (Permission != 0 && Permission != 1)
                    {
                        Error = 3;
                        ErrorMessage = "账号未授权";
                    }
                    else
                    {
                        Error = 0;
                        ErrorMessage = "";
                    }
                }
                //确认了登录信息正常，开始查询回应信息
                if (Error == 0)
                {
                    SQLString = "select Account_Name,Account_Password,Name,TEL,Company,Account_Permission  from  Account where Account_Name=@Account_Name and Account_Password=@Account_Password";
                    comParamerer = new MySqlParameter[]
                    {
                        new MySqlParameter("@Account_Name",Account_Name),
                        new MySqlParameter("@Account_Password",Account_Password)
                    };
                    Login = MySQLHelper.ExecuteDataTableRow(SQLString, comParamerer);
                    AccountModel.Account_Name = Login[0].ToString();
                    AccountModel.Account_Password = Login[1].ToString();
                    AccountModel.Name = Login[2].ToString();
                    AccountModel.TEL = Login[3].ToString();
                    AccountModel.Company = Login[4].ToString();
                    AccountModel.Account_Permission = int.Parse(Login[5].ToString());
                }
            }
            catch (Exception)
            {
                Error = 4;
                ErrorMessage = "数据处理异常";
            }
        }

        /// <summary>
        /// 新增工程信息
        /// </summary>
        /// <param name="RTU_No">RTU编号</param>
        /// <param name="Start_Date">施工开始时间</param>
        /// <param name="Construction_Well_ID">井号</param>
        /// <param name="data1">用户名</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        public void AddProject(int RTU_No, DateTime Start_Date, string Construction_Well_ID,string Team_Leader_Name,string Team_Leader_Tel,string Company_Name, string data1, out int Error, out string ErrorMessage)
        {
            Error = 0;
            ErrorMessage = "";
            //首先查询工程信息是否存在
            string SQLString = "select RTU_No,Start_Date,Construction_Well_ID  from  Project where RTU_No=@RTU_No and Start_Date=@Start_Date and Construction_Well_ID=@Construction_Well_ID and Last_Operate_Type <> 2";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@RTU_No",RTU_No),
                new MySqlParameter("@Start_Date",Start_Date),
                new MySqlParameter("@Construction_Well_ID",Construction_Well_ID)
            };

            bool b;
            try
            {
                b = MySQLHelper.ExecuteExists(SQLString, comParamerer);
            }
            catch (Exception ex)
            {
                b = false;
            }
            if (b == true)
            {
                Error = 2;
                ErrorMessage = "该工程信息已存在";
            }
            else //不存在直接向数据库添加数据，这里先不验证是否存在RTU设备了，后期添加上
            {
                //先看RTU是不是存在（4-25修改）
                if (SelectRTU_No(RTU_No) == null)
                {
                    //为空时，证明没这个RTU,不能新增工程信息
                    Error = 1;
                    ErrorMessage = "填写的设备编号不存在";
                }
                else
                {
                    //再验证上次的项目是否结项,或者不存在上次的项目
                    string LastEndDate;
                    bool bCanOperate = false;
                    SelectLastTimeProjectByRTU(RTU_No, out LastEndDate);
                    if (LastEndDate == null)//为空时,证明没这个项目
                    {
                        bCanOperate = true;
                    }
                    else if (Convert.ToDateTime(LastEndDate) != DateTime.MinValue && Convert.ToDateTime(Start_Date) > Convert.ToDateTime(LastEndDate))//上次项目有时间，并且本次项目要比上次项目大的时候才能建新项目
                    {
                        bCanOperate = true;
                    }
                    else
                    {
                        Error = 4;
                        ErrorMessage = "本次新建项目与上次项目发生冲突,检查上次项目是否已结项";
                    }

                    if (bCanOperate == true)
                    {
                        ProjectModel ProjectModel = new ProjectModel();
                        ProjectModel.Project_ID = Guid.NewGuid().ToString();
                        ProjectModel.Account_ID = SelAccount_ID(data1);
                        ProjectModel.Project_No = RTU_No.ToString() + Start_Date.ToString("yyyyMMddHHmmss");
                        ProjectModel.Start_Date = Convert.ToDateTime(Start_Date);
                        ProjectModel.RTU_No = RTU_No;
                        ProjectModel.Construction_Well_ID = Construction_Well_ID;
                        ProjectModel.Team_Leader_Name = Team_Leader_Name;
                        ProjectModel.Team_Leader_Tel = Team_Leader_Tel;
                        ProjectModel.Company_Name = Company_Name;
                        ProjectModel.Sign = 1;
                        ProjectModel.Last_Operate_People = data1;
                        ProjectModel.Last_Operate_Date = DateTime.Now;
                        ProjectModel.Last_Operate_Type = 0;
                        ProjectModel.Project_State = 0;

                        SQLString = "INSERT INTO Project ";
                        SQLString += "(Project_ID,Account_ID,Project_No,Start_Date,RTU_No,Construction_Well_ID,Team_Leader_Name,Team_Leader_Tel,Company_Name,Sign,Last_Operate_People,Last_Operate_Date,Last_Operate_Type,Project_State)";
                        SQLString += " VALUES(@Project_ID, @Account_ID, @Project_No, @Start_Date, @RTU_No, @Construction_Well_ID,@Team_Leader_Name,@Team_Leader_Tel,@Company_Name, @Sign,@Last_Operate_People,@Last_Operate_Date,@Last_Operate_Type,@Project_State); ";
                        comParamerer = new MySqlParameter[]
                        {
                        new MySqlParameter("@Project_ID",ProjectModel.Project_ID),
                        new MySqlParameter("@Account_ID",ProjectModel.Account_ID),
                        new MySqlParameter("@Project_No",ProjectModel.Project_No),
                        new MySqlParameter("@Start_Date",ProjectModel.Start_Date),
                        new MySqlParameter("@RTU_No",ProjectModel.RTU_No),
                        new MySqlParameter("@Construction_Well_ID",ProjectModel.Construction_Well_ID),
                        new MySqlParameter("@Team_Leader_Name",ProjectModel.Team_Leader_Name),
                        new MySqlParameter("@Company_Name",ProjectModel.Company_Name),
                        new MySqlParameter("@Team_Leader_Tel",ProjectModel.Team_Leader_Tel),
                        new MySqlParameter("@Sign",ProjectModel.Sign),
                        new MySqlParameter("@Last_Operate_People",ProjectModel.Last_Operate_People),
                        new MySqlParameter("@Last_Operate_Date",ProjectModel.Last_Operate_Date),
                        new MySqlParameter("@Last_Operate_Type",ProjectModel.Last_Operate_Type),
                        new MySqlParameter("@Project_State",ProjectModel.Project_State)
                        };
                        try
                        {
                            b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
                            if (b == true)
                            {
                                Error = 0;
                                ErrorMessage = "";
                            }
                            else
                            {
                                Error = 3;
                                ErrorMessage = "数据处理异常，重新尝试";
                            }
                        }
                        catch (Exception)
                        {
                            Error = 3;
                            ErrorMessage = "数据处理异常，重新尝试";
                        }
                    }
                }
            }
        }

        private string SelectRTU_No(int RTU_No)
        {
            string SQLString = "select RTU_ParaMeter_ID  from  rtu_parameter where RTU_No=@RTU_No";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@RTU_No",RTU_No)
            };
            string RTU_ParaMeter_ID = MySQLHelper.ExecuteFirst(SQLString, comParamerer);
            if(RTU_ParaMeter_ID == "")
            {
                RTU_ParaMeter_ID = null;
            }
            return RTU_ParaMeter_ID;
        }

        /// <summary>
        /// 根据RTU_No查询上次的施工结束日期
        /// </summary>
        /// <param name="RTU_No"></param>
        /// <param name="End_State">未结束为最小时间，已结束为真正时间，不存在项目为空</param>
        public void SelectLastTimeProjectByRTU(int RTU_No,out string End_Date)
        {
            End_Date = "";
            string SQLString = "select max(End_Date)  from  Project where RTU_No=@RTU_No and Last_Operate_Type <> 2";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@RTU_No",RTU_No)
            };
            try
            {
                End_Date = MySQLHelper.ExecuteFirst(SQLString, comParamerer);
                if(End_Date=="")
                {
                    End_Date = null;
                }
            }
            catch (Exception ex)
            {
                End_Date = null;
            }

            if(End_Date == null)//Project表没这个项目,证明项目已经结项了,查一下历史表
            {
                SQLString = "select max(End_Date)  from  project_history where RTU_No=@RTU_No and Last_Operate_Type <> 2";
                comParamerer = new MySqlParameter[]
                {
                    new MySqlParameter("@RTU_No",RTU_No)
                };
                try
                {
                    End_Date = MySQLHelper.ExecuteFirst(SQLString, comParamerer);
                    if (End_Date == "")
                    {
                        End_Date = null;
                    }
                }
                catch (Exception ex)
                {
                    End_Date = null;
                }
            }
        }


        /// <summary>
        /// 根据用户名查询用户ID
        /// </summary>
        /// <param name="Account_Name">用户名</param>
        /// <returns></returns>
        private string SelAccount_ID(string Account_Name)
        {
            string SQLString = "select Account_ID  from  Account where Account_Name=@Account_Name";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@Account_Name",Account_Name)
            };
            string Account_ID = (string)MySQLHelper.ExecuteFirst(SQLString, comParamerer);
            return Account_ID;
        }

        /// <summary>
        /// 上传工程详细信息
        /// </summary>
        /// <param name="PM">工程信息</param>
        /// <param name="JM">堵剂信息</param>
        /// <param name="data1">用户名</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        public void UpdateProject(ProjectModel PM, List<JamModel> JM, string data1, out int Error, out string ErrorMessage)
        {
            PM.Account_ID = SelAccount_ID(data1);
            PM.Last_Operate_People = data1;
            PM.Last_Operate_Date = DateTime.Now;
            PM.Last_Operate_Type = 1;
            PM.Sign = 1;
            Error = 0;
            ErrorMessage = "";
            //先对工程信息进行存储
            string SQLString = "update Project set ";
            SQLString += "Account_ID=@Account_ID,";
            SQLString += "Jar_Shape=@Jar_Shape,";
            SQLString += "Jar_Size=@Jar_Size,";
            SQLString += "Jar_Volume=@Jar_Volume,";
            SQLString += "Construction_Name=@Construction_Name,";
            SQLString += "Oil_Factory=@Oil_Factory,";
            SQLString += "Area=@Area,";
            SQLString += "Place=@Place,";
            //SQLString += "Team_Leader_Picture=@Team_Leader_Picture,";
            SQLString += "Team_Worker_Name=@Team_Worker_Name,";
            SQLString += "Team_Worker_Tel=@Team_Worker_Tel,";
            //SQLString += "Team_Worker_Picture=@Team_Worker_Picture,";
            SQLString += "Team_Worker_Name=@Team_Worker_Name,";
            SQLString += "Team_Worker_Tel=@Team_Worker_Tel,";
            SQLString += "Company_Name=@Company_Name,";
            //SQLString += "Con_Begin_Pic1=@Con_Begin_Pic1,";
            //SQLString += "Con_Begin_Pic2=@Con_Begin_Pic2,";
            //SQLString += "Con_Begin_Pic3=@Con_Begin_Pic3,";
            //SQLString += "Con_Begin_Pic4=@Con_Begin_Pic4,";
            //SQLString += "Con_Begin_Pic5=@Con_Begin_Pic5,";
            //SQLString += "Con_Begin_Pic6=@Con_Begin_Pic6,";
            //SQLString += "Con_Begin_Pic7=@Con_Begin_Pic7,";
            //SQLString += "Con_Begin_Pic8=@Con_Begin_Pic8,";
            //SQLString += "Con_Begin_Pic9=@Con_Begin_Pic9,";
            SQLString += "Con_Begin_OilPressure=@Con_Begin_OilPressure,";
            SQLString += "Con_Begin_CasingPressure=@Con_Begin_CasingPressure,";
            SQLString += "Con_Begin_Dayinflow=@Con_Begin_Dayinflow,";
            SQLString += "Con_Begin_IsSeparate=@Con_Begin_IsSeparate,";
            SQLString += "Con_Begin_SepPresure=@Con_Begin_SepPresure,";
            SQLString += "End_Date=@End_Date,";
            SQLString += "Con_End_OilPressure=@Con_End_OilPressure,";
            SQLString += "Con_End_CasingPressure=@Con_End_CasingPressure,";
            SQLString += "Con_End_Dayinflow=@Con_End_Dayinflow,";
            SQLString += "CasPre_Cause=@CasPre_Cause,";
            //SQLString += "Con_End_Pic1=@Con_End_Pic1,";
            SQLString += "Sign=@Sign,";
            SQLString += "Last_Operate_People=@Last_Operate_People,";
            SQLString += "Last_Operate_Date=@Last_Operate_Date,";
            SQLString += "Last_Operate_Type=@Last_Operate_Type,";
            SQLString += "Project_State=@Project_State ";
            SQLString += " where Start_Date=@Start_Date and RTU_No=@RTU_No";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@Start_Date",PM.Start_Date),
                new MySqlParameter("@Account_ID",PM.Account_ID),
                new MySqlParameter("@Jar_Shape",PM.Jar_Shape),
                new MySqlParameter("@Jar_Size",PM.Jar_Size),
                new MySqlParameter("@Jar_Volume",PM.Jar_Volume),
                new MySqlParameter("@RTU_No",PM.RTU_No),
                new MySqlParameter("@Construction_Name",PM.Construction_Name),
                new MySqlParameter("@Oil_Factory",PM.Oil_Factory),
                new MySqlParameter("@Area",PM.Area),
                new MySqlParameter("@Place",PM.Place),
                new MySqlParameter("@Team_Leader_Name",PM.Team_Leader_Name),
                new MySqlParameter("@Team_Leader_Tel",PM.Team_Leader_Tel),
                //new MySqlParameter("@Team_Leader_Picture",PM.Team_Leader_Picture),
                new MySqlParameter("@Team_Worker_Name",PM.Team_Worker_Name),
                new MySqlParameter("@Team_Worker_Tel",PM.Team_Worker_Tel),
                //new MySqlParameter("@Team_Worker_Picture",PM.Team_Worker_Picture),
                new MySqlParameter("@Company_Name",PM.Company_Name),
                //new MySqlParameter("@Con_Begin_Pic1",PM.Con_Begin_Pic1),
                //new MySqlParameter("@Con_Begin_Pic2",PM.Con_Begin_Pic2),
                //new MySqlParameter("@Con_Begin_Pic3",PM.Con_Begin_Pic3),
                //new MySqlParameter("@Con_Begin_Pic4",PM.Con_Begin_Pic4),
                //new MySqlParameter("@Con_Begin_Pic5",PM.Con_Begin_Pic5),
                //new MySqlParameter("@Con_Begin_Pic6",PM.Con_Begin_Pic6),
                //new MySqlParameter("@Con_Begin_Pic7",PM.Con_Begin_Pic7),
                //new MySqlParameter("@Con_Begin_Pic8",PM.Con_Begin_Pic8),
                //new MySqlParameter("@Con_Begin_Pic9",PM.Con_Begin_Pic9),
                new MySqlParameter("@Con_Begin_OilPressure",PM.Con_Begin_OilPressure),
                new MySqlParameter("@Con_Begin_CasingPressure",PM.Con_Begin_CasingPressure),
                new MySqlParameter("@Con_Begin_Dayinflow",PM.Con_Begin_Dayinflow),
                new MySqlParameter("@Con_Begin_IsSeparate",PM.Con_Begin_IsSeparate),
                new MySqlParameter("@Con_Begin_SepPresure",PM.Con_Begin_SepPresure),
                new MySqlParameter("@End_Date",PM.End_Date),
                new MySqlParameter("@Con_End_OilPressure",PM.Con_End_OilPressure),
                new MySqlParameter("@Con_End_CasingPressure",PM.Con_End_CasingPressure),
                new MySqlParameter("@Con_End_Dayinflow",PM.Con_End_Dayinflow),
                new MySqlParameter("@CasPre_Cause",PM.CasPre_Cause),
                //new MySqlParameter("@Con_End_Pic1",PM.Con_End_Pic1),
                new MySqlParameter("@Sign",PM.Sign),
                new MySqlParameter("@Last_Operate_People",PM.Last_Operate_People),
                new MySqlParameter("@Last_Operate_Date",PM.Last_Operate_Date),
                new MySqlParameter("@Last_Operate_Type",PM.Last_Operate_Type),
                new MySqlParameter("@Project_State",PM.Project_State)
            };
            try
            {
                bool b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);

                if (b == false)//更新失败
                {
                    Error = 2;
                    ErrorMessage = "验证工程信息格式是否正确";
                }
                else //更新成功开始添加堵剂信息表
                {
                    b = AddJam(JM);
                    if (b == false)
                    {
                        Error = 2;
                        ErrorMessage = "验证堵剂信息格式是否正确";
                    }
                    else
                    {
                        Error = 0;
                        ErrorMessage = "";
                    }
                }
            }
            catch (Exception)
            {
                Error = 3;
                ErrorMessage = "数据处理异常，重新尝试";
            }
        }

        /// <summary>
        /// 查询出工程信息主键
        /// </summary>
        /// <param name="Start_Date">施工开始时间</param>
        /// <param name="RTU_No">RTU编号</param>
        /// <returns></returns>
        public string SelProject_ID(DateTime Start_Date, int RTU_No)
        {
            string SQLString = "select Project_ID  from  Project where Start_Date=@Start_Date and RTU_No=@RTU_No";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@Start_Date",Start_Date),
                new MySqlParameter("@RTU_No",RTU_No)
            };
            string Project_ID = MySQLHelper.ExecuteFirst(SQLString, comParamerer);
            return Project_ID;
        }

        /// <summary>
        /// 更新堵剂信息
        /// </summary>
        /// <param name="JM">堵剂信息</param>
        /// <returns></returns>
        private bool AddJam(List<JamModel> JM)
        {
            bool bRst = false;
            string SQLString = "";
            //先判断堵剂信息是不是空
            if (JM != null && JM.Count!=0)
            {
                //不是空的情况下将所有相关堵剂信息删除重新更新堵剂表
                SQLString = "delete from Jam where Project_ID=@Project_ID";
                MySqlParameter[] comParamerer = new MySqlParameter[]
                {
                    new MySqlParameter("@Project_ID",JM[0].Project_ID)
                };
                bool b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
                if (b == true)
                {
                    int Sum = 0;
                    //重新更新堵剂信息表
                    for (int i = 0; i < JM.Count; i++)
                    {
                        SQLString = "INSERT INTO Jam ";
                        SQLString += "(Jam_ID,Project_ID,Jam_Name,Jam_Num,Sign,Remark1,Remark2,Last_Operate_People,Last_Operate_Date,Last_Operate_Type)";
                        SQLString += " VALUES(@Jam_ID,@Project_ID,@Jam_Name,@Jam_Num,@Sign,@Remark1,@Remark2,@Last_Operate_People,@Last_Operate_Date,@Last_Operate_Type); ";
                        comParamerer = new MySqlParameter[]
                        {
                            new MySqlParameter("@Jam_ID",JM[i].Jam_ID),
                            new MySqlParameter("@Project_ID",JM[i].Project_ID),
                            new MySqlParameter("@Jam_Name",JM[i].Jam_Name),
                            new MySqlParameter("@Jam_Num",JM[i].Jam_Num),
                            new MySqlParameter("@Sign",JM[i].Sign),
                            new MySqlParameter("@Remark1",JM[i].Remark1),
                            new MySqlParameter("@Remark2",JM[i].Remark2),
                            new MySqlParameter("@Last_Operate_People",JM[i].Last_Operate_People),
                            new MySqlParameter("@Last_Operate_Date",JM[i].Last_Operate_Date),
                            new MySqlParameter("@Last_Operate_Type",JM[i].Last_Operate_Type),
                        };
                        try
                        {
                            b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
                            if (b == true)
                            {
                                Sum++;
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                    if (Sum == JM.Count)
                    {
                        bRst = true;
                    }
                    else
                    {
                        bRst = false;
                    }
                }
            }
            else
            {
                bRst = true;
            }
            return bRst;
        }

        /// <summary>
        /// 工程概要信息查询
        /// </summary>
        /// <param name="RTU_No">RTU编号</param>
        /// <param name="Construction_Well_ID">井号</param>
        /// <param name="Company_Name">施工单位名称</param>
        /// <param name="Start_Time">开始时间</param>
        /// <param name="End_Time">结束时间</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        /// <param name="PM">接收查询工程概要信息</param>
        public void SelectOutLineProject(int RTU_No, string Construction_Well_ID, string Company_Name, DateTime Start_Time, DateTime End_Time, out int Error, out string ErrorMessage, out List<ProjectModel> ProjectModel)
        {
            Error = 0;
            ErrorMessage = "";
            ProjectModel = new List<DSTP_DAL.ProjectModel>();
            string SQLString = "select Start_Date,RTU_No,Construction_Well_ID,End_Date from project_history where ";
            if (RTU_No != 0)
            {
                SQLString += "RTU_No=@RTU_No and ";
            }
            if (Construction_Well_ID != "")
            {
                SQLString += " Construction_Well_ID like @Construction_Well_ID and ";
            }
            if (Company_Name != "")
            {
                SQLString += " Company_Name like @Company_Name and ";
            }
            if (Start_Time == End_Time && Start_Time != DateTime.MinValue)
            {
                End_Time = End_Time.AddHours(24);
                SQLString += " Start_Date between @Start_Time and @End_Time and ";
            }
            else if (Start_Time != End_Time)
            {
                SQLString += " Start_Date between @Start_Time and @End_Time and ";
            }
            SQLString += "Last_Operate_Type <> 2";

            SQLString += " UNION ALL ";

            SQLString += "select Start_Date,RTU_No,Construction_Well_ID,End_Date from project where ";
            if (RTU_No != 0)
            {
                SQLString += "RTU_No=@RTU_No and ";
            }
            if (Construction_Well_ID != "")
            {
                SQLString += " Construction_Well_ID like @Construction_Well_ID and ";
            }
            if (Company_Name != "")
            {
                SQLString += " Company_Name like @Company_Name and ";
            }
            if (Start_Time == End_Time && Start_Time != DateTime.MinValue)
            {
                End_Time = End_Time.AddHours(24);
                SQLString += " Start_Date between @Start_Time and @End_Time and ";
            }
            else if (Start_Time != End_Time)
            {
                SQLString += " Start_Date between @Start_Time and @End_Time and ";
            }
            SQLString += "Last_Operate_Type <> 2";

            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@RTU_No",RTU_No),
                new MySqlParameter("@Construction_Well_ID","%"+Construction_Well_ID+"%"),
                new MySqlParameter("@Company_Name","%"+Company_Name+"%"),
                new MySqlParameter("@Start_Time",Start_Time),
                new MySqlParameter("@End_Time",End_Time)
            };
            try
            {
                DataTable dt = MySQLHelper.ExecuteDataTable(SQLString, comParamerer);

                if (dt.Rows.Count == 0)
                {
                    Error = 0;
                    ErrorMessage = "未查询到信息";
                }
                else
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ProjectModel PM = new ProjectModel();
                        PM.Start_Date = Convert.ToDateTime(dt.Rows[i][0].ToString());
                        PM.RTU_No = Convert.ToInt32(dt.Rows[i][1].ToString());
                        PM.Construction_Well_ID = dt.Rows[i][2].ToString();
                        if (dt.Rows[i][3].ToString() == null || dt.Rows[i][3].ToString() == "")
                        {
                            PM.End_Date = DateTime.MinValue;
                        }
                        else
                        {
                            PM.End_Date = Convert.ToDateTime(dt.Rows[i][3].ToString());
                        }
                        ProjectModel.Add(PM);
                    }
                    ProjectModel = ProjectModel.OrderByDescending(a => a.Start_Date).ToList();
                    Error = 0;
                    ErrorMessage = "";
                }
            }
            catch (Exception ex)
            {
                Error = 3;
                ErrorMessage = "数据处理异常，重新尝试";
            }
        }

        /// <summary>
        /// 查询工程详细信息
        /// </summary>
        /// <param name="RTU_No">RTU编号</param>
        /// <param name="Start_Date">施工开始日期</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        /// <param name="Project">接收工程信息</param>
        /// <param name="Jam">接收堵剂信息</param>
        public void SelectDetailedProject(int RTU_No, DateTime Start_Date, out int Error, out string ErrorMessage, out ProjectModel Project, out List<JamModel> Jam)
        {
            Error = 0;
            ErrorMessage = "";
            Project = new ProjectModel();
            Jam = new List<JamModel>();
            string SQLString = "select ";
            SQLString += "Project_ID,Start_Date,";
            SQLString += "Jar_Shape,Jar_Size,Jar_Volume,";
            SQLString += "RTU_No,Construction_Name,Construction_Well_ID,Oil_Factory,Area,Place,Company_Name,";
            SQLString += "Team_Leader_Name,Team_Leader_Tel,Team_Leader_Picture,";
            SQLString += "Team_Worker_Name,Team_Worker_Tel,Team_Worker_Picture,";
            SQLString += "Con_Begin_Pic1,Con_Begin_Pic2,Con_Begin_Pic3,Con_Begin_Pic4,Con_Begin_Pic5,Con_Begin_Pic6,Con_Begin_Pic7,Con_Begin_Pic8,Con_Begin_Pic9,";
            SQLString += "Con_Begin_OilPressure,Con_Begin_CasingPressure,Con_Begin_Dayinflow,Con_Begin_IsSeparate,Con_Begin_SepPresure,";
            SQLString += "End_Date,";
            SQLString += "Con_End_OilPressure,Con_End_CasingPressure,Con_End_Dayinflow,CasPre_Cause,Con_End_Pic1,";
            SQLString += "Project_State ";
            SQLString += "from Project where ";
            SQLString += "RTU_No=@RTU_No and Start_Date=@Start_Date and Last_Operate_Type <> 2";

            SQLString += " UNION ALL ";

            SQLString += "select ";
            SQLString += "Project_ID,Start_Date,";
            SQLString += "Jar_Shape,Jar_Size,Jar_Volume,";
            SQLString += "RTU_No,Construction_Name,Construction_Well_ID,Oil_Factory,Area,Place,Company_Name,";
            SQLString += "Team_Leader_Name,Team_Leader_Tel,Team_Leader_Picture,";
            SQLString += "Team_Worker_Name,Team_Worker_Tel,Team_Worker_Picture,";
            SQLString += "Con_Begin_Pic1,Con_Begin_Pic2,Con_Begin_Pic3,Con_Begin_Pic4,Con_Begin_Pic5,Con_Begin_Pic6,Con_Begin_Pic7,Con_Begin_Pic8,Con_Begin_Pic9,";
            SQLString += "Con_Begin_OilPressure,Con_Begin_CasingPressure,Con_Begin_Dayinflow,Con_Begin_IsSeparate,Con_Begin_SepPresure,";
            SQLString += "End_Date,";
            SQLString += "Con_End_OilPressure,Con_End_CasingPressure,Con_End_Dayinflow,CasPre_Cause,Con_End_Pic1,";
            SQLString += "Project_State ";
            SQLString += "from project_history where ";
            SQLString += "RTU_No=@RTU_No and Start_Date=@Start_Date and Last_Operate_Type <> 2";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@RTU_No",RTU_No),
                new MySqlParameter("@Start_Date",Start_Date)
            };

            try
            {
                DataRow dr = MySQLHelper.ExecuteDataTableRow(SQLString, comParamerer);

                if (dr == null)
                {
                    Error = 1;
                    ErrorMessage = "未查询到信息";
                }
                else
                {
                    #region 接收Project
                    Project.Project_ID = dr["Project_ID"].ToString();
                    Project.Start_Date = Convert.ToDateTime(dr["Start_Date"].ToString());
                    Project.Jar_Shape = dr["Jar_Shape"].ToString();
                    Project.Jar_Size = dr["Jar_Size"].ToString();
                    if(dr["Jar_Volume"].ToString()==null || dr["Jar_Volume"].ToString()=="")
                    {
                        Project.Jar_Volume = 0;
                    }
                    else
                    {
                        Project.Jar_Volume = float.Parse(dr["Jar_Volume"].ToString());
                    }
                    Project.RTU_No = Convert.ToInt32(dr["RTU_No"].ToString());
                    Project.Construction_Name = dr["Construction_Name"].ToString();
                    Project.Construction_Well_ID = dr["Construction_Well_ID"].ToString();
                    Project.Oil_Factory = dr["Oil_Factory"].ToString();
                    Project.Area = dr["Area"].ToString();
                    Project.Place = dr["Place"].ToString();
                    Project.Company_Name = dr["Company_Name"].ToString();
                    Project.Team_Leader_Name = dr["Team_Leader_Name"].ToString();
                    Project.Team_Leader_Tel = dr["Team_Leader_Tel"].ToString();
                    Project.Team_Leader_Picture = dr["Team_Leader_Picture"].ToString();
                    Project.Team_Worker_Name = dr["Team_Worker_Name"].ToString();
                    Project.Team_Worker_Tel = dr["Team_Worker_Tel"].ToString();
                    Project.Team_Worker_Picture = dr["Team_Worker_Picture"].ToString();
                    Project.Con_Begin_Pic1 = dr["Con_Begin_Pic1"].ToString();
                    Project.Con_Begin_Pic2 = dr["Con_Begin_Pic2"].ToString();
                    Project.Con_Begin_Pic3 = dr["Con_Begin_Pic3"].ToString();
                    Project.Con_Begin_Pic4 = dr["Con_Begin_Pic4"].ToString();
                    Project.Con_Begin_Pic5 = dr["Con_Begin_Pic5"].ToString();
                    Project.Con_Begin_Pic6 = dr["Con_Begin_Pic6"].ToString();
                    Project.Con_Begin_Pic7 = dr["Con_Begin_Pic7"].ToString();
                    Project.Con_Begin_Pic8 = dr["Con_Begin_Pic8"].ToString();
                    Project.Con_Begin_Pic9 = dr["Con_Begin_Pic9"].ToString();
                    if (dr["Con_Begin_OilPressure"].ToString() != null && dr["Con_Begin_OilPressure"].ToString() != "")
                    {
                        Project.Con_Begin_OilPressure = float.Parse(dr["Con_Begin_OilPressure"].ToString());
                    }
                    else
                    {
                        Project.Con_Begin_OilPressure = 0;
                    }
                    if (dr["Con_Begin_CasingPressure"].ToString() != null && dr["Con_Begin_CasingPressure"].ToString() != "")
                    {
                        Project.Con_Begin_CasingPressure = float.Parse(dr["Con_Begin_CasingPressure"].ToString());
                    }
                    else
                    {
                        Project.Con_Begin_CasingPressure = 0;
                    }
                    if (dr["Con_Begin_Dayinflow"].ToString() != null && dr["Con_Begin_Dayinflow"].ToString() != "")
                    {
                        Project.Con_Begin_Dayinflow = float.Parse(dr["Con_Begin_Dayinflow"].ToString());
                    }
                    else
                    {
                        Project.Con_Begin_Dayinflow = 0;
                    }
                    if (dr["Con_Begin_IsSeparate"].ToString() != null && dr["Con_Begin_IsSeparate"].ToString() != "")
                    {
                        Project.Con_Begin_IsSeparate = Convert.ToInt32(dr["Con_Begin_IsSeparate"].ToString());
                    }
                    else
                    {
                        Project.Con_Begin_IsSeparate = 0;
                    }
                    if (dr["Con_Begin_SepPresure"].ToString() != null && dr["Con_Begin_SepPresure"].ToString() != "")
                    {
                        Project.Con_Begin_SepPresure = float.Parse(dr["Con_Begin_SepPresure"].ToString());
                    }
                    else
                    {
                        Project.Con_Begin_SepPresure = 0;
                    }
                    if (dr["End_Date"].ToString() != null && dr["End_Date"].ToString() != "")
                    {
                        Project.End_Date = Convert.ToDateTime(dr["End_Date"].ToString());
                    }
                    else
                    {
                        Project.End_Date = DateTime.MinValue;
                    }
                    if (dr["Con_End_OilPressure"].ToString() != null && dr["Con_End_OilPressure"].ToString() != "")
                    {
                        Project.Con_End_OilPressure = float.Parse(dr["Con_End_OilPressure"].ToString());
                    }
                    else
                    {
                        Project.Con_End_OilPressure = 0;
                    }
                    if (dr["Con_End_CasingPressure"].ToString() != null && dr["Con_End_CasingPressure"].ToString() != "")
                    {
                        Project.Con_End_CasingPressure = float.Parse(dr["Con_End_CasingPressure"].ToString());
                    }
                    else
                    {
                        Project.Con_End_CasingPressure = 0;
                    }
                    if (dr["Con_End_Dayinflow"].ToString() != null && dr["Con_End_Dayinflow"].ToString() != "")
                    {
                        Project.Con_End_Dayinflow = float.Parse(dr["Con_End_Dayinflow"].ToString());
                    }
                    else
                    {
                        Project.Con_End_Dayinflow = 0;
                    }
                    Project.CasPre_Cause = dr["CasPre_Cause"].ToString();
                    Project.Con_End_Pic1 = dr["Con_End_Pic1"].ToString();
                    Project.Project_State = Convert.ToInt32(dr["Project_State"].ToString());
                    #endregion

                    //根据Project_ID查出Jam堵剂表并接收
                    SelectJam(Project.Project_ID, out Jam);

                    Error = 0;
                    ErrorMessage = "";
                }
            }
            catch (Exception ex)
            {
                Error = 3;
                ErrorMessage = "数据处理异常，重新尝试";
            }
        }

        /// <summary>
        /// 根据Project_ID查出Jam堵剂表
        /// </summary>
        /// <param name="Project_ID">工程信息主键</param>
        /// <param name="Jam">接收到的堵剂信息</param>
        private void SelectJam(string Project_ID, out List<JamModel> Jam)
        {
            Jam = new List<JamModel>();
            string SQLString = "select ";
            SQLString += "Jam_Name,Jam_Num ";
            SQLString += "from Jam where ";
            SQLString += "Project_ID=@Project_ID ";

            SQLString += " UNION ALL ";

            SQLString += "select ";
            SQLString += "Jam_Name,Jam_Num ";
            SQLString += "from jam_history where ";
            SQLString += "Project_ID=@Project_ID ";

            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@Project_ID",Project_ID)

            };
            try
            {
                DataTable dt = MySQLHelper.ExecuteDataTable(SQLString, comParamerer);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JamModel JamModel = new JamModel();
                    JamModel.Jam_Name = dt.Rows[i][0].ToString();
                    JamModel.Jam_Num = float.Parse(dt.Rows[i][1].ToString());
                    Jam.Add(JamModel);
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 删除工程信息
        /// </summary>
        /// <param name="RTU_No">RTU编号</param>
        /// <param name="Start_Date">施工开始日期</param>
        /// <param name="Project_ID">工程信息表主键，GUID</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        public void DeleteProject(int RTU_No, DateTime Start_Date, string Project_ID, string Data1, out int Error, out string ErrorMessage)
        {
            Error = 0;
            ErrorMessage = "";
            //首先查询该表是否已经在使用，或者是已经结束的工程
            int Project_State = SelectProject_State(RTU_No, Start_Date, Project_ID);
            if (Project_State == 1)
            {
                Error = 2;
                ErrorMessage = "已结束的工程不能删除";
            }
            else
            {
                if(Project_ID==null|| Project_ID=="")
                {
                    Project_ID = SelProject_ID(Start_Date, RTU_No);
                }
                bool bUse = SelectProject_Log(Project_ID);
                if (bUse == true)
                {
                    Error = 3;
                    ErrorMessage = "该工程正被使用，不能删除";
                }
                else
                {
                    //工程没有结束且没有被使用，修改工程最后操作为删除状态。同时将上传标记改为未上传状态
                    DateTime Last_Operate_Date = DateTime.Now;
                    string SQLString = "update Project set ";
                    SQLString += "Sign=@Sign,";
                    SQLString += "Last_Operate_People=@Last_Operate_People,";
                    SQLString += "Last_Operate_Date=@Last_Operate_Date,";
                    SQLString += "Last_Operate_Type=@Last_Operate_Type ";
                    SQLString += "where (RTU_No=@RTU_No and Start_Date=@Start_Date) or Project_ID=@Project_ID";
                    MySqlParameter[] comParamerer = new MySqlParameter[]
                    {
                        new MySqlParameter("@Sign",1),
                        new MySqlParameter("@Last_Operate_People",Data1),
                        new MySqlParameter("@Last_Operate_Date",Last_Operate_Date),
                        new MySqlParameter("@Last_Operate_Type",2),
                        new MySqlParameter("@Start_Date",Start_Date),
                        new MySqlParameter("@RTU_No",RTU_No),
                        new MySqlParameter("@Project_ID",Project_ID)
                    };
                    try
                    {
                        bool b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
                        if (b == true)
                        {
                            Error = 0;
                            ErrorMessage = "";
                        }
                        else
                        {
                            Error = 4;
                            ErrorMessage = "数据处理异常";
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        /// <summary>
        /// 查询工程状态
        /// </summary>
        /// <param name="RTU_No">RTU编号</param>
        /// <param name="Start_Date">施工开始日期</param>
        /// <param name="Project_ID">工程信息表主键，GUID</param>
        /// <returns>Project_State</returns>
        private int SelectProject_State(int RTU_No, DateTime Start_Date, string Project_ID)
        {
            string SQLString = "select ";
            SQLString += "Project_State ";
            SQLString += "from Project where ";
            SQLString += "(RTU_No=@RTU_No and ";
            SQLString += "Start_Date=@Start_Date) or ";
            SQLString += "Project_ID=@Project_ID ";

            SQLString += " UNION ALL ";

            SQLString += "select ";
            SQLString += "Project_State ";
            SQLString += "from project_history where ";
            SQLString += "(RTU_No=@RTU_No and ";
            SQLString += "Start_Date=@Start_Date) or ";
            SQLString += "Project_ID=@Project_ID ";

            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@Project_ID",Project_ID),
                new MySqlParameter("@RTU_No",RTU_No),
                new MySqlParameter("@Start_Date",Start_Date)
            };
            int Project_State = Convert.ToInt32(MySQLHelper.ExecuteFirst(SQLString, comParamerer));
            return Project_State;
        }

        /// <summary>
        /// 查询工程信息是否正在被使用
        /// </summary>
        /// <param name="Project_ID">工程信息主键</param>
        /// <returns></returns>
        private bool SelectProject_Log(string Project_ID)
        {
            bool bUse = false;
            if (Project_ID == "")
            {
                bUse = false;
            }
            else
            {
                string SQLString = "select ";
                SQLString += "Project_Log_ID ";
                SQLString += "from Project_Log where ";
                SQLString += "Project_ID=@Project_ID";

                SQLString += " UNION ALL ";

                SQLString = "select ";
                SQLString += "Project_Log_ID ";
                SQLString += "from project_log_history where ";
                SQLString += "Project_ID=@Project_ID";

                MySqlParameter[] comParamerer = new MySqlParameter[]
                {
                    new MySqlParameter("@Project_ID",Project_ID)
                };
                bUse = MySQLHelper.ExecuteExists(SQLString, comParamerer);
            }
            return bUse;
        }

        /// <summary>
        /// 工程日志查询
        /// </summary>
        /// <param name="RTU_No">RTU编号</param>
        /// <param name="Start_Date">工程开始时间</param>
        /// <param name="Start_Time">施工开始时间</param>
        /// <param name="End_Time">施工结束时间</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        /// <param name="ListLog">接收工程日志数据</param>
        public void SelectProject_Log(int RTU_No, DateTime Start_Date, DateTime Start_Time, DateTime End_Time, out int Error, out string ErrorMessage, out List<Project_LogModels> ListLog)
        {
            Error = 0;
            ErrorMessage = "";
            ListLog = new List<Project_LogModels>();
            //先根据RTU_No ， Start_Date查出工程日志所在的工程信息主键
            string Project_ID = SelProject_ID(Start_Date, RTU_No);
            if (Project_ID == "")
            {
                Error = 2;
                ErrorMessage = "未查询到该工程信息";
            }
            else
            {
                if (Start_Time == End_Time)
                {
                    End_Time = End_Time.AddDays(1);
                }
                string SQLString = "select ";
                SQLString += "Project_Log_ID,Project_Log_No,Construction_Time,Pressure,Displacement,Displacement_Acc,Slug_Name,Formula,Events,Last_Operate_Type ";
                SQLString += "from Project_Log where ";
                SQLString += "Project_ID=@Project_ID and ";
                SQLString += "Construction_Time >= @Start_Time and Construction_Time <= @End_Time";

                SQLString += " UNION ALL ";

                SQLString += "select ";
                SQLString += "Project_Log_ID,Project_Log_No,Construction_Time,Pressure,Displacement,Displacement_Acc,Slug_Name,Formula,Events,Last_Operate_Type ";
                SQLString += "from project_log_history where ";

                SQLString += "Project_ID=@Project_ID and ";
                SQLString += "Construction_Time >= @Start_Time and Construction_Time <= @End_Time";
                MySqlParameter[] comParamerer = new MySqlParameter[]
                {
                    new MySqlParameter("@Project_ID",Project_ID),
                    new MySqlParameter("@Start_Time",Start_Time),
                    new MySqlParameter("@End_Time",End_Time)
                };

                try
                {
                    DataTable dt = MySQLHelper.ExecuteDataTable(SQLString, comParamerer);

                    if (dt.Rows.Count == 0)
                    {
                        Error = 1;
                        ErrorMessage = "未查询到信息";
                    }
                    else
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            Project_LogModels PLM = new Project_LogModels();
                            PLM.Project_Log_ID = dt.Rows[i]["Project_Log_ID"].ToString();
                            PLM.Project_Log_No = Convert.ToInt32(dt.Rows[i]["Project_Log_No"].ToString());
                            PLM.Construction_Time = Convert.ToDateTime(dt.Rows[i]["Construction_Time"].ToString());
                            PLM.Pressure = float.Parse(dt.Rows[i]["Pressure"].ToString());
                            PLM.Displacement = float.Parse(dt.Rows[i]["Displacement"].ToString());
                            PLM.Displacement_Acc = float.Parse(dt.Rows[i]["Displacement_Acc"].ToString());
                            PLM.Slug_Name = dt.Rows[i]["Slug_Name"].ToString();
                            PLM.Formula = dt.Rows[i]["Formula"].ToString();
                            PLM.Events = dt.Rows[i]["Events"].ToString();
                            PLM.Last_Operate_Type= Convert.ToInt32(dt.Rows[i]["Last_Operate_Type"].ToString());
                            ListLog.Add(PLM);
                        }
                        Error = 0;
                        ErrorMessage = "";
                    }
                }
                catch (Exception)
                {
                    Error = 3;
                    ErrorMessage = "数据处理异常，重新尝试";
                }
            }
        }

        /// <summary>
        /// 删除工程日志
        /// </summary>
        /// <param name="RTU_No">RTU编号</param>
        /// <param name="Start_Date">工程开始时间</param>
        /// <param name="Project_Log_No">日志编号</param>
        /// <param name="Data1">用户名</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        public void DeleteProject_Log(int RTU_No, DateTime Start_Date, int Project_Log_No, string Data1, out int Error, out string ErrorMessage)
        {
            Error = 0;
            ErrorMessage = "";

            //先把工程信息主键查出来
            string Project_ID = SelProject_ID(Start_Date, RTU_No);
            if (Project_ID == "")
            {
                Error = 2;
                ErrorMessage = "未查询到该工程信息，查询是否已经上传工程信息";
            }
            else
            {
                //将工程日志改为删除状态，并且将上传状态改为未上传状态
                DateTime Last_Operate_Date = DateTime.Now;
                string SQLString = "update Project_Log set ";
                SQLString += "Sign=@Sign,";
                SQLString += "Last_Operate_People=@Last_Operate_People,";
                SQLString += "Last_Operate_Date=@Last_Operate_Date,";
                SQLString += "Last_Operate_Type=@Last_Operate_Type ";
                SQLString += "where Project_ID=@Project_ID and Project_Log_No=@Project_Log_No";
                MySqlParameter[] comParamerer = new MySqlParameter[]
                {
                        new MySqlParameter("@Sign",1),
                        new MySqlParameter("@Last_Operate_People",Data1),
                        new MySqlParameter("@Last_Operate_Date",Last_Operate_Date),
                        new MySqlParameter("@Last_Operate_Type",2),
                        new MySqlParameter("@Project_ID",Project_ID),
                        new MySqlParameter("@Project_Log_No",Project_Log_No)
                };
                try
                {
                    bool b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
                    if (b == true)
                    {
                        Error = 0;
                        ErrorMessage = "";
                    }
                    else
                    {
                        Error = 4;
                        ErrorMessage = "数据处理异常";
                    }
                }
                catch (Exception ex)
                {
                    Error = 4;
                    ErrorMessage = "数据处理异常";
                }
            }
        }

        /// <summary>
        /// 工程日志上传
        /// </summary>
        /// <param name="AddLog">要新增的工程日志</param>
        /// <param name="UpdateLog">要修改的工程日志</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        public void UpdateProject_Log(List<Project_LogModels> AddLog, List<Project_LogModels> UpdateLog, out int Error, out string ErrorMessage)
        {
            Error = 0;
            ErrorMessage = "";
            try
            {
                //先进行新增
                #region 新增
                for (int i = 0; i < AddLog.Count; i++)
                {
                    //先看看这条信息存在不存在
                    bool bExist = SelectProject_Log(AddLog[i].Project_ID, AddLog[i].Project_Log_No);
                    if (bExist == true)
                    {
                        ErrorMessage += "工程日志:" + AddLog[i].Construction_Time.ToString() + " 已存在.";
                    }
                    else
                    {
                        string SQLString = "INSERT INTO Project_Log ";
                        SQLString += "(Project_Log_ID,Project_ID,Project_Log_No,Construction_Time,Pressure,Displacement,Displacement_Acc,Slug_Name,Formula,Events,Sign,Last_Operate_People,Last_Operate_Date,Last_Operate_Type)";
                        SQLString += " VALUES(@Project_Log_ID,@Project_ID,@Project_Log_No,@Construction_Time,@Pressure,@Displacement,@Displacement_Acc,@Slug_Name,@Formula,@Events,@Sign,@Last_Operate_People,@Last_Operate_Date,@Last_Operate_Type); ";
                        MySqlParameter[] comParamerer = new MySqlParameter[]
                        {
                            new MySqlParameter("@Project_Log_ID",AddLog[i].Project_Log_ID),
                            new MySqlParameter("@Project_ID",AddLog[i].Project_ID),
                            new MySqlParameter("@Project_Log_No",AddLog[i].Project_Log_No),
                            new MySqlParameter("@Construction_Time",AddLog[i].Construction_Time),
                            new MySqlParameter("@Pressure",AddLog[i].Pressure),
                            new MySqlParameter("@Displacement",AddLog[i].Displacement),
                            new MySqlParameter("@Displacement_Acc",AddLog[i].Displacement_Acc),
                            new MySqlParameter("@Slug_Name",AddLog[i].Slug_Name),
                            new MySqlParameter("@Formula",AddLog[i].Formula),
                            new MySqlParameter("@Events",AddLog[i].Events),
                            new MySqlParameter("@Sign",AddLog[i].Sign),
                            new MySqlParameter("@Last_Operate_People",AddLog[i].Last_Operate_People),
                            new MySqlParameter("@Last_Operate_Date",AddLog[i].Last_Operate_Date),
                            new MySqlParameter("@Last_Operate_Type",AddLog[i].Last_Operate_Type),
                        };
                        bool b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
                        if (b == false)
                        {
                            ErrorMessage += "工程日志:" + AddLog[i].Construction_Time.ToString() + " 新增失败.";
                        }
                    }
                }
                #endregion

                //然后修改
                #region 修改
                for (int i = 0; i < UpdateLog.Count; i++)
                {
                    string SQLString = "update Project_Log set ";
                    SQLString += "Construction_Time=@Construction_Time,";
                    SQLString += "Pressure=@Pressure,";
                    SQLString += "Displacement=@Displacement,";
                    SQLString += "Displacement_Acc=@Displacement_Acc,";
                    SQLString += "Slug_Name=@Slug_Name,";
                    SQLString += "Formula=@Formula,";
                    SQLString += "Events=@Events,";
                    SQLString += "Sign=@Sign,";
                    SQLString += "Last_Operate_People=@Last_Operate_People,";
                    SQLString += "Last_Operate_Date=@Last_Operate_Date,";
                    SQLString += "Last_Operate_Type=@Last_Operate_Type ";
                    SQLString += "where Project_ID=@Project_ID and Project_Log_No=@Project_Log_No";
                    MySqlParameter[] comParamerer = new MySqlParameter[]
                    {
                        new MySqlParameter("@Project_ID",UpdateLog[i].Project_ID),
                        new MySqlParameter("@Project_Log_No",UpdateLog[i].Project_Log_No),
                        new MySqlParameter("@Construction_Time",UpdateLog[i].Construction_Time),
                        new MySqlParameter("@Pressure",UpdateLog[i].Pressure),
                        new MySqlParameter("@Displacement",UpdateLog[i].Displacement),
                        new MySqlParameter("@Displacement_Acc",UpdateLog[i].Displacement_Acc),
                        new MySqlParameter("@Slug_Name",UpdateLog[i].Slug_Name),
                        new MySqlParameter("@Formula",UpdateLog[i].Formula),
                        new MySqlParameter("@Events",UpdateLog[i].Events),
                        new MySqlParameter("@Sign",UpdateLog[i].Sign),
                        new MySqlParameter("@Last_Operate_People",UpdateLog[i].Last_Operate_People),
                        new MySqlParameter("@Last_Operate_Date",UpdateLog[i].Last_Operate_Date),
                        new MySqlParameter("@Last_Operate_Type",UpdateLog[i].Last_Operate_Type),
                    };
                    bool b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
                    if (b == false)
                    {
                        ErrorMessage += "工程日志:" + UpdateLog[i].Construction_Time.ToString() + " 修改失败.";
                    }
                }
                #endregion

                if (ErrorMessage != "")
                {
                    Error = 2;
                }
                else
                {
                    Error = 0;
                    ErrorMessage = "";
                }
            }
            catch (Exception ex)
            {
                Error = 3;
                ErrorMessage = "数据处理异常";
            }

        }

        /// <summary>
        /// 查询是否存在工程日志
        /// </summary>
        /// <param name="Project_ID">工程信息主键</param>
        /// <param name="Project_Log_No">工程日志编号</param>
        /// <returns></returns>
        private bool SelectProject_Log(string Project_ID, int Project_Log_No)
        {
            bool bUse = false;
            string SQLString = "select ";
            SQLString += "Project_Log_ID ";
            SQLString += "from Project_Log where ";
            SQLString += "Project_ID=@Project_ID and Project_Log_No=@Project_Log_No";

            SQLString += " UNION ALL ";

            SQLString += "select ";
            SQLString += "Project_Log_ID ";
            SQLString += "from project_log_history where ";
            SQLString += "Project_ID=@Project_ID and Project_Log_No=@Project_Log_No";


            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                    new MySqlParameter("@Project_ID",Project_ID),
                    new MySqlParameter("@Project_Log_No",Project_Log_No)
            };
            bUse = MySQLHelper.ExecuteExists(SQLString, comParamerer);
            return bUse;
        }

        /// <summary>
        /// RTU实时数据查询
        /// </summary>
        /// <param name="RTU_No">RTU编号</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        /// <param name="RtuData">接收查询到的RTU数据</param>
        public void SelectRTU_Data(int RTU_No, out int Error, out string ErrorMessage, out RtuDataModel RtuData)
        {
            Error = 0;
            ErrorMessage = "";
            RtuData = new RtuDataModel();

            string SQLString = "select ";
            SQLString += "RTU_No,";
            SQLString += "Displacement,";
            SQLString += "Displacement_Acc,";
            SQLString += "Pressure,";
            SQLString += "Detect_Time ";
            SQLString += "from RTU_Data where ";
            SQLString += "RTU_No=@RTU_No ";
            SQLString += "ORDER BY Detect_Time DESC LIMIT 1 ";

            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@RTU_No",RTU_No)
            };

            try
            {
                DataRow dr = MySQLHelper.ExecuteDataTableRow(SQLString, comParamerer);
                if (Convert.ToInt32(dr["RTU_No"].ToString()) == 0)
                {
                    Error = 0;
                    ErrorMessage = "该RTU无数据，尝试稍后再查询";
                }
                else
                {
                    RtuData.RTU_No = Convert.ToInt32(dr["RTU_No"].ToString());
                    RtuData.Displacement = float.Parse(dr["Displacement"].ToString());
                    RtuData.Displacement_Acc = float.Parse(dr["Displacement_Acc"].ToString());
                    RtuData.Pressure = float.Parse(dr["Pressure"].ToString());
                    RtuData.Detect_Time = Convert.ToDateTime(dr["Detect_Time"].ToString());

                    Error = 0;
                    ErrorMessage = "";
                }
            }
            catch (Exception ex)
            {
                Error = 2;
                ErrorMessage = "数据处理异常";
            }

        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="PicName">图片数据库名</param>
        /// <param name="PicContent">图片地址</param>
        /// <param name="Project_ID">工程ID</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        public void UpdateProject_Pic(string PicName, string PicContent, string Project_ID, out int Error, out string ErrorMessage)
        {
            Error = 0;
            ErrorMessage = "";
            string SQLString = "update Project set ";
            if (PicName == "Team_Leader_Picture")
            {
                SQLString += "Team_Leader_Picture=@PicContent ";
            }
            if (PicName == "Team_Worker_Picture")
            {
                SQLString += "Team_Worker_Picture=@PicContent ";
            }
            for (int i = 1; i < 10; i++)
            {
                if (PicName == "Con_Begin_Pic" + i.ToString())
                {
                    SQLString += "Con_Begin_Pic" + i.ToString() + "=@PicContent ";
                }
            }
            if (PicName == "Con_End_Pic1")
            {
                SQLString += "Con_End_Pic1=@PicContent ";
            }
            SQLString += "where Project_ID=@Project_ID";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                        new MySqlParameter("@PicName",PicName),
                        new MySqlParameter("@PicContent",PicContent),
                        new MySqlParameter("@Project_ID",Project_ID)
            };
            try
            {
                bool b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
                if (b == false)
                {
                    Error = 2;
                    ErrorMessage = "图像存储失败";
                }
                else
                {
                    Error = 0;
                    ErrorMessage = "";
                }
            }
            catch (Exception ex)
            {
                Error = 3;
                ErrorMessage = "数据处理异常";
            }
        }


        /// <summary>
        /// 工程信息图片查询
        /// </summary>
        /// <param name="RTU_No">RTU编号</param>
        /// <param name="Start_Date">工程开始时间</param>
        /// <param name="Project_Pic_Name">图片所在数据库中的字段名称</param>
        /// <param name="Project_Pic_Content">图片地址</param>
        public void SelectProject_Pic(int RTU_No,string Start_Date,string Project_Pic_Name, out string Project_Pic_Content,out int Error,out string ErrorMessage)
        {
            Error = 0;
            ErrorMessage = "";
            Project_Pic_Content = "";
            string SQLString = "select ";
            SQLString += Project_Pic_Name;
            SQLString += " from project_history where ";
            SQLString += "RTU_No=@RTU_No and Start_Date=@Start_Date and Last_Operate_Type <> 2";

            SQLString += " UNION ALL ";

            SQLString += "select ";
            SQLString += Project_Pic_Name;
            SQLString += " from project where ";
            SQLString += "RTU_No=@RTU_No and Start_Date=@Start_Date and Last_Operate_Type <> 2";

            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@RTU_No",RTU_No),
                new MySqlParameter("@Start_Date",Start_Date)
            };
            try
            {
                Project_Pic_Content = MySQLHelper.ExecuteFirst(SQLString, comParamerer);
                if(Project_Pic_Content!=null && Project_Pic_Content != "")
                {
                    Error = 0;
                    ErrorMessage = "";
                }
                else
                {
                    Error = 2;
                    ErrorMessage = "验证格式是否正确";
                }
            }
            catch (Exception ex)
            {
                Project_Pic_Content = "";
                Error = 3;
                ErrorMessage = "数据处理异常";
            }
        }


        #region 分表
        /// <summary>
        ///工程结束，保存数据到历史表中
        /// </summary>
        /// <param name="Project_ID">工程信息主键</param>
        /// <param name="Rtu_No">RTU编号</param>
        public void SaveTable(string Project_ID, int Rtu_No)
        {
            bool bRst = SaveProject(Project_ID);
            if (bRst == true)
            {
                bRst = DeleteProject(Project_ID);
            }
            bRst = SaveJam(Project_ID);
            if (bRst == true)
            {
                bRst = DeleteJam(Project_ID);
            }
            bRst = SaveProject_Log(Project_ID);
            if (bRst == true)
            {
                bRst = DeleteProject_Log(Project_ID);
            }
            bRst = SaveRtu_Data(Rtu_No);
            if (bRst == true)
            {
                bRst = DeleteRTU_Data(Rtu_No);
            }
            bRst = SaveRtu_Pic(Rtu_No);
            if (bRst == true)
            {
                bRst = DeleteRTU_Pic(Rtu_No);
            }
        }

        #region 导表以及删除原数据
        /// <summary>
        /// 工程信息表分表
        /// </summary>
        /// <param name="Project_ID">工程信息表主键</param>
        /// <returns></returns>
        private bool SaveProject(string Project_ID)
        {
            bool bRst = false;
            string SQLString = "Insert into project_history(Project_ID,Account_ID,Project_No,Start_Date,Jar_Shape,Jar_Size,Jar_Volume,RTU_No,Construction_Name,Construction_Well_ID,Oil_Factory,Area,Place,Team_Leader_Name,Team_Leader_Tel,Team_Leader_Picture,Team_Worker_Name,Team_Worker_Tel,Team_Worker_Picture,Con_Begin_Pic1,Con_Begin_Pic2,Con_Begin_Pic3,Con_Begin_Pic4,Con_Begin_Pic5,Con_Begin_Pic6,Con_Begin_Pic7,Con_Begin_Pic8,Con_Begin_Pic9,Con_Begin_OilPressure,Con_Begin_CasingPressure,Con_Begin_Dayinflow,Con_Begin_IsSeparate,Con_Begin_SepPresure,End_Date,Con_End_OilPressure,Con_End_CasingPressure,Con_End_Dayinflow,CasPre_Cause,Con_End_Pic1,Sign,Last_Operate_People,Last_Operate_Date,Last_Operate_Type,Project_State) ";
            SQLString += "SELECT Project_ID,Account_ID,Project_No,Start_Date,Jar_Shape,Jar_Size,Jar_Volume,RTU_No,Construction_Name,Construction_Well_ID,Oil_Factory,Area,Place,Team_Leader_Name,Team_Leader_Tel,Team_Leader_Picture,Team_Worker_Name,Team_Worker_Tel,Team_Worker_Picture,Con_Begin_Pic1,Con_Begin_Pic2,Con_Begin_Pic3,Con_Begin_Pic4,Con_Begin_Pic5,Con_Begin_Pic6,Con_Begin_Pic7,Con_Begin_Pic8,Con_Begin_Pic9,Con_Begin_OilPressure,Con_Begin_CasingPressure,Con_Begin_Dayinflow,Con_Begin_IsSeparate,Con_Begin_SepPresure,End_Date,Con_End_OilPressure,Con_End_CasingPressure,Con_End_Dayinflow,CasPre_Cause,Con_End_Pic1,Sign,Last_Operate_People,Last_Operate_Date,Last_Operate_Type,Project_State ";
            SQLString += "from project ";
            SQLString += "where Project_ID=@Project_ID ";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@Project_ID",Project_ID)
            };

            try
            {
                bRst = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
            }
            catch (Exception ex)
            {
                bRst = false;
            }
            return bRst;
        }

        /// <summary>
        /// 堵剂表分表
        /// </summary>
        /// <param name="Project_ID">工程信息主键</param>
        /// <returns></returns>
        private bool SaveJam(string Project_ID)
        {
            bool bRst = false;
            string SQLString = "Insert into jam_history(Jam_ID,Project_ID,Jam_Name,Jam_Num,Sign,Remark1,Remark2,Last_Operate_People,Last_Operate_Date,Last_Operate_Type) ";
            SQLString += "SELECT Jam_ID,Project_ID,Jam_Name,Jam_Num,Sign,Remark1,Remark2,Last_Operate_People,Last_Operate_Date,Last_Operate_Type ";
            SQLString += "from jam ";
            SQLString += "where jam.Project_ID=@Project_ID ";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@Project_ID",Project_ID)
            };

            try
            {
                bRst = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
            }
            catch (Exception ex)
            {
                bRst = false;
            }
            return bRst;
        }

        /// <summary>
        /// 工程日志分表
        /// </summary>
        /// <param name="Project_ID">工程信息表主键</param>
        /// <returns></returns>
        private bool SaveProject_Log(string Project_ID)
        {
            bool bRst = false;
            string SQLString = "Insert into project_log_history(Project_Log_ID,Project_ID,Project_Log_No,Construction_Time,Pressure,Displacement,Displacement_Acc,Slug_Name,Formula,Events,Sign,Remark1,Remark2,Last_Operate_People,Last_Operate_Date,Last_Operate_Type) ";
            SQLString += "SELECT Project_Log_ID,Project_ID,Project_Log_No,Construction_Time,Pressure,Displacement,Displacement_Acc,Slug_Name,Formula,Events,Sign,Remark1,Remark2,Last_Operate_People,Last_Operate_Date,Last_Operate_Type ";
            SQLString += "from project_log ";
            SQLString += "where project_log.Project_ID=@Project_ID ";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@Project_ID",Project_ID)
            };

            try
            {
                bRst = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
            }
            catch (Exception ex)
            {
                bRst = false;
            }
            return bRst;
        }

        /// <summary>
        /// RTU数据表分表
        /// </summary>
        /// <param name="RTU_No">RTU编号</param>
        /// <returns></returns>
        private bool SaveRtu_Data(int RTU_No)
        {
            bool bRst = false;
            string SQLString = "Insert into rtu_data_history(RTU_Data_ID,RTU_No,Displacement,Displacement_Acc,Pressure,Conductance_Ratio,Tem_InBox,Tem_OutSide,Detect_Time,Upload_Time,Sign) ";
            SQLString += "SELECT RTU_Data_ID,RTU_No,Displacement,Displacement_Acc,Pressure,Conductance_Ratio,Tem_InBox,Tem_OutSide,Detect_Time,Upload_Time,Sign ";
            SQLString += "from rtu_data ";
            SQLString += "where rtu_data.RTU_No=@RTU_No ";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@RTU_No",RTU_No)
            };

            try
            {
                bRst = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
            }
            catch (Exception ex)
            {
                bRst = false;
            }
            return bRst;
        }

        /// <summary>
        /// RTU图片表分表
        /// </summary>
        /// <param name="RTU_No">RTU编号</param>
        /// <returns></returns>
        private bool SaveRtu_Pic(int RTU_No)
        {
            bool bRst = false;
            string SQLString = "Insert into rtu_pic_history(RTU_Pic_ID,Pic_name,RTU_No,RTU_Pic_Address,Detect_Time,Upload_Time,Sign) ";
            SQLString += "SELECT RTU_Pic_ID,Pic_name,RTU_No,RTU_Pic_Address,Detect_Time,Upload_Time,Sign ";
            SQLString += "from rtu_pic ";
            SQLString += "where rtu_pic.RTU_No=@RTU_No ";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@RTU_No",RTU_No)
            };

            try
            {
                bRst = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
            }
            catch (Exception ex)
            {
                bRst = false;
            }
            return bRst;
        }

        /// <summary>
        /// 删除工程信息表数据
        /// </summary>
        /// <param name="Project_ID">工程信息表主键</param>
        /// <returns></returns>
        private bool DeleteProject(string Project_ID)
        {
            bool bRst = false;
            string SQLString = "delete from Project where Project_ID=@Project_ID";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@Project_ID",Project_ID)
            };
            try
            {
                bRst = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
            }
            catch (Exception ex)
            {
                bRst = false;
            }
            return bRst;
        }

        /// <summary>
        /// 删除堵剂表数据
        /// </summary>
        /// <param name="Project_ID">工程信息表主键</param>
        /// <returns></returns>
        private bool DeleteJam(string Project_ID)
        {
            bool bRst = false;
            string SQLString = "delete from Jam where Project_ID=@Project_ID";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@Project_ID",Project_ID)
            };
            try
            {
                bRst = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
            }
            catch (Exception ex)
            {
                bRst = false;
            }
            return bRst;
        }

        /// <summary>
        /// 删除工程日志表数据
        /// </summary>
        /// <param name="Project_ID">工程信息表主键</param>
        /// <returns></returns>
        private bool DeleteProject_Log(string Project_ID)
        {
            bool bRst = false;
            string SQLString = "delete from Project_Log where Project_ID=@Project_ID";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@Project_ID",Project_ID)
            };
            try
            {
                bRst = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
            }
            catch (Exception ex)
            {
                bRst = false;
            }
            return bRst;
        }

        /// <summary>
        /// 删除RTU数据表数据
        /// </summary>
        /// <param name="RTU_No">RTU编号</param>
        /// <returns></returns>
        private bool DeleteRTU_Data(int RTU_No)
        {
            bool bRst = false;
            string SQLString = "delete from RTU_Data where RTU_No=@RTU_No";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@RTU_No",RTU_No)
            };
            try
            {
                bRst = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
            }
            catch (Exception ex)
            {
                bRst = false;
            }
            return bRst;
        }

        /// <summary>
        /// 删除RTU图片表数据
        /// </summary>
        /// <param name="RTU_No">RTU编号</param>
        /// <returns></returns>
        private bool DeleteRTU_Pic(int RTU_No)
        {
            bool bRst = false;
            string SQLString = "delete from RTU_Pic where RTU_No=@RTU_No";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@RTU_No",RTU_No)
            };
            try
            {
                bRst = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
            }
            catch (Exception ex)
            {
                bRst = false;
            }
            return bRst;
        }
        #endregion

        #endregion
    }
}
