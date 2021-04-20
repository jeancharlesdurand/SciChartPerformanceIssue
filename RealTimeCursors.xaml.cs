// *************************************************************************************
// SCICHART® Copyright SciChart Ltd. 2011-2021. All rights reserved.
//  
// Web: http://www.scichart.com
//   Support: support@scichart.com
//   Sales:   sales@scichart.com
// 
// RealTimeCursors.xaml.cs is part of the SCICHART® Examples. Permission is hereby granted
// to modify, create derivative works, distribute and publish any part of this source
// code whether for commercial, private or personal use. 
// 
// The SCICHART® examples are distributed in the hope that they will be useful, but
// without any warranty. It is provided "AS IS" without warranty of any kind, either
// expressed or implied. 
// *************************************************************************************
using System;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using RSI.IndissPlus.Plots.SC.Modifiers;
using SciChart.Charting;
using SciChart.Charting.ChartModifiers;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals;
using SciChart.Data.Model;
using SciChart.Drawing.VisualXcceleratorRasterizer;

namespace SciChart.Examples.Examples.CreateRealtimeChart
{
    public partial class RealTimeCursors : UserControl
    {
       // Data Sample Rate (sec) - 20 Hz
        private const double dt = 0.2;

        // FIFO Size is 100 samples, meaning after 100 samples have been appended, each new sample appended
        // results in one sample being discarded
        private const int FifoSize = 100;

        // Timer to process updates
        private readonly Timer _timerNewDataUpdate;

        // The current time
        private double t;

        // The dataseries to fill
        private readonly IXyDataSeries<double, double> _series0;
        private readonly IXyDataSeries<double, double> _series1;
        private readonly IXyDataSeries<double, double> _series2;
        private readonly IXyDataSeries<double, double> _series3;

        private SciChartXYCursor _cursor;

        public RealTimeCursors()
        {
            InitializeComponent();

            _cursor = new SciChartXYCursor(this.sciChartSurface);
            ModifierGroup modifierGroup = new ModifierGroup();
            modifierGroup.ChildModifiers.Add(_cursor);
            this.sciChartSurface.ChartModifier = modifierGroup;

            _timerNewDataUpdate = new Timer(dt * 1000) {AutoReset = true};
            _timerNewDataUpdate.Elapsed += OnNewData;

            // Create new Dataseries of type X=double, Y=double
            _series0 = new XyDataSeries<double, double> {SeriesName = "Orange Series"};
            _series1 = new XyDataSeries<double, double> {SeriesName = "Blue Series"};
            _series2 = new XyDataSeries<double, double> {SeriesName = "Green Series"};
            _series3 = new XyDataSeries<double, double> {SeriesName = "Purple Series"};

            // Set the dataseries on the chart's RenderableSeries
            renderableSeries0.DataSeries = _series0;
            renderableSeries1.DataSeries = _series1;
            renderableSeries2.DataSeries = _series2;
            renderableSeries3.DataSeries = _series3;
        }

        public void ShowCursor(bool visible)
        {
            if (visible)
            {
                _cursor.SetInitialRelativePosition(0.5);
            }

            this._cursor.SetVisible(visible);
        }

        private void ClearDataSeries()
        {
            using (sciChartSurface.SuspendUpdates())
            {
                _series0?.Clear();
                _series1?.Clear();
                _series2?.Clear();
                _series3?.Clear();
            }
        }

        private void OnNewData(object sender, EventArgs e)
        {
            // Compute our three series values
            double y1 = 3.0 * Math.Sin(2 * Math.PI * 1.4 * t * 0.02);
            double y2 = 2.0 * Math.Cos(2 * Math.PI * 0.8 * t * 0.02);
            double y3 = 1.0 * Math.Sin(2 * Math.PI * 2.2 * t * 0.02);
            double y4 = 0.5 * Math.Sin(2 * Math.PI * 3.3 * t * 0.02);

            // Suspending updates is optional, and ensures we only get one redraw
            // once all three dataseries have been appended to
            using (sciChartSurface.SuspendUpdates())
            {
                // Append x,y data to previously created series
                _series0.Append(t, y1);
                _series1.Append(t, y2);
                _series2.Append(t, y3);
                _series3.Append(t, y4);
                sciChartSurface.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() => this.UpdateXAxisIfNeeded(t)));
            }

            // Increment current time
            t += dt;
        }

        private void UpdateXAxisIfNeeded(double t)
        {
            if (this._xAxis.VisibleRange != null && this._xAxis.VisibleRange.IsDefined && (this._xAxis.VisibleRange as DoubleRange).Max < t)
            {
                this._xAxis.VisibleRange.SetMinMax(t - 50, t);
            }
        }

        private void OnExampleLoaded(object sender, RoutedEventArgs e)
        {
            ClearDataSeries();

            _timerNewDataUpdate.Start();
        }

        private void OnExampleUnloaded(object sender, RoutedEventArgs e)
        {
            _timerNewDataUpdate?.Stop();
        }

        private void SciChartSurface_OnLoaded(object sender, RoutedEventArgs e)
        {
            Type renderer = sciChartSurface.RenderSurface.GetType();
            bool gpu = VisualXcceleratorEngine.HasDirectX10OrBetterCapableGpu;
            bool acc = VisualXcceleratorEngine.SupportsHardwareAcceleration;
            Debug.WriteLine("Renderer: {0}", renderer);
            Debug.WriteLine("HasDirectX10OrBetterCapableGpu: {0}", gpu);
            Debug.WriteLine("SupportsHardwareAcceleration: {0}", acc);

            if (renderer != typeof(VisualXcceleratorRenderSurface))
            {
                Debug.WriteLine("Restart engine");
                VisualXcceleratorRenderSurface.RestartEngineWith(DirectXMode.DirectX11);
            }
        }
    }
}
