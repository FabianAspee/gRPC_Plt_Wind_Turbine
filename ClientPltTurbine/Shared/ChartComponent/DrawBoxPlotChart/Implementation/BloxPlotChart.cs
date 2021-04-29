using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartComponent.DrawBoxPlotChart.Contract;
using System;

namespace ClientPltTurbine.Shared.ChartComponent.DrawBoxPlotChart.Implementation
{
    public class BloxPlotChart: ConfigChart, IBoxPlotChart
    {
        public ConfigChart GetConfigChart(BloxPlotChart value)
        {
            throw new NotImplementedException();
        }

        public override string GetNameSetup() => "setupBoxPlotChart";
    }
}
