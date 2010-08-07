using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Cleverscape.UTorrentClient.WebClient.ServiceDefinition
{
    [DataContract(Namespace = "")]
    public class TorrentFiles
    {
        [DataMember(Name = "build", Order = 1)]
        public int BuildNumber { get; set; }

        [DataMember(Name = "files", Order = 2)]
        public object[] Files { get; set; }

    }
}
