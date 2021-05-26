using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model.ChartModel.RecordChart
{
    internal interface IParameterToChart { }
    internal record ParameterToChart(string NameTurbine, string NameSensor, bool IsFinish, ByteString Values) : IParameterToChart;
    internal record ParameterToChartWithWarning(ParameterToChart ParameterToChart, ByteString Warning, ByteString OriginalWarning) : IParameterToChart;
    internal record ParameterToChartMaintenancePeriod(string NameTurbine, ByteString Values, ByteString OriginalWarning, bool IsFinish) : IParameterToChart;
    public record CustomInfoTurbine(int Id, double? Value, DateTime Date);
    public record CustomInfoTurbineWarning(int Id, double? Value, DateTime Date);
    public record RecordLinearChartBase(string NameTurbine, List<CustomInfoTurbine> CustomInfo);
    public record RecordLinearChartMaintenancePeriod(RecordLinearChartBase RecordLinearChart, List<string> OriginalWarning) : RecordLinearChartBase(RecordLinearChart);
    public record RecordLinearChart(RecordLinearChartBase RecordLinearChartBase, string NameSensor) : RecordLinearChartBase(RecordLinearChartBase); 
    public record RecordLinearChartWarning(RecordLinearChart RecordLinearChart, List<CustomInfoTurbineWarning> InfoTurbineWarnings, List<string> OriginalWarning);
}
