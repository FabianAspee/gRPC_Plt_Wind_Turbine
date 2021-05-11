using ClientPltTurbine.Shared.ChartJsComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartJsComponent.DrawLineChart.Contract;
using System.Linq;

namespace ClientPltTurbine.Shared.ChartJsComponent.DrawLineChart.Implementation
{
    public class LineChart: ConfigChart, ILineChart
    {
        public ConfigChart GetConfigChart(LineChart value) => new LineChart()
        {
            Type = value.Type,
            Options = value.Options,
            Data = new DataChart(value.Data.Labels, value.Data.Datasets.Select(dataSet => 
            new DataSetChart(dataSet.Data, dataSet.Label, dataSet.BorderColor, BackgroundColor: dataSet.BackgroundColor ?? dataSet.BorderColor)).ToArray())
        };

        public override string GetNameSetup() => "setupLineChart";
        
    }
}
