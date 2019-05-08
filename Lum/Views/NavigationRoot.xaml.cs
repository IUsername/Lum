using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Lum.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NavigationRoot : Page
    {
        public static NavigationRoot Instance { get; private set; }

        public NavigationRoot()
        {
            Instance = this;
            this.InitializeComponent();
        }

        public Frame AppFrame => AppNavFrame;

        private void AppNavFrame_Navigated(object sender, NavigationEventArgs e)
        {
            //var _ = DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            //{
            //    var nav = SystemNavigationManager.GetForCurrentView();
            //    nav.AppViewBackButtonVisibility = _navigationService.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            //});
        }
    }
}
