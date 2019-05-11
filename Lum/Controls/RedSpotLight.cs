using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Lum
{
    public sealed class RedSpotLight : XamlLight
    {
        public static readonly DependencyProperty IsTargetProperty = DependencyProperty.RegisterAttached(
            "IsTarget", typeof(bool), typeof(RedSpotLight), new PropertyMetadata(default(bool), OnIsTargetChanged));

        private static void OnIsTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isAdding = (bool) e.NewValue;
            if (isAdding)
            {
                switch (d)
                {
                    case UIElement element:
                        XamlLight.AddTargetElement(GetIdStatic(), element);
                        break;
                    case Brush brush:
                        XamlLight.AddTargetBrush(GetIdStatic(), brush);
                        break;
                }
            }
            else
            {
                switch (d)
                {
                    case UIElement element:
                        XamlLight.RemoveTargetElement(GetIdStatic(), element);
                        break;
                    case Brush brush:
                        XamlLight.RemoveTargetBrush(GetIdStatic(), brush);
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
            spotLight.InnerConeColor = Color.FromArgb(0xFF, 0x9F, 0x0D, 0x0D);
            spotLight.OuterConeColor = Colors.DeepSkyBlue;
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

        public void Position(Vector3 offset)
        {
            if (CompositionLight is SpotLight spot)
            {
                spot.Offset = offset;
            }
        }

        public void CoordinateSpace(Visual visual)
        {
            if (CompositionLight is SpotLight spot)
            {
                spot.CoordinateSpace = visual;
            }
        }

        private static string GetIdStatic()
        {
            return typeof(RedSpotLight).FullName;
        }

        protected override string GetId() => GetIdStatic();

        public static void SetIsTarget(DependencyObject target, bool value)
        {
            target.SetValue(IsTargetProperty, value);
        }

        public static bool GetIsTarget(DependencyObject target)
        {
            return (bool) target.GetValue(IsTargetProperty);
        }
    }
}