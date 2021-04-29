using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientPltTurbine.Controllers.LoadFileController
{
    public interface ILoadFileController
    {
        Task[] ReadBasicFiles(Dictionary<string, Microsoft.AspNetCore.Components.Forms.IBrowserFile> files);
        Task[] ReadEventSensorTurbine(Dictionary<string, Microsoft.AspNetCore.Components.Forms.IBrowserFile> files);
        Task[] ReadSensorTurbine(Dictionary<string, Microsoft.AspNetCore.Components.Forms.IBrowserFile> files);
    }
}
