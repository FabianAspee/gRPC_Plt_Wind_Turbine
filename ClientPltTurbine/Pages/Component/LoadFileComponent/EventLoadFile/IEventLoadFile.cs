using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.LoadFileComponent.EventLoadFile
{
    internal interface IEventLoadFile
    {
        public event EventHandler<LoadStatusRecord> LoadSatus;
    }
    public record LoadStatusRecord(int TypeMsg, string Msg);
}
