using PltWindTurbine.Database.TableDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.ResultRecordDB
{ 
    public record SerieBySensorTurbineError(Dictionary<(string, string), List<Value_Sensor_Turbine>> Result);
    public record PartialResultSensorTurbineError(string Key1, string Key2, List<Value_Sensor_Turbine>  Result);
}
