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
using PltTurbineShared.ExtensionMethodList;
using PltTurbineShared.ExtensionMethodDataTable;

namespace PltWindTurbine.Database.Utils
{
    public abstract class CommonImplementationDatabase : EventHandlerSystem, IOperationTurbineDatabase
    {
        private static readonly int IdAngleSensor = 1;
        private static readonly int TotalGrade = 360;
        private readonly IReadOnlyList<double> errors = new List<double> { 180, 3370, 186, 182, 181 };
        private readonly IReadOnlyList<double> warnings = new List<double> { 892, 891, 183, 79, 356 };
        private static IReadOnlyList<TurbineInfo> NameTurbine { get;}
        static CommonImplementationDatabase()
        {
            using var connection = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            NameTurbine = connection.Wind_Turbine_Info.Select(turbine => new TurbineInfo(turbine.Id, turbine.Turbine_Name)).ToList();
        }
        public void CalculateFourierInAngleSerie(int idTurbine, int periodInDays)
        {
            using var connection = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();

        }
        private static Dictionary<int, List<Maintenance_Turbine>> JoinTableWindTurbineMaintenance(ConnectionToDatabase connection) =>
            connection.Wind_Turbine_Info.Select(turbine => turbine.Id)
                .Join(connection.Maintenance_Turbine,
                turbine => turbine,
                maintenance => maintenance.Id,
                (turbine, maintenance) => new { IdTurbine = turbine, MaintenanceTurbine = maintenance })
                .Where(turbineAndMaintenance => turbineAndMaintenance.IdTurbine == turbineAndMaintenance.MaintenanceTurbine.Id_Turbine)
                .GroupBy(key => key.IdTurbine, value => value.MaintenanceTurbine).ToDictionary(key => key.Key, value => value.ToList());
        private static Dictionary<int, List<Value_Sensor_Error>> JoinTableWindTurbineValueError(ConnectionToDatabase connection) =>
            connection.Wind_Turbine_Info.Select(info => info.Id)
                .Join(connection.Value_Sensor_Error,
                turbine => turbine,
                error => error.Id_Turbine,
                (turbine, error) => new { IdTurbine = turbine, ErrorTurbineInfo = error })
                .Where(turbineAndError => turbineAndError.IdTurbine == turbineAndError.ErrorTurbineInfo.Id_Turbine)
                .GroupBy(key => key.IdTurbine, value => value.ErrorTurbineInfo).ToDictionary(key => key.Key, value => value.ToList());

        
        public async Task CalculateCorrelationAllSeriesAllTurbines(int periodDays)
        { 
            using var connection = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            var allMantenaince = JoinTableWindTurbineMaintenance(connection).Select(values=> (values.Key,GetDateBetweenValues(values.Value))).ToDictionary(key => key.Key, value => value.Item2.ToList());
            var allIdTurbine = JoinTableWindTurbineValueError(connection).Join(allMantenaince,
                valueMySensor=>valueMySensor.Key,
                allMantenaince=>allMantenaince.Key,
                (sensor,maintenance)=>new {IdTurbine= sensor.Key,MaintenanceAndValueError=(maintenance.Value,sensor.Value)})
                .ToList()
                .Select(value=>(value.IdTurbine,value.MaintenanceAndValueError.Item1,value.MaintenanceAndValueError.Item2)).ToList();


            //allIdTurbine.Select(infoTurbine=>)


            var allSensor = connection.Sensor_Info.Select(info => info.Id).ToList();
            var allOwnSensor = connection.Own_Serie_Turbine.Select(info => info.Id).ToList(); 
            
            var permutation = allIdTurbine.SelectMany(turbine => allSensor.SelectMany(sensor => allOwnSensor.Select(ownSensor => (turbine, sensor, ownSensor)))).ToList();
            permutation.ForEach(async value =>
            {
            }); 

        }
        private double ComputeCoeff(double[] values1, double[] values2)
        {
            if (values1.Length != values2.Length)
                throw new ArgumentException("values must be the same length");

            var avg1 = values1.Average();
            var avg2 = values2.Average();

            var sum1 = values1.Zip(values2, (x1, y1) => (x1 - avg1) * (y1 - avg2)).Sum();

            var sumSqr1 = values1.Sum(x => Math.Pow((x - avg1), 2.0));
            var sumSqr2 = values2.Sum(y => Math.Pow((y - avg2), 2.0));

            var result = sum1 / Math.Sqrt(sumSqr1 * sumSqr2);

            return result;
        }
        private static IList<(DateTime datesInit, DateTime datesFinish)> GetDateBetweenValues(IList<Maintenance_Turbine> values, DateTime datesInit= default)
        {
            IList<(DateTime datesInit, DateTime datesFinish)> _GetDateBetweenValues(IList<Maintenance_Turbine> maintenance_Turbines, IList<(DateTime datesInit, DateTime datesFinish)> valueAndDates) => maintenance_Turbines switch
            {
                (Maintenance_Turbine head, Maintenance_Turbine head2, IList<Maintenance_Turbine> tail) => _GetDateBetweenValues(tail.Prepend(head2).ToList(), valueAndDates.Append((DateTime.Parse(head.Date_Finish), DateTime.Parse(head2.Date))).ToList()),
                (Maintenance_Turbine head, Maintenance_Turbine head2, _) => _GetDateBetweenValues(new List<Maintenance_Turbine>() { head2 }, valueAndDates.Append((DateTime.Parse(head.Date_Finish), DateTime.Parse(head2.Date))).ToList()),
                (Maintenance_Turbine head, _) => valueAndDates.Append((DateTime.Parse(head.Date_Finish), DateTime.Now)).ToList()
            };

            return _GetDateBetweenValues(values, new List<(DateTime, DateTime)>() { (datesInit, DateTime.Parse(values.First().Date)) });
        }
        private static IList<(DateTime init, DateTime finish)> GetIntervalTime(ConnectionToDatabase connection,int idTurbine) => 
            GetDateBetweenValues(connection.Maintenance_Turbine.Where(info => info.Id_Turbine == idTurbine).ToList(), DateTime.Parse(connection.Value_Sensor_Turbine.Where(turbine=>turbine.Id_Turbine==idTurbine)
                .Min(turbine=>turbine.Date).ToString()));
        public async Task ObtainsAllWarningAndErrorInPeriodMaintenance(int idTurbine, string nameTurbine)
        {
            using var connection = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase(); 
            var intervalTime = GetIntervalTime(connection,idTurbine); 
            IAsyncEnumerable<ILoadInfoTurbine> sequence() => ObtainsAllWarningAndErrorInPeriodMaintenance(idTurbine, nameTurbine, connection, intervalTime);
            async void eventDelegate(ILoadInfoTurbine value) => await SendEventLoadInfoMaintenance(value);
            await CallSelectSeriesFinal(sequence, eventDelegate); 
        }
        private async IAsyncEnumerable<ILoadInfoTurbine> ObtainsAllWarningAndErrorInPeriodMaintenance(int idTurbine, string nameTurbine, ConnectionToDatabase connectionTo, IList<(DateTime init, DateTime finish)> dates)
        {
            var final = dates.Count;
            foreach (var ((init, finish),index) in dates.Zip(Enumerable.Range(1, final)))
            {
                var warning = await connectionTo.Value_Sensor_Error.Where(error => error.Value.HasValue && error.Id_Turbine == idTurbine &&
                    string.Compare(error.Date, finish.ToString("yyyy/MM/dd HH:mm:ss")) < 0 && string.Compare(error.Date, init.ToString("yyyy/MM/dd HH:mm:ss")) > 0)
                         .Select(values => new SerieBySensorTurbineWarningAndError(values.Id, values.Date, values.Value, errors.Any(error=> error==values.Value.Value))).ToListAsync();
                warning = warning.OrderBy(val => val.Date).ToList();
                yield return new WarningAndErrorTurbine(new ValuesByTurbine(nameTurbine, JsonSerializer.Serialize(warning), index== final), JsonSerializer.Serialize(warnings));  
            }  
                
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
         
        public virtual async Task InsertInfoWindTurbine(InfoByTurbineToTable infoByTurbine)
        {
            await SendEventFile(infoByTurbine.IdTurbine.ToString(),  $"Insert sensor info {infoByTurbine.IdSensor}"); 
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

        public virtual async Task InsertInfoEventWindTurbine(InfoByTurbineToTable infoByTurbine)
        {  
            await SendEventFile(infoByTurbine.IdTurbine.ToString(), $"Insert Event sensor info {infoByTurbine.IdSensor}"); 
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
        public async Task SelectAllSensors()
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            var allSensor = connectionTo.Sensor_Info.Select(SelectNameAndIdSensor).ToList();
            connectionTo.Own_Serie_Turbine.Select(SelectOwnNameAndIdSensor).ToList().ForEach(val=>allSensor.Add(val)); 
            await SendEventLoadInfoTurbine(new AllSensorInfo(allSensor));  
        }
        public async Task SelectAllTurbines()
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

            var serieByPeriod = new ResponseSerieByPeriod(new ValuesByTurbine(info.NameTurbine, JsonSerializer.Serialize(resultSerie), infoError.Id == last.Id), info.NameSensor);
            return new ResponseSerieByPeriodWithWarning(serieByPeriod, new ResponseSerieOnlyWarning(JsonSerializer.Serialize(warning), JsonSerializer.Serialize(warnings)));
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
        new ResponseSerieByPeriod(new ValuesByTurbine(info.NameTurbine, JsonSerializer.Serialize(resultSerie), isFinal), info.NameSensor);

        private readonly GetNewValuewWithDate callFunction= (errors, info)=>
            errors.Count > 1 ? GetDateBetweenValues(errors, info.Days) : errors.Select(val => (val, DateTime.Parse(errors.First().Date).AddDays(info.Days))).ToList();

        private delegate IAsyncEnumerable<ILoadInfoTurbine> Sequence();


        private delegate void EventDelegate(ILoadInfoTurbine infoTurbine); 
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

        private static DateTime ParserDateSpecificFormat(string date) => DateTime.Parse(ValidationFormatData(date));

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
            async void eventDelegate(ILoadInfoTurbine value) => await SendEventLoadInfo(value);
            await CallSelectSeriesFinal(sequence, eventDelegate);
        }
        private static async Task CallSelectSeriesFinal(Sequence generateSequence, EventDelegate eventDelegate)
        { 
            await foreach (var values in generateSequence())
            {
                eventDelegate(values);
            }
        }

        private async Task CallSelectOwnSeries(OnlySerieByPeriodAndCode info, bool isWarning = false)
        {
            await CalculateAngleSerieAllTurbines();
            IAsyncEnumerable<ILoadInfoTurbine> sequence() => GenerateOwnSequence(info, isWarning);
            async void eventDelegate(ILoadInfoTurbine value) => await SendEventLoadInfo(value);
            await CallSelectSeriesFinal(sequence, eventDelegate);
        }

        public async Task SelectSerieBySensorByTurbineByError(OnlySerieByPeriodAndCode info) => await CallSelectSeries(info); 

        public async Task SelectSerieBySensorByTurbineByErrorWithWarning(OnlySerieByPeriodAndCode info) => await CallSelectSeries(info, true);

        public async Task SelectOwnSerieBySensorByTurbineByError(OnlySerieByPeriodAndCode info) => await CallSelectOwnSeries(info); 

        public async Task SelectOwnSerieBySensorByTurbineByErrorWithWarning(OnlySerieByPeriodAndCode info) => await CallSelectOwnSeries(info, true);


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

        public Task SaveMaintenanceTurbines(SaveTurbineInfoMaintenance infoMaintenance, bool isFinish) => Task.Run(async () =>
        {
            using var connectionTo = RetreiveImplementationDatabase.Instance.GetConnectionToDatabase();
            infoMaintenance.Date = ParseData(infoMaintenance.Date);
            infoMaintenance.Datef = ParseData(infoMaintenance.Datef);
            var query = connectionTo.Maintenance_Turbine.Where(info => info.Id_Turbine == infoMaintenance.IdTurbine && info.Date == infoMaintenance.Date && info.Date_Finish == infoMaintenance.Datef
            && info.Is_Normal_Maintenance==infoMaintenance.IsNormalMaintenance);
            if (!query.Any())
            {
                var maintenance = new Maintenance_Turbine() { Id_Turbine = infoMaintenance.IdTurbine, Date = infoMaintenance.Date, 
                    Date_Finish = infoMaintenance.Datef, Is_Normal_Maintenance = infoMaintenance.IsNormalMaintenance };
                connectionTo.Maintenance_Turbine.Add(maintenance);
                connectionTo.SaveChanges();
                if (isFinish)
                {
                    await SendEventFinishLoadMaintenanceInfo("", "Finish Save Maintenance");
                }
                else
                { 
                    await SendEventLoadMaintenanceInfo(NameTurbine.FirstOrDefault(x => x.IdTurbine == infoMaintenance.IdTurbine)?.NameTurbine, "Save Turbine");
                }
            }
            else
            {
                await SendEventFinishLoadMaintenanceInfo ("", "Event maintenance already exist");
            }
        });
        private static string ParseData(string data) => ValidationFormatData(data);
    }
}
