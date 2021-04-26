using ClientPltTurbine.Pages.Component.ChartComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Controllers.ChartController
{
    interface IChartController
    {
        Task ChartAllTurbines(InfoChartRecord info);
        Task CallAllTurbinesAndSensors();
        Task<(int, List<string>)> GetErrorByTurbine(int idTurbine);
        Task<List<(int, string)>> GetAllChart();
    }
}
