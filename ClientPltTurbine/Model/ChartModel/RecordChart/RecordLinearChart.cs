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
    public record CustomInfoTurbineWarning(int Id, double? Value, DateTime Date) :CustomInfoTurbine(Id,Value,Date);
    public record CustomInfoTurbineWarningAndError(int Id, double? Value, DateTime Date, bool IsError) : CustomInfoTurbine(Id,Value,Date);
    public record RecordLinearChartBase(string NameTurbine, List<CustomInfoTurbine> CustomInfo);
    public record RecordLinearChartBaseWarningAndError(string NameTurbine, List<CustomInfoTurbineWarningAndError> CustomInfo);
    public record RecordLinearChartMaintenancePeriod(RecordLinearChartBaseWarningAndError RecordLinearChart, List<string> OriginalWarning) : RecordLinearChartBaseWarningAndError(RecordLinearChart);
    public record RecordLinearChart(RecordLinearChartBase RecordLinearChartBase, string NameSensor) : RecordLinearChartBase(RecordLinearChartBase); 
    public record RecordLinearChartWarning(RecordLinearChart RecordLinearChart, List<CustomInfoTurbineWarning> InfoTurbineWarnings, List<string> OriginalWarning);
}
