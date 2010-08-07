using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Cleverscape.UTorrentClient.WebClient.ServiceDefinition
{
    [DataContract(Namespace = "")]
    public class TorrentsAndLabels
    {
        [DataMember(Name = "build", Order = 1)]
        public int BuildNumber { get; set; }

        [DataMember(Name = "label", Order = 2)]
        public LabelsList Labels { get; set; }

        [DataMember(Name = "torrents", Order = 3)]
        public TorrentsList Torrents { get; set; }

        [DataMember(Name = "torrentc", Order = 4)]
        public string CacheID { get; set; }
    }

    [CollectionDataContract(Namespace = "")]
    public class LabelsList : List<string[]>
    {

    }

    [CollectionDataContract(Namespace = "")]
    public class TorrentsList : List<string[]>
    {

    }

}
