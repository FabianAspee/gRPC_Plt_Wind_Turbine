using PltWindTurbine.Database.DatabaseConnection;
using PltWindTurbine.Database.DatabaseContract;
using PltWindTurbine.Subscriber.SubscriberContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber.SubscriberImplementation
{
    public class MetricCalculusSubscriber : AbstractSubscriber, IMetricCalculusSubscriber
    {
        private readonly IOperationTurbineDatabase database = RetreiveImplementationDatabase.Instance.ImplementationDatabase;
        public void InfoStdAndAvgWithNormalDistribution()
        {
            throw new NotImplementedException();
        }

        public void InfoStdAndAvgWithNormalDistribution(int idTurbine)
        { 
            throw new NotImplementedException();
        }
    }
}
