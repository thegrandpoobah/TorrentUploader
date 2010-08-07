using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using Cleverscape.UTorrentClient.WebClient.ServiceDefinition;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Timers;
using System.Windows.Threading;

namespace Cleverscape.UTorrentClient.WebClient
{
    /// <summary>
    /// Represents an instance of uTorrent.
    /// Uses the Web UI API to connect to uTorrent to retrieve and update information.
    /// </summary>
    public class UTorrentWebClient : IDisposable
    {
        #region Private Fields

        private bool _autoUpdate;
        private System.Windows.Forms.Timer _autoUpdateTimerSync;
        private System.Timers.Timer _autoUpdateTimerAsync;
        private bool _autoUpdateAsynchronous;
        private Dispatcher _dispatcher;
        private TimeSpan _minimumTimeBetweenUpdates;

        private IUTorrentWebClient ServiceClient;
        private WebChannelFactory<IUTorrentWebClient> ChannelFactory;

        private string _uTorrentAddress;
        private string _uTorrentUserName;
        private string _uTorrentPassword;

        private long _cacheID;
        private bool _torrentsAndLabelsListStored;
        private TorrentCollection _torrents;
        private TorrentLabelCollection _labels;
        private DateTime _torrentsAndLabelsLastUpdated;

        private SettingsCollection _settings;
        private DateTime _settingsLastUpdated;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new UTorrentWebClient with no auto updating
        /// </summary>
        /// <param name="ServerAddress">The address of the uTorrent Web UI. This can be expressed as a hostname (e.g. utorrent.mynetwork.com) or an IP address (e.g. 10.11.12.13). This can optionally include a preceeding 'http://' and a suffix '/gui/'. If these elements are not included, they will be added.</param>
        /// <param name="UserName">The username used to access the uTorrent Web UI.</param>
        /// <param name="Password">The password used to access the uTorrent Web UI.</param>
        public UTorrentWebClient(string ServerAddress, string UserName, string Password)
            : this(ServerAddress, UserName, Password, false) { }

        /// <summary>
        /// Create a new UTorrentWebClient with optional auto updating
        /// </summary>
        /// <param name="ServerAddress">The address of the uTorrent Web UI. This can be expressed as a hostname (e.g. utorrent.mynetwork.com) or an IP address (e.g. 10.11.12.13). This can optionally include a preceeding 'http://' and a suffix '/gui/'. If these elements are not included, they will be added.</param>
        /// <param name="UserName">The username used to access the uTorrent Web UI.</param>
        /// <param name="Password">The password used to access the uTorrent Web UI.</param>
        /// <param name="AutoUpdate">Whether or not to auto update the UTorrentWebClient object automatically</param>
        public UTorrentWebClient(string ServerAddress, string UserName, string Password, bool AutoUpdate)
            : this(ServerAddress, UserName, Password, AutoUpdate, null) { }

        /// <summary>
        /// Create a new UTorrentWebClient with optional auto updating and optionally passing a System.Windows.Threading.Dispatcher to handle UI events
        /// </summary>
        /// <param name="ServerAddress">The address of the uTorrent Web UI. This can be expressed as a hostname (e.g. utorrent.mynetwork.com) or an IP address (e.g. 10.11.12.13). This can optionally include a preceeding 'http://' and a suffix '/gui/'. If these elements are not included, they will be added.</param>
        /// <param name="UserName">The username used to access the uTorrent Web UI.</param>
        /// <param name="Password">The password used to access the uTorrent Web UI.</param>
        /// <param name="AutoUpdate">Whether or not to auto update the UTorrentWebClient object automatically</param>
        /// <param name="Dispatcher">A System.Windows.Threading.Dispatcher to handle UI events</param>
        public UTorrentWebClient(string ServerAddress, string UserName, string Password, bool AutoUpdate, Dispatcher Dispatcher)
        {
            _autoUpdateAsynchronous = true;
            MinimumTimeBetweenUpdates = TimeSpan.FromSeconds(2);
            _dispatcher = Dispatcher;
            this.AutoUpdate = AutoUpdate;

            SetConnectionDetails(ServerAddress, UserName, Password);
        }

