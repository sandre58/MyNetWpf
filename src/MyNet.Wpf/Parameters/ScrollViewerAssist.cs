// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using MyNet.Wpf.Extensions;

namespace MyNet.Wpf.Parameters;

public static class ScrollViewerAssist
{
    #region IsAutoHideEnabled

    public static readonly DependencyProperty IsAutoHideEnabledProperty = DependencyProperty.RegisterAttached(
        "IsAutoHideEnabled", typeof(bool), typeof(ScrollViewerAssist), new PropertyMetadata(default(bool)));

    public static void SetIsAutoHideEnabled(DependencyObject element, bool value) => element.SetValue(IsAutoHideEnabledProperty, value);

    public static bool GetIsAutoHideEnabled(DependencyObject element) => (bool)element.GetValue(IsAutoHideEnabledProperty);

    #endregion

    #region CornerRectangleVisibility

    public static readonly DependencyProperty CornerRectangleVisibilityProperty = DependencyProperty.RegisterAttached(
        "CornerRectangleVisibility", typeof(Visibility), typeof(ScrollViewerAssist), new PropertyMetadata(default(Visibility)));

    public static void SetCornerRectangleVisibility(DependencyObject element, Visibility value) => element.SetValue(CornerRectangleVisibilityProperty, value);

    public static Visibility GetCornerRectangleVisibility(DependencyObject element) => (Visibility)element.GetValue(CornerRectangleVisibilityProperty);

    #endregion

    #region ShowSeparators

    public static readonly DependencyProperty ShowSeparatorsProperty = DependencyProperty.RegisterAttached(
        "ShowSeparators", typeof(bool), typeof(ScrollViewerAssist), new PropertyMetadata(default(bool)));

    public static void SetShowSeparators(DependencyObject element, bool value) => element.SetValue(ShowSeparatorsProperty, value);

    public static bool GetShowSeparators(DependencyObject element) => (bool)element.GetValue(ShowSeparatorsProperty);

    #endregion

    #region IgnorePadding

    public static readonly DependencyProperty IgnorePaddingProperty = DependencyProperty.RegisterAttached(
        "IgnorePadding", typeof(bool), typeof(ScrollViewerAssist), new PropertyMetadata(true));

    public static void SetIgnorePadding(DependencyObject element, bool value) => element.SetValue(IgnorePaddingProperty, value);
    public static bool GetIgnorePadding(DependencyObject element) => (bool)element.GetValue(IgnorePaddingProperty);

    #endregion

    #region HorizontalScrollHook

    private static readonly DependencyProperty HorizontalScrollHookProperty = DependencyProperty.RegisterAttached(
        "HorizontalScrollHook", typeof(HwndSourceHook), typeof(ScrollViewerAssist), new PropertyMetadata(null));

    public static readonly DependencyProperty SupportHorizontalScrollProperty = DependencyProperty.RegisterAttached(
        "SupportHorizontalScroll", typeof(bool), typeof(ScrollViewerAssist), new PropertyMetadata(false, OnSupportHorizontalScrollChanged));

    private static void OnSupportHorizontalScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        //Based on: https://blog.walterlv.com/post/handle-horizontal-scrolling-of-touchpad-en.html
        if (d is ScrollViewer scrollViewer)
        {
            if ((bool)e.NewValue)
            {
                onLoaded(scrollViewer, sv =>
                {
                    if (GetSupportHorizontalScroll(sv))
                    {
                        registerHook(sv);
                    }
                });
            }
            else
            {
                onLoaded(scrollViewer, sv =>
                {
                    if (!GetSupportHorizontalScroll(sv))
                    {
                        removeHook(sv);
                    }
                });
            }
        }

        static void onLoaded(ScrollViewer scrollViewer, Action<ScrollViewer> doOnLoaded)
        {
            if (scrollViewer.IsLoaded)
            {
                doOnLoaded(scrollViewer);
            }
            else
            {
                void onLoaded(object o, RoutedEventArgs _)
                {
                    scrollViewer.Loaded -= onLoaded;
                    doOnLoaded(scrollViewer);
                }

                scrollViewer.Loaded += onLoaded;
            }
        }

