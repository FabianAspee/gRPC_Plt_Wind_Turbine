using PltWindTurbine.Database.TableDatabase;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.Utils.EqualityComparerElement
{
    public class EqualityComparerValueSensorError : IEqualityComparer<Value_Sensor_Error>
    {
        public bool Equals(Value_Sensor_Error x, Value_Sensor_Error y) => x.Id == y.Id;

        public int GetHashCode([DisallowNull] Value_Sensor_Error obj) =>base.GetHashCode(); 
    }
}
