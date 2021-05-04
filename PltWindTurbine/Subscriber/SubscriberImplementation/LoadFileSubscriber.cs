using PltWindTurbine.Services.LoadFilesService;
using PltWindTurbine.Subscriber.SubscriberContract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PltWindTurbine.Subscriber.EventArgument;
using PltWindTurbine.Database.Utils;
using PltWindTurbine.Database.DatabaseConnection;
using PltWindTurbine.Database.TableDatabase;
using System.Text.RegularExpressions;
using PltWindTurbine.Subscriber.EventArgument.EventContainer;
using System.Threading;

namespace PltWindTurbine.Subscriber.SubscriberImplementation
{
    public class LoadFileSubscriber : AbstractSubscriber, ILoadFileSubscriber
    { 
        private readonly Dictionary<string, List<CreateDataTable>> infoTurbines = new();
        private readonly CommonImplementationDatabase database = RetreiveImplementationDatabase.Instance.ImplementationDatabase;
        private readonly List<string> Turbines = new() { "Active_Power", "Nacelle_Dir", "Rotor_RPM", "Wind_Dir", "Wind_Speed", "Collarmele_K100", "Collarmele_K101" }; 
        private readonly List<string> Event = new() { "WTG_Event" };  
        public async Task LoadFilesInfoTurbine(FileUploadRequest file) =>
            await LoadFile(new CreateDataTable<InfoTurbine>(file.Msg1.File.Metadata.Name, file.Block, file.Msg1, file.TotalDimension, file.IsUpload, file.Msg1.NameTable));
        public async Task LoadFilesNameSensor(FileUploadRequest file) =>
            await LoadFile(new CreateDataTable<NameSensor>(file.Msg2.File.Metadata.Name, file.Block, file.Msg2, file.TotalDimension, file.IsUpload, file.Msg2.NameTable));
        public async Task LoadFilesNameErrorSensor(FileUploadRequest file) =>
            await LoadFile(new CreateDataTable<NameErrorSensor>(file.Msg3.File.Metadata.Name, file.Block, file.Msg3, file.TotalDimension, file.IsUpload, file.Msg3.NameTable));  
        public async Task LoadFilesErrorCode(FileUploadRequest file)=>
            await LoadFile(new CreateDataTable<ErrorCode>(file.Msg4.File.Metadata.Name, file.Block, file.Msg4, file.TotalDimension, file.IsUpload, file.Msg4.NameTable));
        
        private static DataTable AddNewRow(DataTable data, List<string> row)
        {
            DataRow newRow = data.NewRow();
            newRow.ItemArray = row.ToArray();
            data.Rows.Add(newRow);
            return data;
        } 
        private static Task<double> RemainingCalculus(List<CreateDataTable> infoTurbine, long totalSize)
        {
            var LenData = infoTurbine.Count;
            double percent = (100 * Convert.ToDouble(LenData)) / Convert.ToDouble(totalSize);
            return Task.FromResult(Math.Round(percent,2));
        } 
        private async Task<List<CreateDataTable>> AddToDictionary<T>(Dictionary<string,List<CreateDataTable>> infoTurbines, CreateDataTable<T> table)
        {
            if (!infoTurbines.TryGetValue(table.Name, out List<CreateDataTable> info))
            {
                var listaTurbine = new List<CreateDataTable>();

                listaTurbine.Insert(table.Block, table);
                return listaTurbine; 
            }
            else
            {
                info.Insert(table.Block, table);
                var percent = await RemainingCalculus(info, table.TotalDimension);
                if (percent % 10 == 0)
                {
                    SendEventLoadFile(table.Name, "Load File", Convert.ToInt32(percent));
                }
                return info;
            }
        }
        private static byte[] ReturnByteFromContent(CreateDataTable table) => table switch
        {
            CreateDataTable<InfoTurbine> value => value.Msg.File.File.Content.ToByteArray(),
            CreateDataTable<NameSensor> value => value.Msg.File.File.Content.ToByteArray(),
            CreateDataTable<NameErrorSensor> value => value.Msg.File.File.Content.ToByteArray(),
            CreateDataTable<ErrorCode> value => value.Msg.File.File.Content.ToByteArray(),
            CreateDataTable<ReadNormalSensor> value => value.Msg.Files.File.Content.ToByteArray(),
            CreateDataTable<ReadEventSensor> value => value.Msg.Files.File.Content.ToByteArray(),
            _ => throw new NotImplementedException()
        };
        private string ReturnNameTable(CreateDataTable table, string nameFile) => table switch
        {
            CreateDataTable<InfoTurbine> when infoTurbines[nameFile].First() is CreateDataTable<InfoTurbine> => (infoTurbines[nameFile].First() as CreateDataTable<InfoTurbine>).NameTable,
            CreateDataTable<NameSensor>  when infoTurbines[nameFile].First() is CreateDataTable<NameSensor> => (infoTurbines[nameFile].First() as CreateDataTable<NameSensor>).NameTable,
            CreateDataTable<NameErrorSensor>  when infoTurbines[nameFile].First() is CreateDataTable<NameErrorSensor> => (infoTurbines[nameFile].First() as CreateDataTable<NameErrorSensor>).NameTable,
            CreateDataTable<ErrorCode>  when infoTurbines[nameFile].First() is CreateDataTable<ErrorCode> => (infoTurbines[nameFile].First() as CreateDataTable<ErrorCode>).NameTable,
            _ => throw new NotImplementedException()
        };

