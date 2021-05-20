using System.Collections.Generic;

namespace PltWindTurbine.Database.ResultRecordDB
{
    public record SerieBySensorTurbineError(int Id, string Date, double? Value);
    public record SerieBySensorTurbineWarning(int Id, string Date, double? Value);
    public record AngleSerieTurbine(int IdSensor, int IdTurbine, string Date, double? Value);
    public record PeriodMaintenanceByTurbineWithError(int IdTurbine, string DateInitMaintenance, string DateFinishMaintenance, List<(string dateInit,string dateFinish, int errorCode)> InfoError);
}
