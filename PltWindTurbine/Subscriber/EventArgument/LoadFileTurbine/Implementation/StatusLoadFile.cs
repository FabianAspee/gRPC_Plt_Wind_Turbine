using PltWindTurbine.Subscriber.EventArgument.LoadFileTurbine.Contract;
using PltWindTurbine.Protos.UtilProto;
using PltWindTurbine.Subscriber.EventArgument.UtilEventTurbine.Implementation;

namespace PltWindTurbine.Subscriber.EventArgument.LoadFileTurbine.Implementation
{
    public record StatusLoadFile(StatusFile StatusFile, int Percent) : IStatusLoadFile;
    public record StatusFile(StatusEvent Status) : IStatusLoadFile;

}
