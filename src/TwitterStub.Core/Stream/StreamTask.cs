using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TwitterStub.Core.Exceptions;
using TwitterStub.Interfaces.Stream;
using TwitterStub.Models.Configuration;

namespace TwitterStub.Core.Stream
{
    public class StreamTask : IStreamTask
    {
        private readonly ILogger<StreamTask> _logger;
        private readonly TwitterStubSettings _settings;
        public StreamTask(ILogger<StreamTask> logger,IOptions<TwitterStubSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task<StreamReader> StartAsync(System.Threading.CancellationToken stoppingToken)
        {
            using (var client = new HttpClient())
            {
                var queryParams = new Dictionary<string, string>()
                {
                    //{ "expansions","attachments.poll_ids,attachments.media_keys,author_id,entities.mentions.username,geo.place_id,in_reply_to_user_id,referenced_tweets.id,referenced_tweets.id.author_id" },
                    
                    //{ "place.fields","contained_within,country,country_code,full_name,geo,id,name,place_type"},
                    //{ "poll.fields","duration_minutes,end_datetime,id,options,voting_status"},
                    //{ "user.fields","created_at,description,entities,id,location,name,pinned_tweet_id,profile_image_url,protected,public_metrics,url,username,verified,withheld"},
                    { "tweet.fields","attachments,author_id,context_annotations,conversation_id,created_at,entities,geo,id,in_reply_to_user_id,lang,public_metrics,possibly_sensitive,referenced_tweets,source,text,withheld"},
                    { "media.fields","duration_ms,height,media_key,preview_image_url,type,url,width,public_metrics"}
                };


                var uri = QueryHelpers.AddQueryString(_settings.Uri, queryParams);
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

                httpRequestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _settings.Token);

                var response = await client.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                var body = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                var stream = new StreamReader(body, Encoding.GetEncoding("utf-8"));

                if (response.IsSuccessStatusCode)
                {
                    return stream;
                }
                else
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.TooManyRequests:
                            _logger.LogWarning(response.ReasonPhrase);
                            throw new RateExceededException();
                        default:
                            _logger.LogWarning(response.ReasonPhrase);
                            throw new HttpRequestException();
                    }
                }

            }
        }

    }
}
