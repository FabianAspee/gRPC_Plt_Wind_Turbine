using ClientPltTurbine.Shared.ChartJsComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartJsComponent.DrawLineChart.Implementation;

namespace ClientPltTurbine.Shared.ChartJsComponent.DrawLineChart.Contract
{
    interface ILineChart
    {
        ConfigChart GetConfigChart(LineChart value);
    }
}
