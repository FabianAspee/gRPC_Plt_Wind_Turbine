using ClientPltTurbine.EventContainer.Contract;
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
            container.SelectEvent<LoadF>(EventKey.LOAD_FILE_KEY).ReturnComponent().Invoke(this,msg);
        }
        public void SendEventFinishLoadFile(string msg)
        {
            container.SelectEvent(EventKey.LOAD_FILE_KEY).Invoke(this, msg);
        }
    }
}
