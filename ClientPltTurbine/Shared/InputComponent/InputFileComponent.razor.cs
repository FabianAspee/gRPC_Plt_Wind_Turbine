using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ClientPltTurbine.Shared.InputComponent
{
    public partial class InputFileComponent
    {
        [Parameter]
        public string Accept { get; set; }
        [Parameter]
        public string Id { get; set; }
        [Parameter]
        public EventCallback<Dictionary<string, IBrowserFile>> ValueChanged { get; set; }
        [Inject]
        private IToastService Toast { get; set; }
        private string dropClass = "";
        private readonly Dictionary<string, IBrowserFile> loadedFiles = new(); 

        public async Task OnValueChanged(InputFileChangeEventArgs files)
        {
            loadedFiles.Clear();
            try
            {
                foreach (var file in files.GetMultipleFiles(files.FileCount))
                {  
                    loadedFiles.Add(file.Name, file); 
                }
                await ValueChanged.InvokeAsync(loadedFiles);
            }
            catch (Exception)
            {
                Toast.ShowError("Error while attempting load file");
            }
             
        } 

        private void HandleDragEnter()
        {
            dropClass = "dropzone-drag";
        }

        private void HandleDragLeave()
        {
            dropClass = "";
        } 
    }
}