        private static DataTable CreateDataTableInfo<T>(Dictionary<string, List<CreateDataTable>> infoTurbines, CreateDataTable<T> table)
        {
            var CreateColumn = true;
            DataTable data = new();
            infoTurbines[table.Name].ForEach(value =>
            {
                var result = Encoding.UTF8.GetString(ReturnByteFromContent(value));
                JObject json = JObject.Parse(result);
                if (CreateColumn)
                {
                    var column = JsonConvert.DeserializeObject<List<string>>(json["Column"].ToString());
                    data.Columns.AddRange(column.Select(x => new DataColumn(x)).ToArray());
                    CreateColumn = false;
                }
                var row = JsonConvert.DeserializeObject<List<string>>(json["Row"].ToString());
                data = AddNewRow(data, row);

            });
            return data;
        }
        private Task<DataTable> CreateDataTableInfoF<T>(CreateDataTable<T> table) => Task.Run(()=> CreateDataTableInfo(infoTurbines, table));
        private static DataTable ChangeColumnsNameError(DataTable dtInfo, List<string> replaceColumns)
        {
            dtInfo.SelectAllColumn(col=>col.ColumnName.ToLower()).Zip(dtInfo.SelectAllColumn(x=>x.ColumnName)).ToList().ForEach(columns=>
            {
                columns.First = RegexString(columns.First);
                var finalColumn = replaceColumns.FirstOrDefault(column =>RegexString(column).Equals(columns.First)); 
                if(finalColumn is not null)
                {
                    dtInfo.Columns[columns.Second].ColumnName = finalColumn;
                }
                else
                {
                    dtInfo.Columns.Remove(columns.Second);
                } 
            });
            return dtInfo;
        }
        private async Task LoadFile<T>(CreateDataTable<T> table)
        {
            if (table.IsUpload)
            {
                infoTurbines[table.Name] = await AddToDictionary(infoTurbines, table);

            }
            else if (infoTurbines.ContainsKey(table.Name))
            { 
                SendEventFile(table.Name, "Init process file in server");
                await CreateDataTableInfoF(table).ContinueWith(result =>
                {
                    var data = result.Result;
                    var tableName = ReturnNameTable(table, table.Name);
                    var columnReplace = database.SelectColumnFrom(tableName); 
                    SendEventFile(table.Name, "Init insert data into database");
                    return (ChangeColumnsNameError(data, columnReplace), tableName);
                }, TaskContinuationOptions.OnlyOnRanToCompletion).ContinueWith(result =>
                { 
                    var (data,tableName) = result.Result; 
                    database.InsertInfoPlt(data, tableName);
                });


            }
        }