        /// <summary>
        /// Change the connection details used to connect to the uTorrent Web UI
        /// </summary>
        /// <param name="ServerAddress">The address of the uTorrent Web UI. This can be expressed as a hostname (e.g. utorrent.mynetwork.com) or an IP address (e.g. 10.11.12.13). This can optionally include a preceeding 'http://' and a suffix '/gui/'. If these elements are not included, they will be added.</param>
        /// <param name="UserName">The username used to access the uTorrent Web UI.</param>
        /// <param name="Password">The password used to access the uTorrent Web UI.</param>
        public void SetConnectionDetails(string ServerAddress, string UserName, string Password)
        {
            _torrentsAndLabelsListStored = false;

            _uTorrentAddress = ProcessServerAddress(ServerAddress);
            _uTorrentUserName = UserName;
            _uTorrentPassword = Password;

            CustomBinding uTorrentCustomBinding = new CustomBinding(
                new WebMessageEncodingBindingElement() { ContentTypeMapper = new JsonContentTypeMapper() },
                new HttpTransportBindingElement() { ManualAddressing = true, AuthenticationScheme = System.Net.AuthenticationSchemes.Basic, Realm = "uTorrent", AllowCookies = false }
                );
            EndpointAddress uTorrentEndpointAddress = new EndpointAddress(_uTorrentAddress);

            ChannelFactory = new WebChannelFactory<IUTorrentWebClient>(uTorrentCustomBinding);
            ChannelFactory.Endpoint.Address = uTorrentEndpointAddress;
            ChannelFactory.Credentials.UserName.UserName = _uTorrentUserName;
            ChannelFactory.Credentials.UserName.Password = _uTorrentPassword;
            ServiceClient = ChannelFactory.CreateChannel();
        }

        #endregion

        #region Internal Properties

