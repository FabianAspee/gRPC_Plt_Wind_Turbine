﻿using ClientPltTurbine.Pages.Component.ChartComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model.ChartModel.Contract
{
    interface IChartModel
    {
        Task GetAllInfoTurbineForChart(InfoChartRecord info);
        Task GetAllNameTurbineAndSensor();
        Task<(int,List<string>)> GetErroByTurbine(int idTurbine);
        Task<List<(int, string)>> GetAllChart();
    }
}
