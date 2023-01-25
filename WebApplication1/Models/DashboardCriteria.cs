using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class DashboardCriteria
    {
        public string Dashboard { get; set; }
        public string FromMonth { get; set; }
        public string ToMonth { get; set; }
        public string Filter1 { get; set; }
        public string Filter2 { get; set; }
        public string SLSPRSNID { get; set; }
        public string Groups { get; set; }
    }
}