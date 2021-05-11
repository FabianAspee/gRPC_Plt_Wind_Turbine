using ClientPltTurbine.Shared.Chart3DComponent.ConfigGeneral;
using Microsoft.AspNetCore.Components;
using System;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.Chart3DComponent
{
    public partial class Chart3DComponent : ComponentBase
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        [Parameter]
        public ConfigChart3D Config { get; set; }
        private static string GetNameSetup(ConfigChart3D config) => config.GetNameSetup();
        protected override async Task OnAfterRenderAsync(bool firstShouldRender)
        {
            if (Config is not null)
            {
                var (nameSetup, newConfig) = Config switch
                {
                    ConfigChart3D conf when conf is not null => (GetNameSetup(conf), conf), 
                    _ => throw new NotImplementedException()

                };
                await JSRuntime.InvokeVoidAsync(nameSetup, Id, newConfig);
            }

        }

    }
} 
