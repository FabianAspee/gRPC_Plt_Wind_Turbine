using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PltWindTurbine.Subscriber.SubscriberContract
{
    public interface IMetricCalculusSubscriber : ISubscriber
    {
        void InfoStdAndAvgWithNormalDistribution();
        void InfoStdAndAvgWithNormalDistribution(int idTurbine);
    }
}
