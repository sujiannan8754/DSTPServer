using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DSTP_DAL
{
    /// <summary>
    /// 内网数据库操作类
    /// </summary>
    public class LanDAL
    {
        MySQLHelper MySQLHelper = new MySQLHelper();

        /// <summary>
        /// 查询登录信息
        /// </summary>
        /// <param name="Account_Name">用户名</param>
        /// <param name="Account_Password">密码</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        public void SelectLogin(string Account_Name, string Account_Password, out int Error, out string ErrorMessage)
        {
            Error = 0;
            ErrorMessage = "";
            //先查是否存在用户名
            string SQLString = "select Account_Name,Account_Password,Account_Permission from  Account where Account_Name=@Account_Name and Account_Permission=2";
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
                    else if (Permission != 2)
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
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// APP账号信息查询
        /// </summary>
        /// <param name="Data_Tag">查询标记，表示是否全部接收数据；0为全部接收，1为接收未上传数据</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        /// <param name="AccountModel">APP账号信息</param>
        public void SelectAccount(int Data_Tag, out int Error, out string ErrorMessage, out List<AccountModel> AccountModel)
        {
            Error = 0;
            ErrorMessage = "";
            AccountModel = new List<DSTP_DAL.AccountModel>();
            string SQLString = "select Account_ID,Account_Name,Account_Password,Account_Time,Name,TEL,Company,Account_Picture,Account_Permission,Last_Operate_Date,Last_Operate_Type,Account_IsUse from Account ";
            SQLString += " where Account_Permission <> 2 ";
            if (Data_Tag == 1)
            {
                SQLString += " and Sign=1 ";
            }
            else if (Data_Tag == 0)
            {
                SQLString += " and 1=1 ";
            }

            try
            {
                DataTable dt = MySQLHelper.ExecuteDataTable(SQLString);

                if (dt.Rows.Count == 0)
                {
                    Error = 0;
                    ErrorMessage = "未查询到信息";
                }
                else
                {
                    AccountModel = ModelConvertHelper<AccountModel>.ConvertToModel(dt);
                    Error = 0;
                    ErrorMessage = "";
                }
            }
            catch (Exception ex)
            {
                Error = 1;
                ErrorMessage = "数据处理异常，重新尝试";
            }
        }

        /// <summary>
        /// 工程信息查询
        /// </summary>
        /// <param name="RTU_No">RTU编号</param>
        /// <param name="Construction_Well_ID">井号</param>
        /// <param name="Start_Time">施工开始时间</param>
        /// <param name="End_Time">施工结束时间</param>
        /// <param name="Data_Tag">查询标记，表示是否全部接收数据；0为全部接收，1为接收未上传数据</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        /// <param name="ProjectModel">返回工程信息数据</param>
        public void SelectProject(int RTU_No, string Construction_Well_ID, DateTime Start_Time, DateTime End_Time, int Data_Tag, out int Error, out string ErrorMessage, out List<ProjectModel> ProjectModel)
        {
            Error = 0;
            ErrorMessage = "";
            ProjectModel = new List<DSTP_DAL.ProjectModel>();
            string SQLString = "select ";
            SQLString += "Project_ID,Account_ID,Start_Date,";
            SQLString += "Jar_Shape,Jar_Size,Jar_Volume,";
            SQLString += "RTU_No,Construction_Name,Construction_Well_ID,Oil_Factory,Area,Place,Company_Name,";
            SQLString += "Team_Leader_Name,Team_Leader_Tel,Team_Leader_Picture,";
            SQLString += "Team_Worker_Name,Team_Worker_Tel,Team_Worker_Picture,";
            SQLString += "Con_Begin_Pic1,Con_Begin_Pic2,Con_Begin_Pic3,Con_Begin_Pic4,Con_Begin_Pic5,Con_Begin_Pic6,Con_Begin_Pic7,Con_Begin_Pic8,Con_Begin_Pic9,";
            SQLString += "Con_Begin_OilPressure,Con_Begin_CasingPressure,Con_Begin_Dayinflow,Con_Begin_IsSeparate,Con_Begin_SepPresure,";
            SQLString += "End_Date,";
            SQLString += "Con_End_OilPressure,Con_End_CasingPressure,Con_End_Dayinflow,CasPre_Cause,Con_End_Pic1,";
            SQLString += "Last_Operate_Type,Last_Operate_Date ";
            SQLString += "from Project where ";
            if (RTU_No != 0)
            {
                SQLString += "RTU_No=@RTU_No and ";
            }
            if (Construction_Well_ID != "" && Construction_Well_ID != null)
            {
                SQLString += "Construction_Well_ID=@Construction_Well_ID and ";
            }
            if (Start_Time == End_Time && Start_Time == DateTime.MinValue)
            {
                Start_Time = DateTime.MinValue;
                End_Time = DateTime.MaxValue;
            }
            if (Start_Time == End_Time)
            {
                End_Time = End_Time.AddDays(1);
            }
            SQLString += "Start_Date >= @Start_Time and Start_Date <= @End_Time and ";
            if (Data_Tag == 1)
            {
                SQLString += " Sign=1 ";
            }
            else if (Data_Tag == 0)
            {
                SQLString += " 1=1 ";
            }

            SQLString += " UNION ALL ";

            SQLString += "select ";
            SQLString += "Project_ID,Account_ID,Start_Date,";
            SQLString += "Jar_Shape,Jar_Size,Jar_Volume,";
            SQLString += "RTU_No,Construction_Name,Construction_Well_ID,Oil_Factory,Area,Place,Company_Name,";
            SQLString += "Team_Leader_Name,Team_Leader_Tel,Team_Leader_Picture,";
            SQLString += "Team_Worker_Name,Team_Worker_Tel,Team_Worker_Picture,";
            SQLString += "Con_Begin_Pic1,Con_Begin_Pic2,Con_Begin_Pic3,Con_Begin_Pic4,Con_Begin_Pic5,Con_Begin_Pic6,Con_Begin_Pic7,Con_Begin_Pic8,Con_Begin_Pic9,";
            SQLString += "Con_Begin_OilPressure,Con_Begin_CasingPressure,Con_Begin_Dayinflow,Con_Begin_IsSeparate,Con_Begin_SepPresure,";
            SQLString += "End_Date,";
            SQLString += "Con_End_OilPressure,Con_End_CasingPressure,Con_End_Dayinflow,CasPre_Cause,Con_End_Pic1,";
            SQLString += "Last_Operate_Type,Last_Operate_Date ";
            SQLString += "from project_history where ";
            if (RTU_No != 0)
            {
                SQLString += "RTU_No=@RTU_No and ";
            }
            if (Construction_Well_ID != "" && Construction_Well_ID != null)
            {
                SQLString += "Construction_Well_ID=@Construction_Well_ID and ";
            }
            if (Start_Time == End_Time && Start_Time == DateTime.MinValue)
            {
                Start_Time = DateTime.MinValue;
                End_Time = DateTime.MaxValue;
            }
            if (Start_Time == End_Time)
            {
                End_Time = End_Time.AddDays(1);
            }
            SQLString += "Start_Date >= @Start_Time and Start_Date <= @End_Time and ";
            if (Data_Tag == 1)
            {
                SQLString += " Sign=1 ";
            }
            else if (Data_Tag == 0)
            {
                SQLString += " 1=1 ";
            }
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@RTU_No",RTU_No),
                new MySqlParameter("@Construction_Well_ID",Construction_Well_ID),
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
                    ProjectModel = ModelConvertHelper<ProjectModel>.ConvertToModel(dt);
                    Error = 0;
                    ErrorMessage = "";
                }
            }
            catch (Exception ex)
            {
                Error = 1;
                ErrorMessage = "数据处理异常，重新尝试";
            }

        }

        /// <summary>
        /// 工程日志上传
        /// </summary>
        /// <param name="RTU_No">RTU编号</param>
        /// <param name="Construction_Well_ID">井号</param>
        /// <param name="Start_Date">施工开始时间</param>
        /// <param name="Start_Time">施工时间</param>
        /// <param name="End_Time">施工时间</param>
        /// <param name="Data_Tag">查询标记，表示是否全部接收数据；0为全部接收，1为接收未上传数据</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        /// <param name="ListLog">返回查询到的工程日志数据</param>
        public void SelectProject_Log(int RTU_No, string Construction_Well_ID, DateTime Start_Date, DateTime Start_Time, DateTime End_Time, int Data_Tag, out int Error, out string ErrorMessage, out List<Project_LogModels> ListLog)
        {
            Error = 0;
            ErrorMessage = "";
            ListLog = new List<Project_LogModels>();
            //先看看是不是按照工程信息查询工程日志，是则查出工程信息ID,以Start_Date是否为最小日期为标识
            string Project_ID = "";
            if (Start_Date != DateTime.MinValue)
            {
                Project_ID = SelProject_ID(Start_Date, RTU_No, Construction_Well_ID);
            }

            //将Start_Time以及End_Time格式标准化
            if (Start_Time == End_Time && Start_Time == DateTime.MinValue)
            {
                Start_Time = DateTime.MinValue;
                End_Time = DateTime.MaxValue;
            }
            if (Start_Time == End_Time)
            {
                End_Time = End_Time.AddDays(1);
            }

            string SQLString = "select ";
            SQLString += "Project_Log_ID,Project_ID,Construction_Time,Pressure,Displacement,Displacement_Acc,Slug_Name,Formula,Events,Last_Operate_Type,Last_Operate_Date ";
            SQLString += "from Project_Log where ";
            if (Project_ID != "")
            {
                SQLString += "Project_ID=@Project_ID and ";
            }
            SQLString += "Construction_Time >= @Start_Time and Construction_Time <= @End_Time and ";
            if (Data_Tag == 1)
            {
                SQLString += " Sign=1 ";
            }
            else if (Data_Tag == 0)
            {
                SQLString += " 1=1 ";
            }

            SQLString += " UNION ALL ";

            SQLString += "select ";
            SQLString += "Project_Log_ID,Project_ID,Construction_Time,Pressure,Displacement,Displacement_Acc,Slug_Name,Formula,Events,Last_Operate_Type,Last_Operate_Date ";
            SQLString += "from project_log_history where ";
            if (Project_ID != "")
            {
                SQLString += "Project_ID=@Project_ID and ";
            }
            SQLString += "Construction_Time >= @Start_Time and Construction_Time <= @End_Time and ";
            if (Data_Tag == 1)
            {
                SQLString += " Sign=1 ";
            }
            else if (Data_Tag == 0)
            {
                SQLString += " 1=1 ";
            }
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
                    Error = 0;
                    ErrorMessage = "未查询到信息";
                }
                else
                {
                    ListLog = ModelConvertHelper<Project_LogModels>.ConvertToModel(dt);
                    Error = 0;
                    ErrorMessage = "";
                }
            }
            catch (Exception ex)
            {
                Error = 1;
                ErrorMessage = "数据处理异常，重新尝试";
            }
        }

        /// <summary>
        /// 查询出工程信息主键
        /// </summary>
        /// <param name="Start_Date">施工开始时间</param>
        /// <param name="RTU_No">RTU编号</param>
        /// <returns>工程信息ID</returns>
        public string SelProject_ID(DateTime Start_Date, int RTU_No, string Construction_Well_ID)
        {
            try
            {
                string SQLString = "select Project_ID  from  Project where ";
                if (RTU_No != 0)
                {
                    SQLString += "RTU_No=@RTU_No and ";
                }
                if (Construction_Well_ID != "" && Construction_Well_ID != null)
                {
                    SQLString += "Construction_Well_ID=@Construction_Well_ID and ";
                }
                SQLString += "Start_Date=@Start_Date";

                SQLString += " UNION ALL ";

                SQLString = "select Project_ID  from  project_history where ";
                if (RTU_No != 0)
                {
                    SQLString += "RTU_No=@RTU_No and ";
                }
                if (Construction_Well_ID != "" && Construction_Well_ID != null)
                {
                    SQLString += "Construction_Well_ID=@Construction_Well_ID and ";
                }
                SQLString += "Start_Date=@Start_Date";

                MySqlParameter[] comParamerer = new MySqlParameter[]
                {
                new MySqlParameter("@Start_Date",Start_Date),
                new MySqlParameter("@Construction_Well_ID",Construction_Well_ID),
                new MySqlParameter("@RTU_No",RTU_No)
                };
                string Project_ID = MySQLHelper.ExecuteFirst(SQLString, comParamerer);
                return Project_ID;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// RTU数据上传
        /// </summary>
        /// <param name="RTU_No">RTU编号</param>
        /// <param name="Start_Time">检测时间</param>
        /// <param name="End_Time">检测时间</param>
        /// <param name="Data_Tag">查询标记，表示是否全部接收数据；0为全部接收，1为接收未上传数据</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        /// <param name="ListRtuData">返回查询到的RTU数据</param>
        public void SelRtuData(int RTU_No, DateTime Start_Time, DateTime End_Time, int Data_Tag, out int Error, out string ErrorMessage, out List<RtuDataModel> ListRtuData)
        {
            Error = 0;
            ErrorMessage = "";
            ListRtuData = new List<RtuDataModel>();

            //先将时间标准化
            if (Start_Time == End_Time && Start_Time == DateTime.MinValue)
            {
                Start_Time = DateTime.MinValue;
                End_Time = DateTime.MaxValue;
            }
            if (Start_Time == End_Time)
            {
                End_Time = End_Time.AddDays(1);
            }

            string SQLString = "select ";
            SQLString += "RTU_Data_ID,";
            SQLString += "RTU_No,";
            SQLString += "Displacement,";
            SQLString += "Displacement_Acc,";
            SQLString += "Pressure,";
            SQLString += "Detect_Time, ";
            SQLString += "Upload_Time ";
            SQLString += "from RTU_Data where ";
            if (RTU_No != 0)
            {
                SQLString += "RTU_No=@RTU_No and ";
            }
            SQLString += "Detect_Time >= @Start_Time and Detect_Time <= @End_Time and ";
            if (Data_Tag == 1)
            {
                SQLString += " Sign=1 ";
            }
            else if (Data_Tag == 0)
            {
                SQLString += " 1=1 ";
            }

            SQLString += " UNION ALL ";

            SQLString += "select ";
            SQLString += "RTU_Data_ID,";
            SQLString += "RTU_No,";
            SQLString += "Displacement,";
            SQLString += "Displacement_Acc,";
            SQLString += "Pressure,";
            SQLString += "Detect_Time, ";
            SQLString += "Upload_Time ";
            SQLString += "from rtu_data_history where ";

            if (RTU_No != 0)
            {
                SQLString += "RTU_No=@RTU_No and ";
            }
            SQLString += "Detect_Time >= @Start_Time and Detect_Time <= @End_Time and ";
            if (Data_Tag == 1)
            {
                SQLString += " Sign=1 ";
            }
            else if (Data_Tag == 0)
            {
                SQLString += " 1=1 ";
            }
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@RTU_No",RTU_No),
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
                    ListRtuData = ModelConvertHelper<RtuDataModel>.ConvertToModel(dt);
                    Error = 0;
                    ErrorMessage = "";
                }
            }
            catch (Exception ex)
            {
                Error = 1;
                ErrorMessage = "数据处理异常，重新尝试";
            }
        }


        /// <summary>
        /// RTU图片上传
        /// </summary>
        /// <param name="RTU_No">RTU编号</param>
        /// <param name="Start_Time">检测时间</param>
        /// <param name="End_Time">检测时间</param>
        /// <param name="Data_Tag">查询标记，表示是否全部接收数据；0为全部接收，1为接收未上传数据</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        /// <param name="ListRtuData">返回查询到的RTU图片</param>
        public void SelRtuPic(int RTU_No, DateTime Start_Time, DateTime End_Time, int Data_Tag, out int Error, out string ErrorMessage, out List<RtuPicModel> ListRtuPic)
        {
            Error = 0;
            ErrorMessage = "";
            ListRtuPic = new List<RtuPicModel>();

            //先将时间标准化
            if (Start_Time == End_Time && Start_Time == DateTime.MinValue)
            {
                Start_Time = DateTime.MinValue;
                End_Time = DateTime.MaxValue;
            }
            if (Start_Time == End_Time)
            {
                End_Time = End_Time.AddDays(1);
            }

            string SQLString = "select ";
            SQLString += "RTU_Pic_ID,";
            SQLString += "Pic_Name,";
            SQLString += "RTU_No,";
            SQLString += "RTU_Pic_Address,";
            SQLString += "Detect_Time,";
            SQLString += "Upload_Time ";
            SQLString += "from RTU_Pic where ";
            if (RTU_No != 0)
            {
                SQLString += "RTU_No=@RTU_No and ";
            }
            SQLString += "Detect_Time >= @Start_Time and Detect_Time <= @End_Time and ";
            if (Data_Tag == 1)
            {
                SQLString += " Sign=1 ";
            }
            else if (Data_Tag == 0)
            {
                SQLString += " 1=1 ";
            }

            SQLString += " UNION ALL ";

            SQLString += "select ";
            SQLString += "RTU_Pic_ID,";
            SQLString += "Pic_Name,";
            SQLString += "RTU_No,";
            SQLString += "RTU_Pic_Address,";
            SQLString += "Detect_Time,";
            SQLString += "Upload_Time ";
            SQLString += "from rtu_pic_history where ";


            if (RTU_No != 0)
            {
                SQLString += "RTU_No=@RTU_No and ";
            }
            SQLString += "Detect_Time >= @Start_Time and Detect_Time <= @End_Time and ";
            if (Data_Tag == 1)
            {
                SQLString += " Sign=1 ";
            }
            else if (Data_Tag == 0)
            {
                SQLString += " 1=1 ";
            }
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@RTU_No",RTU_No),
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
                    ListRtuPic = ModelConvertHelper<RtuPicModel>.ConvertToModel(dt);
                    Error = 0;
                    ErrorMessage = "";
                }
            }
            catch (Exception ex)
            {
                Error = 1;
                ErrorMessage = "数据处理异常，重新尝试";
            }
        }


        /// <summary>
        /// RTU参数设置
        /// </summary>
        /// <param name="RTU_No">RTU编号</param>
        /// <param name="RTU_ParaMeter_No">RTU状态参数</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        public void UpdateRTUPara(int RTU_No, int RTU_ParaMeter_No, out int Error, out string ErrorMessage)
        {
            Error = 0;
            ErrorMessage = "";
            string SQLString = "update RTU_ParaMeter set ";
            SQLString += "RTU_ParaMeter_No=@RTU_ParaMeter_No ";
            SQLString += "where RTU_No=@RTU_No";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                        new MySqlParameter("@RTU_ParaMeter_No",RTU_ParaMeter_No),
                        new MySqlParameter("@RTU_No",RTU_No)
            };
            try
            {
                bool b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
                if (b == false)
                {
                    Error = 1;
                    ErrorMessage = "参数设置失败";
                }
                else
                {
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
        /// 根据Project_ID查出Jam堵剂表
        /// </summary>
        /// <param name="Project_ID">工程信息主键</param>
        /// <param name="Jam">接收到的堵剂信息</param>
        public void SelectJam(string Project_ID, out List<JamModel> Jam)
        {
            Jam = new List<JamModel>();
            string SQLString = "select ";
            SQLString += "Jam_ID,Jam_Name,Jam_Num ";
            SQLString += "from Jam where ";
            SQLString += "Project_ID=@Project_ID ";

            SQLString += " UNION ALL ";

            SQLString += "select ";
            SQLString += "Jam_ID,Jam_Name,Jam_Num ";
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
                    JamModel.Jam_ID = dt.Rows[i][0].ToString();
                    JamModel.Jam_Name = dt.Rows[i][1].ToString();
                    JamModel.Jam_Num = float.Parse(dt.Rows[i][2].ToString());
                    Jam.Add(JamModel);
                }
            }
            catch (Exception ex)
            {

            }
        }

        #region 事务结束
        /// <summary>
        /// 修改APP账号表标记为已上传
        /// </summary>
        /// <param name="Account_ID">APP账号ID</param>
        public void UpdateAccountSign(string Account_ID)
        {
            string SQLString = "update Account set ";
            SQLString += "Sign=0 ";
            SQLString += "where Account_ID=@Account_ID";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                        new MySqlParameter("@Account_ID",Account_ID)
            };
            bool b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
        }

        /// <summary>
        /// 修改工程信息表标记为已上传
        /// </summary>
        /// <param name="Project_ID">工程信息表主键</param>
        public void UpdateProjectSign(string Project_ID)
        {
            string SQLString = "update Project set ";
            SQLString += "Sign=0 ";
            SQLString += "where Project_ID=@Project_ID";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                        new MySqlParameter("@Project_ID",Project_ID)
            };
            bool b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);

            SQLString = "update project_history set ";
            SQLString += "Sign=0 ";
            SQLString += "where Project_ID=@Project_ID";
            comParamerer = new MySqlParameter[]
            {
                        new MySqlParameter("@Project_ID",Project_ID)
            };
            b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
        }

        /// <summary>
        /// 堵剂信息表标记为已上传
        /// </summary>
        /// <param name="Project_ID">工程信息主键</param>
        public void UpdateJamSign(string Project_ID)
        {
            string SQLString = "update Jam set ";
            SQLString += "Sign=0 ";
            SQLString += "where Project_ID=@Project_ID";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                        new MySqlParameter("@Project_ID",Project_ID)
            };
            bool b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);

            SQLString = "update jam_history set ";
            SQLString += "Sign=0 ";
            SQLString += "where Project_ID=@Project_ID";
             comParamerer = new MySqlParameter[]
            {
                        new MySqlParameter("@Project_ID",Project_ID)
            };
            b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
        }

        /// <summary>
        /// 工程日志表标记为已上传
        /// </summary>
        /// <param name="Project_Log_ID">工程日志主键</param>
        public void UpdateProjectLogSign(string Project_Log_ID)
        {
            string SQLString = "update Project_Log set ";
            SQLString += "Sign=0 ";
            SQLString += "where Project_Log_ID=@Project_Log_ID";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                        new MySqlParameter("@Project_Log_ID",Project_Log_ID)
            };
            bool b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);

            SQLString = "update project_log_history set ";
            SQLString += "Sign=0 ";
            SQLString += "where Project_Log_ID=@Project_Log_ID";
            comParamerer = new MySqlParameter[]
            {
                        new MySqlParameter("@Project_Log_ID",Project_Log_ID)
            };
            b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
        }

        /// <summary>
        /// RTU数据表标记为已上传
        /// </summary>
        /// <param name="RTU_Data_ID">RTU数据表主键</param>
        public void UpdateRTUDataSign(string RTU_Data_ID)
        {
            string SQLString = "update RTU_Data set ";
            SQLString += "Sign=0 ";
            SQLString += "where RTU_Data_ID=@RTU_Data_ID";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                        new MySqlParameter("@RTU_Data_ID",RTU_Data_ID)
            };
            bool b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);

            SQLString = "update rtu_data_history set ";
            SQLString += "Sign=0 ";
            SQLString += "where RTU_Data_ID=@RTU_Data_ID";
            comParamerer = new MySqlParameter[]
            {
                        new MySqlParameter("@RTU_Data_ID",RTU_Data_ID)
            };
            b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
        }

        /// <summary>
        /// RTU图片表标记为已上传
        /// </summary>
        /// <param name="RTU_Pic_ID">RTU数据表主键</param>
        public void UpdateRTUPicSign(string RTU_Pic_ID)
        {
            string SQLString = "update RTU_Pic set ";
            SQLString += "Sign=0 ";
            SQLString += "where RTU_Pic_ID=@RTU_Pic_ID";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                        new MySqlParameter("@RTU_Pic_ID",RTU_Pic_ID)
            };
            bool b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);

            SQLString = "update rtu_pic_history set ";
            SQLString += "Sign=0 ";
            SQLString += "where RTU_Pic_ID=@RTU_Pic_ID";
            comParamerer = new MySqlParameter[]
            {
                        new MySqlParameter("@RTU_Pic_ID",RTU_Pic_ID)
            };
            b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
        }
        #endregion

        /// <summary>
        /// APP账号修改
        /// </summary>
        /// <param name="Account_ID">APP账号主键</param>
        /// <param name="Account_Name">账号名</param>
        /// <param name="Account_Password">账号密码</param>
        /// <param name="Account_Time">注册时间</param>
        /// <param name="Name">姓名</param>
        /// <param name="TEL">联系方式</param>
        /// <param name="Company">所在单位</param>
        /// <param name="Account_Picture">头像</param>
        /// <param name="Account_Permission">权限</param>
        /// <param name="Error">错误号</param>
        /// <param name="ErrorMessage">错误信息</param>
        public void UpdateAccount(string Account_ID, string Account_Name, string Account_Password, string Account_Time, string Name, string TEL, string Company, string Account_Picture, int Account_Permission, out int Error, out string ErrorMessage)
        {
            Error = 0;
            ErrorMessage = "";
            string SQLString = "update Account set ";
            if (Account_Name != "" && Account_Name != null)
            {
                SQLString += "Account_Name=@Account_Name and ";
            }
            if (Account_Password != "" && Account_Password != null)
            {
                SQLString += "Account_Password=@Account_Password and ";
            }
            if (Account_Time != "" && Account_Time != null)
            {
                SQLString += "Account_Time=@Account_Time and ";
            }
            if(Account_Time=="")
            {
                Account_Time = null;
            }
            if (Name != "" && Name != null)
            {
                SQLString += "Name=@Name and ";
            }
            if (TEL != "" && TEL != null)
            {
                SQLString += "TEL=@TEL and ";
            }
            if (Company != "" && Company != null)
            {
                SQLString += "Company=@Company and ";
            }
            if (Account_Picture != "" && Account_Picture != null)
            {
                SQLString += "Account_Picture=@Account_Picture and ";
            }
            SQLString += "Account_Permission=@Account_Permission ";
            SQLString += "Last_Operate_Date=@Last_Operate_Date ";
            SQLString += "Last_Operate_Type=@Last_Operate_Type ";
            SQLString += "where Account_ID=@Account_ID";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@Account_Name",Account_Name),
                new MySqlParameter("@Account_Password",Account_Password),
                new MySqlParameter("@Account_Time",Convert.ToDateTime(Account_Time)),
                new MySqlParameter("@Name",Name),
                new MySqlParameter("@TEL",TEL),
                new MySqlParameter("@Company",Company),
                new MySqlParameter("@Account_Picture",Account_Picture),
                new MySqlParameter("@Account_Permission",Account_Permission),
                new MySqlParameter("@Account_ID",Account_ID),
                new MySqlParameter("@Last_Operate_Date",DateTime.Now),
                new MySqlParameter("@Last_Operate_Type",1),
            };
            try
            {
                bool b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
                if (b == false)
                {
                    Error = 1;
                    ErrorMessage = "账号信息修改失败";
                }
                else
                {
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


    }
}
