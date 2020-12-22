using System.Collections.Generic;
using System.Threading.Tasks;
using TwitterStub.Models.Metrics;

namespace TwitterStub.Interfaces.Statistics
{
    public interface IStatisticsService
    {
        IAsyncEnumerable<Metric> GetStatistics();
    }
}
