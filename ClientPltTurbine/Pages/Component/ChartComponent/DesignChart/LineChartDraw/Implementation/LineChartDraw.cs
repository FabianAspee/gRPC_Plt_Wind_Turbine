using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.LineChart;
using ChartJs.Blazor.Util;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.LineChartDraw.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
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

        public ConfigBase CreateLineChart(ResponseSerieByPeriod responseSerieBy)
        {
            var variant = _variants($"Serie Turbine {responseSerieBy.Record.NameTurbine} Sensor {responseSerieBy.Record.NameSensor}");
            var lineConfigBase = CreateBaseLineConfig(variant, responseSerieBy);
            return CreateLineChart(lineConfigBase, responseSerieBy, variant);
        }
        public ConfigBase CreateLineChartWarning(ResponseSerieByPeriodWarning serieByPeriodWarning)
        {
            var variant = _variants($"Serie Turbine {serieByPeriodWarning.Record.RecordLinearChart.NameTurbine} Sensor {serieByPeriodWarning.Record.RecordLinearChart.NameSensor} with warnings");
            var lineConfigBase = CreateBaseLineConfig(variant, serieByPeriodWarning);
            return CreateLineChartWarning(lineConfigBase, serieByPeriodWarning, variant);
             
        }
        private static LineConfig CreateBaseLineConfig(Variant variant,IEventComponent period)
        {
            LineConfig ConfigLine = new()
            {
                Options = new LineOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = variant.Title
                    }
                }
            }; 
            ConfigLine.Data.Labels.AddRange(SelectRecords(period));
            return ConfigLine;
        }

        private static IEnumerable<string> SelectRecords(IEventComponent period) => period switch
        {
            ResponseSerieByPeriod value => value.Record.CustomInfo.Select(x => x.Date.ToShortDateString()).ToArray(),
            ResponseSerieByPeriodWarning value => value.Record.RecordLinearChart.CustomInfo.Select(x => x.Date.ToShortDateString()).ToArray(),
            _ => throw new NotImplementedException(),
        };

        private static ConfigBase CreateLineChart(LineConfig configLine, ResponseSerieByPeriod periods, Variant variant)
        { 
            string steppedLineCamel = variant.SteppedLine.ToString();
            steppedLineCamel = char.ToUpperInvariant(steppedLineCamel[0]) + steppedLineCamel[1..];
            configLine.Data.Datasets.Add(new LineDataset<double?>(periods.Record.CustomInfo.Select(x => x.Value).ToList())
            {
                
                Label = $"Value sensor: {steppedLineCamel}",
                SteppedLine = variant.SteppedLine,
                BorderColor = ColorUtil.FromDrawingColor(variant.Color),
                Fill = FillingMode.Disabled
            });

            return configLine; 
        }
        private static ConfigBase CreateLineChartWarning(LineConfig configLine, ResponseSerieByPeriodWarning periods, Variant variant)
        {
            string steppedLineCamel = variant.SteppedLine.ToString();
            steppedLineCamel = char.ToUpperInvariant(steppedLineCamel[0]) + steppedLineCamel[1..];
            configLine.Data.Datasets.Add(new LineDataset<double?>(periods.Record.RecordLinearChart.CustomInfo.Select(x => x.Value).ToList())
            {

                Label = $"Value sensor: {steppedLineCamel}",
                SteppedLine = variant.SteppedLine,
                BorderColor = ColorUtil.FromDrawingColor(variant.Color),
                Fill = FillingMode.Disabled
            });
          

            configLine.Data.Datasets.Add( new LineDataset<double?>(periods.Record.InfoTurbineWarnings.Select(x => x.Value).ToList())
            { 
                Label = $"Value sensor: {steppedLineCamel}", 
                SteppedLine = variant.SteppedLine,
                BorderColor = ColorUtil.FromDrawingColor(ChartColors.Blue),
                Fill = FillingMode.Disabled

            });
            return configLine;
        }

       
         
    }
}
