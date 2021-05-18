using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.InputComponent
{
    public partial class InputDateComponent
    {
        [Parameter]
        public string Id { get; set; } = string.Empty;
        [Parameter]
        public string Value { get; set; }
        [Parameter]
        public string Description { get; set; }
        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        private Task OnValueChanged(ChangeEventArgs e) => Cast(e.Value);
        private Task Cast(object value) => value switch
        {
            string val when ContainsId => ValueChanged.InvokeAsync(string.Concat(Id, ",", val)),
            _ => ValueChanged.InvokeAsync(default),
        };
        private bool ContainsId =>!(Id == string.Empty);
         
    }
}
