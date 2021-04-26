using PltWindTurbine.Database.ResultRecordDB;
using PltWindTurbine.Services.ObtainInfoTurbinesService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber.SubscriberContract
{
    public interface IObtainInfoTurbinesSubscriber : ISubscriber
    {
        public Task GetInfoTurbine(OnlySerieByPeriodAndCode info);
        public Task GetInfoTurbineWithWarning(OnlySerieByPeriodAndCodeWithWarning info);
        public Task SerieByPeriodWithStandardDeviation(SeriePeriodByCodeWithStandarDeviation info);
        public Task GetInforTurbineAndSensor();
        public Task<List<string>> GetErrorByTurbine(int id);
        public Task<List<(int,string)>> GetInfoChart();
    }
}
