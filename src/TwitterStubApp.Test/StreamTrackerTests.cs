using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterStub.Core.Metrics;
using TwitterStub.Interfaces.Data;
using TwitterStub.Interfaces.Metrics;
using TwitterStub.Models;
using TwitterStub.Models.Configuration;
using TwitterStub.Models.Metrics;

namespace TwitterStubApp.Test
{
    public class StreamTrackerTests
    {
        private IMetricTracker totalTweetsMetric;
        private IMetricTracker tweetsPerSecondMetric;
        private IMetricTracker tweetsPerMinuteMetric;
        private IMetricTracker tweetsPerHourMetric;
        private IMetricTracker emojiPercentageMetric;
        private IMetricTracker photoPercentageMetric;
        private IMetricTracker urlsPercentageMetric;
        private IMetricTracker topDomainMetric;
        private IMetricTracker topEmojiMetric;
        private IMetricTracker topHashtagMetric;
        private Mock<IMetricSource> _metricSource;
        private Mock<IRepository> _repository;
        private IEnumerable<Tweet> _list;
        private IEnumerable<Emoji> _emojis;
        private IEnumerable<Url> _url;
        private IEnumerable<Hashtag> _hashtags;
        private Mock<IOptions<MetricsSettings>> _settings;

        [OneTimeSetUp]
        public void Init()
        {
            _list = new List<Tweet>();
            _emojis = new List<Emoji>();
            _url = new List<Url>();
            _hashtags = new List<Hashtag>();

            _metricSource = new Mock<IMetricSource>();
            _repository = new Mock<IRepository>();
            _settings = new Mock<IOptions<MetricsSettings>>();

            _settings.Setup(x => x.Value).Returns(new MetricsSettings
            {
                EmojiFile = "c:\\Test\\emojiFile.json",
                PhotoDomains = "pic.twitter.com,instagram"
            });

            totalTweetsMetric = new TotalTweetsMetric(_metricSource.Object);
            tweetsPerSecondMetric = new TweetsPerSecondMetric(_metricSource.Object);
            tweetsPerMinuteMetric = new TweetsPerMinuteMetric(_metricSource.Object);
            tweetsPerHourMetric = new TweetsPerHourMetric(_metricSource.Object);
            emojiPercentageMetric = new EmojiPercentageMetric(_metricSource.Object);
            photoPercentageMetric = new PhotoPercentageMetric(_metricSource.Object, _settings.Object);
            urlsPercentageMetric = new UrlsPercentageMetric(_metricSource.Object);
            topDomainMetric = new TopDomainMetric(_metricSource.Object);
            topEmojiMetric = new TopEmojiMetric(_metricSource.Object);
            topHashtagMetric = new TopHashtagMetric(_metricSource.Object);
        }

        [SetUp]
        public void Setup()
        {
            _list = GetTweets();
            _emojis = GetEmojis();
            _url = GetUrls();
            _hashtags = GetHashtags();
            _metricSource.Setup(x => x.Repository).Returns(_repository.Object);
            _metricSource.Setup(x => x.Emojis).Returns(_emojis);

            _repository.Setup(x => x.GetAll<Tweet>()).Returns(Task.FromResult(_list));
            _repository.Setup(x => x.GetAll<Url>()).Returns(Task.FromResult(_url));
            _repository.Setup(x => x.GetAll<Hashtag>()).Returns(Task.FromResult(_hashtags));


        }

