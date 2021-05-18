﻿using Grpc.Core;
using Microsoft.Extensions.Logging; 
using PltWindTurbine.Subscriber.EventArgument;
using PltWindTurbine.Subscriber.EventArgument.EventContainer;
using PltWindTurbine.Subscriber.EventArgument.EventContainer.Contract;
using PltWindTurbine.Subscriber.EventArgument.EventContainer.Implementation;
using PltWindTurbine.Subscriber.EventArgument.LoadFileTurbine.Contract;
using PltWindTurbine.Subscriber.EventArgument.MaintenanceTurbine.Contract;
using PltWindTurbine.Subscriber.EventArgument.MaintenanceTurbine.Implementation;
using PltWindTurbine.Subscriber.SubscriberContract;
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
        private event EventHandler<IBaseEvent> StatusMaintenance; 
        public MaintenanceService(ISubscriberFactory factoryMethod, ILogger<MaintenanceService> logger)
        {
            _logger = logger;
            _factoryMethod = factoryMethod;
        }
        private EventHandler<IBaseEvent> SelectEvent(EventKey key) => key switch
        {   EventKey.MAINTENANCE_KEY => StatusMaintenance,
            _ => throw new NotImplementedException()
        };
        private void RegisterEvent(EventKey key)
        {
            container.AddEvent(key, SelectEvent(key));
        }
        public override async Task SaveMaintenanceTurbines(IAsyncStreamReader<MaintenanceTurbinesRequest> request, IServerStreamWriter<MaintenanceTurbinesResponse> response, ServerCallContext context)
        {

            using var subscriberMaintenanceTurbine = _factoryMethod.GetMaintenanceSubscriber();
            StatusMaintenance += async (sender, args) =>
               await WriteStatusMaintenanceResponse(response, args as IMaintenanceTurbine);
            RegisterEvent(EventKey.MAINTENANCE_KEY);
            try
            {
                await HandleActionsMaintenanceTurbine(request, subscriberMaintenanceTurbine);
            }
            catch (Exception e)
            { 
                _logger.LogInformation(e.ToString());
            }
            _logger.LogInformation("Subscription finished.");
        }

        private async Task WriteStatusMaintenanceResponse(IServerStreamWriter<MaintenanceTurbinesResponse> stream, IMaintenanceTurbine maintenance)
        {
            try
            {
                var response = GetMaintenanceStatus(maintenance);
                await stream.WriteAsync(response);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to write message: {e.Message}");
            }
        }

        private static MaintenanceTurbinesResponse GetMaintenanceStatus(IMaintenanceTurbine maintenance) => maintenance switch
        {
            StatusEventLoadMaintenance info => new MaintenanceTurbinesResponse() {Name = info.Status.Name, Status = info.Status.Status,Description=info.Status.Description},
            _ => throw new NotImplementedException()
        }; 

        private async Task HandleActionsMaintenanceTurbine(IAsyncStreamReader<MaintenanceTurbinesRequest> request, IMaintenanceSubscriber subscriberMaintenanceTurbine)
        {
            await foreach (var action in request.ReadAllAsync())
            {
                switch (action.ActionCase)
                {
                    case MaintenanceTurbinesRequest.ActionOneofCase.None:
                        _logger.LogWarning("No Action specified.");
                        break;
                    case MaintenanceTurbinesRequest.ActionOneofCase.Msg1:
                        await subscriberMaintenanceTurbine.SaveMaintenanceTurbine(action.Msg1,action.IsFinish);
                        break; 
                    default:
                        _logger.LogWarning($"Unknown Action '{action.ActionCase}'.");
                        break;
                }
            }
        }
    }
}
