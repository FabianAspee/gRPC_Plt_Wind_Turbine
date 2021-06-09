using ClientPltTurbine.EventContainer;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Pages.Component.UtilComponent.EventCommonMethod;
using PltTurbineShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ClientPltTurbine.Pages.Component.UtilComponent
{
    public abstract class CommonMethod:EventHandlerSystem, IEventCommonMethod
    { 
        public int NumTurbine;
        protected TaskCompletionSource<bool> isCompleteS;
        protected TaskCompletionSource<bool> isCompleteT;
        protected BufferBlock<Sensor> Sensors;
        protected BufferBlock<Turbine> Turbines; 
        public event EventHandler<IEventComponent> CommonInfoEvent;
        public void RegisterEvent()
        { 
            container.AddEvent(EventKey.COMMON_KEY, CommonInfoEvent);
        }
        protected Task CommonInfo(IEventComponent eventComponent) => eventComponent switch
        { 
            AllSensorInfo sensor => AllSensorInfo(sensor),
            AllTurbineInfo turbine => AllTurbineInfo(turbine),
            _ => Task.Run(() => throw new NotImplementedException())
        };
        protected Task AllSensorInfo(AllSensorInfo sensor) => Task.Run(() => { 
            foreach (var mySensor in sensor.SensorInfos.Select(sensor => new Sensor(sensor.IdSensor, sensor.NameSensor, $"{sensor.IdSensor},{sensor.IsOwnSensor}", sensor.IsOwnSensor)))
            {
                Sensors.SendAsync(mySensor);
            }
            isCompleteS.SetResult(true);
        });
        protected Task AllTurbineInfo(AllTurbineInfo turbine) => Task.Run(() =>
        {
            foreach (var myTurbine in turbine.TurbineInfos.Select(turbine => new Turbine(turbine.IdTurbine, turbine.NameTurbine, turbine.IdTurbine.ToString())))
            {
                Turbines.SendAsync(myTurbine);
            }
            isCompleteT.SetResult(true);
            NumTurbine = turbine.TurbineInfos.Count;
        });
        protected async IAsyncEnumerable<Sensor> GetSensor()
        {
            await isCompleteS.Task;
            while (Sensors.Count > 0)
            {
                yield return Sensors.Receive();
            }
        }
        protected async IAsyncEnumerable<Turbine> GetTurbine()
        {
            await isCompleteT.Task;
            while (Turbines.Count > 0)
            {
                yield return Turbines.Receive();
            }
        }
        protected void InitliazidedComponent()
        {
            isCompleteS = new();
            isCompleteT = new();
            Sensors = new();
            Turbines = new();
        }
       
    }
}
