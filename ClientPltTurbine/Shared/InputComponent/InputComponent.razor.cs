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
        private int Value { get; set; }
        [Parameter]
        public EventCallback<int> ValueChanged { get; set; } 
        private Task OnValueChanged(ChangeEventArgs e)
        {
            Value = e.Value.ToString().Equals(string.Empty)?default:Convert.ToInt32(e.Value);
            return ValueChanged.InvokeAsync(Value);
        }
    }
}
