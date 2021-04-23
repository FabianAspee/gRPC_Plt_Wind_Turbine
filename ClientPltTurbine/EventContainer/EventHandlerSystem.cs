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
        public async void SendEventLoadFile(string msg)
        { 
            await container.SelectEvent<IEventComponent>(EventKey.LOAD_FILE_KEY).ContinueWith(evento=>evento.Result.Invoke(this,new LoadStatusRecord(1,msg)));
        }
        public async void SendEventFinishLoadFile(string msg)
        {
            await container.SelectEvent<IEventComponent>(EventKey.LOAD_FILE_KEY).ContinueWith(evento => evento.Result.Invoke(this, new LoadStatusRecord(2, msg)));
        }
        public async void SendEventErrorLoadFile(string msg)
        {
            await container.SelectEvent<IEventComponent>(EventKey.LOAD_FILE_KEY).ContinueWith(evento => evento.Result.Invoke(this, new LoadStatusRecord(3, msg)));
        }


        public async void SendEventLoadInfoTurbine(string msg,string nameTurbine)
        {
            await container.SelectEvent<IEventComponent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, new LoadStatusChart(1, msg, nameTurbine)));
        } 
        public async void SendEventFinishLoadInfoTurbine(string msg)
        {
            await container.SelectEvent<IEventComponent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, new LoadStatusRecord(2, msg)));
        }
        public async void SendEventErrorLoadInfoTurbine(string msg)
        {
            await container.SelectEvent<IEventComponent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, new LoadStatusRecord(3, msg)));
        }
        public async void SendEventLoadInfo(ResponseSerieByPeriod info)
        {
            await container.SelectEvent<IEventComponent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, info));
        }
        public async void SendEventLoadInfoStandardDeviation(ResponseSerieByPeriodWithStandardDeviation responseSerieByPeriod)
        {
            await container.SelectEvent<IEventComponent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, responseSerieByPeriod));
        }
    }
}
