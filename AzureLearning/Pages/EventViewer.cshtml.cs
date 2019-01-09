using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace AzureLearning.Pages
{
    public class EventViewerModel : PageModel
    {
        public void OnGet()
        {
        }
    }
    public class GridEventsHub : Hub
    {
        public GridEventsHub()
        {
        }
    }
    public class CloudEvent<T> where T : class
    {
        [JsonProperty("eventID")]
        public string EventId { get; set; }

        [JsonProperty("cloudEventsVersion")]
        public string CloudEventVersion { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("eventTypeVersion")]
        public string EventTypeVersion { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("eventTime")]
        public string EventTime { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
    public class GridEvent<T> where T : class
    {
        public string Id { get; set; }
        public string EventType { get; set; }
        public string Subject { get; set; }
        public DateTime EventTime { get; set; }
        public T Data { get; set; }
        public string Topic { get; set; }
    }
}
