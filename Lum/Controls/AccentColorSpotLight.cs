using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Lum
{
    public sealed class AccentColorSpotLight : XamlLight
    {
        public static readonly DependencyProperty IsTargetProperty = DependencyProperty.RegisterAttached(
            "IsTarget", typeof(bool), typeof(AccentColorSpotLight), new PropertyMetadata(default(bool), OnIsTargetChanged));

        private static void OnIsTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isAdding = (bool) e.NewValue;
            if (isAdding)
            {
                switch (d)
                {
                    case UIElement element:
                        AddTargetElement(GetIdStatic(), element);
                        break;
                    case Brush brush:
                        AddTargetBrush(GetIdStatic(), brush);
                        break;
                }
            }
            else
            {
                switch (d)
                {
                    case UIElement element:
                        RemoveTargetElement(GetIdStatic(), element);
                        break;
                    case Brush brush:
                        RemoveTargetBrush(GetIdStatic(), brush);
                        break;
                }
            }
        }

        protected override void OnConnected(UIElement newElement)
        {
            if (CompositionLight != null)
            {
                return;
            }

            var spotLight = Window.Current.Compositor.CreateSpotLight();
            spotLight.InnerConeColor = (Color) Application.Current.Resources["SystemAccentColor"];
            spotLight.OuterConeColor = Colors.LightBlue;
            spotLight.InnerConeAngleInDegrees = 1;
            spotLight.OuterConeAngleInDegrees = 90;
            spotLight.InnerConeIntensity = 2;
            spotLight.OuterConeIntensity = 2f;
            CompositionLight = spotLight;
        }

        protected override void OnDisconnected(UIElement oldElement)
        {
            if (CompositionLight == null)
            {
                return;
            }

            CompositionLight.Dispose();
            CompositionLight = null;
        }

        public void SetPosition(Vector3 offset)
        {
            if (CompositionLight is SpotLight spot)
            {
                spot.Offset = offset;
            }
        }

        public void SetCoordinateSpace(Visual visual)
        {
            if (CompositionLight is SpotLight spot)
            {
                spot.CoordinateSpace = visual;
            }
        }

        private static string GetIdStatic() => typeof(AccentColorSpotLight).FullName;

        protected override string GetId() => GetIdStatic();

        public static void SetIsTarget(DependencyObject target, bool value)
        {
            target.SetValue(IsTargetProperty, value);
        }

        public static bool GetIsTarget(DependencyObject target) => (bool) target.GetValue(IsTargetProperty);
    }
}