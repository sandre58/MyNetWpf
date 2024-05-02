// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyNet.Wpf.Controls
{
    public class CalendarAppointment : ListBoxItem
    {
        #region Constructors

        public CalendarAppointment() : base()
        {
            var d = DependencyPropertyDescriptor.FromProperty(IsKeyboardFocusedProperty, typeof(CalendarAppointment));
            d.AddValueChanged(this, OnVisualStatePropertyChanged);

            var d1 = DependencyPropertyDescriptor.FromProperty(IsMouseOverProperty, typeof(CalendarAppointment));
            d1.AddValueChanged(this, OnVisualStatePropertyChanged);

            var d2 = DependencyPropertyDescriptor.FromProperty(IsEnabledProperty, typeof(CalendarAppointment));
            d2.AddValueChanged(this, OnVisualStatePropertyChanged);
        }

        static CalendarAppointment()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarAppointment), new FrameworkPropertyMetadata(typeof(CalendarAppointment)));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(CalendarAppointment), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(CalendarAppointment), new FrameworkPropertyMetadata(KeyboardNavigationMode.Local));
        }

        #endregion Constructors

        #region Protected Methods

        private void OnVisualStatePropertyChanged(object? sender, EventArgs e)
        {
            if (sender is CalendarAppointment control)
            {
                control.UpdateVisualState(true);
            }
        }

        protected virtual void UpdateVisualState(bool useTransitions)
        {
            _ = !IsEnabled
                ? VisualStateManager.GoToState(this, Content is Control ? "Normal" : "Disabled", useTransitions)
                : IsMouseOver
                    ? VisualStateManager.GoToState(this, "MouseOver", useTransitions)
                    : VisualStateManager.GoToState(this, "Normal", useTransitions);

            _ = IsKeyboardFocused
                ? VisualStateManager.GoToState(this, "Focused", useTransitions)
                : VisualStateManager.GoToState(this, "Unfocused", useTransitions);
        }

        #endregion Protected Methods

    }
}