        internal Dispatcher Dispatcher
        {
            get
            {
                return _dispatcher;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Whether or not to automatically update the list of torrents and labels.
        /// If true, the collections of torrents and labels will be updated at intervals of MinimumTimeBetweenUpdates
        /// </summary>
        public bool AutoUpdate
        {
            get
            {
                return _autoUpdate;
            }
            set
            {
                _autoUpdate = value;
                if (_autoUpdate)
                {
                    if (_autoUpdateAsynchronous)
                    {
                        if (_autoUpdateTimerAsync == null)
                        {
                            _autoUpdateTimerAsync = new System.Timers.Timer(MinimumTimeBetweenUpdates.TotalMilliseconds);
                            _autoUpdateTimerAsync.Elapsed += new ElapsedEventHandler(AutoUpdateTimer_Elapsed);

                        }
                        else
                        {
                            _autoUpdateTimerAsync.Interval = MinimumTimeBetweenUpdates.TotalMilliseconds;
                        }
                        _autoUpdateTimerAsync.Start();
                    }
                    else
                    {
                        if (_autoUpdateTimerSync == null)
                        {
                            _autoUpdateTimerSync = new System.Windows.Forms.Timer();
                            _autoUpdateTimerSync.Interval = Convert.ToInt32(MinimumTimeBetweenUpdates.TotalMilliseconds);
                            _autoUpdateTimerSync.Tick += new EventHandler(AutoUpdateTimer_Tick);
                        }
                        else
                        {
                            _autoUpdateTimerSync.Interval = Convert.ToInt32(MinimumTimeBetweenUpdates.TotalMilliseconds);
                        }
                        _autoUpdateTimerSync.Start();
                    }
                }
                else
                {
                    if (_autoUpdateTimerSync != null)
                    {
                        _autoUpdateTimerSync.Stop();
                    }
                    if (_autoUpdateTimerAsync != null)
                    {
                        _autoUpdateTimerAsync.Stop();
                    }
                }
            }
        }

        private void AutoUpdateTimer_Tick(object sender, EventArgs e)
        {
            GetTorrentsAndLabels(true);
        }

        private void AutoUpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            GetTorrentsAndLabels(true);
        }

        /// <summary>
        /// The minimum time between updates or the time between automatic updates.
        /// If information is requested from an object that gets its information from uTorrent, and the information is less than MinimumTimeBetweenUpdates old, then the cached information will be returned.
        /// </summary>
        public TimeSpan MinimumTimeBetweenUpdates
        {
            get
            {
                return _minimumTimeBetweenUpdates;
            }
            set
            {
                _minimumTimeBetweenUpdates = value;
                if (AutoUpdate)
                {
                    if (_autoUpdateTimerSync != null)
                    {
                        _autoUpdateTimerSync.Interval = Convert.ToInt32(_minimumTimeBetweenUpdates.TotalMilliseconds);
                    }
                    if (_autoUpdateTimerAsync != null)
                    {
                        _autoUpdateTimerAsync.Interval = _minimumTimeBetweenUpdates.TotalMilliseconds;
                    }
                }
            }
        }

        /// <summary>
        /// The collection of torrents currently managed by uTorrent
        /// </summary>
        public TorrentCollection Torrents
        {
            get
            {
                GetTorrentsAndLabels();
                return _torrents;
            }
        }

        /// <summary>
        /// The collection of torrent labels currently managed by uTorrent
        /// </summary>
        public TorrentLabelCollection Labels
        {
            get
            {
                GetTorrentsAndLabels();
                return _labels;
            }
        }

        /// <summary>
        /// The collection of uTorrent settings
        /// </summary>
        public SettingsCollection Settings
        {
            get
            {
                UpdateSettings();
                return _settings;
            }
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Starts a torrent
        /// </summary>
        /// <param name="Torrent">The torrent to start</param>
        public void TorrentStart(Torrent Torrent)
        {
            ServiceClient.StartTorrent(Torrent.Hash);
            GetTorrentsAndLabels(true);
        }

        /// <summary>
        /// Stops a torrent
        /// </summary>
        /// <param name="Torrent">The torrent to stop</param>
        public void TorrentStop(Torrent Torrent)
        {
            ServiceClient.StopTorrent(Torrent.Hash);
            GetTorrentsAndLabels(true);
        }

        /// <summary>
        /// Pauses a torrent
        /// </summary>
        /// <param name="Torrent">The torrent to pause</param>
        public void TorrentPause(Torrent Torrent)
        {
            ServiceClient.PauseTorrent(Torrent.Hash);
            GetTorrentsAndLabels(true);
        }

        /// <summary>
        /// Un-pauses a torrent
        /// </summary>
        /// <param name="Torrent">The torrent to un-pause</param>
        public void TorrentUnPause(Torrent Torrent)
        {
            ServiceClient.UnPauseTorrent(Torrent.Hash);
            GetTorrentsAndLabels(true);
        }

        /// <summary>
        /// Force starts a torrent
        /// </summary>
        /// <param name="Torrent">The torrent to force start</param>
        public void TorrentForceStart(Torrent Torrent)
        {
            ServiceClient.ForceStartTorrent(Torrent.Hash);
            GetTorrentsAndLabels(true);
        }

        /// <summary>
        /// Re-checks a torrent
        /// </summary>
        /// <param name="Torrent">The torrent to re-check</param>
        public void TorrentReCheck(Torrent Torrent)
        {
            ServiceClient.RecheckTorrent(Torrent.Hash);
            GetTorrentsAndLabels(true);
        }

        /// <summary>
        /// Removes a torrent from uTorrent
        /// </summary>
        /// <param name="Torrent">The torrent to remove</param>
        /// <param name="RemoveData">Whether or not the data is removed</param>
        public void TorrentRemove(Torrent Torrent, bool RemoveData)
        {
            if (RemoveData)
            {
                ServiceClient.RemoveTorrentAndData(Torrent.Hash);
            }
            else
            {
                ServiceClient.RemoveTorrent(Torrent.Hash);
            }
            GetTorrentsAndLabels(true);
        }

        /// <summary>
        /// Removes a torrent from uTorrent and keeps the data
        /// </summary>
        /// <param name="Torrent">The torrent to remove</param>
        public void TorrentRemove(Torrent Torrent)
        {
            TorrentRemove(Torrent, false);
        }

        /// <summary>
        /// Adds a torrent to uTorrent from a url
        /// </summary>
        /// <param name="Url">The url to download the .torrent file</param>
        public void AddTorrentFromUrl(string Url)
        {
            ServiceClient.AddTorrentFromUrl(Url);
            GetTorrentsAndLabels(true);
        }

        /// <summary>
        /// Adds a torrent to uTorrent from a file
        /// </summary>
        /// <param name="FileName">The name of the file to add</param>
        public void AddTorrent(string FileName)
        {
            AddTorrent(File.OpenRead(FileName), FileName);
        }

        /// <summary>
        /// Adds a torrent to uTorrent from a stream
        /// </summary>
        /// <param name="TorrentStream">The stream containing the torrent to add</param>
        public void AddTorrent(Stream TorrentStream)
        {
            AddTorrent(TorrentStream, "C:\\Torrent.torrent");
        }

        /// <summary>
        /// Adds a torrent to uTorrent from a stream, uploading it with a specified file name
        /// </summary>
        /// <param name="TorrentStream">The stream containing the torrent to add</param>
        /// <param name="FileName">The filename to upload the torrent using</param>
        public void AddTorrent(Stream TorrentStream, string FileName)
        {
            CredentialCache uTorrentCredentials = new CredentialCache();
            uTorrentCredentials.Add(new Uri(_uTorrentAddress), "Basic", new NetworkCredential(_uTorrentUserName, _uTorrentPassword));
            HttpWebRequest PostFileRequest = (HttpWebRequest)(HttpWebRequest.Create(String.Format("{0}?action=add-file", _uTorrentAddress)));
            PostFileRequest.KeepAlive = false;
            PostFileRequest.Credentials = uTorrentCredentials;
            string BoundaryString = Guid.NewGuid().ToString("N");
            PostFileRequest.ContentType = "multipart/form-data; boundary=" + BoundaryString;
            PostFileRequest.Method = "POST";

            using (BinaryWriter Writer = new BinaryWriter(PostFileRequest.GetRequestStream()))
            {
                byte[] FileBytes = new byte[TorrentStream.Length];
                TorrentStream.Read(FileBytes, 0, FileBytes.Length);

                Writer.Write(Encoding.ASCII.GetBytes(String.Format("--{0}\r\nContent-Disposition: form-data; name=\"torrent_file\"; filename=\"{0}\"\r\nContent-Type: application/x-bittorrent\r\n\r\n", BoundaryString, FileName)));
                Writer.Write(FileBytes, 0, FileBytes.Length);
                Writer.Write(Encoding.ASCII.GetBytes(String.Format("\r\n--{0}--\r\n", BoundaryString)));
            }

            HttpWebResponse Response = (HttpWebResponse)PostFileRequest.GetResponse();
            Response.Close();
            PostFileRequest.Abort();
            GetTorrentsAndLabels(true);
        }


        #region IDisposable implementation

        public void Dispose()
        {
            ServiceClient = null;
            ChannelFactory.Close();
            ChannelFactory = null;
        }

        #endregion

        #endregion

        #region Helper Methods

        private string ProcessServerAddress(string ServerAddress)
        {
            string ServerAddressProcessed = ServerAddress.ToLowerInvariant();
            if (!ServerAddressProcessed.StartsWith("http://"))
            {
                ServerAddressProcessed = String.Format("http://{0}", ServerAddressProcessed);
            }
            if (!ServerAddressProcessed.EndsWith("/"))
            {
                ServerAddressProcessed = String.Format("{0}/", ServerAddressProcessed);
            }
            if (!ServerAddressProcessed.EndsWith("/gui/"))
            {
                ServerAddressProcessed = String.Format("{0}gui/", ServerAddressProcessed);
            }
            return ServerAddressProcessed;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Called when a torrent enters the started state
        /// </summary>
        public event TorrentEventHandler TorrentStarted;

        /// <summary>
        /// Called when a torrent enters the stopped state
        /// </summary>
        public event TorrentEventHandler TorrentStopped;

        /// <summary>
        /// Called when a torrent is paused
        /// </summary>
        public event TorrentEventHandler TorrentPaused;

        /// <summary>
        /// Called when a torrent is unpaused
        /// </summary>
        public event TorrentEventHandler TorrentUnPaused;

        /// <summary>
        /// Called when a torrent has finished downloading
        /// </summary>
        public event TorrentEventHandler TorrentFinishedDownloading;

        /// <summary>
        /// Called when a new torrent is added
        /// </summary>
        public event TorrentEventHandler TorrentAdded;

        /// <summary>
        /// Called when the list of torrents is updated
        /// </summary>
        public event EventHandler TorrentsUpdated;

        internal enum EventType
        {
            Started,
            Stopped,
            Paused,
            UnPaused,
            FinishedDownloading,
            Added,
            Updated
        }

        internal void CallEvent(EventType Type, Torrent Torrent)
        {
            CallEvent(Type, Torrent, false);
        }

        internal void CallEvent(EventType Type, Torrent Torrent, bool FromDispatcher)
        {
            if (!FromDispatcher && _dispatcher != null)
            {
                _dispatcher.BeginInvoke(new CallEventDelegate(CallEvent), DispatcherPriority.Loaded, new object[] { Type, Torrent, true });
            }
            else
            {
                switch (Type)
                {
                    case EventType.Started:
                        if (TorrentStarted != null)
                        {
                            TorrentStarted(this, new TorrentEventArgs(Torrent));
                        }
                        break;
                    case EventType.Stopped:
                        if (TorrentStopped != null)
                        {
                            TorrentStopped(this, new TorrentEventArgs(Torrent));
                        }
                        break;
                    case EventType.Paused:
                        if (TorrentPaused != null)
                        {
                            TorrentPaused(this, new TorrentEventArgs(Torrent));
                        }
                        break;
                    case EventType.UnPaused:
                        if (TorrentUnPaused != null)
                        {
                            TorrentUnPaused(this, new TorrentEventArgs(Torrent));
                        }
                        break;
                    case EventType.FinishedDownloading:
                        if (TorrentFinishedDownloading != null)
                        {
                            TorrentFinishedDownloading(this, new TorrentEventArgs(Torrent));
                        }
                        break;
                    case EventType.Added:
                        if (TorrentAdded != null)
                        {
                            TorrentAdded(this, new TorrentEventArgs(Torrent));
                        }
                        break;
                    case EventType.Updated:
                        if (TorrentsUpdated != null)
                        {
                            TorrentsUpdated(this, new EventArgs());
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private delegate void CallEventDelegate(EventType Type, Torrent Torrent, bool FromDispatcher);

        #endregion

        #region Getting Torrents And Labels

        private void GetTorrentsAndLabels()
        {
            GetTorrentsAndLabels(false);
        }

        private void GetTorrentsAndLabels(bool ForceUpdate)
        {
            if (_torrentsAndLabelsLastUpdated == null || _torrentsAndLabelsLastUpdated.Add(MinimumTimeBetweenUpdates) <= DateTime.Now || ForceUpdate)
            {
                if (_torrentsAndLabelsListStored && _cacheID > 0)
                {
                    GetTorrentsAndLabelsCached();
                }
                else
                {
                    if (_torrentsAndLabelsListStored)
                    {
                        GetTorrentsAndLabelsUpdate();
                    }
                    else
                    {
                        GetTorrentsAndLabelsFresh();
                    }
                }
                CallEvent(EventType.Updated, null);
            }
        }

        private void GetTorrentsAndLabelsCached()
        {
            UpdatedTorrentsAndLabels UpdatedTorrents = ServiceClient.GetUpdatedTorrentsAndLabels(_cacheID.ToString());

            _torrents.Parse(UpdatedTorrents.Torrents, UpdatedTorrents.RemovedTorrents, UpdatedTorrents.ChangedTorrents);
            _labels.Parse(UpdatedTorrents.Labels);
            SetCache(UpdatedTorrents.CacheID);
        }

        private void GetTorrentsAndLabelsUpdate()
        {
            GetTorrentsAndLabelsUpdate(null);
        }

        private void GetTorrentsAndLabelsUpdate(TorrentsList TorrentsToProcess)
        {
            TorrentsAndLabels CurrentTorrents = ServiceClient.GetAllTorrentsAndLabels();

            _torrents.Parse(CurrentTorrents.Torrents, false);
            _labels.Parse(CurrentTorrents.Labels);
            SetCache(CurrentTorrents.CacheID);
        }

        private void GetTorrentsAndLabelsFresh()
        {
            TorrentsAndLabels CurrentTorrents = ServiceClient.GetAllTorrentsAndLabels();

            _torrents = new TorrentCollection(this);
            _torrents.Parse(CurrentTorrents.Torrents, true);

            _labels = new TorrentLabelCollection();
            _labels.Parse(CurrentTorrents.Labels);

            SetCache(CurrentTorrents.CacheID);
        }

        private void SetCache(string CacheIDString)
        {
            long ParsedCacheID = 0;
            if (long.TryParse(CacheIDString, out ParsedCacheID) && ParsedCacheID > 0)
            {
                _cacheID = ParsedCacheID;
            }
            else
            {
                _cacheID = 0;
            }
            _torrentsAndLabelsListStored = true;
            _torrentsAndLabelsLastUpdated = DateTime.Now;
        }

        #endregion

        #region Getting Settings

        private void UpdateSettings()
        {
            UpdateSettings(false);
        }

        private void UpdateSettings(bool ForceUpdate)
        {
            if (_settingsLastUpdated == null || _settingsLastUpdated.Add(MinimumTimeBetweenUpdates) < DateTime.Now || ForceUpdate)
            {
                UTorrentSettings CurrentSettings = ServiceClient.GetSettings();
                if (_settings == null)
                {
                    _settings = new SettingsCollection();
                }
                _settings.ParseSettings(CurrentSettings.Settings, this);
                _settingsLastUpdated = DateTime.Now;
            }
        }

        internal void SetSetting(SettingBase Setting)
        {
            switch (Setting.Type)
            {
                case SettingType.Integer:
                    ServiceClient.SetIntegerSetting(Setting.Name, ((SettingInteger)Setting).Value);
                    break;
                case SettingType.String:
                    ServiceClient.SetStringSetting(Setting.Name, ((SettingString)Setting).Value);
                    break;
                case SettingType.Boolean:
                    ServiceClient.SetBooleanSetting(Setting.Name, ((SettingBoolean)Setting).Value ? "true" : "false");
                    break;
                default:
                    break;
            }
            UpdateSettings(true);
        }

        #endregion

        #region Getting And Setting Torrent Files

        internal void UpdateTorrentFiles(Torrent TorrentToUpdate, bool ForceUpdate)
        {
            if (TorrentToUpdate.FilesLastUpdated == null || TorrentToUpdate.FilesLastUpdated.Add(MinimumTimeBetweenUpdates) < DateTime.Now || ForceUpdate)
            {
                if (TorrentToUpdate.FileList == null)
                {
                    TorrentToUpdate.FileList = new TorrentFileCollection(TorrentToUpdate);
                }
                TorrentToUpdate.FileList.ParseFiles(ServiceClient.GetFiles(TorrentToUpdate.Hash).Files, TorrentToUpdate);
                TorrentToUpdate.FilesLastUpdated = DateTime.Now;
            }
        }

        internal void UpdateTorrentFiles(Torrent TorrentToUpdate)
        {
            UpdateTorrentFiles(TorrentToUpdate, false);
        }

        internal void SetTorrentFilePriority(TorrentFile FileToUpdate, int Priority)
        {
            ServiceClient.SetFilePriority(
                FileToUpdate.ParentCollection.ParentTorrent.Hash,
                FileToUpdate.Index,
                Priority);
            UpdateTorrentFiles(FileToUpdate.ParentCollection.ParentTorrent, true);
        }

        #endregion

        #region Getting And Setting Torrent Properties

        internal void UpdateTorrentProperties(Torrent TorrentToUpdate, bool ForceUpdate)
        {
            if (TorrentToUpdate.PropertiesLastUpdated == null || TorrentToUpdate.PropertiesLastUpdated.Add(MinimumTimeBetweenUpdates) < DateTime.Now || ForceUpdate)
            {
                TorrentPropertiesList[] CurrentProperties = ServiceClient.GetProperties(TorrentToUpdate.Hash).Properties;
                if (CurrentProperties.Length != 1)
                {
                    throw new FormatException("The array of torrent properties was not in the expected format.");
                }
                TorrentToUpdate.ParseProperties(CurrentProperties[0]);
                TorrentToUpdate.PropertiesLastUpdated = DateTime.Now;
            }
        }

        internal void UpdateTorrentProperties(Torrent TorrentToUpdate)
        {
            UpdateTorrentProperties(TorrentToUpdate, false);
        }

        internal void SetTorrentProperty(Torrent TorrentToUpdate, string PropertyName, int PropertyValue)
        {
            ServiceClient.SetIntegerProperty(TorrentToUpdate.Hash, PropertyName, PropertyValue);
        }

        internal void SetTorrentProperty(Torrent TorrentToUpdate, string PropertyName, long PropertyValue)
        {
            ServiceClient.SetLongProperty(TorrentToUpdate.Hash, PropertyName, PropertyValue);
        }

        internal void SetTorrentProperty(Torrent TorrentToUpdate, string PropertyName, string PropertyValue)
        {
            ServiceClient.SetStringProperty(TorrentToUpdate.Hash, PropertyName, PropertyValue);
        }

        #endregion
    }

    #region Torrent Events

    /// <summary>
    /// Event arguments for a torrent event
    /// </summary>
    public class TorrentEventArgs : EventArgs
    {
        private Torrent _torrent;

        /// <summary>
        /// The torrent to which the event is referring
        /// </summary>
        public Torrent Torrent
        {
            get
            {
                return _torrent;
            }
        }

        internal TorrentEventArgs(Torrent Torrent)
            : base()
        {
            _torrent = Torrent;
        }
    }

    public delegate void TorrentEventHandler(object sender, TorrentEventArgs e);

    #endregion
}
