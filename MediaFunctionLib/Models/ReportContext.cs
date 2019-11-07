using System;
using System.Collections.Generic;
using System.Text;

namespace MediaFunctionLib.Models
{
    public class ReportContext
    {
        public string Template { get; set; }
        public dynamic JSON { get; set; }
        public string ReportName { get; set; }
        public string[] Recipients { get; set; }
        public DateTime Date { get; set; }
    }
}
