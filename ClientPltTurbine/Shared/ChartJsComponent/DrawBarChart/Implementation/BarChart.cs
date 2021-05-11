using ClientPltTurbine.Shared.ChartJsComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartJsComponent.DrawBarChart.Contract;

namespace ClientPltTurbine.Shared.ChartJsComponent.DrawBarChart.Implementation
{
    public class BarChart: ConfigChart, IBarChart
    {

        public override string GetNameSetup() => "setupBarChart";
    }
}
