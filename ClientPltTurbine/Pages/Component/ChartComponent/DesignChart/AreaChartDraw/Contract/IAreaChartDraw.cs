using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.AreaChartDraw.Contract
{
    interface IAreaChartDraw
    {
        ConfigChart CreateAreaChartWarning(ResponseSerieByPeriodWarning serieByPeriodWarning);
    }
}
