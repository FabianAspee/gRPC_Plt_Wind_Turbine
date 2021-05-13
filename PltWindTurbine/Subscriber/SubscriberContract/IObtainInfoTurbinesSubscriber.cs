using PltWindTurbine.Database.ResultRecordDB;
using PltWindTurbine.Services.ObtainInfoTurbinesService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber.SubscriberContract
{
    public interface IObtainInfoTurbinesSubscriber : ISubscriber
    {
        Task GetInfoTurbine(OnlySerieByPeriodAndCode info);
        Task GetInfoTurbineWithWarning(OnlySerieByPeriodAndCodeWithWarning info);
        Task SerieByPeriodWithStandardDeviation(SeriePeriodByCodeWithStandarDeviation info);
        Task GetInforTurbineAndSensor();
        Task<List<string>> GetErrorByTurbine(int id);
        Task<List<(int,string)>> GetInfoChart();
        Task GetInfoTurbineOwnSerie(OnlySerieByOwnSeries info);
        Task GetInfoTurbineOwnSerieWithWarning(OnlySerieByOwnSeriesWithWarning info);
    }
}
