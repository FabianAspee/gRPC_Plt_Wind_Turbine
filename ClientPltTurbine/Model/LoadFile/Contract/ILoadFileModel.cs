using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Model.LoadFile.Contract
{
    public interface ILoadFileModel
    {
        public void LoadCsvFileBasic();
        public void LoadExcelFileBasic();
        public void LoadCsvFileSensor();

        public void LoadExcelFileSensor();
    }
}
