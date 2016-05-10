using System;
using System.Threading;
using System.Windows.Forms;
using UserClientLib;
using UserClientServerLib;
using UserCommon;

namespace TestWinFormClient
{
    public partial class ChatForm : Form, IChatEvents
    {
        public IChatEngine _IChatEngine;
        User ToUser;
        public ChatForm(User _ToUser)
        {
            InitializeComponent();
            ToUser = _ToUser;
            this.label1.Text = ToUser.UserName;
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtContent.Text))
            {
                Thread thread = new Thread(new ParameterizedThreadStart(SetMessage));
                thread.IsBackground = true;
                thread.Start("发送内容为空");
            }
            var msg = new UserCommon.Message(this.txtContent.Text);
            this.listboxMsg.Items.Add(string.Format("你说：{0} {1}", msg.Content, msg.MsgTime.ToString("HH:mm:ss")));
            _IChatEngine.UserChat(msg);
            this.txtContent.Text = string.Empty;
        }

        private void SetMessage(object msg)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                this.label2.Text = msg.ToString();
            }));
            Thread.Sleep(2000);
            this.Invoke(new MethodInvoker(() =>
            {
                this.label2.Text = string.Empty;
            }));
        }

        public void OnUserChat(UserCommon.Message message)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                this.listboxMsg.Items.Add(string.Format("{0} 说：{1} {2}", ToUser.UserName, message.Content, message.MsgTime.ToString("HH:mm:ss")));
            }));
        }

        private void txtContent_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Control)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.btnSend_Click(this.btnSend, null);
                    e.SuppressKeyPress = true;
                }
            }
        }
    }
}
