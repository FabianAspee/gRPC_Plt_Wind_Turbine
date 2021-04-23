using PltWindTurbine.Database.ResultRecordDB;
using PltWindTurbine.Database.TableDatabase;
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
        public abstract void SelectSerieBySensorByTurbineByError(OnlySerieByPeriodAndCode info);
        public abstract Dictionary<string, List<string>> SelectAllSerieBySensorByTurbineByError();
        public abstract List<Wind_Turbine_Info> ReadAllTurbine();
        public (List<string>,bool) TurbineExistInDatabase();

        public void InsertInfoPlt(DataTable dt_info, string name_table);
        public DataTable ReadInfoByTurbine(string path, string nameFile);

        public void InsertInfoWindTurbine(InfoByTurbineToTable infoTurbine);
        public List<string> SelectColumnFrom(string nameTable);
        public List<Wind_Turbine_Info> SelectAllTurbineInfo();
        public List<Sensor_Info> SelectAllNameSensor();
        public List<Error_Sensor> SelectAllNameSensorError();
        public DataTable SelectValueSensorByTurbine();
        public DataTable SelectPivotValueSensorByTurbine();
        public DataTable SelectErrorTableByTurbine();
        public DataTable SelectErrorTurbineByCondition(); 
    }
}