        static void removeHook(ScrollViewer scrollViewer)
        {
            if (scrollViewer.GetValue(HorizontalScrollHookProperty) is HwndSourceHook hook &&
                PresentationSource.FromVisual(scrollViewer) is HwndSource source)
            {
                source.RemoveHook(hook);
                scrollViewer.SetValue(HorizontalScrollHookProperty, null);
            }
        }

        static void registerHook(ScrollViewer scrollViewer)
        {
            removeHook(scrollViewer);
            if (PresentationSource.FromVisual(scrollViewer) is HwndSource source)
            {
                HwndSourceHook h = hook;
                scrollViewer.SetValue(HorizontalScrollHookProperty, h);
                source.AddHook(h);
            }

            IntPtr hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
            {
                const int wM_MOUSEHWHEEL = 0x020E;
                switch (msg)
                {
                    case wM_MOUSEHWHEEL:
                        int tilt = (short)(wParam.ToInt64() >> 16 & 0xFFFF);
                        onMouseTilt(tilt);
                        return (IntPtr)1;
                }
                return IntPtr.Zero;
            }

            void onMouseTilt(int tilt) => scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + tilt);
        }
    }

    public static readonly DependencyProperty BubbleVerticalScrollProperty = DependencyProperty.RegisterAttached(
        "BubbleVerticalScroll", typeof(bool), typeof(ScrollViewerAssist), new PropertyMetadata(false, OnBubbleVerticalScrollChanged));

    private static void OnBubbleVerticalScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ScrollViewer scrollViewer)
        {
            if ((bool)e.NewValue)
            {
                onLoaded(scrollViewer, sv =>
                {
                    if (GetBubbleVerticalScroll(sv))
                    {
                        registerHook(sv);
                    }
                });
            }
            else
            {
                onLoaded(scrollViewer, sv =>
                {
                    if (!GetBubbleVerticalScroll(sv))
                    {
                        removeHook(sv);
                    }
                });
            }
        }

        static void onLoaded(ScrollViewer scrollViewer, Action<ScrollViewer> doOnLoaded)
        {
            if (scrollViewer.IsLoaded)
            {
                doOnLoaded(scrollViewer);
            }
            else
            {
                void onLoaded(object o, RoutedEventArgs _)
                {
                    scrollViewer.Loaded -= onLoaded;
                    doOnLoaded(scrollViewer);
                }

                scrollViewer.Loaded += onLoaded;
            }
        }

        static void removeHook(ScrollViewer scrollViewer) => scrollViewer.RemoveHandler(UIElement.MouseWheelEvent, (RoutedEventHandler)scrollViewerOnMouseWheel);

        static void registerHook(ScrollViewer scrollViewer)
        {
            removeHook(scrollViewer);
            scrollViewer.AddHandler(UIElement.MouseWheelEvent, (RoutedEventHandler)scrollViewerOnMouseWheel, true);
        }

        // This relay is only needed because the UIElement.AddHandler() has strict requirements for the signature of the passed Delegate
        static void scrollViewerOnMouseWheel(object sender, RoutedEventArgs e) => handleMouseWheel(sender, (MouseWheelEventArgs)e);

        static void handleMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;

            if (scrollViewer.GetVisualAncestry().Skip(1).FirstOrDefault() is not UIElement parentUiElement)
                return;

            // Re-raise the mouse wheel event on the visual parent to bubble it upwards
            e.Handled = true;
            var mouseWheelEventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = sender
            };
            parentUiElement.RaiseEvent(mouseWheelEventArgs);
        }
    }

    public static void SetSupportHorizontalScroll(DependencyObject element, bool value)
        => element.SetValue(SupportHorizontalScrollProperty, value);

    public static bool GetSupportHorizontalScroll(DependencyObject element)
        => (bool)element.GetValue(SupportHorizontalScrollProperty);

    public static void SetBubbleVerticalScroll(DependencyObject element, bool value)
        => element.SetValue(BubbleVerticalScrollProperty, value);

    public static bool GetBubbleVerticalScroll(DependencyObject element)
        => (bool)element.GetValue(BubbleVerticalScrollProperty);

    #endregion
}
