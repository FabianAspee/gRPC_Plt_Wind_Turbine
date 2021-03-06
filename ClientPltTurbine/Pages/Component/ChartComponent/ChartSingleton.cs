using Blazored.Toast.Services;
using ClientPltTurbine.Controllers.ChartController;
using ClientPltTurbine.EventContainer;
using ClientPltTurbine.EventContainer.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using ChartJs.Blazor.LineChart;
using System.Linq;
using System.Threading.Tasks;
using ChartJs.Blazor.PieChart; 
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Util;
using ChartJs.Blazor.Common.Enums;
using PltTurbineShared;
using System.Threading.Tasks.Dataflow;

namespace ClientPltTurbine.Pages.Component.ChartComponent
{
    class ChartSingleton:EventHandlerSystem, IEventChart
    {
        public int NumTurbine;
        private BufferBlock<Sensor> Sensors;
        private BufferBlock<Turbine> Turbines;
        private BufferBlock<ResponseSerieByPeriod> InfoTurbineForChart;
        private BufferBlock<ResponseSerieByPeriodWithStandardDeviation> InfoTurbineForChartWithSTD;
        private TaskCompletionSource<bool> isCompleteS;
        private TaskCompletionSource<bool> isCompleteT;
        private TaskCompletionSource<bool> isCompleteStd;
        private TaskCompletionSource<bool> isCompleteChart;
        public IToastService Service;
        private readonly IChartController Controller = new ChartController();
        private readonly IEventContainer container = EventContainer.Implementation.EventContainer.Container; 
        public const int InitalCount = 7;
        public bool initSensor = true;
        public event EventHandler<IEventComponent> InfoChart; 
        public void RegisterEvent()
        {
            container.AddEvent(EventKey.GRAPH_KEY, InfoChart);
        }
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
            ResponseSerieByPeriodWithStandardDeviation status => Task.Run(() => Service.ShowError(status.StandardDeviation.ToString())),
            AllSensorInfo sensor => Task.Run(() => { 
                foreach (var mySensor in sensor.SensorInfos.Select(sensor => new Sensor(sensor.IdSensor, sensor.NameSensor))){
                    Sensors.SendAsync(mySensor);
                }
                isCompleteS.SetResult(true);
            }),
            AllTurbineInfo turbine => Task.Run(() =>
            { 
                foreach (var myTurbine in turbine.TurbineInfos.Select(turbine => new Turbine(turbine.IdTurbine, turbine.NameTurbine)))
                {
                    Turbines.SendAsync(myTurbine);
                }
                isCompleteT.SetResult(true);
                NumTurbine = turbine.TurbineInfos.Count;
            }),
            _ => Task.Run(() => Service.ShowError("ERROR"))
        };
        public async IAsyncEnumerable<Sensor> GetSensor()
        {
            await isCompleteS.Task;
            while(Sensors.Count>0)
            { 
                yield return Sensors.Receive();
            } 
        }
        public async IAsyncEnumerable<Turbine> GetTurbine()
        {
            await isCompleteT.Task;
            while (Turbines.Count > 0)
            {
                yield return Turbines.Receive();
            }
        }
        public async IAsyncEnumerable<ResponseSerieByPeriod> GetInfoChart()
        {
            while (!isCompleteChart.Task.IsCompleted)
            {  
                yield return await InfoTurbineForChart.ReceiveAsync();
            }
        }
        public async IAsyncEnumerable<ResponseSerieByPeriodWithStandardDeviation> GetInfoChartStd()
        {
            while (!isCompleteStd.Task.IsCompleted)
            {
                yield return await InfoTurbineForChartWithSTD.ReceiveAsync();
            }
        }
        public async Task GraphicInfoTurbine()
        {
            isCompleteChart = new();
            InfoTurbineForChart = new();
            await Controller.ChartAllTurbines().ConfigureAwait(false);
        }
        private void InitliazidedComponent()
        { 
            isCompleteS = new();
            isCompleteT = new();
            Sensors = new();
            Turbines = new();
        }
        public async Task CallTurbinesAndSensor()
        {
            InitliazidedComponent();
            await Controller.CallAllTurbinesAndSensors().ConfigureAwait(false);
        } 
    }
     public static class IListExtensions
    {
        // Basically a Polyfill since we now expose IList instead of List
        // which is better but IList doesn't have AddRange
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (list is List<T> asList)
            {
                asList.AddRange(items);
            }
            else
            {
                foreach (T item in items)
                {
                    list.Add(item);
                }
            }
        }
    }
}
