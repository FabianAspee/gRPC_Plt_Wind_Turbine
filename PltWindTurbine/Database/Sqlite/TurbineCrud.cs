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
        private static Lazy<TurbineCrud> instance = new(() => new TurbineCrud());
        private TurbineCrud() { }
        public static TurbineCrud Instance => instance.Value;
        public override void InsertInfoWindTurbine(InfoByTurbineToTable infoTurbine)
        {
            lock (this)
            {
                base.InsertInfoWindTurbine(infoTurbine);
            }
        }
        public override void InsertInfoEventWindTurbine(InfoByTurbineToTable infoTurbine)
        {
            lock (this)
            {
                base.InsertInfoEventWindTurbine(infoTurbine);
            }
        }
        public override Task SelectWarningAllTurbines(int period)
        {
            lock (this)
            {
                return base.SelectWarningAllTurbines(period);
            }
        }
    }
}
