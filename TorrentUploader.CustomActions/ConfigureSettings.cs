using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.IO;
using System;

namespace TorrentUploader.CustomActions
{
    [RunInstaller(true)]
    public partial class ConfigureSettings : Installer
    {
        public ConfigureSettings()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            string path = string.Format("{0}TorrentUploader.exe.config", this.Context.Parameters["TARGETDIR"]);
            string s = File.ReadAllText(path)
                .Replace("{{SERVERADDRESS}}", this.Context.Parameters["SERVERADDRESS"])
                .Replace("{{USERNAME}}", this.Context.Parameters["USERNAME"])
                .Replace("{{PASSWORD}}", this.Context.Parameters["PASSWORD"]);

            File.WriteAllText(path, s);
        }
    }
}
