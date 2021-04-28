using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartComponent.DrawLineChart.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.ChartComponent.DrawLineChart.Contract
{
    interface ILineChart
    {
        ConfigChart GetConfigChart(LineChart value);
    }
}
