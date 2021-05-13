using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.TableDatabase
{
    public class Wind_Turbine_Info
    {
        [Key]
        public int Id { get; set; }
        public string Access_Point { get; set; }
        public string Wind_Power_Plant { get; set; }
        public string Folder_Name { get; set; }
        public string Tension_Line { get; set; }
        public string Turbine_Name { get; set; }
        [ForeignKey("Id_Turbine")]
        public ICollection<Value_Sensor_Turbine> Value_Sensor_Turbines { get; set; }
        [ForeignKey("Id_Turbine")]
        public ICollection<Value_Own_Serie_Turbine> Value_Own_Serie_Turbines { get; set; }
        [ForeignKey("Id_Turbine")]
        public ICollection<Value_Sensor_Error> Value_Sensor_Errors { get; set; }
        public override string ToString() => $"{Access_Point}{Wind_Power_Plant}{Folder_Name}{Tension_Line}{Turbine_Name}";
        public string ToSpecialString() => $"{Access_Point}#{Wind_Power_Plant}#{Folder_Name}#{Tension_Line}#{Turbine_Name}";
        public string ToStringAux() => $"{Tension_Line}{Turbine_Name}";
    }
}
