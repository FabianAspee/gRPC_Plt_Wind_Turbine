using Newtonsoft.Json.Linq;
using PltWindTurbine.Database.DatabaseConnection;
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
        private readonly CommonImplementationDatabase database = RetreiveImplementationDatabase.Instance.ImplementationDatabase; 

        public void Dispose()
        {
            Console.WriteLine("Dispose");
        }

        public Task GetInfoTurbine(OnlySerieByPeriodAndCode info)
        {
           return Task.Run(() =>
           {
               SendEventLoadInfoTurbine(new StatusEventInfoTurbine("Lo ABRUZZO", Status.InProgress, "ES UNA TURBINA")); 
                database.SelectSerieBySensorByTurbineByError(info); 
           });
        }

        public Task SerieByPeriodWithStandardDeviation(SeriePeriodByCodeWithStandarDeviation info)
        {
            throw new NotImplementedException();
        }
    }
}
