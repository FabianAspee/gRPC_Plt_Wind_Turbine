using System;

namespace ClientPltTurbine.Pages.Component.ChartComponent.EventChart
{
    public interface IEventChart
    { 
            public event EventHandler<IEventComponent> InfoChart;
    }
    public record LoadStatusChart(int TypeMsg, string Msg, string NameTurbine) : IEventComponent;
    public record ResponseSerieByPeriod(string NameTurbine, bool IsFinish, string Values="") : IEventComponent;
    public record ResponseSerieByPeriodWithStandardDeviation(ResponseSerieByPeriod InfoSerie, double StandardDeviation) : IEventComponent;

}
