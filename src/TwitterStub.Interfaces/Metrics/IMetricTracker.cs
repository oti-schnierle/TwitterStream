using System;
using System.Threading.Tasks;

namespace TwitterStub.Interfaces.Metrics
{
    public interface IMetricTracker
    {
        string Name { get; }

        Task<object> GetMetric();
    }
}