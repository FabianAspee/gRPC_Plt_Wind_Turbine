using ClientPltTurbine.Shared.ChartJsComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartJsComponent.DrawBoxPlotChart.Contract;
using System;

namespace ClientPltTurbine.Shared.ChartJsComponent.DrawBoxPlotChart.Implementation
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
