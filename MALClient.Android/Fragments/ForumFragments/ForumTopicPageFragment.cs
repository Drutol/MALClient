using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.CollectionAdapters;
using MALClient.Android.Resources;
using MALClient.Android.UserControls;
using MALClient.Android.UserControls.ForumItems;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums;
using MALClient.XShared.ViewModels.Forums.Items;

namespace MALClient.Android.Fragments.ForumFragments
{
    public class ForumTopicPageFragment : MalFragmentBase
    {
        private readonly ForumsTopicNavigationArgs _args;
        private ForumTopicViewModel ViewModel;

        private View _prevHighlightedPageIndicator;

        public ForumTopicPageFragment(ForumsTopicNavigationArgs args)
        {
            _args = args;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.ForumsTopic;
            ViewModel.RequestScroll += ViewModelOnRequestScroll;
            ViewModel.Init(_args);
        }

        private async void ViewModelOnRequestScroll(object sender, int i)
        {
            if(i < ForumTopicPagePostsList.Adapter.Count)
                ForumTopicPagePostsList.SetSelection(i);
            else
            {
                await Task.Delay(100);
                ViewModelOnRequestScroll(sender,i);
            }
        }

        protected override void InitBindings()
        {
            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingTopic,
                    () => ForumTopicPageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));


            Bindings.Add(this.SetBinding(() => ViewModel.Messages).WhenSourceChanges(() =>
            {
                if(ViewModel.Messages != null)
                    ForumTopicPagePostsList.InjectFlingAdapter(ViewModel.Messages,DataTemplateFull,DataTemplateFling,ContainerTemplate   );
            }));

            ForumTopicPagePostsList.MakeFlingAware();

            ViewModel.AvailablePages.CollectionChanged += (sender, args) => UpdatePageSelection();

            Bindings.Add(this.SetBinding(() => ViewModel.AvailablePages).WhenSourceChanges(UpdatePageSelection));
        }

        private View ContainerTemplate(int i)
        {
            return new ForumTopicItem(Context);
        }

        private void DataTemplateFling(View view, int i, ForumTopicMessageEntryViewModel arg3)
        {
            ((ForumTopicItem)view).BindModel(arg3,true);
        }

        private void DataTemplateFull(View view, int i, ForumTopicMessageEntryViewModel arg3)
        {
            ((ForumTopicItem)view).BindModel(arg3, false);
        }

        private void UpdatePageSelection()
        {
            if(ViewModel.AvailablePages != null)
                ForumTopicPageList.SetAdapter(ViewModel.AvailablePages.GetAdapter(GetPageItemTemplateDelegate));
        }

        private View GetPageItemTemplateDelegate(int i, Tuple<int, bool> tuple, View arg3)
        {
            var view = MainActivity.CurrentContext.LayoutInflater.Inflate(Resource.Layout.PageIndicatorItem, null);

            view.Click += PageItemOnClick;
            view.Tag = tuple.Item1;

            view.FindViewById(Resource.Id.PageIndicatorItemBackgroundPanel)
                .SetBackgroundResource(tuple.Item2
                    ? ResourceExtension.AccentColourRes
                    : ResourceExtension.BrushAnimeItemInnerBackgroundRes);

            view.FindViewById<TextView>(Resource.Id.PageIndicatorItemNumber).Text = tuple.Item1.ToString();

            if (tuple.Item2)
                _prevHighlightedPageIndicator = view;

            return view;
        }

        private void PageItemOnClick(object sender, EventArgs eventArgs)
        {
            var view = sender as View;
            ViewModel.LoadPageCommand.Execute((int)view.Tag);
            //update it immediatelly
            view.FindViewById(Resource.Id.PageIndicatorItemBackgroundPanel)
                .SetBackgroundResource(ResourceExtension.AccentColourRes);
            _prevHighlightedPageIndicator.FindViewById(Resource.Id.PageIndicatorItemBackgroundPanel)
                .SetBackgroundResource(ResourceExtension.BrushAnimeItemInnerBackgroundRes);
        }

        public override int LayoutResourceId => Resource.Layout.ForumTopicPage;


        #region Views

        private Button _forumTopicPageToggleWatchingButton;
        private ImageButton _forumTopicPageGotoPageButton;
        private LinearLayout _forumTopicPageList;
        private ListView _forumTopicPagePostsList;
        private ProgressBar _forumTopicPageLoadingSpinner;
        private FloatingActionButton _forumTopicPageActionButton;

        public Button ForumTopicPageToggleWatchingButton => _forumTopicPageToggleWatchingButton ?? (_forumTopicPageToggleWatchingButton = FindViewById<Button>(Resource.Id.ForumTopicPageToggleWatchingButton));

        public ImageButton ForumTopicPageGotoPageButton => _forumTopicPageGotoPageButton ?? (_forumTopicPageGotoPageButton = FindViewById<ImageButton>(Resource.Id.ForumTopicPageGotoPageButton));

        public LinearLayout ForumTopicPageList => _forumTopicPageList ?? (_forumTopicPageList = FindViewById<LinearLayout>(Resource.Id.ForumTopicPageList));

        public ListView ForumTopicPagePostsList => _forumTopicPagePostsList ?? (_forumTopicPagePostsList = FindViewById<ListView>(Resource.Id.ForumTopicPagePostsList));

        public ProgressBar ForumTopicPageLoadingSpinner => _forumTopicPageLoadingSpinner ?? (_forumTopicPageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.ForumTopicPageLoadingSpinner));

        public FloatingActionButton ForumTopicPageActionButton => _forumTopicPageActionButton ?? (_forumTopicPageActionButton = FindViewById<FloatingActionButton>(Resource.Id.ForumTopicPageActionButton));



        #endregion
    }
}