using System;

namespace TwitterStub.Models.Metrics
{
    public class Metric
    {
        public string Key { get; set; }

        public object Value { get; set; }

        public TimeSpan Time { get; set; }
    }
}
