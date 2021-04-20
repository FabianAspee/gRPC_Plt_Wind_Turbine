using PltWindTurbine.Services.Loadfilesservices;
using PltWindTurbine.Subscriber.EventArgument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber.SubscriberContract
{
    public interface ILoadFileSubscriber : ISubscriber
    {
        event EventHandler<StatusLoadFile> StatusLoad;
        public Task LoadFilesInfoTurbine(FileUploadRequest file);
        public Task LoadFilesNameSensor(FileUploadRequest file);
        public Task LoadFilesNameErrorSensor(FileUploadRequest file);
        public Task LoadFilesErrorCode(FileUploadRequest file);
        public Task LoadNormalSensor(ReadInfoSensor files);
        public Task LoadEventSensor(ReadInfoSensor files);

    }
}
