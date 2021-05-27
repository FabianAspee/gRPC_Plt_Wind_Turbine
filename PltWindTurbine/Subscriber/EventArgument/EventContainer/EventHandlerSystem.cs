using PltWindTurbine.Subscriber.EventArgument.EventContainer.Contract;
using PltWindTurbine.Subscriber.EventArgument.LoadFileTurbine.Implementation;
using PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Contract;
using PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Implementation;
using System.Threading.Tasks;
using PltWindTurbine.Protos.UtilProto;
using PltWindTurbine.Subscriber.EventArgument.MaintenanceTurbine.Implementation;
using PltWindTurbine.Subscriber.EventArgument.UtilEventTurbine.Implementation;
using PltWindTurbine.Subscriber.EventArgument.MaintenanceTurbine.Contract;

namespace PltWindTurbine.Subscriber.EventArgument.EventContainer
{
    public abstract class EventHandlerSystem
    { 

        private readonly IEventContainer container = Implementation.EventContainer.Container;
        public async Task SendEventLoadFile(string name, string description, int percent)=>await SendFinalEventFile(name, description, Status.InProgress, percent); 
       
        public async Task SendEventFinishLoadFile(string name, string description, int percent=0)=> await SendFinalEventFile(name, description, Status.Success, percent);
      
        public async Task SendEventErrorLoadFile(string name, string description, int percent=0) => await SendFinalEventFile(name, description, Status.Failed, percent);

        private async Task SendFinalEventFile(string name, string description, Status status, int percent)=>
            await container.SelectEvent<IBaseEvent>(EventKey.LOAD_FILE_KEY).ContinueWith(evento => evento.Result.Invoke(this, new StatusLoadFile(new StatusFile(new StatusEvent(name, status, description)), percent)));


        public async Task SendEventFile(string name, string description) => await SendEventFile(name, description, Status.InProgress);

        public async Task SendEventFinishFile(string name, string description) => await SendEventFile(name, description, Status.Success);

        public async Task SendEventErrorFile(string name, string description) => await SendEventFile(name, description, Status.Failed);

        public async Task SendEventFile(string name, string description, Status status)=>
            await container.SelectEvent<IBaseEvent>(EventKey.LOAD_FILE_KEY).ContinueWith(evento => evento.Result.Invoke(this, new StatusFile(new StatusEvent(name, status, description))));
        

        public Task SendEventLoadInfoTurbine(ILoadInfoTurbine loadInfo)
        {
            return Task.Run(async()=>await container.SelectEvent<IBaseEvent>(EventKey.INFO_TURBINE_SENSOR).ContinueWith(evento => evento.Result.Invoke(this, loadInfo)));
        } 
        public async Task SendEventFinishLoadInfoTurbine(string name, string description)
        {
            await container.SelectEvent<IBaseEvent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, new StatusEventInfoTurbine(new StatusEvent(name, Status.Success, description))));
        }
        public async Task SendEventErrorLoadInfoTurbine(string name, string description)
        {
            await container.SelectEvent<IBaseEvent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, new StatusEventInfoTurbine(new StatusEvent(name, Status.Failed, description))));
        } 
        public async Task SendEventLoadInfo(string name, Status status, string description)
        {
            await container.SelectEvent<IBaseEvent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, new StatusEventInfoTurbine(new StatusEvent(name,status,description))));
        }
        public async Task SendEventLoadInfo(ILoadInfoTurbine infoTurbine)
        {
            await container.SelectEvent<IBaseEvent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, infoTurbine));
        }
        public async Task SendEventLoadInfoMaintenance(ILoadInfoTurbine infoTurbine)
        {
            await container.SelectEvent<IBaseEvent>(EventKey.MAINTENANCE_KEY).ContinueWith(evento => evento.Result.Invoke(this, infoTurbine));
        }
        public async Task SendEventLoadMaintenanceInfo(string name, string description) => await SendEventLoadMaintenanceInfo(name, description, Status.InProgress);

        public async Task SendEventFinishLoadMaintenanceInfo(string name, string description) => await SendEventLoadMaintenanceInfo(name, description, Status.Success);

        public async Task SendEventErrorLoadMaintenanceInfo(string name, string description) => await SendEventLoadMaintenanceInfo(name, description, Status.Failed);

        private async Task SendEventLoadMaintenanceInfo(string name, string description, Status status)
        {
            await container.SelectEvent<IBaseEvent>(EventKey.MAINTENANCE_KEY).ContinueWith(evento => evento.Result.Invoke(this, new StatusEventLoadMaintenance(new StatusEvent(name,status,description))));
        }
        public async Task SendEventLoadInfoStandardDeviation(string nameTurbine, string nameSensor, string values, bool isFinish, double standardDeviation)
        {
            await container.SelectEvent<IBaseEvent>(EventKey.GRAPH_KEY).ContinueWith(evento => evento.Result.Invoke(this, 
                new ResponseSerieByPeriodWithStandardDeviation(new ResponseSerieByPeriod(new ValuesByTurbine(nameTurbine, values, isFinish), nameSensor), standardDeviation)));
        }
    }
}
