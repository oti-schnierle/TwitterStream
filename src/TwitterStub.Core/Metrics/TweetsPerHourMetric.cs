using System.Linq;
using System.Threading.Tasks;
using TwitterStub.Interfaces.Metrics;
using TwitterStub.Models;

namespace TwitterStub.Core.Metrics
{
    public class TweetsPerHourMetric : MetricsTracker
    {
        public TweetsPerHourMetric(IMetricSource source) : base(source)
        {

        }

        public override string Name => "Tweets/hr";

        public override Task<object> GetMetric()
        {
            if (Source?.Tweets == null)
                return Task.FromResult<object>(0);

            var tweets = Source.Tweets;
            var duration = tweets.Where(t => t.created_at >= tweets.Last().created_at.AddHours(-1)).Count();
            return Task.FromResult<object>(duration);
        }

    }
}
