using ClientPltTurbine.EventContainer.Contract;
using ClientPltTurbine.Pages.Component;
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
    }
}
