using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Lum.Services
{
    public class ViewModeService
    {
        public static readonly ViewModeService Instance = new ViewModeService();
        private NavigationView _navView;
        private Frame _appNavFrame;

        public void Register(NavigationView navView, Frame appNavFrame)
        {
            _navView = navView;
            _appNavFrame = appNavFrame;
        }

        public void UnRegister()
        {
            _navView = null;
            _appNavFrame = null;
        }
    }
}
