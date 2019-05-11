using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using Lum.Utilities;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Lum
{
    [TemplatePart(Name = MarkerPartName, Type = typeof(Path))]
    [TemplatePart(Name = SlotPartName, Type = typeof(Path))]
    [TemplatePart(Name = SlotBorderPartName, Type = typeof(Path))]
    [TemplatePart(Name = PercentTextPartName, Type = typeof(TextBlock))]
    [TemplatePart(Name = RootPartName, Type = typeof(Grid))]
    public sealed class RadialIndicator : RangeBase
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
            nameof(EasingFunction), typeof(EasingFunctionBase), typeof(RadialIndicator), new PropertyMetadata(default(EasingFunctionBase)));

        public static readonly DependencyProperty EasedValueProperty = DependencyProperty.Register(
            nameof(EasedValue), typeof(double), typeof(RadialIndicator), new PropertyMetadata(default(double), OnEasedValueChanged));

        private readonly Storyboard _storyboard = new Storyboard();
        private DoubleAnimation _timeline;

        private static void OnEasedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var indicator = (RadialIndicator) d;
            indicator.AnimateToNewValue((double)e.NewValue);
           
        }

        private void AnimateToNewValue(double newValue)
        {
            if (_timeline == null)
            {
                var timeline = new DoubleAnimation
                {
                    To = newValue,
                    Duration = TimeSpan.FromMilliseconds(800),
                    EnableDependentAnimation = true
                };
                if (EasingFunction == null)
                {
                    var elastic = new ElasticEase {Oscillations = 1, Springiness = 3};
                    timeline.EasingFunction = elastic;
                }
                else
                {
                    timeline.EasingFunction = EasingFunction;
                }

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

        public RadialIndicator()
        {
            DefaultStyleKey = typeof(RadialIndicator);

            Minimum = 0d;
            Maximum = 1d;
        }

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            OnValueChanged(this);
            base.OnValueChanged(oldValue, newValue);
        }

        private static void OnValueChanged(DependencyObject d)
        {
            var indicator = (RadialIndicator) d;
            if (double.IsNaN(indicator.Value))
            {
                return;
            }

            if (indicator.GetTemplateChild(MarkerPartName) is Path marker)
            {
                var startAngle = ValueToAngle(0);
                var endAngle = ValueToAngle(indicator.Value);
                marker.Data = CreateArcPathGeometry(startAngle, endAngle);

                if (indicator.GetTemplateChild(RootPartName) is Grid root)
                {
                    if (root.Lights.Count > 0 && root.Lights[0] is RedSpotLight light)
                    {
                        var endPoint = AngleToPoint(endAngle);
                        light.Position(new Vector3((float) endPoint.X, (float) endPoint.Y, 60));
                    }
                }
            }

            if (indicator.GetTemplateChild(PercentTextPartName) is TextBlock text)
            {
                if (text.Inlines.Count != 2)
                {
                    var run = new Run
                    {
                        FontSize = 24,
                        Text = $"{indicator.Value * 100:F0}",
                        FontWeight = FontWeights.SemiBold
                    };
                    var symbol = new Run
                    {
                        Text = "%",
                        FontSize = 12,
                        FontWeight = FontWeights.Thin
                    };
                    text.Inlines.Add(run);
                    text.Inlines.Add(symbol);
                }
                else
                {
                    ((Run) text.Inlines[0]).Text = $"{indicator.Value * 100:F0}";
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
                if (root.Lights.Count > 0 && root.Lights[0] is RedSpotLight light)
                {
                    light.CoordinateSpace(root.GetVisual());
                }
            }

            OnValueChanged(this);
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
            return new Point(centerX + Math.Sin(rad) * size.Width, centerY - Math.Cos(rad) * size.Height);
        }

        private static double ValueToAngle(double value) => Lerp(-135, 135, Clamp(value));

        private static double Lerp(double v0, double v1, double t) => (1 - t) * v0 + t * v1;

        private static double Clamp(double v) => Math.Min(Math.Max(0d, v), 1d);
    }
}