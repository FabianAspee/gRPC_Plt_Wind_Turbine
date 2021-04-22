using ClientPltTurbine.Pages.Component;
using System;
using System.Threading.Tasks;

namespace ClientPltTurbine.EventContainer.Contract
{
    public interface IEventContainer
    { 
        public Task AddEvent(EventKey key, EventHandler<IEventComponent> handler);
        public EventHandler<T> SelectEvent<T>(EventKey key);
    }
}
