// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using GongSolutions.Wpf.DragDrop;
using MyNet.Wpf.Converters;

namespace MyNet.Wpf.TestApp.TacticEditor
{
    internal class TacticEditor : ListBox, IDropTarget, IDragSource
    {
        static TacticEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TacticEditor), new FrameworkPropertyMetadata(typeof(TacticEditor)));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(TacticEditor), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(TacticEditor), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
        }
        public TacticEditor()
        {
            GongSolutions.Wpf.DragDrop.DragDrop.SetDropHandler(this, this);
            GongSolutions.Wpf.DragDrop.DragDrop.SetDragHandler(this, this);
            GongSolutions.Wpf.DragDrop.DragDrop.SetIsDragSource(this, true);
            GongSolutions.Wpf.DragDrop.DragDrop.SetIsDropTarget(this, true);
            GongSolutions.Wpf.DragDrop.DragDrop.SetDragDirectlySelectedOnly(this, true);
            GongSolutions.Wpf.DragDrop.DragDrop.SetUseDefaultEffectDataTemplate(this, false);
        }

        #region CanMove

        public bool CanMove
        {
            get => (bool)GetValue(CanMoveProperty);
            set => SetValue(CanMoveProperty, value);
        }

        public static readonly DependencyProperty CanMoveProperty = DependencyProperty.Register(nameof(CanMove), typeof(bool), typeof(TacticEditor), new PropertyMetadata(true));

        #endregion CanMove

        #region CanSelect

        public bool CanSelect
        {
            get => (bool)GetValue(CanSelectProperty);
            set => SetValue(CanSelectProperty, value);
        }

        public static readonly DependencyProperty CanSelectProperty = DependencyProperty.Register(nameof(CanSelect), typeof(bool), typeof(TacticEditor), new PropertyMetadata(true));

        #endregion CanSelect

        protected override DependencyObject GetContainerForItemOverride() => new PositionItem();

        protected override bool IsItemItsOwnContainerOverride(object item) => item is PositionItem;

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if (item is IPositionWrapper position && element is PositionItem positionItem)
            {
                positionItem.Position = position;

                var translate = new TranslateTransform();
                var multiBindingX = new MultiBinding() { Converter = MathConverter.PercentToValue };
                multiBindingX.Bindings.Add(new Binding(path: nameof(ActualWidth))
                {
                    Source = positionItem,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
                multiBindingX.Bindings.Add(new Binding(path: nameof(IPositionWrapper.OffsetX))
                {
                    Source = position,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
                var multiBindingY = new MultiBinding() { Converter = MathConverter.PercentToValue };
                multiBindingY.Bindings.Add(new Binding(path: nameof(ActualHeight))
                {
                    Source = positionItem,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
                multiBindingY.Bindings.Add(new Binding(path: nameof(IPositionWrapper.OffsetY))
                {
                    Source = position,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
                BindingOperations.SetBinding(translate, TranslateTransform.XProperty, multiBindingX);
                BindingOperations.SetBinding(translate, TranslateTransform.YProperty, multiBindingY);

                positionItem.RenderTransform = translate;
            }
        }

        #region DropTarget

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            if (CanAcceptData(dropInfo))
            {
                dropInfo.Effects = DragDropEffects.Move;
                dropInfo.DropTargetAdorner = null;

                ComputeOffset(dropInfo);
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            if (dropInfo?.DragInfo == null)
            {
                return;
            }

            ComputeOffset(dropInfo);

        }

        private void ComputeOffset(IDropInfo dropInfo)
        {
            if (dropInfo.Data is not IPositionWrapper item) return;

            var rect = PositionsCanvas.ComputePositionLayout(new Size(ActualWidth, ActualHeight), item);

            var offsetX = dropInfo.DropPosition.X - (rect.X + dropInfo.DragInfo.PositionInDraggedItem.X);
            var offsetY = dropInfo.DropPosition.Y - (rect.Y + dropInfo.DragInfo.PositionInDraggedItem.Y);

            item.OffsetX = offsetX / rect.Width * 100;
            item.OffsetY = offsetY / rect.Height * 100;
        }

        /// <summary>
        /// Test the specified drop information for the right data.
        /// </summary>
        /// <param name="dropInfo">The drop information.</param>
        public static bool CanAcceptData(IDropInfo dropInfo)
            => (dropInfo?.DragInfo) != null && dropInfo.IsSameDragDropContextAsSource && dropInfo.TargetCollection is not null && dropInfo.Data is IPositionWrapper;

        #endregion

        #region DragSource

        public virtual void StartDrag(IDragInfo dragInfo)
        {
            dragInfo.Data = dragInfo.SourceItem;
            dragInfo.Effects = DragDropEffects.Move;
        }

        /// <inheritdoc />
        public virtual bool CanStartDrag(IDragInfo dragInfo) => CanMove;

        /// <inheritdoc />
        public virtual void Dropped(IDropInfo dropInfo)
        {
        }

        /// <inheritdoc />
        public virtual void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
            // nothing here
        }

        /// <inheritdoc />
        public virtual void DragCancelled()
        {
        }

        /// <inheritdoc />
        public virtual bool TryCatchOccurredException(Exception exception) => false;

        #endregion
    }
}
