
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Shared.ChartJsComponent.ConfigGeneral;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.LineChartDraw.Contract
{
    public interface ILineChartDraw
    {
        ConfigChart CreateLineChart(ResponseSerieByPeriod responseSerieBy);
        ConfigChart CreateLineChart(ResponseSerieByPeriodWarning serieByPeriodWarning);
        ConfigChart CreateLineChartWarningInPeriod(ResponseSerieByPeriodWarning serieByPeriodWarning);
        ConfigChart CreateLineChart(ResponseSerieByMaintenancePeriod serieByPeriodMaintenance);
    }
}
