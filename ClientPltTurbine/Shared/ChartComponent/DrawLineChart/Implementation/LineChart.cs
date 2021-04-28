using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartComponent.DrawLineChart.Contract; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.ChartComponent.DrawLineChart.Implementation
{
    public class LineChart: ConfigChart, ILineChart
    {
        public ConfigChart GetConfigChart(LineChart value) => new LineChart()
        {
            Type = value.Type,
            Options = value.Options,
            Data = new DataChart(value.Data.Labels, value.Data.Datasets.Select(dataSet => 
            new DataSetChart(dataSet.Data, dataSet.Label, dataSet.BorderColor, dataSet.BackgroundColor ?? dataSet.BorderColor)).ToArray())
        };

        public override string GetNameSetup() => "setupLineChart";
        
    }
}
