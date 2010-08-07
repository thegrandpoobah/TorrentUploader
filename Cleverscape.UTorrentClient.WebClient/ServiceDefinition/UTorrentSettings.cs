using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Cleverscape.UTorrentClient.WebClient.ServiceDefinition
{
    [DataContract(Namespace = "")]
    public class UTorrentSettings
    {
        [DataMember(Name = "build", Order = 1)]
        public int BuildNumber { get; set; }

        [DataMember(Name = "settings", Order = 2)]
        public UTorrentSettingsList Settings { get; set; }

    }

    [CollectionDataContract(Namespace = "")]
    public class UTorrentSettingsList : List<string[]>
    {

    }
}
