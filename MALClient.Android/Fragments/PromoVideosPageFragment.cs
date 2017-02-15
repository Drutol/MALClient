using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.Resources;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments
{
    public class PromoVideosPageFragment : MalFragmentBase
    {
        private PopularVideosViewModel ViewModel;

        private static readonly StyleSpan PrefixStyle;
        private static readonly ForegroundColorSpan PrefixColorStyle;

        static PromoVideosPageFragment()
        {
            PrefixStyle = new StyleSpan(TypefaceStyle.Bold);
            PrefixColorStyle = new ForegroundColorSpan(new Color(ResourceExtension.AccentColour));
        }

        private GridViewColumnHelper _helper;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.PopularVideos;
            ViewModel.Init();
        }

        private bool IsFlingActive
        {
            get { return _isFlingActive; }
            set
            {
                if(value == _isFlingActive)
                    return;

                _isFlingActive = value;
                if (!value)
                    RefreshItemBindings();
            }
        }

        private void RefreshItemBindings()
        {
            for (int i = 0; i < PromoVideosPageGridView.ChildCount; i++)
            {
                var view = PromoVideosPageGridView.GetChildAt(i);
                SetItemBindings(view, view.Tag.Unwrap<AnimeVideoData>());
            }
        }

        protected override void InitBindings()
        {
            Bindings.Add(PromoVideosPageLoadingSpinner.Id, new List<Binding>());
            Bindings[PromoVideosPageLoadingSpinner.Id].Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => PromoVideosPageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            PromoVideosPageGridView.MakeFlingAware(b => IsFlingActive = b);

            _helper = new GridViewColumnHelper(PromoVideosPageGridView);

            Bindings.Add(PromoVideosPageGridView.Id, new List<Binding>());
            Bindings[PromoVideosPageGridView.Id].Add(this.SetBinding(() => ViewModel.Videos).WhenSourceChanges(() =>
            {
                PromoVideosPageGridView.Adapter = ViewModel.Videos.GetAdapter(GetTemplateDelegate);
            }));          
        }

        private View GetTemplateDelegate(int i, AnimeVideoData animeVideoData, View convertView)
        {
            var view = convertView;
            if (view == null)
            {
                view = Activity.LayoutInflater.Inflate(Resource.Layout.PromoVideosPageItem, null);

                view.Click += VideoItemOnClick;
            }

            SetItemBindings(view,animeVideoData);

            view.Tag = animeVideoData.Wrap();
            return view;
        }

        private void SetItemBindings(View view,AnimeVideoData animeVideoData)
        {
            var img = view.FindViewById<ImageViewAsync>(Resource.Id.PromoVideosPageItemImage);
            if (!IsFlingActive && (string)img.Tag != animeVideoData.YtLink)
            {
                view.FindViewById(Resource.Id.PromoVideosPageItemImgPlaceholder).Visibility = ViewStates.Gone;
                img.Tag = animeVideoData.YtLink;
                img.Into(animeVideoData.Thumb);
            }
            else if (IsFlingActive)
            {
                view.FindViewById(Resource.Id.PromoVideosPageItemImgPlaceholder).Visibility = ViewStates.Visible;
                img.Visibility = ViewStates.Invisible;
            }

            var str = new SpannableString($"{animeVideoData.Name} - {animeVideoData.AnimeTitle}");
            str.SetSpan(PrefixStyle, 0, animeVideoData.Name.Length, SpanTypes.InclusiveInclusive);
            str.SetSpan(PrefixColorStyle, 0, animeVideoData.Name.Length, SpanTypes.InclusiveInclusive);
            view.FindViewById<TextView>(Resource.Id.PromoVideosPageItemSubtitle)
                .SetText(str.SubSequenceFormatted(0, str.Length()), TextView.BufferType.Spannable);
        }

        private void VideoItemOnClick(object sender, EventArgs eventArgs)
        {
            
        }

        public override int LayoutResourceId => Resource.Layout.PromoVideosPage;

        #region Views

        private GridView _promoVideosPageGridView;
        private ProgressBar _promoVideosPageLoadingSpinner;
        private bool _isFlingActive;

        public GridView PromoVideosPageGridView => _promoVideosPageGridView ?? (_promoVideosPageGridView = FindViewById<GridView>(Resource.Id.PromoVideosPageGridView));

        public ProgressBar PromoVideosPageLoadingSpinner => _promoVideosPageLoadingSpinner ?? (_promoVideosPageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.PromoVideosPageLoadingSpinner));

        #endregion
    }
}