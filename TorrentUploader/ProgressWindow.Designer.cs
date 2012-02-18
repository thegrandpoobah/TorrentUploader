namespace TorrentUploader
{
    partial class ProgressWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressWindow));
            this.workingPanel = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.workingLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.donePanel = new System.Windows.Forms.Panel();
            this.serverUrl = new System.Windows.Forms.LinkLabel();
            this.btnClose = new System.Windows.Forms.Button();
            this.doneLabel = new System.Windows.Forms.Label();
            this.workingPanel.SuspendLayout();
            this.donePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // workingPanel
            // 
            this.workingPanel.Controls.Add(this.btnCancel);
            this.workingPanel.Controls.Add(this.workingLabel);
            this.workingPanel.Controls.Add(this.progressBar);
            this.workingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.workingPanel.Location = new System.Drawing.Point(0, 0);
            this.workingPanel.Name = "workingPanel";
            this.workingPanel.Size = new System.Drawing.Size(466, 121);
            this.workingPanel.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(194, 83);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // workingLabel
            // 
            this.workingLabel.AutoSize = true;
            this.workingLabel.Location = new System.Drawing.Point(32, 25);
            this.workingLabel.Name = "workingLabel";
            this.workingLabel.Size = new System.Drawing.Size(261, 13);
            this.workingLabel.TabIndex = 1;
            this.workingLabel.Text = "Please wait while the torrent is uploaded to the server.";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(35, 41);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(392, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 0;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // donePanel
            // 
            this.donePanel.Controls.Add(this.serverUrl);
            this.donePanel.Controls.Add(this.btnClose);
            this.donePanel.Controls.Add(this.doneLabel);
            this.donePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.donePanel.Location = new System.Drawing.Point(0, 0);
            this.donePanel.Name = "donePanel";
            this.donePanel.Size = new System.Drawing.Size(466, 121);
            this.donePanel.TabIndex = 1;
            this.donePanel.Visible = false;
            // 
            // serverUrl
            // 
            this.serverUrl.AutoSize = true;
            this.serverUrl.Location = new System.Drawing.Point(23, 51);
            this.serverUrl.Name = "serverUrl";
            this.serverUrl.Size = new System.Drawing.Size(49, 13);
            this.serverUrl.TabIndex = 2;
            this.serverUrl.TabStop = true;
            this.serverUrl.Text = "serverUrl";
            this.serverUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.serverUrl_LinkClicked);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(194, 86);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // doneLabel
            // 
            this.doneLabel.Location = new System.Drawing.Point(23, 12);
            this.doneLabel.Name = "doneLabel";
            this.doneLabel.Size = new System.Drawing.Size(418, 26);
            this.doneLabel.TabIndex = 0;
            this.doneLabel.Text = "Torrent has been uploaded to server. You can view the download progress using the" +
    " following URL:";
            // 
            // ProgressWindow
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(466, 121);
            this.Controls.Add(this.donePanel);
            this.Controls.Add(this.workingPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ProgressWindow";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Torrent Uploader";
            this.workingPanel.ResumeLayout(false);
            this.workingPanel.PerformLayout();
            this.donePanel.ResumeLayout(false);
            this.donePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel workingPanel;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label workingLabel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Panel donePanel;
        private System.Windows.Forms.LinkLabel serverUrl;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label doneLabel;
    }
}

