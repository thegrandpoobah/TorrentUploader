using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;

namespace Cleverscape.UTorrentClient.WebClient.ServiceDefinition
{
    [ServiceContract]
    public interface IUTorrentWebClient
    {
        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&list=1"
            )]
        TorrentsAndLabels GetAllTorrentsAndLabels(string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&list=1&cid={CacheID}"
            )]
        UpdatedTorrentsAndLabels GetUpdatedTorrentsAndLabels(string CacheID, string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=getsettings"
            )]
        UTorrentSettings GetSettings(string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=setsetting&s={SettingName}&v={SettingValue}"
            )]
        GenericResponse SetStringSetting(string SettingName, string SettingValue, string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=setsetting&s={SettingName}&v={SettingValue}"
            )]
        GenericResponse SetBooleanSetting(string SettingName, string SettingValue, string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=setsetting&s={SettingName}&v={SettingValue}"
            )]
        GenericResponse SetIntegerSetting(string SettingName, int SettingValue, string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=getfiles&hash={TorrentHash}"
            )]
        TorrentFiles GetFiles(string TorrentHash, string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=getprops&hash={TorrentHash}"
            )]
        TorrentProperties GetProperties(string TorrentHash, string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=setprops&hash={TorrentHash}&s={PropertyName}&v={PropertyValue}"
            )]
        GenericResponse SetStringProperty(string TorrentHash, string PropertyName, string PropertyValue, string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=setprops&hash={TorrentHash}&s={PropertyName}&v={PropertyValue}"
            )]
        GenericResponse SetIntegerProperty(string TorrentHash, string PropertyName, int PropertyValue, string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=setprops&hash={TorrentHash}&s={PropertyName}&v={PropertyValue}"
            )]
        GenericResponse SetLongProperty(string TorrentHash, string PropertyName, long PropertyValue, string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=start&hash={TorrentHash}"
            )]
        GenericResponse StartTorrent(string TorrentHash, string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=stop&hash={TorrentHash}"
            )]
        GenericResponse StopTorrent(string TorrentHash, string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=pause&hash={TorrentHash}"
            )]
        GenericResponse PauseTorrent(string TorrentHash, string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=forcestart&hash={TorrentHash}"
            )]
        GenericResponse ForceStartTorrent(string TorrentHash, string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=unpause&hash={TorrentHash}"
            )]
        GenericResponse UnPauseTorrent(string TorrentHash, string token);
        
        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=recheck&hash={TorrentHash}"
            )]
        GenericResponse RecheckTorrent(string TorrentHash, string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=remove&hash={TorrentHash}"
            )]
        GenericResponse RemoveTorrent(string TorrentHash, string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=removedata&hash={TorrentHash}"
            )]
        GenericResponse RemoveTorrentAndData(string TorrentHash, string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=setprio&hash={TorrentHash}&p={Priority}&f={FileNumber}"
            )]
        GenericResponse SetFilePriority(string TorrentHash, int FileNumber, int Priority, string token);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?token={token}&action=add-url&s={TorrentUrl}"
            )]
        GenericResponse AddTorrentFromUrl(string TorrentUrl, string token);

        [OperationContract]
        [WebGet(
			BodyStyle = WebMessageBodyStyle.Bare,
			UriTemplate = "/token.html"
			)]
        Stream getToken();
    }
}
