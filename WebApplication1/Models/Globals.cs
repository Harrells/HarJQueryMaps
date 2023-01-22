using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public static class Globals
    {
        public static int RequestID { get; set; }
        public static string FromMonth { get; set; }
        public static string ToMonth { get; set; }
        public static string Filter1 { get; set; }
        public static string Filter2 { get; set; }
        public static List<NameValue> Groups { get; set; }

    }
}