namespace RSI.IndissPlus.Plots.SC.Modifiers
{
    using SciChart.Charting.ChartModifiers;
    using SciChart.Core.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    public static class SciChartModifiersHelpers
    {
        public static bool IsPointWithinModifierBounds(this ChartModifierBase modifier, Point point, IHitTestable element)
        {
            var x1y1 = modifier.ParentSurface.RootGrid.TranslatePoint(point, element);

            var fe = (element as FrameworkElement);
            bool inBounds = (x1y1.X <= fe.ActualWidth && x1y1.X >= 0)
                                && (x1y1.Y <= fe.ActualHeight && x1y1.Y >= 0);

            return inBounds;
        }

        public static System.Windows.Media.Color GetForegroundColorDependingOnBackgroundColor(System.Windows.Media.Color backgroundColor)
        {
            double hsp = Math.Sqrt(
              0.299 * (backgroundColor.R * backgroundColor.R) +
              0.587 * (backgroundColor.G * backgroundColor.G) +
              0.114 * (backgroundColor.B * backgroundColor.B)
            );

            if (hsp < 127.5)
            {
                return System.Windows.Media.Colors.White;
            }
            else
            {
                return System.Windows.Media.Colors.Black;
            }
        }
    }

    public class DoubleEventArgs
    {
        public DoubleEventArgs(double value)
        {
            this.Value = value;
        }

        public double Value { get; private set; }
    }
}
