using PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Contract;
using PltWindTurbine.Protos.UtilProto;
using System.Collections.Generic;
using PltWindTurbine.Subscriber.EventArgument.UtilEventTurbine.Implementation;

namespace PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Implementation
{
    public record StatusEventInfoTurbine(StatusEvent Status) : ILoadInfoTurbine; 
    public record ValuesByTurbine(string NameTurbine, string Values, bool IsFinish) : ILoadInfoTurbine;
    public record ResponseSerieByPeriod(ValuesByTurbine ValuesByTurbine,string NameSensor) : ValuesByTurbine(ValuesByTurbine); 
    public record ResponseSerieOnlyWarning(string Warning, string OriginalWarning) :ILoadInfoTurbine;
    public record ResponseSerieByPeriodWithWarning(ResponseSerieByPeriod SerieByPeriod, ResponseSerieOnlyWarning OnlyWarning) : ResponseSerieOnlyWarning(OnlyWarning),ILoadInfoTurbine;
    public record ResponseSerieByPeriodWithStandardDeviation(ResponseSerieByPeriod InfoSerie, double StandardDeviation) : ILoadInfoTurbine;
    public record SensorInfo(int IdSensor, string NameSensor, bool IsOwnSensor) : ILoadInfoTurbine;
    public record AllSensorInfo(List<SensorInfo> SensorInfos ) : ILoadInfoTurbine;
    public record TurbineInfo(int IdTurbine, string NameTurbine) : ILoadInfoTurbine;
    public record AllTurbineInfo(List<TurbineInfo> TurbineInfos) : ILoadInfoTurbine;
    public record FinishMessage() : ILoadInfoTurbine;
    public record WarningAndErrorTurbine(ValuesByTurbine ValuesByTurbine, string OriginalWarning) : ValuesByTurbine(ValuesByTurbine);
}

 