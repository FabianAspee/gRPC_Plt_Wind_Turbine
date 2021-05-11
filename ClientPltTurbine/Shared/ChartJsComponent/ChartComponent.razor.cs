using ClientPltTurbine.Shared.ChartJsComponent.ConfigGeneral;
using ClientPltTurbine.Shared.ChartJsComponent.DrawLineChart.Implementation;
using ClientPltTurbine.Shared.ChartJsComponent.DrawScatterChart.Implementation;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.ChartJsComponent
{
    public partial class ChartComponent:ComponentBase
    {
        
        public string Id { get; } = Guid.NewGuid().ToString();
        [Parameter]
        public ConfigChart Config { get; set; }
        private static string GetNameSetup(ConfigChart config) => config.GetNameSetup();
        protected override async Task OnAfterRenderAsync(bool firstShouldRender)
        {
            if(Config is not null)
            {
                var (nameSetup, newConfig) = Config switch
                {
                    LineChart line => (GetNameSetup(line), new LineChart().GetConfigChart(line)),
                    ScatterChart scatter => (GetNameSetup(scatter), new ScatterChart().GetConfigChart(scatter)),
                    ConfigChart conf when conf is not null => (GetNameSetup(conf), conf),
                    _ => throw new NotImplementedException()

                };
                await JSRuntime.InvokeVoidAsync(nameSetup, Id, newConfig);
            }
                   
        }
            
    }
}
