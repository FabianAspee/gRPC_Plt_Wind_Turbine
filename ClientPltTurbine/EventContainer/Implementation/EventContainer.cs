using ClientPltTurbine.EventContainer.Contract;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.EventContainer.Implementation
{
    public class EventContainer : IEventContainer
    {
        private readonly Dictionary<string, EventHandler<string>> Events = new();
        private static readonly Lazy<EventContainer> container = new(() => new());
        public static EventContainer Container => container.Value;

        public Task AddEvent(EventKey key, EventHandler<string> handler) => Task.Run(() => Events.Add(key.ToString(), handler));
         
        public EventHandler<string> SelectEvent(EventKey key)
        {
            return Events[key.ToString()];
        }
    } 
}
