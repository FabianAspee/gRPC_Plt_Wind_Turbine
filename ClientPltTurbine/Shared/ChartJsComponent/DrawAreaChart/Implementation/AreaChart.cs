using ClientPltTurbine.Shared.ChartJsComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartJsComponent.DrawAreaChart.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.ChartJsComponent.DrawAreaChart.Implementation
{
    public class AreaChart: ConfigChart, IAreaChart
    {
        public override string GetNameSetup() => "setupAreaChart";
    }
}
