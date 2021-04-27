using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.ChartComponent.ConfigGeneral
{ 
    public interface IDataSetChart
    {
        public string BorderColor { get; init; }
        public string Label { get; init; }
        public string[] BackGroundColor { get; init; }
    }
    public record DataSetChart(string BorderColor, object[] Data, string Label, string[] BackGroundColor):IDataSetChart; 
}
