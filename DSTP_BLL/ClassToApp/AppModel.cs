using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSTP_BLL.ClassToApp
{
    /// <summary>
    /// 接收Json数据(命令号以及指令序号)
    /// </summary>
    public class AppModel
    {
        /// <summary>
        /// 命令号
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
    }

    #region 发起命令
    /// <summary>
    /// 登录信息
    /// </summary>
    public class Login
    {
        /// <summary>
        /// 命令号11，验证登录信息
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// APP用户账号
        /// </summary>
        public string Data1 { get; set; }
        /// <summary>
        /// APP用户密码
        /// </summary>
        public string Data2 { get; set; }
    }

    /// <summary>
    /// 工程信息新增
    /// </summary>
    public class AddProject
    {
        /// <summary>
        /// 命令号21，工程信息新增
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// APP用户账号
        /// </summary>
        public string Data1 { get; set; }
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 施工开始日期 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Start_Date { get; set; }
        /// <summary>
        /// 施工井号
        /// </summary>
        public string Construction_Well_ID { get; set; }
        /// <summary>
        /// 技术负责人名称
        /// </summary>
        public string Team_Leader_Name { get; set; }
        /// <summary>
        /// 技术负责人联系方式
        /// </summary>
        public string Team_Leader_Tel { get; set; }
        /// <summary>
        /// 施工单位名称
        /// </summary>
        public string Company_Name { get; set; }
    }

    /// <summary>
    /// 工程信息上传
    /// </summary>
    public class UpdateProject
    {
        /// <summary>
        /// 命令号22，修改工程信息
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// APP用户账号
        /// </summary>
        public string Data1 { get; set; }
        /// <summary>
        /// 工程信息对象
        /// </summary>
        public Project Project { get; set; }
    }

    /// <summary>
    /// 工程信息对象
    /// </summary>
    public class Project
    {
        /// <summary>
        /// 施工开始日期；不可修改  yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Start_Date { get; set; }
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
        public string End_Date { get; set; }
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
        /// Project子数组；堵剂数量数组
        /// </summary>
        public List<Jam> Jam { get; set; }
        /// <summary>
        /// 工程状态；0未结束、1已结束
        /// </summary>
        public int Project_State { get; set; }
    }

    /// <summary>
    /// Project子数组；堵剂数量数组
    /// </summary>
    public class Jam
    {
        /// <summary>
        /// 堵剂名称（化学名称）
        /// </summary>
        public string Jam_Name { get; set; }
        /// <summary>
        /// 数量（袋或桶）
        /// </summary>
        public float Jam_Num { get; set; }
    }

    /// <summary>
    /// 工程概要信息查询
    /// </summary>
    public class SelectGYProject
    {
        /// <summary>
        /// 命令号23，查询工程信息总览
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// APP用户账号
        /// </summary>
        public string Data1 { get; set; }
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 施工井号
        /// </summary>
        public string Construction_Well_ID { get; set; }
        /// <summary>
        /// 施工单位名称
        /// </summary>
        public string Company_Name { get; set; }
        /// <summary>
        /// 按施工开始时间查询，起始时间 yyyyMMdd
        /// </summary>
        public string Start_Time { get; set; }
        /// <summary>
        /// 按施工开始时间查询，结束时间yyyyMMdd
        /// </summary>
        public string End_Time { get; set; }
    }

    /// <summary>
    /// 工程详细信息查询
    /// </summary>
    public class SelectXXProject
    {
        /// <summary>
        /// 命令号25，查询工程详细信息
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// APP用户账号
        /// </summary>
        public string Data1 { get; set; }
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 施工开始日期 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Start_Date { get; set; }
    }

    /// <summary>
    /// 工程信息删除
    /// </summary>
    public class DeleteProject
    {
        /// <summary>
        /// 命令号24，删除工程信息
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// APP用户账号
        /// </summary>
        public string Data1 { get; set; }
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 施工开始日期 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Start_Date { get; set; }
        /// <summary>
        /// 工程信息表主键，GUID
        /// </summary>
        public string Project_ID { get; set; }
    }

    /// <summary>
    /// 工程日志上传
    /// </summary>
    public class AddProject_Log
    {
        /// <summary>
        /// 命令号31，工程日志上传
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// APP用户账号
        /// </summary>
        public string Data1 { get; set; }
        /// <summary>
        /// 要新增的工程日志数组
        /// </summary>
        public List<Project_log> Project_Log_Add { get; set; }
        /// <summary>
        /// 要修改的工程日志数组
        /// </summary>
        public List<Project_log> Project_Log_Update { get; set; }
    }

    /// <summary>
    /// 工程日志
    /// </summary>
    public class Project_log
    {
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 施工开始时间
        /// </summary>
        public string Start_Date { get; set; }
        /// <summary>
        /// 工程日志编号，由APP端编号
        /// </summary>
        public int Project_Log_No { get; set; }
        /// <summary>
        /// 施工时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Construction_Time { get; set; }
        /// <summary>
        /// 压力MPa
        /// </summary>
        public float Pressure { get; set; }
        /// <summary>
        /// 排量m3/h
        /// </summary>
        public float Displacement { get; set; }
        /// <summary>
        /// 累计注入量m3
        /// </summary>
        public float Displacement_Acc { get; set; }
        /// <summary>
        /// 段塞名
        /// </summary>
        public string Slug_Name { get; set; }
        /// <summary>
        /// 堵剂配方
        /// </summary>
        public string Formula { get; set; }
        /// <summary>
        /// 事件
        /// </summary>
        public string Events { get; set; }

    }

    /// <summary>
    /// 工程日志查询
    /// </summary>
    public class SelectProject_log
    {
        /// <summary>
        /// 命令号33，查询工程日志
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// APP用户账号
        /// </summary>
        public string Data1 { get; set; }
        /// <summary>
        /// 施工开始时间
        /// </summary>
        public string Start_Date { get; set; }
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 按时间查询，起始时间 yyyyMMdd
        /// </summary>
        public string Start_Time { get; set; }
        /// <summary>
        /// 按时间查询，结束时间yyyyMMdd
        /// </summary>
        public string End_Time { get; set; }
    }

    /// <summary>
    /// 工程日志删除
    /// </summary>
    public class DeleteProject_log
    {
        /// <summary>
        /// 命令号34，删除工程日志
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// APP用户账号
        /// </summary>
        public string Data1 { get; set; }
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 施工开始日期 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Start_Date { get; set; }
        /// <summary>
        /// 工程日志编号，由APP端编号
        /// </summary>
        public int Project_Log_No { get; set; }
    }

    /// <summary>
    /// 图片上传
    /// </summary>
    public class AddPic
    {
        /// <summary>
        /// 命令号41，工程现场图片上传
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// APP用户账号
        /// </summary>
        public string Data1 { get; set; }
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 施工开始时间
        /// </summary>
        public string Start_Date { get; set; }
        /// <summary>
        /// 要上传的图片内容数组
        /// </summary>
        public List<Project_Pic> Project_Pic { get; set; }
    }

    /// <summary>
    /// Project_Pic数组
    /// </summary>
    public class Project_Pic
    {
        /// <summary>
        /// 图片名称，该值显示为数据库字段名
        /// </summary>
        public string Pic_Name { get; set; }
        /// <summary>
        /// 图片内容
        /// </summary>
        public string Pic_Content { get; set; }
    }

    /// <summary>
    /// RTU实时数据查询
    /// </summary>
    public class SelectRTUData
    {
        /// <summary>
        /// 命令号61，查询RTU实时数据
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// APP用户账号
        /// </summary>
        public string Data1 { get; set; }
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
    }

    /// <summary>
    /// 数据浏览
    /// </summary>
    public class SelectData
    {
        /// <summary>
        /// 数据准备，命令号52
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// 结果集名称
        /// </summary>
        public string ResultSet_Name { get; set; }
        /// <summary>
        /// 结果集数据起始行ID
        /// </summary>
        public int Row_First { get; set; }
        /// <summary>
        /// 结果集数据结束行ID
        /// </summary>
        public int Row_Last { get; set; }
    }

    /// <summary>
    /// 工程信息图片查询
    /// </summary>
    public class SelectProjectPic
    {
        /// <summary>
        /// 命令号61，查询RTU实时数据
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// APP用户账号
        /// </summary>
        public string Data1 { get; set; }
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 施工开始时间
        /// </summary>
        public string Start_Date { get; set; }
        /// <summary>
        /// 图片名称，该值显示为数据库字段名
        /// </summary>
        public string Project_Pic_Name { get; set; }
    }
    #endregion

    #region 回应信息

    /// <summary>
    /// 回应类型1，登录验证11、
    /// </summary>
    public class Res1
    {
        /// <summary>
        /// 命令号
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public string TEL { get; set; }
        /// <summary>
        /// 施工单位名称
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// 账号权限；0为APP管理员权限，1为场地录入人员权限
        /// </summary>
        public int Account_Permission { get; set; }
        /// <summary>
        /// 成功为0
        /// </summary>
        public int Error { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error_Message { get; set; }
    }

    /// <summary>
    /// 回应类型2，工程信息新增21、工程信息上传22、工程信息删除24、工程日志上传31、工程日志删除34、图片上传、登录失败11、
    /// </summary>
    public class Res2
    {
        /// <summary>
        /// 命令号
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// 成功为0
        /// </summary>
        public int Error { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error_Message { get; set; }
    }

    /// <summary>
    /// 回应类型3,工程概要信息查询23、工程日志查询、
    /// </summary>
    public class Res3
    {
        /// <summary>
        /// 命令号
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// 结果集名称
        /// </summary>
        public string ResultSet_Name { get; set; }
        /// <summary>
        /// 结果集总条目数
        /// </summary>
        public int Rows_Num { get; set; }
        /// <summary>
        /// 成功为0
        /// </summary>
        public int Error { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error_Message { get; set; }
    }

    /// <summary>
    /// 回应类型4,工程详细信息查询25、
    /// </summary>
    public class Res4
    {
        /// <summary>
        /// 命令号
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// 成功为0
        /// </summary>
        public int Error { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error_Message { get; set; }
        /// <summary>
        /// 工程信息表主键，GUID
        /// </summary>
        public string Project_ID { get; set; }
        /// <summary>
        /// 施工开始日期；不可修改  yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Start_Date { get; set; }
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
        ///// <summary>
        ///// 技术负责人头像
        ///// </summary>
        //public string Team_Leader_Picture { get; set; }
        /// <summary>
        /// 现场施工人员名
        /// </summary>
        public string Team_Worker_Name { get; set; }
        /// <summary>
        /// 现场施工人员联系电话
        /// </summary>
        public string Team_Worker_Tel { get; set; }
        ///// <summary>
        ///// 现场施工人员头像
        ///// </summary>
        //public string Team_Worker_Picture { get; set; }
        /// <summary>
        /// 施工单位名称
        /// </summary>
        public string Company_Name { get; set; }
        ///// <summary>
        ///// 现场标准化场地全景图
        ///// </summary>
        //public string Con_Begin_Pic1 { get; set; }
        ///// <summary>
        ///// 现场堵剂摆放那个图片
        ///// </summary>
        //public string Con_Begin_Pic2 { get; set; }
        ///// <summary>
        ///// 消防摆放、安全警示团片
        ///// </summary>
        //public string Con_Begin_Pic3 { get; set; }
        ///// <summary>
        ///// 变压器摆放图
        ///// </summary>
        //public string Con_Begin_Pic4 { get; set; }
        ///// <summary>
        ///// 变电柜接电处图片
        ///// </summary>
        //public string Con_Begin_Pic5 { get; set; }
        ///// <summary>
        ///// 注水井口图片
        ///// </summary>
        //public string Con_Begin_Pic6 { get; set; }
        ///// <summary>
        ///// 开工许可证图片
        ///// </summary>
        //public string Con_Begin_Pic7 { get; set; }
        ///// <summary>
        ///// 井控证图片
        ///// </summary>
        //public string Con_Begin_Pic8 { get; set; }
        ///// <summary>
        ///// 施工设计图片
        ///// </summary>
        //public string Con_Begin_Pic9 { get; set; }
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
        public string End_Date { get; set; }
        /// <summary>
        /// 措施后注水井油压MPa
        /// </summary>
        public float Con_End_CasingPressure { get; set; }
        /// <summary>
        /// 措施后日注水量m3/d
        /// </summary>
        public float Con_End_Dayinflow { get; set; }
        /// <summary>
        /// 措施后无套压原因
        /// </summary>
        public string CasPre_Cause { get; set; }
        ///// <summary>
        ///// 措施后井场全景图
        ///// </summary>
        //public string Con_End_Pic1 { get; set; }
        /// <summary>
        /// Project子数组；堵剂数量数组
        /// </summary>
        public List<Jam> Jam { get; set; }
        /// <summary>
        /// 工程状态；0未结束、1已结束
        /// </summary>
        public int Project_State { get; set; }
    }

    /// <summary>
    /// 回应类型5,RTU数据实时查询61、
    /// </summary>
    public class Res5
    {
        /// <summary>
        /// 命令号
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// 成功为0
        /// </summary>
        public int Error { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error_Message { get; set; }
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 排量m3/h
        /// </summary>
        public float Displacement { get; set; }
        /// <summary>
        /// 累计注入量m3
        /// </summary>
        public float Displacement_Acc { get; set; }
        /// <summary>
        /// 压力MPa
        /// </summary>
        public float Pressure { get; set; }
        /// <summary>
        /// 检测时间
        /// </summary>
        public string Detect_Time { get; set; }

        /// <summary>
        /// 指示查询的RTU是否正常连接到云平台；1为已连接,0为未连接
        /// </summary>
        public int RTU_Connected { get; set; }
        /// <summary>
        /// 指示查询的RTU是否已启用；1为已启用，0为未启用（该值可表示为RTU是否正在工程中被使用）
        /// </summary>
        public int RTU_EnableState { get; set; }
    }

    /// <summary>
    /// 回应类型6,数据浏览52(工程概要信息浏览)
    /// </summary>
    public class Res6
    {
        /// <summary>
        /// 命令号
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// 成功为0
        /// </summary>
        public int Error { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error_Message { get; set; }
        /// <summary>
        /// 应答给内网的结果集名称
        /// </summary>
        public string ResultSet_Name { get; set; }
        /// <summary>
        /// 本次上传的结果集条目数
        /// </summary>
        public int Rows_Num { get; set; }
        /// <summary>
        /// 结果集内容 Project
        /// </summary>
        public List<ResultSet_Data1> ResultSet_Data { get; set; }
    }

    /// <summary>
    /// 回应类型7,数据浏览52(工程日志查询)
    /// </summary>
    public class Res7
    {
        /// <summary>
        /// 命令号
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// 成功为0
        /// </summary>
        public int Error { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error_Message { get; set; }
        /// <summary>
        /// 应答给内网的结果集名称
        /// </summary>
        public string ResultSet_Name { get; set; }
        /// <summary>
        /// 本次上传的结果集条目数
        /// </summary>
        public int Rows_Num { get; set; }
        /// <summary>
        /// 结果集内容 Project_Log
        /// </summary>
        public List<ResultSet_Data2> ResultSet_Data { get; set; }
    }

    /// <summary>
    /// 回应类型8,工程信息图片查询
    /// </summary>
    public class Res8
    {
        /// <summary>
        /// 命令号
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// 成功为0
        /// </summary>
        public int Error { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error_Message { get; set; }
        /// <summary>
        /// 图片所在数据库字段名称
        /// </summary>
        public string Project_Pic_Name { get; set; }
        /// <summary>
        /// 图片内容
        /// </summary>
        public string Project_Pic_Content { get; set; }

    }

    /// <summary>
    /// 结果集内容(工程概要信息查询)
    /// </summary>
    public class ResultSet_Data1
    {
        /// <summary>
        /// 施工开始日期；不可修改  yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Start_Date { get; set; }
        /// <summary>
        /// RTU编号；不可修改
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 施工井号；不可修改
        /// </summary>
        public string Construction_Well_ID { get; set; }
        /// <summary>
        /// 施工结束日期
        /// </summary>
        public string End_Date { get; set; }

    }

    /// <summary>
    /// 结果集内容(工程日志查询)
    /// </summary>
    public class ResultSet_Data2
    {
        /// <summary>
        /// 工程日志主键，GUID
        /// </summary>
        public string Project_Log_ID { get; set; }
        /// <summary>
        /// 工程日志编号
        /// </summary>
        public int Project_Log_No { get; set; }
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 施工开始时间
        /// </summary>
        public string Start_Date { get; set; }
        /// <summary>
        /// 施工时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Construction_Time { get; set; }
        /// <summary>
        /// 压力MPa
        /// </summary>
        public float Pressure { get; set; }
        /// <summary>
        /// 排量m3/h
        /// </summary>
        public float Displacement { get; set; }
        /// <summary>
        /// 累计注入量m3
        /// </summary>
        public float Displacement_Acc { get; set; }
        /// <summary>
        /// 段塞名
        /// </summary>
        public string Slug_Name { get; set; }
        /// <summary>
        /// 堵剂配方
        /// </summary>
        public string Formula { get; set; }
        /// <summary>
        /// 事件
        /// </summary>
        public string Events { get; set; }
        /// <summary>
        /// 最后操作编号，0新增、1修改、2删除
        /// </summary>
        public int Last_Operate_Type { get; set; }
    }
    #endregion
}
