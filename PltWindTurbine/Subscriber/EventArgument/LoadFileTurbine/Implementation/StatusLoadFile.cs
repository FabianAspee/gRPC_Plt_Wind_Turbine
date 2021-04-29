using PltWindTurbine.Services.LoadFilesService;
using PltWindTurbine.Subscriber.EventArgument.LoadFileTurbine.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber.EventArgument.LoadFileTurbine.Implementation
{
    public record StatusLoadFile(StatusFile StatusFile, int Percent) : IStatusLoadFile;
    public record StatusFile(string NameFile, string Description, Status Status) : IStatusLoadFile;

}
