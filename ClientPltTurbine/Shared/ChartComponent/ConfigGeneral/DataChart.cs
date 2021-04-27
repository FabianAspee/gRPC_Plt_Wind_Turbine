using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.ChartComponent.ConfigGeneral
{
    public record DataChart(List<string> Labels, DataSetChart[] Datasets); 
}
