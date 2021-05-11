using System.Collections.Generic;

namespace ClientPltTurbine.Shared.ChartJsComponent.ConfigGeneral
{
    public record DataChart(List<string> Labels, DataSetChart[] Datasets); 
}
