using Grpc.Core;
using Microsoft.Extensions.Logging;
using PltWindTurbine.Subscriber.SubscriberFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PltWindTurbine.Subscriber.SubscriberContract;
using PltWindTurbine.Subscriber.EventArgument;
using PltWindTurbine.Subscriber.EventArgument.LoadFileTurbine.Implementation;
using PltWindTurbine.Subscriber.EventArgument.EventContainer.Contract;
using PltWindTurbine.Subscriber.EventArgument.EventContainer.Implementation;
using PltWindTurbine.Subscriber.EventArgument.EventContainer;

namespace PltWindTurbine.Services.LoadFilesService
{
    public class LoadFileService : LoadFiles.LoadFilesBase
    {
        private readonly ISubscriberFactory _factoryMethod;
        private readonly ILogger<LoadFileService> _logger;
        private readonly IEventContainer container = EventContainer.Container;
        private event EventHandler<IBaseEvent> StatusLoad;
        private event EventHandler<IBaseEvent> StatusLoadReadSensor;
        public LoadFileService(ISubscriberFactory factoryMethod, ILogger<LoadFileService> logger)
        {
            _logger = logger;
            _factoryMethod = factoryMethod;
        }
        private EventHandler<IBaseEvent> SelectEvent(EventKey key) => key switch
        {
            EventKey.LOAD_FILE_KEY => StatusLoad,
            EventKey.LOAD_FILE_SENSOR_KEY => StatusLoadReadSensor,
            _ => throw new NotImplementedException()
        };
        private void RegisterEvent(EventKey key)
        { 
            container.AddEvent(key, SelectEvent(key));
        }
        public override async Task LoadFilesInfoTurbine(IAsyncStreamReader<FileUploadRequest> request, IServerStreamWriter<FileUploadResponse> response, ServerCallContext context)
        {

            using var subscriberLoadFilesInfoTurbine = _factoryMethod.GetLoadFileSubscriber();
            StatusLoad += async (sender, args) =>
               await WriteStatusLoadFileResponse(response, args as StatusLoadFile);
            RegisterEvent(EventKey.LOAD_FILE_KEY);
            try
            {
                await HandleActionsLoadFilesInfoTurbine(request, subscriberLoadFilesInfoTurbine);
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.ToString());
            }
            _logger.LogInformation("Subscription finished.");
        }
        private async Task WriteStatusLoadFileResponse(IServerStreamWriter<FileUploadResponse> stream, StatusLoadFile loadFileService)
        {
            try
            {
                var response = new FileUploadResponse
                {
                    Name = loadFileService.NameFile,
                    Status = loadFileService.Percent != 100 ? Status.InProgress : Status.Success,
                    Description = $"{loadFileService.Description} {loadFileService.Percent}%"
                };  
                await stream.WriteAsync(response);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to write message: {e.Message}");
            }
        }
        private async Task HandleActionsLoadFilesInfoTurbine(IAsyncStreamReader<FileUploadRequest> request, ILoadFileSubscriber subscriberLoadFilesInfoTurbine)
        {

            await foreach (var action in request.ReadAllAsync())
            {
                switch (action.ActionCase)
                {
                    case FileUploadRequest.ActionOneofCase.None:
                        _logger.LogWarning("No Action specified.");
                        break;
                    case FileUploadRequest.ActionOneofCase.Msg1:
                         await subscriberLoadFilesInfoTurbine.LoadFilesInfoTurbine(action); 
                        break;
                    case FileUploadRequest.ActionOneofCase.Msg2:
                        await subscriberLoadFilesInfoTurbine.LoadFilesNameSensor(action);
                        break;
                    case FileUploadRequest.ActionOneofCase.Msg3:
                        await subscriberLoadFilesInfoTurbine.LoadFilesNameErrorSensor(action);
                        break;
                    case FileUploadRequest.ActionOneofCase.Msg4:
                        await subscriberLoadFilesInfoTurbine.LoadFilesErrorCode(action);
                        break;
                    default:
                        _logger.LogWarning($"Unknown Action '{action.ActionCase}'.");
                        break;
                }
            }
        }

        public override async Task ReadSensor(IAsyncStreamReader<ReadInfoSensor> request, IServerStreamWriter<FileUploadResponse> response, ServerCallContext context)
        {
            using var subscriberReadSensor = _factoryMethod.GetLoadFileSubscriber();

            StatusLoadReadSensor += async (sender, args) =>
               await WriteStatusLoadFileResponse(response, args as StatusLoadFile);
            RegisterEvent(EventKey.LOAD_FILE_SENSOR_KEY);
            try
            {
                await HandleActionsReadSensor(request, subscriberReadSensor);
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.ToString());
            }
            _logger.LogInformation("Subscription finished.");
        }

        private async Task HandleActionsReadSensor(IAsyncStreamReader<ReadInfoSensor> request, ILoadFileSubscriber subscriberReadSensor)
        {
            await foreach (var action in request.ReadAllAsync())
            {
                switch (action.ActionCase)
                {
                    case ReadInfoSensor.ActionOneofCase.None:
                        _logger.LogWarning("No Action specified.");
                        break;
                    case ReadInfoSensor.ActionOneofCase.Msg1:
                        await subscriberReadSensor.LoadNormalSensor(action);
                        break;
                    case ReadInfoSensor.ActionOneofCase.Msg2:
                        await subscriberReadSensor.LoadEventSensor(action);
                        break;
                    default:
                        _logger.LogWarning($"Unknown Action '{action.ActionCase}'.");
                        break;
                }
            }
        }

    }
}
