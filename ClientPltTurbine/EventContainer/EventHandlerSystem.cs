using ClientPltTurbine.EventContainer.Contract;
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
        public void SendEventLoadFile(string msg)
        {
            container.SelectEvent<LoadStatusRecord>(EventKey.LOAD_FILE_KEY).Invoke(this,new LoadStatusRecord(1,msg));
        }
        public void SendEventFinishLoadFile(string msg)
        {
            container.SelectEvent<LoadStatusRecord>(EventKey.LOAD_FILE_KEY).Invoke(this, new LoadStatusRecord(1, msg));
        }
    }
}
