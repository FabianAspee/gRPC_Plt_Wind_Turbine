using ClientPltTurbine.Model.LoadFile.Contract;
using ExcelDataReader;
using Google.Protobuf;
using Grpc.Core;
using Newtonsoft.Json.Linq;
using FilePlt = PltWindTurbine.Services.LoadFilesService.File;
using PltWindTurbine.Services.LoadFilesService;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ExcelDataReader.Exceptions;
using Microsoft.AspNetCore.Components.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace ClientPltTurbine.Model.LoadFile.Implementation
{
    public class LoadFileModel : BaseModel, ILoadFileModel
    {
        private readonly LoadFiles.LoadFilesClient _clientReadBasicFiles;
        private readonly AsyncDuplexStreamingCall<FileUploadRequest, FileUploadResponse> _duplexStreamReadBasicFiles;
        private readonly AsyncDuplexStreamingCall<ReadInfoSensor, FileUploadResponse> _duplexStreamReadSensorFiles;
        private static readonly List<(string, string, int)> list = new() { ("specifica_name_turbine.csv", ",", 1), ("name_sensor.csv", ",", 2), ("name_error_sensor.csv", ",", 3), ("Vestas Error Code List.csv", ";", 4) };
        private readonly List<(string, string, int)> myList = list;

        public LoadFileModel()
        {
            _clientReadBasicFiles = new LoadFiles.LoadFilesClient(channel);
            _duplexStreamReadBasicFiles = _clientReadBasicFiles.LoadFilesInfoTurbine();
            _duplexStreamReadSensorFiles = _clientReadBasicFiles.ReadSensor();
            _ = HandleResponsesReadBasicFileAsync();
            _ = HandleResponsesReadSensorFileAsync();
        }  

        public Task<(string,DataTable,int)> LoadCsvFileBasic(KeyValuePair<string, IBrowserFile> infoFile) =>
            Task.Run(() => {
                var myitem = myList.First(name => infoFile.Key.Contains(name.Item1));  
                DataTable data = GetDataTableFromFile(infoFile, myitem.Item2);                 
                return (infoFile.Key, data, myitem.Item3);
            }); 

        public Task<(string, DataTable, int)> LoadExcelFileBasic(KeyValuePair<string,IBrowserFile> infoFile)
        {
            if (ExistInCsv(infoFile.Key))
            {
                return LoadCsvFileBasic(infoFile); 
            }
            else
            {
                return Task.Run(async () =>
                {
                    var myitem = myList.First(name => infoFile.Key.Contains(name.Item1));
                    DataTable data = await ReadExcel(infoFile); 
                    return (infoFile.Key, data, myitem.Item3);
                }); 
            } 
        }
        

        public Task ProcessFileBasic(DataTable file, string name, string type, string sep, int id)=>
            Task.Run(() =>
            { 
                var allColumn = file.Columns.OfType<DataColumn>().Select(x => x.ColumnName).ToList();
                var count = 0;
                var dimension = file.Rows.Count;
                file.AsEnumerable().ToList().ForEach(x => {
                    JArray jarray = new();
                    JObject rowAndColumn = new();
                    allColumn.ForEach(column => jarray.Add(column));
                    rowAndColumn["Column"] = jarray;
                    jarray = new();
                    x.ItemArray.ToList().ForEach(row => jarray.Add(row));
                    rowAndColumn["Row"] = jarray;
                    FileUploadRequest fileUpload = id == 1 ? ConstructInfoTurbine(dimension, count, name, type, sep, rowAndColumn) : (id == 2 ? ConstructNameSensor(dimension, count, name, type, sep, rowAndColumn) :
                    (id == 3 ? ConstructNameErrorSensor(dimension, count, name, type, sep, rowAndColumn) : (id == 4 ? ConstructErrorCode(dimension, count, name, type, sep, rowAndColumn) : throw new Exception())));
                    _duplexStreamReadBasicFiles.RequestStream.WriteAsync(fileUpload).Wait();
                    count++;
                });
                 
                FileUploadRequest fileUpload2 = id == 1 ? ConstructInfoTurbineFinal(name, type) : (id == 2 ? ConstructNameSensorFinal(name, type) :
                    (id == 3 ? ConstructNameErrorSensorFinal(name, type) : (id == 4 ? ConstructErrorCodeFinal(name, type) : throw new Exception())));
                SendEventLoadFile($"Upload file is complete");
                return _duplexStreamReadBasicFiles.RequestStream.WriteAsync(fileUpload2);
            }); 

        public Task<(string, DataTable)> LoadCsvFileSensor(KeyValuePair<string, IBrowserFile> infoFile) =>
            Task.Run(() => {  
                DataTable data = GetDataTableFromFile(infoFile, ",");
                return (infoFile.Key, data);
            });   
        
        public Task<(string, DataTable)> LoadExcelFileSensor(KeyValuePair<string, IBrowserFile> infoFile)
        { 
            if (ExistInCsv(infoFile.Key))
            {
                
                return LoadCsvFileSensor(infoFile);
            }
            else
            {
                return Task.Run(async () =>
                {
                    DataTable data =await ReadExcel(infoFile);
                    return (infoFile.Key, data);
                }); 
            }
        }

        public Task ProcessSensorFile(DataTable file, string name, string type, string sep, bool isEvent) => 
            Task.Run(() =>
            { 
                var allColumn = file.Columns.OfType<DataColumn>().Select(x => x.ColumnName).ToList();
                var count = 0;
                var dimension = file.Rows.Count;
                file.AsEnumerable().ToList().ForEach(x =>
                {
                    JArray jarray = new();
                    JObject rowAndColumn = new();
                    allColumn.ForEach(column => jarray.Add(column));
                    rowAndColumn["Column"] = jarray;
                    jarray = new();
                    x.ItemArray.ToList().ForEach(row => jarray.Add(row));
                    rowAndColumn["Row"] = jarray;
                    ReadInfoSensor fileUpload = !isEvent ? ConstructSensor(dimension, count, name, type, sep, rowAndColumn) : ConstructEventSensor(dimension, count, name, type, sep, rowAndColumn);
                    _duplexStreamReadSensorFiles.RequestStream.WriteAsync(fileUpload).Wait();
                    count++;
                }); 
                ReadInfoSensor fileUpload2 = !isEvent ? ConstructSensorFinal(name, type) : ConstructEventSensorFinal(name, type);
                SendEventLoadFile($"Upload file is complete");
                return _duplexStreamReadSensorFiles.RequestStream.WriteAsync(fileUpload2);
            });

        private static bool ExistInCsv(string path) => System.IO.File.Exists(path);

        private static List<string> CreateListWithValues(IExcelDataReader reader) =>
            Enumerable.Range(0, reader.FieldCount).Select(index => reader.GetValue(index)?.ToString()).ToList();

        private static List<string> ReadStreamReader(IBrowserFile file, int skip = 2)
        {
            List<string> rows = new();
            string line;
            StreamReader streamReader = new(file.OpenReadStream(file.Size));
            while ((line = streamReader.ReadLine()) != null)
            {
                if (skip == 0)
                {
                    rows.Add(line);
                }
                else
                {
                    skip--;
                }
            }
            return rows;
            
        }
        private static async IAsyncEnumerable<(List<string>, bool)> ReadFileExcel(string name, IBrowserFile file, string sep = "\t", int skip=2)
        {
            if (name.EndsWith(".XLS"))
            {
                List<string> rows = ReadStreamReader(file, skip);
                if (rows.Count > 1)
                {
                    yield return (rows.First().Split(sep).ToList(), true);
                    foreach (var row in rows.Skip(1))
                    {
                        yield return (row.Split(sep).ToList(), false);
                    }
                }
            }
            else
            {
                await foreach(var read in ReadExcelFileWithReader(name, file))
                {
                    yield return read;
                }
                
            }
            
        }
        private static async IAsyncEnumerable<(List<string>, bool)> ReadExcelFileWithReader(string name ,IBrowserFile file)
        {
            using Stream stream = file.OpenReadStream(file.Size);
            var path = $"Resources\\TempFiles\\{name}";
            using FileStream fileStream = System.IO.File.Create(path);
            await stream.CopyToAsync(fileStream);
            using var reader = ExcelReaderFactory.CreateReader(fileStream);
            if (reader.Read())
            {
                yield return (CreateListWithValues(reader), true);
                do
                {
                    while (reader.Read())
                    {
                        yield return (CreateListWithValues(reader), false);
                    }
                } while (reader.NextResult());
            }
        }
        private static async Task<DataTable> ReadExcel(KeyValuePair<string,IBrowserFile> infoFile)
        {
            DataTable data = new();
            await foreach (var values in ReadFileExcel(infoFile.Key,infoFile.Value))
            {
                if (values.Item2)
                {
                    values.Item1.Zip(Enumerable.Range(0, values.Item1.Count)).ToList().ForEach(columnName => {
                        if (data.Columns.Contains(columnName.First))
                        {
                            columnName.First = $"{columnName.First}{columnName.Second}";
                        }
                        data.Columns.Add(columnName.First);
                    });
                }
                else
                {
                    DataRow dr = data.NewRow();
                    dr.ItemArray = values.Item1.ToArray();
                    data.Rows.Add(dr);
                }
            };
            return data;
        }

        private async Task HandleResponsesReadBasicFileAsync()
        {
            await foreach (var update in _duplexStreamReadBasicFiles.ResponseStream.ReadAllAsync())
            {
                SendEventLoadFile($"name {update.Name} percent {update.Description} status {update.Status}");
            }
        }

        private async Task HandleResponsesReadSensorFileAsync()
        {
            await foreach (var update in _duplexStreamReadSensorFiles.ResponseStream.ReadAllAsync())
            {
                SendEventLoadFile($"name {update.Name} percent {update.Description} status {update.Status}");
            }
        } 

        private static DataTable GetDataTableFromFile(KeyValuePair<string, IBrowserFile> infoFile, string sep)
        {
            List<string> rows = ReadStreamReader(infoFile.Value,0);
            DataTable data = new();
            if (rows.Count > 1)
            {
                rows[0].Split(sep).ToList().ForEach(columnName => data.Columns.Add(columnName));
                rows.Skip(1)?.ToList().ForEach(row => {
                    string[] rowValues = row.Split(sep);
                    DataRow dr = data.NewRow();
                    dr.ItemArray = rowValues;
                    data.Rows.Add(dr);
                });
            }
            return data;
        }

        private static FileUploadRequest ConstructInfoTurbine(int dimension, int count, string name, string type, string sep, JObject rowAndColumn) => new()
        {
            TotalDimension = dimension,
            IsUpload = true,
            Block = count,
            Msg1 = new InfoTurbine
            {
                File = ConstructTransferFile(name, type, sep, rowAndColumn),
                NameTable = "Wind_Turbine_Info"
            }
        };

        private static FileUploadRequest ConstructNameSensor(int dimension, int count, string name, string type, string sep, JObject rowAndColumn) => new()
        {
            TotalDimension = dimension,
            IsUpload = true,
            Block = count,
            Msg2 = new NameSensor
            {
                File = ConstructTransferFile(name, type, sep, rowAndColumn),
                NameTable = "Sensor_Info"
            }
        };

        private static FileUploadRequest ConstructNameErrorSensor(int dimension, int count, string name, string type, string sep, JObject rowAndColumn) => new()
        {
            TotalDimension = dimension,
            IsUpload = true,
            Block = count,
            Msg3 = new NameErrorSensor
            {
                File = ConstructTransferFile(name, type, sep, rowAndColumn),
                NameTable = "Error_Sensor"
            }
        };

        private static FileUploadRequestInfo ConstructTransferFile(string name, string type, string sep, JObject rowAndColumn) => new()
        {
            Metadata = new MetaData { Name = name, Type = type },
            File = new FilePlt { Separator = sep, Content = ByteString.CopyFrom(Encoding.UTF8.GetBytes(rowAndColumn.ToString())) }
        };

        private static FileUploadRequest ConstructErrorCode(int dimension, int count, string name, string type, string sep, JObject rowAndColumn) => new()
        {
            TotalDimension = dimension,
            IsUpload = true,
            Block = count,
            Msg4 = new ErrorCode
            {
                File = ConstructTransferFile(name, type, sep, rowAndColumn),
                NameTable = "Error_Code"
            }
        };

        private static FileUploadRequest ConstructInfoTurbineFinal(string name, string type) => new()
        {
            IsUpload = false,
            Msg1 = new InfoTurbine
            {
                File = new FileUploadRequestInfo
                {
                    Metadata = new MetaData { Name = name, Type = type }
                }
            }
        };

        private static FileUploadRequest ConstructNameSensorFinal(string name, string type) => new()
        {
            IsUpload = false,
            Msg2 = new NameSensor
            {
                File = new FileUploadRequestInfo
                {
                    Metadata = new MetaData { Name = name, Type = type }
                }
            }
        };

        private static FileUploadRequest ConstructNameErrorSensorFinal(string name, string type) => new()
        {
            IsUpload = false,
            Msg3 = new NameErrorSensor
            {
                File = new FileUploadRequestInfo
                {
                    Metadata = new MetaData { Name = name, Type = type }
                }
            }
        };

        private static FileUploadRequest ConstructErrorCodeFinal(string name, string type) => new()
        {
            IsUpload = false,
            Msg4 = new ErrorCode
            {
                File = new FileUploadRequestInfo
                {
                    Metadata = new MetaData { Name = name, Type = type }
                }
            }
        };

        private static ReadInfoSensor ConstructSensor(int dimension, int count, string name, string type, string sep, JObject rowAndColumn) => new()
        {
            TotalDimension = dimension,
            IsUpload = true,
            Block = count,
            Msg1 = new ReadNormalSensor
            {
                Files = ConstructTransferFile(name, type, sep, rowAndColumn)
            }
        };

        private static ReadInfoSensor ConstructSensorFinal(string name, string type) => new()
        {
            IsUpload = false,
            Msg1 = new ReadNormalSensor
            {
                Files = new FileUploadRequestInfo
                {
                    Metadata = new MetaData { Name = name, Type = type }
                }
            }
        };

        private static ReadInfoSensor ConstructEventSensor(int dimension, int count, string name, string type, string sep, JObject rowAndColumn) => new()
        {
            TotalDimension = dimension,
            IsUpload = true,
            Block = count,
            Msg2 = new ReadEventSensor
            {
                Files = ConstructTransferFile(name, type, sep, rowAndColumn)
            }
        };

        private static ReadInfoSensor ConstructEventSensorFinal(string name, string type) => new()
        {
            IsUpload = false,
            Msg2 = new ReadEventSensor
            {
                Files = new FileUploadRequestInfo
                {
                    Metadata = new MetaData { Name = name, Type = type }
                }
            }
        };
    }
}