using PltWindTurbine.Subscriber.SubscriberContract;
using PltWindTurbine.Subscriber.SubscriberImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber.SubscriberFactory
{
    public class SubscriberFactory : ISubscriberFactory
    {
        public ILoadFileSubscriber GetLoadFileSubscriber() => new LoadFileSubscriber();

        public IMetricCalculusSubscriber GetMetricCalculusSubscriber() => new MetricCalculusSubscriber();

        public IObtainInfoTurbinesSubscriber GetObtainInfoTurbinesSubscriber() => new ObtainInfoTurbineSubscriber();

        public IViewFailureSubscriber GetViewFailureSubscriber() => new ViewFailureSubscriber();
    }
}
