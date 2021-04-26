using ClientPltTurbine.Model.ChartModel.RecordChart;
using System;
using System.Collections.Generic;

namespace ClientPltTurbine.Pages.Component.ChartComponent.EventChart
{
    public interface IEventChart
    { 
            public event EventHandler<IEventComponent> InfoChart;
    }
    public record LoadStatusChart(int TypeMsg, string Msg, string NameTurbine) : IEventComponent;
    public record ResponseSerieByPeriod(bool IsFinish, RecordLinearChart Record) : IEventComponent;
    public record ResponseSerieByPeriodWithStandardDeviation(ResponseSerieByPeriod InfoSerie, double StandardDeviation) : IEventComponent;
    public record TurbineInfo(int IdTurbine, string NameTurbine) : IEventComponent;
    public record AllTurbineInfo(List<TurbineInfo> TurbineInfos) : IEventComponent;
    public record SensorInfo(int IdSensor, string NameSensor) : IEventComponent;
    public record AllSensorInfo(List<SensorInfo> SensorInfos) : IEventComponent;

}
