
using ClientPltTurbine.Model.ChartModel.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using Grpc.Core;
using PltWindTurbine.Services.ObtainInfoTurbinesService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model.ChartModel.Implementation
{
    public class ChartModel : BaseModel, IChartModel
    {
        private readonly ObtainInfoTurbines.ObtainInfoTurbinesClient _clientChart;
        private readonly AsyncDuplexStreamingCall<CodeAndPeriodRequest, CodeAndPeriodResponse> _duplexStreamObtainInfo;

        public ChartModel()
        {
            _clientChart = new ObtainInfoTurbines.ObtainInfoTurbinesClient(channel);
            _duplexStreamObtainInfo = _clientChart.InfoFailureTurbine(); 
            _ = HandleResponsesObtainInfoAsync(); 
        }
        public Task GetAllInfoTurbineForGraph()
        {
            var SerieByPeriod = new CodeAndPeriodRequest()
            {
                Msg1 = new OnlySerieByPeriodAndCode()
                {
                    Code = 180,
                    IdTurbine = 28,
                    Months = -3,
                    QtaGraph = 5,
                }
            };
            return _duplexStreamObtainInfo.RequestStream.WriteAsync(SerieByPeriod);
        }
        private void HandleFinalResponses(ResponseCodePeriod msg2)
        {
            switch (msg2.ActionCase)
            {
                case ResponseCodePeriod.ActionOneofCase.None:
                    SendEventErrorLoadInfoTurbine("No Action specified.");
                    break;
                case ResponseCodePeriod.ActionOneofCase.Msg:
                    SendEventLoadInfo(new ResponseSerieByPeriod(msg2.Msg.NameTurbine, msg2.Msg.IsFinish, Encoding.UTF8.GetString(msg2.Msg.Values.ToByteArray())));
                    break;
                case ResponseCodePeriod.ActionOneofCase.Msg2:
                    SendEventLoadInfoStandardDeviation(new ResponseSerieByPeriodWithStandardDeviation(new ResponseSerieByPeriod(msg2.Msg2.Msg1.NameTurbine, msg2.Msg2.Msg1.IsFinish, 
                        Encoding.UTF8.GetString(msg2.Msg2.Msg1.Values.ToByteArray())),msg2.Msg2.StandardDeviation));
                    break;
                default:
                    SendEventErrorLoadInfoTurbine($"Unknown Action '{msg2.ActionCase}'.");
                    break;
            }
        }
        private async Task HandleResponsesObtainInfoAsync()
        {
            await foreach (var update in _duplexStreamObtainInfo.ResponseStream.ReadAllAsync())
            {
                switch (update.ActionCase)
                {
                    case CodeAndPeriodResponse.ActionOneofCase.None:
                        SendEventErrorLoadInfoTurbine("No Action specified."); 
                        break;
                    case CodeAndPeriodResponse.ActionOneofCase.Msg1:
                        SendEventLoadInfoTurbine(update.Msg1.Status.ToString(), update.Msg1.Name);
                        break;
                    case CodeAndPeriodResponse.ActionOneofCase.Msg2:
                        HandleFinalResponses(update.Msg2);
                        break;
                    default:
                        SendEventErrorLoadInfoTurbine($"Unknown Action '{update.ActionCase}'."); 
                        break;
                }
               
            }
        }

        
    }
}
