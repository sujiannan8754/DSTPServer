using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace DSTP_DAL
{
 
    

    /// <summary>
    /// 日志级别
    /// Level级别：DEBUG <INFO<WARN<ERROR<FATAL
    /// </summary>
    public enum myLogLevel { All, DEBUG, ERROR, WARNING, INFOMATION, FATAL }


    /// <summary>
    /// 这个是对日志框架log4Net的封装类
    /// </summary>
    public class LogHelperNLog
    {
        /// <summary>
        /// 输出错误日志到NLog
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        #region static void WriteLog( Exception ex)

        public static void WriteLog(Type t, Exception ex)
        {
            try
            {
                NLog.Logger log = LogManager.GetCurrentClassLogger(t);
                log.Error(ex.Message.ToString());
            }
            catch (System.Exception ex1)
            {

            }

        }

        #endregion

        /// <summary>
        /// 输出日志到NLog
        /// </summary>
        /// <param name="msgLevel">  消息级别
        ///                         alert,debug,error 等，仅实现少量常用的</param>
        /// <param name="msg"></param>
        #region static void WriteLog(myLogLevel msgLevel, string msg)


        public static void WriteLog(myLogLevel msgLevel, string msg)
        {
            try
            {
                NLog.Logger log = LogManager.GetCurrentClassLogger();
                switch (msgLevel)
                {
                    case myLogLevel.DEBUG: //debug
                        log.Debug(msg);
                        break;
                    case myLogLevel.ERROR:  //错误
                        log.Error(msg);
                        break;
                    case myLogLevel.WARNING: //警告
                        log.Warn(msg);
                        break;
                    case myLogLevel.FATAL: //致命的
                        log.Fatal(msg);
                        break;
                    case myLogLevel.INFOMATION: //信息
                        log.Info(msg);
                        break;
                    case myLogLevel.All: //所有
                        log.Info(msg);
                        break;
                    default:
                        log.Info(msg);
                        break;
                }
                //log.Error(msg);
            }
            catch (System.Exception ex)
            {

            }

        }

        #endregion


    }

}
