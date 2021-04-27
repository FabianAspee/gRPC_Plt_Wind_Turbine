using ChartJs.Blazor.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ClientPltTurbine.Pages.Component.ChartComponent.Charts;

namespace ClientPltTurbine.Pages.Component.ChartComponent.DesignChart
{
    public abstract class BaseChart
    {
        protected class Variant
        {
            public SteppedLine SteppedLine { get; set; }
            public string Title { get; set; }
            public System.Drawing.Color Color { get; set; }
        }
        protected readonly Func<string, Variant> _variants = title =>
             new()
             {
                 SteppedLine = SteppedLine.False,
                 Title = title,
                 Color = ChartColors.Red
             };
    }
}
