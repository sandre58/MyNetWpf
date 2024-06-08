// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Xaml.Behaviors;
using MyNet.Wpf.Extensions;

namespace MyNet.Wpf.Behaviors
{
    public class ListBoxSelectOnMouseOverBehavior : Behavior<ListBox>
    {
        public Brush? SelectionBackground { get; set; }

        public Brush? SelectionBorderBrush { get; set; }

        public bool ShowSelectionArea { get; set; } = true;

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject.IsLoaded)
                Register();
            else
            {
                // We need to wait for it to be loaded so we can find the
                // child controls.
                AssociatedObject.Loaded += OnLoaded;
            }
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            base.OnDetaching();

            UnRegister();
        }

        private ScrollContentPresenter? _scrollContent;
        private ScrollViewer? _scrollViewer;
        private SelectionAdorner? _selectionAdorner;

        private readonly DispatcherTimer _autoScroll = new();
        private bool _mouseCaptured;
        private Point? _selectionStart;
        private Point? _selectionEnd;
        private Rect? _previousSelectionArea;

        private void Register()
        {
            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
            AssociatedObject.MouseMove += OnMouseMove;
        }

        private ScrollContentPresenter? GetScrollContent()
        {
            if (_scrollContent is null)
            {
                _scrollContent = AssociatedObject.FindVisualChild<ScrollContentPresenter>();
                if (_scrollContent != null)
                {
                    _selectionAdorner = new SelectionAdorner(_scrollContent);
                    _scrollContent.AdornerLayer.Add(_selectionAdorner);

                    if (SelectionBackground is not null)
                        _selectionAdorner.Background = SelectionBackground;
                    if (SelectionBorderBrush is not null)
                        _selectionAdorner.BorderBrush = SelectionBorderBrush;
                }
            }

            return _scrollContent;
        }

        private ScrollViewer? GetScrollViewer()
        {
            if (_scrollViewer is null)
            {
                _scrollViewer = AssociatedObject.FindVisualChild<ScrollViewer>();
                if (_scrollViewer is not null)
                {
                    _scrollViewer.ScrollChanged += OnScrollChanged;

                    _autoScroll.Tick += (sender, e) => Scroll();
                    _autoScroll.Interval = TimeSpan.FromMilliseconds(GetRepeatRate());
                }
            }

            return _scrollViewer;
        }

        private void UnRegister()
        {
            StopSelection();

            if (GetScrollViewer() is ScrollViewer scrollViewer)
                scrollViewer.ScrollChanged -= OnScrollChanged;

            AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
            AssociatedObject.MouseMove -= OnMouseMove;
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            Register();
            AssociatedObject.Loaded -= OnLoaded;
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (GetScrollViewer() is null || !_mouseCaptured || _selectionStart is null) return;

            _selectionStart = new Point(_selectionStart.Value.X - e.HorizontalChange, _selectionStart.Value.Y - e.VerticalChange);
            UpdateSelection();
        }

        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_mouseCaptured)
            {
                _mouseCaptured = false;
                StopSelection();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseCaptured && GetScrollContent() is ScrollContentPresenter scrollContent)
            {
                var mouse = e.GetPosition(scrollContent);
                _selectionEnd = mouse;

                if (!_autoScroll.IsEnabled)
                    Scroll();

                UpdateSelection();
            }
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is DependencyObject d && (d is ButtonBase || d.FindVisualParent<ButtonBase>() is not null)
                || e.OriginalSource is DependencyObject d1 && (d1 is ScrollBar || d1.FindVisualParent<ScrollBar>() is not null)
                || e.OriginalSource is DependencyObject d2 && (d2 is MenuItem || d2.FindVisualParent<MenuItem>() is not null)
                || e.RightButton == MouseButtonState.Pressed
                || GetScrollContent() is not ScrollContentPresenter scrollContent
                ) return;

            // Check that the mouse is inside the scroll content (could be on the scroll bars for example).
            var mouse = e.GetPosition(scrollContent);
            if (IsOnScrollContent(mouse))
            {
                _mouseCaptured = true;
                StartSelection(mouse);
            }
        }

        private void StopSelection()
        {
            // Hide the selection rectangle and stop the auto scrolling.
            if (_selectionAdorner is not null)
                _selectionAdorner.IsEnabled = false;
            _selectionStart = null;
            _selectionEnd = null;
            _autoScroll.Stop();
        }

        private void StartSelection(Point location)
        {
            // We've stolen the MouseLeftButtonDown event from the ListBox
            // so we need to manually give it focus.
            AssociatedObject.Focus();

            _previousSelectionArea = null;
            _selectionStart = location;
            _selectionEnd = location;

            // Do we need to start a new selection?
            if (((Keyboard.Modifiers & ModifierKeys.Control) == 0) &&
                ((Keyboard.Modifiers & ModifierKeys.Shift) == 0))
            {
                // Neither the shift key or control key is pressed, so
                // clear the selection.
                AssociatedObject.SelectedItems.Clear();
            }

            UpdateSelection();

            if (_selectionAdorner is not null)
                _selectionAdorner.IsEnabled = ShowSelectionArea;
        }

        private void UpdateSelection()
        {
            if (GetScrollContent() is not ScrollContentPresenter scrollContent) return;

            // Update adorner
            var newSelectionArea = ComputeSelectionArea(_selectionStart, _selectionEnd);
            _selectionAdorner?.UpdateArea(newSelectionArea);

            // And select the items.
            var topLeft = scrollContent.TranslatePoint(newSelectionArea.TopLeft, AssociatedObject);
            var bottomRight = scrollContent.TranslatePoint(newSelectionArea.BottomRight, AssociatedObject);
            var selectionAreaRelative = new Rect(topLeft, bottomRight);

            for (var i = 0; i < AssociatedObject.Items.Count; i++)
            {
                if (AssociatedObject.ItemContainerGenerator.ContainerFromIndex(i) is FrameworkElement item)
                {
                    var itemTopLeft = item.TranslatePoint(new Point(0, 0), AssociatedObject);
                    var itemBounds = new Rect(itemTopLeft.X, itemTopLeft.Y, item.ActualWidth, item.ActualHeight);

                    // Only change the selection if it intersects with the area
                    // (or intersected i.e. we changed the value last time).
                    if (itemBounds.IntersectsWith(selectionAreaRelative))
                    {
                        Selector.SetIsSelected(item, true);
                    }
                    else if (_previousSelectionArea is not null && itemBounds.IntersectsWith(_previousSelectionArea.Value))
                    {
                        // We previously changed the selection to true but it no
                        // longer intersects with the area so clear the selection.
                        Selector.SetIsSelected(item, false);
                    }
                }
            }

            _previousSelectionArea = newSelectionArea;
        }

        private bool IsOnScrollContent(Point mouse) => GetScrollContent() is ScrollContentPresenter scrollContent && mouse.X >= 0 && mouse.X < scrollContent.ActualWidth && mouse.Y >= 0 && mouse.Y < scrollContent.ActualHeight;

        private static Rect ComputeSelectionArea(Point? startPoint, Point? endPoint)
        {
            if (endPoint is not null && startPoint is not null && startPoint != endPoint)
            {
                var x = Math.Min(startPoint.Value.X, endPoint.Value.X);
                var y = Math.Min(startPoint.Value.Y, endPoint.Value.Y);
                var width = Math.Abs(endPoint.Value.X - startPoint.Value.X);
                var height = Math.Abs(endPoint.Value.Y - startPoint.Value.Y);
                return new Rect(x, y, width, height);
            }
            else
                return new Rect(0, 0, 0, 0);
        }

        private void Scroll()
        {
            if (GetScrollContent() is not ScrollContentPresenter scrollContent || GetScrollViewer() is not ScrollViewer scrollViewer || _selectionEnd is null) return;

            var position = _selectionEnd.Value;
            var scrolled = false;

            if (position.X > scrollContent.ActualWidth)
            {
                scrollViewer.LineRight();
                scrolled = true;
            }
            else if (position.X < 0)
            {
                scrollViewer.LineLeft();
                scrolled = true;
            }

            if (position.Y > scrollContent.ActualHeight)
            {
                scrollViewer.LineDown();
                scrolled = true;
            }
            else if (position.Y < 0)
            {
                scrollViewer.LineUp();
                scrolled = true;
            }

            // It's important to disable scrolling if we're inside the bounds of
            // the control so that when the user does leave the bounds we can
            // re-enable scrolling and it will have the correct initial delay.
            _autoScroll.IsEnabled = scrolled;
        }

        private static int GetRepeatRate()
        {
            const double ratio = (400.0 - 33.0) / 31.0;
            return 400 - (int)(SystemParameters.KeyboardSpeed * ratio);
        }


        private sealed class SelectionAdorner : Adorner
        {
            private Rect _area;

            public SelectionAdorner(UIElement parent) : base(parent)
            {
                BorderBrush = SystemColors.HighlightBrush;
                Background = BorderBrush.Clone();
                Background.Opacity = 0.4;

                IsHitTestVisible = false;

                IsEnabledChanged += (sender, e) => InvalidateVisual();
            }

            public Brush Background { get; set; }

            public Brush BorderBrush { get; set; }

            public void UpdateArea(Rect area)
            {
                _area = area;
                InvalidateVisual();
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);

                if (IsEnabled)
                {
                    // Make the lines snap to pixels (add half the pen width [0.5])
                    double[] x = [_area.Left + 0.5, _area.Right + 0.5];
                    double[] y = [_area.Top + 0.5, _area.Bottom + 0.5];
                    drawingContext.PushGuidelineSet(new GuidelineSet(x, y));

                    drawingContext.DrawRectangle(Background, new Pen(BorderBrush, 1.0), _area);
                }
            }
        }
    }
}
