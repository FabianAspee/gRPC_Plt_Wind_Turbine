using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartComponent.DrawBoxPlotChart.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.ChartComponent.DrawBoxPlotChart.Contract
{
    public interface IBoxPlotChart
    {
        string GetNameSetup();
        ConfigChart GetConfigChart(BloxPlotChart value);
    }
}
