using Microsoft.EntityFrameworkCore;
using PltWindTurbine.Database.DatabaseConnection;
using PltWindTurbine.Database.DatabaseContract;
using PltWindTurbine.Database.ResultRecordDB;
using PltWindTurbine.Database.TableDatabase;
using PltWindTurbine.Services.MaintenanceService;
using PltWindTurbine.Services.ObtainInfoTurbinesService;
using PltWindTurbine.Subscriber.EventArgument.EventContainer;
using PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Contract;
using PltWindTurbine.Subscriber.EventArgument.LoadInfoTurbine.Implementation;
using PltWindTurbine.Subscriber.SubscriberImplementation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PltWindTurbine.Protos.UtilProto;
using PltWindTurbine.Subscriber.EventArgument.UtilEventTurbine.Implementation;

namespace PltWindTurbine.Database.Utils
{
    public abstract class CommonImplementationDatabase : EventHandlerSystem, IOperationTurbineDatabase
    {
        private static readonly int IdAngleSensor = 1;
        private static readonly int TotalGrade = 360;
        private static IReadOnlyList<TurbineInfo> NameTurbine { get;}
        static CommonImplementationDatabase()
        {
            using var connection = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            NameTurbine = connection.Wind_Turbine_Info.Select(turbine => new TurbineInfo(turbine.Id, turbine.Turbine_Name)).ToList();
        }
        public void InsertInfoPlt(DataTable dt_info, string name_table)
        { 
            using var connection = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            using var transaction = connection.Database.BeginTransaction();
            using var command = connection.Database.GetDbConnection().CreateCommand();
            var columns = dt_info.SelectAllColumn(column => $"${column.ColumnName}").ToArray();
            command.CommandText = $@"INSERT INTO {name_table}({string.Join(",", dt_info.SelectAllColumn(x => x.ColumnName))})  VALUES ({string.Join(",", columns)})";

            command.Parameters.AddRange(columns.Select(column => {
                var parameter = command.CreateParameter();
                parameter.ParameterName = column;
                return parameter;
            }).ToArray());

            dt_info.AsEnumerable().ToList().ForEach(row => {
                row.ItemArray.Zip(columns).ToList().ForEach(valueWithColumn => {
                    command.Parameters[valueWithColumn.Second].Value = valueWithColumn.First;
                });
                command.ExecuteNonQuery();
            });
            transaction.Commit();
        } 
         
        public virtual void InsertInfoWindTurbine(InfoByTurbineToTable infoByTurbine)
        {
            SendEventFile(infoByTurbine.IdTurbine.ToString(),$"Insert sensor info {infoByTurbine.IdSensor}"); 
            using var connection = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            using var transaction = connection.Database.BeginTransaction();
            using var command = connection.Database.GetDbConnection().CreateCommand(); 
            GetDbCommand(command, infoByTurbine, "value_sensor_turbine");
            transaction.Commit(); 
        }

        private static void GetDbCommand(DbCommand command, InfoByTurbineToTable infoByTurbine, string nameTable)
        {
            var columnsWithoutUnion = infoByTurbine.BaseInfoTurbine.Select(keyValue => keyValue.Key);
            var columns = columnsWithoutUnion.Union(
                new List<string>() { infoByTurbine.IdTurbine.nameColumnT, infoByTurbine.IdSensor.nameColumnS }).ToArray();
            var columnsValues = columns.Select(name => $"${name}");
            command.CommandText = $@"INSERT INTO {nameTable}({string.Join(",", columns)})  VALUES ({string.Join(",", columnsValues)})";
            command.Parameters.AddRange(columnsValues.Select(column =>
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = column;
                return parameter;
            }).ToArray());
            (string idTurbine, string idSensor) = GetIdTurbineAndSensor(infoByTurbine);
            Enumerable.Range(0, infoByTurbine.BaseInfoTurbine.Values.First().Count).ToList().ForEach(index =>
            {
                var row = columnsWithoutUnion.Select(key => infoByTurbine.BaseInfoTurbine[key][index]).Append(idTurbine).Append(idSensor);
                row.Zip(columnsValues).ToList().ForEach(valueWithColumn =>
                command.Parameters[valueWithColumn.Second].Value = valueWithColumn.Second is not "$value" ? valueWithColumn.First : (valueWithColumn.First is null ? DBNull.Value : valueWithColumn.First));
                command.ExecuteNonQuery();
            });
        }

