using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UserClientLib;
using UserCommon;

namespace TestWinFormClient
{
    public partial class Form2 : Form, IUserEvents
    {
        IUserClientEngine engine;
        public Form2()
        {
            InitializeComponent();
            engine = UserClientLib.UserEngineFactory.CreateTransferEngine(this);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
        }


        private void button1_Click(object sender, EventArgs e)
        {
            engine.Login("zs", "lisi");
        }

        public void OnLoginComplete(LoginCompleteEventArgs e)
        {
            MessageBox.Show(e.LoginResult.ToString());
        }

        public void OnGetAllUser(GetAllUserEventArgs e)
        {
        }

        public void OnUserLeave(UserLeaveEventArgs e)
        {
        }

        public void OnUserJoin(UserJoinEventArgs e)
        {
        }

        public void OnReceiveMsg(ReceiveMsgEventArgs e)
        {
        }

        public void OnUserWhisper(UserWhisperEventArgs e)
        {
        }


        public void OnGetUser(GetUserEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
