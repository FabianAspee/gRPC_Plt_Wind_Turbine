using ClientPltTurbine.Model.LoadFile.Contract;
using ClientPltTurbine.Model.LoadFile.Implementation;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Controllers.LoadFileController
{
    public class LoadFileController : BaseController, ILoadFileController
    {
        private static readonly List<(string, string, int)> myList = new() { ("specifica_name_turbine.csv", ",", 1), ("name_sensor.csv", ",", 2), ("name_error_sensor.csv", ",", 3), ("Vestas Error Code List.csv", ";", 4) };
        private static readonly List<string> sensors = new() { "Active_Power", "Nacelle_Dir", "Rotor_RPM", "Wind_Dir", "Wind_Speed", "Collarmele_K100", "Collarmele_K101" };
        private static readonly string eventsensor = "WTG_Event";
        private static bool IsCsv(string file)=> file.EndsWith(".csv"); 
        public Task[] ReadBasicFiles(Dictionary<string, IBrowserFile> files) =>
            files.Select(fileInfo =>
            {  
                FileInfo fi = new(fileInfo.Key);
                SendEventLoadFile($"Init load file {fi.Name}");
                ILoadFileModel loadFile =  new LoadFileModel();
                if (IsCsv(fileInfo.Key))
                {
                        
                    return (loadFile.LoadCsvFileBasic(fileInfo), loadFile);
                        
                }
                else
                {
                    return (loadFile.LoadExcelFileBasic(fileInfo), loadFile);

                } 
            }).Where(x => x.Item1 != null && x.loadFile!=null).Select(task=> 
                task.Item1.ContinueWith(result => {
                    var dt = result.Result;
                    FileInfo fi = new(dt.Item1);
                    SendEventLoadFile($"Send file {fi.Name} to server system");
                    task.loadFile.ProcessFileBasic(dt.Item2, fi.Name, fi.Extension, "", dt.Item3);
                }, TaskContinuationOptions.OnlyOnRanToCompletion)).ToArray();

       

        public Task[] ReadSensorTurbine(Dictionary<string, IBrowserFile> files) =>
            ReadFile(files).Where(x => x.Item1 != null && x.Item2!=null).Select(task => {
                return task.Item1.ContinueWith(result =>
                {
                    var dt = result.Result;
                    FileInfo fi = new(dt.Item1);
                    SendEventLoadFile($"Send file {fi.Name} to server system");
                    return task.Item2.ProcessSensorFile(dt.Item2, fi.Name, fi.Extension, "", false);
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }).ToArray();

        public Task[] ReadEventSensorTurbine(Dictionary<string, IBrowserFile> files) =>
             ReadFile(files,true).Where(x => x.Item1 != null && x.Item2 != null).Select(task =>
                      task.Item1.ContinueWith(result => {
                         var dt = result.Result;
                         FileInfo fi = new(dt.Item1);
                         SendEventLoadFile($"Send file {fi.Name} to server system");
                         return task.Item2.ProcessSensorFile(dt.Item2, fi.Name, fi.Extension, "", false);
                     }, TaskContinuationOptions.OnlyOnRanToCompletion)).ToArray();

        private static IEnumerable<KeyValuePair<string,IBrowserFile>> GetFileDirectoryFileSensor(Dictionary<string, IBrowserFile> files,bool isEvent = false)
        {
            foreach (var file in files)
                if ((file.Key.EndsWith(".xlsx") || file.Key.EndsWith(".csv") || file.Key.EndsWith(".XLS")) && SelectFileSensor(file.Key, isEvent))
                    yield return file;
        }

        private static bool ConditionFilterFile(string sensor, string file) =>
            file.ToLower().Contains(sensor.ToLower()) || file.ToLower().Contains(sensor.Replace("_", "").ToLower())
            || file.ToLower().Contains(sensor.Replace("_", " ").ToLower());

        private static bool SelectFileSensor(string nameFile, bool isEvent) =>
            isEvent ? ConditionFilterFile(eventsensor, nameFile) : sensors.Exists(sensor => ConditionFilterFile(sensor, nameFile));

        private IEnumerable<(Task<(string, DataTable)>, ILoadFileModel)> ReadFile(Dictionary<string, IBrowserFile> files, bool isEvent = false) =>
            GetFileDirectoryFileSensor(files,isEvent).Select(infoFile =>
            {
                if (!myList.Exists(element => infoFile.Key.Contains(element.Item1)))
                {
                    FileInfo fi = new(infoFile.Key);
                    SendEventLoadFile($"Init load file {fi.Name}");
                    ILoadFileModel loadFile = new LoadFileModel();
                    if (IsCsv(infoFile.Key))
                    {
                        return (loadFile.LoadCsvFileSensor(infoFile), loadFile);
                    }
                    else
                    {
                        return (loadFile.LoadExcelFileSensor(infoFile), loadFile);
                    }
                }
                return default;
            });
    }
}

