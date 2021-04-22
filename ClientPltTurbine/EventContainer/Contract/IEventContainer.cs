using System;
using System.Threading.Tasks;

namespace ClientPltTurbine.EventContainer.Contract
{
    public interface IEventContainer
    {
        public Task AddEvent(EventKey key, EventHandler<string> handler);
        public EventHandler<string> SelectEvent(EventKey key);
    }
}
