using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Cleverscape.UTorrentClient.WebClient.ServiceDefinition
{
    [ServiceContract]
    public interface IUTorrentWebClient
    {
        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?list=1"
            )]
        TorrentsAndLabels GetAllTorrentsAndLabels();

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?list=1&cid={CacheID}"
            )]
        UpdatedTorrentsAndLabels GetUpdatedTorrentsAndLabels(string CacheID);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=getsettings"
            )]
        UTorrentSettings GetSettings();

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=setsetting&s={SettingName}&v={SettingValue}"
            )]
        GenericResponse SetStringSetting(string SettingName, string SettingValue);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=setsetting&s={SettingName}&v={SettingValue}"
            )]
        GenericResponse SetBooleanSetting(string SettingName, string SettingValue);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=setsetting&s={SettingName}&v={SettingValue}"
            )]
        GenericResponse SetIntegerSetting(string SettingName, int SettingValue);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=getfiles&hash={TorrentHash}"
            )]
        TorrentFiles GetFiles(string TorrentHash);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=getprops&hash={TorrentHash}"
            )]
        TorrentProperties GetProperties(string TorrentHash);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=setprops&hash={TorrentHash}&s={PropertyName}&v={PropertyValue}"
            )]
        GenericResponse SetStringProperty(string TorrentHash, string PropertyName, string PropertyValue);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=setprops&hash={TorrentHash}&s={PropertyName}&v={PropertyValue}"
            )]
        GenericResponse SetIntegerProperty(string TorrentHash, string PropertyName, int PropertyValue);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=setprops&hash={TorrentHash}&s={PropertyName}&v={PropertyValue}"
            )]
        GenericResponse SetLongProperty(string TorrentHash, string PropertyName, long PropertyValue);


        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=start&hash={TorrentHash}"
            )]
        GenericResponse StartTorrent(string TorrentHash);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=stop&hash={TorrentHash}"
            )]
        GenericResponse StopTorrent(string TorrentHash);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=pause&hash={TorrentHash}"
            )]
        GenericResponse PauseTorrent(string TorrentHash);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=forcestart&hash={TorrentHash}"
            )]
        GenericResponse ForceStartTorrent(string TorrentHash);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=unpause&hash={TorrentHash}"
            )]
        GenericResponse UnPauseTorrent(string TorrentHash);
        
        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=recheck&hash={TorrentHash}"
            )]
        GenericResponse RecheckTorrent(string TorrentHash);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=remove&hash={TorrentHash}"
            )]
        GenericResponse RemoveTorrent(string TorrentHash);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=removedata&hash={TorrentHash}"
            )]
        GenericResponse RemoveTorrentAndData(string TorrentHash);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=setprio&hash={TorrentHash}&p={Priority}&f={FileNumber}"
            )]
        GenericResponse SetFilePriority(string TorrentHash, int FileNumber, int Priority);

        [OperationContract]
        [WebGet(
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/?action=add-url&s={TorrentUrl}"
            )]
        GenericResponse AddTorrentFromUrl(string TorrentUrl);

    }

}
