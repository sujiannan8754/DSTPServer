using DSTP_BLL.ClassToRtu;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DSTP_DAL;
using System.IO;

namespace DSTPServer
{
    public partial class DSTP : Form
    {
        #region 字段
        /// <summary>
        /// 实例化RTU操作类
        /// </summary>
        ConnectToRTU RTU;
        ConnectToAPP APP;
        ConnectToLan Lan;

        public ClassAppConfigOptions appConfig; //应用程序参数配置

        string ServerIP;

        string LocalPortRTU;
        string LocalPortAPP;
        string LocalPortLan;

        string DtuSVRPWD;

        string bDtuSVRPWD;

        /// <summary>
        /// 心跳包开关
        /// </summary>
        string bHeartBeat;

        /// <summary>
        /// RTU连接后是否发送心跳包
        /// </summary>
        string bStartHeartBeat;

        /// <summary>
        /// 心跳包间隔
        /// </summary>
        string HeartBeatTime;

        /// <summary>
        /// 读取图片超时时间
        /// </summary>
        string ImageFileTimeOut;

        /// <summary>
        /// RTU默认接收数据超时断开连接时间
        /// </summary>
        string RTUReceivedTime;

        /// <summary>
        /// 内网默认接收数据超时断开连接时间
        /// </summary>
        string LanReceivedTime;

        public ConnectDBManage ConDBM;
        #endregion

        public DSTP()
        {
            InitializeComponent();

            ConDBM = new ConnectDBManage(); //初始化数据库连接,互斥体实例化

            RTU = new ConnectToRTU();
            APP = new ConnectToAPP();
            Lan = new ConnectToLan();

            #region 读取配置文件
            appConfig = new ClassAppConfigOptions();

            //设置服务器IP地址以及端口
            appConfig.readConfig("LocalIp", out ServerIP);
            appConfig.readConfig("LocalPortRTU", out LocalPortRTU);
            appConfig.readConfig("LocalPortAPP", out LocalPortAPP);
            appConfig.readConfig("LocalPortLan", out LocalPortLan);

            //读取Dtu密码
            appConfig.readConfig("DtuSVRPWD", out DtuSVRPWD);
            //读取图片超时时间
            appConfig.readConfig("ImageFileTimeOut", out ImageFileTimeOut);
            //读取是否验证DTU注册包密码
            appConfig.readConfig("bDtuSVRPWD", out bDtuSVRPWD);

            //读取心跳包开关
            appConfig.readConfig("bHeartBeat", out bHeartBeat);

            //读取RTU连接后是否发送心跳包
            appConfig.readConfig("bStartHeartBeat", out bStartHeartBeat);

            //读取心跳包间隔
            appConfig.readConfig("HeartBeatTime", out HeartBeatTime);

            //RTU默认接收数据超时断开连接时间
            appConfig.readConfig("RTUReceivedTime", out RTUReceivedTime);

            //内网默认接收数据超时断开连接时间
            appConfig.readConfig("LanReceivedTime", out LanReceivedTime);
            #endregion

            RTU.Begin_Listen(ServerIP, int.Parse(LocalPortRTU), DtuSVRPWD, bDtuSVRPWD, bHeartBeat, bStartHeartBeat, HeartBeatTime, RTUReceivedTime, Convert.ToInt32(ImageFileTimeOut) * 1000, this);
            writeDebugBox(" 开始监听RTU设备", 0);

            APP.Begin_Listen(ServerIP, int.Parse(LocalPortAPP), this);
            writeDebugBox(" 开始监听APP", 2);

            Lan.Begin_Listen(bHeartBeat, HeartBeatTime, LanReceivedTime, ServerIP, int.Parse(LocalPortLan), this);
            writeDebugBox(" 开始监听内网", 3);
        }


        #region Lan、RTU、APP
        /// <summary>
        /// 写txtDebug函数
        /// </summary>
        /// <param name="strText">需要写入的字符串</param>
        /// <param name="writeMode">0：写RTU消息，1：写在线RTU,2:写APP信息,3：写内网信息,4:清空在线RTU</param>
        public void writeDebugBox(string strText, int writeMode)
        {
            string DT = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                switch (writeMode)
                {
                    case 0:
                        listBox_RTU.Items.Add(DT + strText);
                        listBox_RTU.TopIndex = this.listBox_RTU.Items.Count - (int)(this.listBox_RTU.Height / this.listBox_RTU.ItemHeight);//通过计算ListBox显示的行数，设置TopIndex属性（ListBox中第一个可见项的索引）而达到目的。
                        break;

                    case 1:
                        listBox_RTU_No.Items.Add(strText);
                        break;

                    case 2:
                        listBox_APP.Items.Add(DT + strText);
                        listBox_APP.TopIndex = this.listBox_APP.Items.Count - (int)(this.listBox_APP.Height / this.listBox_APP.ItemHeight);
                        break;
                    case 3:
                        listBox_Lan.Items.Add(DT + strText);
                        listBox_Lan.TopIndex = this.listBox_Lan.Items.Count - (int)(this.listBox_Lan.Height / this.listBox_Lan.ItemHeight);
                        break;

                    case 4:
                        listBox_RTU_No.Items.Clear();
                        break;
                    case 5:

                        break;
                    case 6:
                        break;
                    case 7:

                        break;
                    case 11:

                        break;
                    case 12:

                        break;
                    default:

                        break;
                }


            }
            catch (System.Exception ex)
            {

            }
        }

