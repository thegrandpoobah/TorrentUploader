using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cleverscape.UTorrentClient.WebClient;
using System.Diagnostics;
using System.IO;
//blah
namespace TorrentUploader
{
    public partial class ProgressWindow : Form
    {
        #region < Fields >

        private Torrent storedTorrent = null;
        private object storedTorrentLock = new object();

        #endregion < Fields >

        public ProgressWindow()
        {
            InitializeComponent();

            this.serverUrl.Text = string.Format("http://{0}:{1}@{2}/gui/",
                Properties.Settings.Default.UserName,
                Properties.Settings.Default.Password,
                Properties.Settings.Default.ServerAddress.Remove(0, 7));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.backgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (UTorrentWebClient webClient = new UTorrentWebClient(Properties.Settings.Default.ServerAddress,
                Properties.Settings.Default.UserName,
                Properties.Settings.Default.Password))
            {
                webClient.TorrentAdded += new TorrentEventHandler(webClient_TorrentAdded);
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                webClient.AddTorrent(Environment.GetCommandLineArgs()[1]);

                if (backgroundWorker.CancellationPending)
                {
                    lock (this.storedTorrentLock)
                    {
                        if (this.storedTorrent != null)
                        {
                            webClient.TorrentRemove(this.storedTorrent, true);
                        }
                    }

                    e.Cancel = true;
                    return;
                }
            }
        }

        private void webClient_TorrentAdded(object sender, TorrentEventArgs e)
        {
            lock (storedTorrentLock) {
                this.storedTorrent = e.Torrent;
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.progressBar.Style = ProgressBarStyle.Blocks;
            this.progressBar.Value = this.progressBar.Maximum;

            if (e.Cancelled)
            {
                this.doneLabel.Text = "Torrent Upload was cancelled.";
            }

            this.workingPanel.Visible = false;
            this.donePanel.Visible = true;

            File.Delete(Environment.GetCommandLineArgs()[1]);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.backgroundWorker.CancelAsync();

            this.btnCancel.Enabled = false;
            this.workingLabel.Text = "Cancelling torrent upload. Please wait...";
        }

        private void serverUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(this.serverUrl.Text);
        }
    }
}
