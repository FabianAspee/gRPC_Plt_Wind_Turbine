using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.ChartComponent
{
    public record InfoChartRecord(int IdTurbine, string NameTurbine, int IdSensor, string NameSensor, int Error, int Period); 
}
