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
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber.SubscriberImplementation
{
    public class ObtainInfoTurbineSubscriber :EventHandlerSystem, IObtainInfoTurbinesSubscriber
    {
        private readonly IOperationTurbineDatabase database = RetreiveImplementationDatabase.Instance.ImplementationDatabase; 

        public void Dispose()
        {
            Console.WriteLine("Dispose");
        }

        public Task<List<string>> GetErrorByTurbine(int id)=> database.GetErrorByTurbine(id);

        public Task<List<(int, string)>> GetInfoChart() => Task.Run(() =>database.GetNameChart());

        public Task GetInforTurbineAndSensor()=> Task.Run(() =>
            { 
                database.SelectAllSensorAndTurbine();
            });
        

        public Task GetInfoTurbine(OnlySerieByPeriodAndCode info)=>Task.Run(() =>
           {
               SendEventLoadInfoTurbine(new StatusEventInfoTurbine(info.NameTurbine, Status.InProgress, "Init process search series")); 
                database.SelectSerieBySensorByTurbineByError(info); 
           });

        public Task GetInfoTurbineWithWarning(OnlySerieByPeriodAndCodeWithWarning info) => Task.Run(() =>
        {
            SendEventLoadInfoTurbine(new StatusEventInfoTurbine(info.Info.NameTurbine, Status.InProgress, "Init process search series with warning"));
            database.SelectSerieBySensorByTurbineByErrorWithWarning(info.Info);
        });

        public Task SerieByPeriodWithStandardDeviation(SeriePeriodByCodeWithStandarDeviation info)
        {
            throw new NotImplementedException();
        }
    }
}
