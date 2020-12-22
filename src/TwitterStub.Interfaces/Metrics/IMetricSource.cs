using System.Collections.Generic;
using System.Threading.Tasks;
using TwitterStub.Interfaces.Data;
using TwitterStub.Models;

namespace TwitterStub.Interfaces.Metrics
{
    public interface IMetricSource
    {
        Task Update();

        IRepository Repository { get; }

        IEnumerable<Emoji> Emojis { get; }

        IEnumerable<Tweet> Tweets { get; }
    }
}
