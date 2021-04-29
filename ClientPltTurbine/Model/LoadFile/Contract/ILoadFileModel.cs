using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model.LoadFile.Contract
{
    public interface ILoadFileModel
    {
        public Task<(string, DataTable, int)> LoadCsvFileBasic(KeyValuePair<string, IBrowserFile> infoFile);
        public Task<(string, DataTable, int)> LoadExcelFileBasic(KeyValuePair<string, IBrowserFile> infoFile);
        public Task<(string, DataTable)> LoadCsvFileSensor(KeyValuePair<string, IBrowserFile> infoFile);

        public Task<(string, DataTable)> LoadExcelFileSensor(KeyValuePair<string, IBrowserFile> infoFile);
        Task ProcessFileBasic(DataTable file, string name, string type, string sep, int id);
        Task ProcessSensorFile(DataTable file, string name, string type, string sep, bool isEvent);
    }
}
