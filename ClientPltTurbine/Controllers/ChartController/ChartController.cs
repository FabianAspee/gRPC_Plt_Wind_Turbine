using ClientPltTurbine.Model.ChartModel.Contract;
using ClientPltTurbine.Model.ChartModel.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Controllers.ChartController
{
    public class ChartController : BaseController, IChartController
    {
        private readonly IChartModel ChartModel = new ChartModel();
        public Task GraphAllTurbines()
        {
           return ChartModel.GetAllInfoTurbineForGraph();
        }
    }
}
