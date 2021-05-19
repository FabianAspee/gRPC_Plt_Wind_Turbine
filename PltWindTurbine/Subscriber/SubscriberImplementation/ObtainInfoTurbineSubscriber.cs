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

        public Task GetInfoTurbine(OnlySerieByPeriodAndCode info) 
        {
            SendEventLoadInfo(info.NameTurbine, Status.InProgress, "Init process search series");
            return database.SelectSerieBySensorByTurbineByError(info); 
        }

        public Task GetInfoTurbineOwnSerie(OnlySerieByOwnSeries info) 
        {
            SendEventLoadInfo(info.Info.NameTurbine, Status.InProgress, "Init process search series");
            return database.SelectOwnSerieBySensorByTurbineByError(info.Info);
        } 
        
        public Task GetInfoTurbineOwnSerieWithWarning(OnlySerieByOwnSeriesWithWarning info)
        {
            SendEventLoadInfo(info.Info.NameTurbine, Status.InProgress, "Init process search series");
            return database.SelectOwnSerieBySensorByTurbineByErrorWithWarning(info.Info);
        }

        public Task GetInfoTurbineWithWarning(OnlySerieByPeriodAndCodeWithWarning info) 
        {
            SendEventLoadInfo(info.Info.NameTurbine, Status.InProgress, "Init process search series with warning");
            return database.SelectSerieBySensorByTurbineByErrorWithWarning(info.Info);
        } 

        public Task SerieByPeriodWithStandardDeviation(SeriePeriodByCodeWithStandarDeviation info)
        {
            throw new NotImplementedException();
        }
    }
}
