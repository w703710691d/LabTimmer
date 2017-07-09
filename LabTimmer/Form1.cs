using Auto_RUN;
using Check_IPorMAC;
using Get_LuYouMac;
using HookTest;
using httpclient;
using Read_Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Time_Dates;

namespace LabTimmer
{
    public partial class Form1 : Form
    {
        public delegate void SendDate(string MACIP, List<string> List_sever_ip);
        private int intKeyBoardCount = 0;
        private int intMouseCount = 0;
        private bool IsThreadAlive = false;
        private DateTime startTime;
        private DateTime stopTime;
        private string MACIP = null;
        private string LUYOU_MAC = null;
        private string YOUXIAN_IP = null;
        private List<string> List_sever_ip;
        private GlobalHook hook;
        private HttpPostToServer _httpPost = new HttpPostToServer();
        private TimeDates t_d = new TimeDates();
        private DateTime point0 = Convert.ToDateTime("23:58:00");
        private DateTime point01 = Convert.ToDateTime("00:01:00");
        private DateTime point2 = Convert.ToDateTime("01:58:00");
        private DateTime point02 = Convert.ToDateTime("02:00:00");
        private DateTime point8 = Convert.ToDateTime("08:01:00");
        private DateTime point08 = Convert.ToDateTime("08:00:00");
        private DateTime point12 = Convert.ToDateTime("11:58:00");
        private DateTime point012 = Convert.ToDateTime("12:00:00");
        private DateTime point14 = Convert.ToDateTime("14:01:00");
        private DateTime point014 = Convert.ToDateTime("14:00:00");
        private object ojb11 = new object();
        private object ojb22 = new object();
        private IContainer components = null;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer3;
        private NotifyIcon notifyIcon1;
        private Button button1;
        private Button button2;
        private System.Windows.Forms.Timer timer2;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer3
            // 
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick_1);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "实验室签到软件";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(20, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(141, 31);
            this.button1.TabIndex = 1;
            this.button1.Text = "开机自启动";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(20, 49);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(141, 31);
            this.button2.TabIndex = 2;
            this.button2.Text = "取消开机自启动";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(180, 95);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.Text = "实验室签到软件";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form_SizeChanged);
            this.ResumeLayout(false);

        }
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread.CurrentThread.IsBackground = true;
            ServicePointManager.DefaultConnectionLimit = 500;
            string retMac = getLocalMac();
            if (String.IsNullOrEmpty(retMac))
            {
                this.MACIP = retMac;
                string[] s_arr = this.MACIP.Split(new char[]
                {
                    ':'
                });
                this.MACIP = s_arr[0];
                for (int i = 1; i < s_arr.Length; i++)
                {
                    this.MACIP = this.MACIP + "-" + s_arr[i];
                }
            }
            this.List_sever_ip = ReadXML.readtext();
            this.startTime = DateTime.MinValue;
            this.stopTime = DateTime.MinValue;
            this.LUYOU_MAC = GetLuYouMac.GetMac(ReadXML.readMcaMsg());
            this.YOUXIAN_IP = GetLuYouMac.GettrueIpV4();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (this.hook == null)
            {
                this.hook = new GlobalHook();
                this.hook.KeyDown += new KeyEventHandler(this.hook_KeyDown);
                this.hook.KeyPress += new KeyPressEventHandler(this.hook_KeyPress);
                this.hook.KeyUp += new KeyEventHandler(this.hook_KeyUp);
                this.hook.OnMouseActivity += new MouseEventHandler(this.hook_OnMouseActivity);
            }
            this.IsThreadAlive = false;
            this.timer1.Interval = 120000;
            this.timer1.Enabled = true;
            this.timer1.Start();
            this.timer2.Interval = 806000;
            this.timer2.Enabled = true;
            this.timer2.Start();
            this.timer3.Interval = 1000;
            this.timer3.Enabled = true;
            this.timer3.Start();
            this.startTime = DateTime.MinValue;
            this.stopTime = DateTime.MinValue;
            this.hook.Start();
        }
        public void CheckThreadStart()
        {
            lock (this.ojb11)
            {
                if (this.YOUXIAN_IP != null || this.LUYOU_MAC != null)
                {
                    if (CheckIPorMAC.IsInLab(this.YOUXIAN_IP, this.LUYOU_MAC, List_sever_ip))
                    {
                        if (!this.IsThreadAlive && this.startTime == DateTime.MinValue && this.stopTime == DateTime.MinValue)
                        {
                            this.startTime = DateTime.Now;
                            this.IsThreadAlive = true;
                        }
                    }
                    else
                    {
                        this.startTime = DateTime.MinValue;
                    }
                }
            }
        }
        public void CheckThreadStop(bool ispoint)
        {
            lock (this.ojb22)
            {
                if (this.MACIP != null)
                {
                    if (this.YOUXIAN_IP != null || this.LUYOU_MAC != null)
                    {
                        if (CheckIPorMAC.IsInLab(this.YOUXIAN_IP, this.LUYOU_MAC, List_sever_ip))
                        {
                            if (!this.IsThreadAlive)
                            {
                                this.startTime = DateTime.MinValue;
                                this.stopTime = DateTime.MinValue;
                            }
                            else
                            {
                                if (this.IsThreadAlive && this.stopTime == DateTime.MinValue && this.startTime != DateTime.MinValue)
                                {
                                    if (!ispoint)
                                    {
                                        this.stopTime = DateTime.Now.AddMinutes(-10.0);
                                    }
                                    else
                                    {
                                        this.stopTime = DateTime.Now;
                                    }
                                    if (this.startTime != DateTime.MinValue && this.stopTime != DateTime.MinValue)
                                    {
                                        this.t_d = new TimeDates();
                                        DateTime str = this.startTime;
                                        DateTime stp = this.stopTime;
                                        if (stp.Day - str.Day == 0)
                                        {
                                            this.t_d.WriteDateTOLog(str.ToString("yyyy-MM-dd HH:mm:ss"), stp.ToString("yyyy-MM-dd HH:mm:ss"));
                                            Form1.SendDate SuccessSendDate = new Form1.SendDate(this.t_d.SendDateToSever);
                                            IAsyncResult async = SuccessSendDate.BeginInvoke(this.MACIP, this.List_sever_ip, null, null);
                                            while (!async.IsCompleted)
                                            {
                                            }
                                        }
                                        this.startTime = DateTime.MinValue;
                                        this.stopTime = DateTime.MinValue;
                                        this.IsThreadAlive = false;
                                    }
                                    else
                                    {
                                        this.startTime = DateTime.MinValue;
                                        this.stopTime = DateTime.MinValue;
                                    }
                                    this.startTime = DateTime.MinValue;
                                    this.stopTime = DateTime.MinValue;
                                }
                                else
                                {
                                    this.startTime = DateTime.MinValue;
                                    this.stopTime = DateTime.MinValue;
                                }
                            }
                        }
                        else
                        {
                            this.startTime = DateTime.MinValue;
                            this.stopTime = DateTime.MinValue;
                        }
                    }
                    else
                    {
                        this.startTime = DateTime.MinValue;
                        this.stopTime = DateTime.MinValue;
                    }
                }
                else
                {
                    this.startTime = DateTime.MinValue;
                    this.stopTime = DateTime.MinValue;
                }
            }
        }
        private void hook_OnMouseActivity(object sender, MouseEventArgs e)
        {
            if ((DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point2) < 0 || DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point8) > 0) && (DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point12) < 0 || DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point14) > 0) && DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point0) < 0 && DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point01) > 0)
            {
                this.intMouseCount = 0;
                if (this.startTime == DateTime.MinValue)
                {
                    this.CheckThreadStart();
                }
            }
        }
        private void hook_KeyUp(object sender, KeyEventArgs e)
        {
            if ((DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point2) < 0 || DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point8) > 0) && (DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point12) < 0 || DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point14) > 0) && DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point0) < 0 && DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point01) > 0)
            {
                this.intKeyBoardCount = 0;
                if (this.startTime == DateTime.MinValue)
                {
                    this.CheckThreadStart();
                }
            }
        }
        private void hook_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
        }
        public string getLocalMac()
        {
            string mac = null;
            ManagementClass query = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection queryCollection = query.GetInstances();
            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = queryCollection.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ManagementObject mo = (ManagementObject)enumerator.Current;
                    if (mo["IPEnabled"].ToString() == "True")
                    {
                        if(GetLuYouMac.isVialdNetCardName(mo["Description"].ToString()))
                        {
                            mac = mo["MacAddress"].ToString();
                            Time_Dates.TimeDates.WriteErroTOLog("Your mac address is " + mac);
                        }
                    }
                }
            }
            return mac;
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (base.WindowState == FormWindowState.Minimized)
            {
                base.Show();
                base.WindowState = FormWindowState.Normal;
                this.notifyIcon1.Visible = false;
                base.ShowInTaskbar = true;
            }
        }
        private void Form_SizeChanged(object sender, EventArgs e)
        {
            if (base.WindowState == FormWindowState.Minimized)
            {
                base.Hide();
                base.ShowInTaskbar = false;
                this.notifyIcon1.Visible = true;
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                if (this.IsThreadAlive)
                {
                    this.hook.Stop();
                    this.CheckThreadStop(true);
                }
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                DialogResult dr = MessageBox.Show("确定退出程序", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    if (this.IsThreadAlive)
                    {
                        this.hook.Stop();
                        this.CheckThreadStop(true);
                    }
                    Thread.Sleep(500);
                    Process.GetCurrentProcess().Kill();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if ((DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point2) < 0 || DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point8) > 0) && (DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point12) < 0 || DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point14) > 0) && DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point0) < 0 && DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point01) > 0)
            {
                if (this.intMouseCount >= 5 && this.intKeyBoardCount >= 5)
                {
                    this.CheckThreadStop(false);
                    this.intKeyBoardCount = 0;
                    this.intMouseCount = 0;
                }
                this.intMouseCount++;
                this.intKeyBoardCount++;
            }
        }
        private void timer3_Tick_1(object sender, EventArgs e)
        {
            if (DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point0) >= 0 || DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point01) <= 0)
            {
                this.CheckThreadStop(true);
            }
            if (DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point2) >= 0 && DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point02) <= 0)
            {
                this.CheckThreadStop(true);
            }
            if (DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point12) >= 0 && DateTime.Compare(Convert.ToDateTime(DateTime.Now.ToLongTimeString()), this.point012) <= 0)
            {
                this.CheckThreadStop(true);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            AutoRUN.SetAutoRun(Application.ExecutablePath, true);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            bool yN = AutoRUN.DeleteAutoRun(Application.ExecutablePath);
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            this.LUYOU_MAC = GetLuYouMac.GetMac(ReadXML.readMcaMsg());
            this.YOUXIAN_IP = GetLuYouMac.GettrueIpV4();
            string retMac = getLocalMac();
            if (String.IsNullOrEmpty(retMac))
            {
                this.MACIP = retMac;
                string[] s_arr = this.MACIP.Split(new char[]
                {
                    ':'
                });
                this.MACIP = s_arr[0];
                for (int i = 1; i < s_arr.Length; i++)
                {
                    this.MACIP = this.MACIP + "-" + s_arr[i];
                }
            }
        }
    }
}
