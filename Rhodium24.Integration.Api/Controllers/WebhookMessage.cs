using System;

namespace Rhodium24.Integration.Api.Controllers
{
    public class WebhookMessage<T>
    {
        public Guid Id { get; set; }
        public string Source { get; set; }
        public string SpecVersion { get; set; }
        public string Subject { get; set; }
        public DateTime Time { get; set; }
        public WebhookMessageType Type { get; set; }
        public T Data { get; set; }
    }

    public class WebhookMessage
    {
        public Guid Id { get; set; }
        public string Source { get; set; }
        public string SpecVersion { get; set; }
        public string Subject { get; set; }
        public DateTime Time { get; set; }
        public WebhookMessageType Type { get; set; }
    }
}
