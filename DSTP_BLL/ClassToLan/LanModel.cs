using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSTP_BLL.ClassToLan
{
    /// <summary>
    /// 接收Json数据(命令号以及指令序号)
    /// </summary>
    public class LanModel
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
    /// 登录信息0
    /// </summary>
    public class Login
    {
        /// <summary>
        /// 命令号0，验证登录信息
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// 内网用户账号
        /// </summary>
        public string Data1 { get; set; }
        /// <summary>
        /// 内网用户密码
        /// </summary>
        public string Data2 { get; set; }
    }

    /// <summary>
    /// APP账号信息读取1
    /// </summary>
    public class ReadAppAccount
    {
        /// <summary>
        /// 命令号1，APP账号信息读取
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// 查询标记，表示是否全部接收数据；0为全部接收，1为接收未上传数据
        /// </summary>
        public int Data_Tag { get; set; }
    }

    /// <summary>
    /// 工程信息读取3
    /// </summary>
    public class ReadProject
    {
        /// <summary>
        /// 命令号3，工程信息读取
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 施工井号
        /// </summary>
        public string Construction_Well_ID { get; set; }
        /// <summary>
        /// 按更新时间查询，起始时间yyyy-MM-dd
        /// </summary>
        public string Start_Time { get; set; }
        /// <summary>
        /// 按更新时间查询，结束时间yyyy-MM-dd
        /// </summary>
        public string End_Time { get; set; }
        /// <summary>
        /// 查询标记，表示是否全部接收数据；0为全部接收，1为接收未上传数据
        /// </summary>
        public int Data_Tag { get; set; }
    }

    /// <summary>
    /// 工程日志读取4
    /// </summary>
    public class ReadProject_Log
    {
        /// <summary>
        /// 命令号4，工程日志读取
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 施工开始时间；yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Construction_Well_ID { get; set; }
        /// <summary>
        /// 施工井号
        /// </summary>
        public string Start_Date { get; set; }
        /// <summary>
        /// 按更新时间查询，起始时间yyyy-MM-dd
        /// </summary>
        public string Start_Time { get; set; }
        /// <summary>
        /// 按更新时间查询，结束时间yyyy-MM-dd
        /// </summary>
        public string End_Time { get; set; }
        /// <summary>
        /// 查询标记，表示是否全部接收数据；0为全部接收，1为接收未上传数据
        /// </summary>
        public int Data_Tag { get; set; }
    }

    /// <summary>
    /// RTU数据读取5
    /// </summary>
    public class ReadRTU_Data
    {
        /// <summary>
        /// 命令号5，读取RTU数据
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 按更新时间查询，起始时间yyyy-MM-dd
        /// </summary>
        public string Start_Time { get; set; }
        /// <summary>
        /// 按更新时间查询，结束时间yyyy-MM-dd
        /// </summary>
        public string End_Time { get; set; }
        /// <summary>
        /// 查询标记，表示是否全部接收数据；0为全部接收，1为接收未上传数据
        /// </summary>
        public int Data_Tag { get; set; }
    }

    /// <summary>
    /// RTU图片接收7
    /// </summary>
    public class ReadRTU_Pic
    {
        /// <summary>
        /// 命令号7，RTU图片接收
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 按更新时间查询，起始时间yyyy-MM-dd
        /// </summary>
        public string Start_Time { get; set; }
        /// <summary>
        /// 按更新时间查询，结束时间yyyy-MM-dd
        /// </summary>
        public string End_Time { get; set; }
        /// <summary>
        /// 查询标记，表示是否全部接收数据；0为全部接收，1为接收未上传数据
        /// </summary>
        public int Data_Tag { get; set; }
    }

    /// <summary>
    /// APP账号修改2
    /// </summary>
    public class UpdateAPPAccount
    {
        /// <summary>
        /// 命令号2，APP账号设置
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// APP账号数组
        /// </summary>
        public List<Account> Account { get; set; }
    }

    /// <summary>
    /// RTU参数设置6
    /// </summary>
    public class UpdateRTUPara
    {
        /// <summary>
        /// 命令号6，RTU参数设置
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
        /// <summary>
        /// 设置RTU状态；1正常使用，2停用，3检修
        /// </summary>
        public int RTU_IsTrue { get; set; }
        /// <summary>
        /// 设置周期，设置周期内上传数据，以分钟为单位
        /// </summary>
        public int RTU_Cycle_Seconds { get; set; }
    }

    /// <summary>
    /// 触发RTU拍照8
    /// </summary>
    public class RTUNowPic
    {
        /// <summary>
        /// 命令号8，触发拍照
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }
    }

    /// <summary>
    /// 数据浏览52
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
    /// 事务结束9
    /// </summary>
    public class SelectDataEnd
    {
        /// <summary>
        /// 验证操作指令，命令号9
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
        /// <summary>
        /// 验证操作是否成功，这里默认1为失败，0为成功
        /// </summary>
        public int Error { get; set; }
    }

    /// <summary>
    /// 心跳包100
    /// </summary>
    public class HeartbBeatData
    {
        /// <summary>
        /// 心跳包发送，命令号100
        /// </summary>
        public int Command_Type { get; set; }
    }

    /// <summary>
    /// 工程信息图片读取11
    /// </summary>
    public class SelectProjectPic
    {
        /// <summary>
        /// 命令号11，工程信息图片读取
        /// </summary>
        public int Command_Type { get; set; }
        /// <summary>
        /// 指令序号
        /// </summary>
        public int Command_SN { get; set; }
    }
    #endregion


    #region 回应信息
    /// <summary>
    /// 回应类型1，登录验证0
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
        /// 成功为0
        /// </summary>
        public int Error { get; set; }
        private string _Error_Message;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error_Message
        {
            get
            {
                return _Error_Message;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Error_Message = value;
                }
            }
        }
    }

    /// <summary>
    /// 回应类型2，app账号信息读取1、工程信息读取3、工程日志读取4、RTU数据读取5、RTU图片读取7、工程信息图片读取11
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
    }

    /// <summary>
    /// 回应类型3，app账号信息读取1、工程信息读取3、工程日志读取4、RTU数据读取5、RTU图片读取7、APP账号修改、RTU参数设置、触发RTU拍照、工程信息图片读取11、
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
        /// 成功为0
        /// </summary>
        public int Error { get; set; }
    }

    /// <summary>
    /// 回应类型4,数据浏览52(APP账号信息读取)
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
        public List<Account> ResultSet_Data { get; set; }
    }

    /// <summary>
    /// 回应类型5,数据浏览5(工程信息读取)
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
        public List<Project> ResultSet_Data { get; set; }
    }

    /// <summary>
    /// 回应类型6,数据浏览52(工程日志读取)
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
        private string _Error_Message;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error_Message
        {
            get
            {
                return _Error_Message;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Error_Message = value;
                }
            }
        }

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
        public List<Project_Log> ResultSet_Data { get; set; }
    }

    /// <summary>
    /// 回应类型7,数据浏览52(RTU数据读取)
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
        public List<RTU_Data> ResultSet_Data { get; set; }
    }

    /// <summary>
    /// 回应类型8,数据浏览52(RTU图片读取)
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
        public List<RTU_Pic> ResultSet_Data { get; set; }
    }

    /// <summary>
    /// 回应类型9,数据浏览52(工程信息图片读取)
    /// </summary>
    public class Res9
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
        public List<Project_pic> ResultSet_Data { get; set; }
    }
    #endregion


    #region 数组
    /// <summary>
    /// APP账号数组
    /// </summary>
    public class Account
    {
        private string _Account_ID;
        /// <summary>
        /// 账号表主键，GUID
        /// </summary>
        public string Account_ID
        {
            get
            {
                return _Account_ID;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Account_ID = value;
                }
            }
        }
        private string _Account_Name;
        /// <summary>
        /// 账号
        /// </summary>
        public string Account_Name
        {
            get
            {
               return _Account_Name;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Account_Name = value;
                }
            }
        }
        private string _Account_Password;
        /// <summary>
        /// 密码
        /// </summary>
        public string Account_Password
        {
            get
            {
                return _Account_Password;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Account_Password = value;
                }
            }
        }
        /// <summary>
        /// 注册时间  yyyy-MM-dd
        /// </summary>
        public string Account_Time { get; set; }
        private string _Name;
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Name = value;
                }
            }
        }
        private string _TEL;
        /// <summary>
        /// 联系方式
        /// </summary>
        public string TEL
        {
            get
            {
                return _TEL;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _TEL = value;
                }
            }
        }
        private string _Company;
        /// <summary>
        /// 施工单位名称
        /// </summary>
        public string Company
        {
            get
            {
                return _Company;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Company = value;
                }
            }
        }
        private string _Account_Picture;
        /// <summary>
        /// 头像
        /// </summary>
        public string Account_Picture
        {
            get
            {
                return _Account_Picture;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Account_Picture = value;
                }
            }
        }
        /// <summary>
        /// 账号权限；0为APP管理员权限，1为场地录入人员权限，2为内网使用者权限
        /// </summary>
        public int Account_Permission { get; set; }
        /// <summary>
        /// 最后操作时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Last_Operate_Date { get; set; }
        /// <summary>
        /// 最后操作；0新增、1修改、2删除
        /// </summary>
        public int Last_Operate_Type { get; set; }
        /// <summary>
        /// 是否使用；0已使用，1未使用
        /// </summary>
        public int Account_IsUse { get; set; }
    }

    /// <summary>
    /// 工程信息对象
    /// </summary>
    public class Project
    {
        private string _Project_ID;
        /// <summary>
        /// 工程信息表主键，GUID
        /// </summary>
        public string Project_ID
        {
            get
            {
                return _Project_ID;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Project_ID = value;
                }
            }
        }

        private string _Account_ID;
        /// <summary>
        /// 账号ID
        /// </summary>
        public string Account_ID
        {
            get
            {
                return _Account_ID;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Account_ID = value;
                }
            }
        }
        /// <summary>
        /// 施工开始日期；不可修改  yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Start_Date { get; set; }
        private string _Jar_Shape;
        /// <summary>
        /// 罐体形状
        /// </summary>
        public string Jar_Shape
        {
            get
            {
                return _Jar_Shape;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Jar_Shape = value;
                }
            }
        }
        private string _Jar_Size;
        /// <summary>
        /// 尺寸
        /// </summary>
        public string Jar_Size
        {
            get
            {
                return _Jar_Size;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Jar_Size = value;
                }
            }
        }
        /// <summary>
        /// 体积
        /// </summary>
        public float Jar_Volume { get; set; }
        /// <summary>
        /// RTU编号；不可修改
        /// </summary>
        public int RTU_No { get; set; }
        private string _Construction_Name;
        /// <summary>
        /// 措施名称
        /// </summary>
        public string Construction_Name
        {
            get
            {
                return _Construction_Name;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Construction_Name = value;
                }
            }
        }
        private string _Construction_Well_ID;
        /// <summary>
        /// 施工井号；不可修改
        /// </summary>
        public string Construction_Well_ID
        {
            get
            {
                return _Construction_Well_ID;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Construction_Well_ID = value;
                }
            }
        }
        private string _Oil_Factory;
        /// <summary>
        /// 采油厂
        /// </summary>
        public string Oil_Factory
        {
            get
            {
                return _Oil_Factory;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Oil_Factory = value;
                }
            }
        }
        private string _Area;
        /// <summary>
        /// 区块
        /// </summary>
        public string Area
        {
            get
            {
                return _Area;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Area = value;
                }
            }
        }
        private string _Place;
        /// <summary>
        /// 施工地点
        /// </summary>
        public string Place
        {
            get
            {
                return _Place;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Place = value;
                }
            }
        }
        private string _Team_Leader_Name;
        /// <summary>
        /// 技术负责人名称
        /// </summary>
        public string Team_Leader_Name
        {
            get
            {
                return _Team_Leader_Name;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Team_Leader_Name = value;
                }
            }
        }
        private string _Team_Leader_Tel;
        /// <summary>
        /// 技术负责人联系电话
        /// </summary>
        public string Team_Leader_Tel
        {
            get
            {
                return _Team_Leader_Tel;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Team_Leader_Tel = value;
                }
            }
        }
        //private string _Team_Leader_Picture;
        ///// <summary>
        ///// 技术负责人头像
        ///// </summary>
        //public string Team_Leader_Picture
        //{
        //    get
        //    {
        //        return _Team_Leader_Picture;
        //    }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            value = "";
        //        }
        //        else
        //        {
        //            _Team_Leader_Picture = value;
        //        }
        //    }
        //}
        private string _Team_Worker_Name;
        /// <summary>
        /// 现场施工人员名
        /// </summary>
        public string Team_Worker_Name
        {
            get
            {
                return _Team_Worker_Name;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Team_Worker_Name = value;
                }
            }
        }
        private string _Team_Worker_Tel;
        /// <summary>
        /// 现场施工人员联系电话
        /// </summary>
        public string Team_Worker_Tel
        {
            get
            {
                return _Team_Worker_Tel;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Team_Worker_Tel = value;
                }
            }
        }
        //private string _Team_Worker_Picture;
        ///// <summary>
        ///// 现场施工人员头像
        ///// </summary>
        //public string Team_Worker_Picture
        //{
        //    get
        //    {
        //        return _Team_Worker_Picture;
        //    }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            value = "";
        //        }
        //        else
        //        {
        //            _Team_Worker_Picture = value;
        //        }
        //    }
        //}
        private string _Company_Name;
        /// <summary>
        /// 施工单位名称
        /// </summary>
        public string Company_Name
        {
            get
            {
                return _Company_Name;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Company_Name = value;
                }
            }
        }
        //private string _Con_Begin_Pic1;
        ///// <summary>
        ///// 现场标准化场地全景图
        ///// </summary>
        //public string Con_Begin_Pic1
        //{
        //    get
        //    {
        //        return _Con_Begin_Pic1;
        //    }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            value = "";
        //        }
        //        else
        //        {
        //            _Con_Begin_Pic1 = value;
        //        }
        //    }
        //}
        //private string _Con_Begin_Pic2;
        ///// <summary>
        ///// 现场堵剂摆放那个图片
        ///// </summary>
        //public string Con_Begin_Pic2
        //{
        //    get
        //    {
        //        return _Con_Begin_Pic2;
        //    }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            value = "";
        //        }
        //        else
        //        {
        //            _Con_Begin_Pic2 = value;
        //        }
        //    }
        //}
        //private string _Con_Begin_Pic3;
        ///// <summary>
        ///// 消防摆放、安全警示团片
        ///// </summary>
        //public string Con_Begin_Pic3
        //{
        //    get
        //    {
        //        return _Con_Begin_Pic3;
        //    }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            value = "";
        //        }
        //        else
        //        {
        //            _Con_Begin_Pic3 = value;
        //        }
        //    }
        //}
        //private string _Con_Begin_Pic4;
        ///// <summary>
        ///// 变压器摆放图
        ///// </summary>
        //public string Con_Begin_Pic4
        //{
        //    get
        //    {
        //        return _Con_Begin_Pic4;
        //    }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            value = "";
        //        }
        //        else
        //        {
        //            _Con_Begin_Pic4 = value;
        //        }
        //    }
        //}
        //private string _Con_Begin_Pic5;
        ///// <summary>
        ///// 变电柜接电处图片
        ///// </summary>
        //public string Con_Begin_Pic5
        //{
        //    get
        //    {
        //        return _Con_Begin_Pic5;
        //    }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            value = "";
        //        }
        //        else
        //        {
        //            _Con_Begin_Pic5 = value;
        //        }
        //    }
        //}
        //private string _Con_Begin_Pic6;
        ///// <summary>
        ///// 注水井口图片
        ///// </summary>
        //public string Con_Begin_Pic6
        //{
        //    get
        //    {
        //        return _Con_Begin_Pic6;
        //    }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            value = "";
        //        }
        //        else
        //        {
        //            _Con_Begin_Pic6 = value;
        //        }
        //    }
        //}
        //private string _Con_Begin_Pic7;
        ///// <summary>
        ///// 开工许可证图片
        ///// </summary>
        //public string Con_Begin_Pic7
        //{
        //    get
        //    {
        //        return _Con_Begin_Pic7;
        //    }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            value = "";
        //        }
        //        else
        //        {
        //            _Con_Begin_Pic7 = value;
        //        }
        //    }
        //}
        //private string _Con_Begin_Pic8;
        ///// <summary>
        ///// 井控证图片
        ///// </summary>
        //public string Con_Begin_Pic8
        //{
        //    get
        //    {
        //        return _Con_Begin_Pic8;
        //    }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            value = "";
        //        }
        //        else
        //        {
        //            _Con_Begin_Pic8 = value;
        //        }
        //    }
        //}
        //private string _Con_Begin_Pic9;
        ///// <summary>
        ///// 施工设计图片
        ///// </summary>
        //public string Con_Begin_Pic9
        //{
        //    get
        //    {
        //        return _Con_Begin_Pic9;
        //    }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            value = "";
        //        }
        //        else
        //        {
        //            _Con_Begin_Pic9 = value;
        //        }
        //    }
        //}
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
        private string _CasPre_Cause;
        /// <summary>
        /// 措施后无套压原因
        /// </summary>
        public string CasPre_Cause
        {
            get
            {
                return _CasPre_Cause;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _CasPre_Cause = value;
                }
            }
        }
        //private string _Con_End_Pic1;
        ///// <summary>
        ///// 措施后井场全景图
        ///// </summary>
        //public string Con_End_Pic1
        //{
        //    get
        //    {
        //        return _Con_End_Pic1;
        //    }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            value = "";
        //        }
        //        else
        //        {
        //            _Con_End_Pic1 = value;
        //        }
        //    }
        //}
        /// <summary>
        /// Project子数组；堵剂数量数组
        /// </summary>
        public List<Jam> Jam { get; set; }
        /// <summary>
        /// 最后操作；0新增、1修改、2删除
        /// </summary>
        public int Last_Operate_Type { get; set; }
        /// <summary>
        /// 最后操作时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Last_Operate_Date { get; set; }
    }

    /// <summary>
    /// Project子数组；堵剂数量数组
    /// </summary>
    public class Jam
    {
        private string _Jam_ID;
        /// <summary>
        /// 堵剂数量ID，GUID
        /// </summary>
        public string Jam_ID
        {
            get
            {
                return _Jam_ID;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Jam_ID = value;
                }
            }
        }
        private string _Jam_Name;
        /// <summary>
        /// 堵剂名称（化学名称）
        /// </summary>
        public string Jam_Name
        {
            get
            {
                return _Jam_Name;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Jam_Name = value;
                }
            }
        }
        /// <summary>
        /// 数量（袋或桶）
        /// </summary>
        public float Jam_Num { get; set; }
    }

    /// <summary>
    /// Project_Log数组
    /// </summary>
    public class Project_Log
    {
        private string _Project_Log_ID;
        /// <summary>
        /// 工程日志主键，GUID
        /// </summary>
        public string Project_Log_ID
        {
            get
            {
                return _Project_Log_ID;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Project_Log_ID = value;
                }
            }
        }
        private string _Project_ID;
        /// <summary>
        /// 工程信息表主键
        /// </summary>
        public string Project_ID
        {
            get
            {
                return _Project_ID;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Project_ID = value;
                }
            }
        }
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
        private string _Slug_Name;
        /// <summary>
        /// 段塞名
        /// </summary>
        public string Slug_Name
        {
            get
            {
                return _Slug_Name;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Slug_Name = value;
                }
            }
        }
        private string _Formula;
        /// <summary>
        /// 堵剂配方
        /// </summary>
        public string Formula
        {
            get
            {
                return _Formula;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Formula = value;
                }
            }
        }
        private string _Events;
        /// <summary>
        /// 事件
        /// </summary>
        public string Events
        {
            get
            {
                return _Events;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Events = value;
                }
            }
        }
        /// <summary>
        /// 最后操作；0新增、1修改、2删除
        /// </summary>
        public int Last_Operate_Type { get; set; }
        /// <summary>
        /// 最后操作时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Last_Operate_Date { get; set; }
    }

    /// <summary>
    /// RTU数据数组
    /// </summary>
    public class RTU_Data
    {
        private string _RTU_Data_ID;
        /// <summary>
        /// RTU数据表ID，GUID
        /// </summary>
        public string RTU_Data_ID
        {
            get
            {
                return _RTU_Data_ID;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _RTU_Data_ID = value;
                }
            }
        }
        /// <summary>
        /// RTU设备号
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
        /// 检测时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Detect_Time { get; set; }
        /// <summary>
        /// 上传时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Upload_Time { get; set; }
    }

    /// <summary>
    /// RTU图片数组
    /// </summary>
    public class RTU_Pic
    {
        private string _RTU_Pic_ID;
        /// <summary>
        /// RTU图片ID，GUID
        /// </summary>
        public string RTU_Pic_ID
        {
            get
            {
                return _RTU_Pic_ID;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _RTU_Pic_ID = value;
                }
            }
        }
        private string _Pic_Name;
        /// <summary>
        /// 图片编号；RTU编号+上传时间
        /// </summary>
        public string Pic_Name
        {
            get
            {
                return _Pic_Name;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Pic_Name = value;
                }
            }
        }
        /// <summary>
        /// RTU设备编号
        /// </summary>
        public int RTU_No { get; set; }
        private string _RTU_Pic_Content;
        /// <summary>
        /// 图片
        /// </summary>
        public string RTU_Pic_Content
        {
            get
            {
                return _RTU_Pic_Content;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _RTU_Pic_Content = value;
                }
            }
        }
        /// <summary>
        /// 检测时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Detect_Time { get; set; }
        /// <summary>
        /// 上传时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Upload_Time { get; set; }
    }

    /// <summary>
    /// 工程信息图片数组
    /// </summary>
    public class Project_pic
    {
        private string _Project_ID;
        /// <summary>
        /// 工程信息表主键，GUID
        /// </summary>
        public string Project_ID
        {
            get
            {
                return _Project_ID;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Project_ID = value;
                }
            }
        }

        /// <summary>
        /// RTU编号
        /// </summary>
        public int RTU_No { get; set; }

        /// <summary>
        /// 施工开始时间
        /// </summary>
        public string Start_Date { get; set; }

        private string _Project_Pic_Name;
        /// <summary>
        /// 图片在数据库中的字段名称
        /// </summary>
        public string Project_Pic_Name
        {
            get
            {
                return _Project_Pic_Name;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Project_Pic_Name = value;
                }
            }
        }

        private string _Project_Pic_Content;
        /// <summary>
        /// 图片内容
        /// </summary>
        public string Project_Pic_Content
        {
            get
            {
                return _Project_Pic_Content;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                else
                {
                    _Project_Pic_Content = value;
                }
            }
        }
    }
    #endregion

}
