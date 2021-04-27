using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.ChartComponent
{
    public partial class ChartComponentssssss
    {  

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public ChartType Type { get; set; }

        [Parameter]
        public int?[] Data { get; set; }
        [Parameter]
        public int[][] DataS { get; set; }

        [Parameter]
        public string[] BackgroundColor { get; set; }

        [Parameter]
        public string[] Labels { get; set; }

        protected void OnAfterRenderAsync(bool firstRender)
        {
            //Here we create an anonymous type with all the options that need to be sent to Chart.js
            if (!Type.ToString().Equals("Line") && !Type.ToString().Equals("Scatter") && !Type.ToString().Equals("Special"))
            {
                var config = new
                {
                    Type = Type.ToString().ToLower(),
                    Options = new
                    {
                        Responsive = true,
                        Scales = new
                        {
                            YAxes = new[]
                        {
                        new { Ticks = new {
                            BeginAtZero=true
                        } }
                    }
                        }
                    },
                    Data = new
                    {
                        Datasets = new[]
                    {
                    new { Data = Data, BackgroundColor = BackgroundColor}
                },
                        Labels = Labels
                    }
                };
                //await JSRuntime.InvokeVoidAsync("setup", Id, config);
            }
            else if (!Type.ToString().Equals("Scatter") && !Type.ToString().Equals("Special"))
            {
                var config2 = new
                {
                    Type = Type.ToString().ToLower(),
                    Options = new
                    {
                        Responsive = true,
                        Fill = "false",
                        interaction = new
                        {
                            intersect = false
                        },
                        radius = 0,
                    },
                    Data = new
                    {
                        Datasets = new[]
                           {
                            new {
                                Data = Data, Label= "My First Dataset", BorderColor = "rgb(75, 192, 192)"}
            },
                        Labels = Labels,
                    }
                };
            //    await JSRuntime.InvokeVoidAsync("setupspecialline", Id, config2);
            }
            else if (!Type.ToString().Equals("Special"))
            {
              /*  var t = new[] { new Record( 10, 1 ), new Record( 9, 11 ),
                    new Record( 20, 21), new Record( 69, 13),
                new Record( 70, 15), new Record( 19, 17),
                new Record( 110, 31), new Record( 91, 10)};
                DataS = new[] { new[]{ 10, 1 }, new[] { 9, 11 },
                new[] { 20, 21}, new[] { 69, 13},
                new[] { 70, 15}, new[] { 19, 17},
                new[] { 110, 31}, new[] { 91, 10} };
                var config2 = new
                {
                    SteppedLine = false,
                    Type = Type.ToString().ToLower(),
                    Options = new
                    {
                        Responsive = true

                    },
                    Data = new
                    {
                        Datasets = new[]
                        {
                            new { Data = t, BackgroundColor = (new[] { "yellow","red","yellow","red","yellow","brown","white","green"})}
                     },

                    }
                };*/
              //  await JSRuntime.InvokeVoidAsync("setup", Id, config2);
            }
            else
            {
                //await JSRuntime.InvokeVoidAsync("setupspeciallines", Id);
            }

        }
     
}
}
