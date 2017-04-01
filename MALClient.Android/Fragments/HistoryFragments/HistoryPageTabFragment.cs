using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Fragments.HistoryFragments
{
    public class HistoryPageTabFragment : MalFragmentBase
    {
        private readonly List<Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>> _data;

        public HistoryPageTabFragment(List<Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>> data)
        {
            _data = data;
        }

        protected override void Init(Bundle savedInstanceState)
        {

        }

        protected override void InitBindings()
        {
            (RootView as ListView).InjectFlingAdapter(_data,DataTemplateFull,DataTemplateFling,ContainerTemplate);
        }

        private View ContainerTemplate(int i)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.HistoryPageTabItem, null);
            view.FindViewById(Resource.Id.HistoryPageTabItemAnimeLightItem).Click += HistoryEntryCoverOnClick;
            return view;
        }

        private void HistoryEntryCoverOnClick(object sender, EventArgs eventArgs)
        {
            (sender as View).Tag.Unwrap<AnimeItemViewModel>().NavigateDetails(PageIndex.PageHistory,HistoryPageFragment.LastArgs);
        }

        private void DataTemplateFling(View view, int i, Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>> tuple)
        {
            view.FindViewById(Resource.Id.AnimeLightItemImgPlaceholder).Visibility = ViewStates.Visible;
            view.FindViewById(Resource.Id.AnimeLightItemImage).Visibility = ViewStates.Invisible;
            view.FindViewById<TextView>(Resource.Id.AnimeLightItemTitle).Text = tuple.Item1.Title;
        }

        private void DataTemplateFull(View view, int i, Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>> tuple)
        {
            view.FindViewById(Resource.Id.AnimeLightItemImgPlaceholder).Visibility = ViewStates.Gone;
            view.FindViewById<TextView>(Resource.Id.AnimeLightItemTitle).Text = tuple.Item1.Title;
            var image = view.FindViewById<ImageViewAsync>(Resource.Id.AnimeLightItemImage);
            if (image.Tag == null || (string)image.Tag != tuple.Item1.ImgUrl)
            {
                image.Into(tuple.Item1.ImgUrl, null, img => img.HandleScaling());
                image.Tag = tuple.Item1.ImgUrl;
            }
            else
            {
                view.FindViewById(Resource.Id.AnimeLightItemImage).Visibility = ViewStates.Visible;
            }
            view.FindViewById<LinearLayout>(Resource.Id.HistoryPageTabItemEventsList).SetAdapter(tuple.Item2.GetAdapter(GetTemplateDelegate));
            view.FindViewById(Resource.Id.HistoryPageTabItemAnimeLightItem).Tag = tuple.Item1.Wrap();
        }

        private View GetTemplateDelegate(int i, MalProfileHistoryEntry malProfileHistoryEntry, View arg3)
        {
            var view = new FrameLayout(Activity) { LayoutParameters = new ViewGroup.LayoutParams(-1, -2) };

            var txt1params = new FrameLayout.LayoutParams(-2, -2);
            txt1params.Gravity = GravityFlags.Start;
            var txt1 = new TextView(Activity)
            {
                LayoutParameters = txt1params,
                Text = $"{malProfileHistoryEntry.ShowUnit} {malProfileHistoryEntry.WatchedEpisode}"
            };
            txt1.SetTextColor(new Color(ResourceExtension.BrushText));


            var txt2params = new FrameLayout.LayoutParams(-2, -2);
            txt2params.Gravity = GravityFlags.End;
            var txt2 = new TextView(Activity)
            {
                LayoutParameters = txt2params,
                Text = malProfileHistoryEntry.Date
            };
            txt2.SetTextColor(new Color(ResourceExtension.BrushText));
            txt2.Typeface = Typeface.Create(ResourceExtension.FontSizeLight,TypefaceStyle.Normal);

            view.AddView(txt1);
            view.AddView(txt2);

            return view;
        }

        public override int LayoutResourceId => Resource.Layout.HistoryPageTab;
    }
}