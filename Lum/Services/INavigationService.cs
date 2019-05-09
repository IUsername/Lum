using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Lum.Services
{
    public interface INavigationService
    {
        event EventHandler<bool> IsNavigatingChanged;

        event EventHandler Navigated;

        bool CanGoBack { get; }

        bool IsNavigating { get; }
      
        Task GoBackAsync();

        void RegisterPageViewModel<TPage, TViewModel>() where TViewModel : class;
        Task NavigateToDashboard(FrameNavigationOptions navOptions);
        Task NavigateToDesktop(FrameNavigationOptions navOptions);
    }
}