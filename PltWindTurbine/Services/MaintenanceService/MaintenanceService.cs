using Grpc.Core;
using Microsoft.Extensions.Logging;
using PltWindTurbine.Protos.MaintenanceProto;
using PltWindTurbine.Subscriber.EventArgument;
using PltWindTurbine.Subscriber.EventArgument.EventContainer;
using PltWindTurbine.Subscriber.EventArgument.EventContainer.Contract;
using PltWindTurbine.Subscriber.EventArgument.EventContainer.Implementation;
using PltWindTurbine.Subscriber.SubscriberFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Services.MaintenanceService
{
    public class MaintenanceService: Maintenances.MaintenancesBase 
    {
        private readonly ISubscriberFactory _factoryMethod;
        private readonly ILogger<MaintenanceService> _logger;
        private readonly IEventContainer container = EventContainer.Container;
        private event EventHandler<IBaseEvent> StatusLoad; 
        public MaintenanceService(ISubscriberFactory factoryMethod, ILogger<MaintenanceService> logger)
        {
            _logger = logger;
            _factoryMethod = factoryMethod;
        }
        private EventHandler<IBaseEvent> SelectEvent(EventKey key) => key switch
        { 
            _ => throw new NotImplementedException()
        };
        private void RegisterEvent(EventKey key)
        {
            container.AddEvent(key, SelectEvent(key));
        }
        public override async Task SaveMaintenanceTurbines(IAsyncStreamReader<MaintenanceTurbinesRequest> request, IServerStreamWriter<MaintenanceTurbinesResponse> response, ServerCallContext context)
        { 
             
            try
            { 
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                _logger.LogInformation(e.ToString());
            }
            _logger.LogInformation("Subscription finished.");
        }
    }
}
