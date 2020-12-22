using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterStub.Interfaces.Metrics;
using TwitterStub.Models;
using TwitterStub.Models.Metrics;

namespace TwitterStub.Core.Metrics
{
    public class TopHashtagMetric : MetricsTracker
    {


        public TopHashtagMetric(IMetricSource source) : base(source)
        {

        }

        public override string Name => "Top Hashtags";

        public override async Task<object> GetMetric()
        {
            if (Source?.Repository == null)
                return new List<Metric>();

            var hashtags = (await Source.Repository.GetAll<Hashtag>()).GroupBy(h => h.tag).GroupBy(group => new { Text = System.Web.HttpUtility.HtmlDecode(group.Key), Count = group.Count() }).OrderByDescending(h => h.Key.Count).Take(10).Select(h => h.Key);
            return hashtags.Select(h => new Metric { Key = h.Text, Value = h.Count });
        }

    }
}
