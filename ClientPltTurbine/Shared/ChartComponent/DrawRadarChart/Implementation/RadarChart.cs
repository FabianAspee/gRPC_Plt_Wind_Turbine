using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartComponent.DrawRadarChart.Contract;

namespace ClientPltTurbine.Shared.ChartComponent.DrawRadarChart.Implementation
{
    public class RadarChart:ConfigChart, IRadarChart
    {
        public override string GetNameSetup() => "setupRadarChart";
    }
}
