using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Lum
{
    [TemplatePart(Name = IndicatorPartName, Type = typeof(Path))]
    [TemplatePart(Name = TempTextPartName, Type = typeof(TextBlock))]
    public sealed class TemperatureIndicator : RangeBase
    {
        private const string IndicatorPartName = "PART_Indicator";
        private const string TempTextPartName = "PART_TempText";

        public static readonly DependencyProperty IndicatorBrushProperty = DependencyProperty.Register(
            nameof(IndicatorBrush), typeof(Brush), typeof(TemperatureIndicator),
            new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public static readonly DependencyProperty SlotBackgroundBrushProperty = DependencyProperty.Register(
            nameof(SlotBackgroundBrush), typeof(Brush), typeof(TemperatureIndicator),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty TemperatureFormatStringProperty = DependencyProperty.Register(
            nameof(TemperatureFormatString), typeof(string), typeof(TemperatureIndicator), new PropertyMetadata("{0:F0}°C"));

        public string TemperatureFormatString
        {
            get => (string) GetValue(TemperatureFormatStringProperty);
            set => SetValue(TemperatureFormatStringProperty, value);
        }

        public TemperatureIndicator()
        {
            DefaultStyleKey = typeof(TemperatureIndicator);
        }

        public Brush SlotBackgroundBrush
        {
            get => (Brush) GetValue(SlotBackgroundBrushProperty);
            set => SetValue(SlotBackgroundBrushProperty, value);
        }

        public Brush IndicatorBrush
        {
            get => (Brush) GetValue(IndicatorBrushProperty);
            set => SetValue(IndicatorBrushProperty, value);
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);

            if (GetTemplateChild(IndicatorPartName) is Path indicator)
            {
                // These values assume a specific path defined in the default control style.
                var t = Tools.UnitValue(Minimum, Maximum, newValue);
                var endY = Tools.Lerp(19, 2.658, t);
                var lg = new LineGeometry
                {
                    StartPoint = new Point(4.5, 24),
                    EndPoint = new Point(4.5, endY)
                };
                indicator.Data = lg;
            }

            if (GetTemplateChild(TempTextPartName) is TextBlock text)
            {
                text.Text = string.Format(TemperatureFormatString, newValue);
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OnValueChanged(Value, Value);
        }
    }
}