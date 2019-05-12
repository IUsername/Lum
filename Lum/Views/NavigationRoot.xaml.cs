using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Lum.Services;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Lum.Views
{
    public sealed partial class NavigationRoot : Page
    {
        private Type _currentPageType;
        private bool _isFirstLoad = true;
        private INavigationService _navService;

        public NavigationRoot()
        {
            Instance = this;
            InitializeComponent();

            var nav = SystemNavigationManager.GetForCurrentView();
            nav.BackRequested += Nav_BackRequested;
        }

        public static NavigationRoot Instance { get; private set; }

        public Frame AppFrame => AppNavFrame;

        private void Nav_BackRequested(object sender, BackRequestedEventArgs e)
        {
            var _ = _navService.GoBackAsync();
            e.Handled = true;
        }

        private void AppNavFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var sourcePageType = e.SourcePageType;
            SelectNav(sourcePageType);
            _currentPageType = sourcePageType;
        }

        private void SelectNav(Type sourcePageType)
        {
            // This is not easily extensible - look into tracking in the navigation service by an attribute.
            switch (sourcePageType)
            {
                case Type _ when sourcePageType == typeof(Dashboard):
                    ((NavigationViewItem) NavView.MenuItems[0]).IsSelected = true;
                    break;
                case Type _ when sourcePageType == typeof(Desktop):
                    ((NavigationViewItem) NavView.MenuItems[1]).IsSelected = true;
                    break;
                default:
                    ((NavigationViewItem) NavView.MenuItems[2]).IsSelected = true;
                    break;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isFirstLoad)
            {
                SelectNav(_currentPageType);
                _isFirstLoad = false;
            }

            ViewModeService.Instance.Register(NavView, AppNavFrame);
        }

        public void InitializeNavigationService(INavigationService navService)
        {
            _navService = navService;
            _navService.Navigated += NavServiceOnNavigated;
        }

        private void NavServiceOnNavigated(object sender, EventArgs e)
        {
            var _ = DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                var nav = SystemNavigationManager.GetForCurrentView();
                nav.AppViewBackButtonVisibility = _navService.CanGoBack
                    ? AppViewBackButtonVisibility.Visible
                    : AppViewBackButtonVisibility.Collapsed;
            });
        }

        private void NavView_OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var navOptions = new FrameNavigationOptions
            {
                TransitionInfoOverride = args.RecommendedNavigationTransitionInfo,
                IsNavigationStackEnabled = sender.PaneDisplayMode != NavigationViewPaneDisplayMode.Top
            };


            if (args.SelectedItemContainer == DashboardItem)
            {
                _navService.NavigateToDashboard(navOptions);
            }
            else if (args.SelectedItemContainer == DesktopItem)
            {
                _navService.NavigateToDesktop(navOptions);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModeService.Instance.UnRegister();
        }
    }
}