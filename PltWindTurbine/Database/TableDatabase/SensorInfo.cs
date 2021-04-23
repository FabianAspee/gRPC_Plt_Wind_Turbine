using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.TableDatabase
{
    public class Sensor_Info
    {
        [Key]
        public int Id { get; set; }
        public string Sensor_Name { get; set; }
        public string Sensor_Data_Type { get; set; }
        [ForeignKey("Id_Sensor")]
        public ICollection<Value_Sensor_Turbine> Value_Sensor_Turbines { get; set; }
    }
}
