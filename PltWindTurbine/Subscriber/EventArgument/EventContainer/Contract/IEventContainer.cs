using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber.EventArgument.EventContainer.Contract
{
    public interface IEventContainer
    { 
        public void AddEvent(EventKey key, EventHandler<IBaseEvent> handler);
        public Task<EventHandler<T>> SelectEvent<T>(EventKey key);
    }
}
