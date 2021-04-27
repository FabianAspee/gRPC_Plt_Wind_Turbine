using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.LineChart;
using ChartJs.Blazor.ScatterChart;
using ChartJs.Blazor.Util;
using ClientPltTurbine.Model.ChartModel.RecordChart;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.ScatterChartDraw.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.LineChartDraw.Implementation.LineChartDraw;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.ScatterChartDraw.Implementation
{
    public class ScatterChartDraw:BaseChart,  IScatterChartDraw
    {
        private static readonly Lazy<IScatterChartDraw> instance = new(() => new ScatterChartDraw());
        private ScatterChartDraw() { }
        public static IScatterChartDraw Instance => instance.Value; 
        
        public ConfigBase CreateScatterChart(ResponseSerieByPeriod responseSerieByPeriod)
        {
            var variant = _variants($"Scatter Turbine {responseSerieByPeriod.Record.NameTurbine} " +
                $"Sensor {responseSerieByPeriod.Record.NameSensor}");
            var lineConfigBase = CreateBaseScatterConfig(variant, responseSerieByPeriod);
            return CreateScatterChart(lineConfigBase, responseSerieByPeriod, variant);
        }
        private static ScatterConfig CreateBaseScatterConfig(Variant variant, IEventComponent period)
        {
            ScatterConfig ConfigLine = new()
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
        private static ConfigBase CreateScatterChart(ScatterConfig configLine, ResponseSerieByPeriod periods, Variant variant)
        {
            string steppedLineCamel = variant.SteppedLine.ToString();
            steppedLineCamel = char.ToUpperInvariant(steppedLineCamel[0]) + steppedLineCamel[1..];
            configLine.Data.Datasets.Add(new LineDataset<(string,double?)>(periods.Record.CustomInfo.Select(x =>(x.Date.ToShortDateString(),x.Value)).ToList())
            { 
                Fill = FillingMode.Disabled, 
                BackgroundColor = ColorUtil.FromDrawingColor(variant.Color),
                BorderColor = ColorUtil.FromDrawingColor(variant.Color),
                Label = $"Value sensor: {steppedLineCamel}", 
                PointBackgroundColor = ColorUtil.FromDrawingColor(variant.Color),
                BorderWidth = 2,
                PointRadius = 3,
                PointBorderWidth = 1,
                SteppedLine = variant.SteppedLine,
              
            });

            return configLine;
        }
        private static IEnumerable<string> SelectRecords(IEventComponent period) => period switch
        {
            ResponseSerieByPeriod value => value.Record.CustomInfo.Select(x => x.Date.ToShortDateString()).ToArray(),
            ResponseSerieByPeriodWarning value => value.Record.RecordLinearChart.CustomInfo.Select(x => x.Date.ToShortDateString()).ToArray(),
            _ => throw new NotImplementedException(),
        };
        public ConfigBase CreateScatterChartWithWarning(ResponseSerieByPeriodWarning responseSerieByPeriodWarning)
        {
            throw new NotImplementedException();
        }
    }
}
