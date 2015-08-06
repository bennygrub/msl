using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.Extensions;

namespace MySelfie.Models
{
    public class EventCreateModel
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }

    public class EventListModel
    {
        public List<EventCreateModel> Events { get; set; }
    }

}