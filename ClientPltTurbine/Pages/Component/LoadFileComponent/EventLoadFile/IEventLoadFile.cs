using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.LoadFileComponent.EventLoadFile
{
    public interface IEventLoadFile 
    {
        public event EventHandler<IEventComponent> LoadSatus;
    }
    public record LoadStatusRecord(int TypeMsg, string Msg) : IEventComponent; 
}
