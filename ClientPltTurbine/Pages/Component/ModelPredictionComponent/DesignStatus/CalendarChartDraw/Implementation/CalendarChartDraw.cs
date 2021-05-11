using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Pages.Component.ModelPredictionComponent.DesignStatus.CalendarChartDraw.Contract;
using ClientPltTurbine.Shared.Chart3DComponent.ConfigGeneral;
using System;

namespace ClientPltTurbine.Pages.Component.ModelPredictionComponent.DesignStatus.CalendarChartDraw.Implementation
{
    public class CalendarChartDraw: ICalendarChartDraw
    {
        private static readonly Lazy<CalendarChartDraw> instance = new(() => new CalendarChartDraw());
        private CalendarChartDraw() { }
        public static CalendarChartDraw Instance => instance.Value;

        public ConfigChart3D CreateCalendarChart(ResponseSerieByPeriod responseSerieBy)
        {
            throw new NotImplementedException();

        }
    }
}
