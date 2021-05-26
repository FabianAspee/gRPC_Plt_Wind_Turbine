using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.ChartComponent
{
    public interface IBaseChart { }
    public record BaseInfoTurbine(int IdTurbine, string NameTurbine):IBaseChart;
    public record InfoChartRecord(BaseInfoTurbine BaseInfoTurbine, int IdSensor, string NameSensor, int Error, int Period, bool IsOwn = false): BaseInfoTurbine(BaseInfoTurbine);
    public record InfoChartRecordMaintenancePeriod(BaseInfoTurbine BaseInfoTurbine) : BaseInfoTurbine(BaseInfoTurbine);
}
