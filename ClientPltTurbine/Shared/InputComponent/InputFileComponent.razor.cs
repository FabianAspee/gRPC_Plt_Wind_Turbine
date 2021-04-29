using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.InputComponent
{
    public partial class InputFileComponent
    {
        [Parameter]
        public string Id { get; set; }
        [Parameter]
        public EventCallback<InputFileChangeEventArgs> ValueChanged { get; set; }

        public Task OnValueChanged(InputFileChangeEventArgs files)
        {
            return null;
        }
    }
}
