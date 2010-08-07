using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cleverscape.UTorrentClient.WebClient
{
    /// <summary>
    /// Represents a file within a torrent
    /// </summary>
    public class TorrentFile
    {
        private int _priorityInteger;
        
        internal TorrentFileCollection ParentCollection;
        internal int Index;

        #region Constructor and parsing data from Web UI

        internal TorrentFile(TorrentFileCollection ParentCollection, object[] FileArray, int Index)
        {
            this.ParentCollection = ParentCollection;
            UpdateValuesFromArray(FileArray, Index);
        }

        internal void UpdateValuesFromArray(object[] FileArray, int Index)
        {
            if (FileArray.Length != 4 || !(FileArray[0] is string) || !(FileArray[1] is int || FileArray[1] is long) || !(FileArray[2] is int || FileArray[2] is long) || !(FileArray[3] is int))
            {
                throw new FormatException("The array of file data was not in the expected format.");
            }
            Name = (string)FileArray[0];
            SizeTotalBytes = Convert.ToInt64(FileArray[1]);
            SizeDownloadedBytes = Convert.ToInt64(FileArray[2]);
            _priorityInteger = Convert.ToInt32(FileArray[3]);
            this.Index = Index;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The name of the file
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The total size of the file in bytes
        /// </summary>
        public long SizeTotalBytes { get; private set; }

        /// <summary>
        /// The total size of the file as a formatted string (using Bytes, KB, MB, GB, etc)
        /// </summary>
        public string SizeTotalFormatted
        {
            get
            {
                return Utilities.FormatFileSize(SizeTotalBytes);
            }
        }

        /// <summary>
        /// The size of the data downloaded from the file in bytes
        /// </summary>
        public long SizeDownloadedBytes { get; private set; }

        /// <summary>
        /// The size of the data downloaded from the file as a formatted string (using Bytes, KB, MB, GB, etc)
        /// </summary>
        public string SizeDownloadedFormatted
        {
            get
            {
                return Utilities.FormatFileSize(SizeDownloadedBytes);
            }
        }
        
        /// <summary>
        /// The priority of the file.
        /// Note: Setting the value of this property will update the uTorrent web interface with the new value of the property and then update the list of files for this torrent.
        /// </summary>
        public FilePriority Priority
        {
            get
            {
                return (FilePriority)_priorityInteger;
            }
            set
            {
                ParentCollection.ParentTorrent.ParentCollection.ParentClient.SetTorrentFilePriority(this, (int)value);
            }
        }

        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            return (obj is TorrentFile) && ((TorrentFile)obj).ParentCollection.ParentTorrent == ParentCollection.ParentTorrent && ((TorrentFile)obj).Index == Index;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion
    }

    #region FilePriority enum

    /// <summary>
    /// Represents the download priority of a file within a torrent
    /// </summary>
    public enum FilePriority : int
    {
        /// <summary>
        /// Don't download the file
        /// </summary>
        DontDownload = 0,
        /// <summary>
        /// Low priority
        /// </summary>
        LowPriority = 1,
        /// <summary>
        /// Normal priority
        /// </summary>
        NormalPriority = 2,
        /// <summary>
        /// High priority
        /// </summary>
        HighPriority = 3
    }

    #endregion

    /// <summary>
    /// Represents a collection of files within a torrent
    /// </summary>
    public class TorrentFileCollection : IEnumerable<TorrentFile>
    {
        private List<TorrentFile> _torrentFileCollectionInternal;

        internal Torrent ParentTorrent;

        #region Constructor and parsing data from Web UI

        internal TorrentFileCollection(Torrent ParentTorrent)
        {
            this.ParentTorrent = ParentTorrent;
            _torrentFileCollectionInternal = new List<TorrentFile>();
        }

        internal void ParseFiles(object[] Files, Torrent ParentTorrent)
        {
            if (Files.Length != 2 || !(Files[1] is object[]))
            {
                throw new FormatException("The array of file data was not in the expected format.");
            }
            object[] FilesList = (object[])Files[1];
            for (int i = 0; i < FilesList.Length; i++)
            {
                if (_torrentFileCollectionInternal.Count <= i)
                {
                    TorrentFile NewFile = new TorrentFile(this, (object[])FilesList[i], i);
                    _torrentFileCollectionInternal.Insert(i, NewFile);
                }
                else
                {
                    _torrentFileCollectionInternal[i].UpdateValuesFromArray((object[])FilesList[i], i);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the index of a TorrentFile within the collection
        /// </summary>
        /// <param name="item">A TorrentFile to return the index of</param>
        /// <returns></returns>
        public int IndexOf(TorrentFile item)
        {
            return _torrentFileCollectionInternal.IndexOf(item);
        }

        /// <summary>
        /// Returns the TorrentFile at the specified index
        /// </summary>
        /// <param name="index">The index used to find the TorrentFile</param>
        /// <returns></returns>
        public TorrentFile this[int index]
        {
            get
            {
                return _torrentFileCollectionInternal[index];
            }
        }

        /// <summary>
        /// Returns whether the collection contains the specified TorrentFile
        /// </summary>
        /// <param name="item">The TorrentFile to determine if the collection contains</param>
        /// <returns></returns>
        public bool Contains(TorrentFile item)
        {
            return _torrentFileCollectionInternal.Contains(item);
        }

        /// <summary>
        /// Copies the collection to an array of TorrentFiles
        /// </summary>
        /// <param name="array">The array to copy the collection to</param>
        /// <param name="arrayIndex">The index to start copying into</param>
        public void CopyTo(TorrentFile[] array, int arrayIndex)
        {
            _torrentFileCollectionInternal.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// The number of TorrentFiles in the collection
        /// </summary>
        public int Count
        {
            get { return _torrentFileCollectionInternal.Count; }
        }

        /// <summary>
        /// Whether the collection is read only (it is)
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        public IEnumerator<TorrentFile> GetEnumerator()
        {
            return _torrentFileCollectionInternal.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _torrentFileCollectionInternal.GetEnumerator();
        }

        #endregion
    }
}
