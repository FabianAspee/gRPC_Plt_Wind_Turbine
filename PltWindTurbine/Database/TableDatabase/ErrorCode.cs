using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.TableDatabase
{
    public class Error_Code
    {
        [Key]
        public int Id { get; set; } 
        public int Id_Error_Sensor { get; set; }
        public string Ec_Name { get; set; }
        public int Wtg_Event { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public bool Is_Downtime { get; set; }
        public bool Is_Fault { get; set; }
        public string Cause { get; set; }
        public string Nature { get; set; }
        public string Reset_Type { get; set; }
        public int Alarm_Level { get; set; }
        public string Notes { get; set; }
        public string Changed { get; set; }
    }
}
