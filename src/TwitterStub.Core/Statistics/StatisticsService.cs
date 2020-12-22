using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TwitterStub.Interfaces.Metrics;
using TwitterStub.Interfaces.Statistics;
using TwitterStub.Models.Metrics;

namespace TwitterStub.Core.Statistics
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IMetricSource _metricSource;
        private readonly IServiceProvider _serviceProvider;

        public StatisticsService(IMetricSource metricSource, IServiceProvider serviceProvider)
        {
            _metricSource = metricSource;
            _serviceProvider = serviceProvider;
        }

        public async IAsyncEnumerable<Metric> GetStatistics()
        {
            await _metricSource.Update();

            var trackers = _serviceProvider.GetServices<IMetricTracker>();
            if (trackers?.Any() == true)
            {
                foreach (var tracker in trackers) {
                    var start = DateTime.Now;
                    var value = await tracker.GetMetric();
                    yield return new Metric { Key = tracker.Name, Value = value, Time = DateTime.Now - start };
                }
                 
            }
            


            
        }
    }
}