        private async Task StartProcess(DataTable data, string nameFile, bool isEvent=false)
        {
            await NormalizationData(data, nameFile).ContinueWith(result => {
                var normalizedData = result.Result; 
                var nameTurbine = database.SelectAllTurbineInfo();
                var nameSensors = isEvent? ConvertToNormalSensor(database.SelectAllNameSensorError()): ConvertToNormalSensor(database.SelectAllNameSensor()); 
                var groupDt = !isEvent ? AddNameSensor(normalizedData, nameSensors) : AddNameSensor(ChangeNameColumn(normalizedData, nameTurbine), nameSensors, isEvent); 
                return CreateDataFrameTurbine(groupDt, nameTurbine, nameSensors, isEvent);
            }, TaskContinuationOptions.OnlyOnRanToCompletion).ContinueWith(finalResult=> { 
                finalResult.Result.ForEach(task =>task.ContinueWith(res =>
                {
                    if (isEvent)
                    { 
                        database.InsertInfoEventWindTurbine(res.Result);
                    }
                    else
                    { 
                        database.InsertInfoWindTurbine(res.Result);
                    }
                }, TaskContinuationOptions.ExecuteSynchronously));
                  
            }, TaskContinuationOptions.OnlyOnRanToCompletion); 
        }
        public record NormalSensor(int IdSensor, string NameSensor);
        private static List<NormalSensor> ConvertToNormalSensor<T>(List<T> sensor_Infos) => sensor_Infos switch
        {
            List<Error_Sensor> error => error.Select(info=>new NormalSensor(info.Id,info.Sensor_Name)).ToList(),
            List<Sensor_Info>  sensor => sensor.Select(info => new NormalSensor(info.Id, info.Sensor_Name)).ToList(),          
           _ =>  throw new NotImplementedException()
        };

