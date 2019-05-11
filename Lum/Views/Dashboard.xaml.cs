using Windows.UI.Xaml.Controls;
using Lum.Services;
using Lum.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Lum.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Dashboard : Page, IPageWithViewModel<DashboardViewModel>
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        public DashboardViewModel ViewModel { get; set; }

        public void UpdateBindings()
        {
            DataContext = ViewModel;
        }
    }
}