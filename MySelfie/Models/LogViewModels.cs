using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.Extensions;

namespace MySelfie.Models
{
    public class LogViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Type { get; set; }
        public string UserName { get; set; }
        public string Context { get; set; }
        public string Source { get; set; }
        public string Header { get; set; }
        public string Status { get; set; }
        public LogViewModel()
        {

        }
        public LogViewModel(WebLog entity)
        {
            this.MergeWithOtherType(entity);
            this.Id = entity.WebLogId;
            this.Source = "Web";
        }
        public LogViewModel(WorkerStatus entity)
        {
            this.MergeWithOtherType(entity);
            this.Id = entity.WorkerStatusID;
            this.Source = "Scraper";
        }
    }

    public class LogListViewModel
    {
        public IList<LogViewModel> LogList { get; set; }

        public int LastWebLogId { get; set; }
        public int LastWorkerLogId { get; set; }

        public int FetchSpeedInSeconds { get; set; }

        public int FetchAmount { get; set; }

        public string Filter { get; set; }
    }
}