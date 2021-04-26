using Microsoft.AspNetCore.Components;

namespace ClientPltTurbine.Shared.ButtonComponent
{
    public partial class ButtonComponent: ComponentBase
    {
        [Parameter]
        public EventCallback OnClick { get; set; }
        [Parameter]
        public string Text { get; set; } 
         
    }
}
