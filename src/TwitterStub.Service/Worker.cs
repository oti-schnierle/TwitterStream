using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitterStub.Core.Exceptions;
using TwitterStub.Interfaces.Data;
using TwitterStub.Interfaces.Stream;
using TwitterStub.Models;

namespace TwitterStub.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IStreamTask _streamTask;
        private readonly IRepository _repository;

        public Worker(ILogger<Worker> logger, IStreamTask streamTask, IRepository repository)
        {
            _logger = logger;
            _streamTask = streamTask;
            _repository = repository;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var waitTime = TimeSpan.FromSeconds(30);
            var startTime = DateTime.Now;
            try
            {
                using (var stream = await _streamTask.StartAsync(stoppingToken))
                {
                    await SaveStream(stream);
                }
            }
            catch (RateExceededException ex)
            {
                waitTime = TimeSpan.FromMilliseconds(300000);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await this.StopAsync(stoppingToken);
            }

            var elapsedTime = DateTime.Now - startTime;
            waitTime = waitTime < elapsedTime ? TimeSpan.FromSeconds(0) : waitTime - elapsedTime;
            _logger.LogInformation($"Stopping worker for {waitTime}");
            await Task.Delay(waitTime);
            await this.ExecuteAsync(stoppingToken);
        }

        private async Task SaveStream(StreamReader stream)
        {
            var json = await stream.ReadLineAsync();
            while (!(stream.EndOfStream || string.IsNullOrEmpty(json)))
            {
                _logger.LogDebug($" Tweet received: {json}");
                var tweetData = JsonConvert.DeserializeObject<TweetData>(json);

                if (tweetData?.data?.entities != null)
                {
                    tweetData.data.entities.entities_id = Guid.NewGuid().ToString();
                    tweetData.data.entities.id = tweetData.data.id;
                    if (tweetData.data.entities.hashtags?.Any() == true)
                    {
                        foreach (var hashtag in tweetData.data.entities.hashtags)
                        {
                            hashtag.id = tweetData.data.id;
                            hashtag.entities_id = tweetData.data.entities.entities_id;
                            hashtag.hashtag_id = Guid.NewGuid().ToString();
                        }
                    }
                    if (tweetData.data.entities.urls?.Any() == true)
                    {
                        foreach (var url in tweetData.data.entities.urls)
                        {
                            url.id = tweetData.data.id;
                            url.entities_id = tweetData.data.entities.entities_id;
                            url.url_id = Guid.NewGuid().ToString();
                        }
                    }
                }

                
                await _repository.Add(tweetData.data);
                _logger.LogDebug($" Tweet added: {tweetData.data}");

                json = await stream.ReadLineAsync();
            }
        }
    }
}
