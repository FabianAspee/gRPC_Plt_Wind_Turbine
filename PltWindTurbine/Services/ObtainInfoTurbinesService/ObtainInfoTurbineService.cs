using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using PltWindTurbine.Services.ObtainInfoTurbinesService;
using PltWindTurbine.Subscriber.EventArgument;
using PltWindTurbine.Subscriber.EventArgument.EventContainer;
using PltWindTurbine.Subscriber.EventArgument.EventContainer.Contract;
using PltWindTurbine.Subscriber.EventArgument.EventContainer.Implementation;
using PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Contract;
using PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Implementation;
using PltWindTurbine.Subscriber.SubscriberContract;
using PltWindTurbine.Subscriber.SubscriberFactory;

namespace PltWindTurbine.Services.ObtaininfoTurbinesService
{
    public class ObtainInfoTurbineService : ObtainInfoTurbines.ObtainInfoTurbinesBase
    {
        private readonly ISubscriberFactory _factoryMethod;
        private readonly ILogger<ObtainInfoTurbineService> _logger;
        private event EventHandler<IBaseEvent> Status;
        private readonly IEventContainer container = EventContainer.Container;
        public ObtainInfoTurbineService(ISubscriberFactory factoryMethod, ILogger<ObtainInfoTurbineService> logger)
        {
            _logger = logger;
            _factoryMethod = factoryMethod;
        }
        private void RegisterEvent()
        {
            container.AddEvent(EventKey.GRAPH_KEY, Status);
        }
        public override async Task InfoFailureTurbine(IAsyncStreamReader<CodeAndPeriodRequest> request, IServerStreamWriter<CodeAndPeriodResponse> response, ServerCallContext context)
        {

            using var obtainInfoTurbinesSubscriber = _factoryMethod.GetObtainInfoTurbinesSubscriber();
            Status += async (sender, args) =>
               await WriteStatusLoadInfoTurbine(response, args as ILoadInfoTurbine);
            RegisterEvent();
            try
            {
                await HandleActionsInfoFailureTurbine(request, obtainInfoTurbinesSubscriber);
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.ToString());
            }
            _logger.LogInformation("Subscription finished.");
        }

        private async Task HandleActionsInfoFailureTurbine(IAsyncStreamReader<CodeAndPeriodRequest> request, IObtainInfoTurbinesSubscriber obtainInfoTurbinesSubscriber)
        {
            await foreach (var action in request.ReadAllAsync())
            {
                switch (action.ActionCase)
                {
                    case CodeAndPeriodRequest.ActionOneofCase.None:
                        _logger.LogWarning("No Action specified.");
                        break;
                    case CodeAndPeriodRequest.ActionOneofCase.Msg1:
                        await obtainInfoTurbinesSubscriber.GetInfoTurbine(action.Msg1);
                        break;
                    case CodeAndPeriodRequest.ActionOneofCase.Msg2:
                        await obtainInfoTurbinesSubscriber.SerieByPeriodWithStandardDeviation(action.Msg2);
                        break; 
                    default:
                        _logger.LogWarning($"Unknown Action '{action.ActionCase}'.");
                        break;
                }
            }
        }

        private async Task WriteStatusLoadInfoTurbine(IServerStreamWriter<CodeAndPeriodResponse> stream, ILoadInfoTurbine infoTurbine)
        {
            try
            {
                var response = GetCodeAndPeriodResponse(infoTurbine);
                await stream.WriteAsync(response);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to write message: {e.Message}");
            }
        }
        private static CodeAndPeriodResponse GetCodeAndPeriodResponse(ILoadInfoTurbine infoTurbine) => infoTurbine switch
        {
            StatusEventInfoTurbine status => new CodeAndPeriodResponse() { Msg1 = new StatusLoadInfo() { Name = status.Name, Status = status.Status, Description = status.Description } },
            ResponseSerieByPeriod responseSerie => new CodeAndPeriodResponse() { Msg2 = new ResponseCodePeriod() { Msg = GetByPeriodAndCodeResponse(responseSerie) } },
            ResponseSerieByPeriodWithStandardDeviation responseSerieByPeriod => new CodeAndPeriodResponse()
            {
                Msg2 = new ResponseCodePeriod()
                {
                    Msg2 = new SeriePeriodByCodeWithStandarDeviationResponse() { Msg1 = GetByPeriodAndCodeResponse(responseSerieByPeriod.InfoSerie), StandardDeviation = responseSerieByPeriod.StandardDeviation }
                }
            },
            _ => throw new NotImplementedException()
        };
        private static OnlySerieByPeriodAndCodeResponse GetByPeriodAndCodeResponse(ResponseSerieByPeriod responseSerieBy) =>
            new()
            {
                NameTurbine = responseSerieBy.NameTurbine,
                Values = ByteString.CopyFrom(Encoding.UTF8.GetBytes(responseSerieBy.Values)),
                IsFinish = responseSerieBy.IsFinish
            };


    }
}
