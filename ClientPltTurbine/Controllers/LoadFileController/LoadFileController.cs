using ClientPltTurbine.EventContainer;
using ClientPltTurbine.Model.LoadFile.Contract;
using ClientPltTurbine.Model.LoadFile.Implementation;
using Google.Protobuf;
using Grpc.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; 
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientPltTurbine.Controllers.LoadFileController
{
    public class LoadFileController : BaseController, ILoadFileController
    {
        private static readonly List<(string, string, int)> myList = new() { ("specifica_name_turbine.csv", ",", 1), ("name_sensor.csv", ",", 2), ("name_error_sensor.csv", ",", 3), ("Vestas Error Code List.csv", ";", 4) };
        private static readonly List<string> sensors = new() { "Active_Power", "Nacelle_Dir", "Rotor_RPM", "Wind_Dir", "Wind_Speed", "Collarmele_K100", "Collarmele_K101" };
        private static readonly string eventsensor = "WTG_Event";
        private bool IsCsv(string file)=> file.EndsWith(".csv"); 
        public List<Task> ReadBasicFiles()=>
            GetFileDirectory().ToList().Select((filePath) =>
            { 
                if (myList.Exists(element => filePath.Contains(element.Item1)))
                {
                    FileInfo fi = new(filePath);
                    SendEventLoadFile($"Init load file {fi.Name}");
                    ILoadFileModel loadFile = new LoadFileModel();
                    if (IsCsv(filePath))
                    {
                        
                        return (loadFile.LoadCsvFileBasic(filePath), loadFile);
                        
                    }
                    else
                    {
                        return (loadFile.LoadExcelFileBasic(filePath), loadFile);

                    }
                    
                }
                return default;
            }).Where(x => x.Item1 != null && x.loadFile!=null).Select(task=> 
                task.Item1.ContinueWith(result => {
                    var dt = result.Result;
                    FileInfo fi = new(dt.Item1);
                    SendEventLoadFile($"Send file {fi.Name} to server system");
                    task.loadFile.ProcessFileBasic(dt.Item2, fi.Name, fi.Extension, "", dt.Item3);
                }, TaskContinuationOptions.OnlyOnRanToCompletion)).ToList();

        private IEnumerable<string> GetFileDirectory(bool isEvent = false)
        {
            foreach (var file in Directory.GetFiles("Resources/files/"))
                if((file.EndsWith(".xlsx") || file.EndsWith(".csv") || file.EndsWith(".XLS")) && SelectFileSensor(file, isEvent))
                    yield return file;
        }
        private static bool ConditionFilterFile(string sensor, string file) =>
            file.ToLower().Contains(sensor.ToLower()) || file.ToLower().Contains(sensor.Replace("_", "").ToLower())
            || file.ToLower().Contains(sensor.Replace("_", " ").ToLower());

        private static bool SelectFileSensor(string nameFile, bool isEvent)=>
            isEvent? ConditionFilterFile(eventsensor, nameFile) : sensors.Exists(sensor => ConditionFilterFile(sensor, nameFile)); 

        private IEnumerable<(Task<(string, DataTable)>, ILoadFileModel)> ReadFile(bool isEvent = false) =>
            GetFileDirectory(isEvent).Select((filePath) =>
            {
                if (!myList.Exists(element => filePath.Contains(element.Item1)))
                {
                    FileInfo fi = new(filePath);
                    SendEventLoadFile($"Init load file {fi.Name}");
                    ILoadFileModel loadFile = new LoadFileModel();
                    if (IsCsv(filePath))
                    {
                        return (loadFile.LoadCsvFileSensor(filePath), loadFile);
                    }
                    else
                    {
                        return (loadFile.LoadExcelFileSensor(filePath), loadFile);
                    }
                }
                return default;
            });

        public List<Task> ReadSensorTurbine()=>
            ReadFile().Where(x => x.Item1 != null && x.Item2!=null).Select(task =>
                    task.Item1.ContinueWith(result => {
                        var dt = result.Result;
                        FileInfo fi = new(dt.Item1);
                        SendEventLoadFile($"Send file {fi.Name} to server system");
                        task.Item2.ProcessSensorFile(dt.Item2, fi.Name, fi.Extension, "", false);
                    }, TaskContinuationOptions.OnlyOnRanToCompletion)).ToList();

        public List<Task> ReadEventSensorTurbine() =>
             ReadFile(true).Where(x => x.Item1 != null && x.Item2 != null).Select(task =>
                      task.Item1.ContinueWith(result => {
                         var dt = result.Result;
                         FileInfo fi = new(dt.Item1);
                         SendEventLoadFile($"Send file {fi.Name} to server system");
                         task.Item2.ProcessSensorFile(dt.Item2, fi.Name, fi.Extension, "", false);
                     }, TaskContinuationOptions.OnlyOnRanToCompletion)).ToList();

    }
}

