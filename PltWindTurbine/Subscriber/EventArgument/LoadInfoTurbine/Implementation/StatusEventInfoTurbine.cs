using PltWindTurbine.Database.ResultRecordDB;
using PltWindTurbine.Services.ObtainInfoTurbinesService;
using PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Contract;
using System.Collections.Generic;

namespace PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Implementation
{
    public record StatusEventInfoTurbine(string Name, Status Status, string Description) : ILoadInfoTurbine;
    public record ResponseSerieByPeriod(string NameTurbine,string NameSensor, string Values, bool IsFinish) : ILoadInfoTurbine;
    public record ResponseSerieByPeriodWithWarning(ResponseSerieByPeriod SerieByPeriod, string Warning):  ILoadInfoTurbine;
    public record ResponseSerieByPeriodWithStandardDeviation(ResponseSerieByPeriod InfoSerie, double StandardDeviation) : ILoadInfoTurbine;
    public record SensorInfo(int IdSensor, string NameSensor) : ILoadInfoTurbine;
    public record AllSensorInfo(List<SensorInfo> SensorInfos ) : ILoadInfoTurbine;
    public record TurbineInfo(int IdTurbine, string NameTurbine) : ILoadInfoTurbine;
    public record AllTurbineInfo(List<TurbineInfo> TurbineInfos) : ILoadInfoTurbine;
    public record FinishMessage() : ILoadInfoTurbine;
}
