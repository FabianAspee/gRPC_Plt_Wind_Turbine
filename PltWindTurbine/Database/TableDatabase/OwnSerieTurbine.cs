using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.TableDatabase
{
    public class Own_Serie_Turbine
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Is_Ok { get; set; }
        public string Sensor_Data_Type { get; set; }
        [ForeignKey("Id_Own_Serie")]
        public ICollection<Value_Own_Serie_Turbine> Value_Own_Serie_Turbines { get; set; }
      
    }
}
