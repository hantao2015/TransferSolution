namespace TestWinFormClient
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ProjectId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsUpLoading = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SaveName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransferPos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransferLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransferedLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Speed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errormsg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ProjectId,
            this.IsUpLoading,
            this.FileName,
            this.FileSize,
            this.SaveName,
            this.TransferPos,
            this.TransferLength,
            this.TransferedLength,
            this.Speed,
            this.errormsg});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1130, 356);
            this.dataGridView1.TabIndex = 5;
            // 
            // ProjectId
            // 
            this.ProjectId.DataPropertyName = "ProjectId";
            this.ProjectId.HeaderText = "文件Id";
            this.ProjectId.Name = "ProjectId";
            this.ProjectId.ReadOnly = true;
            // 
            // IsUpLoading
            // 
            this.IsUpLoading.DataPropertyName = "IsUpLoading";
            this.IsUpLoading.HeaderText = "是否正在上传文件";
            this.IsUpLoading.Name = "IsUpLoading";
            this.IsUpLoading.ReadOnly = true;
            // 
            // FileName
            // 
            this.FileName.DataPropertyName = "FileName";
            this.FileName.HeaderText = "上传文件名称";
            this.FileName.Name = "FileName";
            this.FileName.ReadOnly = true;
            this.FileName.Width = 130;
            // 
            // FileSize
            // 
            this.FileSize.DataPropertyName = "FileSize";
            this.FileSize.HeaderText = "上传文件大小";
            this.FileSize.Name = "FileSize";
            this.FileSize.ReadOnly = true;
            this.FileSize.Width = 120;
            // 
            // SaveName
            // 
            this.SaveName.DataPropertyName = "SaveName";
            this.SaveName.HeaderText = "保存名称";
            this.SaveName.Name = "SaveName";
            this.SaveName.ReadOnly = true;
            this.SaveName.Width = 120;
            // 
            // TransferPos
            // 
            this.TransferPos.DataPropertyName = "TransferPos";
            this.TransferPos.HeaderText = "上传位置";
            this.TransferPos.Name = "TransferPos";
            this.TransferPos.ReadOnly = true;
            // 
            // TransferLength
            // 
            this.TransferLength.DataPropertyName = "TransferLength";
            this.TransferLength.HeaderText = "上传大小";
            this.TransferLength.Name = "TransferLength";
            this.TransferLength.ReadOnly = true;
            // 
            // TransferedLength
            // 
            this.TransferedLength.DataPropertyName = "TransferedLength";
            this.TransferedLength.HeaderText = "已上传大小";
            this.TransferedLength.Name = "TransferedLength";
            this.TransferedLength.ReadOnly = true;
            // 
            // Speed
            // 
            this.Speed.DataPropertyName = "Speed";
            this.Speed.HeaderText = "传输速度";
            this.Speed.Name = "Speed";
            this.Speed.ReadOnly = true;
            // 
            // errormsg
            // 
            this.errormsg.DataPropertyName = "errormsg";
            this.errormsg.HeaderText = "错误信息";
            this.errormsg.Name = "errormsg";
            this.errormsg.ReadOnly = true;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(203, 362);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(94, 33);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "添加文件";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(519, 362);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(94, 33);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "开始上传";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStartEnd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(350, 362);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(94, 33);
            this.btnDelete.TabIndex = 7;
            this.btnDelete.Text = "删除文件";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(673, 362);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(94, 33);
            this.btnStop.TabIndex = 8;
            this.btnStop.Text = "停止上传";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1130, 397);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnStart);
            this.Name = "Form1";
            this.Text = "客户端";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProjectId;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsUpLoading;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn SaveName;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransferPos;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransferLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransferedLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn Speed;
        private System.Windows.Forms.DataGridViewTextBoxColumn errormsg;
    }
}

