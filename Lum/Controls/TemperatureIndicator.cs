using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Lum
{
    [TemplatePart(Name = IndicatorPartName, Type = typeof(Path))]
    [TemplatePart(Name = TempTextPartName, Type = typeof(TextBlock))]
    public sealed class TemperatureIndicator : RangeBase
    {
        private const string IndicatorPartName = "PART_Indicator";
        private const string TempTextPartName = "PART_TempText";

        public TemperatureIndicator()
        {
            this.DefaultStyleKey = typeof(TemperatureIndicator);
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);

            if (GetTemplateChild(IndicatorPartName) is Path indicator)
            {
                var t = UnitValue(Minimum, Maximum, newValue);
                var endY = Lerp(19, 2.658, t);
                var lg = new LineGeometry
                {
                    StartPoint = new Point(4.5, 24),
                    EndPoint = new Point(4.5, endY)
                };
                indicator.Data = lg;
            }

            if(GetTemplateChild(TempTextPartName) is TextBlock text)
            {
                text.Text = $"{newValue:F0}°C";
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OnValueChanged(Value, Value);
        }

        private static double Lerp(double v0, double v1, double t) => (1 - t) * v0 + t * v1;

        private static double Clamp(double min, double max, double v) => Math.Min(Math.Max(min, v), max);

        private static double UnitValue(double min, double max, double v)
        {
            return (Clamp(min, max, v) - min) / (max - min);
        }
    }
}
