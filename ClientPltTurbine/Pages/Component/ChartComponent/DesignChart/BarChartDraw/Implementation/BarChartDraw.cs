using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.BarChartDraw.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartComponent.DrawBarChart.Implementation;
using System;
using System.Linq;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.BarChartDraw.Implementation
{
    public class BarChartDraw :BaseChart, IBarChartDraw
    {
        private static readonly Lazy<BarChartDraw> instance = new(() => new BarChartDraw());
        private BarChartDraw() { }
        public static BarChartDraw Instance => instance.Value;
        public ConfigChart CreateBarChartWarning(ResponseSerieByPeriodWarning serieByPeriodWarning)
        {
            var variant = _variants($"Serie Turbine {serieByPeriodWarning.Record.RecordLinearChart.NameTurbine} Sensor {serieByPeriodWarning.Record.RecordLinearChart.NameSensor} with warnings");
             
            var firstFilter = serieByPeriodWarning.Record.InfoTurbineWarnings.Where(value =>value.Value.HasValue && value.Value != -1 && value.Value != 0).Select(val=>val.Value.ToString()).ToList();
            firstFilter.AddRange(serieByPeriodWarning.Record.OriginalWarning.ToList());
            var warning = firstFilter.GroupBy(info => info).Select(info => (info.Key, info.Count())).ToList();
            var finalWarning = warning.Select(value => value.Key.ToString()).ToList(); 
             
            return new BarChart()
            {
                Type = Shared.ChartComponent.ChartType.Bar.ToString().ToLower(),
                Options = new OptionChart(true, false, new Interaction(false), 0),
                Data = new DataChart(finalWarning, new[] { new DataSetChart(warning.Select(val=>val.Item2.ToString()).ToArray(), variant.Title, "rgba(255, 99, 132, 1)") })
            };
        }
    }
}
