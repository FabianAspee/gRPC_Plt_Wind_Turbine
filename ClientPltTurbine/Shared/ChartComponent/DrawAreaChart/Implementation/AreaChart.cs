using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartComponent.DrawAreaChart.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.ChartComponent.DrawAreaChart.Implementation
{
    public class AreaChart: ConfigChart, IAreaChart
    {
        public override string GetNameSetup() => "setupAreaChart";
    }
}
