using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.RadarChartDraw.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Shared.ChartJsComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartJsComponent.DrawRadarChart.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.RadarChartDraw.Implementation
{
    public class RadarChartDraw:BaseChart, IRadarChartDraw
    {
        private static readonly Lazy<RadarChartDraw> instance = new(() => new RadarChartDraw());
        private RadarChartDraw() { }
        public static RadarChartDraw Instance => instance.Value;
        public ConfigChart CreateRadarChartWithWarning(ResponseSerieByPeriodWarning responseSerieByPeriodWarning)
        {
            var variant = _variants($"Radar Turbine {responseSerieByPeriodWarning.Record.RecordLinearChart.NameTurbine} " +
               $"Sensor {responseSerieByPeriodWarning.Record.RecordLinearChart.NameSensor}");
            var firstFilter = responseSerieByPeriodWarning.Record.InfoTurbineWarnings.Where(value => value.Value != -1 && value.Value != 0); 
            var warning = firstFilter.GroupBy(info => info.Value).Select(info => (info.Key, info.Count())).ToList();
            var finalWarning =  warning.Select(value => value.Key.ToString()).ToList();
            responseSerieByPeriodWarning.Record.OriginalWarning.ForEach(val => finalWarning.Add(val));
            return new RadarChart()
            {
                Type = Shared.ChartJsComponent.ChartType.Radar.ToString().ToLower(),
                Options = new OptionChart(true, false, new Interaction(false), 0),
<<<<<<< HEAD
                Data = new DataChart(finalWarning, new[] { new DataSetChart(warning.
                Select(value => value.Item2.ToString()).ToArray(), variant.Title, "red", 
                BackgroundColor: "red") })
=======
                Data = new DataChart(finalWarning, new[] { new DataSetChart(warning.Select(value => $"warning {value.Key} qta {value.Item2.ToString()}").ToArray(), variant.Title, "red", BackgroundColor: "red") })
>>>>>>> 29de40746d4fca32f05a1d781072a6deee3682c7
            };
        }
    }
}
