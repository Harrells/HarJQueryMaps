using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class CountyResults
    {
        public string countryID { get; set; }
        public string stateID { get; set; }
        public string countyID { get; set; }
        public string county { get; set; }
        public string zipID { get; set; }
        public string state { get; set; }
        public string total { get; set; }
        public string realtotal { get; set; }
        public string rep1 { get; set; }
        public string rep1total { get; set; }
        public string rep2 { get; set; }
        public string rep2total { get; set; }
        public string rep3 { get; set; }
        public string rep3total { get; set; }
        public string otherstotal { get; set; }
        public string rep { get; set; }
        public string displaytext { get; set; }
    }
}