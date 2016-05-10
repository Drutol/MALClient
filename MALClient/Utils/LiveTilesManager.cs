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
using MALClient.Models;

namespace MALClient
{
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

        public static async Task PinTile(string targetUrl, ILibraryData entry, string imgUrl, string title)
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var thumb = await folder.CreateFileAsync($"{entry.Id}.png", CreationCollisionOption.ReplaceExisting);

                var http = new HttpClient();
                var response = await http.GetByteArrayAsync(imgUrl); //get bytes

                var fs = await thumb.OpenStreamForWriteAsync(); //get stream

                using (var writer = new DataWriter(fs.AsOutputStream()))
                {
                    writer.WriteBytes(response); //write
                    await writer.StoreAsync();
                    await writer.FlushAsync();
                }
                await Task.Delay(1000);

                if (!targetUrl.Contains("http"))
                    targetUrl = "http://" + targetUrl;
                var tile = new SecondaryTile($"{entry.Id}", $"{title}", targetUrl, new Uri($"ms-appdata:///local/{entry.Id}.png"),
                    TileSize.Wide310x150);
                tile.WideLogo = new Uri($"ms-appdata:///local/{entry.Id}.png");
                RegisterTile(entry.Id.ToString());
                await tile.RequestCreateAsync();


                string tileXmlString = 
                       "<tile>"
                     + "<visual version='2'>"
                     + "<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>"
                     + $"<image id=\"1\" hint-wrap=\"true\" src=\"{new Uri($"ms-appdata:///local/{entry.Id}.png")}\" alt=\"alt text\"/>"
                     + $"<text hint-style=\"base\" hint-maxLines=\"2\" id=\"1\">{entry.Title}</text>"
                     + $"<text id=\"2\">{entry.MyStatus}</text>"
                     + $"<text id=\"3\">{entry.MyScore}</text>"
                     + "</binding>"
                     + "<binding template='TileWide310x150ImageAndText02' fallback='TileWideImageAndText02'>"
                     + $"<image id=\"1\" src=\"{new Uri($"ms-appdata:///local/{entry.Id}.png")}\" alt=\"alt text\"/>"
                     + $"<text hint-style=\"base\" hint-maxLines=\"2\" id=\"1\">{entry.Title}</text>"
                     + $"<text id=\"2\">{entry.MyStatus}</text>"
                     + $"<text id=\"3\">{entry.MyScore}</text>"
                     + "</binding>"
                     + "</visual>"
                     + "</tile>";

                var mgr = TileUpdateManager.CreateTileUpdaterForSecondaryTile($"{entry.Id}");
                var notif = new Windows.Data.Xml.Dom.XmlDocument();
                notif.LoadXml(tileXmlString);
                mgr.Update(new TileNotification(notif));
                

                //Windows.Data.Xml.Dom.XmlDocument tileDOM = new Windows.Data.Xml.Dom.XmlDocument();
                //tileDOM.LoadXml(tileXmlString);
                //TileNotification tile = new TileNotification(tileDOM);

                //// Send the notification to the secondary tile by creating a secondary tile updater
                //TileUpdateManager.CreateTileUpdaterForSecondaryTile(MainPage.dynamicTileId).Update(tile);

                //StringBuilder xmlTile = new StringBuilder();
                //xmlTile.AppendLine("<tile>");
                //xmlTile.AppendLine("<visual version=\"2\">");
                //xmlTile.AppendLine("<binding template=\"TileSquare150x150PeekImageAndText01\" fallback=\"TileSquarePeekImageAndText01\">");
                //xmlTile.AppendLine($"<image id=\"1\" src=\"{new Uri($"ms-appdata:///local/{entry.Id}.png").AbsolutePath}\" alt=\"alt text\"/>");
                //xmlTile.AppendLine($"<text id=\"1\">{entry.Title}</text>");
                //xmlTile.AppendLine($"<text id=\"2\">{entry.MyStatus}</text>");
                //xmlTile.AppendLine($"<text id=\"3\">{entry.MyEpisodes}</text>");
                //xmlTile.AppendLine($"<text id=\"4\">{entry.MyScore}</text>");
                //xmlTile.AppendLine("</binding>");
                //xmlTile.AppendLine("</visual>");
                //xmlTile.AppendLine("</tile>");

                //Windows.Data.Xml.Dom.XmlDocument xmlDoc = new Windows.Data.Xml.Dom.XmlDocument();
                //xmlDoc.LoadXml(xmlTile.ToString());

                //TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
                //TileNotification notifyTile = new TileNotification(xmlDoc);
                //TileUpdateManager.CreateTileUpdaterForSecondaryTile(entry.Id.ToString()).Update(notifyTile);

                // var xmlTile = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquarePeekImageAndText01);

                //xmlTile.FirstChild.FirstChild.FirstChild.ChildNodes[0]. .SetAttribute("src", new Uri($"ms-appdata:///local/{entry.Id}.png").AbsolutePath);
                //xmlTile.ChildNodes[1].NodeValue = ;
                //xmlTile.ChildNodes[2].NodeValue = ;
                //xmlTile.ChildNodes[3].NodeValue = ;
                //xmlTile.ChildNodes[4].NodeValue = ;





            }
            catch (Exception e)
            {
                //TODO : feedback
            }
        }

    }
}
