using ChartJs.Blazor.Common;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.LineChartDraw.Contract
{
    public interface ILineChartDraw
    {
        ConfigChart CreateLineChart(ResponseSerieByPeriod responseSerieBy);
        ConfigChart CreateLineChartWarning(ResponseSerieByPeriodWarning serieByPeriodWarning);
    }
}
