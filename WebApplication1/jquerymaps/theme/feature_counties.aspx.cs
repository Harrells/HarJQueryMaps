using System;
using System.Data.SqlClient;
using System.Collections;
using Dapper;
using System.Linq;
using WebApplication1.Models;
using System.Collections.Generic;

public partial class jquerymaps_theme_feature_counties : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {


            Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            Response.Write("<jqm_features xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"../../xsd/jqm_features.xsd\">\n");

            String id = ""; if (Request["id"] != null) { id = Request["id"].ToString(); }

            //MTW012023


            var rangedata = WebApplication1.Models.Globals.MapRanges.Where(o => o.range_level == "county").ToList();


            var results = WebApplication1.Models.Globals.Results;
            String category_str = "";

            foreach (var rd in results)
            {

                category_str = "";
                Double msa_value = Convert.ToDouble(rd.total);

                var range = rangedata.Where(o => o.range_min <= msa_value && o.range_max >= msa_value).FirstOrDefault();
                if (range != null)
                    category_str = "county_" + range.rangeID;


                if (category_str != "")
                {
                    //Response.Write("<feature id=\"" + rd.countryID.ToString() + "_" + rd.stateID.ToString() + "_" + rd.countyID.ToString() + "\" category=\"" + category_str + "\" label=\"" + Server.HtmlEncode(rd.county.ToString()) + "\" popup=\"" + "Total " + WebApplication1.Models.Globals.DashboardUM + ": " + Convert.ToDouble(rd.total.ToString()).ToString("##,##0") + " " +  rd.rep1 + ": " + rd.rep1total + " " + rd.rep2 + ": " + rd.rep2total + "   " + rd.rep3 + ": " + rd.rep3total +  rd.otherstotal + "\" >\n");
                    Response.Write("<feature id=\"" + rd.countryID.ToString() + "_" + rd.stateID.ToString() + "_" + rd.countyID.ToString() + "\" category=\"" + category_str + "\" label=\"" + Server.HtmlEncode(rd.county.ToString()) + "\" popup=\"" + "Total " + WebApplication1.Models.Globals.DashboardUM + ": " + Convert.ToDouble(rd.total.ToString()).ToString("##,##0") + " " + rd.displaytext  + "\" >\n");
                    Response.Write("<action event=\"onClick\" target=\"loadChild\" url=\"us_county.aspx?id=##id##\" />");
                    Response.Write("</feature>");
                }
            }

            Response.Write("</jqm_features>");
        }
        catch (Exception ex)
        {
            //throw ex;
        }

    }
}
