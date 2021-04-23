using PltWindTurbine.Services.ObtainInfoTurbinesService;
using PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Contract;

namespace PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Implementation
{
    public record StatusEventInfoTurbine(string Name, Status Status, string Description) : ILoadInfoTurbine;
    public record ResponseSerieByPeriod(string NameTurbine,string Values, bool IsFinish) : ILoadInfoTurbine;
    public record ResponseSerieByPeriodWithStandardDeviation(ResponseSerieByPeriod InfoSerie, double StandardDeviation) : ILoadInfoTurbine;

}