        private IEnumerable<Hashtag> GetHashtags()
        {
            var list = new List<Hashtag>();
            list.Add(new Hashtag { tag = "ThisIsATaGA" });
            list.Add(new Hashtag { tag = "ThisIsATaGA" });
            list.Add(new Hashtag { tag = "ThisIsATaGA" });
            list.Add(new Hashtag { tag = "ThisIsATaGA" });
            list.Add(new Hashtag { tag = "ThisIsATaGB" });
            list.Add(new Hashtag { tag = "ThisIsATaGB" });
            list.Add(new Hashtag { tag = "ThisIsATaGB" });
            list.Add(new Hashtag { tag = "ThisIsATaGB" });
            list.Add(new Hashtag { tag = "ThisIsATaGC" });
            list.Add(new Hashtag { tag = "ThisIsATaGC" });
            list.Add(new Hashtag { tag = "ThisIsATaGC" });
            list.Add(new Hashtag { tag = "ThisIsATaGD" });
            list.Add(new Hashtag { tag = "ThisIsATaGD" });
            list.Add(new Hashtag { tag = "ThisIsATaGE" });
            list.Add(new Hashtag { tag = "ThisIsATaGF" });
            return list;
        }

        private IEnumerable<Emoji> GetEmojis()
        {
            var list = new List<Emoji>();
            list.Add(new Emoji { name = "HASH KEY", unified = "0023-FE0F-20E3" });
            list.Add(new Emoji { name = "KEYCAP 2", unified = "0032-FE0F-20E3" });
            list.Add(new Emoji { name = "HEAVY LARGE CIRCLE", unified = "2B55" });
            return list;
        }

        private IEnumerable<Url> GetUrls()
        {
            var list = new List<Url>();
            list.Add(new Url { expanded_url = "http://bit.ly/2iOQUrw", display_url = "bit.ly/2iOQUrw" });
            list.Add(new Url { expanded_url = "https://twitter.com/tripplannermama/status/yyyyyyyyy/photo/1", display_url = "pic.twitter.com/yyyyyyyy" });
            list.Add(new Url { expanded_url = "https://twitter.com/tripplannermama/status/xxxxxxxxxxx/photo/1", display_url = "pic.twitter.com/xxxxxx" });
            list.Add(new Url { expanded_url = "https://www.instagram.com/xxxxxx/", display_url = "instagram.com/xxxxxx/" });
            return list;
        }

        private IEnumerable<Tweet> GetTweets()
        {
            var list = new List<Tweet>();
            var startDate = DateTime.Now;
            for (int i = 0; i < 500; i++)
            {
                list.Add(new Tweet { text = $"this is a tweet # {i}!", created_at = DateTime.Now });
            }
            return list;
        }

        private IEnumerable<Emoji> GetEmojis(string text)
        {
            return _emojis.Where(e => text.Contains(e.unified));
        }


        [Test]
        public async Task TrackNumberOfTweets()
        {

            Assert.AreEqual(_list.Count(), await totalTweetsMetric.GetMetric());
        }

        [Test]
        public async Task TrackNumberOfTweets_NoItems()
        {
            IEnumerable<Tweet> list = new List<Tweet>();
            _repository.Setup(x => x.GetAll<Tweet>()).Returns(Task.FromResult(list));
            Assert.AreEqual(0, await totalTweetsMetric.GetMetric());
        }

        [Test]
        public async Task TrackNumberOfTweets_NoSource()
        {
            _metricSource.Reset();
            Assert.AreEqual(0, await totalTweetsMetric.GetMetric());
        }

        [Test]
        public async Task TrackTweetsPerSecond()
        {
            _metricSource.Setup(x => x.Tweets).Returns(_list);
            var duration = _list.Where(t => t.created_at >= _list.Max(t => t.created_at).AddSeconds(-1)).Count();

            Assert.AreEqual(duration, await tweetsPerSecondMetric.GetMetric());


        }

        [Test]
        public async Task TrackTweetsPerSeconds_NoItems()
        {
            IEnumerable<Tweet> list = new List<Tweet>();
            _repository.Setup(x => x.GetAll<Tweet>()).Returns(Task.FromResult(list));
            _metricSource.Setup(x => x.Tweets).Returns(list);
            Assert.AreEqual(0, await tweetsPerSecondMetric.GetMetric());
        }

        [Test]
        public async Task TrackTweetsPerSeconds_NoSource()
        {
            _metricSource.Reset();
            Assert.AreEqual(0, await tweetsPerSecondMetric.GetMetric());
        }

