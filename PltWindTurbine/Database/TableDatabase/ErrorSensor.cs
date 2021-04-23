using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.TableDatabase
{
    public class Error_Sensor
    {   
        [Key]
        public int Id { get; set; }
        public string Sensor_Name { get; set; }
        public string Data_Type_Turbine { get; set; }

        [ForeignKey("Id_Error_Sensor")]
        public ICollection<Error_Code> Error_Codes { get; set; }

        [ForeignKey("Id_Error_Sensor")]
        public ICollection<Value_Sensor_Error> Value_Sensor_Errors { get; set; }
    }
}
