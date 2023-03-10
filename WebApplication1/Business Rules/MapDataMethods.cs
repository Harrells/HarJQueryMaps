using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Collections;
using Dapper;
using WebApplication1.Models;

namespace WebApplication1.Business_Rules
{
    public class MapDataMethods
    {
        public static List<CountyResults> GetCountyResults(DashboardCriteria criteria)
        {
            try
            {
                using (SqlConnection db = new SqlConnection(Environment.GetEnvironmentVariable("SQLCONNSTR_CMDTAContext")))
                using (SqlConnection azdb = new SqlConnection(Environment.GetEnvironmentVariable("SQLCONNSTR_HarMgmtDBContext")))
                {
                    if (string.IsNullOrEmpty(criteria.Dashboard))
                        return new List<CountyResults>();

                    if (string.IsNullOrEmpty(criteria.FromMonth))
                        return new List<CountyResults>();

                    if (string.IsNullOrEmpty(criteria.ToMonth))
                        return new List<CountyResults>();

                    if (criteria.Filter1 == null)
                        criteria.Filter1 = "";
                    if (criteria.Filter2 == null)
                        criteria.Filter2 = "";

                    bool includesoil = false;

                    string sqlstring = "SELECT * FROM BOGODashboards b WHERE b.DashboardName=@dashboard";
                    var dashboardinfo = azdb.Query(sqlstring, new { @dashboard = criteria.Dashboard }).FirstOrDefault();
                    if (dashboardinfo == null)
                        return new List<CountyResults>();

                    includesoil = dashboardinfo.IncludeSoilFertTons;

                    Models.Globals.SumFieldName = dashboardinfo.SumFieldName;

                    // fill global ranges list
                    sqlstring = "SELECT br.rangeID, br.range_level, range, br.range_min, br.range_max, br.range_color"
                        + " FROM BOGODashboardsRanges br"
                        + " WHERE dashboardname=@dashboard";
                    Models.Globals.MapRanges = azdb.Query<Ranges>(sqlstring, new { @dashboard = Models.Globals.Dashboard }).ToList();

                    string filterfieldname1 = "";
                    string filterfieldname2 = "";
                    string dashboardname = dashboardinfo.DashboardName.TrimEnd().ToUpper();
                    string dashboardum = "Tons";
                    string company = "South";
                    string whereclause = "";
                    string fieldname = "BaseQty";

                    int maxnmbr = 0;

                    Models.Globals.DashboardUM = "Tons";

                    if (criteria.Filter1 != "")
                    {
                        if (!string.IsNullOrEmpty(dashboardinfo.Filter1FieldName))
                            filterfieldname1 = dashboardinfo.Filter1FieldName;
                    }
                    if (criteria.Filter2 != "")
                    {
                        if (!string.IsNullOrEmpty(dashboardinfo.Filter2FieldName))
                            filterfieldname2 = dashboardinfo.Filter2FieldName;
                    }

                    if (!string.IsNullOrEmpty(Models.Globals.SumFieldName))
                        fieldname = Models.Globals.SumFieldName;

                    string convertUM = "";
                    if (!string.IsNullOrEmpty(dashboardinfo.ConvertUM))
                    {
                        convertUM = dashboardinfo.ConvertUM;
                        // if they are converting to a um, the sumfieldname has to be ess.baseqty*iv.EQUOMQTY
                        fieldname = "ess.baseqty * iv.EQUOMQTY";
                    }

                    if (!string.IsNullOrEmpty(dashboardinfo.DashboardUM))
                    {
                        Models.Globals.DashboardUM = dashboardinfo.DashboardUM;
                        dashboardum = dashboardinfo.DashboardUM;
                    }

                    if (!string.IsNullOrEmpty(dashboardinfo.WhereClause))
                        whereclause = dashboardinfo.WhereClause;


                    string fromyearstring = criteria.FromMonth.Substring(0, criteria.FromMonth.IndexOf("/"));
                    string frommonthstring = criteria.FromMonth.Substring(criteria.FromMonth.IndexOf("/") + 1, criteria.FromMonth.Length - criteria.FromMonth.IndexOf("/") - 1);
                    int frommonth = int.Parse(frommonthstring);
                    int fromyear = int.Parse(fromyearstring);

                    DateTime begdate = new DateTime(fromyear, frommonth, 1);

                    string toyearstring = criteria.ToMonth.Substring(0, criteria.ToMonth.IndexOf("/"));
                    string tomonthstring = criteria.ToMonth.Substring(criteria.ToMonth.IndexOf("/") + 1, criteria.ToMonth.Length - criteria.ToMonth.IndexOf("/") - 1);
                    int tomonth = int.Parse(tomonthstring);
                    int toyear = int.Parse(toyearstring);

                    DateTime enddate = new DateTime(toyear, tomonth, 1);
                    enddate = enddate.AddMonths(1);
                    enddate = enddate.AddDays(-1);

                    DateTime currbeg = begdate;
                    DateTime currend = enddate;

                    int priorfromyear = fromyear - 1;
                    int priortoyear = toyear - 1;

                    DateTime priorbeg = new DateTime(1900, 1, 1);
                    DateTime priorend = new DateTime(1900, 1, 1);

                    // if more than one year, can't do priors

                    if (currend < currbeg)
                        return new List<CountyResults>();

                    if (currend < currbeg.AddYears(1))
                    {
                        priorbeg = currbeg.AddYears(-1);
                        priorend = currend.AddYears(-1);
                    }

                    List<CountyResults> results = new List<CountyResults>();
                    List<CountyResults> details = new List<CountyResults>();

                    if (dashboardname == "POLYON")
                    {
                        // polyon dashboard is slightly different

                        sqlstring = "SELECT bc.countryID, bc.stateID, bc.countyID, bc.county, bc.StateName as state, ISNULL(DATA.total,0)  AS total"
                            + " FROM BOGODashboardsCounties bc left JOIN"
                            + " (SELECT  r.JQMCountyId, SUM(ROUND((CASE WHEN ((Year > @fromyear AND Year < @toyear) OR ( (Year = @fromyear AND Month >= @frommonth) or(Year = @toyear AND Month <= @tomonth))) THEN QtyTons ELSE 0 END), 2)) AS total"
                            + " FROM RawSalesHistoryByCounty r INNER JOIN webhub.dbo.employees we ON r.SLPRSNID = we.ImportKey"
                            + " INNER JOIN webhub.dbo.Segments s ON we.SegmentId = s.SegmentId"
                            + " INNER JOIN webhub.dbo.EmployeeGroups eg ON we.EmployeeGroupId = eg.EmployeeGroupId"
                            + " inner Join CM_Inventory ON r.ITEMNMBR = CM_Inventory.inv_number"
                            + " INNER JOIN HAR.dbo.IV00101 ON r.ITEMNMBR = HAR.dbo.IV00101.ITEMNMBR"
                            + " WHERE eg.Name IN(" + criteria.Groups + ")  and s.Description IS NOT null AND eg.Description IS NOT null";

                        if (criteria.Filter1 != string.Empty)
                            sqlstring = sqlstring + " AND (" + filterfieldname1 + " = @filter1)";
                        if (criteria.Filter2 != string.Empty)
                            sqlstring = sqlstring + " AND (" + filterfieldname2 + " = @filter2)";

                        //  and ((RawSalesHistory.Year > @fromyear) AND (RawSalesHistory.Year < @toyear)";

                        sqlstring = sqlstring + " and  (((r.Year = @fromyear) AND(r.Month >= @frommonth) AND(CM_Inventory.polyon <> 0)) OR ((r.Year = @toyear) "
                            + " AND(r.Month <= @tomonth) AND(CM_Inventory.polyon <> 0)) OR((r.Year = @priorfromyear) AND(r.Month >= @frommonth) AND(CM_Inventory.polyon <> 0))"
                            + " OR((r.Year = @priortoyear) AND(r.Month <= @tomonth) AND(CM_Inventory.polyon <> 0)))"
                            + " GROUP BY r.JQMCountyId) AS Data ON Data.JQMCountyId = bc.countyID"
                            + " ORDER BY bc.countyID";

                        results = db.Query<CountyResults>(sqlstring, new { @fromyear = fromyear, @frommonth = frommonth, @toyear = toyear, @tomonth = tomonth, @priorfromyear = priorfromyear, @priortoyear = priortoyear, @filter1 = criteria.Filter1, @filter2 = criteria.Filter2 }).ToList();


                        sqlstring = "SELECT Data.Rep, bc.countyID, ISNULL(DATA.total,0)  AS total"
                            + " FROM BOGODashboardsCounties bc left JOIN"
                            + " (SELECT we.LastName AS Rep,  r.JQMCountyId, SUM(ROUND((CASE WHEN ((Year > @fromyear AND Year < @toyear) OR (Year = @fromyear AND Month >= @frommonth) or  (Year = @toyear AND Month <= @tomonth)) THEN QtyTons ELSE 0 END), 2)) AS total "
                            + " FROM RawSalesHistoryByCounty r INNER JOIN webhub.dbo.employees we ON r.SLPRSNID = we.ImportKey"
                            + " INNER JOIN webhub.dbo.Segments s ON we.SegmentId = s.SegmentId"
                            + " INNER JOIN webhub.dbo.EmployeeGroups eg ON we.EmployeeGroupId = eg.EmployeeGroupId"
                            + " inner Join CM_Inventory ON r.ITEMNMBR = CM_Inventory.inv_number"
                            + " INNER JOIN HAR.dbo.IV00101 ON r.ITEMNMBR = HAR.dbo.IV00101.ITEMNMBR"
                            + " WHERE eg.Name IN(" + criteria.Groups + ")  and s.Description IS NOT null AND eg.Description IS NOT null";

                        if (criteria.Filter1 != string.Empty)
                            sqlstring = sqlstring + " AND (" + filterfieldname1 + " = @filter1)";
                        if (criteria.Filter2 != string.Empty)
                            sqlstring = sqlstring + " AND (" + filterfieldname2 + " = @filter2)";

                        //  and ((RawSalesHistory.Year > @fromyear) AND (RawSalesHistory.Year < @toyear)";

                        sqlstring = sqlstring + " and  (((r.Year = @fromyear) AND(r.Month >= @frommonth) AND(CM_Inventory.polyon <> 0)) OR ((r.Year = @toyear) "
                            + " AND(r.Month <= @tomonth) AND(CM_Inventory.polyon <> 0)) OR((r.Year = @priorfromyear) AND(r.Month >= @frommonth) AND(CM_Inventory.polyon <> 0))"
                            + " OR((r.Year = @priortoyear) AND(r.Month <= @tomonth) AND(CM_Inventory.polyon <> 0)))"
                            + " GROUP BY we.LastName, r.JQMCountyId) AS Data ON Data.JQMCountyId = bc.countyID"
                            + " ORDER BY bc.countyID, total DESC";

                        details = db.Query<CountyResults>(sqlstring, new { @fromyear = fromyear, @frommonth = frommonth, @toyear = toyear, @tomonth = tomonth, @priorfromyear = priorfromyear, @priortoyear = priortoyear, @filter1 = criteria.Filter1, @filter2 = criteria.Filter2 }).ToList();
                    }
                    else
                    {
                        // not the polyon dashboard

                        sqlstring = "SELECT bc.countryID, bc.stateID, bc.countyID, bc.county, bc.StateName as state, ISNULL(DATA.total,0)  AS total"
                            + " FROM BOGODashboardsCounties bc left JOIN"
                            + " (SELECT ess.JQMCountyId, SUM((CASE WHEN ess.invoice_date >= @currbeg AND ess.invoice_date <= @currend then " + fieldname + "  ELSE 0 end) ";  // * (CASE WHEN ess.essoptype = 4 THEN - 1 ELSE 1 END)) AS total"
                        if (fieldname.ToUpper().TrimEnd() == "BASEQTY")
                            sqlstring = sqlstring + "* (CASE WHEN ess.essoptype = 4 THEN - 1 ELSE 1 END)";
                        sqlstring = sqlstring + ") AS total "
                            + " FROM ExecSummarySales ess INNER JOIN GPSItemMaster gm ON ess.ITEMNMBR = gm.ITEMNMBR"
                            + " INNER JOIN Employees e ON ess.salesman = e.emp_id"
                            + " LEFT JOIN webhub.dbo.Segments s ON ess.Segment = s.Name"
                            + " LEFT JOIN webhub.dbo.EmployeeGroups eg ON ess.RepGroup = eg.Name"
                            + " INNER JOIN BOGODashboardsCounties bc ON ess.JQMCountyId = bc.countyID";

                        if (convertUM != "")
                            sqlstring = sqlstring + " INNER JOIN har.dbo.iv40202 iv ON gm.UOMSCHDL = iv.UOMSCHDL AND ess.BaseUofM=iv.EQUIVUOM AND iv.UOFM " + convertUM;

                        sqlstring = sqlstring+ " WHERE eg.Name IN (" + criteria.Groups + ") and ess.invoice_date >=";

                        if (currend < currbeg.AddYears(1))
                        {
                            sqlstring = sqlstring + " @priorbeg ";
                        }
                        else
                        {
                            sqlstring = sqlstring + " @currbeg ";
                        }

                        sqlstring = sqlstring + " AND ess.invoice_date <= @currend ";

                        if (!string.IsNullOrEmpty(company))
                            sqlstring = sqlstring + " AND ess.Company=@company ";

                        if (!string.IsNullOrEmpty(whereclause))
                            sqlstring = sqlstring + " AND  " + whereclause;

                        if (criteria.Filter1 != string.Empty)
                            sqlstring = sqlstring + " AND (" + filterfieldname1 + " = @filter1)";
                        if (criteria.Filter2 != string.Empty)
                            sqlstring = sqlstring + " AND (" + filterfieldname2 + " = @filter2)";


                        sqlstring = sqlstring + "  GROUP BY ess.JQMCountyId) AS Data ON Data.JQMCountyId=bc.countyID"
                            + " ORDER BY bc.countyID";
                        results = db.Query<CountyResults>(sqlstring, new { @whereclause = whereclause, @company = company, @priorbeg = priorbeg, @currbeg = currbeg, @priorend = priorend, @currend = currend, @filter1 = criteria.Filter1, @filter2 = criteria.Filter2 }).ToList();

                        sqlstring = "SELECT rep, bc.countyID, ISNULL(DATA.total,0)  AS total"
                            + " FROM BOGODashboardsCounties bc inner JOIN"
                            + " (SELECT e.emp_last AS Rep, ess.JQMCountyId, SUM((CASE WHEN ess.invoice_date >= @currbeg AND ess.invoice_date <= @currend then " + fieldname + "  ELSE 0 end) ";
                        // if using baseqty, it is always positive so need to adjust for soptype 4.  Other #'s are negative already
                        if (fieldname.ToUpper().TrimEnd() == "BASEQTY")
                            sqlstring = sqlstring + "* (CASE WHEN ess.essoptype = 4 THEN - 1 ELSE 1 END)";
                        sqlstring = sqlstring + ") AS total "
                            + " FROM ExecSummarySales ess INNER JOIN GPSItemMaster gm ON ess.ITEMNMBR = gm.ITEMNMBR"
                            + " INNER JOIN Employees e ON ess.salesman = e.emp_id"
                            + " LEFT JOIN webhub.dbo.Segments s ON ess.Segment = s.Name"
                            + " LEFT JOIN webhub.dbo.EmployeeGroups eg ON ess.RepGroup = eg.Name"
                            + " INNER JOIN BOGODashboardsCounties bc ON ess.JQMCountyId = bc.countyID";

                        if (convertUM != "")
                            sqlstring = sqlstring + " INNER JOIN har.dbo.iv40202 iv ON gm.UOMSCHDL = iv.UOMSCHDL AND ess.BaseUofM=iv.EQUIVUOM AND iv.UOFM " + convertUM;

                        sqlstring = sqlstring + " WHERE eg.Name IN (" + criteria.Groups + ") and ess.invoice_date >=";

                        if (currend < currbeg.AddYears(1))
                        {
                            sqlstring = sqlstring + " @priorbeg ";
                        }
                        else
                        {
                            sqlstring = sqlstring + " @currbeg ";
                        }

                        sqlstring = sqlstring + " AND ess.invoice_date <= @currend ";

                        if (!string.IsNullOrEmpty(company))
                            sqlstring = sqlstring + " AND ess.Company=@company ";

                        if (!string.IsNullOrEmpty(whereclause))
                            sqlstring = sqlstring + " AND  " + whereclause;

                        if (criteria.Filter1 != string.Empty)
                            sqlstring = sqlstring + " AND (" + filterfieldname1 + " = @filter1)";
                        if (criteria.Filter2 != string.Empty)
                            sqlstring = sqlstring + " AND (" + filterfieldname2 + " = @filter2)";


                        sqlstring = sqlstring + "  GROUP BY e.emp_last, ess.JQMCountyId) AS Data ON Data.JQMCountyId=bc.countyID"
                            + " ORDER BY bc.countyID, total desc";
                        details = db.Query<CountyResults>(sqlstring, new { @whereclause = whereclause, @company = company, @priorbeg = priorbeg, @currbeg = currbeg, @priorend = priorend, @currend = currend, @filter1 = criteria.Filter1, @filter2 = criteria.Filter2 }).ToList();

                        // 2/15/23 - We need to add in the sales of fertilizer that was in soil sales.

                        if (includesoil)
                        {
                            // this is a ridiculous query but needed to get data in a format to use the where clause from teh dashboard setup
                            sqlstring = "select e.emp_last AS rep, ESS.JQMCountyId AS countyID,SUM(ess.BaseQty) AS total"
                                + " FROM(SELECT sfts.*, sfts.QtyTons AS BaseQty, gm1.ITMCLSCD"
                                + " FROM SoilFertTonsSold sfts INNER JOIN GPSItemMaster gm1 ON sfts.ITEMNMBR = gm1.ITEMNMBR) AS ESS"
                                + " INNER JOIN GPSItemMaster gm ON ess.ITEMNMBR = gm.ITEMNMBR"
                                + " INNER JOIN Employees e ON ess.SLSPRSNID = e.emp_id LEFT JOIN webhub.dbo.Segments s ON e.CommGroup = s.Name"
                                + " LEFT JOIN webhub.dbo.EmployeeGroups eg ON e.CommRptGroup = eg.Name"
                                + " WHERE ess.DocDate >=";
                            if (currend < currbeg.AddYears(1))
                            {
                                sqlstring = sqlstring + " @priorbeg ";
                            }
                            else
                            {
                                sqlstring = sqlstring + " @currbeg ";
                            }

                            sqlstring = sqlstring + " AND ess.DocDate <= @currend ";
                            if (!string.IsNullOrEmpty(whereclause))
                                sqlstring = sqlstring + " AND  " + whereclause;

                            if (criteria.Filter1 != string.Empty)
                                sqlstring = sqlstring + " AND (" + filterfieldname1 + " = @filter1)";
                            if (criteria.Filter2 != string.Empty)
                                sqlstring = sqlstring + " AND (" + filterfieldname2 + " = @filter2)";

                            sqlstring = sqlstring + " and ess.itemnmbr not like 'zz%'"
                                + " GROUP BY e.emp_last, ESS.JQMCountyId ";

                            var soilresults = db.Query<CountyResults>(sqlstring, new { @whereclause = whereclause, @company = company, @priorbeg = priorbeg, @currbeg = currbeg, @priorend = priorend, @currend = currend, @filter1 = criteria.Filter1, @filter2 = criteria.Filter2 }).ToList();

                            bool foundit = false;

                            foreach (var rep in soilresults)
                            {
                                if (rep.countyID == "12099")
                                {
                                    foundit = true;
                                }

                                var resultsrep = results.Where(o =>  o.countyID==rep.countyID).FirstOrDefault();
                                if (resultsrep != null)
                                {
                                    double temptotal = Convert.ToDouble(resultsrep.total);
                                    double soiltotal = Convert.ToDouble(rep.total);

                                    resultsrep.total = (temptotal + soiltotal).ToString();
                                }
                                var detailrep = details.Where(o => o.rep == rep.rep && o.countyID == rep.countyID).FirstOrDefault();
                                if (detailrep != null)
                                {
                                    double temptotal = Convert.ToDouble(detailrep.total);
                                    double soiltotal = Convert.ToDouble(rep.total);

                                    detailrep.total = (temptotal + soiltotal).ToString();
                                }

                            }

                            details = details.OrderBy(o => o.countyID).ThenByDescending(o => o.total).ToList();

                        }






                    }

                    // fill in some rep details for the display - top 3 reps then add everyone else into all others.

                    foreach (var row in results)
                    {
                        int rowtotal = 0;
                        if (row.countyID == "12099")
                        {
                            rowtotal = 0;
                        }


                        row.realtotal = row.total;
                        double temptotal = Convert.ToDouble(row.total);
                        temptotal = temptotal + .5;
                        temptotal = Math.Truncate(temptotal);
                        if (temptotal == 0)
                            row.total = "0";
                        else
                            row.total = temptotal.ToString();
                        rowtotal = 0;
                        row.otherstotal = "0";
                        row.displaytext = "";
                        int repnmbr = 1;
                        var reps = details.Where(o => o.countyID == row.countyID && o.total != "0").OrderByDescending(x=> Convert.ToDouble(x.total)).ToList();
                        if (reps.Count != 0)
                        {
                            if (reps.Count > maxnmbr)
                                maxnmbr = reps.Count;

                            foreach (var rep in reps)
                            {
                                temptotal = Convert.ToDouble(rep.total);
                                temptotal = temptotal + .5;
                                rowtotal = rowtotal + (int)temptotal;
                                temptotal = (int)temptotal;
                                if (repnmbr == 1)
                                {
                                    row.rep1 = rep.rep;
                                    row.rep1total = temptotal.ToString();
                                    row.displaytext = rep.rep + ": " + Convert.ToDouble(rep.total.ToString()).ToString("##,##0");
                                }
                                else if (repnmbr == 2)
                                {
                                    row.rep2 = rep.rep;
                                    row.rep2total = temptotal.ToString();

                                    if (row.displaytext != "")
                                        row.displaytext = row.displaytext + Environment.NewLine;
                                    row.displaytext = row.displaytext + rep.rep + ": " + Convert.ToDouble(rep.total.ToString()).ToString("##,##0");
                                }
                                else if (repnmbr == 3)
                                {
                                    row.rep3 = rep.rep;
                                    row.rep3total = temptotal.ToString();
                                    if (row.displaytext != "")
                                        row.displaytext = row.displaytext + Environment.NewLine;
                                    row.displaytext = row.displaytext + rep.rep + ": " + Convert.ToDouble(rep.total.ToString()).ToString("##,##0");
                                }
                                else
                                {

                                    double othtotal = Convert.ToDouble(row.otherstotal);
                                    othtotal = othtotal + Convert.ToDouble(rep.total);

                                    row.otherstotal = othtotal.ToString();
                                }
                                repnmbr = repnmbr + 1;
                            }

                            if (row.otherstotal == "0")
                                row.otherstotal = "";
                            else
                            {
                                double otherstotal = Convert.ToDouble(row.otherstotal);
                                double rep1total = Convert.ToDouble(row.rep1total);
                                double rep2total = Convert.ToDouble(row.rep2total);
                                double rep3total = Convert.ToDouble(row.rep3total);
                                otherstotal = (int)otherstotal;

                                int diff =(int)( rowtotal - otherstotal - rep1total - rep2total - rep3total);
                                if(diff!=0)
                                    otherstotal = otherstotal + diff;

                                row.otherstotal = "  Others: " + otherstotal.ToString("##,##0");
                                row.displaytext = row.displaytext + Environment.NewLine;
                                row.displaytext = row.displaytext + row.otherstotal;
                            }
                        }

                        if (rowtotal != Int32.Parse(row.total))
                            row.total = rowtotal.ToString();
                    }
                    return results;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<CountyResults> SalesAnalysisMap(SATCriteria criteria)
        {
            try
            {
                using (SqlConnection db = new SqlConnection(Environment.GetEnvironmentVariable("SQLCONNSTR_CMDTAContext")))
                using (SqlConnection azdb = new SqlConnection(Environment.GetEnvironmentVariable("SQLCONNSTR_HarMgmtDBContext")))
                {

                    List<CountyResults> results = new List<CountyResults>();
                    List<CountyResults> details = new List<CountyResults>();

                    Models.Globals.SumFieldName = "Sales $";

                    // fill global ranges list
                    string sqlstring = "SELECT br.rangeID, br.range_level, range, br.range_min, br.range_max, br.range_color"
                        + " FROM BOGODashboardsRanges br"
                        + " WHERE dashboardname=@dashboard";
                    Models.Globals.MapRanges = azdb.Query<Ranges>(sqlstring, new { @dashboard = Models.Globals.Dashboard }).ToList();

                    sqlstring = "SELECT bc.countryID, bc.stateID, bc.countyID, bc.county, bc.StateName as state, ISNULL(DATA.total,0)  AS total"
                        + " FROM BOGODashboardsCounties bc left JOIN"
                        + " (SELECT ess.JQMCountyId, SUM((CASE WHEN ess.invoice_date >= @currbeg AND ess.invoice_date <= @currend then ess.ext_retail  ELSE 0 end) ) AS total"
                        + " FROM ExecSummarySales ess"
                        + " where Company = 'south' and invoice_date>=@currbeg and invoice_date<=@currend ";

                    if (criteria.ExcludeZZs == true)
                        sqlstring = sqlstring + "  AND ess.ITEMNMBR not like 'ZZ%' ";

                    if (!string.IsNullOrEmpty(criteria.LOCNCODE))
                        sqlstring = sqlstring + " AND warehouse=@locncode ";

                    if (!string.IsNullOrEmpty(criteria.Segment))
                        sqlstring = sqlstring + " AND segment=@segment ";

                    if (!string.IsNullOrEmpty(criteria.ReportGroup))
                        sqlstring = sqlstring + " AND RepGroup=@repgroup ";

                    if (!string.IsNullOrEmpty(criteria.RepID))
                        sqlstring = sqlstring + " AND salesman=@repid ";

                    if (!string.IsNullOrEmpty(criteria.Partnership))
                        sqlstring = sqlstring + " AND Partnership=@partnership ";

                    if (!string.IsNullOrEmpty(criteria.Category))
                        sqlstring = sqlstring + " AND CustCategory=@category ";

                    if (!string.IsNullOrEmpty(criteria.Item))
                        sqlstring = sqlstring + " AND ess.itemnmbr=@item ";

                    if (!string.IsNullOrEmpty(criteria.Class))
                        sqlstring = sqlstring + " AND ITMCLSCD=@class ";

                    if (!string.IsNullOrEmpty(criteria.Vendor))
                        sqlstring = sqlstring + " AND vendor=@vendor ";

                    if (!string.IsNullOrEmpty(criteria.Type))
                        sqlstring = sqlstring + " AND ProdType=@type ";

                    sqlstring = sqlstring + "  group by ess.JQMCountyId) AS Data ON Data.JQMCountyId=bc.countyID"
                        + " ORDER BY bc.countyID, total desc";

                    results = db.Query<CountyResults>(sqlstring, new {  @currbeg=criteria.BeginDate, @currend=criteria.EndDate, @segment = criteria.Segment, @repgroup = criteria.ReportGroup, @repid = criteria.RepID, @partnership = criteria.Partnership, @category = criteria.Category, @item = criteria.Item, @class = criteria.Class, @vendor = criteria.Vendor, @type = criteria.Type, @locncode = criteria.LOCNCODE }).ToList();

                    sqlstring = "SELECT rep, bc.countryID, bc.stateID, bc.countyID, bc.county, bc.StateName as state, ISNULL(DATA.total,0)  AS total"
                        + " FROM BOGODashboardsCounties bc INNER JOIN"
                        + " (SELECT e.emp_last AS Rep, ess.JQMCountyId, SUM((CASE WHEN ess.invoice_date >= @currbeg AND ess.invoice_date <= @currend then ess.ext_retail  ELSE 0 end) ) AS total"
                        + " FROM ExecSummarySales ess"
                        + " INNER JOIN Employees e ON ess.salesman = e.emp_id"
                        + " LEFT JOIN webhub.dbo.Segments s ON ess.Segment = s.Name"
                        + " LEFT JOIN webhub.dbo.EmployeeGroups eg ON ess.RepGroup = eg.Name"
                        + " where Company = 'south' and invoice_date>=@currbeg and invoice_date<=@currend ";

                    if (criteria.ExcludeZZs == true)
                        sqlstring = sqlstring + "  AND ess.ITEMNMBR not like 'ZZ%' ";

                    if (!string.IsNullOrEmpty(criteria.LOCNCODE))
                        sqlstring = sqlstring + " AND warehouse=@locncode ";

                    if (!string.IsNullOrEmpty(criteria.Segment))
                        sqlstring = sqlstring + " AND segment=@segment ";

                    if (!string.IsNullOrEmpty(criteria.ReportGroup))
                        sqlstring = sqlstring + " AND RepGroup=@repgroup ";

                    if (!string.IsNullOrEmpty(criteria.RepID))
                        sqlstring = sqlstring + " AND salesman=@repid ";

                    if (!string.IsNullOrEmpty(criteria.Partnership))
                        sqlstring = sqlstring + " AND Partnership=@partnership ";

                    if (!string.IsNullOrEmpty(criteria.Category))
                        sqlstring = sqlstring + " AND CustCategory=@category ";

                    if (!string.IsNullOrEmpty(criteria.Item))
                        sqlstring = sqlstring + " AND ess.itemnmbr=@item ";

                    if (!string.IsNullOrEmpty(criteria.Class))
                        sqlstring = sqlstring + " AND ITMCLSCD=@class ";

                    if (!string.IsNullOrEmpty(criteria.Vendor))
                        sqlstring = sqlstring + " AND vendor=@vendor ";

                    if (!string.IsNullOrEmpty(criteria.Type))
                        sqlstring = sqlstring + " AND ProdType=@type ";

                    sqlstring = sqlstring + "  group by e.emp_last, ess.JQMCountyId) AS Data ON Data.JQMCountyId=bc.countyID"
                        + " ORDER BY bc.countyID, total desc";

                    details = db.Query<CountyResults>(sqlstring, new { @currbeg = criteria.BeginDate, @currend = criteria.EndDate, @segment = criteria.Segment, @repgroup = criteria.ReportGroup, @repid = criteria.RepID, @partnership = criteria.Partnership, @category = criteria.Category, @item = criteria.Item, @class = criteria.Class, @vendor = criteria.Vendor, @type = criteria.Type, @locncode = criteria.LOCNCODE }).ToList();


                    // fill in some rep details for the display - top 3 reps then add everyone else into all others.

                    foreach (var row in results)
                    {
                        int rowtotal = 0;

                        row.realtotal = row.total;
                        double temptotal = Convert.ToDouble(row.total);
                        temptotal = temptotal + .5;
                        temptotal = Math.Truncate(temptotal);
                        if (temptotal == 0)
                            row.total = "0";
                        else
                            row.total = temptotal.ToString();
                        rowtotal = 0;
                        row.otherstotal = "0";
                        row.displaytext = "";
                        int repnmbr = 1;
                        var reps = details.Where(o => o.countyID == row.countyID && o.total != "0").ToList();
                        if (reps.Count != 0)
                        {

                            foreach (var rep in reps)
                            {
                                temptotal = Convert.ToDouble(rep.total);
                                temptotal = temptotal + .5;
                                rowtotal = rowtotal + (int)temptotal;
                                temptotal = (int)temptotal;
                                if (repnmbr == 1)
                                {
                                    row.rep1 = rep.rep;
                                    row.rep1total = temptotal.ToString();
                                    row.displaytext = rep.rep + ": " + Convert.ToDouble(rep.total.ToString()).ToString("##,##0");
                                }
                                else if (repnmbr == 2)
                                {
                                    row.rep2 = rep.rep;
                                    row.rep2total = temptotal.ToString();

                                    if (row.displaytext != "")
                                        row.displaytext = row.displaytext + Environment.NewLine;
                                    row.displaytext = row.displaytext + rep.rep + ": " + Convert.ToDouble(rep.total.ToString()).ToString("##,##0");
                                }
                                else if (repnmbr == 3)
                                {
                                    row.rep3 = rep.rep;
                                    row.rep3total = temptotal.ToString();
                                    if (row.displaytext != "")
                                        row.displaytext = row.displaytext + Environment.NewLine;
                                    row.displaytext = row.displaytext + rep.rep + ": " + Convert.ToDouble(rep.total.ToString()).ToString("##,##0");
                                }
                                else
                                {

                                    double othtotal = Convert.ToDouble(row.otherstotal);
                                    othtotal = othtotal + Convert.ToDouble(rep.total);

                                    row.otherstotal = othtotal.ToString();
                                }
                                repnmbr = repnmbr + 1;
                            }

                            if (row.otherstotal == "0")
                                row.otherstotal = "";
                            else
                            {
                                double otherstotal = Convert.ToDouble(row.otherstotal);
                                double rep1total = Convert.ToDouble(row.rep1total);
                                double rep2total = Convert.ToDouble(row.rep2total);
                                double rep3total = Convert.ToDouble(row.rep3total);
                                otherstotal = (int)otherstotal;

                                int diff = (int)(rowtotal - otherstotal - rep1total - rep2total - rep3total);
                                if (diff != 0)
                                    otherstotal = otherstotal + diff;

                                row.otherstotal = "  Others: " + otherstotal.ToString("##,##0");
                                row.displaytext = row.displaytext + Environment.NewLine;
                                row.displaytext = row.displaytext + row.otherstotal;
                            }
                        }

                        if (rowtotal != Int32.Parse(row.total))
                            row.total = rowtotal.ToString();
                    }

                    return results;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




    }
}