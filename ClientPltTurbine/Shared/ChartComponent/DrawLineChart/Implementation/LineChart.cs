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
        public override string GetNameSetup() => "setupLineChart";
        
    }
}
