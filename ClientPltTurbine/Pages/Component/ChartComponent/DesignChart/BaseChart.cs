﻿using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart
{
    public abstract class BaseChart
    {
        protected class Variant
        {
            public bool SteppedLine { get; set; }
            public string Title { get; set; }
            public Color Color { get; set; }
        }
        protected readonly Func<string, Variant> _variants = title =>
             new()
             {
                 SteppedLine = false,
                 Title = title,
                 Color = Color.Red
             };
        protected string[] GetWarningColor(string[] warning)
        {
            Random random = new();
            var palletcolor = warning.GroupBy(value => value).Select(grouping => {
                Color randomColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                return (grouping.Key,$"rgb({randomColor.R},{randomColor.G},{randomColor.B})");
            }).ToList();
            return warning.Select(value => palletcolor.Find(colorKey => colorKey.Key.Equals(value)).Item2).ToArray(); 
        }
        protected static IEnumerable<string> SelectRecords(IEventComponent period) => period switch
        {
            ResponseSerieByPeriod value => value.Record.CustomInfo.Select(x => x.Date.ToString("yyyy/MM/dd HH:mm:ss")),
            ResponseSerieByPeriodWarning value => value.Record.RecordLinearChart.CustomInfo.Select(x => x.Date.ToString("yyyy/MM/dd HH:mm:ss")),
            _ => throw new NotImplementedException(),
        };
    }
}