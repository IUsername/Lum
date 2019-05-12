using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using Lum.Utilities;

namespace Lum
{
    [TemplatePart(Name = MarkerPartName, Type = typeof(Path))]
    [TemplatePart(Name = SlotPartName, Type = typeof(Path))]
    [TemplatePart(Name = SlotBorderPartName, Type = typeof(Path))]
    [TemplatePart(Name = PercentTextPartName, Type = typeof(TextBlock))]
    [TemplatePart(Name = RootPartName, Type = typeof(Grid))]
    public sealed class RadialIndicator : Control
    {
        private const string MarkerPartName = "PART_Marker";
        private const string SlotPartName = "PART_Slot";
        private const string SlotBorderPartName = "PART_Slot_Border";
        private const string PercentTextPartName = "PART_PercentText";
        private const string RootPartName = "PART_Root";

        private const double DegToRad = Math.PI / 180;

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title), typeof(string), typeof(RadialIndicator), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register(
            nameof(EasingFunction), typeof(EasingFunctionBase), typeof(RadialIndicator),
            new PropertyMetadata(default(EasingFunctionBase)));

        public static readonly DependencyProperty EasedValueProperty = DependencyProperty.Register(
            nameof(EasedValue), typeof(double), typeof(RadialIndicator),
            new PropertyMetadata(default(double), OnEasedValueChanged));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(double), typeof(RadialIndicator),
            new PropertyMetadata(default(double), OnValueChanged));

        public static readonly DependencyProperty EasingDurationProperty = DependencyProperty.Register(
            nameof(EasingDuration), typeof(TimeSpan), typeof(RadialIndicator),
            new PropertyMetadata(TimeSpan.FromMilliseconds(800)));

        public static readonly DependencyProperty PercentTextBrushProperty = DependencyProperty.Register(
            nameof(PercentTextBrush), typeof(Brush), typeof(RadialIndicator),
            new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public static readonly DependencyProperty SlotBackgroundBrushProperty = DependencyProperty.Register(
            nameof(SlotBackgroundBrush), typeof(Brush), typeof(RadialIndicator),
            new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty LightColorProperty = DependencyProperty.Register(
            nameof(LightColor), typeof(Color), typeof(RadialIndicator), new PropertyMetadata(Colors.White));

        public Color LightColor
        {
            get => (Color) GetValue(LightColorProperty);
            set => SetValue(LightColorProperty, value);
        }

        private readonly Storyboard _storyboard = new Storyboard();
        private DoubleAnimation _timeline;

        public RadialIndicator()
        {
            DefaultStyleKey = typeof(RadialIndicator);
        }

        public Brush SlotBackgroundBrush
        {
            get => (Brush) GetValue(SlotBackgroundBrushProperty);
            set => SetValue(SlotBackgroundBrushProperty, value);
        }

        public Brush PercentTextBrush
        {
            get => (Brush) GetValue(PercentTextBrushProperty);
            set => SetValue(PercentTextBrushProperty, value);
        }

        public TimeSpan EasingDuration
        {
            get => (TimeSpan) GetValue(EasingDurationProperty);
            set => SetValue(EasingDurationProperty, value);
        }

        public double Value
        {
            get => (double) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public double EasedValue
        {
            get => (double) GetValue(EasedValueProperty);
            set => SetValue(EasedValueProperty, value);
        }

        public EasingFunctionBase EasingFunction
        {
            get => (EasingFunctionBase) GetValue(EasingFunctionProperty);
            set => SetValue(EasingFunctionProperty, value);
        }

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        private static void OnEasedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var indicator = (RadialIndicator) d;
            indicator.AnimateToNewValue((double) e.NewValue);
        }

        private void AnimateToNewValue(double newValue)
        {
            if (_timeline == null)
            {
                var timeline = new DoubleAnimation
                {
                    To = newValue,
                    Duration = EasingDuration,
                    EnableDependentAnimation = true,
                    EasingFunction = EasingFunction ?? new QuadraticEase {EasingMode = EasingMode.EaseInOut}
                };
                _storyboard.Children.Add(timeline);
                Storyboard.SetTarget(timeline, this);
                Storyboard.SetTargetProperty(timeline, nameof(Value));
                _timeline = timeline;
                _storyboard.Begin();
            }
            else
            {
                _timeline.To = newValue;
                _storyboard.Begin();
            }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var indicator = (RadialIndicator) d;
            var newValue = (double) e.NewValue;
            if (double.IsNaN(newValue))
            {
                return;
            }

            if (indicator.GetTemplateChild(MarkerPartName) is Path marker)
            {
                var startAngle = ValueToAngle(0);
                var endAngle = ValueToAngle(newValue);
                marker.Data = CreateArcPathGeometry(startAngle, endAngle);

                if (indicator.GetTemplateChild(RootPartName) is Grid root)
                {
                    if (root.Lights.Count > 0 && root.Lights[0] is AccentColorSpotLight light)
                    {
                        var endPoint = AngleToPoint(endAngle);
                        light.SetPosition(new Vector3((float) endPoint.X, (float) endPoint.Y, 40));
                    }
                }
            }

            if (indicator.GetTemplateChild(PercentTextPartName) is TextBlock text)
            {
                // Necessary to allow Run of different font sizes on the same text block line.
                if (text.Inlines.Count != 2)
                {
                    var value = new Run
                    {
                        FontSize = 24,
                        Text = $"{newValue * 100:F0}",
                        FontWeight = FontWeights.SemiBold
                    };
                    var symbol = new Run
                    {
                        Text = "%",
                        FontSize = 12,
                        FontWeight = FontWeights.Thin
                    };
                    text.Inlines.Add(value);
                    text.Inlines.Add(symbol);
                }
                else
                {
                    ((Run) text.Inlines[0]).Text = $"{newValue * 100:F0}";
                }
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild(SlotPartName) is Path slot)
            {
                var startAngle = ValueToAngle(0);
                var endAngle = ValueToAngle(1);

                slot.Data = CreateArcPathGeometry(startAngle, endAngle);

                if (GetTemplateChild(SlotBorderPartName) is Path slotBorder)
                {
                    slotBorder.Data = CreateArcPathGeometry(startAngle, endAngle);
                }
            }

            if (GetTemplateChild(RootPartName) is Grid root)
            {
                if (root.Lights.Count > 0 && root.Lights[0] is AccentColorSpotLight light)
                {
                    light.SetCoordinateSpace(root.GetVisual());
                }
            }
        }

        private static PathGeometry CreateArcPathGeometry(double startAngle, double endAngle)
        {
            var pg = new PathGeometry();
            var pf = new PathFigure {IsClosed = false, StartPoint = AngleToPoint(startAngle)};
            var seg = new ArcSegment
            {
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = endAngle - startAngle > 180d,
                Size = GetArcSize(),
                Point = AngleToPoint(endAngle)
            };
            pf.Segments.Add(seg);
            pg.Figures.Add(pf);
            return pg;
        }

        private static Size GetArcSize() => new Size(40, 40);

        private static Point AngleToPoint(double angle)
        {
            const int centerX = 50;
            const int centerY = 50;
            var size = GetArcSize();
            var rad = DegToRad * angle;
            return new Point(centerX + Math.Sin(rad) * size.Width,
                             centerY - Math.Cos(rad) * size.Height);
        }

        private static double ValueToAngle(double value) => Tools.Lerp(-135, 135, Tools.Clamp(0, 1, value));
    }
}