﻿using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.LineChartDraw.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartComponent.DrawLineChart.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    new DataSetChart(warning, variant.Title, colors, BackgroundColor:colors)}.ToArray())
            }; 
        }

        public ConfigChart CreateLineChartWarningInPeriod(ResponseSerieByPeriodWarning serieByPeriodWarning)
        {
            var variant = _variants($"Warning Turbine {serieByPeriodWarning.Record.RecordLinearChart.NameTurbine} Sensor {serieByPeriodWarning.Record.RecordLinearChart.NameSensor} with warnings");

            var firstFilter = serieByPeriodWarning.Record.InfoTurbineWarnings.Where(value =>value.Value.HasValue && value.Value != -1 && value.Value != 0);
            var warning = firstFilter.GroupBy(info => info.Value).Select(info => info.Select(val=>(val.Date,val.Value)).ToList()).ToList();
            var initSerie = serieByPeriodWarning.Record.RecordLinearChart.CustomInfo.First().Date;
            var finalSerie = serieByPeriodWarning.Record.RecordLinearChart.CustomInfo.Last().Date;
            (int week, List<(DateTime, DateTime)> initFinishWeek) = CalculusWeekAndInitFinishWeek(finalSerie,initSerie);
            var weekList = Enumerable.Range(0, week).Select(val => val.ToString()).ToList();
            var colors = GetWarningColor(warning.Select(warning => warning.First().Value.ToString()).ToArray()); 
            return new LineChart()
            {
                Type = Shared.ChartComponent.ChartType.Line.ToString().ToLower(),
                Options = new OptionChart(true, false, new Interaction(false), 0),
                Data = new DataChart(weekList,warning.Zip(colors).Select(warningVal=> GetInfoWarningByWeek(initFinishWeek, warningVal.First, warningVal.Second)).ToArray())
            };
        }
        private static (int, List<(DateTime,DateTime)>) CalculusWeekAndInitFinishWeek(DateTime finalSerie, DateTime initSerie)
        {
            int week = (finalSerie - initSerie).Days / 7;
            week = (finalSerie - initSerie).Days % 7 == 0 ? week : week + 1;
            var dayWeek = Convert.ToInt32(initSerie.DayOfWeek) != 7?(initSerie,initSerie.AddDays(7-Convert.ToInt32(initSerie.DayOfWeek))):(initSerie,initSerie);
            var weekDate = new List<(DateTime, DateTime)>() { dayWeek };
            List<(DateTime, DateTime)> _CalculusWeekAndInitfinishWeek(DateTime newWeek, List<(DateTime, DateTime)> weekDate) => newWeek switch
            {
                DateTime initWeek when (finalSerie - initWeek).TotalDays > 7 => _CalculusWeekAndInitfinishWeek(initWeek.AddDays(7), weekDate.Append((initWeek, initWeek.AddDays(6))).ToList()),
                DateTime initWeek when (finalSerie - initWeek).TotalDays < 7 && initWeek<finalSerie => _CalculusWeekAndInitfinishWeek(initWeek.AddDays(7), weekDate.Append((initWeek, finalSerie)).ToList()),
                _ => weekDate,
            };
            var newWeek = initSerie.AddDays(8 - Convert.ToInt32(initSerie.DayOfWeek));
            return (week, _CalculusWeekAndInitfinishWeek(newWeek, weekDate));
        }
        private static DataSetChart GetInfoWarningByWeek(List<(DateTime, DateTime)> weekList, List<(DateTime Date, double? Value)> warningVal, string color)
        {
            var count= weekList.Select(week =>warningVal.FindAll(warning => warning.Date >= week.Item1 && warning.Date <= week.Item2).Count.ToString()).Reverse().ToArray();
            return new DataSetChart(count, warningVal.First().Value.ToString(), color);
        }
    }
}
