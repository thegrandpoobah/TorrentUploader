using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cleverscape.UTorrentClient.WebClient
{
    /// <summary>
    /// Represents a tracker used by a torrent
    /// </summary>
    public class Tracker
    {
        internal TrackerCollection ParentCollection;
        private string _address;

        #region Constructors

        public Tracker(string Address)
        {
            _address = Address;
        }

        internal Tracker(string Address, TrackerCollection ParentCollection)
        {
            this.ParentCollection = ParentCollection;
            _address = Address;
        }

        #endregion

        /// <summary>
        /// The address of the tracker
        /// </summary>
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
                if (IsReady)
                {
                    ParentCollection.UpdateTrackers();
                }
            }
        }

        internal bool IsReady
        {
            get
            {
                return ParentCollection != null;
            }
        }

        #region Public Methods

        public override bool Equals(object obj)
        {
            if (obj is Tracker && ((Tracker)obj).Address == Address)
            {
                if (IsReady || ((Tracker)obj).IsReady)
                {
                    return IsReady && ((Tracker)obj).IsReady
                        && ((Tracker)obj).ParentCollection.IsReady && ParentCollection.IsReady
                        && ((Tracker)obj).ParentCollection.ParentTorrent == ParentCollection.ParentTorrent;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }   
        }

        public override int GetHashCode()
        {
            return Address.GetHashCode();
        }

        public override string ToString()
        {
            return Address.ToString();
        }

        #endregion
    }

    /// <summary>
    /// Represents a collection of trackers
    /// </summary>
    public class TrackerCollection : IList<Tracker>
    {
        private List<Tracker> _trackerCollectionInternal;
        internal Torrent ParentTorrent;

        internal bool IsReady
        {
            get
            {
                return ParentTorrent != null;
            }
        }

        #region Constructors and parsing data from Web UI

        public TrackerCollection()
        {
            _trackerCollectionInternal = new List<Tracker>();
        }

        internal TrackerCollection(Torrent ParentTorrent)
            : this()
        {
            this.ParentTorrent = ParentTorrent;
        }

        internal void ParseTrackers(string TrackersList)
        {
            string[] TrackersArray = TrackersList.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (string CurrentTrackerString in TrackersArray)
            {
                if (CurrentTrackerString.Trim().Length > 0)
                {
                    Tracker CurrentTracker = GetTrackerByAddress(CurrentTrackerString);
                    if (CurrentTracker == null)
                    {
                        CurrentTracker = new Tracker(CurrentTrackerString, this);
                        _trackerCollectionInternal.Add(CurrentTracker);
                    }
                }
            }
            List<Tracker> TrackersToRemove = new List<Tracker>();
            foreach (Tracker CurrentTracker in _trackerCollectionInternal)
            {
                bool TrackerExists = false;
                foreach (string CurrentTrackerString in TrackersArray)
                {
                    if (CurrentTracker.Address.ToLowerInvariant() == CurrentTrackerString.ToLowerInvariant())
                    {
                        TrackerExists = true;
                    }
                }
                if (!TrackerExists)
                {
                    TrackersToRemove.Add(CurrentTracker);
                }
            }
            foreach (Tracker TrackerToRemove in TrackersToRemove)
            {
                _trackerCollectionInternal.Remove(TrackerToRemove);
            }
        }

        #endregion

        internal void UpdateTrackers()
        {
            if (IsReady)
            {
                ParentTorrent.SaveTrackers();
            }
        }

        #region Public Methods

        public override string ToString()
        {
            string ReturnString = "";
            foreach (Tracker CurrentTracker in _trackerCollectionInternal)
            {
                ReturnString = String.Format("{0}{1}\r\n\r\n", ReturnString, CurrentTracker.Address);
            }
            return ReturnString;
        }

        public Tracker GetTrackerByAddress(string Address)
        {
            foreach (Tracker CurrentTracker in _trackerCollectionInternal)
            {
                if (CurrentTracker.Address.ToLowerInvariant() == Address.ToLowerInvariant())
                {
                    return CurrentTracker;
                }
            }
            return null;
        }

        public void Add(Tracker TrackerToAdd)
        {
            if (GetTrackerByAddress(TrackerToAdd.Address) != null)
            {
                _trackerCollectionInternal.Add(TrackerToAdd);
                UpdateTrackers();
            }
        }

        public int IndexOf(Tracker item)
        {
            return _trackerCollectionInternal.IndexOf(item);
        }

        public void Insert(int index, Tracker item)
        {
            if (GetTrackerByAddress(item.Address) != null)
            {
                _trackerCollectionInternal.Insert(index, item);
                UpdateTrackers();
            }
        }

        public void RemoveAt(int index)
        {
            _trackerCollectionInternal.RemoveAt(index);
            UpdateTrackers();
        }

        public Tracker this[int index]
        {
            get
            {
                return _trackerCollectionInternal[index];
            }
            set
            {
                _trackerCollectionInternal[index] = value;
                UpdateTrackers();
            }
        }

        public void Clear()
        {
            _trackerCollectionInternal.Clear();
            UpdateTrackers();
        }

        public bool Contains(Tracker item)
        {
            return (GetTrackerByAddress(item.Address) != null);
        }

        public void CopyTo(Tracker[] array, int arrayIndex)
        {
            _trackerCollectionInternal.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _trackerCollectionInternal.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Tracker item)
        {
            if (GetTrackerByAddress(item.Address) != null)
            {
                _trackerCollectionInternal.Remove(item);
                UpdateTrackers();
                return true;
            }
           return false;
        }

        public IEnumerator<Tracker> GetEnumerator()
        {
            return _trackerCollectionInternal.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _trackerCollectionInternal.GetEnumerator();
        }

        #endregion
    }
}
