using System.Linq;
using System.Threading.Tasks;
using TwitterStub.Interfaces.Metrics;
using TwitterStub.Models;

namespace TwitterStub.Core.Metrics
{
    public class UrlsPercentageMetric : MetricsTracker
    {
        public UrlsPercentageMetric(IMetricSource source) : base(source)
        {

        }

        public override string Name => "% Tweets with Url";
        public override async Task<object> GetMetric()
        {
            if (Source?.Repository == null)
                return null;

            var tweets = await Source.Repository.GetAll<Tweet>();
            var urls = await Source.Repository.GetAll<Url>();

            if (!tweets.Any())
                return null;

            var percentage = (decimal)urls.Count() / tweets.Count();

            return percentage.ToString("p");
        }

    }
}
