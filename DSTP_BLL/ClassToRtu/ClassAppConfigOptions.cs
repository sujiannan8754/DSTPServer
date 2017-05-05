using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace DSTP_BLL.ClassToRtu
{
    /// <summary>
    /// 读取Config文件信息
    /// </summary>
   public class ClassAppConfigOptions
    {
        Configuration config;
        Mutex myMutex;
        public ClassAppConfigOptions()
        {
            config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            myMutex = new Mutex();
        }

        /// <summary>
        /// 读取配置字段的值
        /// </summary>
        /// <param name="strName">字段名称</param>
        /// <param name="strValue">字段值</param>
        /// <returns>成功返回true,失败返回false</returns>
        public bool readConfig(string strName, out string strValue)
        {
            if (config == null)
            {
                strValue = null;
                return false;
            }
            bool result = false;
            string strTmp = null;
            myMutex.WaitOne();
            try
            {
                #region 
                strTmp = config.AppSettings.Settings[strName].Value;
                result = true;
                strValue = strTmp;
                #endregion
            }
            catch (System.Exception ex)
            {
                result = false;
                strValue = null;
            }
            myMutex.ReleaseMutex();
            return result;
        }

        /// <summary>
        /// 写特定配置字段的值
        /// </summary>
        /// <param name="strName">字段名称</param>
        /// <param name="strValue">字段值</param>
        /// <returns>成功返回true,失败返回false</returns>
        public bool writeConfig(string strName, string strValue)
        {
            if (config == null)
            {
                return false;
            }
            bool result = false;
            myMutex.WaitOne();
            try
            {
                #region 
                config.AppSettings.Settings[strName].Value = strValue;
                config.Save(ConfigurationSaveMode.Modified);
                System.Configuration.ConfigurationManager.RefreshSection("appSettings");
                result = true;

                #endregion
            }
            catch (System.Exception ex)
            {
                result = false;
            }
            myMutex.ReleaseMutex();
            return result;
        }
    }
}