        [Test]
        public async Task TrackTweetsPerMinute()
        {
            _metricSource.Setup(x => x.Tweets).Returns(_list);
            var duration = _list.Where(t => t.created_at >= _list.Max(t => t.created_at).AddMinutes(-1)).Count();

            Assert.AreEqual(duration, await tweetsPerMinuteMetric.GetMetric());


        }

        [Test]
        public async Task TrackTweetsPerMinutes_NoItems()
        {
            IEnumerable<Tweet> list = new List<Tweet>();
            _repository.Setup(x => x.GetAll<Tweet>()).Returns(Task.FromResult(list));
            Assert.AreEqual(0, await tweetsPerMinuteMetric.GetMetric());
        }

        [Test]
        public async Task TrackTweetsPerMinute_NoSource()
        {
            _metricSource.Reset();
            Assert.AreEqual(0, await tweetsPerMinuteMetric.GetMetric());
        }

        [Test]
        public async Task TrackTweetsPerHour()
        {
            _metricSource.Setup(x => x.Tweets).Returns(_list);
            var duration = _list.Where(t => t.created_at >= _list.Max(t => t.created_at).AddHours(-1)).Count();

            Assert.AreEqual(duration, await tweetsPerHourMetric.GetMetric());


        }

        [Test]
        public async Task TrackTweetsPerHour_NoItems()
        {
            IEnumerable<Tweet> list = new List<Tweet>();
            _repository.Setup(x => x.GetAll<Tweet>()).Returns(Task.FromResult(list));
            _metricSource.Setup(x => x.Tweets).Returns(list);
            Assert.AreEqual(0, await tweetsPerHourMetric.GetMetric());
        }

        [Test]
        public async Task TrackTweetsPerHour_NoSource()
        {
            _metricSource.Reset();
            Assert.AreEqual(0, await tweetsPerHourMetric.GetMetric());
        }

        [Test]
        public async Task TrackEmojiPercentageMetric()
        {
            List<Tweet> list = new List<Tweet>();
            for (int x = 0; x < _emojis.Count(); x++)
            {
                list.Add(new Tweet { text = $"this is a tweet {_emojis.ElementAt(x).unified}!", created_at = DateTime.Now });
            }
            _repository.Setup(x => x.GetAll<Tweet>()).Returns(Task.FromResult((IEnumerable<Tweet>)list));
            decimal percentage = ((decimal)_emojis.Count() / list.Count());

            Assert.AreEqual(percentage.ToString("p"), await emojiPercentageMetric.GetMetric());


        }

        [Test]
        public async Task TrackEmojiPercentageMetric_NoItems()
        {
            IEnumerable<Tweet> list = new List<Tweet>();
            _repository.Setup(x => x.GetAll<Tweet>()).Returns(Task.FromResult(list));
            Assert.AreEqual(null, await emojiPercentageMetric.GetMetric());
        }

        [Test]
        public async Task TrackEmojiPercentageMetric_NoSource()
        {
            _metricSource.Reset();
            Assert.AreEqual(null, await emojiPercentageMetric.GetMetric());
        }

        [Test]
        public async Task TrackPhotoPercentageMetric()
        {


            decimal picDomainCount = _url.Where(u => _settings.Object.Value.PhotoDomains.Split(",").Any(d => u.display_url.Contains(d))).Count();
            var percentage = (picDomainCount / _list.Count());

            Assert.AreEqual(percentage.ToString("p"), await photoPercentageMetric.GetMetric());


        }

        [Test]
        public async Task TrackPhotoPercentageMetric_NoItems()
        {
            IEnumerable<Tweet> list = new List<Tweet>();
            _repository.Setup(x => x.GetAll<Tweet>()).Returns(Task.FromResult(list));
            Assert.AreEqual(null, await photoPercentageMetric.GetMetric());
        }

