﻿using PltWindTurbine.Database.ResultRecordDB;
using PltWindTurbine.Database.TableDatabase;
using PltWindTurbine.Services.MaintenanceService;
using PltWindTurbine.Services.ObtainInfoTurbinesService;
using PltWindTurbine.Subscriber.SubscriberImplementation;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PltWindTurbine.Database.DatabaseContract
{ 
    interface IOperationTurbineDatabase
    { 
        public abstract Dictionary<string, List<string>> SelectSerieTurbineByError();
        public abstract Task SelectSerieBySensorByTurbineByError(OnlySerieByPeriodAndCode info);
        public abstract Dictionary<string, List<string>> SelectAllSerieBySensorByTurbineByError();
        public abstract Task SelectSerieBySensorByTurbineByErrorWithWarning(OnlySerieByPeriodAndCode info);
        public abstract List<Wind_Turbine_Info> ReadAllTurbine();
        public abstract Task<List<string>> GetErrorByTurbine(int idTurbine);
        public abstract Task<List<(int,string)>> GetNameChart();
        public abstract Task SelectAllSensors();
        public abstract Task SelectAllTurbines();
        public (List<string>,bool) TurbineExistInDatabase();

        public void InsertInfoPlt(DataTable dt_info, string name_table);
        public DataTable ReadInfoByTurbine(string path, string nameFile);

        public Task InsertInfoWindTurbine(InfoByTurbineToTable infoTurbine);
        public Task InsertInfoEventWindTurbine(InfoByTurbineToTable infoTurbine);
        public List<string> SelectColumnFrom(string nameTable);
        public List<Wind_Turbine_Info> SelectAllTurbineInfo();
        public List<Sensor_Info> SelectAllNameSensor();
        public List<Error_Sensor> SelectAllNameSensorError();
        public DataTable SelectValueSensorByTurbine();
        public DataTable SelectPivotValueSensorByTurbine();
        public DataTable SelectErrorTableByTurbine();
        public DataTable SelectErrorTurbineByCondition();
        Task SelectOwnSerieBySensorByTurbineByError(OnlySerieByPeriodAndCode info);
        Task SelectOwnSerieBySensorByTurbineByErrorWithWarning(OnlySerieByPeriodAndCode info);

        public Task SelectWarningAllTurbines(int period);
        public Task SaveMaintenanceTurbines(SaveTurbineInfoMaintenance infoMaintenance, bool isFinish);
        void CalculateFourierInAngleSerie(int idTurbine, int periodInDays);
        Task ObtainsAllWarningAndErrorInPeriodMaintenance(int idTurbine, string nameTurbine);
        Task CalculateCorrelationAllSeriesAllTurbines(int periodDays);
    }
}
