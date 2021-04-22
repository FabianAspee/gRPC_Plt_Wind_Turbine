using ClientPltTurbine.Pages.Component;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientPltTurbine.EventContainer.Contract
{
    public interface IEventContainer
    { 
        public void AddEvent(EventKey key, EventHandler<IEventComponent> handler);
        public Task<EventHandler<T>> SelectEvent<T>(EventKey key);
    }
}
