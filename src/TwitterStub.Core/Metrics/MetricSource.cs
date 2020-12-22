using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TwitterStub.Interfaces.Data;
using TwitterStub.Interfaces.Metrics;
using TwitterStub.Models;
using TwitterStub.Models.Configuration;

namespace TwitterStub.Core.Metrics
{
    public class MetricSource : IMetricSource
    {
        private readonly MetricsSettings _settings;
        private IEnumerable<Emoji> _emojis;

        public MetricSource(IRepository repository, IOptions<MetricsSettings> settings)
        {
            Repository = repository;
            _settings = settings.Value;
        }


        public IRepository Repository { get; }

        public IEnumerable<Tweet> Tweets { get; private set; }

        public IEnumerable<Emoji> Emojis => _emojis;

        public async Task Update()
        {
            await Task.WhenAll(LoadEmojis(), LoadTweets());

        }

        private async Task LoadTweets()
        {
            Tweets = (await Repository.GetAll<Tweet>()).OrderBy(t => t.created_at).ToList();
        }

        private async Task LoadEmojis()
        {
            if (_emojis == null)
            {
                JArray o1 = JArray.Parse(await File.ReadAllTextAsync(_settings.EmojiFile));
                var list = new List<Emoji>();
                foreach (var item in o1)
                {
                    var emoji = item.ToObject<Emoji>();
                    emoji.unified = DecodeEncodedNonAsciiCharacters(@"\u" + emoji.unified);
                    list.Add(emoji);

                }
                _emojis = list;
            }
        }

        public static string DecodeEncodedNonAsciiCharacters(string value)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m =>
                {
                    return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                });
        }
    }
}
