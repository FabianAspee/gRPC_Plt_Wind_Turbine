using ChartJs.Blazor.Common;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.LineChartDraw.Contract
{
    public interface ILineChartDraw
    {
        ConfigBase CreateLineChart(ResponseSerieByPeriod responseSerieBy);
        ConfigBase CreateLineChartWarning(ResponseSerieByPeriodWarning serieByPeriodWarning);
    }
}