        private static Dictionary<string, List<string>> ReconstructSeries(Dictionary<string, List<string>> finalFrame)
        {
            var indexDelete = finalFrame.Last().Value.Zip(Enumerable.Range(0, finalFrame.Last().Value.Count))
                .Where(val => val.First == "NV" || val.First == "-" || val.First == null || val.First.Equals(string.Empty)).Select(val=>val.Second).ToList();

            var finalDt= ClearFinalValue(finalFrame.Select(values => 
                (values.Key,values.Value.Where((value, index) => !indexDelete.Exists(val => val == index)))).ToDictionary(key=>key.Key,value=>value.Item2.ToList()));
            var maxDate = finalDt.Values.First().Max(dateStr => ParserDateSpecificFormat(dateStr));
            var dictionary = new Dictionary<string,List<string>>();
            return DateTimeRange(new DateTime(2018, 6, 22, 00, 10, 00), maxDate, 10, finalDt).SelectMany(value=>value).GroupBy(val=>val.Item1)
                .ToDictionary(key=>key.Key, value=>value.Select(val=>val.Item2).ToList());
              
        }
        private static IEnumerable<List<(string, string)>> DateTimeRange(DateTime start, DateTime end,int delta, Dictionary<string,List<string>> oldDate)
        {
            var current = start;
            while(current < end)
            {
                
                if (oldDate.First().Value.Count > 0 && Math.Abs((current - ParserDateSpecificFormat(ValidationFormatData(oldDate.First().Value.First()))).TotalSeconds / 60) <= 5)
                {
                    var result = oldDate.Select(keyValue=>(keyValue.Key,keyValue.Value.First())).ToList();
                    oldDate = oldDate.ToDictionary(key=>key.Key, value=>value.Value.Skip(1).ToList());
                    yield return  new List<(string, string)>() { result.First(), result.Skip(1).First() };
                } 
                else
                {
                    var currentAux = current;
                    current = currentAux.AddMinutes(delta);
                    yield return new List<(string, string)>() { ("date", currentAux.ToString("yyyy/MM/dd HH:mm:ss")), ("value", (-1).ToString())};
                    
                }
            }
               
        }
        private static List<Task<InfoByTurbineToTable>> CreateDataFrameTurbine((Dictionary<string, List<string>>, string) dtWithName, List<Wind_Turbine_Info> nameTurbines, List<NormalSensor> nameSensors, bool isEvent)
        {
            var date = dtWithName.Item1.Where(keysValue => ((keysValue.Key == "Date" ? keysValue.Key : (keysValue.Key == "PCTimeStamp" ? keysValue.Key : (keysValue.Key == "Data" ? keysValue.Key : string.Empty))).Length > 0))
                .ToDictionary(value => "date", element => element.Value);
            return dtWithName.Item1.Select(keyAndValue =>
            {
                var nameTurbine = nameTurbines.FirstOrDefault(name =>
                 RemoveSpecialCharacters(keyAndValue.Key).Contains(RemoveSpecialCharacters(name.ToSpecialString())) || keyAndValue.Key.StartsWith(SelectNameTurbine(name.ToSpecialString())));
                if (nameTurbine is not null && !isEvent)
                {    
                    return (Task.Run(() => ClearFinalValue(date.Concat(new Dictionary<string, List<string>>() { { "value", keyAndValue.Value } }).ToDictionary(values => values.Key, values => values.Value.ToList()))), nameTurbine);
                }
                else if (nameTurbine is not null && isEvent)
                { 
                    return (Task.Run(() => ReconstructSeries(date.Concat(new Dictionary<string, List<string>>() { { "value", keyAndValue.Value} }).ToDictionary(values => values.Key, values => values.Value.ToList()))), nameTurbine);
                }
                else
                {
                    return default;
                }
            }).Where(x => x.Item1 is not null && x.nameTurbine is not null)
            .Select(clearDictionaryByTurbine =>clearDictionaryByTurbine.Item1.ContinueWith(result=> SearchInTurbineAndSensor(clearDictionaryByTurbine.nameTurbine, dtWithName.Item2, nameSensors, result.Result, isEvent)))
            .Where(result => result != null).ToList();
             
             
        }
        private static string RemoveSpecialCharacters(string value) => string.Join("", value.Where(x =>char.IsLetterOrDigit(x)).ToArray()); 
         private static string SelectNameTurbine(string turbine)
        { 
            var values = turbine.Split(".");
            return $"{values[^1]}_";
        }
        
        private static InfoByTurbineToTable SearchInTurbineAndSensor(Wind_Turbine_Info nameTurbine, string columnSensor, List<NormalSensor> nameSensors, Dictionary<string, List<string>> dtTurbine, bool isEvent=false)
        {
            var idTurbine = nameTurbine.Id;
            var columnLower = RegexString(columnSensor.ToLower());
            var idSensor = nameSensors.FirstOrDefault(sensor => RegexString(sensor.NameSensor.ToLower()).Contains(columnLower))?.IdSensor;
            if(idSensor is not null)
            {
                return new InfoByTurbineToTable(dtTurbine, ("id_turbine",idTurbine), (!isEvent ? "id_sensor": "id_error_sensor",idSensor.Value));
            }
            else
            { 
                Console.WriteLine($"Turbine with name {nameTurbine.ToSpecialString()} will be deleted from the system");
                return default;
            } 
        }
 
        private static Dictionary<string, List<string>> ClearFinalValue(Dictionary<string, List<string>> valuesTurbine)
        {
            var indexToDelete = valuesTurbine.First().Value.Zip(Enumerable.Range(0, valuesTurbine.First().Value.Count)).GroupBy(date => date.First, date => date.Second).Where(date => date.Count() > 1)
               .ToDictionary(key => key.Key, value => value.ToList()).Values.SelectMany(index => {
                   var result = Convert.ToDouble(index.Aggregate(0, (total, next) => valuesTurbine.Last().Value[next] is not null ? total + next : total)) / Convert.ToDouble(index.Where(next => valuesTurbine.Last().Value[next] is not null).Count());
                   valuesTurbine.Last().Value[index.First()] = result.ToString();
                   return index.Skip(1).ToList();
               }).ToList();
            return valuesTurbine.Select(keyValue => {
                indexToDelete.FastReverse().ToList().ForEach(inbdex => {
                    keyValue.Value.RemoveAt(inbdex);
                });
                return keyValue;
            }).ToDictionary(key=>key.Key, value=> value.Value);   
        }
        private static string RegexString(string stringa) => Regex.Replace(stringa, @"[^a-zA-Z0-9]", ""); 

