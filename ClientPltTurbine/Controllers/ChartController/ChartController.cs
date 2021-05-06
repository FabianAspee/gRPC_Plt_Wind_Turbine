using ClientPltTurbine.Model.ChartModel.Contract;
using ClientPltTurbine.Model.ChartModel.Implementation;
using ClientPltTurbine.Pages.Component.ChartComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Controllers.ChartController
{
    public class ChartController : BaseController, IChartController
    {
        private readonly IChartModel ChartModel = new ChartModel();
         
        public Task CallAllTurbinesAndSensors()=>ChartModel.GetAllNameTurbineAndSensor(); 

        public Task ChartAllTurbines(InfoChartRecord info) =>ChartModel.GetAllInfoTurbineForChart(info);

        public Task ChartAllTurbinesWarning(InfoChartRecord info) => ChartModel.GetAllInfoTurbineForChartWithWarning(info);

        public Task<List<(int, string)>> GetAllChart()=> ChartModel.GetAllChart();
         

        public Task<(int, List<string>)> GetErrorByTurbine(int idTurbine) => ChartModel.GetErroByTurbine(idTurbine);
    }
}
