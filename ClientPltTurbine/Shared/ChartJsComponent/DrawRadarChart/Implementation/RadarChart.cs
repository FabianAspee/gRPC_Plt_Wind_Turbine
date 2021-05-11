using ClientPltTurbine.Shared.ChartJsComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartJsComponent.DrawRadarChart.Contract;

namespace ClientPltTurbine.Shared.ChartJsComponent.DrawRadarChart.Implementation
{
    public class RadarChart:ConfigChart, IRadarChart
    {
        public override string GetNameSetup() => "setupRadarChart";
    }
}
