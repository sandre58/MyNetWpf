// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using MyNet.UI.Layout;
using MyNet.Utilities;
using MyNet.Wpf.Extensions;
using MyNet.Wpf.Parameters;

namespace MyNet.Wpf.Behaviors
{
    public sealed class ListViewLayoutBehavior : Behavior<ListView>, IDisposable
    {
        public ListViewLayoutBehavior() { }

        private const int Margin = 0;
        private const long RefreshTime = Timeout.Infinite;
        private const long Delay = 0;
        private IColumnLayoutCollection? _columnLayouts;

        private Timer? _timer;

        public IColumnLayoutCollection? ColumnLayouts => _columnLayouts ??= ListViewAssist.GetColumnLayouts(AssociatedObject);

        private void InitializeLayout(GridViewColumnCollection columns)
        {
            if (ColumnLayouts is null)
                ListViewAssist.SetColumnLayouts(AssociatedObject, []);

            if (ColumnLayouts is null) return;

            if (!ColumnLayouts.Any())
            {
                ColumnLayouts.AddRange(
                    columns.Select((x, index) => new GridViewColumnLayout(x, index)
                    {
                        CanBeHidden = GridViewColumnAssist.GetCanBeHidden(x),
                        Width = GridViewColumnAssist.GetWidth(x)
                    }));

                ColumnLayouts.Reset();
            }
            else
            {
                ColumnLayouts.OfType<GridViewColumnLayout>().ForEach((x, index) =>
                {
                    if (index < columns.Count && index >= 0)
                        x.Column = columns[index];
                });

                Refresh();
            }

            ColumnLayouts.RefreshRequested -= ColumnLayouts_RefreshRequested;
            ColumnLayouts.RefreshRequested += ColumnLayouts_RefreshRequested;

            columns.CollectionChanged -= Columns_CollectionChanged;
            columns.CollectionChanged += Columns_CollectionChanged;
            ColumnLayouts.OfType<GridViewColumnLayout>().ForEach(x =>
            {
                x.IsVisibleChanged -= OnBehaviorIsVisibleChanged;
                x.IsVisibleChanged += OnBehaviorIsVisibleChanged;
            });
        }

        private void ColumnLayouts_RefreshRequested(object? sender, EventArgs e) => Refresh();

        private void Columns_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (ColumnLayouts is null || sender is not GridViewColumnCollection columns) return;

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
            {
                var column = e.NewItems?.Cast<GridViewColumn>().FirstOrDefault();

                if (column is null || e.NewStartingIndex == e.OldStartingIndex) return;

                var moveAfter = e.NewStartingIndex > e.OldStartingIndex;

                var nextIndex = e.NewStartingIndex < columns.Count - 1 ? e.NewStartingIndex + 1 : columns.Count;
                var nextColumn = columns[nextIndex];

                var columnBehavior = ColumnLayouts.OfType<GridViewColumnLayout>().FirstOrDefault(x => Equals(x.Column, column));
                var nextColumnBehavior = ColumnLayouts.OfType<GridViewColumnLayout>().FirstOrDefault(x => Equals(x.Column, nextColumn));

                var nextIndexInBehavior = nextColumnBehavior?.Index ?? 0;
                var oldIndexInBehaviors = columnBehavior?.Index ?? 0;

                foreach (var behavior in ColumnLayouts)
                {
                    if (moveAfter)
                    {
                        if (behavior.Index == oldIndexInBehaviors)
                            behavior.Index = nextIndexInBehavior - 1;
                        else if (behavior.Index < nextIndexInBehavior && behavior.Index > oldIndexInBehaviors)
                            behavior.Index -= 1;
                    }
                    else
                    {
                        if (behavior.Index == oldIndexInBehaviors)
                            behavior.Index = nextIndexInBehavior;
                        else if (behavior.Index >= nextIndexInBehavior && behavior.Index < oldIndexInBehaviors)
                            behavior.Index += 1;
                    }
                }
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.OnLoading<ListView>(_ => AddBehavior());
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            RemoveBehavior();
        }

