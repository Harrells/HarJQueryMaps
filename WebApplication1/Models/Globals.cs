using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public static class Globals
    {
        public static int RequestID { get; set; }
        public static string Dashboard { get; set; }
        public static string FromMonth { get; set; }
        public static string ToMonth { get; set; }
        public static string Filter1 { get; set; }
        public static string Filter2 { get; set; }
        public static string Groups { get; set; }
        public static string DashboardUM { get; set; }
        public static List<CountyResults> Results { get; set; }
        public static List<Ranges> MapRanges { get; set; }

    }
}