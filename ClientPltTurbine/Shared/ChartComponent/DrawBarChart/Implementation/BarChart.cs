using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartComponent.DrawBarChart.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.ChartComponent.DrawBarChart.Implementation
{
    public class BarChart: ConfigChart, IBarChart
    {

        public override string GetNameSetup() => "setupBarChart";
    }
}
