using ClientPltTurbine.Shared.ChartJsComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartJsComponent.DrawBoxPlotChart.Implementation;

namespace ClientPltTurbine.Shared.ChartJsComponent.DrawBoxPlotChart.Contract
{
    public interface IBoxPlotChart
    {
        string GetNameSetup();
        ConfigChart GetConfigChart(BloxPlotChart value);
    }
}
