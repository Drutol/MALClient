using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Listeners;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.ArticlesPageFragments
{
    public class ArticlesPageTabFragment : MalFragmentBase
    {
        private MalArticlesViewModel ViewModel;

        public ArticlesPageTabFragment(bool initBindings) : base(initBindings)
        {
            ViewModel = ViewModelLocator.MalArticles;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            
        }

        protected override void InitBindings()
        {
            Bindings.Add(ArticlesPageTabItemList.Id, new List<Binding>());
            Bindings[ArticlesPageTabItemList.Id].Add(
                this.SetBinding(() => ViewModel.Articles).WhenSourceChanges(() =>
                {
                    ArticlesPageTabItemList.Adapter = ViewModel.Articles.GetAdapter(GetTemplateDelegate);
                }));
        }

        private View GetTemplateDelegate(int i, MalNewsUnitModel malNewsUnitModel, View convertView)
        {
            var view = convertView;
            if (view == null)
            {
                view = Activity.LayoutInflater.Inflate(Resource.Layout.ArtclesPageItem, null);
                view.SetOnClickListener(new OnClickListener(OnItemClick));
                if(ViewModel.ThumbnailWidth == 100)
                    view.FindViewById<ImageViewAsync>(Resource.Id.ArticlesPageItemImage).SetScaleType(ImageView.ScaleType.Center);
            }
            view.Tag = malNewsUnitModel.Wrap();

            view.FindViewById<TextView>(Resource.Id.ArticlesPageItemAuthor).Text = malNewsUnitModel.Author;
            view.FindViewById<TextView>(Resource.Id.ArticlesPageItemViews).Text = malNewsUnitModel.Views;
            view.FindViewById<TextView>(Resource.Id.ArticlesPageItemTags).Text = malNewsUnitModel.Tags;
            view.FindViewById<TextView>(Resource.Id.ArticlesPageItemHeader).Text = malNewsUnitModel.Title;
            view.FindViewById<TextView>(Resource.Id.ArticlesPageItemHighlight).Text = malNewsUnitModel.Highlight;
            ImageService.Instance.LoadUrl(malNewsUnitModel.ImgUrl)
                .Into(view.FindViewById<ImageViewAsync>(Resource.Id.ArticlesPageItemImage));

            return view;
        }

        private void OnItemClick(View view)
        {
            ViewModel.LoadArticleCommand.Execute(view.Tag.Unwrap<MalNewsUnitModel>());
        }

        public override int LayoutResourceId => Resource.Layout.ArticlesPageTabItem;


        #region Views

        private ListView _articlesPageTabItemList;

        public ListView ArticlesPageTabItemList => _articlesPageTabItemList ?? (_articlesPageTabItemList = FindViewById<ListView>(Resource.Id.ArticlesPageTabItemList));



        #endregion
    }
}