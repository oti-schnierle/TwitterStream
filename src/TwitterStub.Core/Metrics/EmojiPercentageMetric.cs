using System.Linq;
using System.Threading.Tasks;
using TwitterStub.Interfaces.Metrics;
using TwitterStub.Models;

namespace TwitterStub.Core.Metrics
{
    public class EmojiPercentageMetric : MetricsTracker
    {
        public EmojiPercentageMetric(IMetricSource source) : base(source)
        {

        }

        public override string Name => "% Tweets with Emojis";

        public override async Task<object> GetMetric()
        {
            if (Source.Repository == null || Source.Emojis == null)
                return null;

            var tweets = await Source.Repository.GetAll<Tweet>();

            if (!tweets.Any())
                return null;

            decimal emojiCount = tweets.Where(t => Source.Emojis.Any(e => t.text.Contains(e.unified))).Count();
            var percentage = ((decimal)emojiCount / tweets.Count());

            return percentage.ToString("p");
        }

    }
}
