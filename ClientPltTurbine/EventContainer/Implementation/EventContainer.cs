using ClientPltTurbine.EventContainer.Contract;
using ClientPltTurbine.Pages.Component;
using ClientPltTurbine.Pages.Component.LoadFileComponent.EventLoadFile;
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
        private readonly Dictionary<string, IEventComponent> Events = new();
        private static readonly Lazy<EventContainer> container = new(() => new());
        public static EventContainer Container => container.Value;

        public Task AddEvent(EventKey key, IEventComponent handler) => Task.Run(() => Events.Add(key.ToString(), handler));

        public IEventComponent<T> SelectEvent<T>(EventKey key) => Events[key.ToString()] as IEventComponent<T>; 
    } 
}
