using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Autofac;
using Lum.Services;
using Lum.ViewModels;
using Lum.Views;

namespace Lum
{
    /// <summary>
    ///     Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private IContainer _container;
        private NavigationRoot _rootPage;

        /// <summary>
        ///     Initializes the singleton application object.  This is the first line of authored code
        ///     executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        /// <summary>
        ///     Invoked when the application is launched normally by the end user.  Other entry points
        ///     will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            //var rootFrame = Window.Current.Content as Frame;

            //// Do not repeat app initialization when the Window already has content,
            //// just ensure that the window is active
            //if (rootFrame == null)
            //{
            //    // Create a Frame to act as the navigation context and navigate to the first page
            //    rootFrame = new Frame();

            //    rootFrame.NavigationFailed += OnNavigationFailed;

            //    if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            //    {
            //        //TODO: Load state from previously suspended application
            //    }

            //    // Place the frame in the current Window
            //    Window.Current.Content = rootFrame;
            //}

            //if (e.PrelaunchActivated == false)
            //{
            //    if (rootFrame.Content == null)
            //    {
            //        // When the navigation stack isn't restored navigate to the first page,
            //        // configuring the new page by passing required information as a navigation
            //        // parameter
            //        rootFrame.Navigate(typeof(Views.NavigationRoot), e.Arguments);
            //    }

            //    // Ensure the current window is active
            //    Window.Current.Activate();

            //    // Extend acrylic
            //    ExtendAcrylicIntoTitleBar();
            //}

            await InitializeAsync();

            InitWindow(e.SplashScreen, e.PrelaunchActivated);

            await StartupAsync();
        }

        private async Task StartupAsync()
        {
          
        }

        private static void ExtendAcrylicIntoTitleBar()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = Colors.White;
        }

        /// <summary>
        ///     Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        ///     Invoked when application execution is being suspended.  Application state is saved
        ///     without knowing whether the application will be terminated or resumed with the contents
        ///     of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await InitializeAsync();
            InitWindow(null);

            if (args.Kind == ActivationKind.Protocol)
            {
                Window.Current.Activate();

                // Tasks after activation
                await StartupAsync();
            }
        }

        private async Task InitializeAsync()
        {
            

            ExtendAcrylicIntoTitleBar();
        }

        private async void InitWindow(SplashScreen splash, bool skipWindowCreation = false)
        {
            
            _rootPage = Window.Current.Content as NavigationRoot;
            var shouldInit = _rootPage == null && !skipWindowCreation;

            if (shouldInit)
            {
                var extSplash = new ExtendedSplash(splash);
                Window.Current.Content = extSplash;
                Window.Current.Activate();

                _rootPage = new NavigationRoot();
                var adapter = new FrameAdapter(_rootPage.AppFrame);
                adapter.NavigationFailed += OnNavigationFailed;

                var builder = new ContainerBuilder();
                builder.RegisterInstance(adapter).AsImplementedInterfaces();

                builder.RegisterType<DashboardViewModel>();
                builder.RegisterType<DesktopViewModel>();
                builder.RegisterType<NavigationService>().AsImplementedInterfaces().SingleInstance();

                _container = builder.Build();

                var navService = _container.Resolve<INavigationService>();
                navService.RegisterPageViewModel<Dashboard, DashboardViewModel>();
                navService.RegisterPageViewModel<Desktop, DesktopViewModel>();
                _rootPage.InitializeNavigationService(navService);

                await navService.NavigateToDashboard(new FrameNavigationOptions());

                await extSplash.RunAsync().ConfigureAwait(true);

                Window.Current.Content = _rootPage;
                Window.Current.Activate();
            }

            //GetFrame().Navigate(typeof(Views.Dashboard));
        }

        public NavigationRoot GetNavigationRoot()
        {
            switch (Window.Current.Content)
            {
                case NavigationRoot _:
                    return (NavigationRoot) Window.Current.Content;
                case Frame _:
                    return ((Frame) Window.Current.Content).Content as NavigationRoot;
                default:
                    throw new Exception("Window content is an unknown type.");
            }
        }

        public Frame GetFrame() => GetNavigationRoot().AppFrame;
    }
}