        private static (Dictionary<string, List<string>>, string) ChangeNameColumn((Dictionary<string, List<string>>,string) normalizedData, List<Wind_Turbine_Info> nameTurbines)=>
            (normalizedData.Item1.Keys.Select(key =>
                    {
                        if (key != "Date" && key != "PCTimeStamp" && key != "Data")
                        {
                            var nameTurbine = RegexString(key).ToLower();
                            var finalName = nameTurbines.FirstOrDefault(name =>nameTurbine.Contains(name.ToString().ToLower()))?? nameTurbines.FirstOrDefault(name => nameTurbine.Contains(name.ToStringAux().ToLower()));
                            if(finalName is not null)
                            {
                                return (finalName.ToString(), normalizedData.Item1[key]);
                            }
                        }
                        else if (key == "Date" || key == "PCTimeStamp" || key == "Data")
                        {

                            return (key,normalizedData.Item1[key]);
                        }
                        return default;
                    }).Where(x=>x.Item2 is not null).ToDictionary(x=>x.Item1, x=>x.Item2),normalizedData.Item2);   
        

        private (Dictionary<string, List<string>>, string) AddNameSensor((Dictionary<string, List<string>>,string) normalizedDataByColumn, List<NormalSensor> nameSensors, bool isEvent=false)=>
            (normalizedDataByColumn.Item1, !isEvent?Turbines.First(nameSensor => ConditionFilterFile(nameSensor, normalizedDataByColumn.Item2)):
            Event.First(nameSensor => ConditionFilterFile(nameSensor, normalizedDataByColumn.Item2))); //pensar en el caso tenga solo un porcentaje de la string
            
        

        private static bool ConditionFilterFile(string nameSensor, string nameFile)
        {
            return nameFile.ToLower().Contains(nameSensor.ToLower()) || nameFile.ToLower().Contains(nameSensor.Replace("_", "").ToLower()) || 
                nameFile.ToLower().Contains(nameSensor.Replace("_", " ").ToLower()) || nameFile.Replace("_", " ").ToLower().Contains(nameSensor.ToLower());
        }
         

