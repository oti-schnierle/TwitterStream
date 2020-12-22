using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterStub.Interfaces.Metrics;
using TwitterStub.Models;
using TwitterStub.Models.Metrics;

namespace TwitterStub.Core.Metrics
{
    public class TopEmojiMetric : MetricsTracker
    {


        public TopEmojiMetric(IMetricSource source) : base(source)
        {

        }

        public override string Name => "Top Emojis";

        public override async Task<object> GetMetric()
        {
            if (Source?.Repository == null || Source.Emojis == null)
                return new List<Metric>();

            var emojis = (await Source.Repository.GetAll<Tweet>()).SelectMany(t => GetEmojis(t.text)).GroupBy(e => e.unified).GroupBy(g => (Text: g.Key, Count: g.Count())).OrderByDescending(h => h.Key.Count).Take(10).Select(h => h.Key).ToList();

            var result = emojis.Select(h => new Metric { Key = h.Text, Value = h.Count }).ToList();
            return result;
        }

        private IEnumerable<Emoji> GetEmojis(string text)
        {
            return Source.Emojis.Where(e => text.Contains(e.unified));
        }




    }
}
