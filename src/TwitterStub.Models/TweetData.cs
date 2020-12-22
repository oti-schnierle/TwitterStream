using System;
using System.Collections.Generic;

namespace TwitterStub.Models
{

    public class TweetData
    {
        public Tweet data { get; set; }
    }

    public class Tweet
    {
        public string author_id { get; set; }
        public DateTime created_at { get; set; }
        public string source { get; set; }
        public string lang { get; set; }
        public bool possibly_sensitive { get; set; }
        public string conversation_id { get; set; }
        public string text { get; set; }
        public Entities entities { get; set; }
        public string id { get; set; }
    }

    public class Entities
    {
        public string entities_id { get; set; }

        public string id { get; set; }

        public ICollection<Hashtag> hashtags { get; set; }

        public ICollection<Url> urls { get; set; }
    }

    public class Hashtag
    {
        public string id { get; set; }

        public string entities_id { get; set; }
        public string hashtag_id { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public string tag { get; set; }
    }

    public class Url
    {
        public string id { get; set; }

        public string entities_id { get; set; }

        public string url_id { get; set; }
        public int start { get; set; }
        public int end { get; set; }
        public string url { get; set; }
        public string expanded_url { get; set; }
        public string display_url { get; set; }
    }


}
