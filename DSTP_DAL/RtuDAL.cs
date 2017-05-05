using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DSTP_DAL
{
    /// <summary>
    /// RTU数据库操作
    /// </summary>
    public class RtuDAL
    {
        MySQLHelper sqlhelper = new MySQLHelper();

        /// <summary>
        /// 新增RTU数据
        /// </summary>
        /// <param name="rtuModel">RTU数据模型</param>
        /// <returns></returns>
        public bool AddRtuDate(RtuDataModel rtuModel)
        {
            string SQLString = "INSERT INTO rtu_data ";
            SQLString += "(RTU_Data_ID,RTU_No,Displacement,Displacement_Acc,Pressure,Conductance_Ratio,Tem_InBox,Tem_OutSide,Detect_Time,Upload_Time,Sign)";
            SQLString += " VALUES(@RTU_Data_ID, @RTU_No, @Displacement, @Displacement_Acc, @Pressure, @Conductance_Ratio, @Tem_InBox, @Tem_OutSide, @Detect_Time, @Upload_Time, @Sign); ";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@RTU_Data_ID",rtuModel.RTU_Data_ID),
                new MySqlParameter("@RTU_No",rtuModel.RTU_No),
                new MySqlParameter("@Displacement",rtuModel.Displacement),
                new MySqlParameter("@Displacement_Acc",rtuModel.Displacement_Acc),
                new MySqlParameter("@Pressure",rtuModel.Pressure),
                new MySqlParameter("@Conductance_Ratio",rtuModel.ConductanceRatio),
                new MySqlParameter("@Tem_InBox",rtuModel.TemperatureInBox),
                new MySqlParameter("@Tem_OutSide",rtuModel.TemperatureOutside),
                new MySqlParameter("@Detect_Time",rtuModel.Detect_Time),
                new MySqlParameter("@Upload_Time",rtuModel.Upload_Time),
                new MySqlParameter("@Sign",rtuModel.Sign)
            };
            return sqlhelper.ExecuteNonQuery(SQLString, comParamerer);
        }

        /// <summary>
        /// 新增RTU图片数据
        /// </summary>
        /// <param name="picModel">图片模型</param>
        /// <returns></returns>
        public bool AddRtuPic(RtuPicModel picModel)
        {
            string SQLString = "INSERT INTO rtu_pic ";
            SQLString += "(RTU_Pic_ID,Pic_name,RTU_No,RTU_Pic_Address,Detect_Time,Upload_Time,Sign)";
            SQLString += " VALUES(@RTU_Pic_ID, @Pic_name, @RTU_No, @RTU_Pic_Address, @Detect_Time, @Upload_Time, @Sign); ";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@RTU_Pic_ID",picModel.RTU_Pic_ID),
                new MySqlParameter("@Pic_name",picModel.Pic_name),
                new MySqlParameter("@RTU_No",picModel.RTU_No),
                new MySqlParameter("@RTU_Pic_Address",picModel.RTU_Pic_Address),
                new MySqlParameter("@Detect_Time",picModel.Detect_Time),
                new MySqlParameter("@Upload_Time",picModel.Upload_Time),
                new MySqlParameter("@Sign",picModel.Sign)
            };
            return sqlhelper.ExecuteNonQuery(SQLString, comParamerer);
        }

        /// <summary>
        /// 设置RTU启用状态
        /// </summary>
        /// <param name="RTU_No">RTU设备编号</param>
        /// <param name="RTU_ParaMeter_No">启用状态：1正常使用，2停用，3检修</param>
        /// <returns></returns>
        public bool RtuParaMeter(int RTU_No,int RTU_ParaMeter_No)
        {
            bool b;
            string SQLString = "update rtu_parameter set RTU_ParaMeter_No=@RTU_ParaMeter_No where RTU_No=@RTU_No";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@RTU_ParaMeter_No",RTU_ParaMeter_No),
                new MySqlParameter("@RTU_No",RTU_No)
            };
            try
            {
                b = sqlhelper.ExecuteNonQuery(SQLString, comParamerer);
            }
            catch (Exception ex)
            {
                b = false;
            }
            return b;
        }

        /// <summary>
        /// 查询某RTU开启状态 启用状态：1正常使用，2停用，3检修
        /// </summary>
        /// <param name="RTU_No"></param>
        /// <returns></returns>
        public int SelectRtuParaMeter(int RTU_No)
        {
            string SQLString = "select RTU_ParaMeter_No  from  rtu_parameter where RTU_No=@RTU_No";
            MySqlParameter[] comParamerer = new MySqlParameter[]
            {
                new MySqlParameter("@RTU_No",RTU_No)
            };
            int RTU_ParaMeter_No = int.Parse(sqlhelper.ExecuteFirst(SQLString, comParamerer));
            return RTU_ParaMeter_No;
        }
    }
}
