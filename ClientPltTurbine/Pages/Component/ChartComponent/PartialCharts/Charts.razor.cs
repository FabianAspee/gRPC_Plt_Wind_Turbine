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
        private int idSensor;
        private int period;
        private int error;
        private int idChart;
        private bool recallChartInfo = false;
        private void ChangeInfoSensor(int idSensor) => this.idSensor = idSensor; 
        private void ChangeInfoError(int error) => this.error = error;  
        private void ChangeInfoPeriod(int period) => this.period = period;
        private void ChangeInfoChart(int idChart) => (this.idChart,recallChartInfo) = (idChart,true);
        private async void ChangeInfoTurbine(int idTurbine)
        {
            async Task InfoTurbine()
            {
                ErrorByTurbine.Clear();
                infoChart.Clear();
                this.idTurbine = idTurbine;
                var result = await ChartSingleton.CallErrorByTurbine(idTurbine);
                ErrorByTurbine.AddRange(result.Item2.Zip(Enumerable.Range(0, result.Item2.Count))
                    .Select(values => new ErrorTurbine(values.Second, values.First)).ToList());
                StateHasChanged();
            }
            await Call(InfoTurbine);
        }

    }
}
