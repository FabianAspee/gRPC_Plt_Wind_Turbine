using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PltWindTurbine.Database.DatabaseConnection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.Utils
{
    public static class ReadFiles
    {
        public static T ReadJsonAndDeserialize<T>(string dbpath)
        {
            using StreamReader file = File.OpenText(dbpath);
            using JsonTextReader reader = new(file);
            return new JsonSerializer().Deserialize<T>(reader); 
            
        }
        public static string CombinePath(string pathCombine) => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), pathCombine);
        
        
    }
   
}
