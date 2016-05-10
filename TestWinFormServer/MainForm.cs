using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using System;
using System.Data;
using System.Windows.Forms;
using TransferServerLib;
using UserServerLib;


namespace TestWinFormServer
{
    public partial class MainForm : Form
    {
        bool ServiceStatus = false; 
        IBootstrap bootstrap;
        DataTable dtUser;
        DataTable dtFile;
        object lockobj = new object();
        public MainForm()
        {
            InitializeComponent();
            this.dgvFile.AutoGenerateColumns = false;
            this.dgvUser.AutoGenerateColumns = false;
            dtUser = new DataTable();
            dtUser.Columns.Add("SessionId", typeof(string));
            dtUser.Columns.Add("UserId", typeof(string));
            dtUser.Columns.Add("UserName", typeof(string));
            dtFile = new DataTable();
            dtFile.Columns.Add("ProjectId", typeof(string));
            dtFile.Columns.Add("IsUpLoading", typeof(bool));
            dtFile.Columns.Add("FileName", typeof(string));
            dtFile.Columns.Add("FileSize", typeof(long));
            dtFile.Columns.Add("SaveName", typeof(string));
            dtFile.Columns.Add("TransferPos", typeof(long));
            dtFile.Columns.Add("TransferLength", typeof(long));
            dtFile.Columns.Add("TransferedLength", typeof(long));
            dtFile.Columns.Add("Speed", typeof(string));
            this.dgvUser.DataSource = dtUser;
            this.dgvFile.DataSource = dtFile;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (btnStartStop.Text == "启动服务")
            {
                bootstrap = BootstrapFactory.CreateBootstrap();
                if (!bootstrap.Initialize())
                {
                    ServiceStatus = false;
                    MessageBox.Show("Failed to initialize!");
                    return;
                }
                else
                {
                    StartResult result = bootstrap.Start();
                    if (result == StartResult.Failed)
                    {
                        ServiceStatus = false;
                    }
                    else if (result == StartResult.Success)
                    {
                        var transferServer = bootstrap.GetServerByName("TransferServer") as TransferServer;
                        transferServer.NewSessionConnected += transferServer_NewSessionConnected;
                        transferServer.SessionClosed += transferServer_SessionClosed;
                        var userServer = bootstrap.GetServerByName("UserServer") as UserServer;
                        userServer.UserJoin += userServer_UserJoin;
                        userServer.UserLeave += userServer_UserLeave;
                        userServer.SendToServer += userServer_UserSendToServer;
                        userServer.UserWhisper += userServer_UserWhisper;
                        userServer.Broadcast += userServer_UserBroadcast;
                        ServiceStatus = true;
                    }
                    else
                    {
                        //MessageBox.Show(string.Format("Start result: {0}!", result));
                    }
                    this.lblStatus.Text = result.ToString();
                }
            }
            else
            {
                bootstrap.Stop();
                ServiceStatus = false;
                this.lblStatus.Text = "The server was stopped!";
            }
            if (ServiceStatus)
                btnStartStop.Text = "停止服务";
            else
                btnStartStop.Text = "启动服务";
        }

        void userServer_UserBroadcast(BroadcastEventArgs e)
        {
            lock (lockobj)
            {
                this.listBox1.Items.Add(string.Format("{0} Broadcast to everyone :{1} {2}", e.User.UserName, e.Message.Content, e.Message.MsgTime.ToString("HH:mm:ss")));
            }
        }

        void userServer_UserWhisper(UserWhisperEventArgs e)
        {
            lock (lockobj)
            {
                this.listBox1.Items.Add(string.Format("{0} Whisper to {1} :{2} {3}", e.FromUser.UserName, e.ToUser.UserName, e.Message.Content, e.Message.MsgTime.ToString("HH:mm:ss")));
            }
        }

        void userServer_UserSendToServer(SendToServerEventArgs e)
        {
            lock (lockobj)
            {
                this.listBox1.Items.Add(string.Format("{0} say to Server：{1} {2}", e.User.UserName, e.Message.Content, e.Message.MsgTime.ToString("HH:mm:ss")));
            }
        }

        void userServer_UserLeave(UserLeaveEventArgs e)
        {
            lock (lockobj)
            {
                var drs = dtUser.Select(string.Format("SessionId='{0}'", e.User.SessionId));
                if (drs.Length > 0)
                {
                    dtUser.Rows.Remove(drs[0]);
                    dtUser.AcceptChanges();
                }
            }
        }

        void userServer_UserJoin(UserJoinEventArgs e)
        {
            lock (lockobj)
            {
                if (dtUser.Select(string.Format("SessionId='{0}'", e.User.SessionId)).Length <= 0)
                {
                    DataRow dr = dtUser.NewRow();
                    dr["SessionId"] = e.User.SessionId;
                    dr["UserId"] = e.User.UserId;
                    dr["UserName"] = e.User.UserName;
                    dtUser.Rows.Add(dr);
                    dr.AcceptChanges();
                }
            }
        }

        private void btnDelFile_Click(object sender, EventArgs e)
        {
            System.IO.DirectoryInfo dirinfo = new System.IO.DirectoryInfo(".\\Attach");
            if (dirinfo.Exists)
                dirinfo.Delete(true);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ServiceStatus)
                bootstrap.Stop();
        }

        void transferServer_SessionClosed(TransferSession session, SuperSocket.SocketBase.CloseReason value)
        {
        }


        void transferServer_NewSessionConnected(TransferSession session)
        {
            session.UpLoadEngine.StartTransfer += (e) =>
            {
                lock (lockobj)
                {
                    DataRow row = null;
                    var rows = dtFile.Select(string.Format("ProjectId='{0}'", e.ProjectId));
                    if (rows.Length > 0)
                    {
                        row = rows[0];
                    }
                    else
                    {
                        row = dtFile.NewRow();
                        dtFile.Rows.Add(row);
                    }
                    row["IsUpLoading"] = true;
                    row["FileName"] = e.FileName;
                    row["SaveName"] = e.SaveName;
                    row["FileSize"] = e.FileSize;
                    row["TransferPos"] = e.TransferPos;
                    row["TransferLength"] = e.TransferLength;
                    row["ProjectId"] = e.ProjectId;
                    row.AcceptChanges();
                }
                Application.DoEvents();
            };
            DateTime dtTime = DateTime.Now;
            session.UpLoadEngine.TransferStep += (e) =>
            {
                lock (lockobj)
                {
                    var row = dtFile.Select(string.Format("ProjectId='{0}'", e.ProjectId))[0];
                    row["TransferedLength"] = e.TransferLen;// session.UpLoadEngine.TransferedLength;
                    var second = (DateTime.Now - dtTime).Seconds;
                    if (second != 0)
                        row["Speed"] = string.Format("{0}Mb/s", e.TransferLen / (1024 * 1024) / second);
                    row.AcceptChanges();
                }
                Application.DoEvents();
            };
            session.UpLoadEngine.TransferComplete += (e) =>
            {
                lock (lockobj)
                {
                    var row = dtFile.Select(string.Format("ProjectId='{0}'", e.ProjectId))[0];
                    row["IsUpLoading"] = false;
                    row.AcceptChanges();
                }
                Application.DoEvents();
            };
            session.UpLoadEngine.StopTransfer += (e) =>
            {
                lock (lockobj)
                {
                    var drs = dtFile.Select(string.Format("ProjectId='{0}'", e.ProjectId));
                    if (drs.Length > 0)
                    {
                        var dr = drs[0];
                        dr["Status"] = false;
                        dr.AcceptChanges();
                    }
                }
                Application.DoEvents();
            };
        }
    }
}
