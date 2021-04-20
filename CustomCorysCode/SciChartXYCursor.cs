using System.Windows.Media;
using SciChart.Charting.Visuals;

namespace RSI.IndissPlus.Plots.SC.Modifiers
{
    using SciChart.Charting.ChartModifiers;
    using SciChart.Charting.Visuals.Annotations;
    using SciChart.Charting.Visuals.Axes;
    using SciChart.Core.Framework;
    using SciChart.Core.Utility.Mouse;
    using SciChart.Data.Model;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Shapes;

    public class SciChartXYCursor : VerticalSliceModifier
    {
        private SciChartSurface _sciChartControl;
        private VerticalLineAnnotation _cursor;
        private double? _pixelCoordX;
        private double? _lastRelativePosition;
        private bool _relativePositionNeedToBeSet;

        public SciChartXYCursor(SciChartSurface sciChartControl)
        {
            this._sciChartControl = sciChartControl;

            this._cursor = new VerticalLineAnnotation()
            {
                IsEditable = true,
                IsHidden = true,
                LabelPlacement = LabelPlacement.Axis,
                FontSize = 12,                
            };

            this._cursor.DragDelta += Cursor_DragDelta;

            this.InitIds();

            this.CursorColor = Colors.White;
            this.VerticalLines.Add(_cursor);
            this.ShowTooltipOn = ShowTooltipOptions.Always;
            this.ShowAxisLabels = true;
            this.UseInterpolation = true;
        }

        private void InitIds()
        {
            IAxis axis = this._sciChartControl.YAxes.FirstOrDefault();
            if (axis != null)
            {
                this._cursor.YAxisId = axis.Id;
            }

            axis = this._sciChartControl.XAxes.FirstOrDefault();
            if (axis != null)
            {
                this._sciChartControl.XAxes.First().VisibleRangeChanged += SciChartXYCursor_VisibleRangeChanged;
                this._cursor.XAxisId = this._sciChartControl.XAxes.First().Id;
            }

        }

        private void SciChartXYCursor_VisibleRangeChanged(object sender, SciChart.Charting.Visuals.Events.VisibleRangeChangedEventArgs e)
        {
            //if (this._sciChartControl.Visitor != null && this._sciChartControl.Visitor.IsSetCursorOnClickEnabled)
            //{
            //    if (this._sciChartControl.Visitor.IsStaticCursor)
            //    {
            //        return;
            //    }

            //    if (this._cursor.X1 == null)
            //    {
            //        this._cursor.IsEditable = false;
            //        DateRange range = e.NewVisibleRange as DateRange;
            //        this.SetCursorPosition(range.Max);
            //    }
            //}

            if (!this._cursor.IsHidden && this._cursor.X1 != null)
            {
                if (!_pixelCoordX.HasValue || double.IsNaN(_pixelCoordX.Value))
                {
                    _pixelCoordX = this.GetCursorPixelPosition();
                }

                if (double.IsNaN(_pixelCoordX.Value))
                {
                    if (this._lastRelativePosition.HasValue && this._relativePositionNeedToBeSet)
                    {
                        this.SetVisible(false);
                        this.SetRelativePosition(this._lastRelativePosition.Value);
                        this.SetVisible(true);

                        this._relativePositionNeedToBeSet = false;
                        _pixelCoordX = this.GetCursorPixelPosition();
                    }
                    else
                    {
                        return;
                    }
                }

                double dataValue = (double)_sciChartControl.XAxes.First().GetDataValue(_pixelCoordX.Value);
                this.SetCursorPosition(dataValue);

                this.UpdateCursorPosition();
            }
            
        }

        private void Cursor_DragDelta(object sender, SciChart.Charting.Visuals.Events.AnnotationDragDeltaEventArgs e)
        {
            _pixelCoordX = this.GetCursorPixelPosition();
            this.UpdateCursorPosition();
        }

        private void UpdateCursorPosition()
        {
            if (_sciChartControl.XAxes.FirstOrDefault() != null && !this._cursor.IsHidden && _pixelCoordX.HasValue)
            {
                IAxis axis = _sciChartControl.XAxes.First();
                this.PositionAsDouble = (double)axis.GetDataValue(_pixelCoordX.Value);

                DoubleRange doubleRange = axis.VisibleRange as DoubleRange;
                if (doubleRange != null)
                {
                    double max = doubleRange.Max;
                    double min = doubleRange.Min;
                    double threshold = (max - min) * 0.005;
                    this.PositionAsDouble = (max - PositionAsDouble) < threshold ? max : PositionAsDouble;
                }

                if (this.DataCursorPositionChanged != null)
                {
                    this.DataCursorPositionChanged(this, new DoubleEventArgs(this.PositionAsDouble));
                }
            }
        }

        private double GetCursorPixelPosition()
        {
            return Canvas.GetLeft(this._cursor) + (this._cursor.ActualWidth / 2);
        }

