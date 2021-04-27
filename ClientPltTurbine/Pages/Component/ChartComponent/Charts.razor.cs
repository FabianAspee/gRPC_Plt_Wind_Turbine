using ChartJs.Blazor;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.LineChart;
using ChartJs.Blazor.PieChart;
using ChartJs.Blazor.ScatterChart;
using ChartJs.Blazor.Util;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.LineChartDraw.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.LineChartDraw.Implementation;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.ScatterChartDraw.Contract;
using ClientPltTurbine.Pages.Component.ChartComponent.DesignChart.ScatterChartDraw.Implementation;
using ClientPltTurbine.Pages.Component.ChartComponent.EventChart;
using PltTurbineShared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.ChartComponent
{
    public partial class Charts
    {
        private LineConfig Config;
        private readonly List<IEventComponent> infoChart = new();
        private readonly List<Sensor> Sensors = new();
        private readonly List<Turbine> Turbines = new();
        private readonly List<ErrorTurbine> ErrorByTurbine = new();
        private readonly List<ChartInfo> ChartInfo = new();
        private readonly ILineChartDraw lineChartDraw = LineChartDraw.Instance;
        private readonly IScatterChartDraw scatterChartDraw = ScatterChartDraw.Instance;
        private bool shouldRender = true;
        private int idTurbine; 
        private int idSensor;
        private int period;
        private int error;
        private int idChart;
        private Chart _chart;
        protected override bool ShouldRender() => shouldRender;
        protected override async Task OnInitializedAsync()
        {
            if (!Sensors.Any() && !Turbines.Any())
            {
                await ChartSingleton.CallTurbinesAndSensor();
                ChartSingleton.Service = toastService;
                ChartSingleton.InfoChart += async (sender, args) =>
                   await ChartSingleton.WriteInfo(args);
                await Task.Run(() => ChartSingleton.RegisterEvent());
                await AwaitSensorAndTurbine();
                var allChart = await ChartSingleton.GetAllChart();
                ChartInfo.AddRange(allChart.Select(info => new ChartInfo(info.Item1, info.Item2)).ToList());
            } 
        }
        private async void ChangeInfoTurbine(int idTurbine)
        {
            ErrorByTurbine.Clear();
            infoChart.Clear();
            this.idTurbine = idTurbine;
            var result = await ChartSingleton.CallErrorByTurbine(idTurbine);
            ErrorByTurbine.AddRange(result.Item2.Zip(Enumerable.Range(0,result.Item2.Count))
                .Select(values=>new ErrorTurbine(values.Second,values.First)).ToList());
            StateHasChanged();
        }
        private void ChangeInfoSensor(int idSensor) => this.idSensor = idSensor;

        private void ChangeInfoError(int error) => this.error = error;


        private void ChangeInfoPeriod(int period) => this.period = period;
        private void ChangeInfoChart(int idChart) => this.idChart = idChart;

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
            var nameTurbine = Turbines.Find(value => value.Id == idTurbine).Value;
            var nameSensor = Sensors.Find(value => value.Id == idSensor).Value;
            var valueError = ErrorByTurbine.Find(value=>value.Id==error).Value;
            var info = new InfoChartRecord(idTurbine,nameTurbine,idSensor, nameSensor, Convert.ToInt32(valueError), period);
            await ChartSingleton.ChartInfoTurbine(info, idChart);
            await foreach (var turbine in ChartSingleton.GetInfoChart())
            {
                infoChart.Add(turbine);
            }
            StateHasChanged();
        } 
         
        public ConfigBase GetConfig(IEventComponent periods) => idChart switch
        {
            TypeChartUtils.LinearChart => lineChartDraw.CreateLineChart(periods as ResponseSerieByPeriod),
            TypeChartUtils.LinearChartWithWarning => lineChartDraw.CreateLineChartWarning(periods as ResponseSerieByPeriodWarning),
            TypeChartUtils.ScatterChart => scatterChartDraw.CreateScatterChart(periods as ResponseSerieByPeriod),
            TypeChartUtils.ScatterChartWithWarning => scatterChartDraw.CreateScatterChartWithWarning(periods as ResponseSerieByPeriodWarning),
            _ => throw new NotImplementedException("This chart are not implemented"),
        };  
        public async void CreateChart()
        {
            PieConfig ConfigPie = new()
            {
                Options = new PieOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = "ChartJs.Blazor Pie Chart"
                    }
                }
            };

            foreach (string color in new[] { "Red", "Yellow", "Green", "Blue" })
            {
                ConfigPie.Data.Labels.Add(color);
            }

            PieDataset<int> dataset = new PieDataset<int>(new[] { 6, 5, 3, 7 })
            {
                BackgroundColor = new[]
                {
            ColorUtil.ColorHexString(255, 99, 132), // Slice 1 aka "Red"
            ColorUtil.ColorHexString(255, 205, 86), // Slice 2 aka "Yellow"
            ColorUtil.ColorHexString(75, 192, 192), // Slice 3 aka "Green"
            ColorUtil.ColorHexString(54, 162, 235), // Slice 4 aka "Blue"
        }
            };

            ConfigPie.Data.Datasets.Add(dataset);
        }

        private static readonly Random _rng = new();

        public static class ChartColors
        {
            private static readonly Lazy<IReadOnlyList<Color>> _all = new Lazy<IReadOnlyList<Color>>(() => new Color[7]
            {
                Red, Orange, Yellow, Green, Blue, Purple, Grey
            });

            public static IReadOnlyList<Color> All => _all.Value;

            public static readonly Color Red = Color.FromArgb(255, 99, 132);
            public static readonly Color Orange = Color.FromArgb(255, 159, 64);
            public static readonly Color Yellow = Color.FromArgb(255, 205, 86);
            public static readonly Color Green = Color.FromArgb(75, 192, 192);
            public static readonly Color Blue = Color.FromArgb(54, 162, 235);
            public static readonly Color Purple = Color.FromArgb(153, 102, 255);
            public static readonly Color Grey = Color.FromArgb(201, 203, 207);
        }

        public static IReadOnlyList<string> Months { get; } = new ReadOnlyCollection<string>(new[]
        {
            "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
        });

        private static int RandomScalingFactorThreadUnsafe() => _rng.Next(-100, 100);

        public static int RandomScalingFactor()
        {
            lock (_rng)
            {
                return RandomScalingFactorThreadUnsafe();
            }
        }

        public static IEnumerable<int> RandomScalingFactor(int count)
        {
            int[] factors = new int[count];
            lock (_rng)
            {
                for (int i = 0; i < count; i++)
                {
                    factors[i] = RandomScalingFactorThreadUnsafe();
                }
            }

            return factors;
        }

        public static IEnumerable<DateTime> GetNextDays(int count)
        {
            DateTime now = DateTime.Now;
            DateTime[] factors = new DateTime[count];
            for (int i = 0; i < factors.Length; i++)
            {
                factors[i] = now.AddDays(i);
            }

            return factors;
        }
    }
}
