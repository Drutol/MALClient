using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using MALClient.Models.Enums;
using MALClient.Models.Models.Library;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Comm.Articles;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;

namespace MALClient.Shared.Managers
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
        private const string NewsTileId = "TileNews";
        private const string ArticlesTileId = "TileArticles";
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
            #region RemoveImages

            var tiles = (string)ApplicationData.Current.LocalSettings.Values["tiles"];
            if (string.IsNullOrWhiteSpace(tiles))
                return;

            bool removed = false;
            var newTiles = "";
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
            #endregion


            if (SecondaryTile.Exists(ArticlesTileId))
                UpdateNewsTile(ArticlePageWorkMode.Articles);
            if (SecondaryTile.Exists(NewsTileId))
                UpdateNewsTile(ArticlePageWorkMode.News);


        }

        #region NewsTiles

        public static async void PinNewsTile()
        {
            SecondaryTile tile = new SecondaryTile(NewsTileId, "News", "https://myanimelist.net/news",
                new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png"), TileSize.Square150x150);
            tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.scale-200.png");
            tile.VisualElements.Square310x310Logo = new Uri("ms-appx:///Assets/Wide310x310Logo.scale-200.png");

            if (await tile.RequestCreateAsync())
                UpdateNewsTile(ArticlePageWorkMode.News);
        }

        public static async void PinArticlesTile()
        {
            SecondaryTile tile = new SecondaryTile(ArticlesTileId, "Articles", "https://myanimelist.net/featured",
                new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png"), TileSize.Square150x150);
            tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.scale-200.png");
            tile.VisualElements.Square310x310Logo = new Uri("ms-appx:///Assets/Wide310x310Logo.scale-200.png");

            if(await tile.RequestCreateAsync())
                UpdateNewsTile(ArticlePageWorkMode.Articles);
        }

        private static async void UpdateNewsTile(ArticlePageWorkMode mode)
        {
            var news = await new MalArticlesIndexQuery(mode).GetArticlesIndex();

            var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(mode == ArticlePageWorkMode.Articles ? ArticlesTileId : NewsTileId);
            updater.EnableNotificationQueue(true);
            updater.Clear();
            foreach (var malNewsUnitModel in news.Take(5))
            {
                var tileContent = new TileContent
                {
                    Visual = new TileVisual
                    {
                        TileMedium = GenerateTileBindingMedium(malNewsUnitModel),
                        TileWide = GenerateTileBindingWide(malNewsUnitModel),
                    }
                };
                if (!ViewModelLocator.Mobile)
                    tileContent.Visual.TileLarge = GenerateTileBindingLarge(malNewsUnitModel);
                updater.Update(new TileNotification(tileContent.GetXml()));
            }
        }

        private static TileBinding GenerateTileBindingMedium(MalNewsUnitModel news)
        {
            return new TileBinding
            {
                Content = new TileBindingContentAdaptive()
                {                  
                    PeekImage = new TilePeekImage
                    {
                        Source = news.ImgUrl,
                        HintCrop = TilePeekImageCrop.Default,
                    },
                    Children =
                    {
                        new AdaptiveText
                        {
                            Text = news.Title,
                            HintWrap = true,
                        }
                    }
                }
            };
        }

        private static TileBinding GenerateTileBindingWide(MalNewsUnitModel news)
        {
            return new TileBinding
            {
                Content = new TileBindingContentAdaptive()
                {                                  
                    Children =
                    {
                        new AdaptiveGroup
                        {
                            Children =
                            {
                                new AdaptiveSubgroup
                                {
                                    HintWeight = 3,
                                    Children =
                                    {
                                        new AdaptiveImage
                                        {
                                            Source = news.ImgUrl,
                                            HintRemoveMargin = true,
                                            HintAlign = AdaptiveImageAlign.Stretch,                                          
                                        }
                                    }
                                },
                                new AdaptiveSubgroup
                                {
                                    HintWeight = 7,

                                    Children =
                                    {
                                        new AdaptiveText
                                        {
                                            Text = news.Title,
                                            HintMaxLines = 2,
                                            HintWrap = true,
                                            HintStyle = AdaptiveTextStyle.Body,
                                        },
                                        new AdaptiveText
                                        {
                                            Text = news.Highlight,
                                            HintMaxLines = 10,
                                            HintWrap = true,
                                            HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                        }
                                    }
                                }
                                
                            }
                        }

                    }
                }
            };
        }

        private static TileBinding GenerateTileBindingLarge(MalNewsUnitModel news)
        {
            return new TileBinding
            {
                Content = new TileBindingContentAdaptive()
                {
                    PeekImage = new TilePeekImage
                    {
                        Source = news.ImgUrl,
                    },
                    Children =
                    {
                        new AdaptiveGroup
                        {
                            Children =
                            {
                                new AdaptiveSubgroup
                                {
                                    HintWeight = 4,
                                    Children =
                                    {
                                        new AdaptiveImage
                                        {
                                            Source = news.ImgUrl,
                                            HintRemoveMargin = true,
                                            HintAlign = AdaptiveImageAlign.Stretch,
                                        }
                                    }
                                },
                                new AdaptiveSubgroup
                                {
                                    HintWeight = 6,

                                    Children =
                                    {
                                        new AdaptiveText
                                        {
                                            Text = news.Title,
                                            HintMaxLines = 3,
                                            HintWrap = true,                                          
                                            HintStyle = AdaptiveTextStyle.Subtitle,
                                        },
                                        new AdaptiveText
                                        {
                                            Text = news.Highlight,
                                            HintMaxLines = 12,
                                            HintWrap = true,
                                            HintStyle = AdaptiveTextStyle.BodySubtle,
                                        }
                                    }
                                }

                            }
                        }

                    }
                }
            };
        }
        #endregion



        #region StandardTiles
        public static async Task PinTile(IAnimeData entry, Uri imgUri, Uri wideImgUri, PinTileSettings settings, PinTileActionSetting action)
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
                var tile = new SecondaryTile(entry.Id.ToString(),
                    "MALClient",
                    string.Join(";", action.Action.ToString(), action.Param),
                    imgUri,
                    TileSize.Square150x150);

                tile.WideLogo = wideImgUri;
                RegisterTile(entry.Id.ToString());
                await tile.RequestCreateAsync();
                RegisterTileCache(entry.Id, new PinnedTileCache { ImgUri = imgUri, WideImgUri = wideImgUri, Settings = settings });
                if (settings.AnythingAtAll)
                    UpdateTile(entry, imgUri, wideImgUri, settings);
                ResourceLocator.TelemetryProvider.TelemetryTrackEvent(TelemetryTrackedEvents.PinnedTile);
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
            if (settings.AddAirDay && entry is AnimeItemViewModel && ((AnimeItemViewModel)entry).TopLeftInfoBind != "") tileXmlString.Append($"<text hint-style=\"caption\" id=\"4\">\n{((AnimeItemViewModel)entry).TopLeftInfoBind}</text>");
            tileXmlString.Append("</binding>");
            tileXmlString.Append("<binding template='TileWide310x150ImageAndText02' fallback='TileWideImageAndText02'>");
            if (settings.AddImage) tileXmlString.Append($"<image id=\"1\" src=\"{wideImgUri}\"/>");
            if (settings.AddTitle) tileXmlString.Append($"<text hint-style=\"title\" hint-maxLines=\"{(settings.BigTitle ? "2" : "1")}\" id=\"1\">{entry.Title}</text>");
            tileXmlString.Append("<text id=\"2\"  hint-style=\"body\">");
            if (settings.AddStatus) tileXmlString.Append($"{(AnimeStatus)entry.MyStatus}{(settings.AddWatched ? " - " + entry.MyEpisodes + $"/{(entry.AllEpisodes == 0 ? "?" : entry.AllEpisodes.ToString())}" : "")}");
            if (settings.AddScore) tileXmlString.Append($"\n{(entry.MyScore == 0 ? "Unranked" : entry.MyScore + $"/{(Settings.SelectedApiType == ApiType.Mal ? "10" : "5")}")}");
            if (settings.AddAirDay && entry is AnimeItemViewModel && ((AnimeItemViewModel)entry).TopLeftInfoBind != "") tileXmlString.Append($" - {((AnimeItemViewModel)entry).TopLeftInfoBind}");
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
                UpdateTile(entry, cache.ImgUri, cache.WideImgUri, cache.Settings);
            }
        }


        #endregion


    }
}
