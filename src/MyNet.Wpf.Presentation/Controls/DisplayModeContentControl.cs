// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows;
using MyNet.UI.ViewModels;
using MyNet.Wpf.Controls;
using MyNet.Wpf.Helpers;

namespace MyNet.Wpf.Presentation.Controls
{
    public class DisplayModeContentControl : TransitioningContent
    {
        private readonly Dictionary<IDisplayMode, DataTemplate?> _templates = [];

        static DisplayModeContentControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(DisplayModeContentControl), new FrameworkPropertyMetadata(typeof(TransitioningContent)));

        #region Mode

        public static readonly DependencyProperty ModeProperty = DependencyProperty.RegisterAttached(nameof(Mode), typeof(IDisplayMode), typeof(DisplayModeContentControl), new PropertyMetadata(OnModeChanged));

        public IDisplayMode Mode
        {
            get => (IDisplayMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not DisplayModeContentControl control) return;

            if (e.NewValue is not IDisplayMode mode)
                control.SetValue(ContentTemplateProperty, null);
            else
            {
                if (!control._templates.TryGetValue(mode, out var value))
                {
                    value = WpfHelper.GetResource<DataTemplate>(control.Resources, $"{mode.Key}ContentTemplate");
                    control._templates.Add(mode, value);
                }
                control.SetValue(ContentTemplateProperty, value);
            }

            control.StartOpeningEffects();
        }

        #endregion Mode

    }
}
