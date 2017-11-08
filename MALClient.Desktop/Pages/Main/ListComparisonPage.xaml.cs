using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private TypeInfo _typeInfo;
        private ScrollViewer _myScrollViewer;
        private CompositionPropertySet _scrollProperties;
        private ScalarNode _parallaxExpression;
        private int _lastColumns;
        private bool _isWide;

        public ListComparisonPage()
        {
            this.InitializeComponent();
            Loaded+=OnLoaded;
            SizeChanged += OnSizeChanged;
        }

        private async void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            var newColumns = (int)(GridView.ActualWidth / 380.0f);
            if (newColumns != _lastColumns)
            {
                _lastColumns = newColumns;

                if(_viewModel.CurrentItems.Count == 0)
                    return;
                await Task.Delay(100);

                var panel = GridView.GetFirstDescendantOfType<ItemsWrapGrid>();

                for (int i = panel.FirstCacheIndex; i < panel.LastCacheIndex; i++)
                {
                    var container = GridView.ContainerFromIndex(i);
                    if (container == null)
                        return;
                    Image image = container.GetFirstDescendantOfType<Image>();

                    UpdateParalax(image, i);
                }

                await Task.Delay(500);
                _myScrollViewer.ChangeView(null, _myScrollViewer.VerticalOffset + 4, null);
            }
            var wasWide = _isWide;
            _isWide = sizeChangedEventArgs.NewSize.Width > 1205;
            if(wasWide && _isWide)
                return;
            UpdateVisualStates();

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
            _myScrollViewer = GridView.GetFirstDescendantOfType<ScrollViewer>();
            _scrollProperties = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(_myScrollViewer);

            // Setup the expression
            var scrollPropSet = _scrollProperties.GetSpecializedReference<ManipulationPropertySetReferenceNode>();
            var startOffset = ExpressionValues.Constant.CreateConstantScalar("startOffset", 0.0f);
            var parallaxValue = 0.7f;
            var parallax = (scrollPropSet.Translation.Y + startOffset);
            _parallaxExpression = parallax * parallaxValue - parallax/1.11f;

            _lastColumns =  (int)(GridView.ActualWidth / 380.0f);

            _isWide = Root.ActualWidth > 1205;
            UpdateVisualStates();

            _viewModel.CurrentItems.CollectionChanged += async (o, args) =>
            {
                await Task.Delay(500);
                _myScrollViewer.ChangeView(null, _myScrollViewer.VerticalOffset + 4, null);
            };
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _viewModel.NavigatedTo(e.Parameter as ListComparisonPageNavigationArgs);
            base.OnNavigatedTo(e);
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

        private void GridView_OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {

            Image image = args.ItemContainer.ContentTemplateRoot.GetFirstDescendantOfType<Image>();


            var vm = args.Item as ComparisonItemViewModel;
            if (vm.IsOnMyList)
            {
                image.Width = 316;
                image.Height = 400;
                if (image.Parent != null)
                    (image.Parent as FrameworkElement).Margin = new Thickness(115, -40, 0, 0);
            }
            else
            {
                image.Width = 380;
                image.Height = 560;
                if (image.Parent != null)
                    (image.Parent as FrameworkElement).Margin = new Thickness(0, -20, 0, 0);
            }


            UpdateParalax(image, args.ItemIndex);
        }

        private void UpdateParalax(FrameworkElement image, int index)
        {
            Visual visual = ElementCompositionPreview.GetElementVisual(image);
            visual.Size = new Vector2(380, 270);

            if (!(_parallaxExpression is null))
            {
                var row = index / _lastColumns;
                var offset = row * visual.Size.Y;
                _parallaxExpression.SetScalarParameter("StartOffset", offset);
                visual.StopAnimation("Offset.Y");
                visual.StartAnimation("Offset.Y", _parallaxExpression);
            }
        }

        private void UpdateVisualStates()
        {
            if (_isWide)
                VisualStateManager.GoToState(this, "Wide", true);
            else
                VisualStateManager.GoToState(this, "Narrow", true);
        }

        private void GridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            _viewModel.NavigateDetailsCommand.Execute(e.ClickedItem as ComparisonItemViewModel);
        }
    }
}
