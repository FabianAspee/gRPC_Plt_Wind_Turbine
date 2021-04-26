using ClientPltTurbine.EventContainer.Contract;
using ClientPltTurbine.Pages.Component;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Pages.Component.LoadFileComponent.EventLoadFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.EventContainer
{
    public abstract class EventHandlerSystem
    {

        private readonly IEventContainer container = Implementation.EventContainer.Container;
        protected async void SendEventLoadFile(string msg)
        { 
            await container.SelectEvent<IEventComponent>(EventKey.LOAD_FILE_KEY).ContinueWith(evento=>evento.Result.Invoke(this,new LoadStatusRecord(1,msg)));
        }
        protected async void SendEventFinishLoadFile(string msg)
        {
            await container.SelectEvent<IEventComponent>(EventKey.LOAD_FILE_KEY).ContinueWith(evento => evento.Result.Invoke(this, new LoadStatusRecord(2, msg)));
        }
        protected async void SendEventErrorLoadFile(string msg)
        {
            await container.SelectEvent<IEventComponent>(EventKey.LOAD_FILE_KEY).ContinueWith(evento => evento.Result.Invoke(this, new LoadStatusRecord(3, msg)));
        }

        protected async void SendEventInfoTurbineAndSensor(IEventComponent component)
        {
            await container.SelectEvent<IEventComponent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, component));
        }
        protected async void SendEventLoadInfoTurbine(string msg,string nameTurbine)
        {
            await container.SelectEvent<IEventComponent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, new LoadStatusChart(1, msg, nameTurbine)));
        } 
        protected async void SendEventFinishLoadInfoTurbine(string msg)
        {
            await container.SelectEvent<IEventComponent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, new LoadStatusRecord(2, msg)));
        }
        protected async void SendEventErrorLoadInfoTurbine(string msg)
        {
            await container.SelectEvent<IEventComponent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, new LoadStatusRecord(3, msg)));
        }
        protected async void SendEventLoadInfo(ResponseSerieByPeriod info)
        {
            await container.SelectEvent<IEventComponent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, info));
        }
        protected async void SendEventLoadInfoStandardDeviation(ResponseSerieByPeriodWithStandardDeviation responseSerieByPeriod)
        {
            await container.SelectEvent<IEventComponent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, responseSerieByPeriod));
        }
    }
}
