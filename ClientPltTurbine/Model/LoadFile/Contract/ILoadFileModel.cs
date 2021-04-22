using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model.LoadFile.Contract
{
    public interface ILoadFileModel
    {
        public Task<(string, DataTable, int)> LoadCsvFileBasic(string filepath);
        public Task<(string, DataTable, int)> LoadExcelFileBasic(string filepath);
        public Task<(string, DataTable)> LoadCsvFileSensor(string filepath);

        public Task<(string, DataTable)> LoadExcelFileSensor(string filepath);
        Task ProcessFileBasic(DataTable file, string name, string type, string sep, int id);
        Task ProcessSensorFile(DataTable file, string name, string type, string sep, bool isEvent);
    }
}
