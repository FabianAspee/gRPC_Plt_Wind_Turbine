using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.LineChartDraw.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartComponent.DrawLineChart.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ClientPltTurbine.Pages.Component.ChartComponent.Charts;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.LineChartDraw.Implementation
{
    public class LineChartDraw:BaseChart, ILineChartDraw
    {
        private static readonly Lazy<ILineChartDraw> instance = new(() => new LineChartDraw());
        private LineChartDraw() { }
        public static ILineChartDraw Instance => instance.Value;

        public ConfigChart CreateLineChart(ResponseSerieByPeriod responseSerieBy)
        {
            var variant = _variants($"Serie Turbine {responseSerieBy.Record.NameTurbine} Sensor {responseSerieBy.Record.NameSensor}");
            var data = responseSerieBy.Record.CustomInfo.Select(value => value.Value.HasValue?value.Value.ToString():null).ToArray();

            return new LineChart()
            {
                Type = Shared.ChartComponent.ChartType.Line.ToString().ToLower(),
                Options = new OptionChart(true, false, new Interaction(false), 0),
                Data = new DataChart(SelectRecords(responseSerieBy).ToList(), new[]{new DataSetChart(
                    data, variant.Title,"rgb(192,75,75)")}.ToArray())
            }; 
             
        }
        public ConfigChart CreateLineChartWarning(ResponseSerieByPeriodWarning serieByPeriodWarning)
        {
            var variant = _variants($"Serie Turbine {serieByPeriodWarning.Record.RecordLinearChart.NameTurbine} Sensor {serieByPeriodWarning.Record.RecordLinearChart.NameSensor} with warnings");

            var data = serieByPeriodWarning.Record.RecordLinearChart.CustomInfo.Select(value => value.Value.HasValue ? value.Value.ToString() : null).ToArray();
            var warning = serieByPeriodWarning.Record.InfoTurbineWarnings.Select(value => value.Value.HasValue ? value.Value.ToString() : null).ToArray();
            var colors = GetWarningColor(warning);
            return new LineChart()
            {
                Type = Shared.ChartComponent.ChartType.Line.ToString().ToLower(),
                Options = new OptionChart(true, false, new Interaction(false), 0),
                Data = new DataChart(SelectRecords(serieByPeriodWarning).ToList(), new[]{new DataSetChart(data, variant.Title, "rgb(192,75,75)" ),
                    new DataSetChart(warning, variant.Title, colors,colors)}.ToArray())
            }; 
        } 
         
    }
}
