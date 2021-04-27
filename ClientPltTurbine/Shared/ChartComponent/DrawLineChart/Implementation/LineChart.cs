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
        public LineChart() { } 

        public Task CreateChart(LineChart lineChart, string Id)
        { 
            var config2 = new
            {
                Type = lineChart.Type.ToString().ToLower(),
                Options = new OptionChart(lineChart.Options.Responsive,lineChart.Options.Fill,lineChart.Options.Interaction,lineChart.Options.Radius),              
                Data = new DataChart(lineChart.Data.Labels,lineChart.Data.Datasets) 
            };
            return null;
        }
    }
}
