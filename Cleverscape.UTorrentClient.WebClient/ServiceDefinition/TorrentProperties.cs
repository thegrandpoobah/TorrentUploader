using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Cleverscape.UTorrentClient.WebClient.ServiceDefinition
{
    [DataContract(Namespace = "")]
    public class TorrentProperties
    {
        [DataMember(Name = "build", Order = 1)]
        public int BuildNumber { get; set; }

        [DataMember(Name = "props", Order = 2)]
        public TorrentPropertiesList[] Properties { get; set; }

    }

    [DataContract(Namespace = "")]
    public class TorrentPropertiesList
    {
        [DataMember(Name = "hash", Order = 1)]
        public string Hash { get; set; }

        [DataMember(Name = "trackers", Order = 2)]
        public string Trackers { get; set; }

        [DataMember(Name = "ulrate", Order = 3)]
        public long MaximumUploadRate { get; set; }

        [DataMember(Name = "dlrate", Order = 4)]
        public long MaximumDownloadRate { get; set; }

        [DataMember(Name = "superseed", Order = 5)]
        public int InitialSeeding { get; set; }

        [DataMember(Name = "dht", Order = 6)]
        public int UseDht { get; set; }

        [DataMember(Name = "pex", Order = 7)]
        public int UsePeerExchange { get; set; }

        [DataMember(Name = "seed_override", Order = 8)]
        public int OverrideQueueing { get; set; }

        [DataMember(Name = "seed_ratio", Order = 9)]
        public int SeedRatio { get; set; }

        [DataMember(Name = "seed_time", Order = 10)]
        public long SeedTime { get; set; }

        [DataMember(Name = "ulslots", Order = 11)]
        public int UploadSlots { get; set; }

    }
}
