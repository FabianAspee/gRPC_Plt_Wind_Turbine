using ChartJs.Blazor.Common;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.ScatterChartDraw.Contract
{
    public interface IScatterChartDraw
    {
        ConfigBase CreateScatterChart(ResponseSerieByPeriod responseSerieByPeriod);
        ConfigBase CreateScatterChartWithWarning(ResponseSerieByPeriodWarning responseSerieByPeriodWarning);
    }
}
