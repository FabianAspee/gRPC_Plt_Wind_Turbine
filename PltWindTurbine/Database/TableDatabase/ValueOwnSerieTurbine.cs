using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.TableDatabase
{
    public class Value_Own_Serie_Turbine
    {
        public int Id { get; set; }
        public int Id_Turbine { get; set; }
        public int Id_Own_Serie { get; set; }
        public double? Value { get; set; }
        public string Date { get; set; } 
     
    }
}
