using Microsoft.AspNetCore.Components;
using PltTurbineShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.SelectComponent
{
    public partial class SelectComponent : ComponentBase
    {
        [Parameter]
        public string Warning { get; set; }
        [Parameter]
        public string Text { get; set; } 
        [Parameter]
        public IReadOnlyList<IInformationDropDrownComponent> Values { get; set; } 
        public IReadOnlyList<IInformationDropDrownComponent> ValuesView { get; set; } 
        [Parameter]
        public int Value { get; set; }
        [Parameter]
        public EventCallback<int> ValueChanged { get; set; } 
        private Task OnValueChanged(ChangeEventArgs e)
        {
            var t = e.Value; 
            return ValueChanged.InvokeAsync(Convert.ToInt32(Value));
        } 
        
        protected override async Task OnParametersSetAsync()
        {    
            ValuesView = Values?.ToList();
            await base.OnParametersSetAsync();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            ValuesView = Values?.ToList(); 
            await base.OnAfterRenderAsync(firstRender);
        }
       
    }
}
