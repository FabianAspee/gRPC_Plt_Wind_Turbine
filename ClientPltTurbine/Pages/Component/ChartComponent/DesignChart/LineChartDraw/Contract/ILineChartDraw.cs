
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.LineChartDraw.Contract
{
    public interface ILineChartDraw
    {
        ConfigChart CreateLineChart(ResponseSerieByPeriod responseSerieBy);
        ConfigChart CreateLineChartWarning(ResponseSerieByPeriodWarning serieByPeriodWarning);
        ConfigChart CreateLineChartWarningInPeriod(ResponseSerieByPeriodWarning serieByPeriodWarning);
    }
}
