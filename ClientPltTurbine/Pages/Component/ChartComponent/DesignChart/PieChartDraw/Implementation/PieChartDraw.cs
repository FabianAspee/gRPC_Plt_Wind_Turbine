using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.PieChartDraw.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.PieChartDraw.Implementation
{
    public class PieChartDraw: IPieChartDraw
    {
        private static readonly Lazy<PieChartDraw> instance = new(() => new PieChartDraw());
        private PieChartDraw() { }
        public static PieChartDraw Instance => instance.Value;
    }
}
