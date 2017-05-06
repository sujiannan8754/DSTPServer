using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace DSTP_DAL
{
    /// <summary>
    /// MySQLHelper类
    /// </summary>
    public class MySQLHelper
    {
        /// <summary>
        /// 数据库连接串
        /// </summary>
        private string ConnString = "Server=127.0.0.1;Port=3306;DataBase=db_dstp;Uid=root;Pwd=root;pooling=false;charset=utf8;";
        /// <summary>
        /// 数据库连接
        /// </summary>
        private MySqlConnection Conn;
        /// <summary>
        /// 数据库连接
        /// </summary>
        private MySqlDataReader reader;
        /// <summary>
        /// 错误信息
        /// </summary>
        public static string ErrorString = "";
        /// <summary>
        /// 超时（秒）
        /// </summary>
        public int TimeOut = 0;

        /// <summary>
        /// 去掉SQL中的特殊字符
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns></returns>
        public string ReplaceSql(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            value = value.Replace("\\", "\\\\");
            value = value.Replace("'", "''");
            value = value.Replace("\"", "\\\"");
            value = value.Replace("%", "\\%");
            return value;
        }

        /// <summary>
        /// 执行sql返回DataTable
        /// </summary>
        /// <param name="SqlString">SQL语句</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string SqlString)
        {
            DataTable dt=null;
            try
            {
                dt= ExecuteDataTable(SqlString, null);
            } 
            catch(Exception ex)
            {
                dt = null;
                AddError(ex.Message, SqlString);
            }
            return dt;
        }

        /// <summary>
        /// 执行sql返回DataTable
        /// </summary>
        /// <param name="SqlString">SQL语句</param>
        /// <param name="parms">Sql参数</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteDataTable(string SqlString, MySqlParameter[] parms)
        {
            if (Conn == null || Conn.State != ConnectionState.Open)
                ConnTo();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = SqlString;
                cmd.CommandTimeout = TimeOut;
                if (parms != null)
                    foreach (MySqlParameter pram in parms)
                        cmd.Parameters.Add(pram);
                DataTable dt = new DataTable();
                try
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                    reader = cmd.ExecuteReader();
                    dt.Load(reader);
                }
                catch
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                    reader = cmd.ExecuteReader();
                    dt = Read(ref reader);
                }
                return dt;
            }
            catch (Exception e)
            {
                AddError(e.Message, SqlString);
                return null;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
        }

        /// <summary>
        /// 读取所有数据
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private DataTable Read(ref MySqlDataReader reader)
        {
            DataTable dt = new DataTable();
            bool frist = true;
            while (reader.Read())
            {
                if (frist)
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string s = reader.GetName(i);
                        //var type = reader[0].GetType();
                        dt.Columns.Add(s, Type.GetType("System.String"));
                    }
                    frist = false;
                }
                DataRow dr = dt.NewRow();
                for (int i = 0; i < reader.FieldCount; i++)
                    dr[i] = reader.GetString(i);
                dt.Rows.Add(dr);
            }
            return dt;
        }


        public DataRow ExecuteDataTableRow(string SqlString)
        {
            return ExecuteDataTableRow(SqlString, null);
        }

        /// <summary>
        /// 返回第一行
        /// </summary>
        /// <param name="SqlString"></param>
        /// <returns></returns>
        public DataRow ExecuteDataTableRow(string SqlString, MySqlParameter[] parms)
        {
            if (Conn == null || Conn.State != ConnectionState.Open)
            {
                ConnTo();
            }
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = SqlString;
                cmd.CommandTimeout = TimeOut;
                if (parms != null)
                    foreach (MySqlParameter pram in parms)
                        cmd.Parameters.Add(pram);
                DataTable dt = new DataTable();
                try
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                    reader = cmd.ExecuteReader();
                    dt.Load(reader);
                }
                catch
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                    reader = cmd.ExecuteReader();
                    dt = Read(ref reader);
                }
                if (dt.Rows.Count > 0)
                    return dt.Rows[0];
            }
            catch (Exception e)
            {
                AddError(e.Message, SqlString);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return null;
        }

        public string ExecuteFirst(string SqlString)
        {
            return ExecuteFirst(SqlString, null);
        }

        /// <summary>
        /// 返回第一个值
        /// </summary>
        /// <param name="SqlString"></param>
        /// <returns></returns>
        public string ExecuteFirst(string SqlString, MySqlParameter[] parms)
        {
            if (Conn == null || Conn.State != ConnectionState.Open)
                ConnTo();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = SqlString;
                cmd.CommandTimeout = TimeOut;
                if (parms != null)
                    foreach (MySqlParameter pram in parms)
                        cmd.Parameters.Add(pram);
                if (reader != null && !reader.IsClosed)
                    reader.Close();
                reader = cmd.ExecuteReader();
                string xx = "";
                if (reader.Read())
                    xx = reader[0].ToString();
                return xx;
            }
            catch (Exception e)
            {
                AddError(e.Message, SqlString);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            return null;
        }

        public long ExecuteInsertId(string SqlString)
        {
            return ExecuteInsertId(SqlString, null);
        }

        /// <summary>
        /// 返回第一个值
        /// </summary>
        /// <param name="SqlString"></param>
        /// <returns></returns>
        public long ExecuteInsertId(string SqlString, MySqlParameter[] parms)
        {
            if (Conn == null || Conn.State != ConnectionState.Open)
            {
                ConnTo();
            }
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = SqlString;
                cmd.CommandTimeout = TimeOut;
                if (parms != null)
                    foreach (MySqlParameter pram in parms)
                        cmd.Parameters.Add(pram);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {

                    throw;
                }
                return cmd.LastInsertedId;
            }
            catch (Exception e)
            {
                AddError(e.Message, SqlString);
            }
            return 0;
        }

        public bool ExecuteNonQuery(string SqlString)
        {
            return ExecuteNonQuery(SqlString, null);
        }

        /// <summary>
        /// 执行无返回SQL语句
        /// </summary>
        /// <param name="SqlString">SQL语句</param>
        /// <param name="parms">Sql参数</param>
        ///<returns>是否执行成功</returns>
        public bool ExecuteNonQuery(string SqlString, MySqlParameter[] parms)
        {
            if (Conn == null || Conn.State != ConnectionState.Open)
                ConnTo();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = SqlString;
                cmd.CommandTimeout = TimeOut;
                if (parms != null)
                {
                    foreach (MySqlParameter pram in parms)
                    {
                        if((pram.Direction==ParameterDirection.InputOutput||pram.Direction==ParameterDirection.Input)&&(pram.Value==null))
                        {
                            pram.Value = DBNull.Value;
                        }
                        cmd.Parameters.Add(pram);
                    }
                }

                try
                {
                    int rows = cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    AddError(ex.Message, SqlString);
                    return false;
                }
            }
            catch (Exception e)
            {
                AddError(e.Message, SqlString);
                return false;
            }
        }

        public bool ExecuteExists(string SqlString)
        {
            return ExecuteExists(SqlString, null);
        }

        /// <summary>
        /// 查询是否存在
        /// </summary>
        /// <param name="SqlString">SQL语句</param>
        /// <param name="parms">SQL参数</param>
        /// <returns>是否存在</returns>
        public bool ExecuteExists(string SqlString, MySqlParameter[] parms)
        {
            if (Conn == null || Conn.State != ConnectionState.Open)
                ConnTo();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = SqlString;
                cmd.CommandTimeout = TimeOut;
                if (parms != null)
                    foreach (MySqlParameter pram in parms)
                        cmd.Parameters.Add(pram);
                if (reader != null && !reader.IsClosed)
                    reader.Close();
                reader = cmd.ExecuteReader();
                if (reader.Read())
                    return true;
                return false;
            }
            catch (Exception e)
            {
                AddError(e.Message, SqlString);
                return false;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
        }
        /// <summary>
        /// 连接数据库
        /// </summary>
        public void ConnTo()
        {
            Close();
            try
            {
                Conn = new MySqlConnection(ConnString);
                Conn.Open();
            }
            catch (Exception e)
            {
                AddError(e.Message, ConnString);
            }
        }


        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sql"></param>
        private void AddError(string message, string sql)
        {
            ErrorString = "数据库操作错误：" + message + "\r\nSQL语句：" + sql + "\r\n";
            LogHelperNLog.WriteLog(myLogLevel.INFOMATION, " ExecuteNonQuery " + ErrorString);
            if (!string.IsNullOrEmpty(ErrorString) && ErrorString.Length > 1000)
                ErrorString = "";
        }

        /// <summary>
        /// 关闭数据库链接
        /// </summary>
        public void Close()
        {
            if (Conn != null && Conn.State == ConnectionState.Open)
            {
                Conn.Close();
                Conn = null;
            }
            else
                Conn = null;
            GC.Collect();
            //try
            //{
            //    Conn.Close();
            //    Conn = null;
            //}
            //catch
            //{
            //}
        }

    }
}
