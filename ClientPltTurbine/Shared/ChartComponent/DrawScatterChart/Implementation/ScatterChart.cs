using ClientPltTurbine.Shared.ChartComponent.DrawScatterChart.Contract;
using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClientPltTurbine.Shared.ChartComponent.DrawScatterChart.Implementation
{
    public class ScatterChart:ConfigChart,IScatterChart
    {
        public record ScatterData(double X, object Y);
        public ScatterChart() { }

        public ConfigChart GetConfigChart(ScatterChart value)=> new ScatterChart()
        {
            Type = value.Type,
            Options = value.Options,
            Data =    new DataChart(value.Data.Labels, value.Data.Datasets.Select(dataSet=>new DataSetChart(CreateNewData(value.Data.Labels.Zip(dataSet.Data).ToList()), dataSet.Label,dataSet.BackgroundColor, BackgroundColor: dataSet.BorderColor)).ToArray())
        };  

        private static object[] CreateNewData(List<(string Data, object Value)> enumerable)
        {
            var baseDate = new DateTime(1970, 01, 01,12,00,00); 
            return enumerable.Select(value =>new ScatterData(TotalMillisecond(value.Data,baseDate), value.Value)).ToArray();
        }
        private static double TotalMillisecond(string data, DateTime baseDate) => DateTime.Parse(data).Subtract(baseDate).TotalMilliseconds; 
        public override string GetNameSetup() => "setupScatterChart"; 
    }
}