        private void btn_Sign_Click(object sender, EventArgs e)
        {
            if (btn_Sign.Text == "关闭标识(测试用)")
            {
                Lan.bSelDataEnd = false;
                btn_Sign.Text = "打开标识(测试用)";
            }
            else
            {
                Lan.bSelDataEnd = true;
                btn_Sign.Text = "关闭标识(测试用)";
            }
        }

        private void btn_UpdateListBoxRTUNo_Click(object sender, EventArgs e)
        {
            RTU.UpdateRTUListBox();
        }
        #endregion


        #region 账号维护
        #region 字段
        AccountDAL AccDAL = new AccountDAL();
        DataTable Account;
        string AccountPicAddress = "";

        Image OldImage;
        Image NewImage;

        #endregion

        private void btn_display_Click(object sender, EventArgs e)
        {
            //更新表DGV_Account
            AccDAL.SelectAccount(out Account);
            DGV_Account.DataSource = Account;
            Account = null;
        }

        private void DGV_Account_SelectionChanged(object sender, EventArgs e)
        {
            //更新选中的行数据到文本框中
            try
            {
                if (this.DGV_Account.SelectedRows.Count > 0)
                {
                    if (DGV_Account.RowCount > 0)
                    {
                        if (DGV_Account.SelectedRows[0].Cells["Account_Picture"].Value.ToString() != "" && DGV_Account.SelectedRows[0].Cells["Account_Picture"].Value.ToString() != null)
                        {
                            string Address = Application.StartupPath + "\\AccountPic\\" + DGV_Account.SelectedRows[0].Cells["Account_Picture"].Value.ToString();
                            try
                            {
                                //将老图片给图片的蓝本
                                OldImage = Image.FromFile(Address);
                                Bitmap bitmap = new Bitmap(OldImage);
                                picbox_Account_Picture.Image = bitmap;
                                OldImage.Dispose();
                            }
                            catch (Exception ex)
                            {
                                picbox_Account_Picture.Image = null;
                            }
                        }
                        else
                        {
                            picbox_Account_Picture.Image = null;
                        }
                        txtBox_Account_Name.Text = DGV_Account.SelectedRows[0].Cells["Account_Name"].Value.ToString();
                        txtbox_Account_Password.Text = DGV_Account.SelectedRows[0].Cells["Account_Password"].Value.ToString();
                        txtbox_Name.Text = DGV_Account.SelectedRows[0].Cells["Name"].Value.ToString();
                        txtbox_Tel.Text = DGV_Account.SelectedRows[0].Cells["TEL"].Value.ToString();
                        txtbox_Company.Text = DGV_Account.SelectedRows[0].Cells["Company"].Value.ToString();
                        int Account_IsUse = Convert.ToInt32(DGV_Account.SelectedRows[0].Cells["Account_IsUse"].Value.ToString());
                        if (Account_IsUse == 0)
                        {
                            comboBox_IsUse.Text = "是";
                        }
                        else
                        {
                            comboBox_IsUse.Text = "否";
                        }
                        int Account_Permission = Convert.ToInt32(DGV_Account.SelectedRows[0].Cells["Account_Permission"].Value.ToString());
                        if (Account_Permission == 0)
                        {
                            comboBox_Permission.Text = "APP管理员";
                        }
                        if (Account_Permission == 1)
                        {
                            comboBox_Permission.Text = "APP操作员";
                        }
                        if (Account_Permission == 2)
                        {
                            comboBox_Permission.Text = "内网使用者";
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            Image myImg;

            //新增数据
            AccountModel AM = new AccountModel();
            AM.Account_ID = Guid.NewGuid().ToString();
            if (txtBox_Account_Name.Text == "")
            {
                AM.Account_Name = null;
            }
            else
            {
                AM.Account_Name = txtBox_Account_Name.Text;
            }
            if (txtbox_Account_Password.Text == "")
            {
                AM.Account_Password = null;
            }
            else
            {
                AM.Account_Password = txtbox_Account_Password.Text;
            }
            AM.Account_Time = DateTime.Now;
            AM.Name = txtbox_Name.Text;
            AM.TEL = txtbox_Tel.Text;
            AM.Company = txtbox_Company.Text;
            if (AccountPicAddress == "")
            {
                AM.Account_Picture = "";
            }
            else
            {
                try
                {
                    AM.Account_Picture = AM.Account_Name + ".jpg";
                    //将这个图片存起来
                    myImg = Image.FromFile(AccountPicAddress);
                    myImg.Save("AccountPic//" + AM.Account_Picture, System.Drawing.Imaging.ImageFormat.Jpeg);
                    myImg.Dispose();
                }
                catch (Exception ex)
                {
                    AM.Account_Picture = "";
                }
            }
            if (comboBox_IsUse.Text == "是")
            {
                AM.Account_IsUse = 0;
            }
            else
            {
                AM.Account_IsUse = 1;
            }
            if (comboBox_Permission.Text == "APP操作员")
            {
                AM.Account_Permission = 1;
            }
            else if (comboBox_Permission.Text == "APP管理员")
            {
                AM.Account_Permission = 0;
            }
            else
            {
                AM.Account_Permission = 2;
            }
            AM.Sign = 1;
            AM.Last_Operate_Date = DateTime.Now;
            AM.Last_Operate_Type = 0;

            string Error_message;
            AccDAL.AddAccount(AM, out Error_message);
            if (Error_message != "")
            {
                MessageBox.Show(Error_message);
                System.IO.File.Delete(AccountPicAddress);
            }
            else
            {
                //更新表DGV_Account
                AccDAL.SelectAccount(out Account);
                DGV_Account.DataSource = Account;
                Account = null;
            }
            AccountPicAddress = "";
        }

        private void ClearTextBox()
        {
            picbox_Account_Picture.Image = null;
            txtBox_Account_Name.Text = null;
            txtbox_Account_Password.Text = null;
            txtbox_Name.Text = null;
            txtbox_Tel.Text = null;
            txtbox_Company.Text = null;
            comboBox_IsUse.Text = "是";
            comboBox_Permission.Text = "APP操作员";
        }

        private void btn_Cleartxt_Click(object sender, EventArgs e)
        {
            ClearTextBox();
        }

        private void btn_sel_display_Click(object sender, EventArgs e)
        {
            //条件查询账号信息
            if (txtbox_sel_AccountName.Text == "" && txtbox_sel_Name.Text == "" && comboBox_sel_Permission.Text == "")
            {
                MessageBox.Show("选择某一项信息进行查询");
            }
            else
            {
                string Account_Name = txtbox_sel_AccountName.Text;
                string Name = txtbox_sel_Name.Text;
                int Account_Permission = 0;
                if (comboBox_sel_Permission.Text == "APP操作员")
                {
                    Account_Permission = 1;
                }
                else if (comboBox_sel_Permission.Text == "APP管理员")
                {
                    Account_Permission = 0;
                }
                else if (comboBox_sel_Permission.Text == "内网使用者")
                {
                    Account_Permission = 2;
                }
                else
                {
                    Account_Permission = -1;
                }

                string Error_message;
                DataTable Account;

                AccDAL.SelectAccountToCondition(Account_Name, Name, Account_Permission, out Error_message, out Account);

                if (Error_message == "")
                {
                    DGV_Account.DataSource = Account;
                }
                else
                {
                    MessageBox.Show(Error_message);
                }

            }
        }

        private void btn_update_Click(object sender, EventArgs e)
        {

            try
            {
                //修改账号信息
                AccountModel AM = new AccountModel();
                AM.Account_ID = DGV_Account.SelectedRows[0].Cells["Account_ID"].Value.ToString();
                if (txtBox_Account_Name.Text == "")
                {
                    AM.Account_Name = null;
                }
                else
                {
                    AM.Account_Name = txtBox_Account_Name.Text;
                }
                if (txtbox_Account_Password.Text == "")
                {
                    AM.Account_Password = null;
                }
                else
                {
                    AM.Account_Password = txtbox_Account_Password.Text;
                }
                AM.Name = txtbox_Name.Text;
                AM.TEL = txtbox_Tel.Text;
                AM.Company = txtbox_Company.Text;

                //图片的操作
                if (picbox_Account_Picture.Image == null)//如果没有图片了,数据库字段为空，把本地的图片删了（如果存在）
                {
                    AM.Account_Picture = "";
                    try
                    {
                        using (picbox_Account_Picture.Image)
                        {
                            File.Delete(Application.StartupPath + "\\AccountPic\\" + AM.Account_Name + ".jpg");
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else//如果有图片的话，将新图片覆盖到旧图片上去
                {
                    AM.Account_Picture = AM.Account_Name + ".jpg";
                    try
                    {
                        if (NewImage == null)
                        {
                            //为空,图片没变化没操作
                        }
                        else
                        {
                            Bitmap newBitmap = new Bitmap(NewImage);
                            NewImage.Dispose();
                            string Address = Application.StartupPath + "\\AccountPic\\" + AM.Account_Name + ".jpg";
                            newBitmap.Save(Address, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                if (comboBox_IsUse.Text == "是")
                {
                    AM.Account_IsUse = 0;
                }
                else
                {
                    AM.Account_IsUse = 1;
                }
                if (comboBox_Permission.Text == "APP操作员")
                {
                    AM.Account_Permission = 1;
                }
                else if (comboBox_Permission.Text == "APP管理员")
                {
                    AM.Account_Permission = 0;
                }
                else
                {
                    AM.Account_Permission = 2;
                }
                AM.Sign = 1;
                AM.Last_Operate_Date = DateTime.Now;
                AM.Last_Operate_Type = 1;

                string Error_message;
                AccDAL.UpdateAccount(AM, out Error_message);
                if (Error_message != "")
                {
                    MessageBox.Show(Error_message);
                }
                else
                {
                    MessageBox.Show("更新成功");
                    //更新表DGV_Account
                    AccDAL.SelectAccount(out Account);
                    DGV_Account.DataSource = Account;
                    Account = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            //删除账号,只修改标记,不进行删除操作
            string Account_ID = DGV_Account.SelectedRows[0].Cells["Account_ID"].Value.ToString();

            string Error_Message;
            AccDAL.DeleteAccount(Account_ID, out Error_Message);
            if (Error_Message != "")
            {
                MessageBox.Show(Error_Message);
            }
            else
            {
                //更新表DGV_Account
                AccDAL.SelectAccount(out Account);
                DGV_Account.DataSource = Account;
                Account = null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox_Lan.Items.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox_APP.Items.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox_RTU.Items.Clear();
        }

        private void picbox_Account_Picture_Click(object sender, EventArgs e)
        {
            if (txtBox_Account_Name.Text == "")
            {
                MessageBox.Show("先填写用户名");
            }
            else
            {
                AccountPicAddress = "";
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Title = "选择文件";
                fileDialog.Filter = "jpg files (*.jpg)|*.jpg";
                fileDialog.FilterIndex = 1;
                fileDialog.RestoreDirectory = true;

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    //点击确定，保存该图片地址
                    AccountPicAddress = fileDialog.FileName;
                    try
                    {
                        NewImage = Image.FromFile(AccountPicAddress);
                        Bitmap bitmap = new Bitmap(NewImage);
                        picbox_Account_Picture.Image = bitmap;
                    }
                    catch (Exception ex)
                    {

                    }

                }
                else
                {
                    // 没有选择文件时的操作
                    AccountPicAddress = "";
                    picbox_Account_Picture.Image = null;
                    try
                    {
                        NewImage.Dispose();
                        OldImage.Dispose();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        #region
        /// <summary>
        /// 将图片数据化为String格式
        /// </summary>
        /// <returns></returns>
        private string PicToStr(Image image)
        {
            string TmpPicStr = "";
            try
            {
                byte[] ImageBytes = { 0 };
                Bitmap IMageBit = new Bitmap(image);
                ImageBytes = ImgToBytes2(IMageBit);
                try
                {
                    IMageBit.Dispose();
                }
                catch (Exception ex)
                {

                }
                TmpPicStr = ByteToString(ImageBytes);
            }
            catch (Exception ex)
            {
                TmpPicStr = "";
            }
            return TmpPicStr;
        }


        /// <summary>
        /// 将字节数组转换成16进制的字符串
        /// </summary>
        /// <param name="bytes">转换前字节数组</param>
        /// <returns></returns>
        private string ByteToString(byte[] bytes)
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (byte bt in bytes)
            {
                strBuilder.AppendFormat("{0:X2}", bt);
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 这个方法将内存图像以Png格式序列化，数据量大大的降低，目测效率也是比前一种要好一点，代码更加简单易读。
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        private byte[] ImgToBytes2(Bitmap bmp)
        {
            try
            {
                MemoryStream sr = new MemoryStream();
                //bmp.Save(sr, System.Drawing.Imaging.ImageFormat.Png);
                bmp.Save(sr, System.Drawing.Imaging.ImageFormat.Jpeg);
                int len = (int)sr.Position;
                byte[] ret = new byte[sr.Position];
                sr.Seek(0, SeekOrigin.Begin);
                sr.Read(ret, 0, len);
                return ret;
            }
            catch (System.Exception ex)
            {
                return null;
            }

        }
        #endregion

        #endregion
    }
}
