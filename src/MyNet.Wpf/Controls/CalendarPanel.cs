// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MyNet.Utilities;

namespace MyNet.Wpf.Controls
{
    public class CalendarPanel : Panel
    {
        private GridLinesRenderer? _gridLinesRenderer;

        public CalendarPanel()
            : base()
        {
        }

        #region Public Properties

        public Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        public static readonly DependencyProperty BorderBrushProperty =
                DependencyProperty.Register(
                        "BorderBrush",
                        typeof(Brush),
                        typeof(CalendarPanel),
                        new FrameworkPropertyMetadata(
                                null,
                                FrameworkPropertyMetadataOptions.AffectsRender));

        public double BorderThickess
        {
            get => (double)GetValue(BorderThickessProperty);
            set => SetValue(BorderThickessProperty, value);
        }

        public static readonly DependencyProperty BorderThickessProperty =
                DependencyProperty.Register(
                        "BorderThickess",
                        typeof(double),
                        typeof(CalendarPanel),
                        new FrameworkPropertyMetadata(
                                1d,
                                FrameworkPropertyMetadataOptions.AffectsRender));

        public int Columns
        {
            get => (int)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        /// <summary>
        /// DependencyProperty for <see cref="Columns" /> property.
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty =
                DependencyProperty.Register(
                        "Columns",
                        typeof(int),
                        typeof(CalendarPanel),
                        new FrameworkPropertyMetadata(
                                1,
                                FrameworkPropertyMetadataOptions.AffectsMeasure),
                        new ValidateValueCallback(ValidateColumns));

        private static bool ValidateColumns(object o) => (int)o >= 0;

        /// <summary>
        /// Specifies the number of rows in the grid
        /// A value of 0 indicates that the row count should be dynamically 
        /// computed based on the number of columns (if specified) and the 
        /// number of non-collapsed children in the grid
        /// </summary>
        public int Rows
        {
            get => (int)GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        public static readonly DependencyProperty RowsProperty =
                DependencyProperty.Register(
                        "Rows",
                        typeof(int),
                        typeof(CalendarPanel),
                        new FrameworkPropertyMetadata(
                                1,
                                FrameworkPropertyMetadataOptions.AffectsMeasure),
                        new ValidateValueCallback(ValidateRows));

        private static bool ValidateRows(object o) => (int)o >= 0;

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// Compute the desired size of this UniformGrid by measuring all of the
        /// children with a constraint equal to a cell's portion of the given
        /// constraint (e.g. for a 2 x 4 grid, the child constraint would be
        /// constraint.Width*0.5 x constraint.Height*0.25).  The maximum child
        /// width and maximum child height are tracked, and then the desired size
        /// is computed by multiplying these maximums by the row and column count
        /// (e.g. for a 2 x 4 grid, the desired size for the UniformGrid would be
        /// maxChildDesiredWidth*2 x maxChildDesiredHeight*4).
        /// </summary>
        /// <param name="availableSize">Constraint</param>
        /// <returns>Desired size</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            var childConstraint = new Size(availableSize.Width / Columns, availableSize.Height / Rows);
            var maxChildDesiredWidth = 0.0;
            var maxChildDesiredHeight = 0.0;

            //  Measure each child, keeping track of maximum desired width and height.
            for (int i = 0, count = InternalChildren.Count; i < count; ++i)
            {
                var child = InternalChildren[i];

                // Measure the child.
                child.Measure(childConstraint);
                var childDesiredSize = child.DesiredSize;

                if (maxChildDesiredWidth < childDesiredSize.Width)
                {
                    maxChildDesiredWidth = childDesiredSize.Width;
                }

                if (maxChildDesiredHeight < childDesiredSize.Height)
                {
                    maxChildDesiredHeight = childDesiredSize.Height;
                }
            }

            return new Size(maxChildDesiredWidth * Columns, maxChildDesiredHeight * Rows);
        }

        /// <summary>
        /// Arrange the children of this UniformGrid by distributing space evenly 
        /// among all of the children, making each child the size equal to a cell's
        /// portion of the given arrangeSize (e.g. for a 2 x 4 grid, the child size
        /// would be arrangeSize*0.5 x arrangeSize*0.25)
        /// </summary>
        /// <param name="finalSize">Arrange size</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var childBounds = new Rect(0, 0, finalSize.Width / Columns, finalSize.Height / Rows);

            // Arrange and Position each child to the same cell size
            foreach (UIElement child in InternalChildren)
            {
                var columnIndex = Grid.GetColumn(child);
                var rowIndex = Grid.GetRow(child);

                var cellRect = new Rect(childBounds.Width * columnIndex, childBounds.Height * rowIndex, childBounds.Width, childBounds.Height);

                child.Arrange(cellRect);
            }

            EnsureGridLinesRenderer().UpdateRenderBounds(finalSize);

            return finalSize;
        }

