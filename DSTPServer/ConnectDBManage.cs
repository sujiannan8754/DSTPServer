using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DSTP_DAL;
using System.Windows.Forms;
using DSTP_BLL.ClassToOther;

namespace DSTPServer
{
    /// <summary>
    /// 用于数据库连接，管理读写的互斥锁
    /// </summary>
    public class ConnectDBManage
    {
        MySQLHelper MySQL;

        /// <summary>
        /// 写数据库时的的互斥锁
        /// </summary>
        public Mutex[] WriteDBMutex;

        /// <summary>
        /// 工程信息互斥锁
        /// </summary>
        public Mutex WriteProjectMutex;

        /// <summary>
        /// 工程日志互斥锁
        /// </summary>
        public Mutex WriteLogMutex;

        /// <summary>
        /// RTU数据互斥锁
        /// </summary>
        public Mutex WriteRTUDataMutex;

        public Mutex WriteRTUpicMutex;

        public ConnectDBManage()
        {
            WriteDBMutex = new Mutex[4];
            WriteProjectMutex = new Mutex();
            WriteLogMutex = new Mutex();
            WriteRTUDataMutex = new Mutex();
            WriteRTUpicMutex = new Mutex();
            WriteDBMutex[0] = WriteProjectMutex;
            WriteDBMutex[1] = WriteLogMutex;
            WriteDBMutex[2] = WriteRTUDataMutex;
            WriteDBMutex[3] = WriteRTUpicMutex;


            MySQL = new MySQLHelper();
            try
            {
                MySQL.ConnTo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库连接失败");
                DSTP_BLL.ClassToOther.LogHelperNLog.WriteLog(DSTP_BLL.ClassToOther.myLogLevel.INFOMATION, " ConnectDBManage " + ex.Message.ToString());
            }
        }
    }
}
