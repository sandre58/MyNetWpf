// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MyNet.Wpf.TestApp.TacticEditor
{
    public class PositionsCanvas : Canvas
    {
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            //Canvas arranges children at their DesiredSize.
            //This means that Margin on children is actually respected and added
            //to the size of layout partition for a child. 
            //Therefore, is Margin is 10 and Left is 20, the child's ink will start at 30.

            foreach (UIElement child in InternalChildren)
            {
                if (child is not PositionItem positionItem || positionItem.Position is null) { continue; }

                var rectangle = ComputePositionLayout(arrangeSize, positionItem.Position);

                child.Arrange(rectangle);
            }
            return arrangeSize;
        }

        public static Rect ComputePositionLayout(Size containerSize, IPositionWrapper position)
        {
            var column = (int)position.Position.Side;
            var row = (int)position.Position.Line;
            var rowInverse = Position.RowsCount - 1 - row;

            var coef = 0.5 / Position.RowsCount;
            var decrementWidthByRow = containerSize.Width * coef;
            var rowWidth = containerSize.Width - decrementWidthByRow * rowInverse;
            var width = rowWidth / Position.ColumnsCount;
            var height = containerSize.Height / Position.RowsCount;
            var x = (containerSize.Width - rowWidth) / 2 + width * column;
            var y = row * height;

            return new Rect(x, y, width, height);
        }
    }
}
