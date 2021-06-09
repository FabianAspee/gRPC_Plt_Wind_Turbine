using Blazored.Toast.Services;
using ClientPltTurbine.Controllers.ChartController;
using ClientPltTurbine.EventContainer;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ClientPltTurbine.Pages.Component.UtilComponent;
using PltTurbineShared;

namespace ClientPltTurbine.Pages.Component.ChartComponent
{
    class ChartSingleton: CommonMethod, IEventChart
    {
        private BufferBlock<IEventComponent> InfoTurbineForChart;
        private BufferBlock<ResponseSerieByPeriodWithStandardDeviation> InfoTurbineForChartWithSTD;
        private TaskCompletionSource<bool> isCompleteStd;
        private TaskCompletionSource<bool> isCompleteChart;
        public IToastService Service;
        private readonly IChartController Controller = new ChartController();
        public const int InitalCount = 7;
        public bool initSensor = true;
        public event EventHandler<IEventComponent> InfoChart; 
        public new void RegisterEvent()
        {
            container.AddEvent(EventKey.GRAPH_KEY, InfoChart);
            base.RegisterEvent();
        }
        public Task<List<(int,string)>> GetAllChart() => Controller.GetAllChart();
        public new Task CommonInfo(IEventComponent turbineAndSensor) => base.CommonInfo(turbineAndSensor);
        public Task WriteInfo(IEventComponent loadStatus) => loadStatus switch
        {
            LoadStatusChart { Msg: _, TypeMsg: 1 } status => Task.Run(() => Service.ShowInfo($"Turbine {status.NameTurbine} Status {status.Msg}")),
            ResponseSerieByPeriod status => Task.Run(() =>
            {
                InfoTurbineForChart.SendAsync(status);
                if (status.IsFinish)
                {
                    isCompleteChart.SetResult(true);
                }
                Service.ShowSuccess($"Load {status.Record.NameTurbine}");
            }),
            ResponseSerieByPeriodWarning status => Task.Run(() =>
            {
                InfoTurbineForChart.SendAsync(status);
                if (status.IsFinish)
                {
                    isCompleteChart.SetResult(true);
                }
                Service.ShowSuccess($"Load {status.Record.RecordLinearChart.NameTurbine}");
            }),
            ResponseSerieByMaintenancePeriod status => Task.Run(async () =>
            {
                await InfoTurbineForChart.SendAsync(status);
                if (status.IsFinish)
                {
                    isCompleteChart.SetResult(true);
                }
                Service.ShowSuccess($"Load {status.Record.RecordLinearChart.NameTurbine} {status.IsFinish} {InfoTurbineForChart.Count}");
            }),
            ResponseSerieByPeriodWithStandardDeviation status => Task.Run(() => Service.ShowError(status.StandardDeviation.ToString())),
            _ => Task.Run(() => Service.ShowError("ERROR"))
        };
        public new async IAsyncEnumerable<Sensor> GetSensor()
        {
            await foreach (var value in base.GetSensor())
                yield return value;
        }
        public new async IAsyncEnumerable<Turbine> GetTurbine()
        {
            await foreach (var value in base.GetTurbine())
                yield return value;
        }
        public async IAsyncEnumerable<IEventComponent> GetInfoChart()
        {
            while (!isCompleteChart.Task.IsCompleted || InfoTurbineForChart.Count!=0)
            {  
                yield return await InfoTurbineForChart.ReceiveAsync();
            }
        }
        public async IAsyncEnumerable<ResponseSerieByPeriodWithStandardDeviation> GetInfoChartStd()
        {
            while (!isCompleteStd.Task.IsCompleted || InfoTurbineForChartWithSTD.Count != 0)
            {
                yield return await InfoTurbineForChartWithSTD.ReceiveAsync();
            }
        }
        private Task CallTypeChart(IBaseChart info, int idChart) => (idChart,info) switch
        {
            (1 or 3, InfoChartRecord infoChart) => Controller.ChartAllTurbines(infoChart),
            (2 or 4 or 5 or 6 or 7, InfoChartRecord infoChart) => Controller.ChartAllTurbinesWarning(infoChart),
            (8,InfoChartRecordMaintenancePeriod infoChartMaintenance) => Controller.ChartTurbineByMaintenancePeriod(infoChartMaintenance),
            _ => throw new NotImplementedException()
        };
        public async Task ChartInfoTurbine(IBaseChart info, int type)
        {
            isCompleteChart = new();
            InfoTurbineForChart = new();
            await CallTypeChart(info, type).ConfigureAwait(false);
        } 

        public async Task CallTurbinesAndSensor()
        {
            InitliazidedComponent();
            await Controller.CallAllTurbinesAndSensors().ConfigureAwait(false);
        }
        
        public async Task<(int, List<string>)> CallErrorByTurbine(int idTurbine)=> await Controller.GetErrorByTurbine(idTurbine).ConfigureAwait(false);
        
    } 
      
}
