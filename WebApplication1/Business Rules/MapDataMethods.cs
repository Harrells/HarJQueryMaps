﻿using System;
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

                    string sqlstring = "SELECT * FROM BOGODashboards b WHERE b.DashboardName=@dashboard";
                    var dashboardinfo = azdb.Query(sqlstring, new { @dashboard = criteria.Dashboard }).FirstOrDefault();
                    if (dashboardinfo == null)
                        return new List<CountyResults>();

                    // fill global ranges list
                    sqlstring = "SELECT br.rangeID, br.range_level, range, br.range_min, br.range_max, br.range_color"
                        + " FROM BOGODashboardsRanges br"
                        + " WHERE dashboardname=@dashboard";
                    Models.Globals.MapRanges = azdb.Query<Ranges>(sqlstring,new { @dashboard = Models.Globals.Dashboard }).ToList();



                    string filterfieldname1 = "";
                    string filterfieldname2 = "";
                    string dashboardname = dashboardinfo.DashboardName.TrimEnd().ToUpper();
                    string dashboardum = "Tons";
                    string company = "South";
                    string whereclause = "";
                    string fieldname = "BaseQty";

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



                    if (!string.IsNullOrEmpty(dashboardinfo.SumFieldName))
                        fieldname = dashboardinfo.SumFieldName;
                    if (!string.IsNullOrEmpty(dashboardinfo.DashboardUM))
                    {
                        Models.Globals.DashboardUM= dashboardinfo.DashboardUM;
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

                    if (dashboardname == "POLYON")
                    {
                        // polyon dashboard is slightly different

                        if (criteria.Filter1 != "" || criteria.Filter2 != "")
                        {
                            if (fromyear != toyear)
                            {
                                sqlstring = "SELECT trim(case when isnull(s.Description,'')<>'' then s.Description else s.Name end) as RepSegment, trim(case when isnull(eg.Description,'')<>'' then eg.Description else eg.Name end) as RepGroup, trim(RawSalesHistory.SLPRSNID) as SLPRSNID, we.LastName + ', ' + we.FirstName AS SalesmanName, "
                                    + " SUM(ROUND((CASE WHEN(Year > @fromyear AND Year < @toyear) OR"
                            + " (Year = @fromyear AND Month >= @frommonth) or "
                            + " (Year = @toyear AND Month <= @tomonth) THEN QtyTons ELSE 0 END), 2)) AS ActualNmbr, SUM(ROUND((CASE WHEN(Year > @priorfromyear AND Year < @priortoyear) OR"
                            + " (Year = @priorfromyear AND Month >= @frommonth) or "
                            + " (Year = @priortoyear AND Month <= @tomonth) THEN case when month =month(getdate()) then cast(day(getdate()) as float) / cast(day(EOMONTH(getdate())) as float) else 1 end * qtytons ELSE 0 END), 2)) AS PriorNmbr "
                            + " FROM RawSalesHistory"
                            + " INNER JOIN webhub.dbo.employees we ON RawSalesHistory.SLPRSNID=we.ImportKey"
                            + " INNER JOIN webhub.dbo.Segments s ON we.SegmentId = s.SegmentId"
                            + " INNER JOIN webhub.dbo.EmployeeGroups eg ON we.EmployeeGroupId = eg.EmployeeGroupId"
                            + "  INNER JOIN CM_Inventory ON RawSalesHistory.ITEMNMBR = CM_Inventory.inv_number INNER JOIN"
                            + " HAR.dbo.IV00101 ON RawSalesHistory.ITEMNMBR = HAR.dbo.IV00101.ITEMNMBR"
                            + " WHERE";
                                sqlstring = sqlstring + " ((RawSalesHistory.Year > @fromyear) AND (RawSalesHistory.Year < @toyear)";

                                if (criteria.Filter1 != string.Empty)
                                    sqlstring = sqlstring + " AND (" + filterfieldname1 + " = @filter1)";
                                if (criteria.Filter2 != string.Empty)
                                    sqlstring = sqlstring + " AND (" + filterfieldname2 + " = @filter2)";

                                sqlstring = sqlstring + ") OR ( (1=1) ";

                                if (criteria.Filter1 != string.Empty)
                                    sqlstring = sqlstring + " AND (" + filterfieldname1 + " = @filter1)";
                                if (criteria.Filter2 != string.Empty)
                                    sqlstring = sqlstring + " AND (" + filterfieldname2 + " = @filter2)";

                                sqlstring = sqlstring + " and (((RawSalesHistory.Year = @fromyear) AND(RawSalesHistory.Month >= @frommonth) AND(CM_Inventory.polyon <> 0)) OR"
                                + " ((RawSalesHistory.Year = @toyear) AND(RawSalesHistory.Month <= @tomonth) AND(CM_Inventory.polyon <> 0)) OR"
                                + " ((RawSalesHistory.Year = @priorfromyear) AND(RawSalesHistory.Month >= @frommonth) AND(CM_Inventory.polyon <> 0)) OR"
                                + " ((RawSalesHistory.Year = @priortoyear) AND(RawSalesHistory.Month <= @tomonth) AND(CM_Inventory.polyon <> 0))))"
                                + " GROUP BY trim(case when isnull(s.Description,'')<>'' then s.Description else s.Name end), trim(case when isnull(eg.Description,'')<>'' then eg.Description else eg.Name end),  RawSalesHistory.SLPRSNID, we.LastName + ', ' + we.FirstName"
                                    + " Order by we.LastName  + ', ' +  we.FirstName";

                                results = db.Query<CountyResults>(sqlstring, new { @fromyear = fromyear, @frommonth = frommonth, @toyear = toyear, @tomonth = tomonth, @priorfromyear = priorfromyear, @priortoyear = priortoyear, @filter1 = criteria.Filter1, @filter2 = criteria.Filter2 }).ToList();
                            }
                            else
                            {
                                sqlstring = "SELECT  trim(case when isnull(s.Description,'')<>'' then s.Description else s.Name end) as RepSegment, trim(case when isnull(eg.Description,'')<>'' then eg.Description else eg.Name end) as RepGroup, trim(RawSalesHistory.SLPRSNID) as SLPRSNID, we.LastName + ', ' + we.FirstName AS SalesmanName, SUM(ROUND((CASE WHEN"
                            + " (Year = @fromyear AND Month >= @frommonth) AND "
                            + " (Year = @toyear AND Month <= @tomonth) THEN QtyTons ELSE 0 END), 2)) AS ActualNmbr, SUM(ROUND((CASE WHEN"
                            + " (Year = @priorfromyear AND Month >= @frommonth) AND "
                            + " (Year = @priortoyear AND Month <= @tomonth) THEN case when month =month(getdate()) then cast(day(getdate()) as float) / cast(day(EOMONTH(getdate())) as float) else 1 end * qtytons ELSE 0 END), 2)) AS PriorNmbr"
                            + " FROM RawSalesHistory"
                            + " INNER JOIN webhub.dbo.employees we ON RawSalesHistory.SLPRSNID=we.ImportKey"
                            + " INNER JOIN webhub.dbo.Segments s ON we.SegmentId = s.SegmentId"
                            + " INNER JOIN webhub.dbo.EmployeeGroups eg ON we.EmployeeGroupId = eg.EmployeeGroupId"
                            + " inner Join CM_Inventory ON RawSalesHistory.ITEMNMBR = CM_Inventory.inv_number INNER JOIN"
                            + " HAR.dbo.IV00101 ON RawSalesHistory.ITEMNMBR = HAR.dbo.IV00101.ITEMNMBR"
                            + " WHERE  s.Description IS NOT null AND eg.Description IS NOT null and  (( (1=1) ";

                                if (criteria.Filter1 != string.Empty)
                                    sqlstring = sqlstring + " AND (" + filterfieldname1 + " = @filter1)";
                                if (criteria.Filter2 != string.Empty)
                                    sqlstring = sqlstring + " AND (" + filterfieldname2 + " = @filter2)";

                                sqlstring = sqlstring + " and (((RawSalesHistory.Year = @fromyear) AND(RawSalesHistory.Month >= @frommonth) AND(CM_Inventory.polyon <> 0)) OR"
                                + " ((RawSalesHistory.Year = @toyear) AND(RawSalesHistory.Month <= @tomonth) AND(CM_Inventory.polyon <> 0)) OR"
                                + " ((RawSalesHistory.Year = @priorfromyear) AND(RawSalesHistory.Month >= @frommonth) AND(CM_Inventory.polyon <> 0)) OR"
                                + " ((RawSalesHistory.Year = @priortoyear) AND(RawSalesHistory.Month <= @tomonth) AND(CM_Inventory.polyon <> 0)))))"
                                 + " GROUP BY trim(case when isnull(s.Description,'')<>'' then s.Description else s.Name end), trim(case when isnull(eg.Description,'')<>'' then eg.Description else eg.Name end), we.LastName + ', ' + we.FirstName, RawSalesHistory.SLPRSNID"
                                    + " Order by we.LastName  + ', ' +  we.FirstName";
                                results = db.Query<CountyResults>(sqlstring, new { @fromyear = fromyear, @frommonth = frommonth, @toyear = toyear, @tomonth = tomonth, @priorfromyear = priorfromyear, @priortoyear = priortoyear, @filter1 = criteria.Filter1, @filter2 = criteria.Filter2 }).ToList();
                            }
                        }
                        else
                        {
                            if (fromyear != toyear)
                            {
                                sqlstring = "SELECT        trim(case when isnull(s.Description,'')<>'' then s.Description else s.Name end) as RepSegment, trim(case when isnull(eg.Description,'')<>'' then eg.Description else eg.Name end) as RepGroup,  EmpComm.Emp_ID as SLSPRSNID, we.LastName + ', ' + we.FirstName AS SalesmanName , SUM(ROUND((CASE WHEN (CommYear > @fromyear AND CommYear < @fromyear) OR"
                                    + " (CommYear = @fromyear AND MonthNmbr >= @frommonth) or (CommYear = @toyear AND MonthNmbr <= @tomonth) THEN PolyonTons ELSE 0 END), 2)) AS ActualNmbr, "
                                    + " SUM(ROUND((CASE WHEN(CommYear > @priorfromyear AND CommYear < @priortoyear) OR"
                                    + " (CommYear = @priorfromyear AND MonthNmbr >= @frommonth) or (CommYear = @priortoyear AND MonthNmbr <= @tomonth) THEN CASE WHEN MonthNmbr = Month(getdate()) THEN CAST(day(getdate()) AS float) / CAST(day(EOMonth(getdate())) AS float)"
                                    + " ELSE 1 END * PolyonTons ELSE 0 END), 2)) AS PriorNmbr"
                                    + " FROM EmpCommMonths INNER JOIN"
                                    + " EmpComm ON EmpCommMonths.EmpCommYearID = EmpComm.EmpCommYearID"
                                    + " INNER JOIN webhub.dbo.employees we ON EmpComm.Emp_ID=we.ImportKey"
                                    + " INNER JOIN webhub.dbo.Segments s ON we.SegmentId = s.SegmentId"
                                    + " INNER JOIN webhub.dbo.EmployeeGroups eg ON we.EmployeeGroupId = eg.EmployeeGroupId"
                                    + " WHERE  s.Description IS NOT null AND eg.Description IS NOT null and  ((EmpComm.CommYear > @fromyear) AND(EmpComm.CommYear < @toyear) OR"
                                    + " (EmpComm.CommYear = @fromyear) AND(EmpCommMonths.MonthNmbr >= @frommonth) OR"
                                    + " (EmpComm.CommYear = @toyear) AND(EmpCommMonths.MonthNmbr <= @tomonth) OR"
                                    + " (EmpComm.CommYear = @priorfromyear) AND(EmpCommMonths.MonthNmbr >= @frommonth) OR"
                                    + " (EmpComm.CommYear = @priortoyear) AND(EmpCommMonths.MonthNmbr <= @tomonth))"
                                    + " GROUP BY trim(case when isnull(s.Description,'')<>'' then s.Description else s.Name end), trim(case when isnull(eg.Description,'')<>'' then eg.Description else eg.Name end), we.LastName + ', ' + we.FirstName, EmpComm.Emp_ID";
                                results = db.Query<CountyResults>(sqlstring, new { @fromyear = fromyear, @frommonth = frommonth, @toyear = toyear, @tomonth = tomonth, @priorfromyear = priorfromyear, @priortoyear = priortoyear, @filter1 = criteria.Filter1, @filter2 = criteria.Filter2 }).ToList();
                            }
                            else
                            {
                                sqlstring = "SELECT        trim(case when isnull(s.Description,'')<>'' then s.Description else s.Name end) as RepSegment, trim(case when isnull(eg.Description,'')<>'' then eg.Description else eg.Name end) as RepGroup,  EmpComm.Emp_ID as SLSPRSNID  , we.LastName + ', ' + we.FirstName AS SalesmanName, "
                                    + " SUM(ROUND((CASE WHEN (CommYear > @fromyear AND CommYear < @fromyear) OR"
                                    + " (CommYear = @fromyear AND MonthNmbr >= @frommonth) AND (CommYear = @toyear AND MonthNmbr <= @tomonth) THEN PolyonTons ELSE 0 END), 2)) AS ActualNmbr, "
                                    + " SUM(ROUND((CASE WHEN(CommYear > @priorfromyear AND CommYear < @priortoyear) OR"
                                    + " (CommYear = @priorfromyear AND MonthNmbr >= @frommonth) AND (CommYear = @priortoyear AND MonthNmbr <= @tomonth) THEN CASE WHEN MonthNmbr = Month(getdate()) THEN CAST(day(getdate()) AS float) / CAST(day(EOMonth(getdate())) AS float)"
                                    + " ELSE 1 END * PolyonTons ELSE 0 END), 2)) AS PriorNmbr"
                                    + " FROM EmpCommMonths INNER JOIN"
                                    + " EmpComm ON EmpCommMonths.EmpCommYearID = EmpComm.EmpCommYearID"
                                    //+ " Inner Join Employees e on e.emp_id=empcomm.emp_id "
                                    + " INNER JOIN webhub.dbo.employees we ON EmpComm.Emp_ID=we.ImportKey"
                                    + " INNER JOIN webhub.dbo.Segments s ON we.SegmentId = s.SegmentId"
                                    + " INNER JOIN webhub.dbo.EmployeeGroups eg ON we.EmployeeGroupId = eg.EmployeeGroupId"
                                    + " WHERE  s.Description IS NOT null AND eg.Description IS NOT null and  ((EmpComm.CommYear > @fromyear) AND(EmpComm.CommYear < @toyear) OR"
                                    + " (EmpComm.CommYear = @fromyear) AND(EmpCommMonths.MonthNmbr >= @frommonth) OR"
                                    + " (EmpComm.CommYear = @toyear) AND(EmpCommMonths.MonthNmbr <= @tomonth) OR"
                                    + " (EmpComm.CommYear = @priorfromyear) AND(EmpCommMonths.MonthNmbr >= @frommonth) OR"
                                    + " (EmpComm.CommYear = @priortoyear) AND(EmpCommMonths.MonthNmbr <= @tomonth))"
                                    + " GROUP BY trim(case when isnull(s.Description,'')<>'' then s.Description else s.Name end), trim(case when isnull(eg.Description,'')<>'' then eg.Description else eg.Name end),  EmpComm.Emp_ID, we.LastName + ', ' + we.FirstName"
                                    + " Order by we.LastName  + ', ' +  we.FirstName";
                                results = db.Query<CountyResults>(sqlstring, new { @fromyear = fromyear, @frommonth = frommonth, @toyear = toyear, @tomonth = tomonth, @priorfromyear = priorfromyear, @priortoyear = priortoyear, @filter1 = criteria.Filter1, @filter2 = criteria.Filter2 }).ToList();

                            }
                        }
                    }
                    else
                    {
                        // not the polyon dashboard

                        sqlstring = "SELECT bc.countryID, bc.stateID, bc.countyID, bc.county, bc.StateName as state, ISNULL(DATA.total,0) AS total"
                            + " FROM BOGODashboardsCounties bc left JOIN"
                            + " (SELECT ess.JQMCountyId, SUM((CASE WHEN ess.invoice_date >= @currbeg AND ess.invoice_date <= @currend then ess.BaseQty ELSE 0 end) * (CASE WHEN ess.essoptype = 4 THEN - 1 ELSE 1 END)) AS total"
                            + " FROM ExecSummarySales ess INNER JOIN GPSItemMaster gm ON ess.ITEMNMBR = gm.ITEMNMBR"
                            + " INNER JOIN Employees e ON ess.salesman = e.emp_id"
                            + " LEFT JOIN webhub.dbo.Segments s ON ess.Segment = s.Name"
                            + " LEFT JOIN webhub.dbo.EmployeeGroups eg ON ess.RepGroup = eg.Name"
                            + " INNER JOIN BOGODashboardsCounties bc ON ess.JQMCountyId = bc.countyID"
                            + " WHERE eg.Name IN (" + criteria.Groups + ") and ess.invoice_date >=";

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