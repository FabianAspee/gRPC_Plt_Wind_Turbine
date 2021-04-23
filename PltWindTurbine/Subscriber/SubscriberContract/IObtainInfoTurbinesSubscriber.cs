using PltWindTurbine.Services.ObtainInfoTurbinesService;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber.SubscriberContract
{
    public interface IObtainInfoTurbinesSubscriber : ISubscriber
    { 
        public Task GetInfoTurbine(OnlySerieByPeriodAndCode info);
        public Task SerieByPeriodWithStandardDeviation(SeriePeriodByCodeWithStandarDeviation info);
    }
}
