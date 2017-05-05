using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSTP_DAL
{
    /// <summary>
    /// 工程信息模型
    /// </summary>
    public class ProjectModel
    {
        /// <summary>
        /// 主键，GUID生成
        /// </summary>
        public string Project_ID { get; set; }
        /// <summary>
        /// APP账号主键
        /// </summary>
        public string Account_ID { get; set; }
        /// <summary>
        /// 工程编号；RTU编号（井号）+施工开始日期（yyyyMMdd）
        /// </summary>
        public string Project_No { get; set; }
        /// <summary>
        /// 施工开始日期；不可修改  yyyy-MM-dd HH:mm:ss
        /// </summary>
        public DateTime Start_Date { get; set; }
        /// <summary>
        /// 罐体形状
        /// </summary>
        public string Jar_Shape { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        public string Jar_Size { get; set; }
        /// <summary>
        /// 体积
        /// </summary>
        public float Jar_Volume { get; set; }
        /// <summary>
        /// RTU编号；不可修改
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 措施名称
        /// </summary>
        public string Construction_Name { get; set; }
        /// <summary>
        /// 施工井号；不可修改
        /// </summary>
        public string Construction_Well_ID { get; set; }
        /// <summary>
        /// 采油厂
        /// </summary>
        public string Oil_Factory { get; set; }
        /// <summary>
        /// 区块
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// 施工地点
        /// </summary>
        public string Place { get; set; }
        /// <summary>
        /// 技术负责人名称
        /// </summary>
        public string Team_Leader_Name { get; set; }
        /// <summary>
        /// 技术负责人联系电话
        /// </summary>
        public string Team_Leader_Tel { get; set; }
        /// <summary>
        /// 技术负责人头像
        /// </summary>
        public string Team_Leader_Picture { get; set; }
        /// <summary>
        /// 现场施工人员名
        /// </summary>
        public string Team_Worker_Name { get; set; }
        /// <summary>
        /// 现场施工人员联系电话
        /// </summary>
        public string Team_Worker_Tel { get; set; }
        /// <summary>
        /// 现场施工人员头像
        /// </summary>
        public string Team_Worker_Picture { get; set; }
        /// <summary>
        /// 施工单位名称
        /// </summary>
        public string Company_Name { get; set; }
        /// <summary>
        /// 现场标准化场地全景图
        /// </summary>
        public string Con_Begin_Pic1 { get; set; }
        /// <summary>
        /// 现场堵剂摆放那个图片
        /// </summary>
        public string Con_Begin_Pic2 { get; set; }
        /// <summary>
        /// 消防摆放、安全警示团片
        /// </summary>
        public string Con_Begin_Pic3 { get; set; }
        /// <summary>
        /// 变压器摆放图
        /// </summary>
        public string Con_Begin_Pic4 { get; set; }
        /// <summary>
        /// 变电柜接电处图片
        /// </summary>
        public string Con_Begin_Pic5 { get; set; }
        /// <summary>
        /// 注水井口图片
        /// </summary>
        public string Con_Begin_Pic6 { get; set; }
        /// <summary>
        /// 开工许可证图片
        /// </summary>
        public string Con_Begin_Pic7 { get; set; }
        /// <summary>
        /// 井控证图片
        /// </summary>
        public string Con_Begin_Pic8 { get; set; }
        /// <summary>
        /// 施工设计图片
        /// </summary>
        public string Con_Begin_Pic9 { get; set; }
        /// <summary>
        /// 措施前注水井油压MPa
        /// </summary>
        public float Con_Begin_OilPressure { get; set; }
        /// <summary>
        /// 措施前套压MPa
        /// </summary>
        public float Con_Begin_CasingPressure { get; set; }
        /// <summary>
        /// 措施前日注水量m3/d
        /// </summary>
        public float Con_Begin_Dayinflow { get; set; }
        /// <summary>
        /// 措施前是否分井；0不分井，1分井
        /// </summary>
        public int Con_Begin_IsSeparate { get; set; }
        /// <summary>
        /// 措施前配水间分压MPa
        /// </summary>
        public float Con_Begin_SepPresure { get; set; }
        /// <summary>
        /// 施工结束日期         yyyy-MM-dd HH:mm:ss
        /// </summary>
        public DateTime End_Date { get; set; }
        /// <summary>
        /// 措施后套压MPa
        /// </summary>
        public float Con_End_CasingPressure { get; set; }
        /// <summary>
        /// 措施后注水井油压MPa
        /// </summary>
        public float Con_End_OilPressure { get; set; }
        /// <summary>
        /// 措施后日注水量m3/d
        /// </summary>
        public float Con_End_Dayinflow { get; set; }
        /// <summary>
        /// 措施后无套压原因
        /// </summary>
        public string CasPre_Cause { get; set; }
        /// <summary>
        /// 措施后井场全景图
        /// </summary>
        public string Con_End_Pic1 { get; set; }
        /// <summary>
        /// 工程状态；0未结束、1已结束
        /// </summary>
        public int Project_State { get; set; }
        /// <summary>
        /// 最后操作人
        /// </summary>
        public string Last_Operate_People { get; set; }
        /// <summary>
        /// 最后操作时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public DateTime Last_Operate_Date { get; set; }
        /// <summary>
        /// 最后操作；0新增、1修改、2删除
        /// </summary>
        public int Last_Operate_Type { get; set; }
        /// <summary>
        /// 标记是否已上传至内网；0已上传，1未上传
        /// </summary>
        public int Sign { get; set; }
    }
}
