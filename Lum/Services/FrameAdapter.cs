using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Lum.Utilities;

namespace Lum.Services
{
    public sealed class FrameAdapter : IFrameAdapter
    {
        private readonly Frame _internalFrame;

        public FrameAdapter(Frame frame)
        {
            _internalFrame = frame;
        }

        public event NavigatedEventHandler Navigated
        {
            add => _internalFrame.Navigated += value;
            remove => _internalFrame.Navigated -= value;
        }

        public event NavigatingCancelEventHandler Navigating
        {
            add => _internalFrame.Navigating += value;
            remove => _internalFrame.Navigating -= value;
        }

        public event NavigationFailedEventHandler NavigationFailed
        {
            add => _internalFrame.NavigationFailed += value;
            remove => _internalFrame.NavigationFailed -= value;
        }

        public event NavigationStoppedEventHandler NavigationStopped
        {
            add => _internalFrame.NavigationStopped += value;
            remove => _internalFrame.NavigationStopped -= value;
        }

        public object Content => _internalFrame.Content;

        public bool CanGoBack => _internalFrame.CanGoBack;

        public bool CanGoForward => _internalFrame.CanGoForward;

        public string GetNavigationState() => _internalFrame.GetNavigationState();

        public void GoBack()
        {
            _internalFrame.GoBack();
        }

        public void GoForward()
        {
            _internalFrame.GoForward();
        }

        public bool Navigate(Type sourcePageType, object parameter, FrameNavigationOptions navOptions)
        {
            return _internalFrame.NavigateToType(sourcePageType, parameter, navOptions);
            //return _internalFrame.NavigateWithFadeOutgoing(parameter, sourcePageType);
        }

        public void SetNavigationState(string navigationState)
        {
            _internalFrame.SetNavigationState(navigationState);
        }
    }
}