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
using FFImageLoading;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android;
using MALClient.Android.Resources;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;
using UK.CO.Deanwild.Flowtextview;

namespace MALClient.Android.Fragments.AnimeDetailsPageTabs
{
    public class AnimeDetailsPageRecomsTabFragment : MalFragmentBase
    {
        private AnimeDetailsPageViewModel ViewModel;
        private ObservableAdapter<DirectRecommendationData> _adapter;
        private ListView _list;

        private AnimeDetailsPageRecomsTabFragment()
        {
            ViewModel = ViewModelLocator.AnimeDetails;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            
        }

        protected override void InitBindings()
        {
            _adapter = ViewModel.Recommendations.GetAdapter(RecomItemDelegate);
            _list = FindViewById<ListView>(Resource.Id.AnimeDetailsPageRecomTabsList);
            _list.Adapter = _adapter;
            _list.ItemClick += ListOnItemClick;
        }

        private void ListOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            var data = itemClickEventArgs.View.Tag.Unwrap<DirectRecommendationData>();
            ViewModel.NavigateDetailsCommand.Execute(data);
        }

        private View RecomItemDelegate(int i, DirectRecommendationData animeReviewData, View convertView)
        {
            var view = convertView;
            FlowTextView txt = null;
            if (view == null)
            {
                view = MainActivity.CurrentContext.LayoutInflater.Inflate(Resource.Layout.AnimeRecomItem,null);
                txt = view.FindViewById<FlowTextView>(Resource.Id.AnimeRecomItemRecomContent);
                txt.TextColor = ResourceExtension.BrushText;
                txt.SetTypeface(Typeface.Create(Typeface.Default, TypefaceStyle.Normal));     
                txt.SetTextSize(28f);         
            }

            view.Tag = new JavaObjectWrapper<DirectRecommendationData>(animeReviewData);

            view.FindViewById<TextView>(Resource.Id.AnimeRecomItemShowTitle).Text = animeReviewData.Title;
            view.FindViewById<TextView>(Resource.Id.AnimeRecomItemShowType).Text = animeReviewData.Type.ToString();
            txt = txt ?? view.FindViewById<FlowTextView>(Resource.Id.AnimeRecomItemRecomContent);
            txt.Text = animeReviewData.Description;
            

            var img = view.FindViewById<ImageViewAsync>(Resource.Id.AnimeRecomItemImage);
            ImageService.Instance.LoadUrl(animeReviewData.ImageUrl).FadeAnimation(true, true).Into(img);

            

            return view;
        }

        protected override void Cleanup()
        {
            _list.Adapter = null;
            _adapter.Dispose();
            _list.ItemClick -= ListOnItemClick;
            base.Cleanup();
        }

        public static AnimeDetailsPageRecomsTabFragment Instance => new AnimeDetailsPageRecomsTabFragment();

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageRecomsTab;

    }
}