using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.ScatterChartDraw.Contract
{
    public interface IScatterChartDraw
    {
        ConfigChart CreateScatterChart(ResponseSerieByPeriod responseSerieByPeriod);
        ConfigChart CreateScatterChartWithWarning(ResponseSerieByPeriodWarning responseSerieByPeriodWarning);
    }
}
