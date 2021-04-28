using ClientPltTurbine.Shared.ChartComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartComponent.DrawLineChart.Implementation;
using ClientPltTurbine.Shared.ChartComponent.DrawScatterChart.Implementation; 
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ClientPltTurbine.Pages.Component.ModelPredictionComponent.ModelPredictions;

namespace ClientPltTurbine.Shared.ChartComponent
{
    public partial class ChartComponent:ComponentBase
    {
        
        public string Id { get; } = Guid.NewGuid().ToString();
        [Parameter]
        public ConfigChart Config { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstShouldRender)
        {
            var nameSetup = Config.GetNameSetup();
            await JSRuntime.InvokeVoidAsync(nameSetup, Id, Config); 
        }
            
    }
}
