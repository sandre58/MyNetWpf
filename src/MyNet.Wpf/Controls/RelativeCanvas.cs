// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MyNet.Wpf.Controls
{
    public class RelativeCanvas : Canvas
    {

        /// <summary>
        /// Canvas computes a position for each of its children taking into account their margin and
        /// attached Canvas properties: Top, Left.  
        /// 
        /// Canvas will also arrange each of its children.
        /// </summary>
        /// <param name="arrangeSize">Size that Canvas will assume to position children.</param>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            //Canvas arranges children at their DesiredSize.
            //This means that Margin on children is actually respected and added
            //to the size of layout partition for a child. 
            //Therefore, is Margin is 10 and Left is 20, the child's ink will start at 30.

            foreach (UIElement child in InternalChildren)
            {
                if (child == null) { continue; }

                double x = 0;
                double y = 0;


                //Compute offset of the child:
                //If Left is specified, then Right is ignored
                //If Left is not specified, then Right is used
                //If both are not there, then 0
                var left = GetLeft(child);
                if (!double.IsNaN(left))
                {
                    x = GetRelativeValue(left, arrangeSize.Width);
                }
                else
                {
                    var right = GetRight(child);

                    if (!double.IsNaN(right))
                    {
                        x = arrangeSize.Width - child.DesiredSize.Width - GetRelativeValue(right, arrangeSize.Width);
                    }
                }

                var top = GetTop(child);
                if (!double.IsNaN(top))
                {
                    y = GetRelativeValue(top, arrangeSize.Height);
                }
                else
                {
                    var bottom = GetBottom(child);

                    if (!double.IsNaN(bottom))
                    {
                        y = arrangeSize.Height - child.DesiredSize.Height - GetRelativeValue(bottom, arrangeSize.Height);
                    }
                }

                child.Arrange(new Rect(new Point(x, y), child.DesiredSize));
            }
            return arrangeSize;
        }

        private static double GetRelativeValue(double value, double size) => value * size / 100;
    }
}
