using ClientPltTurbine.Shared.ChartJsComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartJsComponent.DrawScatterChart.Implementation;

namespace ClientPltTurbine.Shared.ChartJsComponent.DrawScatterChart.Contract
{
    interface IScatterChart
    {
        string GetNameSetup();
        ConfigChart GetConfigChart(ScatterChart value);
    }
}
