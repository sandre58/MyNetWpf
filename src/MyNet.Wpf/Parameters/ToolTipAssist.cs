// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MaterialDesignThemes.Wpf;
using MyNet.Wpf.Helpers;

namespace MyNet.Wpf.Parameters
{
    public static class ToolTipAssist
    {
        #region Content

        public static readonly DependencyProperty ContentProperty = DependencyProperty.RegisterAttached(
            "Content",
            typeof(object),
            typeof(ToolTipAssist),
            new PropertyMetadata(null, OnContentChangedCallback));

        public static object GetContent(UIElement item) => item.GetValue(ContentProperty);

        public static void SetContent(UIElement item, object value) => item.SetValue(ContentProperty, value);

        private static void OnContentChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement target) return;
            if (e.NewValue is null) return;

            UpdateTooltip(target);
        }

        private static void UpdateTooltip(FrameworkElement target)
        {
            var placementMode = GetPlacementMode(target);
            var tooltip = new ToolTip
            {
                Content = GetContent(target),
                ContentTemplate = GetContentTemplate(target),
                Placement = placementMode.HasValue ? PlacementMode.Custom : PlacementMode.Mouse,
            };
            target.ToolTip = tooltip;
            HeaderAssist.SetHeader(tooltip, GetHeader(target));
            HeaderAssist.SetHeaderTemplate(tooltip, GetHeaderTemplate(target));

            if (placementMode.HasValue)
                InitializePlacementMode(target, placementMode.Value);
        }
        #endregion Content

        #region ContentTemplate

        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.RegisterAttached(
            "ContentTemplate",
            typeof(DataTemplate),
            typeof(ToolTipAssist),
            new PropertyMetadata(null, OnContentChangedCallback));

        public static DataTemplate GetContentTemplate(UIElement item) => (DataTemplate)item.GetValue(ContentTemplateProperty);

        public static void SetContentTemplate(UIElement item, DataTemplate value) => item.SetValue(ContentTemplateProperty, value);

        #endregion ContentTemplate

        #region Header

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached(
            "Header",
            typeof(object),
            typeof(ToolTipAssist),
            new PropertyMetadata(null, OnHeaderChangedCallback));

        public static object GetHeader(UIElement item) => item.GetValue(HeaderProperty);

        public static void SetHeader(UIElement item, object value) => item.SetValue(HeaderProperty, value);

        private static void OnHeaderChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement target) return;
            if (target.ToolTip is not ToolTip tooltip) return;

            HeaderAssist.SetHeader(tooltip, e.NewValue);
        }

        #endregion Header

        #region HeaderTemplate

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.RegisterAttached(
            "HeaderTemplate",
            typeof(DataTemplate),
            typeof(ToolTipAssist),
            new PropertyMetadata(null, OnHeaderTemplateChangedCallback));

        public static DataTemplate GetHeaderTemplate(UIElement item) => (DataTemplate)item.GetValue(HeaderTemplateProperty);

        public static void SetHeaderTemplate(UIElement item, DataTemplate value) => item.SetValue(HeaderTemplateProperty, value);

        private static void OnHeaderTemplateChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement target) return;
            if (target.ToolTip is not ToolTip tooltip) return;

            HeaderAssist.SetHeaderTemplate(tooltip, (DataTemplate)e.NewValue);
        }

        #endregion Header

        #region PlacementMode

        public static readonly DependencyProperty PlacementModeProperty = DependencyProperty.RegisterAttached(
            "PlacementMode",
            typeof(PopupBoxPlacementMode?),
            typeof(ToolTipAssist),
            new PropertyMetadata(null, OnPlacementModeChangedCallback));

        public static PopupBoxPlacementMode? GetPlacementMode(UIElement item) => (PopupBoxPlacementMode?)item.GetValue(PlacementModeProperty);

        public static void SetPlacementMode(UIElement item, PopupBoxPlacementMode? value) => item.SetValue(PlacementModeProperty, value);

        private static void OnPlacementModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement target) return;
            if (e.NewValue is not PopupBoxPlacementMode placementMode) return;

            InitializePlacementMode(target, placementMode);
        }

        private static void InitializePlacementMode(FrameworkElement target, PopupBoxPlacementMode placementMode)
        {
            if (target.ToolTip is not ToolTip tooltip) return;

            tooltip.Placement = PlacementMode.Custom;
            PopupAssist.SetPlacementMode(tooltip, placementMode);
            tooltip.CustomPopupPlacementCallback = (x, y, z) => GetPopupPlacement(target, placementMode, x, y, z);
            ToolTipService.SetShowsToolTipOnKeyboardFocus(target, true);
            ToolTipService.SetInitialShowDelay(target, 0);
        }

        private static CustomPopupPlacement[] GetPopupPlacement(FrameworkElement target, PopupBoxPlacementMode placementMode, Size popupSize, Size targetSize, Point _)
        {
            double x, y;
            var offsetX = ToolTipService.GetHorizontalOffset(target);
            var offsetY = ToolTipService.GetVerticalOffset(target);

            var popupSizeTransformed = new Size(DpiHelper.TransformFromDeviceX(target, popupSize.Width), DpiHelper.TransformFromDeviceY(target, popupSize.Height));
            var targetSizeTransformed = new Size(DpiHelper.TransformFromDeviceX(target, targetSize.Width), DpiHelper.TransformFromDeviceY(target, targetSize.Height));

            switch (placementMode)
            {
                case PopupBoxPlacementMode.BottomAndAlignLeftEdges:
                    x = 0 + offsetX - useOffsetIfRtl(popupSizeTransformed.Width);
                    y = targetSizeTransformed.Height + offsetY;
                    break;
                case PopupBoxPlacementMode.BottomAndAlignRightEdges:
                    x = 0 - popupSizeTransformed.Width + targetSizeTransformed.Width + offsetX + useOffsetIfRtl(popupSizeTransformed.Width - targetSizeTransformed.Width * 2);
                    y = targetSizeTransformed.Height + offsetY;
                    break;
                case PopupBoxPlacementMode.BottomAndAlignCentres:
                    x = (targetSizeTransformed.Width - popupSizeTransformed.Width) / 2 + offsetX - useOffsetIfRtl(targetSizeTransformed.Width);
                    y = targetSizeTransformed.Height + offsetY;
                    break;
                case PopupBoxPlacementMode.TopAndAlignLeftEdges:
                    x = 0 + offsetX - useOffsetIfRtl(popupSizeTransformed.Width);
                    y = 0 - popupSizeTransformed.Height + offsetY;
                    break;
                case PopupBoxPlacementMode.TopAndAlignRightEdges:
                    x = 0 - popupSizeTransformed.Width + targetSizeTransformed.Width + offsetX + useOffsetIfRtl(popupSizeTransformed.Width - targetSizeTransformed.Width * 2);
                    y = 0 - popupSizeTransformed.Height + offsetY;
                    break;
                case PopupBoxPlacementMode.TopAndAlignCentres:
                    x = (targetSizeTransformed.Width - popupSizeTransformed.Width) / 2 + offsetX - useOffsetIfRtl(targetSizeTransformed.Width);
                    y = 0 - popupSizeTransformed.Height + offsetY;
                    break;
                case PopupBoxPlacementMode.LeftAndAlignTopEdges:
                    x = 0 - popupSizeTransformed.Width + offsetX + useOffsetIfRtl(popupSizeTransformed.Width);
                    y = 0 + offsetY;
                    break;
                case PopupBoxPlacementMode.LeftAndAlignBottomEdges:
                    x = 0 - popupSizeTransformed.Width + offsetX + useOffsetIfRtl(popupSizeTransformed.Width);
                    y = 0 - (popupSizeTransformed.Height - targetSizeTransformed.Height) + offsetY;
                    break;
                case PopupBoxPlacementMode.LeftAndAlignMiddles:
                    x = 0 - popupSizeTransformed.Width + offsetX + useOffsetIfRtl(popupSizeTransformed.Width);
                    y = 0 - (popupSizeTransformed.Height - targetSizeTransformed.Height) / 2 + offsetY;
                    break;
                case PopupBoxPlacementMode.RightAndAlignTopEdges:
                    x = targetSizeTransformed.Width + offsetX - useOffsetIfRtl(popupSizeTransformed.Width + targetSizeTransformed.Width * 2);
                    y = 0 + offsetY;
                    break;
                case PopupBoxPlacementMode.RightAndAlignBottomEdges:
                    x = targetSizeTransformed.Width + offsetX - useOffsetIfRtl(popupSizeTransformed.Width + targetSizeTransformed.Width * 2);
                    y = 0 - (popupSizeTransformed.Height - targetSizeTransformed.Height) + offsetY;
                    break;
                case PopupBoxPlacementMode.RightAndAlignMiddles:
                    x = targetSizeTransformed.Width + offsetX - useOffsetIfRtl(popupSizeTransformed.Width + targetSizeTransformed.Width * 2);
                    y = 0 - (popupSizeTransformed.Height - targetSizeTransformed.Height) / 2 + offsetY;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(placementMode));
            }

            var xTransformed = DpiHelper.TransformToDeviceX(target, x);
            var yTransformed = DpiHelper.TransformToDeviceY(target, y);

            return [new CustomPopupPlacement(new Point(xTransformed, yTransformed), PopupPrimaryAxis.Horizontal)];

            double useOffsetIfRtl(double rtlOffset) => target.FlowDirection == FlowDirection.LeftToRight ? 0 : rtlOffset;
        }
        #endregion Header
    }
}
