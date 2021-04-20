using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber.EventArgument
{
    public class StatusLoadFile : EventArgs
    {
        public readonly string nameFile;
        public readonly string description;
        public readonly int percent;

        public StatusLoadFile(string nameFile, string description, int percent)
        {
            this.nameFile = nameFile;
            this.description = description;
            this.percent = percent;
        }
    }   
}
