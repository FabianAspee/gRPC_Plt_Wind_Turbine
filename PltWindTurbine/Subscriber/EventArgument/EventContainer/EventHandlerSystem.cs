using PltWindTurbine.Services.ObtainInfoTurbinesService;
using PltWindTurbine.Subscriber.EventArgument.EventContainer.Contract;
using PltWindTurbine.Subscriber.EventArgument.LoadFileTurbine.Implementation;
using PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Contract;
using PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Implementation;
using System.Collections.Generic;
using loadFile = PltWindTurbine.Services.LoadFilesService;
namespace PltWindTurbine.Subscriber.EventArgument.EventContainer
{
    public abstract class EventHandlerSystem
    {

        private readonly IEventContainer container = Implementation.EventContainer.Container;
        public async void SendEventLoadFile(string name, string description, int percent)
        {
            
            await container.SelectEvent<IBaseEvent>(EventKey.LOAD_FILE_KEY).ContinueWith(evento=>evento.Result.Invoke(this, new StatusLoadFile(name, description,percent, loadFile.Status.InProgress)));
        }
        public async void SendEventFinishLoadFile(string name, string description, int percent=0)
        {
            await container.SelectEvent<IBaseEvent>(EventKey.LOAD_FILE_KEY).ContinueWith(evento => evento.Result.Invoke(this, new StatusLoadFile(name, description, percent, loadFile.Status.Success)));
        }
        public async void SendEventErrorLoadFile(string name, string description, int percent=0)
        {
            await container.SelectEvent<IBaseEvent>(EventKey.LOAD_FILE_KEY).ContinueWith(evento => evento.Result.Invoke(this, new StatusLoadFile(name, description, percent, loadFile.Status.Failed)));
        }

        public async void SendEventLoadInfoTurbine(ILoadInfoTurbine loadInfo)
        {
            await container.SelectEvent<IBaseEvent>(EventKey.INFO_TURBINE_SENSOR).ContinueWith(evento => evento.Result.Invoke(this, loadInfo));
        } 
        public async void SendEventFinishLoadInfoTurbine(string name, string description)
        {
            await container.SelectEvent<IBaseEvent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, new StatusEventInfoTurbine(name, Status.Success, description)));
        }
        public async void SendEventErrorLoadInfoTurbine(string name, string description)
        {
            await container.SelectEvent<IBaseEvent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, new StatusEventInfoTurbine(name, Status.Failed, description)));
        } 
        public async void SendEventLoadInfo(ILoadInfoTurbine serie)
        {
            await container.SelectEvent<IBaseEvent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, serie));
        }
        public async void SendEventLoadInfoStandardDeviation(string nameTurbine, string nameSensor, string values, bool isFinish, double standardDeviation)
        {
            await container.SelectEvent<IBaseEvent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, 
                new ResponseSerieByPeriodWithStandardDeviation(new ResponseSerieByPeriod(nameTurbine, nameSensor, values, isFinish), standardDeviation)));
        }
    }
}
