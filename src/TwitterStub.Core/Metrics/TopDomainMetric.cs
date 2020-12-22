using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterStub.Interfaces.Metrics;
using TwitterStub.Models;
using TwitterStub.Models.Metrics;

namespace TwitterStub.Core.Metrics
{
    public class TopDomainMetric : MetricsTracker
    {
        public TopDomainMetric(IMetricSource source) : base(source)
        {

        }

        public override string Name => "Top Domains";

        public override async Task<object> GetMetric()
        {
            if (Source.Repository == null)
                return new List<Metric>();

            var urls = (await Source.Repository.GetAll<Url>()).Where(u => { return Uri.IsWellFormedUriString(u.expanded_url, UriKind.Absolute); });

            var domains = urls.GroupBy(u => new Uri(u.expanded_url).Host).GroupBy(g => new { Text = g.Key, Count = g.Count() }).OrderByDescending(h => h.Key.Count).Take(10).Select(h => h.Key);
            return domains.Select(h => new Metric { Key = h.Text, Value = h.Count });
        }

    }
}
