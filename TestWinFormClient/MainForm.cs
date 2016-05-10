using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using TransferClientLib;
using UserClientLib;
using UserClientServerLib;
using UserCommon;


namespace TestWinFormClient
{
    public partial class MainForm : Form, IUserEvents
    {
        object lockobj = new object();
        IUserClientEngine UserClientEngine;
        DataTable dtUser;
        DataTable dtFile;
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
            dtFile.Columns.Add("Errormsg", typeof(string));
            this.dgvUser.DataSource = dtUser;
            this.dgvFile.DataSource = dtFile;
            UserClientEngine = UserClientLib.UserEngineFactory.CreateUserEngine(this);
            UserClientEngine.UserChat += UserClientEngine_UserChat;
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (UserClientEngine.IsConnected)
            {
                MessageBox.Show("您已经登陆了");
                return;
            }
            UserClientEngine.Login(this.txtUserId.Text, this.txtPwd.Text);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        public void OnLoginComplete(LoginCompleteEventArgs e)
        {
            if (e.LoginResult)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    this.lblStatus.Text = "登陆成功";
                }));
            }
            else
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    this.lblStatus.Text = e.ErrorMessage;
                }));
            }
        }

        public void OnGetAllUser(GetAllUserEventArgs e)
        {
            lock (lockobj)
            {
                dtUser.Clear();
                foreach (var item in e.Users)
                {
                    DataRow dr = dtUser.NewRow();
                    dr["SessionId"] = item.SessionId;
                    dr["UserId"] = item.UserId;
                    dr["UserName"] = item.UserName;
                    dtUser.Rows.Add(dr);
                }
                dtUser.AcceptChanges();
            }
        }

        public void OnUserLeave(UserLeaveEventArgs e)
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

        public void OnUserJoin(UserJoinEventArgs e)
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

        public void OnReceiveMsg(ReceiveMsgEventArgs e)
        {
            lock (lockobj)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    this.listBox1.Items.Add(string.Format("{0} Broadcast to everyone :{1} {2}", e.FromUser.UserName, e.Message.Content, e.Message.MsgTime.ToString("HH:mm:ss")));
                }));
            }
        }

        public void OnUserWhisper(UserWhisperEventArgs e)
        {
            lock (lockobj)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    this.listBox1.Items.Add(string.Format("{0} Whisper to Me :{1} {2}", e.FromUser.UserName, e.Message.Content, e.Message.MsgTime.ToString("HH:mm:ss")));
                }));
            }
        }

        public void OnGetUser(GetUserEventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    this.lblUserName.Text = e.User.UserName;
                }));
            }));
        }

        private void btnWhisper_Click(object sender, EventArgs e)
        {
            if (this.dgvUser.SelectedRows.Count <= 0)
            {
                MessageBox.Show("必须选择一个说话的对象");
                return;
            }
            DataRowView drv = (this.dgvUser.SelectedRows[0].DataBoundItem as DataRowView);
            string touserSessionId = drv["SessionId"].ToString();
            if (touserSessionId == UserClientEngine.CurrentUser.SessionId)
            {
                MessageBox.Show("不可以自己跟自己说话");
                return;
            }
            UserClientEngine.Whisper(touserSessionId, new UserCommon.Message(this.txtContent.Text));
            this.listBox1.Items.Add(string.Format("I Whisper to {0} :{1} {2}", drv["UserName"], this.txtContent.Text, DateTime.Now.ToString("HH:mm:ss")));
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (UserClientEngine != null && UserClientEngine.IsConnected)
                UserClientEngine.LogOut();
        }

        private void btnBroadcast_Click(object sender, EventArgs e)
        {
            if (UserClientEngine != null && UserClientEngine.IsConnected)
            {
                UserClientEngine.Speak(new UserCommon.Message(this.txtContent.Text));
            }
        }

        private void btnSendToServer_Click(object sender, EventArgs e)
        {
            if (UserClientEngine != null && UserClientEngine.IsConnected)
            {
                UserClientEngine.SendToServer(new UserCommon.Message(this.txtContent.Text));
            }
        }

        private void btnStartChat_Click(object sender, EventArgs e)
        {
            if (this.dgvUser.SelectedRows.Count <= 0)
            {
                MessageBox.Show("必须选择一个说话的对象");
                return;
            }
            DataRowView drv = (this.dgvUser.SelectedRows[0].DataBoundItem as DataRowView);
            string touserSessionId = drv["SessionId"].ToString();
            if (touserSessionId == UserClientEngine.CurrentUser.SessionId)
            {
                MessageBox.Show("不可以对自己发起会话");
                return;
            }
            this.UserClientEngine.UserStartCreateChat(touserSessionId);
        }

        public void OnUserStartCreateChat(UserStartCreateChatEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    var chatForm = new ChatForm(e.User);
                    chatForm.Text = string.Format("当前用户：{0}", this.UserClientEngine.CurrentUser.UserName);
                    IChatEngine _IChatEngine = ChatEngineFactory.CreateChatEngineFromClient(chatForm, e.Ip, e.Port, this.UserClientEngine.CurrentUser);
                    chatForm._IChatEngine = _IChatEngine;
                    chatForm.Show();
                }));
            }
            else
            {
                var chatForm = new ChatForm(e.User);
                chatForm.Text = string.Format("当前用户：{0}", this.UserClientEngine.CurrentUser.UserName);
                IChatEngine _IChatEngine = ChatEngineFactory.CreateChatEngineFromClient(chatForm, e.Ip, e.Port, this.UserClientEngine.CurrentUser);
                chatForm._IChatEngine = _IChatEngine;
                chatForm.Show();
            }
        }

        void UserClientEngine_UserChat(UserChatEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    var chatForm = new ChatForm(e.ChatUser);
                    chatForm.Text = string.Format("当前用户：{0}", this.UserClientEngine.CurrentUser.UserName);
                    IChatEngine _IChatEngine =
                        ChatEngineFactory.CreateChatEngineFromServer(e.UserClientSession, chatForm);
                    chatForm._IChatEngine = _IChatEngine;
                    chatForm.Show();
                }));
            }
            else
            {
                var chatForm = new ChatForm(e.ChatUser);
                chatForm.Text = string.Format("当前用户：{0}", this.UserClientEngine.CurrentUser.UserName);
                IChatEngine _IChatEngine =
                    ChatEngineFactory.CreateChatEngineFromServer(e.UserClientSession, chatForm);
                chatForm._IChatEngine = _IChatEngine;
                chatForm.Show();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"D:\Soft\test";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var files = openFileDialog.FileNames;
                foreach (var item in files)
                {
                    bool isFileAdd = IsFileAdd(item);
                    if (!isFileAdd)
                    {
                        System.IO.FileInfo fileInfo = new System.IO.FileInfo(item);
                        DataRow dr = dtFile.NewRow();
                        dr["ProjectId"] = string.Empty;
                        dr["IsUpLoading"] = false;
                        dr["FileName"] = fileInfo.FullName;
                        dr["FileSize"] = fileInfo.Length;
                        dr["SaveName"] = fileInfo.Name;
                        dr["TransferPos"] = 0;
                        dr["TransferLength"] = fileInfo.Length;
                        dr["TransferedLength"] = 0;
                        dr["Speed"] = "0kb/s";
                        dr["Errormsg"] = string.Empty;
                        dtFile.Rows.Add(dr);
                    }
                }
                dtFile.AcceptChanges();
            }
        }
        private bool IsFileAdd(string item)
        {
            var files = dtFile.Select(string.Format("FileName='{0}'", item));
            if (files.Length > 0)
                return true;
            return false;
        }
        Dictionary<string, ITransferEngine> uploadings = new Dictionary<string, ITransferEngine>();
        private void btnUpLoad_Click(object sender, EventArgs e)
        {
            this.btnStop.Enabled = false;
            for (int i = 0; i < dtFile.Rows.Count; i++)
            {
                var row = dtFile.Rows[i];
                DateTime dtTime = DateTime.Now;
                ITransferEngine engine = TransferClientLib.TransferEngineFactory.CreateTransferEngine();
                engine.FileUploadingEvents.TransferStart += (args) =>
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        this.btnStop.Enabled = true;
                    }));
                    lock (lockobj)
                    {
                        row["ProjectId"] = args.ProjectId;
                        row.AcceptChanges();
                    }
                    Application.DoEvents();
                };
                engine.FileUploadingEvents.FileExist += (args) =>
                {
                    if (MessageBox.Show("存在相同的文件，是否覆盖？", "提示", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        return true;
                    uploadings.Remove(row["FileName"].ToString());
                    lock (lockobj)
                    {
                        row["IsUpLoading"] = false;
                        row.AcceptChanges();
                    }
                    Application.DoEvents();
                    return false;
                };
                engine.FileUploadingEvents.TransferComplete += (args) =>
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        this.btnStart.Enabled = true;
                    }));
                    uploadings.Remove(row["FileName"].ToString());
                    lock (lockobj)
                    {
                        row["IsUpLoading"] = false;
                        row.AcceptChanges();
                    }
                    Application.DoEvents();
                };
                engine.FileUploadingEvents.TransferError += (args) =>
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        this.btnStart.Enabled = true;
                    }));
                    uploadings.Remove(row["FileName"].ToString());
                    lock (lockobj)
                    {
                        row["errormsg"] = args.Exception.Message;
                        row.AcceptChanges();
                    }
                    Application.DoEvents();
                };
                engine.FileUploadingEvents.TransferStep += (args) =>
                {
                    lock (lockobj)
                    {
                        row["TransferedLength"] = args.TransferLen;
                        var second = (DateTime.Now - dtTime).Seconds;
                        if (second != 0)
                            row["Speed"] = string.Format("{0}Mb/s", args.TransferLen / (1024 * 1024) / second);
                        row.AcceptChanges();
                    }
                    Application.DoEvents();
                };
                string saveName = "Attach\\" + Path.GetFileName(row["SaveName"].ToString());
                string fileProjectId = string.Empty;
                engine.StartUpLoad(row["FileName"].ToString(), saveName, ref fileProjectId);
                lock (lockobj)
                {
                    row["ProjectId"] = fileProjectId;
                    row.AcceptChanges();
                }
                Application.DoEvents();
                uploadings.Add(row["FileName"].ToString(), engine);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            lock (lockobj)
            {
                for (int i = 0; i < dtFile.Rows.Count; i++)
                {
                    var row = dtFile.Rows[i];
                    string fileName = row["FileName"].ToString();
                    if (uploadings.ContainsKey(fileName))
                    {
                        this.btnStart.Enabled = false;
                        uploadings[fileName].StopUpLoad();
                        uploadings.Remove(fileName);
                    }
                    else
                    {
                        MessageBox.Show("键不存在");
                    }
                }

            }
        }
    }
}
