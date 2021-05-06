using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.ChartComponent.ConfigGeneral
{ 
    public interface IDataSetChart
    {
        public object BorderColor { get; init; }
        public string Label { get; init; }
        public object BackgroundColor { get; init; }
    }
    public record DataSetChart(object[] Data, string Label, object BorderColor,bool Fill=false, object BackgroundColor = null) : IDataSetChart;
}
