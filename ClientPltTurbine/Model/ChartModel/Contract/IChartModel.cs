using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model.ChartModel.Contract
{
    interface IChartModel
    {
        Task GetAllInfoTurbineForChart();
        Task GetAllNameTurbineAndSensor();
    }
}
