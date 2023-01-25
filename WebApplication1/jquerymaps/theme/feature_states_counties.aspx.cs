using System;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using Dapper;
using System.Linq;
using WebApplication1.Models;

public partial class jquerymaps_theme_feature_states_counties : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            Response.Write("<jqm_features xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"../../xsd/jqm_features.xsd\">\n");

            //MTW012023

            using (SqlConnection azdb = new SqlConnection(Environment.GetEnvironmentVariable("SQLCONNSTR_HarMgmtDBContext")))
            {




                var rangedata = WebApplication1.Models.Globals.MapRanges.Where(o => o.range_level == "county").ToList();

                var result = WebApplication1.Models.Globals.Results;
                String category_str = "";
                
                foreach (var rd in result)
                {
                    category_str = "";
                    Double msa_value = Convert.ToDouble(rd.total);
                    msa_value = Convert.ToInt32(msa_value);

                    var range = rangedata.Where(o => o.range_min <= msa_value && o.range_max >= msa_value).FirstOrDefault();
                    if (range != null)
                        category_str = "county_" + range.rangeID;

                    if (category_str != "")
                    {
                        Response.Write("<feature id=\"" + rd.countryID.ToString() + "_" + rd.stateID.ToString() + "_" + rd.countyID.ToString() + "\" category=\"" + category_str + "\" label=\"" + Server.HtmlEncode(rd.county.ToString()) + "\" popup=\"" + Convert.ToDouble(rd.total.ToString()).ToString("##,##0") + "\" />\n");
                    }
                }


                string sql = "SELECT bs.countryID, bs.stateID, bs.state"
                    + " FROM BOGODashboardsStates bs"
                    + " ORDER BY bs.stateID";

                var states = azdb.Query(sql).ToList();

                foreach (var rd in states)
                {
                    Response.Write("<feature id=\"" + rd.countryID.ToString() + "_" + rd.stateID.ToString() + "\" category=\"state\" label=\"" + Server.HtmlEncode(rd.state.ToString()) + "\" popup=\"\" >\n");
                    Response.Write("<action event=\"onClick\" target=\"loadChild\" url=\"us_state.aspx?id=##id##\" />");
                    Response.Write("</feature>");
                }

            }

            Response.Write("</jqm_features>");

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
