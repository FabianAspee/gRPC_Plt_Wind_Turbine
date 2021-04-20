using Google.Protobuf;
using Grpc.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PltWindTurbine.Services.Loadfilesservices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFillesAsDatatable.Controller.LoadFileController
{
    public class LoadFileController : BaseController, IAsyncDisposable, ILoadFileController
    {
        private readonly LoadFiles.LoadFilesClient _clientReadBasicFiles;
        private readonly AsyncDuplexStreamingCall<FileUploadRequest, FileUploadResponse> _duplexStreamReadBasicFiles;
        private readonly AsyncDuplexStreamingCall<ReadInfoSensor, FileUploadResponse> _duplexStreamReadSensorFiles;

        public LoadFileController()
        {
            _clientReadBasicFiles = new LoadFiles.LoadFilesClient(channel);
            _duplexStreamReadBasicFiles = _clientReadBasicFiles.LoadFilesInfoTurbine();
            _duplexStreamReadSensorFiles = _clientReadBasicFiles.ReadSensor();
            _ = HandleResponsesReadBasicFileAsync();
            _ = HandleResponsesReadSensorFileAsync();
        }

       
        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
        private FileUploadRequest ConstructInfoTurbine(int dimension, int count, string name, string type, string sep, JObject rowAndColumn)
        {
            return new()
            {
                TotalDimension = dimension,
                IsUpload = true,
                Block = count,
                Msg1 = new InfoTurbine
                {
                    File = new FileUploadRequestInfo
                    {
                        Metadata = new MetaData { Name = name, Type = type },
                        File = new File { Separator = sep, Content = ByteString.CopyFrom(Encoding.UTF8.GetBytes(rowAndColumn.ToString())) }
                    },
                    NameTable = "Wind_Turbine_Info"
                }
            };
        }
        private FileUploadRequest ConstructNameSensor(int dimension, int count, string name, string type, string sep, JObject rowAndColumn)
        {
            return new()
            {
                TotalDimension = dimension,
                IsUpload = true,
                Block = count,
                Msg2 = new NameSensor
                {
                    File = new FileUploadRequestInfo
                    {
                        Metadata = new MetaData { Name = name, Type = type },
                        File = new File { Separator = sep, Content = ByteString.CopyFrom(Encoding.UTF8.GetBytes(rowAndColumn.ToString())) }
                    },
                    NameTable = "Sensor_Info"
                }
            };
        }
        private FileUploadRequest ConstructNameErrorSensor(int dimension, int count, string name, string type, string sep, JObject rowAndColumn)
        {
            return new()
            {
                TotalDimension = dimension,
                IsUpload = true,
                Block = count,
                Msg3 = new NameErrorSensor
                {
                    File = new FileUploadRequestInfo
                    {
                        Metadata = new MetaData { Name = name, Type = type },
                        File = new File { Separator = sep, Content = ByteString.CopyFrom(Encoding.UTF8.GetBytes(rowAndColumn.ToString())) }
                    },
                    NameTable = "Error_Sensor"
                }
            };
        }
        private FileUploadRequest ConstructErrorCode(int dimension, int count, string name, string type, string sep, JObject rowAndColumn)
        {
            return new()
            {
                TotalDimension = dimension,
                IsUpload = true,
                Block = count,
                Msg4 = new ErrorCode
                {
                    File = new FileUploadRequestInfo
                    {
                        Metadata = new MetaData { Name = name, Type = type },
                        File = new File { Separator = sep, Content = ByteString.CopyFrom(Encoding.UTF8.GetBytes(rowAndColumn.ToString())) }
                    },
                    NameTable = "Error_Code"
                }
            };
        }

        private FileUploadRequest ConstructInfoTurbineFinal(string name, string type)
        {
            return new()
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
        }
        private FileUploadRequest ConstructNameSensorFinal(string name, string type)
        {
            return new()
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
        }
        private FileUploadRequest ConstructNameErrorSensorFinal(string name, string type)
        {
            return new()
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
        }
        private FileUploadRequest ConstructErrorCodeFinal(string name, string type)
        {
            return new()
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
        }
        public Task ReadBasicFiles(DataTable file, string name, string type, string sep, int id)
        {
            return Task.Run(async () =>
            {
                Console.WriteLine(name);
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
                Console.WriteLine("ESTOY AQUI!");
                FileUploadRequest fileUpload2 = id == 1 ? ConstructInfoTurbineFinal(name, type) : (id == 2 ? ConstructNameSensorFinal(name, type) :
                    (id == 3 ? ConstructNameErrorSensorFinal(name, type) : (id == 4 ? ConstructErrorCodeFinal(name, type) : throw new Exception())));
                await _duplexStreamReadBasicFiles.RequestStream.WriteAsync(fileUpload2);
            });
        }
        private ReadInfoSensor ConstructSensor(int dimension, int count, string name, string type, string sep, JObject rowAndColumn)
        {
            return new()
            {
                TotalDimension = dimension,
                IsUpload = true,
                Block = count,
                Msg1 = new ReadNormalSensor
                { 
                    Files = new FileUploadRequestInfo
                    {
                        Metadata = new MetaData { Name = name, Type = type },
                        File = new File { Separator = sep, Content = ByteString.CopyFrom(Encoding.UTF8.GetBytes(rowAndColumn.ToString())) }
                    }
                }
            };
        }
        private ReadInfoSensor ConstructSensorFinal(string name, string type)
        {
            return new()
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
        }
        private ReadInfoSensor ConstructEventSensor(int dimension, int count, string name, string type, string sep, JObject rowAndColumn)
        {
            return new()
            {
                TotalDimension = dimension,
                IsUpload = true,
                Block = count,
                Msg2 = new ReadEventSensor
                {
                    Files = new FileUploadRequestInfo
                    {
                        Metadata = new MetaData { Name = name, Type = type },
                        File = new File { Separator = sep, Content = ByteString.CopyFrom(Encoding.UTF8.GetBytes(rowAndColumn.ToString())) }
                    }
                }
            };
        }
        private ReadInfoSensor ConstructEventSensorFinal(string name, string type)
        {
            return new()
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
        public Task ReadSensorTurbine(DataTable file, string name, string type, string sep, bool isEvent)
        {
            return Task.Run(async () =>
            {
                Console.WriteLine(name);
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
                    ReadInfoSensor fileUpload = !isEvent ? ConstructSensor(dimension, count, name, type, sep, rowAndColumn) : ConstructEventSensor(dimension, count, name, type, sep, rowAndColumn);
                    _duplexStreamReadSensorFiles.RequestStream.WriteAsync(fileUpload).Wait();
                    count++;
                });
                Console.WriteLine("ESTOY AQUI!");
                ReadInfoSensor fileUpload2 = !isEvent? ConstructSensorFinal(name, type) :  ConstructEventSensorFinal(name, type) ;
                await _duplexStreamReadSensorFiles.RequestStream.WriteAsync(fileUpload2);
            });
        }
        private async Task HandleResponsesReadBasicFileAsync()
        {
            await foreach (var update in _duplexStreamReadBasicFiles.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"name {update.Name} percent {update.Description} status {update.Status}");
            }
        }
        private async Task HandleResponsesReadSensorFileAsync()
        {
            await foreach (var update in _duplexStreamReadSensorFiles.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"name {update.Name} percent {update.Description} status {update.Status}");
            }
        }

    }
}

