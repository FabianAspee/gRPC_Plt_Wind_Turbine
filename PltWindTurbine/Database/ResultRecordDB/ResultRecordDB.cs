using PltWindTurbine.Database.TableDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.ResultRecordDB
{
    public record SerieBySensorTurbineError(int Id, string Date, double? Value);
    public record SerieBySensorTurbineWarning(int Id, string Date, double? Value);
}
