
using ClientPltTurbine.Model.ChartModel.Contract;
using ClientPltTurbine.Model.ChartModel.RecordChart;
using ClientPltTurbine.Pages.Component.ChartComponent;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using Google.Protobuf;
using Grpc.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PltWindTurbine.Services.ObtainInfoTurbinesService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model.ChartModel.Implementation
{
    public class ChartModel : BaseModel, IChartModel, IDisposable
    {
        private readonly ObtainInfoTurbines.ObtainInfoTurbinesClient _clientChart;
        private readonly AsyncDuplexStreamingCall<CodeAndPeriodRequest, CodeAndPeriodResponse> _duplexStreamObtainInfo;
        private AsyncServerStreamingCall<ResponseNameTurbineAndSensor> _asyncStreamGetInfoTurbineSensor;
        private readonly Dictionary<string, List<IParameterToChart>> infoChartByTurbine = new();
        public ChartModel()
        {
            _clientChart = new ObtainInfoTurbines.ObtainInfoTurbinesClient(channel);
            _duplexStreamObtainInfo = _clientChart.InfoFailureTurbine();
            _ = HandleResponsesObtainInfoAsync();
        }

        public Task GetAllInfoTurbineForChart(InfoChartRecord info)
        {
            var SerieByPeriod = new CodeAndPeriodRequest()
            {
                Msg1 = CreatePeriodAndCode(info)
            };
            return _duplexStreamObtainInfo.RequestStream.WriteAsync(SerieByPeriod);
        }
        
        public Task GetAllInfoTurbineForChartWithWarning(InfoChartRecord info)
        {
            var SerieByPeriod = new CodeAndPeriodRequest()
            {
                Msg3 = new OnlySerieByPeriodAndCodeWithWarning()
                { 
                    Info = CreatePeriodAndCode(info)
                }
            };
            return _duplexStreamObtainInfo.RequestStream.WriteAsync(SerieByPeriod);
        }

        public Task GetAllNameTurbineAndSensor()
        {
            return Task.Run(() =>
            { 
                _asyncStreamGetInfoTurbineSensor = _clientChart.GetNameTurbineAndSensor(new WithoutMessage());
                _ = HandleResponsesInfoTurbineSensorAsync();
            });
        }

        public Task<(int,List<string>)> GetErroByTurbine(int idTurbine)=>_clientChart.GetErrorByTurbineAsync(new ErrorByTurbineRequest { IdTurbine = idTurbine })
            .ResponseAsync.ContinueWith(response=>(response.Result.IdTurbine, response.Result.Errors.ToList()),TaskContinuationOptions.OnlyOnRanToCompletion);

        public Task<List<(int, string)>> GetAllChart() => _clientChart.GetChartSystemAsync(new WithoutMessage{})
            .ResponseAsync.ContinueWith(response =>response.Result.Info.Select(info=>(info.IdChart,info.NameChart)).ToList(), TaskContinuationOptions.OnlyOnRanToCompletion);

        private static OnlySerieByPeriodAndCode CreatePeriodAndCode(InfoChartRecord info) => new()
        {
            Code = info.Error,
            IdTurbine = info.IdTurbine,
            NameTurbine = info.NameTurbine,
            IdSensor = info.IdSensor,
            NameSensor = info.NameSensor,
            Months = info.Period,
            QtaGraph = 5,
        };

        private static byte[] ReturnByteFromContent(ByteString values) => values.ToByteArray();
        private static string EncodingByteToString(byte[] values) => Encoding.UTF8.GetString(values);
        private static T DeserializeObject<T>(string values) => JsonConvert.DeserializeObject<T>(values);

        private void CreateLoadInfo(ParameterToChart parameterToChart)
        {
            var result = EncodingByteToString(ReturnByteFromContent(parameterToChart.Values));
            try
            {
                var customInfos = DeserializeObject<List<CustomInfoTurbine>>(result);
                var info = new RecordLinearChart(parameterToChart.NameTurbine, parameterToChart.NameSensor, customInfos);
                SendEventLoadInfo(new ResponseSerieByPeriod(parameterToChart.IsFinish, info));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private void CreateLoadInfoWarning(ParameterToChartWithWarning parameterToChart)
        {
            var result = EncodingByteToString(ReturnByteFromContent(parameterToChart.ParameterToChart.Values));
            var warning = EncodingByteToString(ReturnByteFromContent(parameterToChart.Warning));
            try
            {
                var customInfos = DeserializeObject<List<CustomInfoTurbine>>(result);
                var customInfosWarning = DeserializeObject<List<CustomInfoTurbineWarning>>(warning);
                var info = new RecordLinearChart(parameterToChart.ParameterToChart.NameTurbine, parameterToChart.ParameterToChart.NameSensor, customInfos);
                var infoWarning = new RecordLinearChartWarning(info, customInfosWarning);
                SendEventLoadInfo(new ResponseSerieByPeriodWarning(parameterToChart.ParameterToChart.IsFinish, infoWarning));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private void SaveInfoTurbineForChart(ParameterToChart parameterToChart)
        {
            if (!infoChartByTurbine.TryGetValue(parameterToChart.NameTurbine, out _))
            {
                List<IParameterToChart> value = new(){ parameterToChart };
                infoChartByTurbine[parameterToChart.NameTurbine] = value;
                CreateLoadInfo(parameterToChart);
            }
            else if (infoChartByTurbine.TryGetValue(parameterToChart.NameTurbine, out List<IParameterToChart> valueExisting))
            {
                valueExisting.Add(parameterToChart);
                infoChartByTurbine[parameterToChart.NameTurbine] = valueExisting; 
                CreateLoadInfo(parameterToChart); 
            }   
        }
        private void SaveInfoTurbineForChartWarning(ParameterToChartWithWarning parameterToChartWithWarning)
        {
            if (!infoChartByTurbine.TryGetValue(parameterToChartWithWarning.ParameterToChart.NameTurbine, out _))
            {
                List<IParameterToChart> value = new() { parameterToChartWithWarning };
                infoChartByTurbine[parameterToChartWithWarning.ParameterToChart.NameTurbine] = value;
                CreateLoadInfoWarning(parameterToChartWithWarning);
            }
            else if (infoChartByTurbine.TryGetValue(parameterToChartWithWarning.ParameterToChart.NameTurbine, out List<IParameterToChart> valueExisting))
            {
                valueExisting.Add(parameterToChartWithWarning);
                infoChartByTurbine[parameterToChartWithWarning.ParameterToChart.NameTurbine] = valueExisting;
                CreateLoadInfoWarning(parameterToChartWithWarning);
            }
        }
        private void HandleFinalResponses(ResponseCodePeriod msg2)
        {
            switch (msg2.ActionCase)
            {
                case ResponseCodePeriod.ActionOneofCase.None:
                    SendEventErrorLoadInfoTurbine("No Action specified.");
                    break;
                case ResponseCodePeriod.ActionOneofCase.Msg:
                    SaveInfoTurbineForChart(new ParameterToChart(msg2.Msg.NameTurbine,msg2.Msg.NameSensor, msg2.Msg.IsFinish, msg2.Msg.Values)); 
                    break;
                case ResponseCodePeriod.ActionOneofCase.Msg2:
                    SendEventLoadInfoStandardDeviation(new ResponseSerieByPeriodWithStandardDeviation(new ResponseSerieByPeriod(msg2.Msg2.Msg1.IsFinish, 
                        null),msg2.Msg2.StandardDeviation));
                    break;
                case ResponseCodePeriod.ActionOneofCase.Msg3:
                    SaveInfoTurbineForChartWarning(new ParameterToChartWithWarning(
                        new ParameterToChart(msg2.Msg3.Msg1.NameTurbine, msg2.Msg3.Msg1.NameSensor, msg2.Msg3.Msg1.IsFinish, msg2.Msg3.Msg1.Values),msg2.Msg3.Warning));
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
        private async Task HandleResponsesInfoTurbineSensorAsync()
        {
            await foreach (var turbineSensor in _asyncStreamGetInfoTurbineSensor.ResponseStream.ReadAllAsync())
            {
                switch (turbineSensor.ActionCase)
                {
                    case ResponseNameTurbineAndSensor.ActionOneofCase.None:
                        SendEventErrorLoadInfoTurbine("No Action specified.");
                        break;
                    case ResponseNameTurbineAndSensor.ActionOneofCase.Msg4:
                        SendEventInfoTurbineAndSensor(new AllTurbineInfo(turbineSensor.Msg4.Msg.Select(turbine => new TurbineInfo(turbine.IdTurbine,turbine.NameTurbine)).ToList()));
                        break;
                    case ResponseNameTurbineAndSensor.ActionOneofCase.Msg3:
                        SendEventInfoTurbineAndSensor(new AllSensorInfo(turbineSensor.Msg3.Msg.Select(sensor=>new SensorInfo(sensor.IdSensor, sensor.NameSensor)).ToList()));
                        break;
                    default:
                        SendEventErrorLoadInfoTurbine($"Unknown Action '{turbineSensor.ActionCase}'.");
                        break;
                }

            }
        }
        async void IDisposable.Dispose()
        {
            try
            {
                GC.SuppressFinalize(this);
                await _duplexStreamObtainInfo.RequestStream.CompleteAsync(); 
            }
            finally
            {
                _asyncStreamGetInfoTurbineSensor.Dispose();
                _duplexStreamObtainInfo.Dispose(); 
            }
        }

    }
}
