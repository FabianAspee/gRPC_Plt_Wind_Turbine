using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Shared.Chart3DComponent.ConfigGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.ModelPredictionComponent.DesignStatus.CalendarChartDraw.Contract
{
    interface ICalendarChartDraw
    {
        ConfigChart3D CreateCalendarChart(ResponseSerieByPeriod responseSerieBy);
    }
}
