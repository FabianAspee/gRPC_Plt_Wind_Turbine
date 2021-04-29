using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ClientPltTurbine.Pages.Component.LoadFileComponent
{
    public partial class LoadFiles
    {
        protected override async Task OnInitializedAsync()
        {
            LoadFile.Service = toastService;
            LoadFile.LoadSatus += async (sender, args) =>
                   await LoadFile.WriteInfo(args);
            await Task.Run(() => LoadFile.RegisterEvent());
        }
        void LoadInputFile(FileStream e)
        {

        }
    }
}
