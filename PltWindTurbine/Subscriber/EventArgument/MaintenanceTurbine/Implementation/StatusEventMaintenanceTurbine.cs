using PltWindTurbine.Subscriber.EventArgument.MaintenanceTurbine.Contract;
using PltWindTurbine.Subscriber.EventArgument.UtilEventTurbine.Implementation;

namespace PltWindTurbine.Subscriber.EventArgument.MaintenanceTurbine.Implementation
{

    public record StatusEventLoadMaintenance(StatusEvent Status) : IMaintenanceTurbine;
}
