using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.RadarChartDraw.Contract
{
    public interface IRadarChartDraw
    {
        ConfigChart CreateRadarChartWithWarning(ResponseSerieByPeriodWarning responseSerieByPeriodWarning);
    }
}
