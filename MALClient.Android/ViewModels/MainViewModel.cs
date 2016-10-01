using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using MALClient.Android.Fragments;
using MALClient.XShared.Delegates;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.ViewModels
{
    public delegate void AndroidNavigationRequest(Fragment fragment);

    public class MainViewModel : MainViewModelBase
    {
        public new event AndroidNavigationRequest MainNavigationRequested;

        protected override void CurrentStatusStoryboardBegin()
        {
            //throw new NotImplementedException();
        }

        protected override void CurrentOffSubStatusStoryboardBegin()
        {
           // throw new NotImplementedException();
        }

        protected override void CurrentOffStatusStoryboardBegin()
        {
          //  throw new NotImplementedException();
        }

        public override void Navigate(PageIndex index, object args = null)
        {
            switch (index)
            {
                case PageIndex.PageAnimeList:
                    break;
                case PageIndex.PageAnimeDetails:
                    break;
                case PageIndex.PageSettings:
                    break;
                case PageIndex.PageSearch:
                    break;
                case PageIndex.PageLogIn:
                    MainNavigationRequested?.Invoke(LogInPageFragment.Instance);
                    break;
                case PageIndex.PageProfile:
                    break;
                case PageIndex.PageAbout:
                    break;
                case PageIndex.PageRecomendations:
                    break;
                case PageIndex.PageSeasonal:
                    break;
                case PageIndex.PageMangaList:
                    break;
                case PageIndex.PageMangaSearch:
                    break;
                case PageIndex.PageTopAnime:
                    break;
                case PageIndex.PageTopManga:
                    break;
                case PageIndex.PageCalendar:
                    break;
                case PageIndex.PageArticles:
                    break;
                case PageIndex.PageNews:
                    break;
                case PageIndex.PageMessanging:
                    break;
                case PageIndex.PageMessageDetails:
                    break;
                case PageIndex.PageForumIndex:
                    break;
                case PageIndex.PageHistory:
                    break;
                case PageIndex.PageCharacterDetails:
                    break;
                case PageIndex.PageStaffDetails:
                    break;
                case PageIndex.PageCharacterSearch:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
        }
    }
}