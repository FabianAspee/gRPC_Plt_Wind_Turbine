using PltWindTurbine.Database.DatabaseConnection;
using PltWindTurbine.Database.Utils;
using PltWindTurbine.Subscriber.SubscriberImplementation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.Mysql
{
    public class TurbineCrud: CommonImplementationDatabase
    {
        private readonly ConnectionToDatabase connectionTo = RetreiveImplementationDatabase.Instance.connectionToDatabase; 
        private static InfoByTurbineToTable ReplaceSpecialCharacter(InfoByTurbineToTable infoByTurbines)=>
             new(infoByTurbines.BaseInfoTurbine.Select(keyValues=>
            (keyValues.Key,keyValues.Value.Select(value=>value=="NV"?null:value))).ToDictionary(key=>key.Key,value=>value.Item2.ToList())
            , infoByTurbines.IdTurbine, infoByTurbines.IdSensor); 
        public override void InsertInfoWindTurbine(InfoByTurbineToTable infoTurbine)=> base.InsertInfoWindTurbine(ReplaceSpecialCharacter(infoTurbine));
        
    }
}
