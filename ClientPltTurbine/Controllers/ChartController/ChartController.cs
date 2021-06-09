using ClientPltTurbine.Model.ChartModel.Contract;
using ClientPltTurbine.Model.ChartModel.Implementation;
using ClientPltTurbine.Pages.Component.ChartComponent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientPltTurbine.Controllers.ChartController
{
    public class ChartController : BaseController, IChartController
    {
        private readonly IChartModel ChartModel = new ChartModel(); 
         
        public Task CallAllTurbinesAndSensors()=>GetAllNameTurbineAndSensor(); 

        public Task ChartAllTurbines(InfoChartRecord info) =>
            info.IsOwn? ChartModel.GetAllInfoTurbineForChartOwnSeries(info) :ChartModel.GetAllInfoTurbineForChart(info);

        public Task ChartAllTurbinesWarning(InfoChartRecord info) => 
            info.IsOwn ? ChartModel.GetAllInfoTurbineForChartWithWarningOwnSeries(info) : ChartModel.GetAllInfoTurbineForChartWithWarning(info);

        public Task ChartTurbineByMaintenancePeriod(InfoChartRecordMaintenancePeriod infoChartMaintenance)=> ChartModel.GetMaintenancePeriodChart(infoChartMaintenance);

        public Task<List<(int, string)>> GetAllChart()=> ChartModel.GetAllChart(); 

        public Task<(int, List<string>)> GetErrorByTurbine(int idTurbine) => ChartModel.GetErroByTurbine(idTurbine);
    }
}
