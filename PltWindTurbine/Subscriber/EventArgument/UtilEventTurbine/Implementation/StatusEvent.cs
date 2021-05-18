using PltWindTurbine.Protos.UtilProto;
using PltWindTurbine.Subscriber.EventArgument.UtilEventTurbine.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber.EventArgument.UtilEventTurbine.Implementation
{
    public record StatusEvent(string Name, Status Status, string Description) :IUtilEvent; 
}
