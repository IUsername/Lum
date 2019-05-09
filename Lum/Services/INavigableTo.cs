using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Lum.Services
{
    internal interface INavigableTo
    {
        Task NavigatedTo(NavigationMode navigationMode, object parameter);
    }
}