        //public override void OnModifierMouseDown(ModifierMouseArgs e)
        //{
        //    bool isOnChart = this.IsPointWithinModifierBounds(e.MousePoint, ModifierSurface);
        //    if (isOnChart && this._sciChartControl.Visitor != null && this._sciChartControl.Visitor.IsSetCursorOnClickEnabled && _sciChartControl.XAxes.Count() != 0)
        //    {
        //        if (this._cursor.IsEditable)
        //        {
        //            this._cursor.IsEditable = false;
        //        }

        //        this._sciChartControl.Visitor.IsStaticCursor = true;

        //        System.Windows.Point xy = e.MousePoint;
        //        // Translates the mouse point (from root grid coords) to ModifierSurface coords
        //        _pixelCoordX = base.GetPointRelativeTo(xy, base.ModifierSurface).X;
        //        // you can now use this coordinate to convert to data values
        //        DateTime dataValue = (DateTime)_sciChartControl.XAxes.First().GetDataValue(_pixelCoordX.Value);
        //        this.SetCursorPosition(dataValue);
        //        this.UpdateCursorPosition();
        //    }
        //}

        public void SetVisible(bool visible)
        {
            this._cursor.IsHidden = !visible;
        }

        public void SetInitialRelativePosition(double relativePosition)
        {
            //if (this._sciChartControl.Visitor != null && this._sciChartControl.Visitor.IsSetCursorOnClickEnabled)
            //{
            //    return;
            //}

            this.SetRelativePosition(relativePosition);
        }


        private void SetRelativePosition(double relativePosition)
        {
            try
            {
                this._lastRelativePosition = relativePosition;
                if (!string.IsNullOrEmpty(this._cursor.XAxisId))
                {
                    this._relativePositionNeedToBeSet = true;
                    IAxis axis = this._sciChartControl.XAxes.GetAxisById(this._cursor.XAxisId);
                    if (axis != null && axis.VisibleRange != null)
                    {
                        DoubleRange doubleRange = axis.VisibleRange as DoubleRange;
                        if (doubleRange != null)
                        {
                            double diffSpan = doubleRange.Max - doubleRange.Min;
                            double relativeSpan = diffSpan * relativePosition;
                            double value = doubleRange.Min + relativeSpan;
                            this.SetCursorPosition(value);

                            double pixelPosition = this.GetCursorPixelPosition();
                            if (!double.IsNaN(pixelPosition))
                            {
                                _pixelCoordX = pixelPosition;
                                this.UpdateCursorPosition();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        //private void SetRelativePosition(double relativePosition)
        //{
        //    try
        //    {
        //        this._lastRelativePosition = relativePosition;
        //        if (!string.IsNullOrEmpty(this._cursor.XAxisId))
        //        {
        //            this._relativePositionNeedToBeSet = true;
        //            IAxis axis = this._sciChartControl.XAxes.GetAxisById(this._cursor.XAxisId);
        //            if (axis != null && axis.VisibleRange != null)
        //            {
        //                DateRange dateRange = axis.VisibleRange as DateRange;
        //                if (dateRange != null)
        //                {
        //                    TimeSpan diffSpan = dateRange.Max - dateRange.Min;
        //                    TimeSpan relativeSpan = new TimeSpan((long)(diffSpan.Ticks * relativePosition));
        //                    DateTime value = dateRange.Min.Add(relativeSpan);
        //                    this.SetCursorPosition(value);

        //                    double pixelPosition = this.GetCursorPixelPosition();
        //                    if (!double.IsNaN(pixelPosition))
        //                    {
        //                        _pixelCoordX = pixelPosition;
        //                        this.UpdateCursorPosition();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }            
        //}

        private void SetCursorPosition(double value)
        {
            this._cursor.X1 = value;
        }

        public void HideCursorHintText()
        {
            this.ShowTooltipOn = ShowTooltipOptions.Never;
        }

        //public void MoveTo(DateTime dateTime)
        //{
        //    this.SetCursorPosition(dateTime);
        //}

        public event EventHandler<DoubleEventArgs> DataCursorPositionChanged;

        private System.Windows.Media.SolidColorBrush _cursorColor;
        public Color CursorColor
        {
            get
            {
                return Color.FromArgb(_cursorColor.Color.A, _cursorColor.Color.R, _cursorColor.Color.G, _cursorColor.Color.B);
            }

            set
            {
                _cursorColor = new System.Windows.Media.SolidColorBrush(new System.Windows.Media.Color()
                {
                    A = value.A,
                    R = value.R,
                    G = value.G,
                    B = value.B
                });


                this._cursor.Stroke = _cursorColor;
                this._cursor.StrokeThickness = 2.0;
                this._cursor.Foreground = new System.Windows.Media.SolidColorBrush(SciChartModifiersHelpers.GetForegroundColorDependingOnBackgroundColor(_cursorColor.Color));
            }
        }

        public bool ShowCursorAxisLabel
        {
            get { return this._cursor.ShowLabel; }
            set
            {
                this._cursor.ShowLabel = value;
            }
        }

        public double PositionAsDouble { get; private set; }
    }
}