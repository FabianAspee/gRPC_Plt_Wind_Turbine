using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartComponent.DrawScatterChart.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.ChartComponent.DrawScatterChart.Contract
{
    interface IScatterChart
    {
        string GetNameSetup();
        ConfigChart GetConfigChart(ScatterChart value);
    }
}
