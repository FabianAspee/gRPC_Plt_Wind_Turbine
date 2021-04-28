using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.ChartComponent.ConfigGeneral
{
    public abstract class ConfigChart
    {
        public string Type { get; set; }
        public OptionChart Options { get; set; }
        public DataChart Data { get; set; } 
        public bool SteppedLine { get; set; }
        public abstract string GetNameSetup(); 
    }
}
