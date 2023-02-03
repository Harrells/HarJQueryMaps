using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class SATCriteria
    {
        public string LOCNCODE { get; set; }
        public string Segment { get; set; }
        public string ReportGroup { get; set; }
        public string RepID { get; set; }
        public string Partnership { get; set; }
        public string Category { get; set; }
        public string Item { get; set; }
        public string Class { get; set; }
        public string Vendor { get; set; }
        public string Type { get; set; }
        public bool ExcludeZZs { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string GroupByField { get; set; }
        public string SearchFor { get; set; }
        public int? PagingStart { get; set; }
        public int? NumRecs { get; set; }
        public string SortFieldName { get; set; }
        public string SortDirection { get; set; }

    }
}