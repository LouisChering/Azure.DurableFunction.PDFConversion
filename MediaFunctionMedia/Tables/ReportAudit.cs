using System;
using System.Collections.Generic;
using System.Text;

namespace MediaFunctionMedia.Tables
{
    public class ReportAudit
    {
        public string PartitionKey { get; set; }
        public string ReportName { get; set; }
        public string RowKey { get; set; }
        public string[] Recipients { get; set; }
    }
}
