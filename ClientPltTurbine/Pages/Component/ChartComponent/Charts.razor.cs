using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.LineChartDraw.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.LineChartDraw.Implementation;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.RadarChartDraw.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.RadarChartDraw.Implementation;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.ScatterChartDraw.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.ScatterChartDraw.Implementation;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using PltTurbineShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.ChartComponent
{
    public partial class Charts
    { 
        private ChartSingleton ChartSingleton = new(); 
        private readonly List<IEventComponent> infoChart = new(); 
        private readonly ILineChartDraw lineChartDraw = LineChartDraw.Instance;
        private readonly IScatterChartDraw scatterChartDraw = ScatterChartDraw.Instance;
        private readonly IRadarChartDraw radarCharttDraw = RadarChartDraw.Instance;
        private bool shouldRender = true;
        protected override bool ShouldRender() => shouldRender;
        private async void InitializedComponent()
        {
            ChartSingleton.Service = toastService;
            ChartSingleton.InfoChart += async (sender, args) =>
               await ChartSingleton.WriteInfo(args);
            await Task.Run(() => ChartSingleton.RegisterEvent());
        }
        protected override async Task OnInitializedAsync()
        {
            async Task Initialized(){
                if (!Sensors.Any() && !Turbines.Any())
                {
                    await ChartSingleton.CallTurbinesAndSensor();
                    InitializedComponent();
                    await AwaitSensorAndTurbine();
                    var allChart = await ChartSingleton.GetAllChart();
                    ChartInfo.AddRange(allChart.Select(info => new ChartInfo(info.Item1, info.Item2)).ToList());
                }
            }
            await Call(Initialized);
        }
        private async Task Call(Func<Task> call)
        {
            try
            {
                await call();
            }
            catch (Exception e)
            {
                ChartSingleton = new();
                InitializedComponent();
                toastService.ShowError(e.ToString());

            }
        }  
        private async Task AwaitSensorAndTurbine()
        {
            await foreach (var sensor in ChartSingleton.GetSensor())
            {
                Sensors.Add(sensor);
            }
            await foreach (var turbine in ChartSingleton.GetTurbine())
            {
                Turbines.Add(turbine);
            }
        } 
        private async void CallChartData()
        {
            async Task ChartData()
            {
                var nameTurbine = Turbines.Find(value => value.Id == idTurbine).Value;
                var nameSensor = Sensors.Find(value => value.Id == idSensor).Value;
                var valueError = ErrorByTurbine.Find(value => value.Id == error).Value;
                var info = new InfoChartRecord(idTurbine, nameTurbine, idSensor, nameSensor, Convert.ToInt32(valueError), period);
                await ChartSingleton.ChartInfoTurbine(info, idChart);
                await foreach (var turbine in ChartSingleton.GetInfoChart())
                {
                    infoChart.Add(turbine);
                }
                StateHasChanged();
            } 
            await Call(ChartData);
        } 
         
        public ConfigChart GetConfig(IEventComponent periods) => idChart switch
        {
            TypeChartUtils.LinearChart => lineChartDraw.CreateLineChart(periods as ResponseSerieByPeriod),
            TypeChartUtils.LinearChartWithWarning => lineChartDraw.CreateLineChartWarning(periods as ResponseSerieByPeriodWarning),
            TypeChartUtils.ScatterChart => scatterChartDraw.CreateScatterChart(periods as ResponseSerieByPeriod),
            TypeChartUtils.ScatterChartWithWarning => scatterChartDraw.CreateScatterChartWithWarning(periods as ResponseSerieByPeriodWarning),
            TypeChartUtils.RadarChartWithWarning => radarCharttDraw.CreateRadarChartWithWarning(periods as ResponseSerieByPeriodWarning),
            _ => throw new NotImplementedException("This chart are not implemented"),
        };  
         
    }
}
