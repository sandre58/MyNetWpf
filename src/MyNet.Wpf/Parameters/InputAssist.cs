// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MyNet.Wpf.Extensions;

namespace MyNet.Wpf.Parameters
{
    public static class InputAssist
    {
        private static readonly Dictionary<int, Tuple<WeakReference<FrameworkElement>, List<InputBinding>>> TrackedFrameWorkElementsToBindings =
        [];

        public static readonly DependencyProperty InputBindingsProperty = DependencyProperty.RegisterAttached(
            "InputBindings",
            typeof(InputBindingCollection),
            typeof(InputAssist),
            new PropertyMetadata(new InputBindingCollection(), OnInputBindingsChanged));

        public static InputBindingCollection GetInputBindings(FrameworkElement uiElement) => (InputBindingCollection)uiElement.GetValue(InputBindingsProperty);

        public static void SetInputBindings(FrameworkElement uiElement, InputBindingCollection value) => uiElement.SetValue(InputBindingsProperty, value);

        private static void OnInputBindingsChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is not FrameworkElement element)
            {
                return;
            }

            element.InputBindings.AddRange((InputBindingCollection)e.NewValue);
            _ = SaveInputBindings(element);
        }

        public static readonly DependencyProperty IsSuspendedProperty = DependencyProperty.RegisterAttached(
"IsSuspended",
typeof(bool),
typeof(InputAssist),
new PropertyMetadata(false, OnIsSuspendedChanged));

        public static bool GetIsSuspended(FrameworkElement uiElement) => (bool)uiElement.GetValue(IsSuspendedProperty);

        public static void SetIsSuspended(FrameworkElement uiElement, bool value) => uiElement.SetValue(IsSuspendedProperty, value);

        private static void OnIsSuspendedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                RemoveBindingsFromElement(element);
                _ = SaveInputBindings(element);
                element.InputBindings.Clear();
            }
            else
            {
                AddBindingsFromElement(element);
            }
        }

        public static readonly DependencyProperty PropagateInWindowProperty = DependencyProperty.RegisterAttached(
    "PropagateInWindow",
    typeof(bool),
    typeof(InputAssist),
    new PropertyMetadata(false, OnPropagateInWindowChanged));

        public static bool GetPropagateInWindow(FrameworkElement uiElement) => (bool)uiElement.GetValue(PropagateInWindowProperty);

        public static void SetPropagateInWindow(FrameworkElement uiElement, bool value) => uiElement.SetValue(PropagateInWindowProperty, value);

        private static void OnPropagateInWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                d.OnLoading<FrameworkElement>(x =>
                {
                    AddBindingsFromElement(x);
                    element.IsEnabledChanged += Element_IsEnabledChanged;
                    element.IsVisibleChanged += Element_IsEnabledChanged;
                }, x =>
                {
                    RemoveBindingsFromElement(x);
                    element.IsEnabledChanged -= Element_IsEnabledChanged;
                    element.IsVisibleChanged -= Element_IsEnabledChanged;
                });
            }
            else
            {
                RemoveBindingsFromElement(element);
                element.IsEnabledChanged -= Element_IsEnabledChanged;
                element.IsVisibleChanged -= Element_IsEnabledChanged;
            }
        }

        private static void Element_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                AddBindingsFromElement((FrameworkElement)sender);
            }
            else
            {
                RemoveBindingsFromElement((FrameworkElement)sender);
            }
        }

        private static void AddBindingsFromElement(FrameworkElement frameworkElement)
        {
            if (GetIsSuspended(frameworkElement)) return;

            var window = Window.GetWindow(frameworkElement);
            window ??= Application.Current.MainWindow;
            if (window != null)
            {
                // transfer InputBindings into our control
                var trackingData = SaveInputBindings(frameworkElement);

                if (trackingData is not null)
                {
                    // apply Bindings to Window
                    foreach (var inputBinding in trackingData.Item2)
                    {
                        if (!window.InputBindings.Contains(inputBinding))
                        {
                            _ = window.InputBindings.Add(inputBinding);
                        }
                    }
                }

                frameworkElement.InputBindings.Clear();
            }
        }

        private static Tuple<WeakReference<FrameworkElement>, List<InputBinding>>? SaveInputBindings(FrameworkElement frameworkElement)
        {
            if (!TrackedFrameWorkElementsToBindings.TryGetValue(frameworkElement.GetHashCode(), out var trackingData) && frameworkElement.InputBindings.Count > 0)
            {
                trackingData = Tuple.Create(
                    new WeakReference<FrameworkElement>(frameworkElement),
                    frameworkElement.InputBindings.Cast<InputBinding>().ToList());

                TrackedFrameWorkElementsToBindings.Add(frameworkElement.GetHashCode(), trackingData);
            }

            return trackingData;
        }

        private static void RemoveBindingsFromElement(FrameworkElement frameworkElement)
        {
            var window = Window.GetWindow(frameworkElement);
            window ??= Application.Current.MainWindow;
            var hashCode = frameworkElement.GetHashCode();

            // remove Bindings from Window
            if (window != null && TrackedFrameWorkElementsToBindings.TryGetValue(hashCode, out var trackedData))
            {
                foreach (var binding in trackedData.Item2)
                {
                    if (!frameworkElement.InputBindings.Contains(binding))
                    {
                        _ = frameworkElement.InputBindings.Add(binding);
                    }

                    window.InputBindings.Remove(binding);
                }
                trackedData.Item2.Clear();
                _ = TrackedFrameWorkElementsToBindings.Remove(hashCode);

                // catch removed and orphaned entries
                CleanupBindingsDictionary(window, TrackedFrameWorkElementsToBindings);
            }
        }

        private static void CleanupBindingsDictionary(Window window, Dictionary<int, Tuple<WeakReference<FrameworkElement>, List<InputBinding>>> bindingsDictionary)
        {
            foreach (var hashCode in bindingsDictionary.Keys.ToList())
            {
                if (bindingsDictionary.TryGetValue(hashCode, out var trackedData) &&
                    !trackedData.Item1.TryGetTarget(out _))
                {
                    foreach (var binding in trackedData.Item2)
                    {
                        window.InputBindings.Remove(binding);
                    }

                    trackedData.Item2.Clear();
                    _ = bindingsDictionary.Remove(hashCode);
                }
            }
        }
    }
}
