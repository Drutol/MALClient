using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using MalClient.Shared.NavArgs;
using MALClient.Models;
using MALClient.Utils;
using MALClient.Utils.Enums;
using MALClient.ViewModels;

namespace MALClient.Items
{
    public sealed partial class AnimeSearchItem : UserControl, IAnimeData
    {
        private static bool _rowAlternator;

        private static readonly Brush _b2 =
            new SolidColorBrush(Application.Current.RequestedTheme == ApplicationTheme.Dark
                ? Color.FromArgb(170, 44, 44, 44)
                : Color.FromArgb(170, 230, 230, 230));

        private static readonly Brush _b1 =
            new SolidColorBrush(Application.Current.RequestedTheme == ApplicationTheme.Dark
                ? Color.FromArgb(255, 11, 11, 11)
                : Colors.WhiteSmoke);

        private readonly bool _animeMode;
        private readonly AnimeGeneralDetailsData _item;
        private Point _initialPoint;

        public AnimeSearchItem()
        {
            InitializeComponent();
            Root.Background = _rowAlternator
                ? _b2
                : _b1;
            _rowAlternator = !_rowAlternator;
        }

        public AnimeSearchItem(AnimeGeneralDetailsData data, bool anime = true) : this()
        {
            _item = data;
            Id = data.Id;
            GlobalScore = data.GlobalScore;
            AllEpisodes = data.AllEpisodes;
            if (!anime)
                AllVolumes = data.AllVolumes;
            Title = data.Title;
            Type = data.Type;
            Status = data.Status;
            TxtType.Text = Type;
            TxtTitle.Text = Title;
            TxtGlobalScore.Text = GlobalScore.ToString("N2");
            TxtSynopsis.Text = data.Synopsis;
            Img.Source = new BitmapImage(new Uri(data.ImgUrl));
            WatchedEps.Text = $"{(anime ? "Episodes" : "Chapters")} : {AllEpisodes}";
            _animeMode = anime;
        }

        public string Type { get; set; }
        private string Status { get; }

        public int Id { get; set; }
        public float GlobalScore { get; set; }
        public int AllEpisodes { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Notes { get; set; }
        public int MyVolumes { get; set; }
        public int AllVolumes { get; set; }
        public string Title { get; set; }

        //They must be here because reasons (interface reasons)
        public int MyEpisodes { get; set; }
        public float MyScore { get; set; }
        public int MyStatus { get; set; }

        private void ManipStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _initialPoint = e.Position;
        }

        private  void ManipDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (!e.IsInertial || !(e.Position.X - _initialPoint.X >= 70)) return;
            if (!(e.Position.X - _initialPoint.X >= 70)) return;
            
                ViewModelLocator.GeneralMain
                    .Navigate(PageIndex.PageAnimeDetails,
                        new AnimeDetailsPageNavigationArgs(Id, Title, _item, this,
                            new SearchPageNavigationArgs
                            {
                                Query = ViewModelLocator.SearchPage.PrevQuery,
                                Anime = _animeMode
                            })
                        {
                            Source = _animeMode ? PageIndex.PageSearch : PageIndex.PageMangaSearch,
                            AnimeMode = _animeMode
                        });
            e.Complete();
        }

        public async void NavigateDetails()
        {
            await Task.Delay(10);

            ViewModelLocator.GeneralMain
                    .Navigate(PageIndex.PageAnimeDetails,
                        new AnimeDetailsPageNavigationArgs(Id, Title, _item, this,
                            new SearchPageNavigationArgs
                            {
                                Query = ViewModelLocator.SearchPage.PrevQuery,
                                Anime = _animeMode
                            })
                        {
                            Source = _animeMode ? PageIndex.PageSearch : PageIndex.PageMangaSearch,
                            AnimeMode = _animeMode
                        });
        }

        private async void CopyLinkToClipboardCommand(object sender, RoutedEventArgs e)
        {
            FlyoutMore.Hide();
            var dp = new DataPackage();
            dp.SetText($"http://www.myanimelist.net/{(_animeMode ? "anime" : "manga")}/{Id}");
            Clipboard.SetContent(dp);
            FlyoutMore.Hide();
            Utilities.GiveStatusBarFeedback("Copied to clipboard...");
        }

        private async void OpenInMALCommand(object sender, RoutedEventArgs e)
        {
            FlyoutMore.Hide();
            await
                Launcher.LaunchUriAsync(
                    new Uri(
                        $"http://myanimelist.net/{(_animeMode ? "anime" : "manga")}/{Id}"));
        }
    }
}