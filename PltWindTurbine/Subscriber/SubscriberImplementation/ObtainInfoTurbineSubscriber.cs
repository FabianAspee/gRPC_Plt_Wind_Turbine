using Newtonsoft.Json.Linq;
using PltWindTurbine.Database.DatabaseConnection;
using PltWindTurbine.Database.DatabaseContract;
using PltWindTurbine.Database.ResultRecordDB;
using PltWindTurbine.Database.Utils;
using PltWindTurbine.Services.ObtainInfoTurbinesService;
using PltWindTurbine.Subscriber.EventArgument.EventContainer;
using PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Contract;
using PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Implementation;
using PltWindTurbine.Subscriber.SubscriberContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber.SubscriberImplementation
{
    public class ObtainInfoTurbineSubscriber : AbstractSubscriber, IObtainInfoTurbinesSubscriber
    {
        private readonly IOperationTurbineDatabase database = RetreiveImplementationDatabase.Instance.ImplementationDatabase;
         
 
        public Task<List<string>> GetErrorByTurbine(int id)=> database.GetErrorByTurbine(id);

        public Task<List<(int, string)>> GetInfoChart() => Task.Run(() =>database.GetNameChart());

        public Task GetInforTurbineAndSensor()=> Task.Run(() =>
            {  
                database.SelectAllSensorAndTurbine();
            });
        

        public Task GetInfoTurbine(OnlySerieByPeriodAndCode info)=>Task.Run(() =>
           {
               SendEventLoadInfo(new StatusEventInfoTurbine(info.NameTurbine, Status.InProgress, "Init process search series")); 
                database.SelectSerieBySensorByTurbineByError(info); 
           });

        public Task GetInfoTurbineWithWarning(OnlySerieByPeriodAndCodeWithWarning info) => Task.Run(() =>
        {
            SendEventLoadInfo(new StatusEventInfoTurbine(info.Info.NameTurbine, Status.InProgress, "Init process search series with warning"));
            database.SelectSerieBySensorByTurbineByErrorWithWarning(info.Info);
        });

        public Task SerieByPeriodWithStandardDeviation(SeriePeriodByCodeWithStandarDeviation info)
        {
            throw new NotImplementedException();
        }
    }
}
