using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Cleverscape.UTorrentClient.WebClient.ServiceDefinition
{
    [DataContract(Namespace = "")]
    public class UpdatedTorrentsAndLabels
    {
        [DataMember(Name = "build", Order = 1)]
        public int BuildNumber { get; set; }

        [DataMember(Name = "label", Order = 2)]
        public LabelsList Labels { get; set; }

        [DataMember(Name = "torrentp", Order = 3)]
        public ChangedTorrentsList ChangedTorrents { get; set; }

        [DataMember(Name = "torrentm", Order = 4)]
        public RemovedTorrentsList RemovedTorrents { get; set; }

        [DataMember(Name = "torrents", Order = 5)]
        public TorrentsList Torrents { get; set; }

        [DataMember(Name = "torrentc", Order = 6)]
        public string CacheID { get; set; }
    }

    [CollectionDataContract(Namespace = "")]
    public class ChangedTorrentsList : List<string[]>
    {

    }

    [CollectionDataContract(Namespace = "")]
    public class RemovedTorrentsList : List<string[]>
    {

    }
}
