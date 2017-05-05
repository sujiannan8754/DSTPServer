using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DSTP_DAL
{
    /// <summary>
    /// 账号管理数据库操作类
    /// </summary>
    public class AccountDAL
    {
        MySQLHelper MySQLHelper = new MySQLHelper();

        /// <summary>
        /// 查询所有账号信息
        /// </summary>
        public void SelectAccount(out DataTable Account)
        {
            string SQLString = "Select * from Account";
            Account = MySQLHelper.ExecuteDataTable(SQLString);
        }

        /// <summary>
        /// 条件查询账号信息
        /// </summary>
        /// <param name="Account_Name">用户名</param>
        /// <param name="Name">姓名</param>
        /// <param name="Account_Permission">账号权限</param>
        /// <param name="Error_Message">错误信息，成功为""</param>
        public void SelectAccountToCondition(string Account_Name, string Name, int Account_Permission, out string Error_Message, out DataTable Account)
        {
            Error_Message = "";
            Account = null;

            string SQLString = "Select * from Account Where ";
            if (Account_Name != "")
            {
                SQLString += " Account_Name=@Account_Name and ";
            }
            if (Name != "")
            {
                SQLString += " Name like @Name and ";
            }
            if (Account_Permission != -1)
            {
                SQLString += " Account_Permission=@Account_Permission and ";
            }
            SQLString += " 1=1 ";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@Account_Name",Account_Name),
                new MySqlParameter("@Name","%"+Name+"%"),
                new MySqlParameter("@Account_Permission",Account_Permission)
            };
            try
            {
                Account = MySQLHelper.ExecuteDataTable(SQLString, comParamerer);
                if (Account == null)
                {
                    Error_Message = "未查询到账号信息";
                }
            }
            catch (Exception ex)
            {
                Error_Message = "数据库操作操作失败";
            }
        }


        /// <summary>
        /// 新增账号
        /// </summary>
        /// <param name="AM">账号模型</param>
        /// <param name="Error_Message">错误信息，成功为""</param>
        public void AddAccount(AccountModel AM, out string Error_Message)
        {
            Error_Message = "";
            string SQLString = "INSERT INTO Account ";
            SQLString += "(Account_ID,Account_Name,Account_Password,Account_Time,Name,TEL,Company,Account_Picture,Account_IsUse,Account_Permission,Sign,Last_Operate_Date,Last_Operate_Type) ";
            SQLString += " VALUES(@Account_ID,@Account_Name,@Account_Password,@Account_Time,@Name,@TEL,@Company,@Account_Picture,@Account_IsUse,@Account_Permission,@Sign,@Last_Operate_Date,@Last_Operate_Type)";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@Account_ID",AM.Account_ID),
                new MySqlParameter("@Account_Name",AM.Account_Name),
                new MySqlParameter("@Account_Password",AM.Account_Password),
                new MySqlParameter("@Account_Time",AM.Account_Time),
                new MySqlParameter("@Name",AM.Name),
                new MySqlParameter("@TEL",AM.TEL),
                new MySqlParameter("@Company",AM.Company),
                new MySqlParameter("@Account_Picture",AM.Account_Picture),
                new MySqlParameter("@Account_IsUse",AM.Account_IsUse),
                new MySqlParameter("@Account_Permission",AM.Account_Permission),
                new MySqlParameter("@Sign",AM.Sign),
                new MySqlParameter("@Last_Operate_Date",AM.Last_Operate_Date),
                new MySqlParameter("@Last_Operate_Type",AM.Last_Operate_Type)
            };
            try
            {
                bool b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
                if (b == true)
                {
                    Error_Message = "";
                }
                else
                {
                    Error_Message = "数据处理异常";
                }
            }
            catch (Exception ex)
            {
                Error_Message = "数据库操作操作失败";
            }
        }

        /// <summary>
        /// 修改账号信息
        /// </summary>
        /// <param name="AM">账号模型</param>
        /// <param name="Error_Message">错误信息，成功为""</param>
        public void UpdateAccount(AccountModel AM, out string Error_Message)
        {
            Error_Message = "";

            string SQLString = "update Account set ";
            SQLString += "Account_Name=@Account_Name,";
            SQLString += "Account_Password=@Account_Password,";
            SQLString += "Name=@Name,";
            SQLString += "TEL=@TEL,";
            SQLString += "Company=@Company,";
            SQLString += "Account_Picture=@Account_Picture,";
            SQLString += "Account_IsUse=@Account_IsUse,";
            SQLString += "Account_Permission=@Account_Permission,";
            SQLString += "Sign=@Sign,";
            SQLString += "Last_Operate_Date=@Last_Operate_Date,";
            SQLString += "Last_Operate_Type=@Last_Operate_Type ";
            SQLString += "where Account_ID=@Account_ID ";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@Account_Name",AM.Account_Name),
                new MySqlParameter("@Account_Password",AM.Account_Password),
                new MySqlParameter("@Name",AM.Name),
                new MySqlParameter("@TEL",AM.TEL),
                new MySqlParameter("@Company",AM.Company),
                new MySqlParameter("@Account_Picture",AM.Account_Picture),
                new MySqlParameter("@Account_IsUse",AM.Account_IsUse),
                new MySqlParameter("@Account_Permission",AM.Account_Permission),
                new MySqlParameter("@Sign",AM.Sign),
                new MySqlParameter("@Account_ID",AM.Account_ID),
                new MySqlParameter("@Last_Operate_Date",AM.Last_Operate_Date),
                new MySqlParameter("@Last_Operate_Type",AM.Last_Operate_Type)
            };
            try
            {
                bool b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
                if (b == true)
                {
                    Error_Message = "";
                }
                else
                {
                    Error_Message = "数据处理异常";
                }
            }
            catch (Exception ex)
            {
                Error_Message = "数据库操作操作失败";
            }
        }

        /// <summary>
        /// 删除账号信息
        /// </summary>
        /// <param name="Account_ID">账号主键</param>
        /// <param name="Error_Message">错误信息，成功为""</param>
        public void DeleteAccount(string Account_ID,out string Error_Message)
        {
            Error_Message = "";
            string SQLString = "update Account set ";
            SQLString += "Last_Operate_Date=@Last_Operate_Date,";
            SQLString += "Last_Operate_Type=@Last_Operate_Type,";
            SQLString += "Sign=@Sign ";
            SQLString += "where Account_ID=@Account_ID ";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@Account_ID",Account_ID),
                new MySqlParameter("@Sign",1),
                new MySqlParameter("@Last_Operate_Date",DateTime.Now),
                new MySqlParameter("@Last_Operate_Type",2)
            };
            try
            {
                bool b = MySQLHelper.ExecuteNonQuery(SQLString, comParamerer);
                if (b == true)
                {
                    Error_Message = "";
                }
                else
                {
                    Error_Message = "数据处理异常";
                }
            }
            catch (Exception ex)
            {
                Error_Message = "数据库操作操作失败";
            }
        }
    }
}
