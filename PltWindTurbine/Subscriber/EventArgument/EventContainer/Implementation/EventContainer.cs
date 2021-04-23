using PltWindTurbine.Subscriber.EventArgument.EventContainer.Contract;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber.EventArgument.EventContainer.Implementation
{
    public class EventContainer : IEventContainer
    {
        public ConcurrentDictionary<string, EventHandler<IBaseEvent>> Events= new();
        private static readonly Lazy<EventContainer> container = new(() => new());
        
        public static EventContainer Container => container.Value;

        public async void AddEvent(EventKey key, EventHandler<IBaseEvent> handler) => await Task.Run(() => Events.TryAdd(key.ToString(), handler)); 

        public async Task<EventHandler<T>> SelectEvent<T>(EventKey key)=> await Task.FromResult(Events[key.ToString()] as EventHandler<T>); 
        
    } 
}
