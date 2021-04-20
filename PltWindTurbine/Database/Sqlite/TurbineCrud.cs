using PltWindTurbine.Database.DatabaseConnection;
using PltWindTurbine.Database.Utils;
using PltWindTurbine.Subscriber.SubscriberImplementation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.Sqlite
{
    public class TurbineCrud : CommonImplementationDatabase
    {
        public override Task InsertInfoWindTurbine(InfoByTurbineToTable infoTurbine)=>base.InsertInfoWindTurbine(infoTurbine);
        
    }
}
