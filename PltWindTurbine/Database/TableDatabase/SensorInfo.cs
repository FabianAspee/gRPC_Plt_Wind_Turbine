using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.TableDatabase
{
    public class Sensor_Info
    {
        public int Id { get; set; }
        public string Sensor_Name { get; set; }
        public string Sensor_Data_Type { get; set; }
        public List<Value_Sensor_Turbine> Value_Sensor_Turbines { get; set; }
    }
}
