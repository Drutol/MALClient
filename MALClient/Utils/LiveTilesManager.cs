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
using Newtonsoft.Json;

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
        public bool AddBranding { get; set; } = true;
        public bool BigTitle { get; set; }
        public bool AnythingAtAll => AddTitle || AddScore || AddStatus || AddAirDay || AddWatched;
    }

    public class PinTileActionSetting
    {
        public TileActions Action { get; set; }
        public string Param { get; set; }
    }

    public class PinnedTileCache
    {
        public PinTileSettings Settings { get; set; }
        public Uri ImgUri { get; set; }
        public Uri WideImgUri { get; set; }
    }

    public static class LiveTilesManager
    {
        private static Dictionary<int,PinnedTileCache> _pinnedCache = new Dictionary<int, PinnedTileCache>();

        public static async void LoadTileCache()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync("pinned_tiles.json");
                var data = await FileIO.ReadTextAsync(file);
                _pinnedCache = JsonConvert.DeserializeObject<Dictionary<int, PinnedTileCache>>(data) ??
                                     new Dictionary<int, PinnedTileCache>();
            }
            catch (Exception)
            {
                _pinnedCache = new Dictionary<int, PinnedTileCache>();
            }
            CheckTiles();
        }

        public static async Task SavePinnedData()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_pinnedCache);
                var file =
                    await
                        ApplicationData.Current.LocalFolder.CreateFileAsync("pinned_tiles.json",
                            CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, json);
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private static void RegisterTileCache(int id,PinnedTileCache cache)
        {
            if (_pinnedCache.ContainsKey(id))
                _pinnedCache[id] = cache;
            else
                _pinnedCache.Add(id, cache);
            SavePinnedData();
        }

        private static void RegisterTile(string id)
        {
            var tiles = (string)ApplicationData.Current.LocalSettings.Values["tiles"];
            if (string.IsNullOrWhiteSpace(tiles))
                tiles = "";
            tiles += id + ";";
            ApplicationData.Current.LocalSettings.Values["tiles"] = tiles;
        }

        private static async void CheckTiles()
        {
            var tiles = (string)ApplicationData.Current.LocalSettings.Values["tiles"];
            if (string.IsNullOrWhiteSpace(tiles))
                return;

            bool removed = false;
            var newTiles = "";
            //var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("PinnedTilesImages",
            //    CreationCollisionOption.OpenIfExists);
            foreach (var tileId in tiles.Split(';'))
            {
                if (!SecondaryTile.Exists(tileId))
                {
                    try
                    {
                        int id = int.Parse(tileId);
                        if (_pinnedCache.ContainsKey(id))
                        {
                            var cache = _pinnedCache[id];
                            if (cache.ImgUri.Equals(cache.WideImgUri)) //the same image
                                await (await StorageFile.GetFileFromApplicationUriAsync(cache.ImgUri)).DeleteAsync(
                                    StorageDeleteOption.PermanentDelete);
                            else //2 images
                            {
                                await (await StorageFile.GetFileFromApplicationUriAsync(cache.ImgUri)).DeleteAsync(
                                    StorageDeleteOption.PermanentDelete);
                                await (await StorageFile.GetFileFromApplicationUriAsync(cache.WideImgUri)).DeleteAsync(
                                    StorageDeleteOption.PermanentDelete);
                            }
                            removed = true;
                            _pinnedCache.Remove(id);
                        }
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
            if (removed)
                SavePinnedData();
            ApplicationData.Current.LocalSettings.Values["tiles"] = newTiles;
        }

        public static async Task PinTile(IAnimeData entry, Uri imgUri, Uri wideImgUri,PinTileSettings settings,PinTileActionSetting action)
        {
            try
            {
                //prepare images
                if (imgUri != null)
                {
                    var file = await StorageFile.GetFileFromApplicationUriAsync(imgUri);
                    var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("PinnedTilesImages",
                        CreationCollisionOption.OpenIfExists);
                    await file.CopyAsync(folder, entry.Id + ".png", NameCollisionOption.ReplaceExisting);

                    if (!imgUri.Equals(wideImgUri))
                    {
                        file = await StorageFile.GetFileFromApplicationUriAsync(wideImgUri);
                        await file.CopyAsync(folder, entry.Id + "Wide.png", NameCollisionOption.ReplaceExisting);
                        wideImgUri = new Uri($"ms-appdata:///local/PinnedTilesImages/{entry.Id}Wide.png");
                    }
                    else
                        wideImgUri = new Uri($"ms-appdata:///local/PinnedTilesImages/{entry.Id}.png");

                    imgUri = new Uri($"ms-appdata:///local/PinnedTilesImages/{entry.Id}.png");
                }
                else
                {
                    imgUri = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");
                    wideImgUri = new Uri("ms-appx:///Assets/Wide310x150Logo.scale-200.png");
                }
                //pin tile
                if (action.Action == TileActions.OpenUrl)
                if (!action.Param.Contains("http"))
                    action.Param = "http://" + action.Param;
                var tile = new SecondaryTile(entry.Id.ToString(), "MALClient", string.Join(";",new string[] { action.Action.ToString(), action.Param}), imgUri,
                    TileSize.Square150x150);
                tile.WideLogo = wideImgUri;
                RegisterTile(entry.Id.ToString());
                await tile.RequestCreateAsync();
                RegisterTileCache(entry.Id,new PinnedTileCache {ImgUri = imgUri,WideImgUri = wideImgUri,Settings = settings});
                if(settings.AnythingAtAll)
                    UpdateTile(entry,imgUri,wideImgUri,settings);
                Utils.TelemetryTrackEvent(TelemetryTrackedEvents.PinnedTile);
            }
            catch (Exception)
            {
                //who knows?
            }
        }

        private static void UpdateTile(IAnimeData entry, Uri imgUri, Uri wideImgUri, PinTileSettings settings)
        {
            //scaryy...
            StringBuilder tileXmlString = new StringBuilder();
            tileXmlString.Append("<tile>");
            tileXmlString.Append($"<visual version='3' {(settings.AddBranding ? "branding='nameAndLogo'" : "")}>");
            tileXmlString.Append("<binding template='TileSquare150x150PeekImageAndText01' fallback='TileSquarePeekImageAndText01'>");
            if (settings.AddImage) tileXmlString.Append($"<image id=\"1\" src=\"{imgUri}\"/>");
            if (settings.AddTitle) tileXmlString.Append($"<text hint-style=\"subtitle\" hint-wrap=\"true\" hint-maxLines=\"{(settings.BigTitle ? "2" : "1")}\" id=\"1\">{entry.Title}</text>");
            if (settings.AddStatus) tileXmlString.Append($"<text hint-style=\"caption\" hint-wrap=\"false\" id=\"2\">{(AnimeStatus)entry.MyStatus}</text>");
            if (settings.AddScore) tileXmlString.Append($"<text hint-style=\"caption\" id=\"3\">{(entry.MyScore == 0 ? "Unranked" : entry.MyScore + $"/{(Settings.SelectedApiType == ApiType.Mal ? "10" : "5")}")}{(settings.AddWatched ? " - " + entry.MyEpisodes + $"/{(entry.AllEpisodes == 0 ? "?" : entry.AllEpisodes.ToString())}" : "")}</text>");
            if (settings.AddAirDay && entry is AnimeItemViewModel && ((AnimeItemViewModel)entry).AirDayBind != "") tileXmlString.Append($"<text hint-style=\"caption\" id=\"4\">\n{((AnimeItemViewModel)entry).AirDayBind}</text>");
            tileXmlString.Append("</binding>");
            tileXmlString.Append("<binding template='TileWide310x150ImageAndText02' fallback='TileWideImageAndText02'>");
            if (settings.AddImage) tileXmlString.Append($"<image id=\"1\" src=\"{wideImgUri}\"/>");
            if (settings.AddTitle) tileXmlString.Append($"<text hint-style=\"title\" hint-maxLines=\"{(settings.BigTitle ? "2" : "1")}\" id=\"1\">{entry.Title}</text>");
            tileXmlString.Append("<text id=\"2\"  hint-style=\"body\">");
            if (settings.AddStatus) tileXmlString.Append($"{(AnimeStatus)entry.MyStatus}{(settings.AddWatched ? " - " + entry.MyEpisodes + $"/{(entry.AllEpisodes == 0 ? "?" : entry.AllEpisodes.ToString())}" : "")}");
            if (settings.AddScore) tileXmlString.Append($"\n{(entry.MyScore == 0 ? "Unranked" : entry.MyScore + $"/{(Settings.SelectedApiType == ApiType.Mal ? "10" : "5")}")}");
            if (settings.AddAirDay && entry is AnimeItemViewModel && ((AnimeItemViewModel)entry).AirDayBind != "") tileXmlString.Append($" - {((AnimeItemViewModel)entry).AirDayBind}");
            tileXmlString.Append("</text>");
            tileXmlString.Append("</binding>");
            tileXmlString.Append("</visual>");
            tileXmlString.Append("</tile>");
            //uff, yup... that was scarry mess

            try
            {
                var mgr = TileUpdateManager.CreateTileUpdaterForSecondaryTile(entry.Id.ToString());
                var notif = new Windows.Data.Xml.Dom.XmlDocument();
                notif.LoadXml(tileXmlString.ToString());
                mgr.Update(new TileNotification(notif));
            }
            catch (Exception)
            {
                // no tile
            }

        }

        /// <summary>
        /// Updates tile but tries to find it's setting beforehand.
        /// </summary>
        /// <param name="entry"></param>
        public static void UpdateTile(IAnimeData entry)
        {
            if (_pinnedCache.ContainsKey(entry.Id))
            {
                var cache = _pinnedCache[entry.Id];
                UpdateTile(entry,cache.ImgUri,cache.WideImgUri,cache.Settings);
            }
        }

    }
}
