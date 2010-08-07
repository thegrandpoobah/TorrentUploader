using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Cleverscape.UTorrentClient.WebClient
{
    /// <summary>
    /// Represents a torrent currently active within uTorrent
    /// </summary>
    public class Torrent : INotifyPropertyChanged
    {

        #region Private Fields

        private DateTime _filesLastUpdated;
        private DateTime _propertiesLastUpdated;

        internal TorrentCollection ParentCollection;
        private TorrentFileCollection _files;

        private string _hash;
        private int _statusCode;
        private string _name;
        private long _sizeTotalBytes;
        private int _progressTenthsPercent;
        private long _sizeDownloadedBytes;
        private long _sizeUploadedBytes;
        private int _seedRatioTenthsPercent;
        private long _speedUploadBytes;
        private long _speedDownloadBytes;
        private long _timeEstimateSeconds;
        private string _label;
        private int _peersConnected;
        private int _peersTotal;
        private int _seedsConnected;
        private int _seedsTotal;
        private long _availabilityFraction;
        private int _queueOrder;
        private long _sizeRemainingBytes;

        #endregion

        #region Constructor

        internal Torrent(string[] InputValues, TorrentCollection ParentCollection)
        {
            this.ParentCollection = ParentCollection;
            UpdateValuesFromStringArray(InputValues, true);
        }

        internal bool UpdateValuesFromStringArray(string[] InputValues)
        {
            return UpdateValuesFromStringArray(InputValues, false);
        }

        private bool UpdateValuesFromStringArray(string[] InputValues, bool NewTorrent)
        {
            bool ReturnChanged = false;
            bool CallFinishedDownloading = false;
            bool CallStarted = false;
            bool CallStopped = false;
            bool CallPaused = false;
            bool CallUnPaused = false;
            if (InputValues.Length != 19)
            {
                throw new FormatException("The array of torrent data was not in the expected format.");
            }
            if (Hash != InputValues[0])
            {
                Hash = InputValues[0];
                ReturnChanged = true;
            }
            if (StatusCode != int.Parse(InputValues[1]))
            {
                TorrentStatus NewStatus = (TorrentStatus)int.Parse(InputValues[1]);
                if (((NewStatus & TorrentStatus.Paused) == TorrentStatus.Paused) && ((Status & TorrentStatus.Paused) != TorrentStatus.Paused) && !NewTorrent)
                {
                    CallPaused = true;
                }
                if (((NewStatus & TorrentStatus.Paused) != TorrentStatus.Paused) && ((Status & TorrentStatus.Paused) == TorrentStatus.Paused) && !NewTorrent)
                {
                    CallUnPaused = true;
                }
                if (((NewStatus & TorrentStatus.Started) == TorrentStatus.Started) && ((Status & TorrentStatus.Started) != TorrentStatus.Started) && !NewTorrent)
                {
                    CallStarted = true;
                }
                if (((NewStatus & TorrentStatus.Started) != TorrentStatus.Started) && ((Status & TorrentStatus.Started) == TorrentStatus.Started) && !NewTorrent)
                {
                    CallStopped = true;
                }
                StatusCode = int.Parse(InputValues[1]);
                ReturnChanged = true;
            }
            if (Name != InputValues[2])
            {
                Name = InputValues[2];
                ReturnChanged = true;
            }
            if (SizeTotalBytes != long.Parse(InputValues[3]))
            {
                SizeTotalBytes = long.Parse(InputValues[3]);
                ReturnChanged = true;
            }
            if (ProgressTenthsPercent != int.Parse(InputValues[4]))
            {
                ProgressTenthsPercent = int.Parse(InputValues[4]);
                if (ProgressTenthsPercent < 1000 && int.Parse(InputValues[4]) == 1000 && !NewTorrent)
                {
                    CallFinishedDownloading = true;
                }
                ReturnChanged = true;
            }
            if (SizeDownloadedBytes != long.Parse(InputValues[5]))
            {
                SizeDownloadedBytes = long.Parse(InputValues[5]);
                ReturnChanged = true;
            }
            if (SizeUploadedBytes != long.Parse(InputValues[6]))
            {
                SizeUploadedBytes = long.Parse(InputValues[6]);
                ReturnChanged = true;
            }
            if (SeedRatioTenthsPercent != int.Parse(InputValues[7]))
            {
                SeedRatioTenthsPercent = int.Parse(InputValues[7]);
                ReturnChanged = true;
            }
            if (SpeedUploadBytes != long.Parse(InputValues[8]))
            {
                SpeedUploadBytes = long.Parse(InputValues[8]);
                ReturnChanged = true;
            }
            if (SpeedDownloadBytes != long.Parse(InputValues[9]))
            {
                SpeedDownloadBytes = long.Parse(InputValues[9]);
                ReturnChanged = true;
            }
            if (TimeEstimateSeconds != long.Parse(InputValues[10]))
            {
                TimeEstimateSeconds = long.Parse(InputValues[10]);
                ReturnChanged = true;
            }
            if (Label != InputValues[11])
            {
                Label = InputValues[11];
                ReturnChanged = true;
            }
            if (PeersConnected != int.Parse(InputValues[12]))
            {
                PeersConnected = int.Parse(InputValues[12]);
                ReturnChanged = true;
            }
            if (PeersTotal != int.Parse(InputValues[13]))
            {
                PeersTotal = int.Parse(InputValues[13]);
                ReturnChanged = true;
            }
            if (SeedsConnected != int.Parse(InputValues[14]))
            {
                SeedsConnected = int.Parse(InputValues[14]);
                ReturnChanged = true;
            }
            if (SeedsTotal != int.Parse(InputValues[15]))
            {
                SeedsTotal = int.Parse(InputValues[15]);
                ReturnChanged = true;
            }
            if (AvailabilityFraction != long.Parse(InputValues[16]))
            {
                AvailabilityFraction = long.Parse(InputValues[16]);
                ReturnChanged = true;
            }
            if (QueueOrder != int.Parse(InputValues[17]))
            {
                QueueOrder = int.Parse(InputValues[17]);
                ReturnChanged = true;
            }
            if (SizeRemainingBytes != long.Parse(InputValues[18]))
            {
                SizeRemainingBytes = long.Parse(InputValues[18]);
                ReturnChanged = true;
            }
            if (CallFinishedDownloading)
            {
                ParentCollection.ParentClient.CallEvent(UTorrentWebClient.EventType.FinishedDownloading, this);
            }
            if (CallPaused)
            {
                ParentCollection.ParentClient.CallEvent(UTorrentWebClient.EventType.Paused, this);
            }
            if (CallUnPaused)
            {
                ParentCollection.ParentClient.CallEvent(UTorrentWebClient.EventType.UnPaused, this);
            }
            if (CallStarted)
            {
                ParentCollection.ParentClient.CallEvent(UTorrentWebClient.EventType.Started, this);
            }
            if (CallStopped)
            {
                ParentCollection.ParentClient.CallEvent(UTorrentWebClient.EventType.Stopped, this);
            }
            return ReturnChanged;
        }

        #endregion

        #region Public Methods (Overriden from object)

        public override bool Equals(object obj)
        {
            return (obj is Torrent) && ((Torrent)obj).Hash == this.Hash;
        }

        public override int GetHashCode()
        {
            return this.Hash.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }

        #endregion

        #region Public Events (INotifyPropertyChanged)

        /// <summary>
        /// Event to notify when a property of the torrent has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        #endregion

        #region Internal Properties

        internal DateTime FilesLastUpdated
        {
            get
            {
                return _filesLastUpdated;
            }
            set
            {
                _filesLastUpdated = FilesLastUpdated;
            }
        }

        internal TorrentFileCollection FileList
        {
            get
            {
                return _files;
            }
            set
            {
                _files = value;
            }
        }

        internal DateTime PropertiesLastUpdated
        {
            get
            {
                return _propertiesLastUpdated;
            }
            set
            {
                _propertiesLastUpdated = value;
            }
        }

        #endregion

        #region Private Properties

        private int StatusCode
        {
            get
            {
                return _statusCode;
            }
            set
            {
                _statusCode = value;
                NotifyPropertyChanged("Status");
                NotifyPropertyChanged("SimpleStatus");
                NotifyPropertyChanged("StatusCategory");
            }
        }

        private int ProgressTenthsPercent
        {
            get
            {
                return _progressTenthsPercent;
            }
            set
            {
                _progressTenthsPercent = value;
                NotifyPropertyChanged("ProgressPercent");
                NotifyPropertyChanged("SimpleStatus");
                NotifyPropertyChanged("StatusCategory");
            }
        }

        private int SeedRatioTenthsPercent
        {
            get
            {
                return _seedRatioTenthsPercent;
            }
            set
            {
                _seedRatioTenthsPercent = value;
                NotifyPropertyChanged("SeedRatio");
            }
        }

        private long TimeEstimateSeconds
        {
            get
            {
                return _timeEstimateSeconds;
            }
            set
            {
                _timeEstimateSeconds = value;
                NotifyPropertyChanged("TimeEstimate");
            }
        }

        private long AvailabilityFraction
        {
            get
            {
                return _availabilityFraction;
            }
            set
            {
                _availabilityFraction = value;
                NotifyPropertyChanged("Availability");
            }
        }

        #endregion

        #region Public Properties (Basic Torrent Properties)

        /// <summary>
        /// The hash code of the torrent
        /// </summary>
        public string Hash
        {
            get
            {
                return _hash;
            }
            private set
            {
                _hash = value;
                NotifyPropertyChanged("Hash");
            }
        }

        /// <summary>
        /// The status of the torrent
        /// </summary>
        public TorrentStatus Status
        {
            get
            {
                return (TorrentStatus)_statusCode;
            }
        }

        /// <summary>
        /// The name of the torrent
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            private set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        /// <summary>
        /// The total size of the torrent in bytes
        /// </summary>
        public long SizeTotalBytes
        {
            get
            {
                return _sizeTotalBytes;
            }
            private set
            {
                _sizeTotalBytes = value;
                NotifyPropertyChanged("SizeTotalBytes");
                NotifyPropertyChanged("SizeTotalFormatted");
            }
        }

        /// <summary>
        /// The total size of the torrent as a formatted string (using Bytes, KB, MB, GB, etc)
        /// </summary>
        public string SizeTotalFormatted
        {
            get { return Utilities.FormatFileSize(SizeTotalBytes); }
        }

        /// <summary>
        /// The progress of the torrent expressed as a percentage
        /// </summary>
        public decimal ProgressPercent
        {
            get { return Decimal.Divide(_progressTenthsPercent, 1000); }
        }

        /// <summary>
        /// The size of the data downloaded so far in bytes
        /// </summary>
        public long SizeDownloadedBytes
        {
            get
            {
                return _sizeDownloadedBytes;
            }
            private set
            {
                _sizeDownloadedBytes = value;
                NotifyPropertyChanged("SizeDownloadedBytes");
                NotifyPropertyChanged("SizeDownloadedFormatted");
            }
        }

        /// <summary>
        /// The size of the data downloaded so far as a formatted string (using Bytes, KB, MB, GB, etc)
        /// </summary>
        public string SizeDownloadedFormatted
        {
            get { return Utilities.FormatFileSize(SizeDownloadedBytes); }
        }

        /// <summary>
        /// The size of the data uploaded so far in bytes
        /// </summary>
        public long SizeUploadedBytes
        {
            get
            {
                return _sizeUploadedBytes;
            }
            private set
            {
                _sizeUploadedBytes = value;
                NotifyPropertyChanged("SizeUploadedBytes");
                NotifyPropertyChanged("SizeUploadedFormatted");
            }
        }

        /// <summary>
        /// The size of the data uploaded so far as a formatted string (using Bytes, KB, MB, GB, etc)
        /// </summary>
        public string SizeUploadedFormatted
        {
            get { return Utilities.FormatFileSize(SizeUploadedBytes); }
        }

        /// <summary>
        /// The seed ratio expressed as a fraction of the total size (for example 0.415 or 1.368)
        /// </summary>
        public decimal SeedRatio
        {
            get { return Decimal.Divide(_seedRatioTenthsPercent, 1000); }
        }

        /// <summary>
        /// The current upload speed for this torrent in bytes per second
        /// </summary>
        public long SpeedUploadBytes
        {
            get
            {
                return _speedUploadBytes;
            }
            private set
            {
                _speedUploadBytes = value;
                NotifyPropertyChanged("SpeedUploadBytes");
                NotifyPropertyChanged("SpeedUploadBytesFormatted");
                NotifyPropertyChanged("SpeedUploadBitsFormatted");
                NotifyPropertyChanged("StatusCategory");
            }
        }

        /// <summary>
        /// The current download speed for this torrent in bytes per second
        /// </summary>
        public long SpeedDownloadBytes
        {
            get
            {
                return _speedDownloadBytes;
            }
            private set
            {
                _speedDownloadBytes = value;
                NotifyPropertyChanged("SpeedDownloadBytes");
                NotifyPropertyChanged("SpeedDownloadBytesFormatted");
                NotifyPropertyChanged("SpeedDownloadBitsFormatted");
                NotifyPropertyChanged("StatusCategory");
            }
        }

        /// <summary>
        /// The current upload speed for this torrent as a formatted string (using Bytes/s, KB/s, MB/s, GB/s, etc)
        /// </summary>
        public string SpeedUploadBytesFormatted
        {
            get { return Utilities.FormatFileSize(SpeedUploadBytes, Utilities.FileSizeFormat.SpeedBytes); }
        }

        /// <summary>
        /// The current download speed for this torrent as a formatted string (using Bytes/s, KB/s, MB/s, GB/s, etc)
        /// </summary>
        public string SpeedDownloadBytesFormatted
        {
            get { return Utilities.FormatFileSize(SpeedDownloadBytes, Utilities.FileSizeFormat.SpeedBytes); }
        }

        /// <summary>
        /// The current upload speed for this torrent as a formatted string (using bits/s, Kb/s, Mb/s, Gb/s, etc)
        /// </summary>
        public string SpeedUploadBitsFormatted
        {
            get { return Utilities.FormatFileSize(SpeedUploadBytes, Utilities.FileSizeFormat.SpeedBits); }
        }

        /// <summary>
        /// The current download speed for this torrent as a formatted string (using bits/s, Kb/s, Mb/s, Gb/s, etc)
        /// </summary>
        public string SpeedDownloadBitsFormatted
        {
            get { return Utilities.FormatFileSize(SpeedDownloadBytes, Utilities.FileSizeFormat.SpeedBits); }
        }

        /// <summary>
        /// The estimated time before the torrent completes its current action (checking, downloading, seeding, etc)
        /// Note: this returns TimeSpan.MaxValue for infinite time periods
        /// </summary>
        public TimeSpan TimeEstimate
        {
            get
            {
                if (_timeEstimateSeconds > 0)
                {
                    return TimeSpan.FromSeconds(_timeEstimateSeconds);
                }
                else
                {
                    return TimeSpan.MaxValue;
                }
            }
        }

        /// <summary>
        /// The torrent's label
        /// </summary>
        public string Label
        {
            get
            {
                return _label;
            }
            private set
            {
                _label = value;
                NotifyPropertyChanged("Label");
            }
        }

        /// <summary>
        /// The number of peers connected
        /// </summary>
        public int PeersConnected
        {
            get
            {
                return _peersConnected;
            }
            private set
            {
                _peersConnected = value;
                NotifyPropertyChanged("PeersConnected");
                NotifyPropertyChanged("StatusCategory");
            }
        }

        /// <summary>
        /// The total number of peers
        /// </summary>
        public int PeersTotal
        {
            get
            {
                return _peersTotal;
            }
            private set
            {
                _peersTotal = value;
                NotifyPropertyChanged("PeersTotal");
            }
        }

        /// <summary>
        /// The number of peers connected and total displayed as 'Connected (Total)'
        /// </summary>
        public string Peers
        {
            get
            {
                return String.Format("{0} ({1})", PeersConnected, PeersTotal);
            }
        }

        /// <summary>
        /// The number of seeds connected
        /// </summary>
        public int SeedsConnected
        {
            get
            {
                return _seedsConnected;
            }
            private set
            {
                _seedsConnected = value;
                NotifyPropertyChanged("SeedsConnected");
                NotifyPropertyChanged("StatusCategory");
            }
        }

        /// <summary>
        /// The total number of seeds
        /// </summary>
        public int SeedsTotal
        {
            get
            {
                return _seedsTotal;
            }
            private set
            {
                _seedsTotal = value;
                NotifyPropertyChanged("SeedsTotal");
            }
        }

        /// <summary>
        /// The number of seeds connected and total displayed as 'Connected (Total)'
        /// </summary>
        public string Seeds
        {
            get
            {
                return String.Format("{0} ({1})", SeedsConnected, SeedsTotal);
            }
        }

        /// <summary>
        /// The ratio of seeds to peers
        /// </summary>
        public decimal SeedsPeersRatio
        {
            get
            {
                if (PeersTotal == 0)
                {
                    return decimal.MaxValue;
                }
                return decimal.Divide(SeedsTotal, PeersTotal);
            }
        }


        /// <summary>
        /// The availability of the torrent expressed as a fraction of the total torrent (for example 0.998, 1.045 or 3.381)
        /// </summary>
        public decimal Availability
        {
            get { return Decimal.Divide(_availabilityFraction, 65536); }
        }

        /// <summary>
        /// The order of the torrent in the uTorrent queue
        /// </summary>
        public int QueueOrder
        {
            get
            {
                return _queueOrder;
            }
            private set
            {
                _queueOrder = value;
                NotifyPropertyChanged("QueueOrder");
            }
        }

        /// <summary>
        /// The size of the data remaining to download in bytes
        /// </summary>
        public long SizeRemainingBytes
        {
            get
            {
                return _sizeRemainingBytes;
            }
            private set
            {
                _sizeRemainingBytes = value;
                NotifyPropertyChanged("SizeRemainingBytes");
                NotifyPropertyChanged("SizeRemainingFormatted");
            }
        }

        /// <summary>
        /// The size of the data remaining to download as a formatted string (using Bytes, KB, MB, GB, etc)
        /// </summary>
        public string SizeRemainingFormatted
        {
            get { return Utilities.FormatFileSize(SizeRemainingBytes); }
        }

        /// <summary>
        /// The simple status of the torrent.
        /// Note that the web interface doesn't support the statuses DownloadingError or SeedingError
        /// </summary>
        public TorrentSimpleStatus SimpleStatus
        {
            get
            {
                if ((Status & TorrentStatus.Error) == TorrentStatus.Error)
                {
                    return TorrentSimpleStatus.Error;
                }
                else
                {
                    if ((Status & TorrentStatus.Paused) == TorrentStatus.Paused)
                    {
                        return TorrentSimpleStatus.Paused;
                    }
                    if ((Status & TorrentStatus.Started) == TorrentStatus.Started)
                    {
                        return (IsDownloading) ? TorrentSimpleStatus.Downloading : TorrentSimpleStatus.Seeding;
                    }
                    else
                    {
                        if ((Status & TorrentStatus.Queued) == TorrentStatus.Queued)
                        {
                            return (IsDownloading) ? TorrentSimpleStatus.DownloadingQueued : TorrentSimpleStatus.SeedingQueued;
                        }
                        else
                        {
                            return (IsDownloading) ? TorrentSimpleStatus.InactiveIncomplete : TorrentSimpleStatus.InactiveCompleted;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The status category(s) that the torrent is in (e.g. Downloading, Active)
        /// </summary>
        public TorrentStatusCategory StatusCategory
        {
            get
            {
                TorrentStatusCategory ReturnCategory = TorrentStatusCategory.All;
                ReturnCategory = ReturnCategory | ((IsDownloading) ? TorrentStatusCategory.Downloading : TorrentStatusCategory.Completed);
                ReturnCategory = ReturnCategory | ((IsActive) ? TorrentStatusCategory.Active : TorrentStatusCategory.Inactive);
                return ReturnCategory;
            }
        }

        private bool IsActive
        {
            get
            {
                return SeedsConnected > 0 || PeersConnected > 0 || SpeedDownloadBytes > 0 || SpeedUploadBytes > 0;
            }
        }

        private bool IsDownloading
        {
            get
            {
                return (ProgressTenthsPercent < 1000);
            }
        }

        #endregion

        #region Public Properties (Collections)

        /// <summary>
        /// The collection of files that make up the torrent.
        /// Note: Accessing this property will call the uTorrent web interface to update the list of files for this torrent if it hasn't been updated recently.
        /// </summary>
        public TorrentFileCollection Files
        {
            get
            {
                ParentCollection.ParentClient.UpdateTorrentFiles(this);
                return _files;
            }
        }

        #endregion

        #region Public Methods (Torrent Actions)

        /// <summary>
        /// Start the torrent
        /// </summary>
        public void Start()
        {
            ParentCollection.ParentClient.TorrentStart(this);
        }

        /// <summary>
        /// Stop the torrent
        /// </summary>
        public void Stop()
        {
            ParentCollection.ParentClient.TorrentStop(this);
        }

        /// <summary>
        /// Pause the torrent
        /// </summary>
        public void Pause()
        {
            ParentCollection.ParentClient.TorrentPause(this);
        }

        /// <summary>
        /// Un-pause the torrent
        /// </summary>
        public void UnPause()
        {
            ParentCollection.ParentClient.TorrentUnPause(this);
        }

        /// <summary>
        /// Force start the torrent
        /// </summary>
        public void ForceStart()
        {
            ParentCollection.ParentClient.TorrentForceStart(this);
        }

        /// <summary>
        /// Re-check the torrent
        /// </summary>
        public void ReCheck()
        {
            ParentCollection.ParentClient.TorrentReCheck(this);
        }

        /// <summary>
        /// Remove the torrent
        /// </summary>
        /// <param name="RemoveData">Whether or not to remove the data downloaded</param>
        public void Remove(bool RemoveData)
        {
            ParentCollection.ParentClient.TorrentRemove(this, RemoveData);
        }

        /// <summary>
        /// Remove the torrent, keeping the data
        /// </summary>
        public void Remove()
        {
            Remove(false);
        }

        #endregion

        #region Extended Torrent Properties

        #region Private Fields

        private long _maximumUploadBytesPerSecond;
        private long _maximumDownloadBytesPerSecond;
        private long _maximumSeedTime;
        private int _maximumSeedRatio;
        private int _uploadSlots;
        private int _initialSeeding;
        private int _useDistributedHashTable;
        private int _usePeerExchange;
        private int _overrideQueueing;
        private TrackerCollection _trackers;

        #endregion

        #region Public Properties

        /// <summary>
        /// The maximum speed of upload in bytes per second.
        /// Note: Accessing this property will call the uTorrent web interface to update the list of properties for this torrent if it hasn't been updated recently.
        /// Note: Setting the value of this property will update the uTorrent web interface with the new value of the property and then update the list of properties for this torrent.
        /// </summary>
        public long MaximumUploadBytesPerSecond
        {
            get
            {
                UpdateProperties();
                return _maximumUploadBytesPerSecond;
            }
            set
            {
                ParentCollection.ParentClient.SetTorrentProperty(this, "ulrate", value);
                UpdateProperties(true);
            }
        }

        /// <summary>
        /// The maximum speed of download in bytes per second.
        /// Note: Accessing this property will call the uTorrent web interface to update the list of properties for this torrent if it hasn't been updated recently.
        /// Note: Setting the value of this property will update the uTorrent web interface with the new value of the property and then update the list of properties for this torrent.
        /// </summary>
        public long MaximumDownloadBytesPerSecond
        {
            get
            {
                UpdateProperties();
                return _maximumDownloadBytesPerSecond;
            }
            set
            {
                ParentCollection.ParentClient.SetTorrentProperty(this, "dlrate", value);
                UpdateProperties(true);
            }
        }

        /// <summary>
        /// The maximum time to seed the torrent for (must be used in conjunction with the option 'OverrideQueueing' to have any effect).
        /// Note: Accessing this property will call the uTorrent web interface to update the list of properties for this torrent if it hasn't been updated recently.
        /// Note: Setting the value of this property will update the uTorrent web interface with the new value of the property and then update the list of properties for this torrent.
        /// </summary>
        public TimeSpan MaximumSeedTime
        {
            get
            {
                UpdateProperties();
                if (_maximumSeedTime == 0)
                {
                    return TimeSpan.MaxValue;
                }
                else
                {
                    return TimeSpan.FromSeconds(_maximumSeedTime);
                }
            }
            set
            {
                if (value == TimeSpan.MaxValue)
                {
                    ParentCollection.ParentClient.SetTorrentProperty(this, "seed_time", 0);
                }
                else
                {
                    ParentCollection.ParentClient.SetTorrentProperty(this, "seed_time", Convert.ToInt64(value.TotalSeconds));
                }
                UpdateProperties(true);
            }
        }

        /// <summary>
        /// The maximum seed ratio for the torrent (must be used in conjunction with the option 'OverrideQueueing' to have any effect).
        /// Note: Accessing this property will call the uTorrent web interface to update the list of properties for this torrent if it hasn't been updated recently.
        /// Note: Setting the value of this property will update the uTorrent web interface with the new value of the property and then update the list of properties for this torrent.
        /// </summary>
        public decimal MaximumSeedRatio
        {
            get
            {
                UpdateProperties();
                return Decimal.Divide(_maximumSeedRatio, 1000);
            }
            set
            {
                ParentCollection.ParentClient.SetTorrentProperty(this, "seed_ratio", Convert.ToInt32(Decimal.Multiply(value, 1000)));
                UpdateProperties(true);
            }
        }

        /// <summary>
        /// The number of upload slots for the torrent.
        /// Note: Accessing this property will call the uTorrent web interface to update the list of properties for this torrent if it hasn't been updated recently.
        /// Note: Setting the value of this property will update the uTorrent web interface with the new value of the property and then update the list of properties for this torrent.
        /// </summary>
        public int UploadSlots
        {
            get
            {
                UpdateProperties();
                return _uploadSlots;
            }
            set
            {
                ParentCollection.ParentClient.SetTorrentProperty(this, "ulslots", value);
                UpdateProperties(true);
            }
        }

        /// <summary>
        /// Whether or not the torrent is in 'Initial Seeding' mode.
        /// Note: Accessing this property will call the uTorrent web interface to update the list of properties for this torrent if it hasn't been updated recently.
        /// Note: Setting the value of this property will update the uTorrent web interface with the new value of the property and then update the list of properties for this torrent.
        /// </summary>
        public OptionValue InitialSeeding
        {
            get
            {
                UpdateProperties();
                return (OptionValue)_initialSeeding;
            }
            set
            {
                ParentCollection.ParentClient.SetTorrentProperty(this, "superseed", (int)value);
                UpdateProperties(true);
            }
        }

        /// <summary>
        /// Whether or not to use Distributed Hash Table (DHT) for this torrent.
        /// Note: Accessing this property will call the uTorrent web interface to update the list of properties for this torrent if it hasn't been updated recently.
        /// Note: Setting the value of this property will update the uTorrent web interface with the new value of the property and then update the list of properties for this torrent.
        /// </summary>
        public OptionValue UseDistributedHashTable
        {
            get
            {
                UpdateProperties();
                return (OptionValue)_useDistributedHashTable;
            }
            set
            {
                ParentCollection.ParentClient.SetTorrentProperty(this, "dht", (int)value);
                UpdateProperties(true);
            }
        }

        /// <summary>
        /// Whether or not to use peer exchange for this torrent.
        /// Note: Accessing this property will call the uTorrent web interface to update the list of properties for this torrent if it hasn't been updated recently.
        /// Note: Setting the value of this property will update the uTorrent web interface with the new value of the property and then update the list of properties for this torrent.
        /// </summary>
        public OptionValue UsePeerExchange
        {
            get
            {
                UpdateProperties();
                return (OptionValue)_usePeerExchange;
            }
            set
            {
                ParentCollection.ParentClient.SetTorrentProperty(this, "pex", (int)value);
                UpdateProperties(true);
            }
        }

        /// <summary>
        /// Whether or not to override the queueing settings for this torrent (use in conjunction with 'MaximumSeedTime' and 'MaximumSeedRatio' properties).
        /// Note: Accessing this property will call the uTorrent web interface to update the list of properties for this torrent if it hasn't been updated recently.
        /// Note: Setting the value of this property will update the uTorrent web interface with the new value of the property and then update the list of properties for this torrent.
        /// </summary>
        public OptionValue OverrideQueueing
        {
            get
            {
                UpdateProperties();
                return (OptionValue)_overrideQueueing;
            }
            set
            {
                ParentCollection.ParentClient.SetTorrentProperty(this, "seed_override", (int)value);
                UpdateProperties(true);
            }
        }

        /// <summary>
        /// The trackers for this torrent.
        /// Note: Accessing this property will call the uTorrent web interface to update the list of properties for this torrent if it hasn't been updated recently.
        /// Note: Setting the value of this property will update the uTorrent web interface with the new value of the property and then update the list of properties for this torrent.
        /// </summary>
        public TrackerCollection Trackers
        {
            get
            {
                UpdateProperties();
                return _trackers;
            }
            set
            {
                ParentCollection.ParentClient.SetTorrentProperty(this, "trackers", value.ToString());
                UpdateProperties(true);
            }
        }

        #endregion

        #region Helper Methods

        private void UpdateProperties()
        {
            UpdateProperties(false);
        }

        private void UpdateProperties(bool ForceUpdate)
        {
            ParentCollection.ParentClient.UpdateTorrentProperties(this, ForceUpdate);
        }

        internal void ParseProperties(ServiceDefinition.TorrentPropertiesList ListToParse)
        {
            _maximumUploadBytesPerSecond = ListToParse.MaximumUploadRate;
            _maximumDownloadBytesPerSecond = ListToParse.MaximumDownloadRate;
            _initialSeeding = ListToParse.InitialSeeding;
            _useDistributedHashTable = ListToParse.UseDht;
            _usePeerExchange = ListToParse.UsePeerExchange;
            _overrideQueueing = ListToParse.OverrideQueueing;
            _maximumSeedRatio = ListToParse.SeedRatio;
            _maximumSeedTime = ListToParse.SeedTime;
            _uploadSlots = ListToParse.UploadSlots;
            if (_trackers == null)
            {
                _trackers = new TrackerCollection(this);
            }
            _trackers.ParseTrackers(ListToParse.Trackers);
        }

        internal void SaveTrackers()
        {
            ParentCollection.ParentClient.SetTorrentProperty(this, "trackers", _trackers.ToString());
            UpdateProperties(true);
        }

        #endregion

        #endregion
    }

    #region Enums

    /// <summary>
    /// The status of a torrent, a torrent can be in multiple states at one time, for example (TorrentStatus.Started | TorrentStatus.Loaded)
    /// </summary>
    [Flags]
    public enum TorrentStatus
    {
        /// <summary>
        /// The torrent has started
        /// </summary>
        Started = 0x01,
        /// <summary>
        /// The torrent is being checked
        /// </summary>
        Checking = 0x02,
        /// <summary>
        /// The torrent will start after the check is complete
        /// </summary>
        StartAfterCheck = 0x04,
        /// <summary>
        /// The torrent has been checked
        /// </summary>
        Checked = 0x08,
        /// <summary>
        /// There is an error with the torrent
        /// </summary>
        Error = 0x10,
        /// <summary>
        /// The torrent is paused
        /// </summary>
        Paused = 0x20,
        /// <summary>
        /// The torrent is queued
        /// </summary>
        Queued = 0x40,
        /// <summary>
        /// The torrent is loaded
        /// </summary>
        Loaded = 0x80
    }

    /// <summary>
    /// The status category of a torrent, a torrent can be in multiple status categories at one time, for example (TorrentStatusCategory.All | TorrentStatusCategory.Completed)
    /// </summary>
    [Flags]
    public enum TorrentStatusCategory
    {
        /// <summary>
        /// The torrent exists in uTorrent
        /// </summary>
        All = 0x01,
        /// <summary>
        /// The torrent is currently downloading or waiting to download
        /// </summary>
        Downloading = 0x02,
        /// <summary>
        /// The torrent has finished downloading
        /// </summary>
        Completed = 0x04,
        /// <summary>
        /// The torrent is currently transferring data
        /// </summary>
        Active = 0x08,
        /// <summary>
        /// The torrent is currently not transferring data
        /// </summary>
        Inactive = 0x10
    }

    /// <summary>
    /// The status of a torrent to be represented simply so there is only one status
    /// </summary>
    public enum TorrentSimpleStatus
    {
        DownloadingQueued,
        Downloading,
        DownloadingError,
        InactiveIncomplete,
        SeedingQueued,
        Seeding,
        SeedingError,
        InactiveCompleted,
        Paused,
        Error
    }

    /// <summary>
    /// The value of a torrent property
    /// </summary>
    public enum OptionValue : int
    {
        /// <summary>
        /// This value is not applicable to the torrent, and cannot be changed
        /// </summary>
        NotAllowed = -1,
        /// <summary>
        /// This value is negative (i.e. 'no', 'false' or 'disabled')
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// This value is positive (i.e. 'yes', 'true' or 'enabled')
        /// </summary>
        Enabled = 1
    }

    #endregion
}
