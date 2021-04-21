using ClientPltTurbine.EventContainer.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.EventContainer.Implementation
{
    public class EventContainer : IEventContainer
    {
        private static readonly Lazy<EventContainer> container = new(() => new());
        public static EventContainer Container => container.Value;
    }
}
