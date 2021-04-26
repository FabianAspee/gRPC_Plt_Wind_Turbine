
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
    public class ChartModel : BaseModel, IChartModel
    {
        private readonly ObtainInfoTurbines.ObtainInfoTurbinesClient _clientChart;
        private readonly AsyncDuplexStreamingCall<CodeAndPeriodRequest, CodeAndPeriodResponse> _duplexStreamObtainInfo;
        private AsyncServerStreamingCall<ResponseNameTurbineAndSensor> _asyncStreamGetInfoTurbineSensor;
        private readonly Dictionary<string, List<ByteString>> infoChartByTurbine = new();
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
                Msg1 = new OnlySerieByPeriodAndCode()
                {
                    Code = info.Error,
                    IdTurbine = info.IdTurbine,
                    NameTurbine = info.NameTurbine,
                    IdSensor = info.IdSensor,
                    NameSensor = info.NameSensor,
                    Months = info.Period,
                    QtaGraph = 5,
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
        private static byte[] ReturnByteFromContent(ByteString table) => table.ToByteArray();
        private void SaveInfoTurbineForChart(string nameTurbine,string nameSensor, bool isFinish, ByteString values)
        {
            if (!isFinish && !infoChartByTurbine.TryGetValue(nameTurbine, out _))
            {
                List<ByteString> value = new()
                {
                    values//refactoring
                };
                infoChartByTurbine[nameTurbine] = value;
                var result = Encoding.UTF8.GetString(ReturnByteFromContent(values));
                try
                {
                    var customInfos = JsonConvert.DeserializeObject<List<CustomInfoTurbine>>(result);
                    var info = new RecordLinearChart(nameTurbine, nameSensor, customInfos);
                    SendEventLoadInfo(new ResponseSerieByPeriod(isFinish, info));
                }
                catch (Exception e)
                { 
                    Console.WriteLine(e);
                }
            }
            else if (!isFinish && infoChartByTurbine.TryGetValue(nameTurbine, out List<ByteString> valueExisting))
            {
                valueExisting.Add(values);
                infoChartByTurbine[nameTurbine] = valueExisting; 
                var result = Encoding.UTF8.GetString(ReturnByteFromContent(values));
                try
                {
                    var customInfos = JsonConvert.DeserializeObject<List<CustomInfoTurbine>>(result);
                    var info = new RecordLinearChart(nameTurbine, nameSensor, customInfos);
                    SendEventLoadInfo(new ResponseSerieByPeriod(isFinish, info));
                }
                catch (Exception e)
                {

                    Console.WriteLine(e);
                }
            }
            else if(isFinish && !infoChartByTurbine.TryGetValue(nameTurbine, out _))
            { 
                var result = Encoding.UTF8.GetString(ReturnByteFromContent(values));
                try
                {
                    var customInfos = JsonConvert.DeserializeObject<List<CustomInfoTurbine>>(result);
                    var info = new RecordLinearChart(nameTurbine, nameSensor, customInfos);  
                    SendEventLoadInfo(new ResponseSerieByPeriod(isFinish, info));
                }
                catch(Exception e)
                {
                    
                    Console.WriteLine(e);
                }
                
            }
            else if (isFinish && infoChartByTurbine.TryGetValue(nameTurbine, out _))
            {
                var result = Encoding.UTF8.GetString(ReturnByteFromContent(values));
                try
                {
                    var customInfos = JsonConvert.DeserializeObject<List<CustomInfoTurbine>>(result);
                    var info = new RecordLinearChart(nameTurbine, nameSensor, customInfos);
                    SendEventLoadInfo(new ResponseSerieByPeriod(isFinish, info));
                }
                catch (Exception e)
                {

                    Console.WriteLine(e);
                }

            }
            else
            {
                 
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
                    SaveInfoTurbineForChart(msg2.Msg.NameTurbine,msg2.Msg.NameSensor, msg2.Msg.IsFinish, msg2.Msg.Values); 
                    break;
                case ResponseCodePeriod.ActionOneofCase.Msg2:
                    SendEventLoadInfoStandardDeviation(new ResponseSerieByPeriodWithStandardDeviation(new ResponseSerieByPeriod(msg2.Msg2.Msg1.IsFinish, 
                        null),msg2.Msg2.StandardDeviation));
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

       
    }
}
