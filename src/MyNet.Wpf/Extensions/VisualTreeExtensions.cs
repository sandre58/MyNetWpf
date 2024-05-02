// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MyNet.Wpf.Extensions
{
    public static class VisualTreeExtensions
    {
        public static IEnumerable<DependencyObject> VisualDepthFirstTraversal(this DependencyObject node)
        {
            yield return node is null ? throw new ArgumentNullException(nameof(node)) : node;
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(node); i++)
            {
                var child = VisualTreeHelper.GetChild(node, i);
                foreach (var descendant in child.VisualDepthFirstTraversal())
                {
                    yield return descendant;
                }
            }
        }

        public static T? FindVisualParent<T>(this DependencyObject dependencyObject, string name = "") where T : FrameworkElement
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);

            if (parent == null && dependencyObject is FrameworkElement element)
            {
                parent = element.Parent;
            }

            return parent == null
                ? null
                : parent is T variable && (variable.Name == name || string.IsNullOrEmpty(name)) ? variable : FindVisualParent<T>(parent);
        }

        public static IEnumerable<DependencyObject> GetVisualAncestry(this DependencyObject? leaf)
        {
            while (leaf is not null)
            {
                yield return leaf;
                leaf = leaf is Visual || leaf is Visual3D
                    ? VisualTreeHelper.GetParent(leaf)
                    : LogicalTreeHelper.GetParent(leaf);
            }
        }


        public static T? FindVisualChild<T>(this DependencyObject dependencyObject, string name = "") where T : FrameworkElement
        {
            if (dependencyObject == null)
            {
                return null;
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(dependencyObject);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);

                var result = child is T foundChild && (foundChild.Name == name || string.IsNullOrEmpty(name)) ? foundChild : FindVisualChild<T>(child, name);
                if (result != null)
                {
                    return result;
                }
            }
            return null;

        }

        /// <summary>
        /// Gets the first visual child of <paramref name="parent"/>.
        /// If there are no visual children <c>null</c> is returned.
        /// </summary>
        /// <returns>The first visual child of <paramref name="parent"/> or <c>null</c> if there are no children.</returns>
        public static DependencyObject? FindFirstVisualChild(this DependencyObject parent)
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            return childrenCount == 0
                       ? null
                       : VisualTreeHelper.GetChild(parent, 0);
        }

        public static DependencyObject? FindTopLevelParent(this DependencyObject dependencyObject)
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);
            if (parent != null)
            {
                var nextParent = VisualTreeHelper.GetParent(parent);
                return nextParent != null ? parent.FindTopLevelParent() : parent;
            }
            return null;
        }

        public static bool IsDescendantOf(this DependencyObject child, DependencyObject parent)
        {
            var success = false;

            var curr = child;

            while (curr != null)
            {
                if (curr == parent)
                {
                    success = true;
                    break;
                }

                // Find popup if curr is a PopupRoot
                if (curr is Popup popupRoot)
                {
                    //Now Popup does not have a visual link to its parent (for context menu)
                    //it is stored in its parent's arraylist (DP)
                    //so we get its parent by looking at PlacementTarget
                    var popup = popupRoot.Parent as Popup;

                    curr = popup;

                    if (popup != null)
                    {
                        // Try the poup Parent
                        curr = popup.Parent;

                        // Otherwise fall back to placement target
                        curr ??= popup.PlacementTarget;
                    }
                }
                else // Otherwise walk tree
                {
                    curr = curr.FindVisualParent<FrameworkElement>();

                }

            }

            return success;
        }

    }
}
