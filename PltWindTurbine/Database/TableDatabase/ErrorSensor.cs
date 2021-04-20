using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.TableDatabase
{
    public class Error_Sensor
    {
        public int Id { get; set; }
        public string Sensor_Name { get; set; }
        public string Data_Type_Turbine { get; set; }
        public List<Value_Sensor_Error> Value_Sensor_Errors { get; set; }
    }
}
