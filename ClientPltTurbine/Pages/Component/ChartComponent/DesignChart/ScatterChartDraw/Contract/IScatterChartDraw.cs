using ChartJs.Blazor.Common;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.ScatterChartDraw.Contract
{
    public interface IScatterChartDraw
    {
        ConfigChart CreateScatterChart(ResponseSerieByPeriod responseSerieByPeriod);
        ConfigChart CreateScatterChartWithWarning(ResponseSerieByPeriodWarning responseSerieByPeriodWarning);
    }
}
