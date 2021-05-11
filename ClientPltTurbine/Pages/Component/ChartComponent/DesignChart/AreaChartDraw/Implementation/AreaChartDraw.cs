using ClientPltTurbine.Pages.Component.ChartJsComponent.DesignChart.AreaChartDraw.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Shared.ChartJsComponent.ConfigGeneral;
using System;
using System.Linq;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.AreaChartDraw.Implementation
{
    public class AreaChartDraw :BaseChart, IAreaChartDraw
    { 
        private static readonly Lazy<AreaChartDraw> instance = new(() => new AreaChartDraw());
        private AreaChartDraw() { }
        public static AreaChartDraw Instance => instance.Value;
        public ConfigChart CreateAreaChartWarning(ResponseSerieByPeriodWarning serieByPeriodWarning)
        {
            var variant = _variants($"Area Chart Turbine {serieByPeriodWarning.Record.RecordLinearChart.NameTurbine} " +
                $"Sensor {serieByPeriodWarning.Record.RecordLinearChart.NameSensor}");
            var firstFilter = serieByPeriodWarning.Record.InfoTurbineWarnings.Where(value => value.Value != -1 && value.Value != 0);
            //var warning = firstFilter.GroupBy(info => info.Value).Select(value=>value.Select(val=>val.)).ToList();
            return null;/* new AreaChart()
            {
                Type = Shared.ChartComponent.ChartType.Radar.ToString().ToLower(),
                Options = new OptionChart(true, false, new Interaction(false), 0),
                Data = new DataChart(warning.Select(value => value.Key.ToString()).ToList(), new[] { new DataSetChart(warning.Select(value => value.Item2.ToString()).ToArray(), variant.Title, "red", "red") })
            };*/
        }
    }
}
