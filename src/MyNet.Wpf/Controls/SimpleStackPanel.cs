// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;

namespace MyNet.Wpf.Controls
{
    /// <summary>
    /// Arranges child elements into a single line that can be oriented horizontally
    /// or vertically.
    /// </summary>
    public class SimpleStackPanel : Panel
    {
        static SimpleStackPanel() => DefaultStyleKeyProperty.OverrideMetadata(typeof(SimpleStackPanel), new FrameworkPropertyMetadata(typeof(SimpleStackPanel)));

        /// <summary>
        /// Gets or sets a value that indicates the dimension by which child elements are
        /// stacked.
        /// </summary>
        /// <returns>The Orientation of child content.</returns>
        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        /// <summary>
        /// Identifies the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
                DependencyProperty.Register(
                        nameof(Orientation),
                        typeof(Orientation),
                        typeof(SimpleStackPanel),
                        new FrameworkPropertyMetadata(
                                Orientation.Vertical,
                                FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets a uniform distance (in pixels) between stacked items. It is applied
        /// in the direction of the SimpleStackPanel's Orientation.
        /// </summary>
        /// <returns>The uniform distance (in pixels) between stacked items.</returns>
        public double Spacing
        {
            get => (double)GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }

        /// <summary>
        /// Identifies the Spacing dependency property.
        /// </summary>
        public static readonly DependencyProperty SpacingProperty =
                DependencyProperty.Register(
                        nameof(Spacing),
                        typeof(double),
                        typeof(SimpleStackPanel),
                        new FrameworkPropertyMetadata(
                                0.0,
                                FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets a value that indicates if this SimpleStackPanel has vertical
        /// or horizontal orientation.
        /// </summary>
        /// <returns>This property always returns true.</returns>
        protected override bool HasLogicalOrientation => true;

        /// <summary>
        /// Gets a value that represents the Orientation of the SimpleStackPanel.
        /// </summary>
        /// <returns>An Orientation value.</returns>
        protected override Orientation LogicalOrientation => Orientation;

        /// <summary>
        /// Measures the child elements of a SimpleStackPanel in anticipation
        /// of arranging them during the SimpleStackPanel.ArrangeOverride(System.Windows.Size)
        /// pass.
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns>The System.Windows.Size that represents the desired size of the element.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            var stackDesiredSize = new Size();
            var children = InternalChildren;
            var layoutSlotSize = availableSize;
            var fHorizontal = Orientation == Orientation.Horizontal;
            var spacing = Spacing;
            var hasVisibleChild = false;

            if (fHorizontal)
                layoutSlotSize.Width = double.PositiveInfinity;
            else
                layoutSlotSize.Height = double.PositiveInfinity;

            for (int i = 0, count = children.Count; i < count; ++i)
            {
                var child = children[i];

                if (child == null) continue;
                var isVisible = child.Visibility != Visibility.Collapsed;

                if (isVisible && !hasVisibleChild)
                    hasVisibleChild = true;

                child.Measure(layoutSlotSize);
                var childDesiredSize = child.DesiredSize;

                if (fHorizontal)
                {
                    stackDesiredSize.Width += (isVisible ? spacing : 0) + childDesiredSize.Width;
                    stackDesiredSize.Height = Math.Max(stackDesiredSize.Height, childDesiredSize.Height);
                }
                else
                {
                    stackDesiredSize.Width = Math.Max(stackDesiredSize.Width, childDesiredSize.Width);
                    stackDesiredSize.Height += (isVisible ? spacing : 0) + childDesiredSize.Height;
                }
            }

            if (fHorizontal)
                stackDesiredSize.Width -= hasVisibleChild ? spacing : 0;
            else
                stackDesiredSize.Height -= hasVisibleChild ? spacing : 0;

            return stackDesiredSize;
        }

        /// <summary>
        /// Arranges the content of a SimpleStackPanel element.
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns>
        /// The System.Windows.Size that represents the arranged size of this SimpleStackPanel
        /// element and its child elements.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var children = InternalChildren;
            var fHorizontal = Orientation == Orientation.Horizontal;
            var rcChild = new Rect(finalSize);
            var previousChildSize = 0.0;
            var spacing = Spacing;

            for (int i = 0, count = children.Count; i < count; ++i)
            {
                var child = children[i];

                if (child == null) continue;
                if (fHorizontal)
                {
                    rcChild.X += previousChildSize;
                    previousChildSize = child.DesiredSize.Width;
                    rcChild.Width = previousChildSize;
                    rcChild.Height = Math.Max(finalSize.Height, child.DesiredSize.Height);
                }
                else
                {
                    rcChild.Y += previousChildSize;
                    previousChildSize = child.DesiredSize.Height;
                    rcChild.Height = previousChildSize;
                    rcChild.Width = Math.Max(finalSize.Width, child.DesiredSize.Width);
                }

                if (child.Visibility != Visibility.Collapsed)
                    previousChildSize += spacing;

                child.Arrange(rcChild);
            }
            return finalSize;
        }
    }
}
