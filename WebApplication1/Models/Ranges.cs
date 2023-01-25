using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Ranges
    {
        public int rangeID { get; set; }
        public string range_level { get; set; }
        public string range { get; set; }
        public double range_min { get; set; }
        public double range_max { get; set; }
        public string range_color { get; set; }
    }
}