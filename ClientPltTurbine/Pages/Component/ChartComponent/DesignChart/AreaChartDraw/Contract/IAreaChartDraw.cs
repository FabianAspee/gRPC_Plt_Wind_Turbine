using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Shared.ChartJsComponent.ConfigGeneral;

namespace ClientPltTurbine.Pages.Component.ChartJsComponent.DesignChart.AreaChartDraw.Contract
{
    interface IAreaChartDraw
    {
        ConfigChart CreateAreaChartWarning(ResponseSerieByPeriodWarning serieByPeriodWarning);
    }
}
