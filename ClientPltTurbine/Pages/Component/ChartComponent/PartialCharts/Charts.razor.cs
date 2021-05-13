using PltTurbineShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.ChartComponent
{
    public partial class Charts
    {
        private readonly List<Sensor> Sensors = new(); 
        private readonly List<Turbine> Turbines = new();
        private readonly List<ErrorTurbine> ErrorByTurbine = new();
        private readonly List<ChartInfo> ChartInfo = new(); 
        private int idTurbine;
        private (int idSensor,bool isOwn) infoSensor;
        private int period;
        private int error;
        private int idChart=-1;
        private bool recallChartInfo = false;
        private string text = string.Empty;
        private static int GetId(string id) => Convert.ToInt32(id);
        private void ChangeInfoSensor(string idSensor)
        { 
            var information = idSensor.Split(",");
            infoSensor = (GetId(information.First()), Convert.ToBoolean(information.Skip(1).First())); 
        }
        private void ChangeInfoError(string error) => this.error = GetId(error);
        private void ChangeInfoPeriod(int period) =>
            (this.period, text) = period is < 0 ? (period,string.Empty) : (this.period,"Only negative integer are allow");
        
        private void ChangeInfoChart(string idChart) => (this.idChart,recallChartInfo) = (GetId(idChart), true);
        private async void ChangeInfoTurbine(string idTurbine)
        {
            async Task InfoTurbine()
            {
                ErrorByTurbine.Clear();
                infoChart.Clear();
                this.idTurbine = GetId(idTurbine);
                var result = await ChartSingleton.CallErrorByTurbine(this.idTurbine);
                ErrorByTurbine.AddRange(result.Item2.Zip(Enumerable.Range(0, result.Item2.Count))
                    .Select(values => new ErrorTurbine(values.Second, values.First, values.Second.ToString())).ToList());
                StateHasChanged();
            }
            await Call(InfoTurbine);
        }
        public bool IsWarningChart => idChart is not 5 and not 6 and not 7 and not -1;
    }
}
