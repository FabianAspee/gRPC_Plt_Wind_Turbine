using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Shared.ChartJsComponent.ConfigGeneral;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.RadarChartDraw.Contract
{
    public interface IRadarChartDraw
    {
        ConfigChart CreateRadarChartWithWarning(ResponseSerieByPeriodWarning responseSerieByPeriodWarning);
    }
}
