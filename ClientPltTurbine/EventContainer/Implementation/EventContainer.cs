using ClientPltTurbine.EventContainer.Contract;
using ClientPltTurbine.Pages.Component;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.EventContainer.Implementation
{
<<<<<<< HEAD
    public static class EventContainer
    {
        private static IEventContainer container= new EventContainerImp() ;
        public static IEventContainer Container { get=>container; }
        private class EventContainerImp : IEventContainer
        {
            private ConcurrentDictionary<string, EventHandler<IEventComponent>> Events = new();

            public async void AddEvent(EventKey key, EventHandler<IEventComponent> handler) => await Task.Run(() =>
            {
                if (Events.TryGetValue(key.ToString(), out _))
                {
                    Events[key.ToString()] = handler;
                }
                else
                {

                    Events.TryAdd(key.ToString(), handler);
                }
            });

            public async Task<EventHandler<T>> SelectEvent<T>(EventKey key) => await Task.FromResult(Events[key.ToString()] as EventHandler<T>);

        }
    } 
=======
    public class EventContainer : IEventContainer
    {
        public ConcurrentDictionary<string, EventHandler<IEventComponent>> Events = new();
        private static readonly Lazy<EventContainer> container = new(() => new());

        public static EventContainer Container => container.Value;

        public async void AddEvent(EventKey key, EventHandler<IEventComponent> handler) => await Task.Run(() =>
        {
            if (Events.TryGetValue(key.ToString(), out _))
            {
                Events[key.ToString()] = handler;
            }
            else
            {

                Events.TryAdd(key.ToString(), handler);
            }
        });

        public async Task<EventHandler<T>> SelectEvent<T>(EventKey key) => await Task.FromResult(Events[key.ToString()] as EventHandler<T>);

    }
>>>>>>> 372a3c51eb5de7635ea6908d75988352b9a01215
}
