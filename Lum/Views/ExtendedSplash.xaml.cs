using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Lum.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtendedSplash : Page
    {
        private readonly SplashScreen _splash; // Variable to hold the splash screen object.
        internal bool Dismissed; // Variable to track splash screen dismissal status.
        //internal Frame rootFrame;
        internal Rect SplashImageRect; // Rect to store splash screen image coordinates.
        private readonly TaskCompletionSource<object> _tcs;


        public ExtendedSplash(SplashScreen splashScreen)
        {
            InitializeComponent();

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

            _tcs = new TaskCompletionSource<object>();
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

        public void DismissExtendedSplash()
        {
            _tcs.SetResult(null);
        }

        private void ExtendedSplash_OnResize(object sender, WindowSizeChangedEventArgs e)
        {
            // Safely update the extended splash screen image coordinates. This function will be fired in response to snapping, unsnapping, rotation, etc...
            if (_splash != null)
            {
                // Update the coordinates of the splash screen image.
                SplashImageRect = _splash.ImageLocation;
                PositionImage();
            }
        }

        private void ExtendedSplash_OnLoaded(object sender, RoutedEventArgs e)
        {
            var playAction = Logo.PlayAsync(0, 1, false);
            playAction.Completed = (info, status) => DismissExtendedSplash();
        }

        public Task RunAsync()
        {
            return _tcs.Task;
        }
    }
}