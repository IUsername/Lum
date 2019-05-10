using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Autofac;
using Lum.Views;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Lum.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IComponentContext _iocContext;
        private Type _current;
        private object _currentParameter;
        private bool _isNavigating;

        public NavigationService(IFrameAdapter frameAdapter, IComponentContext iocContext)
        {
            _iocContext = iocContext;
            Frame = frameAdapter;
            Frame.Navigated += Frame_Navigated;
            PageViewModels = new Dictionary<Type, NavigatedToViewModelDelegate>();
        }

        private Dictionary<Type, NavigatedToViewModelDelegate> PageViewModels { get; }

        private IFrameAdapter Frame { get; }

        public bool CanGoBack => Frame.CanGoBack;

        public bool IsNavigating
        {
            get => _isNavigating;
            private set
            {
                if (value == _isNavigating)
                {
                    return;
                }

                _isNavigating = value;
                IsNavigatingChanged?.Invoke(this, _isNavigating);
                if (!_isNavigating)
                {
                    Navigated?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public async Task GoBackAsync()
        {
            if (Frame.CanGoBack)
            {
                IsNavigating = true;
                var _ = await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    Frame.GoBack();
                    return Frame.Content as Page;
                });
            }
        }

        public void RegisterPageViewModel<TPage, TViewModel>() where TViewModel : class
        {
            async Task NavigatedTo(object page, object parameter, NavigationEventArgs args)
            {
                if (!(page is IPageWithViewModel<TViewModel> pageWithViewModel))
                {
                    return;
                }

                pageWithViewModel.ViewModel = _iocContext.Resolve<TViewModel>();
                if (pageWithViewModel.ViewModel is INavigableTo navigableViewModel)
                {
                    await navigableViewModel.NavigatedTo(args.NavigationMode, parameter);
                }

                pageWithViewModel.UpdateBindings();
            }

            PageViewModels[typeof(TPage)] = NavigatedTo;
        }

        public Task NavigateToDashboard(FrameNavigationOptions navOptions) => NavigateToPage<Dashboard>(navOptions);

        public Task NavigateToDesktop(FrameNavigationOptions navOptions) => NavigateToPage<Desktop>(navOptions);

        public event EventHandler<bool> IsNavigatingChanged;

        public event EventHandler Navigated;

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            IsNavigating = false;
            if (PageViewModels.ContainsKey(e.SourcePageType))
            {
                var loadViewModel = PageViewModels[e.SourcePageType];
                var _ = loadViewModel(e.Content, e.Parameter, e);
            }
            _current = e.SourcePageType;
            _currentParameter = e.Parameter;
        }

        private Task NavigateToPage<TPage>(FrameNavigationOptions navOptions) =>
            NavigateToPage<TPage>(null, navOptions);

        private async Task NavigateToPage<TPage>(object parameter, FrameNavigationOptions navOptions)
        {
            if (IsNavigating)
            {
                return;
            }

            var type = typeof(TPage);
            if (type == _current && parameter == _currentParameter)
            {
                return;
            }

           

            IsNavigating = true;
            await DispatcherHelper.ExecuteOnUIThreadAsync(() => Frame.Navigate(type, parameter, navOptions));
        }

        private protected delegate Task NavigatedToViewModelDelegate(object page,
                                                                     object parameter,
                                                                     NavigationEventArgs navArgs);
    }
}