        public async Task LoadNormalSensor(ReadInfoSensor file)
        {
            // await LoadFile(new CreateDataTable<NameSensor>(file.Msg2.File.Metadata.Name,file.Block,file.Msg2, file.TotalDimension, file.Msg2.NameTable, file.IsUpload));
           
            if (file.IsUpload)
             {
                 var table = new CreateDataTable<ReadNormalSensor>(file.Msg1.Files.Metadata.Name, file.Block, file.Msg1, file.TotalDimension, file.IsUpload);
                 infoTurbines[file.Msg1.Files.Metadata.Name] = await AddToDictionary(infoTurbines, table); 
             }
             else if(infoTurbines.ContainsKey(file.Msg1.Files.Metadata.Name))
             {
                var table = new CreateDataTable<ReadNormalSensor>(file.Msg1.Files.Metadata.Name, file.Block, file.Msg1, file.TotalDimension, file.IsUpload);
                 await CreateDataTableInfoF(table).ContinueWith(result =>
                 {
                     var data = result.Result;
                    data.Columns.OfType<DataColumn>().Select(x => x.ColumnName).Where(x => x.Contains("Flag")).ToList().ForEach(x => data.Columns.Remove(x));
                    return data;
                }, TaskContinuationOptions.OnlyOnRanToCompletion).ContinueWith(async result=>
                {
                    var data = result.Result; 
                    await StartProcess(data, file.Msg1.Files.Metadata.Name);
                }); 
             } 
        }
        public async Task LoadEventSensor(ReadInfoSensor file)
        {
            if (file.IsUpload)
            {
                var table = new CreateDataTable<ReadEventSensor>(file.Msg2.Files.Metadata.Name, file.Block, file.Msg2, file.TotalDimension, file.IsUpload);
                infoTurbines[file.Msg2.Files.Metadata.Name] = await AddToDictionary(infoTurbines, table);
            }
            else if (infoTurbines.ContainsKey(file.Msg2.Files.Metadata.Name))
            {
                var table = new CreateDataTable<ReadEventSensor>(file.Msg2.Files.Metadata.Name, file.Block, file.Msg2, file.TotalDimension, file.IsUpload);
                await CreateDataTableInfoF(table).ContinueWith(result =>
                {
                    var data = result.Result;
                    data.Columns.OfType<DataColumn>().Select(x => x.ColumnName).Where(x => x.Contains("Flag")).ToList().ForEach(x => data.Columns.Remove(x));
                    return data;
                }, TaskContinuationOptions.OnlyOnRanToCompletion).ContinueWith(async result =>
                {
                    var data = result.Result;
                    await StartProcess(data, file.Msg2.Files.Metadata.Name,true);
                });
            }
        }
        private static Task<(Dictionary<string,List<string>>, string)> NormalizationData(DataTable dataTurbine, string nameFile)
        { 
           return Task.Run(() =>
            {
                var all_column = dataTurbine.Columns.OfType<DataColumn>().ToList();
                var column_date = all_column.First(x => ("Date" == x.ColumnName ? x.ColumnName :
                    ("PCTimeStamp" == x.ColumnName ? x.ColumnName : ("Data" == x.ColumnName ? x.ColumnName : string.Empty))) != string.Empty).ColumnName;
                var rowCollection = dataTurbine.AsEnumerable().ToList();
                var info_all_columns = all_column.Select(x =>
                {
                    return (x.ColumnName, rowCollection.Select(xx => xx.Field<string>(x.ColumnName)).ToList());
                }).ToDictionary(x => x.ColumnName, x => x.Item2);


                return (rowCollection, all_column, info_all_columns, column_date);
            }).ContinueWith(antecedent =>
                  {

                    var (rowCollection, all_column, info_all_columns, column_date) = antecedent.Result;
                    all_column.ForEach(my_column =>
                    {
                        var column = my_column.ColumnName;
                        if (column_date == column && column_date == "PCTimeStamp")
                        {

                            info_all_columns[column] = ChangeFormatDate(info_all_columns[column]);
                        }
                        if (column_date == column)
                        { 
                            info_all_columns[column] = ChangeFormatDate(info_all_columns[column]);
                        }
                        else
                        {

                            info_all_columns[column] = ChangeFormatValue(info_all_columns[column]); 
                        }
                    });
                      return (info_all_columns, nameFile);
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            
        }

        private static List<string> ChangeFormatValue(List<string> values)=> values.Select(value => value != "-" && value != "NV"? value.Replace(",","."):null).ToList();


        private static List<string> ChangeFormatDate(List<string> dates) => dates.Select(date => ValidationFormatData(date.Replace(".", ":"))).ToList();


        private static string ValidationFormatData(string date) => DateTime.Parse(date).ToString("yyyy/MM/dd HH:mm:ss"); 
        private static DateTime ParserDateSpecificFormat(string date) => DateTime.Parse(DateTime.Parse(date).ToString("yyyy/MM/dd HH:mm:ss"));
    }
    public record InfoByTurbineToTable(Dictionary<string, List<string>> BaseInfoTurbine, (string nameColumnT, int idTurbine) IdTurbine , (string nameColumnS, int idSensor) IdSensor);
    public record CreateDataTable();
    public record CreateDataTable<T>(string Name, int Block, T Msg, long TotalDimension, bool IsUpload, string NameTable="") :CreateDataTable; 
}