        private void AddBehavior()
        {
            if (AssociatedObject.View is GridView gridView)
                InitializeLayout(gridView.Columns);

            void resizeAndEnableSize()
            {
                Resize();
                AssociatedObject.SizeChanged += OnSizeChanged;
            }
            _timer = new Timer(x => UI.Threading.Scheduler.GetUIOrCurrent().Schedule(resizeAndEnableSize), null, Delay, RefreshTime);
        }

        private void RemoveBehavior()
        {
            AssociatedObject.SizeChanged -= OnSizeChanged;

            if (ColumnLayouts is not null)
            {
                ColumnLayouts.RefreshRequested -= ColumnLayouts_RefreshRequested;
                ColumnLayouts.OfType<GridViewColumnLayout>().ForEach(x => x.IsVisibleChanged -= OnBehaviorIsVisibleChanged);
            }

            if (AssociatedObject.View is GridView gridView)
                gridView.Columns.CollectionChanged -= Columns_CollectionChanged;

            _timer?.Dispose();
        }

        private void Refresh()
        {
            if (AssociatedObject.View is GridView gridView && ColumnLayouts is not null)
                gridView.Columns.Sort(x => ColumnLayouts.OfType<GridViewColumnLayout>().First(y => Equals(y.Column, x)).Index);
        }

        private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                AssociatedObject.SizeChanged -= OnSizeChanged;
                _ = (_timer?.Change(Delay, RefreshTime));
            }
        }

        private void OnBehaviorIsVisibleChanged(object? sender, EventArgs e) => Resize();

        private void Resize()
        {
            if (ColumnLayouts is null || !ListViewAssist.GetAutoResizeIsEnabled(AssociatedObject)) return;

            var layouts = ColumnLayouts.OfType<GridViewColumnLayout>().OrderBy(x => x.Index).ToList();
            if (AssociatedObject.ActualWidth > 0)
            {
                var totalWidth = AssociatedObject.ActualWidth;
                if (AssociatedObject.View is GridView gv)
                {
                    var allowedSpace = totalWidth - GetAllocatedSpace();
                    allowedSpace -= Margin;
                    var totalPercentage = layouts.Where(x => x.IsVisible).Sum(x => x.Percentage);
                    var columnsToInsert = new Dictionary<int, GridViewColumn>();
                    foreach (var layout in layouts)
                    {
                        layout.SetWidth(allowedSpace, totalPercentage);
                        if (layout.Column.Width.NearlyEqual(0))
                        {
                            if (gv.Columns.Contains(layout.Column))
                                _ = gv.Columns.Remove(layout.Column);
                        }
                        else
                        {
                            if (!gv.Columns.Contains(layout.Column))
                                columnsToInsert.Add(layout.Index, layout.Column);
                        }
                    }

                    if (columnsToInsert.Count != 0)
                    {
                        foreach (var column in columnsToInsert)
                        {
                            var nextLayout = layouts.Find(x => x.Column.Width > 0 && x.Index > column.Key);
                            var newIndex = nextLayout != null ? gv.Columns.IndexOf(nextLayout.Column) : gv.Columns.Count - 1;

                            gv.Columns.Insert(newIndex, column.Value);
                        }
                    }
                }
            }
        }

        private double GetAllocatedSpace()
        {
            double totalWidth = 0;
            foreach (var layout in ColumnLayouts?.OfType<GridViewColumnLayout>() ?? [])
            {
                if (layout is GridViewColumnLayout columnLayout && columnLayout.IsStatic && columnLayout.IsVisible)
                    totalWidth += columnLayout.StaticWidth > 0 ? columnLayout.StaticWidth : columnLayout.Column.ActualWidth;
            }
            return totalWidth;
        }

        public void Dispose() => _timer?.Dispose();
    }
}
