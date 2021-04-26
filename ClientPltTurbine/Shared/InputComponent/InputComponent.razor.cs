using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
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
    }
}
