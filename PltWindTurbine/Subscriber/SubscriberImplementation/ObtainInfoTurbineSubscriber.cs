using PltWindTurbine.Database.DatabaseConnection;
using PltWindTurbine.Database.DatabaseContract;
using PltWindTurbine.Protos.UtilProto;
using PltWindTurbine.Services.ObtainInfoTurbinesService;
using PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Implementation;
using PltWindTurbine.Subscriber.SubscriberContract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber.SubscriberImplementation
{
    public class ObtainInfoTurbineSubscriber : AbstractSubscriber, IObtainInfoTurbinesSubscriber
    {
        private readonly IOperationTurbineDatabase database = RetreiveImplementationDatabase.Instance.ImplementationDatabase; 
 
        public Task<List<string>> GetErrorByTurbine(int id)=> database.GetErrorByTurbine(id);

        public Task<List<(int, string)>> GetInfoChart() => database.GetNameChart(); 
       
        public Task GetInfoSensors() => database.SelectAllSensors();
        public Task GetInfoTurbines() => database.SelectAllTurbines();

        public async Task GetInfoTurbine(OnlySerieByPeriodAndCode info) 
        {
            await SendEventLoadInfo(info.NameTurbine, Status.InProgress, "Init process search series");
            await database.SelectSerieBySensorByTurbineByError(info); 
        }

        public async Task GetInfoTurbineOwnSerie(OnlySerieByOwnSeries info) 
        {
            await SendEventLoadInfo(info.Info.NameTurbine, Status.InProgress, "Init process search series");
            await database.SelectOwnSerieBySensorByTurbineByError(info.Info);
        } 
        
        public async Task GetInfoTurbineOwnSerieWithWarning(OnlySerieByOwnSeriesWithWarning info)
        {
            await SendEventLoadInfo(info.Info.NameTurbine, Status.InProgress, "Init process search series");
            await database.SelectOwnSerieBySensorByTurbineByErrorWithWarning(info.Info);
        }

        public async Task GetInfoTurbineWithWarning(OnlySerieByPeriodAndCodeWithWarning info) 
        {
            await SendEventLoadInfo(info.Info.NameTurbine, Status.InProgress, "Init process search series with warning");
            await database.SelectSerieBySensorByTurbineByErrorWithWarning(info.Info);
        } 

        public Task SerieByPeriodWithStandardDeviation(SeriePeriodByCodeWithStandarDeviation info)
        {
            throw new NotImplementedException();
        }
    }
}
