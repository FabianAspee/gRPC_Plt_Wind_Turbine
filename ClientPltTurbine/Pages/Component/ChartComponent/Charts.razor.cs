using ChartJs.Blazor;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Common.Enums;
using ChartJs.Blazor.LineChart;
using ChartJs.Blazor.PieChart;
using ChartJs.Blazor.Util;
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
        private readonly List<ResponseSerieByPeriod> infoChart = new();
        private readonly List<Sensor> Sensors = new();
        private readonly List<Turbine> Turbines = new(); 
        private bool shouldRender = true;
        private int infoTurbine;
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
            await ChartSingleton.GraphicInfoTurbine(); 
            await foreach (var turbine in ChartSingleton.GetInfoChart())
            {
                infoChart.Add(turbine);
            }
            base.StateHasChanged();
        } 
        public Variant[] _variants = new[]
        {
            new Variant
            {
                SteppedLine = SteppedLine.False,
                Title = "No Step Interpolation",
                Color = ChartColors.Red
            }
        }; 
       
        public LineConfig GetConfig( ResponseSerieByPeriod period)
        {
            LineConfig ConfigLine = new()
            {
                Options = new LineOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = _variants[0].Title
                    }
                }
            };

            string steppedLineCamel = _variants[0].SteppedLine.ToString();
            steppedLineCamel = char.ToUpperInvariant(steppedLineCamel[0]) + steppedLineCamel.Substring(1);

            ConfigLine.Data.Labels.AddRange(period.Record.CustomInfo.Select(x=> x.Date.ToShortDateString()).ToArray());
            ConfigLine.Data.Datasets.Add(new LineDataset<double?>(period.Record.CustomInfo.Select(x => x.Value).ToList())
            {
                Label = $"SteppedLine: SteppedLine.{steppedLineCamel}",
                SteppedLine = _variants[0].SteppedLine,
                BorderColor = ColorUtil.FromDrawingColor(_variants[0].Color),
                Fill = FillingMode.Disabled
            });

            return ConfigLine;
        }

        public class Variant
        {
            public SteppedLine SteppedLine { get; set; }
            public string Title { get; set; }
            public System.Drawing.Color Color { get; set; }
        }

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