        [Test]
        public async Task TrackPhotoPercentageMetric_NoSettings()
        {
            IEnumerable<Tweet> list = new List<Tweet>();
            _settings.Reset();
            photoPercentageMetric = new PhotoPercentageMetric(_metricSource.Object, _settings.Object);
            Assert.AreEqual(null, await photoPercentageMetric.GetMetric());
        }

        [Test]
        public async Task TrackPhotoPercentageMetric_NoSource()
        {
            _metricSource.Reset();
            Assert.AreEqual(null, await photoPercentageMetric.GetMetric());
        }

        [Test]
        public async Task TrackTopDomainMetric()
        {


            var domains = _url.Where(u => { return Uri.IsWellFormedUriString(u.expanded_url, UriKind.Absolute); }).GroupBy(u => new Uri(u.expanded_url).Host).GroupBy(g => new { Text = g.Key, Count = g.Count() }).OrderByDescending(h => h.Key.Count).Take(10).Select(h => h.Key);
            var expected = domains.Select(h => new Metric { Key = h.Text, Value = h.Count });
            var result = (IEnumerable<Metric>)await topDomainMetric.GetMetric();


            Assert.AreEqual(expected.Count(), result.Count());
            for (var x = 0; x < expected.Count(); x++)
            {
                Assert.AreEqual(expected.ElementAt(x).Key, result.ElementAt(x).Key);
                Assert.AreEqual(expected.ElementAt(x).Value, result.ElementAt(x).Value);
            }



        }

        [Test]
        public async Task TrackTopDomainMetric_NoItems()
        {
            IEnumerable<Url> list = new List<Url>();
            _repository.Setup(x => x.GetAll<Url>()).Returns(Task.FromResult(list));

            var domains = list.GroupBy(u => new Uri(u.expanded_url).Host).GroupBy(g => new { Text = g.Key, Count = g.Count() }).OrderByDescending(h => h.Key.Count).Take(10).Select(h => h.Key);
            domains.Select(h => new Metric { Key = h.Text, Value = h.Count });

            Assert.AreEqual(domains.Select(h => new Metric { Key = h.Text, Value = h.Count }), await topDomainMetric.GetMetric());
        }


        [Test]
        public async Task TrackTopDomainMetric_NoSource()
        {
            _metricSource.Reset();
            Assert.AreEqual(new List<Metric>(), await topDomainMetric.GetMetric());
        }

        [Test]
        public async Task TrackUrlsPercentageMetric()
        {



            decimal percentage = ((decimal)_url.Count() / _list.Count());

            Assert.AreEqual(percentage.ToString("p"), await urlsPercentageMetric.GetMetric());


        }

        [Test]
        public async Task TrackUrlsPercentageMetric_NoItems()
        {
            IEnumerable<Tweet> list = new List<Tweet>();
            _repository.Setup(x => x.GetAll<Tweet>()).Returns(Task.FromResult(list));
            Assert.AreEqual(null, await urlsPercentageMetric.GetMetric());
        }


        [Test]
        public async Task TrackUrlsPercentageMetric_NoSource()
        {
            _metricSource.Reset();
            Assert.AreEqual(null, await urlsPercentageMetric.GetMetric());
        }

        [Test]
        public async Task TrackUrlsPercentageMetric_NoUrls()
        {
            IEnumerable<Url> list = new List<Url>();
            _repository.Setup(x => x.GetAll<Url>()).Returns(Task.FromResult(list));

            decimal percentage = ((decimal)list.Count() / _list.Count());

            Assert.AreEqual(percentage.ToString("p"), await urlsPercentageMetric.GetMetric());
        }

