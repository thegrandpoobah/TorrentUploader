using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Cleverscape.UTorrentClient.WebClient.ServiceDefinition;
using System.ComponentModel;
using System.Windows.Threading;

namespace Cleverscape.UTorrentClient.WebClient
{
    /// <summary>
    /// Represents a collection of Torrents
    /// </summary>
    public class TorrentCollection : IList<Torrent>, INotifyCollectionChanged, INotifyPropertyChanged
    {

        private List<Torrent> _torrentCollectionInternal;

        internal UTorrentWebClient ParentClient { get; private set; }

        internal TorrentCollection(UTorrentWebClient ParentClient)
        {
            this.ParentClient = ParentClient;
            _torrentCollectionInternal = new List<Torrent>();
        }

        #region Internal Methods

        internal void Parse(TorrentsList TorrentsToParse, bool IsFresh)
        {
            List<string> ListOfHashes = new List<string>();
            foreach (string[] TorrentArray in TorrentsToParse)
            {
                if (TorrentArray.Length == 0)
                {
                    throw new FormatException("The array of torrent data was not in the expected format.");
                }
                ListOfHashes.Add(TorrentArray[0]);
                Torrent NewTorrent = GetByHashCode(TorrentArray[0]);
                if (NewTorrent == null)
                {
                    NewTorrent = new Torrent(TorrentArray, this);
                    _torrentCollectionInternal.Add(NewTorrent);
                    if (!IsFresh)
                    {
                        ParentClient.CallEvent(UTorrentWebClient.EventType.Added, NewTorrent);
                    }
                    CallCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, NewTorrent));
                }
                else
                {
                    NewTorrent.UpdateValuesFromStringArray(TorrentArray);
                    
                }
            }
            IEnumerable<Torrent> RemovedTorrents = RemoveWhereHashCodeIsNotInList(ListOfHashes);
            foreach (Torrent RemovedTorrent in RemovedTorrents)
            {
                CallCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, RemovedTorrent));
            }
        }

        internal void Parse(TorrentsList TorrentsToParse, RemovedTorrentsList RemovedTorrentsToParse, ChangedTorrentsList ChangedTorrentsToParse)
        {
            if (RemovedTorrentsToParse == null || ChangedTorrentsToParse == null)
            {
                Parse(TorrentsToParse, false);
            }
            else
            {
                List<Torrent> RemovedTorrents = new List<Torrent>();
                foreach (string[] TorrentArray in RemovedTorrentsToParse)
                {
                    if (TorrentArray.Length == 0)
                    {
                        throw new FormatException("The array of torrent data was not in the expected format.");
                    }
                    RemovedTorrents.Clear();
                    RemovedTorrents.AddRange(RemoveByHashCode(TorrentArray[0]));
                    foreach (Torrent RemovedTorrent in RemovedTorrents)
                    {
                        CallCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, RemovedTorrent));
                    }
                }
                foreach (string[] TorrentArray in ChangedTorrentsToParse)
                {
                    if (TorrentArray.Length == 0)
                    {
                        throw new FormatException("The array of torrent data was not in the expected format.");
                    }
                    Torrent NewTorrent = GetByHashCode(TorrentArray[0]);
                    if (NewTorrent == null)
                    {
                        NewTorrent = new Torrent(TorrentArray, this);
                        _torrentCollectionInternal.Add(NewTorrent);
                        ParentClient.CallEvent(UTorrentWebClient.EventType.Added, NewTorrent);
                        CallCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, NewTorrent));
                    }
                    else
                    {
                        NewTorrent.UpdateValuesFromStringArray(TorrentArray);
                    }
                }
            }
        }

        private void CallCollectionChanged(NotifyCollectionChangedEventArgs EventArgs)
        {
            if (CollectionChanged != null)
            {
                if (ParentClient.Dispatcher == null || ParentClient.Dispatcher.CheckAccess())
                {
                    CollectionChanged(this, EventArgs);
                }
                else
                {
                    ParentClient.Dispatcher.BeginInvoke(new CallCollectionChangedAsyncCallback(CallCollectionChangedAsync), DispatcherPriority.Loaded, new object[] { EventArgs });
                }
            }
        }

        private void CallCollectionChangedAsync(NotifyCollectionChangedEventArgs EventArgs)
        {
            CollectionChanged(this, EventArgs);
        }

        private delegate void CallCollectionChangedAsyncCallback(NotifyCollectionChangedEventArgs EventArgs);

        internal void AddTorrent(Torrent TorrentToAdd)
        {
            _torrentCollectionInternal.Add(TorrentToAdd);
        }

        #endregion

        #region Private Methods

        private IEnumerable<Torrent> RemoveByHashCode(string Hash)
        {
            List<Torrent> TorrentsToRemove = new List<Torrent>();
            foreach (Torrent CurrentTorrent in _torrentCollectionInternal)
            {
                if (CurrentTorrent.Hash == Hash)
                {
                    TorrentsToRemove.Add(CurrentTorrent);
                }
            }
            foreach (Torrent TorrentToRemove in TorrentsToRemove)
            {
                _torrentCollectionInternal.Remove(TorrentToRemove);
            }
            return TorrentsToRemove;
        }

        private IEnumerable<Torrent> RemoveWhereHashCodeIsNotInList(IEnumerable<string> ListOfHashes)
        {
            List<Torrent> TorrentsToRemove = new List<Torrent>();
            foreach (Torrent CurrentTorrent in _torrentCollectionInternal)
            {
                if (!ListOfHashes.Contains(CurrentTorrent.Hash))
                {
                    TorrentsToRemove.Add(CurrentTorrent);
                }
            }
            foreach (Torrent TorrentToRemove in TorrentsToRemove)
            {
                _torrentCollectionInternal.Remove(TorrentToRemove);
            }
            return TorrentsToRemove;
        }

        #endregion

        #region Public Methods

        public bool Contains(string Hash)
        {
            foreach (Torrent CurrentTorrent in _torrentCollectionInternal)
            {
                if (CurrentTorrent.Hash == Hash)
                {
                    return true;
                }
            }
            return false;
        }

        public Torrent GetByHashCode(string Hash)
        {
            foreach (Torrent CurrentTorrent in _torrentCollectionInternal)
            {
                if (CurrentTorrent.Hash == Hash)
                {
                    return CurrentTorrent;
                }
            }
            return null;
        }

        public TorrentCollection GetTorrentsByLabel(TorrentLabel Label)
        {
            TorrentCollection ReturnList = new TorrentCollection(ParentClient);
            foreach (Torrent CurrentTorrent in _torrentCollectionInternal)
            {
                if (CurrentTorrent.Label == Label.ToString())
                {
                    ReturnList.AddTorrent(CurrentTorrent);
                }
            }
            return ReturnList;
        }

        public void Insert(int index, Torrent item)
        {
        }

        public void Clear()
        {
        }

        public void Add(Torrent item)
        {
        }

        public int IndexOf(Torrent item)
        {
            return _torrentCollectionInternal.IndexOf(item);
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public Torrent this[int index]
        {
            get
            {
                return _torrentCollectionInternal[index];
            }
            set
            {
            }
        }

        public bool Contains(Torrent item)
        {
            return (GetByHashCode(item.Hash) != null);
        }

        public void CopyTo(Torrent[] array, int arrayIndex)
        {
            _torrentCollectionInternal.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _torrentCollectionInternal.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(Torrent item)
        {
            if (GetByHashCode(item.Hash) != null)
            {
                _torrentCollectionInternal.Remove(item);
                return true;
            }
            return false;
        }

        public IEnumerator<Torrent> GetEnumerator()
        {
            return _torrentCollectionInternal.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _torrentCollectionInternal.GetEnumerator();
        }

        #endregion

        #region Public Events (Interface Implementation)

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        #endregion

    }
}
