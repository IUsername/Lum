﻿using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace Lum.Utilities
{
    public static class VisualHelpers
    {
        public static Visual GetVisual(this UIElement element) => ElementCompositionPreview.GetElementVisual(element);

        public static CompositionCommitBatch ApplyImplicitAnimation(this Visual target, TimeSpan duration)
        {
            var myBatch = target.Compositor.GetCommitBatch(CompositionBatchTypes.Animation);
            target.Opacity = 0.0f;
            var implicitAnimationCollection = target.Compositor.CreateImplicitAnimationCollection();

            implicitAnimationCollection[nameof(Visual.Opacity)] = CreateOpacityAnimation(target.Compositor, duration);
            target.ImplicitAnimations = implicitAnimationCollection;
            return myBatch;
        }

        public static KeyFrameAnimation CreateOpacityAnimation(Compositor compositor, TimeSpan duration)
        {
            var kf = compositor.CreateScalarKeyFrameAnimation();
            kf.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            kf.Duration = duration;
            kf.Target = "Opacity";
            return kf;
        }

        public static void SetSize(this Visual v, FrameworkElement element)
        {
            v.Size = new Vector2((float) element.ActualWidth, (float) element.ActualHeight);
        }

        public static void FadeVisual(this Visual v, double seconds)
        {
            var fadeAnimation = CreateImplicitFadeAnimation(seconds);
            v.ImplicitAnimations = Window.Current.Compositor.CreateImplicitAnimationCollection();
            v.ImplicitAnimations.Add(nameof(Visual.Opacity), fadeAnimation);
        }

        public static ICompositionAnimationBase CreateOpacityAnimation(double seconds, float finalValue)
        {
            var animation = Window.Current.Compositor.CreateScalarKeyFrameAnimation();
            animation.Target = nameof(Visual.Opacity);
            animation.Duration = TimeSpan.FromSeconds(seconds);
            animation.InsertKeyFrame(1, finalValue);
            return animation;
        }

        public static ICompositionAnimationBase CreateAnimationGroup(CompositionAnimation listContentShowAnimations,
                                                                     ScalarKeyFrameAnimation listContentOpacityAnimations)
        {
            var group = Window.Current.Compositor.CreateAnimationGroup();
            group.Add(listContentShowAnimations);
            group.Add(listContentOpacityAnimations);
            return group;
        }

        public static void EnableLayoutImplicitAnimations(this UIElement element, TimeSpan t)
        {
            var result = element.GetVisual();
            var compositor = result.Compositor;

            var elementImplicitAnimation = compositor.CreateImplicitAnimationCollection();
            elementImplicitAnimation[nameof(Visual.Offset)] = CreateOffsetAnimation(compositor, t);

            result.ImplicitAnimations = elementImplicitAnimation;
        }

        private static CompositionAnimation CreateImplicitFadeAnimation(double seconds)
        {
            var animation = Window.Current.Compositor.CreateScalarKeyFrameAnimation();
            animation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            animation.Target = nameof(Visual.Opacity);
            animation.Duration = TimeSpan.FromSeconds(seconds);
            return animation;
        }

        private static KeyFrameAnimation CreateOffsetAnimation(Compositor compositor, TimeSpan duration)
        {
            var kf = compositor.CreateVector3KeyFrameAnimation();
            kf.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            kf.Duration = duration;
            kf.Target = "Offset";
            return kf;
        }

        public static CompositionAnimation CreateHorizontalOffsetAnimation(
            double seconds,
            float offset,
            double delaySeconds,
            bool from)
        {
            var animation = Window.Current.Compositor.CreateScalarKeyFrameAnimation();
            if (delaySeconds != 0.0)
            {
                animation.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
                animation.DelayTime = TimeSpan.FromSeconds(delaySeconds);
            }

            animation.Duration = TimeSpan.FromSeconds(seconds);
            animation.Target = "Translation.X";
            if (from)
            {
                animation.InsertKeyFrame(0, offset);
                animation.InsertKeyFrame(1, 0);
            }
            else
            {
                animation.InsertKeyFrame(1, offset);
            }

            return animation;
        }

        public static CompositionAnimation
            CreateHorizontalOffsetAnimation(double seconds, float offset, double delaySeconds) =>
            CreateHorizontalOffsetAnimation(seconds, offset, delaySeconds, true);

        public static CompositionAnimation CreateVerticalOffsetAnimation(
            double seconds,
            float offset,
            double delaySeconds,
            bool from)
        {
            var animation = Window.Current.Compositor.CreateScalarKeyFrameAnimation();
            if (delaySeconds != 0.0)
            {
                animation.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
                animation.DelayTime = TimeSpan.FromSeconds(delaySeconds);
            }

            animation.Duration = TimeSpan.FromSeconds(seconds);
            animation.Target = "Translation.Y";
            if (from)
            {
                animation.InsertKeyFrame(0, offset);
                animation.InsertKeyFrame(1, 0);
            }
            else
            {
                animation.InsertKeyFrame(1, offset);
            }

            return animation;
        }

        public static CompositionAnimation
            CreateVerticalOffsetAnimation(double seconds, float offset, double delaySeconds) =>
            CreateVerticalOffsetAnimation(seconds, offset, delaySeconds, true);

        public static CompositionAnimation CreateVerticalOffsetAnimationFrom(double seconds, float offset) =>
            CreateVerticalOffsetAnimation(seconds, offset, 0.0f);

        public static CompositionAnimation CreateVerticalOffsetAnimationTo(double seconds, float offset) =>
            CreateVerticalOffsetAnimation(seconds, offset, 0.0f, false);

        public static T GetVisualChildByName<T>(this FrameworkElement root, string name)
            where T : FrameworkElement
        {
            var _ = VisualTreeHelper.GetChild(root, 0);
            FrameworkElement child = null;

            var count = VisualTreeHelper.GetChildrenCount(root);

            for (var i = 0; i < count && child == null; i++)
            {
                var current = (FrameworkElement) VisualTreeHelper.GetChild(root, i);
                if (current?.Name != null && current.Name == name)
                {   
                    child = current;
                    break;
                }

                child = current.GetVisualChildByName<FrameworkElement>(name);
            }

            return child as T;
        }
    }
}