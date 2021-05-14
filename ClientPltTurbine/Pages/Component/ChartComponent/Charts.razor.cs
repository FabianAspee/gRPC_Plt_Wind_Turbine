using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.BarChartDraw.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.BarChartDraw.Implementation;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.LineChartDraw.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.LineChartDraw.Implementation;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.RadarChartDraw.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.RadarChartDraw.Implementation;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.ScatterChartDraw.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.ScatterChartDraw.Implementation;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using ClientPltTurbine.Pages.Component.UtilComponent;
using ClientPltTurbine.Shared.ChartJsComponent.ConfigGeneral;
using PltTurbineShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.ChartComponent
{
    public partial class Charts 
    {
        private readonly ChartSingleton ChartSingleton = new();
        private readonly IList<IEventComponent> infoChart = new List<IEventComponent>(); 
        private readonly ILineChartDraw lineChartDraw = LineChartDraw.Instance;
        private readonly IScatterChartDraw scatterChartDraw = ScatterChartDraw.Instance;
        private readonly IRadarChartDraw radarCharttDraw = RadarChartDraw.Instance;
        private readonly IBarChartDraw barrCharttDraw = BarChartDraw.Instance;
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
                    InitializedComponent();
                    await ChartSingleton.CallTurbinesAndSensor();
                    await AwaitSensorAndTurbine();
                    var allChart = await ChartSingleton.GetAllChart();
                    ChartInfo.AddRange(allChart.Select(info => new ChartInfo(info.Item1, info.Item2, info.Item1.ToString())).ToList());
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
                var nameSensor = Sensors.Find(value => value.Id == infoSensor.idSensor && value.IsOwn==infoSensor.isOwn);
                var valueError = ErrorByTurbine.Find(value => value.Id == error).Value;
                var info = new InfoChartRecord(idTurbine, nameTurbine, infoSensor.idSensor, nameSensor.Value, Convert.ToInt32(valueError), period, nameSensor.IsOwn);
                await ChartSingleton.ChartInfoTurbine(info, idChart);
                infoChart.Clear();
                await foreach (var turbine in ChartSingleton.GetInfoChart())
                {
                    infoChart.Add(turbine);
                }
                recallChartInfo = true;

                StateHasChanged();
            } 
            await Call(ChartData);
        } 
         
        private ConfigChart GetConfig(IEventComponent periods) => idChart switch//attenzione con chart with warning e quelli senza
        {
            TypeChartUtils.LinearChart => lineChartDraw.CreateLineChart(GetResponseSerieByPeriod(periods)),
            TypeChartUtils.LinearChartWithWarning when periods is ResponseSerieByPeriodWarning periodWarning=> lineChartDraw.CreateLineChartWarning(periodWarning),
            TypeChartUtils.ScatterChart => scatterChartDraw.CreateScatterChart(GetResponseSerieByPeriod(periods)),
            TypeChartUtils.ScatterChartWithWarning when periods is ResponseSerieByPeriodWarning periodWarning => scatterChartDraw.CreateScatterChartWithWarning(periodWarning),
            TypeChartUtils.RadarChartWithWarning when periods is ResponseSerieByPeriodWarning periodWarning => radarCharttDraw.CreateRadarChartWithWarning(periodWarning),
            TypeChartUtils.LineAreaChartWithWarning when periods is ResponseSerieByPeriodWarning periodWarning => lineChartDraw.CreateLineChartWarningInPeriod(periodWarning),
            TypeChartUtils.BarChartWithWarning when periods is ResponseSerieByPeriodWarning periodWarning => barrCharttDraw.CreateBarChartWarning(periodWarning),
            TypeChartUtils.LinearChartWithWarning or TypeChartUtils.LineAreaChartWithWarning 
            or TypeChartUtils.RadarChartWithWarning or TypeChartUtils.BarChartWithWarning or TypeChartUtils.ScatterChartWithWarning=>RecallChartData(),
            _ => throw new NotImplementedException("This chart are not implemented"),
        };

        private ConfigChart RecallChartData()
        {
            recallChartInfo = false;
            CallChartData();
            return default;
        }

        private static ResponseSerieByPeriod GetResponseSerieByPeriod(IEventComponent periods) => periods switch
        {
            ResponseSerieByPeriod response => response,
            ResponseSerieByPeriodWarning periodWarning => new ResponseSerieByPeriod(periodWarning.IsFinish, periodWarning.Record.RecordLinearChart),
            _ => throw new NotImplementedException()
        };
    }
}
