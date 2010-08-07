using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TorrentUploader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Environment.GetCommandLineArgs().Length != 2)
            {
                MessageBox.Show(
                    "Must supply path to a torrent file.",
                    "TorrentUploader",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ProgressWindow());
            }
        }
    }
}
