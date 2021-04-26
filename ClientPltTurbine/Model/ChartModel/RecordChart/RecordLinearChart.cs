using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model.ChartModel.RecordChart
{
    internal interface IParameterToChart { }
    internal record ParameterToChart(string NameTurbine, string NameSensor, bool IsFinish, ByteString Values) : IParameterToChart;
    internal record ParameterToChartWithWarning(ParameterToChart ParameterToChart, ByteString Warning) : IParameterToChart;
    public record CustomInfoTurbine(int Id, double? Value, DateTime Date);
    public record CustomInfoTurbineWarning(int Id, double? Value, DateTime Date);
    public record RecordLinearChart(string NameTurbine, string NameSensor, List<CustomInfoTurbine> CustomInfo); 
    public record RecordLinearChartWarning(RecordLinearChart RecordLinearChart, List<CustomInfoTurbineWarning> InfoTurbineWarnings);
}
