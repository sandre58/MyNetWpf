// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Windows;
using System.Windows.Media;
using GongSolutions.Wpf.DragDrop;
using MahApps.Metro.Controls;
using MyNet.Wpf.Controls;
using MyNet.Wpf.Helpers;

namespace MyNet.Wpf.DragAndDrop
{
    public class CalendarDropTargetAdorner : DropTargetAdorner
    {
        private static readonly PathGeometry Triangle;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3963:\"static\" fields should be initialized inline", Justification = "Impossible")]
        static CalendarDropTargetAdorner()
        {
            // Create the pen and triangle in a static constructor and freeze them to improve performance.
            const int triangleSize = 5;

            var firstLine = new LineSegment(new Point(0, -triangleSize), false);
            firstLine.Freeze();
            var secondLine = new LineSegment(new Point(0, triangleSize), false);
            secondLine.Freeze();

            var figure = new PathFigure { StartPoint = new Point(triangleSize, 0) };
            figure.Segments.Add(firstLine);
            figure.Segments.Add(secondLine);
            figure.Freeze();

            Triangle = new PathGeometry();
            Triangle.Figures.Add(figure);
            Triangle.Freeze();
        }

        public CalendarDropTargetAdorner(UIElement adornedElement, DropInfo dropInfo)
            : base(adornedElement, dropInfo)
        {
            DateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
            FontStyle = FontStyles.Normal;
            FontWeight = FontWeights.Normal;
            FontStretch = FontStretches.Normal;
            FontFamily = WpfHelper.GetResource<FontFamily>("MyNet.Font.Family.Roboto");
            FontSize = WpfHelper.GetResource<double>("MyNet.Font.Size");
            Foreground = WpfHelper.GetResource<Brush>("MyNet.Brushes.Primary");
            Opacity = 1;
            Pen = new Pen(WpfHelper.GetResource<Brush>("MyNet.Brushes.Primary"), 2)
            {
                DashStyle = DashStyles.Solid,
                DashCap = PenLineCap.Round
            };
        }

        public Brush Foreground { get; set; }

        public string DateFormat { get; set; }

        public double FontSize { get; set; }

        public FontFamily FontFamily { get; set; }

        public FontStretch FontStretch { get; set; }

        public FontWeight FontWeight { get; set; }

        public FontStyle FontStyle { get; set; }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var dropInfo = DropInfo;

            if (dropInfo.VisualTarget is not CalendarBase calendarBase) return;

            var date = calendarBase.GetAccurateDateFromPosition(dropInfo.DropPosition);
            var calendarItem = calendarBase.GetCalendarItemFromPosition(dropInfo.DropPosition);

            if (calendarItem is null || date is null) return;

            var orientation = calendarBase.ItemsOrientation;
            Point start;
            Point end;
            Point textPosition;
            double rotation = 0;

            var calendarItemLocation = calendarItem.TranslatePoint(new Point(0, 0), calendarBase.FindChild<CalendarItemsControl>());
            var calendarItemBounds = new Rect(calendarItemLocation, calendarItem.RenderSize);

            if (orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                start = new Point(dropInfo.DropPosition.X, calendarItemBounds.Y);
                end = new Point(dropInfo.DropPosition.X, calendarItemBounds.Bottom);
                textPosition = new Point(start.X + 5, start.Y);
                rotation = 90;
            }
            else
            {
                start = new Point(calendarItemBounds.X, dropInfo.DropPosition.Y);
                end = new Point(calendarItemBounds.Right, dropInfo.DropPosition.Y);
                textPosition = new Point(start.X, start.Y - 20);
            }

            drawingContext.PushOpacity(Opacity);
            drawingContext.DrawText(new FormattedText(date.Value.ToString(DateFormat, CultureInfo.CurrentUICulture),
                                                      CultureInfo.CurrentUICulture,
                                                      FlowDirection.LeftToRight,
                                                      new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                                                      FontSize,
                                                      Foreground,
                                                      VisualTreeHelper.GetDpi(this).PixelsPerDip), textPosition);
            drawingContext.DrawLine(Pen, start, end);
            DrawTriangle(drawingContext, start, rotation);
            DrawTriangle(drawingContext, end, 180 + rotation);
        }

        private void DrawTriangle(DrawingContext drawingContext, Point origin, double rotation)
        {
            drawingContext.PushTransform(new TranslateTransform(origin.X, origin.Y));
            drawingContext.PushTransform(new RotateTransform(rotation));

            drawingContext.DrawGeometry(Pen.Brush, null, Triangle);

            drawingContext.Pop();
            drawingContext.Pop();
        }
    }
}
