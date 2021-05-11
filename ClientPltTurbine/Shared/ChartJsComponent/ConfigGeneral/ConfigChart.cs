using ClientPltTurbine.Shared.ChartJsComponent.ConfigGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.ChartJsComponent.ConfigGeneral
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
