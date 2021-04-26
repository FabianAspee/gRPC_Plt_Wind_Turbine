using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.LineChart;
using ChartJs.Blazor.Util;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ClientPltTurbine.Pages.Component.ChartComponent.Charts;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.LineChartDraw.Implementation
{
    public class LineChartDraw
    {
        private static readonly Lazy<LineChartDraw> instance = new(() => new LineChartDraw());
        private LineChartDraw() { }
        public static LineChartDraw Instance => instance.Value;

        public ConfigBase CreateLineChart(ResponseSerieByPeriod responseSerieBy)
        {
            throw new NotImplementedException(); 
        }
        public ConfigBase CreateLineChartWarning(ResponseSerieByPeriodWarning serieByPeriodWarning)
        {
            throw new NotImplementedException();
        }
        private ConfigBase CreateLineChart(IEventComponent periods)
        {
            var title = "Test";
            var period = periods as ResponseSerieByPeriod;
            var variant = _variants(title);
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

            string steppedLineCamel = variant.SteppedLine.ToString();
            steppedLineCamel = char.ToUpperInvariant(steppedLineCamel[0]) + steppedLineCamel.Substring(1);

            ConfigLine.Data.Labels.AddRange(period.Record.CustomInfo.Select(x => x.Date.ToShortDateString()).ToArray());
            ConfigLine.Data.Datasets.Add(new LineDataset<double?>(period.Record.CustomInfo.Select(x => x.Value).ToList())
            {
                Label = $"SteppedLine: SteppedLine.{steppedLineCamel}",
                SteppedLine = variant.SteppedLine,
                BorderColor = ColorUtil.FromDrawingColor(variant.Color),
                Fill = FillingMode.Disabled
            });

            return ConfigLine;

        }

        public class Variant
        {
            public SteppedLine SteppedLine { get; set; }
            public string Title { get; set; }
            public System.Drawing.Color Color { get; set; }
        }
        private readonly Func<string,Variant> _variants = title => 
            new()
            {
                SteppedLine = SteppedLine.False,
                Title = title,
                Color = ChartColors.Red
            };
         
    }
}
