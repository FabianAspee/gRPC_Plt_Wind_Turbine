using Blazored.Toast.Services;
using ExcelDataReader;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.LoadFileComponent
{
    public partial class LoadFiles
    {
        private int totalFile = 0;
        private Dictionary<string, IBrowserFile> myfiles = new Dictionary<string, IBrowserFile>();
        [Inject]
        protected IToastService Service { get; set; }
        protected override async Task OnInitializedAsync()
        {
            LoadFile.Service = toastService;
            LoadFile.LoadSatus += async (sender, args) =>
                   await LoadFile.WriteInfo(args);
            await Task.Run(() => LoadFile.RegisterEvent());
        }

        private void LoadInputFile(Dictionary<string, IBrowserFile> files)=>
            files.ToList().ForEach(file=> {
                if (!myfiles.TryGetValue(file.Key, out _))
                {
                    myfiles.Add(file.Key,file.Value);
                }
                else
                {
                    Service.ShowWarning("File with same name already exists, are conserve the latest file");
                    myfiles[file.Key] = file.Value;
                }
            });
        private async void LoadInfoTurbines()
        {
            await LoadFile.LoadInfoTurbines(myfiles);
            myfiles.Clear();
        }
        private async void LoadSensorTurbines()
        {
            await LoadFile.LoadSensorTurbines(myfiles);
            myfiles.Clear();
        }
         private async void LoadEventSensorTurbines()
        {
            await LoadFile.LoadEventSensorTurbines(myfiles);
            myfiles.Clear();
        }

     }
}
