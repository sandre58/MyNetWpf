// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyNet.Wpf.Parameters
{
    public static class ProgressBarAssist
    {
        #region BorderThickness

        /// <summary>
        /// The AutoWireViewModel attached property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.RegisterAttached(
            "BorderThickness",
            typeof(double),
            typeof(ProgressBarAssist),
            new PropertyMetadata(2.0D));

        public static double GetBorderThickness(DependencyObject item) => (double)item.GetValue(BorderThicknessProperty);

        public static void SetBorderThickness(DependencyObject item, double value) => item.SetValue(BorderThicknessProperty, value);

        #endregion BorderThickness

        #region StrokeThickness

        /// <summary>
        /// The AutoWireViewModel attached property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.RegisterAttached(
            "StrokeThickness",
            typeof(double),
            typeof(ProgressBarAssist),
            new PropertyMetadata(2.0D));

        public static double GetStrokeThickness(DependencyObject item) => (double)item.GetValue(StrokeThicknessProperty);

        public static void SetStrokeThickness(DependencyObject item, double value) => item.SetValue(StrokeThicknessProperty, value);

        #endregion StrokeThickness

        #region Stroke

        /// <summary>
        /// The AutoWireViewModel attached property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.RegisterAttached(
            "Stroke",
            typeof(Brush),
            typeof(ProgressBarAssist),
            new PropertyMetadata(null));

        public static Brush GetStroke(DependencyObject item) => (Brush)item.GetValue(StrokeProperty);

        public static void SetStroke(DependencyObject item, Brush value) => item.SetValue(StrokeProperty, value);

        #endregion Stroke

        #region InnerPadding

        /// <summary>
        /// The AutoWireViewModel attached property.
        /// </summary>
        public static readonly DependencyProperty InnerPaddingProperty = DependencyProperty.RegisterAttached(
            "InnerPadding",
            typeof(double),
            typeof(ProgressBarAssist),
            new PropertyMetadata(6.0D));

        public static double GetInnerPadding(DependencyObject item) => (double)item.GetValue(InnerPaddingProperty);

        public static void SetInnerPadding(DependencyObject item, double value) => item.SetValue(InnerPaddingProperty, value);

        #endregion InnerPadding

        #region ShowValue

        /// <summary>
        /// The AutoWireViewModel attached property.
        /// </summary>
        public static readonly DependencyProperty ShowValueProperty = DependencyProperty.RegisterAttached(
            "ShowValue",
            typeof(bool),
            typeof(ProgressBarAssist),
            new PropertyMetadata(false));

        public static bool GetShowValue(DependencyObject item) => (bool)item.GetValue(ShowValueProperty);

        public static void SetShowValue(DependencyObject item, bool value) => item.SetValue(ShowValueProperty, value);

        #endregion ShowValue

        #region ShowSecondProgress

        /// <summary>
        /// The AutoWireViewModel attached property.
        /// </summary>
        public static readonly DependencyProperty ShowSecondProgressProperty = DependencyProperty.RegisterAttached(
            "ShowSecondProgress",
            typeof(bool),
            typeof(ProgressBarAssist),
            new PropertyMetadata(false));

        public static bool GetShowSecondProgress(DependencyObject item) => (bool)item.GetValue(ShowSecondProgressProperty);

        public static void SetShowSecondProgress(DependencyObject item, bool value) => item.SetValue(ShowSecondProgressProperty, value);

        #endregion ShowSecondProgress

        #region ContentStringFormat

        /// <summary>
        /// The AutoWireViewModel attached property.
        /// </summary>
        public static readonly DependencyProperty ContentStringFormatProperty = DependencyProperty.RegisterAttached(
            "ContentStringFormat",
            typeof(string),
            typeof(ProgressBarAssist),
            new PropertyMetadata(null));

        public static string GetContentStringFormat(DependencyObject item) => (string)item.GetValue(ContentStringFormatProperty);

        public static void SetContentStringFormat(DependencyObject item, string value) => item.SetValue(ContentStringFormatProperty, value);

        #endregion ContentStringFormat

        #region ContentTemplate

        /// <summary>
        /// The AutoWireViewModel attached property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.RegisterAttached(
            "ContentTemplate",
            typeof(DataTemplate),
            typeof(ProgressBarAssist),
            new PropertyMetadata(null));

        public static DataTemplate GetContentTemplate(DependencyObject item) => (DataTemplate)item.GetValue(ContentTemplateProperty);

        public static void SetContentTemplate(DependencyObject item, DataTemplate value) => item.SetValue(ContentTemplateProperty, value);

        #endregion ContentTemplate
    }
}