        private static (string, string) GetIdTurbineAndSensor(InfoByTurbineToTable infoByTurbine) => (infoByTurbine.IdTurbine.idTurbine.ToString(), infoByTurbine.IdSensor.idSensor.ToString());

        public virtual void InsertInfoEventWindTurbine(InfoByTurbineToTable infoByTurbine)
        {
            SendEventFile(infoByTurbine.IdTurbine.ToString(), $"Insert Event sensor info {infoByTurbine.IdSensor}"); 
            using var connection = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            using var transaction = connection.Database.BeginTransaction();
            using var command = connection.Database.GetDbConnection().CreateCommand();
            GetDbCommand(command, infoByTurbine, "value_sensor_error"); 
            transaction.Commit();
        }
        public List<Wind_Turbine_Info> ReadAllTurbine() 
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            return connectionTo.Wind_Turbine_Info.ToList();
        } 
        public DataTable ReadInfoByTurbine(string path, string nameFile)
        {
            throw new NotImplementedException();
        }

        public List<Sensor_Info> SelectAllNameSensor()
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            return connectionTo.Sensor_Info.ToList();
        }

        private readonly Func<Own_Serie_Turbine, SensorInfo> SelectOwnNameAndIdSensor = sensor => new SensorInfo(sensor.Id, sensor.Name,true);
        private readonly Func<Sensor_Info, SensorInfo> SelectNameAndIdSensor = sensor => new SensorInfo(sensor.Id, sensor.Sensor_Name,false);
        private readonly Func<Wind_Turbine_Info, TurbineInfo> SelectNameAndIdTurbine = turbine => new TurbineInfo(turbine.Id, turbine.Turbine_Name);
        public async void SelectAllSensors()
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            var allSensor = connectionTo.Sensor_Info.Select(SelectNameAndIdSensor).ToList();
            connectionTo.Own_Serie_Turbine.Select(SelectOwnNameAndIdSensor).ToList().ForEach(val=>allSensor.Add(val)); 
            await SendEventLoadInfoTurbine(new AllSensorInfo(allSensor));  
        }
        public async void SelectAllTurbines()
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();  
            await SendEventLoadInfoTurbine(new AllTurbineInfo(connectionTo.Wind_Turbine_Info.Select(SelectNameAndIdTurbine).ToList())); 
        }
        public List<Error_Sensor> SelectAllNameSensorError()
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            return connectionTo.Error_Sensor.ToList();
        }
        
        public Dictionary<string, List<string>> SelectAllSerieBySensorByTurbineByError()
        {
            throw new NotImplementedException();
        }

        public List<Wind_Turbine_Info> SelectAllTurbineInfo()
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            return connectionTo.Wind_Turbine_Info.ToList();
        }

        public List<string> SelectColumnFrom(string nameTable)
        {
            lock (this)
            { 
                return RetreiveImplementationDatabase.Instance.GetConnectionToDatabase().Model.GetRelationalModel().
                Tables.FirstOrDefault(table => table.Name.Equals(nameTable))?.Columns.Select(column => column.Name.ToLower()).ToList();
            }
        }
        public DataTable SelectErrorTableByTurbine()
        {
            throw new NotImplementedException();
        }

        public DataTable SelectErrorTurbineByCondition()
        {
            throw new NotImplementedException();
        }

        public DataTable SelectPivotValueSensorByTurbine()
        {
            throw new NotImplementedException();
        }
        private static IList<(Value_Sensor_Error values, DateTime dates)> GetDateBetweenValues(IList<Value_Sensor_Error> values, int period)
        {
            IList<(Value_Sensor_Error values, DateTime dates)> _GetDateBetweenValues(IList<Value_Sensor_Error> valuesRemaining, IList<(Value_Sensor_Error values, DateTime dates)> valueAndDates) => valuesRemaining switch
            {
                (Value_Sensor_Error head, Value_Sensor_Error head2, IList<Value_Sensor_Error> tail) =>_GetDateBetweenValues(tail.Prepend(head2).ToList(), valueAndDates.Append((head,DateTime.Parse(head2.Date).AddSeconds(5))).ToList()),
                (Value_Sensor_Error head, Value_Sensor_Error head2, _)=> _GetDateBetweenValues(new List<Value_Sensor_Error>() { head2}, valueAndDates.Append((head, DateTime.Parse(head2.Date).AddSeconds(5))).ToList()),
                (Value_Sensor_Error head, _) => valueAndDates.Append((head, DateTime.Parse(head.Date).AddDays(period))).ToList()
            };

            return _GetDateBetweenValues(values, new List<(Value_Sensor_Error, DateTime)>());
        }
        private async Task<ResponseSerieByPeriodWithWarning> GetWarning(ConnectionToDatabase connectionTo, OnlySerieByPeriodAndCode info, Value_Sensor_Error infoError, DateTime dates, List<SerieBySensorTurbineError> resultSerie, Value_Sensor_Error last)
        {
            var warning = await connectionTo.Value_Sensor_Error.Where(error => error.Value.HasValue && error.Id_Turbine == info.IdTurbine &&
                   string.Compare(error.Date, infoError.Date) < 0 && string.Compare(error.Date, dates.ToString("yyyy/MM/dd HH:mm:ss")) > 0)
                        .Select(values => new SerieBySensorTurbineWarning(values.Id, values.Date, values.Value)).ToListAsync();
            warning = warning.OrderBy(val => val.Date).ToList();
            warning = DateTimeRange(dates, ParserDateSpecificFormat(infoError.Date), 10, warning).ToList();
            var serieByPeriod = new ResponseSerieByPeriod(info.NameTurbine, info.NameSensor, JsonSerializer.Serialize(resultSerie), infoError.Id == last.Id);
            return new ResponseSerieByPeriodWithWarning(serieByPeriod, JsonSerializer.Serialize(warning), JsonSerializer.Serialize(warnings));
        }
        private async IAsyncEnumerable<ILoadInfoTurbine> GenerateSequence(OnlySerieByPeriodAndCode info, bool isWarning=false)
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            (ILoadInfoTurbine status, IList<Value_Sensor_Error> errors, Value_Sensor_Error last) values = await GetValuesAndLast(info);
            yield return values.status;
            var newValuesWithDate = callFunction(values.errors, info);
            foreach (var (infoError, dates) in newValuesWithDate)
            {
                var resultSerie = await connectionTo.Value_Sensor_Turbine.Where(infoSensor => infoSensor.Id_Turbine == info.IdTurbine && infoSensor.Id_Sensor == info.IdSensor &&
                   string.Compare(infoSensor.Date, infoError.Date) < 0 && string.Compare(infoSensor.Date, dates.ToString("yyyy/MM/dd HH:mm:ss")) > 0)
                    .Select(values => new SerieBySensorTurbineError(values.Id, values.Date, values.Value)).ToListAsync();

                yield return isWarning ? await GetWarning(connectionTo, info, infoError, dates, resultSerie, values.last) :
                    callFunctionResponseSerie(info, resultSerie, infoError.Id == values.last.Id);
            }

        }
        private static async Task<(ILoadInfoTurbine status,IList<Value_Sensor_Error> errors, Value_Sensor_Error last)> GetValuesAndLast(OnlySerieByPeriodAndCode info)
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            var values = await connectionTo.Value_Sensor_Error.Where(error => error.Id_Turbine == info.IdTurbine && error.Value == info.Code).OrderByDescending(value => value.Date).ToListAsync();
            var last = values.LastOrDefault();
            return (new StatusEventInfoTurbine(new StatusEvent(info.NameTurbine, Status.InProgress, $"Init Search Temporary Data with {values.Count} precence of error {info.Code}")),values,last);
        }
        private delegate IList<(Value_Sensor_Error value, DateTime date)> GetNewValuewWithDate(IList<Value_Sensor_Error> errors, OnlySerieByPeriodAndCode info);

        private delegate ResponseSerieByPeriod GetResponseSerieByPeriod(OnlySerieByPeriodAndCode info, IList<SerieBySensorTurbineError> resultSerie, bool isFinal);

        private readonly GetResponseSerieByPeriod callFunctionResponseSerie = (info, resultSerie, isFinal) =>
        new ResponseSerieByPeriod(info.NameTurbine, info.NameSensor, JsonSerializer.Serialize(resultSerie), isFinal);

        private readonly GetNewValuewWithDate callFunction= (errors, info)=>
            errors.Count > 1 ? GetDateBetweenValues(errors, info.Days) : errors.Select(val => (val, DateTime.Parse(errors.First().Date).AddDays(info.Days))).ToList();

        private delegate IAsyncEnumerable<ILoadInfoTurbine> Sequence(); 

        private async IAsyncEnumerable<ILoadInfoTurbine> GenerateOwnSequence(OnlySerieByPeriodAndCode info, bool isWarning = false)
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            (ILoadInfoTurbine status, IList<Value_Sensor_Error> errors, Value_Sensor_Error last) values = await GetValuesAndLast(info);
            yield return values.status;
            var newValuesWithDate = callFunction(values.errors, info); 
            foreach (var (infoError, dates) in newValuesWithDate)
            {
                var resultSerie = await connectionTo.Value_Own_Serie_Turbine.Where(infoSensor => infoSensor.Id_Turbine == info.IdTurbine && infoSensor.Id_Own_Serie == info.IdSensor &&
                   string.Compare(infoSensor.Date, infoError.Date) < 0 && string.Compare(infoSensor.Date, dates.ToString("yyyy/MM/dd HH:mm:ss")) > 0)
                    .Select(values => new SerieBySensorTurbineError(values.Id, values.Date, values.Value)).ToListAsync();

                yield return isWarning ? await GetWarning(connectionTo, info, infoError, dates, resultSerie, values.last) :
                    callFunctionResponseSerie(info, resultSerie, infoError.Id == values.last.Id);
            }

        }

        private static string ValidationFormatData(string date) => DateTime.Parse(date).ToString("yyyy/MM/dd HH:mm:ss");

        private static DateTime ParserDateSpecificFormat(string date) => DateTime.Parse(DateTime.Parse(date).ToString("yyyy/MM/dd HH:mm:ss"));

        private static IEnumerable<SerieBySensorTurbineWarning> DateTimeRange(DateTime start, DateTime end, int delta, List<SerieBySensorTurbineWarning> serieBySensors)
        {
            var current = start.Minute%10==0?start: start.AddMinutes(-(start.Minute % 10));
            current =  current.AddSeconds(-(current.Second));
            var id = 0;
            while (current < end)
            {

                if (serieBySensors.Count > 0 && Math.Abs((current - ParserDateSpecificFormat(ValidationFormatData(serieBySensors.First().Date))).TotalSeconds / 60) <= 5)
                {
                    var result = serieBySensors.First();
                    serieBySensors = serieBySensors.Skip(1).ToList();
                    yield return result;
                }
                else
                {
                    var currentAux = current;
                    current = currentAux.AddMinutes(delta);
                    yield return new SerieBySensorTurbineWarning(id++,currentAux.ToString("yyyy/MM/dd HH:mm:ss"), double.Parse("0"));

                }
            }

        } 

        private async Task CallSelectSeries(OnlySerieByPeriodAndCode info, bool isWarning = false) {
            IAsyncEnumerable<ILoadInfoTurbine> sequence() => GenerateSequence(info, isWarning);
            await CallSelectSeriesFinal(sequence);
        }
        
        private async Task CallSelectSeriesFinal(Sequence generateSequence)
        { 
            await foreach (var values in generateSequence())
            {
                SendEventLoadInfo(values);
            }
        }

        private async Task CallSelectOwnSeries(OnlySerieByPeriodAndCode info, bool isWarning = false)
        {
            await CalculateAngleSerieAllTurbines();
            IAsyncEnumerable<ILoadInfoTurbine> sequence() => GenerateOwnSequence(info, isWarning);
            await CallSelectSeriesFinal(sequence);
        }

        public async void SelectSerieBySensorByTurbineByError(OnlySerieByPeriodAndCode info) => await CallSelectSeries(info); 

        public async void SelectSerieBySensorByTurbineByErrorWithWarning(OnlySerieByPeriodAndCode info) => await CallSelectSeries(info, true);

        public async void SelectOwnSerieBySensorByTurbineByError(OnlySerieByPeriodAndCode info) => await CallSelectOwnSeries(info); 

        public async void SelectOwnSerieBySensorByTurbineByErrorWithWarning(OnlySerieByPeriodAndCode info) => await CallSelectOwnSeries(info, true);

        private readonly IReadOnlyList<double> errors = new List<double> { 180, 3370, 186, 182, 181 };
        private readonly IReadOnlyList<double> warnings = new List<double> { 892, 891, 183, 79, 356 };

        public Task<List<string>> GetErrorByTurbine(int idTurbine) => Task.Run(() =>
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            return connectionTo.Value_Sensor_Error.Where(turbine=>turbine.Id_Turbine==idTurbine && turbine.Value.HasValue &&
            errors.Contains(turbine.Value.Value))
            .GroupBy(value=>value.Value)
            .Select(value =>value.Key.ToString()).ToList(); 
            
        });
        
        public Dictionary<string, List<string>> SelectSerieTurbineByError()
        {
            throw new NotImplementedException();
        }

        public DataTable SelectValueSensorByTurbine()
        {
            throw new NotImplementedException();
        }

        public (List<string>, bool) TurbineExistInDatabase()
        {
            throw new NotImplementedException();
        }
        public Task<List<(int, string)>> GetNameChart()=>Task.Run(() =>
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            return connectionTo.Chart_System.ToList().Select(chart=>(chart.Id,chart.Chart_Name)).ToList(); 
        });
        private AngleSerieTurbine CalculateAngleserie((Value_Sensor_Turbine first, Value_Sensor_Turbine second) series)
        {
            return series.first.Value.HasValue && series.second.Value.HasValue ? (Math.Abs(series.first.Value.Value - series.second.Value.Value) > 180? 
                new AngleSerieTurbine(IdAngleSensor, series.first.Id_Turbine, series.first.Date, TotalGrade- Math.Abs(series.first.Value.Value - series.second.Value.Value)) :
                new AngleSerieTurbine(IdAngleSensor, series.first.Id_Turbine, series.first.Date, Math.Abs(series.first.Value.Value - series.second.Value.Value))) :
                  new AngleSerieTurbine(IdAngleSensor, series.first.Id_Turbine, series.first.Date, double.NaN);
        }
        private List<AngleSerieTurbine> SelectAndCalculateAngleSerie(int idTurbine)
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            var (first, second) = connectionTo.Value_Sensor_Turbine.Where(value => (value.Id_Sensor == 2 || value.Id_Sensor == 4) && value.Id_Turbine == idTurbine).ToList()
                .Partition(values=>values.Id_Sensor==2);
            return first.OrderBy(x => x.Date).Zip(second.OrderBy(x => x.Date)).Select(CalculateAngleserie).ToList();
        }
        private Task CalculateAngleSerieAllTurbines() => Task.Run(async () =>
        { 
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase(); 
            if (connectionTo.Own_Serie_Turbine.Where(idTurbine=>idTurbine.Id==IdAngleSensor && !idTurbine.Is_Ok).Any())
            {
                using var transaction = connectionTo.Database.BeginTransaction();
               
                var tasks = connectionTo.Wind_Turbine_Info.Select(turbine => turbine.Id).ToList().Select(idTurbine =>
                    Task.Run(() => SelectAndCalculateAngleSerie(idTurbine))
                ).ToArray();
                await Task.WhenAll(tasks).ContinueWith(result =>result.Result.ToList().ForEach(resultToInsert=> {
                    GetDbCommandOwnSeries(connectionTo, resultToInsert, "value_own_serie_turbine");
                }), TaskContinuationOptions.OnlyOnRanToCompletion);
                var sensor = connectionTo.Own_Serie_Turbine.Where(idTurbine => idTurbine.Id == IdAngleSensor).First();
                sensor.Is_Ok = true;
                connectionTo.SaveChanges();
                transaction.Commit();
            } 
        });

        private static void GetDbCommandOwnSeries(ConnectionToDatabase connectionTo, List<AngleSerieTurbine> angleSerieTurbine, string nameTable)
        {
            using var command = connectionTo.Database.GetDbConnection().CreateCommand();
            var columns = new List<string>() { "id_turbine","id_own_serie","value","date" };
            var columnsValues = columns.Select(name => $"${name}");
            command.CommandText = $@"INSERT INTO {nameTable}({string.Join(",", columns)})  VALUES ({string.Join(",", columnsValues)})";
            command.Parameters.AddRange(columnsValues.Select(column =>
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = column;
                return parameter;
            }).ToArray());

            angleSerieTurbine.ForEach(values =>
            {
                var row = new List<string> { values.IdTurbine.ToString(), values.IdSensor.ToString(), values.Value.ToString(), values.Date };
                row.Zip(columnsValues).ToList().ForEach(valueWithColumn =>
                command.Parameters[valueWithColumn.Second].Value = valueWithColumn.Second is not "$value" ? valueWithColumn.First : (valueWithColumn.First is null ? DBNull.Value : valueWithColumn.First));
                command.ExecuteNonQuery();
            });
             
        } 

        public virtual async Task SelectWarningAllTurbines(int period)
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            var allTurbines = await connectionTo.Wind_Turbine_Info.Select(turbine=> new TurbineInfo(turbine.Id,turbine.Turbine_Name)).ToListAsync();
            await foreach(var infoTurbine in SelectWarningAllTurbineByPeriod(allTurbines))
            {

            }
        }

        private IAsyncEnumerable<object> SelectWarningAllTurbineByPeriod(List<TurbineInfo> allTurbines)
        {
            throw new NotImplementedException();
        }

        public Task SaveMaintenanceTurbines(SaveTurbineInfoMaintenance infoMaintenance, bool isFinish) => Task.Run(() =>
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            var query = connectionTo.Maintenance_Turbine.Where(info => info.Id_Turbine == infoMaintenance.IdTurbine && info.Date == infoMaintenance.Date);
            if (!query.Any())
            {
                var maintenance = new Maintenance_Turbine() { Id_Turbine = infoMaintenance.IdTurbine, Date = infoMaintenance.Date };
                connectionTo.Maintenance_Turbine.Add(maintenance);
                connectionTo.SaveChanges();
                if (isFinish)
                {
                    SendEventFinishLoadMaintenanceInfo("", "Finish Save Maintenance");
                }
                else
                { 
                    SendEventLoadMaintenanceInfo(NameTurbine.FirstOrDefault(x => x.IdTurbine == infoMaintenance.IdTurbine)?.NameTurbine, "Save Turbine");
                }
            }
        });
         
    }
}
