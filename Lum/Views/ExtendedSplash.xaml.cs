using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Lum.Views
{
    public sealed partial class ExtendedSplash : Page
    {
        private readonly SplashScreen _splash;
        private readonly TaskCompletionSource<object> _tcs;

        internal bool Dismissed; // Track if launch splash screen has completed. Not currently used.

        internal Rect SplashImageRect; // Rect of splash screen image;


        public ExtendedSplash(SplashScreen splashScreen)
        {
            InitializeComponent();
            _tcs = new TaskCompletionSource<object>();

            // Listen for window resize events to reposition the extended splash screen image accordingly.
            // This is important to ensure that the extended splash screen is formatted properly in response to snapping, unsnapping, rotation, etc...
            Window.Current.SizeChanged += ExtendedSplash_OnResize;

            _splash = splashScreen;

            if (_splash != null)
            {
                // Register an event handler to be executed when the splash screen has been dismissed.
                _splash.Dismissed += DismissedEventHandler;

                // Retrieve the window coordinates of the splash screen image.
                SplashImageRect = _splash.ImageLocation;
                PositionImage();
            }
        }

        private void PositionImage()
        {
            Logo.SetValue(Canvas.LeftProperty, SplashImageRect.X);
            Logo.SetValue(Canvas.TopProperty, SplashImageRect.Y);
            Logo.Width = SplashImageRect.Width;
            Logo.Height = SplashImageRect.Height;
        }

        private void DismissedEventHandler(SplashScreen sender, object args)
        {
            Dismissed = true;
        }

        private void ExtendedSplash_OnResize(object sender, WindowSizeChangedEventArgs e)
        {
            if (_splash != null)
            {
                // Update the coordinates of the splash screen image.
                SplashImageRect = _splash.ImageLocation;
                PositionImage();
            }
        }

        private void ExtendedSplash_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Play lottie animation once [0..1].
            var playAction = Logo.PlayAsync(0, 1, false);
            playAction.Completed = (info, status) => _tcs.SetResult(null);
        }

        public Task RunAsync() => _tcs.Task;
    }
}