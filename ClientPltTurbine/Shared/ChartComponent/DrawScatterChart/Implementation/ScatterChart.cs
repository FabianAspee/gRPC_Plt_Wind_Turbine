using ClientPltTurbine.Shared.ChartComponent.DrawScatterChart.Contract;
using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.ChartComponent.DrawScatterChart.Implementation
{
    public class ScatterChart:ConfigChart,IScatterChart
    { 
        public ScatterChart() { }
        public override string GetNameSetup() => "setupScatterChart"; 
    }
}
