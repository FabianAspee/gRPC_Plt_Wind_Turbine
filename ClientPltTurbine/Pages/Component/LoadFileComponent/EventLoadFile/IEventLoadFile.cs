using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.LoadFileComponent.EventLoadFile
{
    internal interface IEventLoadFile :IEventComponent
    {
        public event EventHandler<LoadStatusRecord> LoadSatus;
    }
    public record LoadStatusRecord(int TypeMsg, string Msg) : IEventComponent<EventHandler<LoadStatusRecord>>
    {

        public EventHandler<LoadStatusRecord> ReturnComponent() => this;
    }
}
