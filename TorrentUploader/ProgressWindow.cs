using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Windows.Forms;
using Cleverscape.UTorrentClient.WebClient;
using TorrentUploader.Native;

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

        private void Flash()
        {
            FLASHWINFO fw = new FLASHWINFO();

            fw.cbSize = Convert.ToUInt32(Marshal.SizeOf(typeof(FLASHWINFO)));
            fw.hwnd = this.Handle;
            fw.dwFlags = (int)FlashWindowExFlags.FLASHW_TRAY;
            fw.uCount = UInt32.MaxValue;

            Methods.FlashWindowEx(ref fw);
        }

        #region < Event Handlers >

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

                if (this.IsMagnetLink)
                {
                    webClient.AddTorrentFromUrl(this.TorrentURI);
                }
                else
                {
                    webClient.AddTorrent(this.TorrentURI);
                }

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
            lock (storedTorrentLock)
            {
                this.storedTorrent = e.Torrent;
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.progressBar.Style = ProgressBarStyle.Blocks;
            this.progressBar.Value = this.progressBar.Maximum;

            if (e.Error != null)
            {
                if (e.Error is UriFormatException)
                {
                    this.doneLabel.Text = string.Format("{0}\n{1}", Strings.ErrorHeader, Strings.IncorrectServerAddressFormat);
                }
                else if (e.Error is MessageSecurityException)
                {
                    this.doneLabel.Text = string.Format("{0}\n{1}", Strings.ErrorHeader, Strings.IncorrectUserNameOrPassword);
                }
                else if (e.Error is EndpointNotFoundException)
                {
                    this.doneLabel.Text = string.Format("{0}\n{1}", Strings.ErrorHeader, Strings.NoResponse);
                }
                else if (e.Error is FileNotFoundException)
                {
                    this.doneLabel.Text = string.Format("{0}\n{1}", Strings.ErrorHeader, Strings.FileNotFound);
                }
                else
                {
                    this.doneLabel.Text = e.Error.ToString();
                }
            }
            else if (e.Cancelled)
            {
                this.doneLabel.Text = Strings.UploadCancelled;
            }

            if (Properties.Settings.Default.FlashWindowOnComplete)
            {
                this.Flash();
            }

            this.workingPanel.Visible = false;
            this.donePanel.Visible = true;

            if (!this.IsMagnetLink)
            {
                File.Delete(this.TorrentURI);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.backgroundWorker.CancelAsync();

            this.btnCancel.Enabled = false;
            this.workingLabel.Text = Strings.UploadCancelling;
        }

        private void serverUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(this.serverUrl.Text);
        }

        #endregion < Event Handlers >

        #region < Properties >

        private string TorrentURI
        {
            get
            {
                return Environment.GetCommandLineArgs()[1];
            }
        }

        private bool IsMagnetLink
        {
            get
            {
                return this.TorrentURI.ToLowerInvariant().StartsWith("magnet:");
            }
        }

        #endregion
    }
}
