using ChartJs.Blazor.Common;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.ScatterChartDraw.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.ScatterChartDraw.Implementation
{
    public class ScatterChartDraw: IScatterChartDraw
    {
        private static readonly Lazy<ScatterChartDraw> instance = new(() => new ScatterChartDraw());
        private ScatterChartDraw() { }
        public static ScatterChartDraw Instance => instance.Value;

        public ConfigBase CreateScatterChart()
        {
            throw new NotImplementedException();
        }
    }
}
