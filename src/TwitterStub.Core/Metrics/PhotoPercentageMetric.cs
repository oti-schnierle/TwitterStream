using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using TwitterStub.Interfaces.Metrics;
using TwitterStub.Models;
using TwitterStub.Models.Configuration;

namespace TwitterStub.Core.Metrics
{
    public class PhotoPercentageMetric : MetricsTracker
    {
        MetricsSettings _settings;

        public PhotoPercentageMetric(IMetricSource source, IOptions<MetricsSettings> settings) : base(source)
        {
            _settings = settings.Value;
        }

        public override string Name => "% Tweets with Pic";

        public override async Task<object> GetMetric()
        {
            if (Source.Repository == null || string.IsNullOrWhiteSpace(_settings?.PhotoDomains))
                return null;

            var picDomains = _settings.PhotoDomains.Split(",");
            var count = (await Source.Repository.GetAll<Tweet>()).Count();

            if (count == 0 || picDomains.Count() == 0)
            {
                return null;
            }

            decimal picDomainCount = (await Source.Repository.GetAll<Url>()).Where(u => picDomains.Any(d => u.display_url.Contains(d))).Count();
            var percentage = (picDomainCount / count);

            return percentage.ToString("p");
        }

    }
}
