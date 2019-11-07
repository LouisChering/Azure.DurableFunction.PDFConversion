using System;
using System.Collections.Generic;
using System.Text;

namespace MediaFunctionLib.Models
{
    public class EmailContext
    {
        public string[] Recipients { get; set; }
        public string[] CC { get; set; }
        public string[] BCC { get; set; }
        public string Subject { get; set; }
        public string body { get; set; }
    }
}
