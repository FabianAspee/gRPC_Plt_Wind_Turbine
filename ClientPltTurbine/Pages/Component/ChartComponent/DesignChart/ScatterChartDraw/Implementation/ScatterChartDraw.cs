using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.ScatterChartDraw.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartComponent.DrawScatterChart.Implementation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.ScatterChartDraw.Implementation
{
    public class ScatterChartDraw:BaseChart,  IScatterChartDraw
    {
        private static readonly Lazy<IScatterChartDraw> instance = new(() => new ScatterChartDraw());
        private ScatterChartDraw() { }
        public static IScatterChartDraw Instance => instance.Value; 
        
        public ConfigChart CreateScatterChart(ResponseSerieByPeriod responseSerieByPeriod)
        {
            var variant = _variants($"Scatter Turbine {responseSerieByPeriod.Record.NameTurbine} " +
                $"Sensor {responseSerieByPeriod.Record.NameSensor}");
            var data = responseSerieByPeriod.Record.CustomInfo.Select(value =>value.Value.ToString()).ToArray();

            return new ScatterChart()
            {
                Type = Shared.ChartComponent.ChartType.Scatter.ToString().ToLower(),
                Options = new OptionChart(true, false, new Interaction(false), 0),
                Data = new DataChart(SelectRecords(responseSerieByPeriod).ToList(), new[]{new DataSetChart(data, variant.Title, "red", BackgroundColor: "red") }.ToArray())
            };
            
        } 
        public ConfigChart CreateScatterChartWithWarning(ResponseSerieByPeriodWarning responseSerieByPeriodWarning)
        {
            var variant = _variants($"Scatter Turbine {responseSerieByPeriodWarning.Record.RecordLinearChart.NameTurbine} " +
               $"Sensor {responseSerieByPeriodWarning.Record.RecordLinearChart.NameSensor}");
            var data = responseSerieByPeriodWarning.Record.RecordLinearChart.CustomInfo.Select(value => value.Value.ToString()).ToArray();
            var warning = responseSerieByPeriodWarning.Record.InfoTurbineWarnings.Select(value => value.Value.ToString()).ToArray();
            var colors = GetWarningColor(warning);
            return new ScatterChart()
            {
                Type = Shared.ChartComponent.ChartType.Scatter.ToString().ToLower(),
                Options = new OptionChart(true, false, new Interaction(false), 0),
                Data = new DataChart(SelectRecords(responseSerieByPeriodWarning).ToList(), new[]{new DataSetChart(data, variant.Title, "red", BackgroundColor: "red"),
                    new DataSetChart(warning, variant.Title, colors, BackgroundColor:colors)}.ToArray())
            };
        }
       
    }
}
