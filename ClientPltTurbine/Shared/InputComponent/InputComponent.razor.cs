using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.InputComponent
{
    public partial class InputComponent
    { 
        [Parameter]
        public string Type { get; set; }
        [Parameter]
        public string Id { get; set; }
        [Parameter]
        public string PlaceHolder { get; set; }
        [Parameter]
        public string Description { get; set; } 
        [Parameter]
        public EventCallback<int> ValueChanged { get; set; }

        private Task OnValueChanged(ChangeEventArgs e) => Cast(e.Value); 
        private Task Cast(object value)=>value switch
        {
            int val => ValueChanged.InvokeAsync(val), 
            _ => ValueChanged.InvokeAsync(default), 
        };
    }
}
