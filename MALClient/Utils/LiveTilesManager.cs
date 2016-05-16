using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using MALClient.Comm;
using MALClient.Items;
using MALClient.Models;
using MALClient.ViewModels;

namespace MALClient
{
    public enum TileActions
    {
        OpenUrl,
        OpenDetails,
    }

    public class PinTileSettings
    {
        public bool AddTitle { get; set; } = true;
        public bool AddScore { get; set; } = true;
        public bool AddStatus { get; set; } = true;
        public bool AddImage { get; set; } = true;
        public bool AddAirDay { get; set; } = true;
        public bool AddWatched { get; set; } = true;
        public bool BigTitle { get; set; } = false;
    }

    public class PinTileActionSetting
    {
        public TileActions Action { get; set; }
        public string Param { get; set; }
    }

    public static class LiveTilesManager
    {
        public static void RegisterTile(string id)
        {
            var tiles = (string)ApplicationData.Current.LocalSettings.Values["tiles"];
            if (string.IsNullOrWhiteSpace(tiles))
                tiles = "";
            tiles += id + ";";
            ApplicationData.Current.LocalSettings.Values["tiles"] = tiles;
        }

        public static async void CheckTiles()
        {
            var tiles = (string)ApplicationData.Current.LocalSettings.Values["tiles"];
            if (string.IsNullOrWhiteSpace(tiles))
                return;


            var newTiles = "";
            foreach (var tileId in tiles.Split(';'))
            {
                if (!SecondaryTile.Exists(tileId))
                {
                    try
                    {
                        var file = await ApplicationData.Current.LocalFolder.GetFileAsync($"{tileId}.png");
                        await file.DeleteAsync();
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                else
                {
                    newTiles += tileId + ";";
                }
            }
            ApplicationData.Current.LocalSettings.Values["tiles"] = newTiles;
        }

        public static async Task PinTile(IAnimeData entry, Uri imgUri, Uri wideImgUri,PinTileSettings settings,PinTileActionSetting action)
        {
            try
            {
                if(action.Action == TileActions.OpenUrl)
                if (!action.Param.Contains("http"))
                    action.Param = "http://" + action.Param;
                var tile = new SecondaryTile($"{entry.Id}", $"{entry.Title}", string.Join(";",new string[] { action.Action.ToString(), action.Param}), imgUri,
                    TileSize.Square150x150);
                tile.WideLogo = wideImgUri;
                RegisterTile(entry.Id.ToString());
                await tile.RequestCreateAsync();

                //scaryy...
                StringBuilder tileXmlString =  new StringBuilder();
                tileXmlString.Append("<tile>");
                tileXmlString.Append("<visual version='2'>");
                tileXmlString.Append("<binding template = 'TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>");
                if(settings.AddImage) tileXmlString.Append( $"<image id=\"1\" src=\"{imgUri}\" alt=\"alt text\"/>");
                if (settings.AddTitle) tileXmlString.Append( $"<text hint-style=\"base\" hint-wrap=\"true\" hint-maxLines=\"{(settings.BigTitle ? "2" : "1")}\" id=\"1\">{entry.Title}</text>");
                if (settings.AddStatus) tileXmlString.Append( $"<text hint-wrap=\"false\" id=\"2\">{(AnimeStatus)entry.MyStatus}</text>");
                if (settings.AddScore) tileXmlString.Append( $"<text id=\"3\">{(entry.MyScore == 0 ? "Unranked" : entry.MyScore + $"/{(Settings.SelectedApiType == ApiType.Mal ? "10" : "5")}")}{(settings.AddWatched ? " - " + entry.MyEpisodes + $"/{(entry.AllEpisodes == 0 ? "?" : entry.AllEpisodes.ToString())}" : "")}</text>");
                if (settings.AddAirDay && entry is AnimeItemViewModel && ((AnimeItemViewModel) entry).AirDayBind != "") tileXmlString.Append( $"<text id=\"4\">{((AnimeItemViewModel) entry).AirDayBind}</text>");
                tileXmlString.Append( "</binding>");
                tileXmlString.Append( "<binding template='TileWide310x150ImageAndText02' fallback='TileWideImageAndText02'>");
                if (settings.AddImage) tileXmlString.Append( $"<image id=\"1\" src=\"{wideImgUri}\" alt=\"alt text\"/>");
                if (settings.AddTitle) tileXmlString.Append( $"<text hint-style=\"base\" hint-maxLines=\"{(settings.BigTitle ? "2" : "1")}\" id=\"1\">{entry.Title}</text>");
                tileXmlString.Append("<text id=\"2\">");
                if (settings.AddStatus) tileXmlString.Append( $"{(AnimeStatus)entry.MyStatus}{(settings.AddWatched ? " - " + entry.MyEpisodes + $"/{(entry.AllEpisodes == 0 ? "?" : entry.AllEpisodes.ToString())}" : "")}");
                if (settings.AddScore) tileXmlString.Append( $"\n{(entry.MyScore == 0 ? "Unranked" : entry.MyScore + $"/{(Settings.SelectedApiType == ApiType.Mal ? "10" : "5")}")}");
                if (settings.AddAirDay && entry is AnimeItemViewModel && ((AnimeItemViewModel)entry).AirDayBind != "") tileXmlString.Append($"   -   {((AnimeItemViewModel)entry).AirDayBind}");
                tileXmlString.Append("</text>");
                tileXmlString.Append( "</binding>");
                tileXmlString.Append( "</visual>");
                tileXmlString.Append("</tile>");
                //uff, yup... that was scarry mess

                var mgr = TileUpdateManager.CreateTileUpdaterForSecondaryTile($"{entry.Id}");
                var notif = new Windows.Data.Xml.Dom.XmlDocument();
                notif.LoadXml(tileXmlString.ToString());
                mgr.Update(new TileNotification(notif));
            }
            catch (Exception e)
            {
                //TODO : feedback
            }
        }

    }
}
