using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.TableDatabase
{
    public class Maintenance_Turbine
    {
        [Key]
        public int Id { get; set; }
        public int Id_Turbine { get; set; }
        public string Date { get; set; }
    }
} 