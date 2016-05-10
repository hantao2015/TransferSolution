using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TransferClientLib;

namespace TestWinFormClient
{
    public partial class Form1 : Form
    {
        DataTable dt;
        public Form1()
        {
            InitializeComponent();
            this.dataGridView1.AutoGenerateColumns = false;
            dt = new DataTable();
            dt.Columns.Add("ProjectId", typeof(string));
            dt.Columns.Add("IsUpLoading", typeof(bool));
            dt.Columns.Add("FileName", typeof(string));
            dt.Columns.Add("FileSize", typeof(long));
            dt.Columns.Add("SaveName", typeof(string));
            dt.Columns.Add("TransferPos", typeof(long));
            dt.Columns.Add("TransferLength", typeof(long));
            dt.Columns.Add("TransferedLength", typeof(long));
            dt.Columns.Add("errormsg", typeof(string));
            dt.Columns.Add("Speed", typeof(string));
            this.dataGridView1.DataSource = dt;
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
                        DataRow dr = dt.NewRow();
                        dr["ProjectId"] = string.Empty;
                        dr["IsUpLoading"] = false;
                        dr["FileName"] = fileInfo.FullName;
                        dr["FileSize"] = fileInfo.Length;
                        dr["SaveName"] = fileInfo.Name;
                        dr["TransferPos"] = 0;
                        dr["TransferLength"] = fileInfo.Length;
                        dr["TransferedLength"] = 0;
                        dr["errormsg"] = string.Empty;
                        dt.Rows.Add(dr);
                    }
                }
                dt.AcceptChanges();
            }
        }

        private bool IsFileAdd(string item)
        {
            var files = dt.Select(string.Format("FileName='{0}'", item));
            if (files.Length > 0)
                return true;
            return false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                var row = (this.dataGridView1.SelectedRows[0].DataBoundItem as DataRowView).Row;
                this.dt.Rows.Remove(row);
                this.dt.AcceptChanges();
            }
        }
        object lockobj = new object();
        Dictionary<string, ITransferEngine> uploadings = new Dictionary<string, ITransferEngine>();
        private void btnStartEnd_Click(object sender, EventArgs e)
        {
            this.btnStop.Enabled = false;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
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
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //var row = dt.Rows[i];
                    //uploadings[row["FileName"].ToString()].StopUpLoad();
                    //uploadings.Remove(row["FileName"].ToString());

                    var row = dt.Rows[i];
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
