
using ClientPltTurbine.Model.ChartModel.Contract;
using ClientPltTurbine.Model.ChartModel.RecordChart;
using ClientPltTurbine.Pages.Component.ChartComponent;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using Google.Protobuf;
using Grpc.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PltWindTurbine.Services.MaintenanceService;
using PltWindTurbine.Services.ObtainInfoTurbinesService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model.ChartModel.Implementation
{
    public class ChartModel : CommonMethodModel, IChartModel, IDisposable
    {
        private readonly ObtainInfoTurbines.ObtainInfoTurbinesClient _clientChart;
        private readonly Maintenances.MaintenancesClient _clientMaintenance;
        private readonly AsyncDuplexStreamingCall<CodeAndPeriodRequest, CodeAndPeriodResponse> _duplexStreamObtainInfo;
        private readonly AsyncDuplexStreamingCall<TurbineRequest, TurbineResponse> _duplexStreamObtainInfoMaintenancePeriod;
        private readonly Dictionary<string, List<IParameterToChart>> infoChartByTurbine = new();
        public ChartModel()
        {
            _clientChart = new ObtainInfoTurbines.ObtainInfoTurbinesClient(channel);
            _duplexStreamObtainInfo = _clientChart.InfoFailureTurbine();
            _ = HandleResponsesObtainInfoAsync();
            _clientMaintenance = new Maintenances.MaintenancesClient(channel);
            _duplexStreamObtainInfoMaintenancePeriod = _clientMaintenance.ObtainsAllWarningAndErrorInPeriodMaintenance();
            _ = HandleResponsesObtainInfoMaintenanceAsync();
        } 

        public Task GetAllInfoTurbineForChart(InfoChartRecord info)
        {
            var SerieByPeriod = new CodeAndPeriodRequest()
            {
                Msg1 = CreatePeriodAndCode(info)
            };
            return _duplexStreamObtainInfo.RequestStream.WriteAsync(SerieByPeriod);
        }

        public Task GetMaintenancePeriodChart(InfoChartRecordMaintenancePeriod infoChartMaintenance)
        {
            var TurbineRequest = new TurbineRequest()
            {
                IdTurbine = infoChartMaintenance.IdTurbine,
                NameTurbine = infoChartMaintenance.NameTurbine
            };
            return _duplexStreamObtainInfoMaintenancePeriod.RequestStream.WriteAsync(TurbineRequest);
        }

        public Task GetAllInfoTurbineForChartOwnSeries(InfoChartRecord info)
        {
            var SerieByPeriod = new CodeAndPeriodRequest()
            {
                Msg4 = new OnlySerieByOwnSeries()
                {
                    Info = CreatePeriodAndCode(info)
                }
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

        public Task GetAllInfoTurbineForChartWithWarningOwnSeries(InfoChartRecord info)
        {
            var SerieByPeriod = new CodeAndPeriodRequest()
            {
                Msg5 = new OnlySerieByOwnSeriesWithWarning()
                {
                    Info = CreatePeriodAndCode(info)
                }
            };
            return _duplexStreamObtainInfo.RequestStream.WriteAsync(SerieByPeriod);
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
            Days = info.Period,
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
                var info = new RecordLinearChart(new RecordLinearChartBase(parameterToChart.NameTurbine, customInfos), parameterToChart.NameSensor);
                SendEventLoadInfo(new ResponseSerieByPeriod(parameterToChart.IsFinish, info));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private void CreateLoadInfo(ParameterToChartWithWarning parameterToChart)
        {
            var result = EncodingByteToString(ReturnByteFromContent(parameterToChart.ParameterToChart.Values));
            var warning = EncodingByteToString(ReturnByteFromContent(parameterToChart.Warning));
            var originalWarning = EncodingByteToString(ReturnByteFromContent(parameterToChart.OriginalWarning));
            try
            {
                var customInfos = DeserializeObject<List<CustomInfoTurbine>>(result);
                var customInfosWarning = DeserializeObject<List<CustomInfoTurbineWarning>>(warning);
                var customInfosOriginalWarning = DeserializeObject<List<string>>(originalWarning);
                var info = new RecordLinearChart(new RecordLinearChartBase(parameterToChart.ParameterToChart.NameTurbine, customInfos), parameterToChart.ParameterToChart.NameSensor);
                var infoWarning = new RecordLinearChartWarning(info, customInfosWarning, customInfosOriginalWarning);
                SendEventLoadInfo(new ResponseSerieByPeriodWarning(parameterToChart.ParameterToChart.IsFinish, infoWarning));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void CreateLoadInfo(ParameterToChartMaintenancePeriod parameterToChart)
        {
            var result = EncodingByteToString(ReturnByteFromContent(parameterToChart.Values)); 
            var originalWarning = EncodingByteToString(ReturnByteFromContent(parameterToChart.OriginalWarning));
            try
            {
                var customInfos = DeserializeObject<List<CustomInfoTurbine>>(result); 
                var customInfosOriginalWarning = DeserializeObject<List<string>>(originalWarning);
                var info = new RecordLinearChartMaintenancePeriod(new RecordLinearChartBase(parameterToChart.NameTurbine,customInfos), customInfosOriginalWarning); 
                SendEventLoadInfo(new ResponseSerieByMaintenancePeriod(parameterToChart.IsFinish, info));
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

        private void SaveInfoTurbineForChart(ParameterToChartWithWarning parameterToChartWithWarning)
        {
            if (!infoChartByTurbine.TryGetValue(parameterToChartWithWarning.ParameterToChart.NameTurbine, out _))
            {
                List<IParameterToChart> value = new() { parameterToChartWithWarning };
                infoChartByTurbine[parameterToChartWithWarning.ParameterToChart.NameTurbine] = value;
                CreateLoadInfo(parameterToChartWithWarning);
            }
            else if (infoChartByTurbine.TryGetValue(parameterToChartWithWarning.ParameterToChart.NameTurbine, out List<IParameterToChart> valueExisting))
            {
                valueExisting.Add(parameterToChartWithWarning);
                infoChartByTurbine[parameterToChartWithWarning.ParameterToChart.NameTurbine] = valueExisting;
                CreateLoadInfo(parameterToChartWithWarning);
            }
        }

        private void SaveInfoTurbineForChart(ParameterToChartMaintenancePeriod parameterToChart)
        {
            if (!infoChartByTurbine.TryGetValue(parameterToChart.NameTurbine, out _))
            {
                List<IParameterToChart> value = new() { parameterToChart };
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
                    SaveInfoTurbineForChart(new ParameterToChartWithWarning(
                        new ParameterToChart(msg2.Msg3.Msg1.NameTurbine, msg2.Msg3.Msg1.NameSensor, msg2.Msg3.Msg1.IsFinish, msg2.Msg3.Msg1.Values),msg2.Msg3.Warning, msg2.Msg3.OriginalWarning));
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

        private async Task HandleResponsesObtainInfoMaintenanceAsync()
        {
            await foreach (var update in _duplexStreamObtainInfoMaintenancePeriod.ResponseStream.ReadAllAsync())
            { 
                SaveInfoTurbineForChart(new ParameterToChartMaintenancePeriod(update.Msg1.NameTurbine, update.Msg1.Values, update.OriginalWarning, update.Msg1.IsFinish));
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
                _duplexStreamObtainInfo.Dispose(); 
            }
        } 
        
    }
}
