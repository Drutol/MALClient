using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ExpressionBuilder;
using MALClient.Models.Enums;
using MALClient.UWP.Shared.Managers;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Items;
using MALClient.XShared.ViewModels.Main;
using WinRTXamlToolkit.Controls.Extensions;
using System.Threading.Tasks;

namespace MALClient.UWP.Pages.Main
{
    public sealed partial class ListComparisonPage : Page
    {
        private ListComparisonViewModel _viewModel = ViewModelLocator.Comparison;
        private BlurHelper _blurHelper;
        private CompositionPropertySet _scrollProperties;
        private bool _initialized;
        private ScalarNode _parallaxExpression;
        private int _lastColumns;

        public ListComparisonPage()
        {
            this.InitializeComponent();
            Loaded+=OnLoaded;
            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            var newColumns = (int) (GridView.ActualWidth / 380.0f);
            if (newColumns != _lastColumns)
            {
                _lastColumns = newColumns;

                for (int i = 0; i < _viewModel.CurrentItems.Count; i++)
                {
                    var container = GridView.ContainerFromIndex(i);
                    if(container == null)
                        return;
                    Image image = container.GetFirstDescendantOfType<Image>();

                    Visual visual = ElementCompositionPreview.GetElementVisual(image);
                    visual.Size = new Vector2(380, 270);

                    if (!(_parallaxExpression is null))
                    {
                        var row = i / (int) (GridView.ActualWidth / 380.0f);
                        var offset = row * visual.Size.Y;
                        _parallaxExpression.SetScalarParameter("StartOffset", offset);
                        visual.StartAnimation("Offset.Y", _parallaxExpression);
                    }
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _blurHelper = new BlurHelper(HeaderBlurBg,false);

            ComparisonFiltersList.SelectedIndex = (int)_viewModel.ComparisonFilter;
            var index = (int) _viewModel.StatusFilter;
            index = index > 4 ? index - 1 : index;
            StatusFiltersList.SelectedIndex = index-1;
            SortOptionsList.SelectedIndex = (int) _viewModel.ComparisonSorting;
            FilterStatusComboBox.SelectedIndex = (int) _viewModel.StatusFilterTarget;

            // Get scrollviewer
            ScrollViewer myScrollViewer = GridView.GetFirstDescendantOfType<ScrollViewer>();
            _scrollProperties = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(myScrollViewer);

            // Setup the expression
            var scrollPropSet = _scrollProperties.GetSpecializedReference<ManipulationPropertySetReferenceNode>();
            var startOffset = ExpressionValues.Constant.CreateConstantScalar("startOffset", 0.0f);
            var parallaxValue = 0.7f;
            var parallax = (scrollPropSet.Translation.Y + startOffset);
            _parallaxExpression = parallax * parallaxValue - parallax/1.11f;

            _lastColumns =  (int)(GridView.ActualWidth / 380.0f);
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _viewModel.NavigatedTo(e.Parameter as ListComparisonPageNavigationArgs);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _parallaxExpression.Dispose();
            base.OnNavigatedFrom(e);
        }

        private void OnStatusFilterSelected(object sender, ItemClickEventArgs e)
        {
            _viewModel.StatusFilter = (AnimeStatus) (e.ClickedItem as FrameworkElement).Tag;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.StatusFilterTarget = (ComparisonStatusFilterTarget) FilterStatusComboBox.SelectedIndex;
        }

        private void OnComparisonStatusFilterSelected(object sender, ItemClickEventArgs e)
        {
            _viewModel.ComparisonFilter = (ComparisonFilter)(e.ClickedItem as FrameworkElement).Tag;
        }

        private void OnSortingSelected(object sender, ItemClickEventArgs e)
        {
            _viewModel.ComparisonSorting = (ComparisonSorting)(e.ClickedItem as FrameworkElement).Tag;
        }

        private void Header_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridHeader.Height = Header.ActualHeight;
        }

        private TypeInfo _typeInfo;


        private void FilterStatusComboBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var typeInfo = _typeInfo ?? (_typeInfo = typeof(FrameworkElement).GetTypeInfo());
                var prop = typeInfo.GetDeclaredProperty("AllowFocusOnInteraction");
                prop?.SetValue(sender, true);
            }
            catch (Exception)
            {
                //not AU
            }
        }

        private async void GridView_OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            Image image = args.ItemContainer.ContentTemplateRoot.GetFirstDescendantOfType<Image>();


            var vm = args.Item as ComparisonItemViewModel;
            if (vm.IsOnMyList)
            {
                image.Width = 286;
                image.Height = 359;
                (image.Parent as FrameworkElement).Margin = new Thickness(130,0,0,0);
            }
            else
            {
                image.Width = 380;
                image.Height = 500;
                (image.Parent as FrameworkElement).Margin = new Thickness(0);
            }

            Visual visual = ElementCompositionPreview.GetElementVisual(image);
            visual.Size = new Vector2(380, 270);

            if (!(_parallaxExpression is null))
            {
                var row = args.ItemIndex / (int) (GridView.ActualWidth / 380.0f);
                var offset = row * visual.Size.Y;
                _parallaxExpression.SetScalarParameter("StartOffset", offset);
                visual.StartAnimation("Offset.Y", _parallaxExpression);
            }

        }
    }
}
