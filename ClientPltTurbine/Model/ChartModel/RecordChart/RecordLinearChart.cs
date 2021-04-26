using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model.ChartModel.RecordChart
{
    public record CustomInfoTurbine(int Id, double? Value, DateTime Date);
    public record RecordLinearChart(string NameTurbine,string NameSensor, List<CustomInfoTurbine> CustomInfo); 
 
}