        /// <summary>
        /// Synchronized ShowGridLines property with the state of the grid's visual collection
        /// by adding / removing GridLinesRenderer visual.
        /// Returns a reference to GridLinesRenderer visual or null.
        /// </summary>
        private GridLinesRenderer EnsureGridLinesRenderer()
        {
            if (_gridLinesRenderer == null)
            {
                _gridLinesRenderer = new GridLinesRenderer();
                AddVisualChild(_gridLinesRenderer);
            }

            return _gridLinesRenderer;
        }

        /// <summary>
        ///   Derived class must implement to support Visual children. The method must return
        ///    the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark:
        ///       During this virtual call it is not valid to modify the Visual tree.
        /// </summary>
        protected override Visual GetVisualChild(int index) =>
            // because "base.Count + 1" for GridLinesRenderer
            // argument checking done at the base class
            index == base.VisualChildrenCount
                ? _gridLinesRenderer == null
                    ? throw new ArgumentOutOfRangeException(nameof(index))
                    : (Visual)_gridLinesRenderer
                : base.GetVisualChild(index);

        /// <summary>
        ///  Derived classes override this property to enable the Visual code to enumerate
        ///  the Visual children. Derived classes need to return the number of children
        ///  from this method.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: During this virtual method the Visual tree must not be modified.
        /// </summary>
        protected override int VisualChildrenCount
            //since GridLinesRenderer has not been added as a child, so we do not subtract
            => base.VisualChildrenCount + (_gridLinesRenderer != null ? 1 : 0);

        #endregion Protected Methods

        /// <summary>
        /// Helper to render grid lines.
        /// </summary>
        internal class GridLinesRenderer : DrawingVisual
        {
            /// <summary>
            /// UpdateRenderBounds.
            /// </summary>
            /// <param name="boundsSize">Size of render bounds</param>
            internal void UpdateRenderBounds(Size boundsSize)
            {
                using var drawingContext = RenderOpen();

                if (VisualTreeHelper.GetParent(this) is not CalendarPanel panel) return;

                var childBounds = new Rect(0, 0, boundsSize.Width / panel.Columns, boundsSize.Height / panel.Rows);
                var children = panel.InternalChildren.OfType<UIElement>().ToList();

                for (var columnIndex = 0; columnIndex < panel.Columns; columnIndex++)
                {
                    var childrenInThisColumnOrPreviousColumn = children.Where(x => Grid.GetColumn(x) == columnIndex || Grid.GetColumn(x) == columnIndex - 1).ToList();
                    var minRowIndex = childrenInThisColumnOrPreviousColumn.MinOrDefault(Grid.GetRow);
                    var maxRowIndex = childrenInThisColumnOrPreviousColumn.MaxOrDefault(Grid.GetRow) + 1;
                    var startColumn = new Point(columnIndex * childBounds.Width, minRowIndex * childBounds.Height);
                    var endColumn = new Point(columnIndex * childBounds.Width, maxRowIndex * childBounds.Height);
                    if (columnIndex > 0)
                        DrawGridLine(drawingContext, panel.BorderBrush, panel.BorderThickess, startColumn, endColumn);

                    for (var rowColumnIndex = 1; rowColumnIndex < panel.Rows; rowColumnIndex++)
                    {
                        if (children.Exists(x => Grid.GetColumn(x) == columnIndex && (Grid.GetRow(x) == rowColumnIndex || Grid.GetRow(x) == rowColumnIndex - 1)))
                        {
                            var startRow = new Point(columnIndex * childBounds.Width, rowColumnIndex * childBounds.Height);
                            var endRow = new Point((columnIndex + 1) * childBounds.Width, rowColumnIndex * childBounds.Height);
                            DrawGridLine(drawingContext, panel.BorderBrush, panel.BorderThickess, startRow, endRow);
                        }
                    }
                }
            }

            /// <summary>
            /// Draw single hi-contrast line.
            /// </summary>
            private static void DrawGridLine(
            DrawingContext drawingContext,
            Brush brush,
            double borderThickess,
            Point start,
            Point end)
            {
                var pen = new Pen(brush, borderThickess);
                drawingContext.DrawLine(pen, start, end);
            }
        }
    }
}