        [Test]
        public async Task TrackTopEmojiMetric()
        {


            List<Tweet> list = new List<Tweet>();
            for (int x = 0; x < _emojis.Count(); x++)
            {
                list.Add(new Tweet { text = $"this is a tweet {_emojis.ElementAt(x).unified}!", created_at = DateTime.Now });
            }
            _repository.Setup(x => x.GetAll<Tweet>()).Returns(Task.FromResult((IEnumerable<Tweet>)list));

            var emojis = list.SelectMany(t => GetEmojis(t.text)).GroupBy(e => e.unified).GroupBy(g => (Text: g.Key, Count: g.Count())).OrderByDescending(h => h.Key.Count).Take(10).Select(h => h.Key).ToList();

            var expected = emojis.Select(h => new Metric { Key = h.Text, Value = h.Count }).ToList();

            var result = (IEnumerable<Metric>)await topEmojiMetric.GetMetric();

            Assert.AreEqual(expected.Count(), result.Count());
            for (var x = 0; x < expected.Count(); x++)
            {
                Assert.AreEqual(expected.ElementAt(x).Key, result.ElementAt(x).Key);
                Assert.AreEqual(expected.ElementAt(x).Value, result.ElementAt(x).Value);
            }
        }

        [Test]
        public async Task TrackTopEmojiMetric_NoItems()
        {
            IEnumerable<Tweet> list = new List<Tweet>();
            _repository.Setup(x => x.GetAll<Tweet>()).Returns(Task.FromResult(list));

            var emojis = list.SelectMany(t => GetEmojis(t.text)).GroupBy(e => e.unified).GroupBy(g => (Text: g.Key, Count: g.Count())).OrderByDescending(h => h.Key.Count).Take(10).Select(h => h.Key).ToList();

            Assert.AreEqual(emojis.Select(h => new Metric { Key = h.Text, Value = h.Count }), await topEmojiMetric.GetMetric());
        }


        [Test]
        public async Task TrackTopEmojiMetric_NoSource()
        {
            _metricSource.Reset();
            Assert.AreEqual(new List<Metric>(), await topEmojiMetric.GetMetric());
        }

        [Test]
        public async Task TrackTopEmojiMetric_NoEmojis()
        {
            IEnumerable<Emoji> list = new List<Emoji>();
            _metricSource.Setup(x => x.Emojis).Returns(list);


            Assert.AreEqual(new List<Metric>(), await topEmojiMetric.GetMetric());
        }


        [Test]
        public async Task TrackTopHashtagMetric()
        {

            var hashtags = _hashtags.GroupBy(h => h.tag).GroupBy(group => new { Text = System.Web.HttpUtility.HtmlDecode(group.Key), Count = group.Count() }).OrderByDescending(h => h.Key.Count).Take(10).Select(h => h.Key);
            var expected = hashtags.Select(h => new Metric { Key = h.Text, Value = h.Count }).ToList();

            var result = (IEnumerable<Metric>)await topHashtagMetric.GetMetric();

            Assert.AreEqual(expected.Count(), result.Count());
            for (var x = 0; x < expected.Count(); x++)
            {
                Assert.AreEqual(expected.ElementAt(x).Key, result.ElementAt(x).Key);
                Assert.AreEqual(expected.ElementAt(x).Value, result.ElementAt(x).Value);
            }
        }

        [Test]
        public async Task TrackTopHashtagMetric_NoItems()
        {
            IEnumerable<Hashtag> list = new List<Hashtag>();
            _repository.Setup(x => x.GetAll<Hashtag>()).Returns(Task.FromResult(list));

            var hashtags = list.GroupBy(h => h.tag).GroupBy(group => new { Text = System.Web.HttpUtility.HtmlDecode(group.Key), Count = group.Count() }).OrderByDescending(h => h.Key.Count).Take(10).Select(h => h.Key);

            Assert.AreEqual(hashtags.Select(h => new Metric { Key = h.Text, Value = h.Count }), await topHashtagMetric.GetMetric());
        }


        [Test]
        public async Task TrackTopHashtagMetric_NoSource()
        {
            _metricSource.Reset();
            Assert.AreEqual(new List<Metric>(), await topHashtagMetric.GetMetric());
        }
    }
}