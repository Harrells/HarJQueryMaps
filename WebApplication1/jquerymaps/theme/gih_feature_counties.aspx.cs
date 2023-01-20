using System;
using System.Data.SqlClient;
using System.Collections;

public partial class jquerymaps_theme_gih_feature_counties : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        Response.Write("<jqm_features xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"../../xsd/jqm_features.xsd\">\n");

        String id = ""; if (Request["id"] != null) { id = Request["id"].ToString(); }

        //MTW012023

        //database dbAdmin = new database();
        //dbAdmin.dbConnect();

        //ArrayList ranges = new ArrayList();
        //String sql = "SELECT rangeID, range_min, range_max FROM bag_ranges WHERE range_level = 'county' ORDER BY rangeID";
        //SqlDataReader rd = dbAdmin.sqlOpen(sql);

        //if (rd.HasRows)
        //    while (rd.Read())
        //    {
        //        String[] element = new String[3];
        //        element[0] = rd["rangeID"].ToString();
        //        element[1] = rd["range_min"].ToString();
        //        element[2] = rd["range_max"].ToString();
        //        ranges.Add(element);
        //    }
        //dbAdmin.sqlClose();

        //sql = "SELECT c.countryID, c.stateID, c.countyID, c.county, c.courses, c.rep, c.CurSalesTotal AS total ";
        //sql += "FROM jqm_us_countiesNew c ";
        //sql += "WHERE c.stateID = '" + id + "' ";
        ////sql += "AND c.courses > 0";
        //sql += "ORDER BY c.countyID";
        //rd = dbAdmin.sqlOpen(sql);

        //if (rd.HasRows)
        //    while (rd.Read())
        //    {

        //        String category_str = "";
        //        Double msa_value = Convert.ToDouble(rd["total"]);

        //        foreach (string[] element in ranges)
        //        {
        //            if (msa_value >= Convert.ToDouble(element[1]) && msa_value <= Convert.ToDouble(element[2]))
        //                category_str = "feature_" + element[0];
        //            else
        //                if (msa_value >= Convert.ToDouble(element[1]) && Convert.ToDouble(element[2]) == 0)
        //                    category_str = "feature_" + element[0];
        //        }

        //        // made change from county to rep so that rep shows up where county does when you zoom to state.
        //        Response.Write("<feature id=\"" + rd["countryID"].ToString() + "_" + rd["stateID"].ToString() + "_" + rd["countyID"].ToString() + "\" category=\"" + category_str + "\" label=\"" + Server.HtmlEncode(rd["county"].ToString()) + "\" popup=\"" + Convert.ToDouble(rd["total"].ToString()).ToString("##,##0") + "\" salesRep=\"" + rd["rep"].ToString() + "\" >\n");
        //        //Response.Write("<feature id=\"" + rd["countryID"].ToString() + "_" + rd["stateID"].ToString() + "_" + rd["countyID"].ToString() + "\" category=\"" + category_str + "\" label=\"" + Server.HtmlEncode(rd["county"].ToString()) + "\" popup=\"" + Convert.ToDouble(rd["total"].ToString()).ToString("##,##0") + "\" courses=\"" + Convert.ToDouble(rd["courses"].ToString()).ToString("##,##0") + "\" salesRep=\"" + rd["rep"].ToString() + "\" >\n");
        //        Response.Write("<action event=\"onClick\" target=\"loadChild\" url=\"gih_us_county.aspx?id=##id##\" />");
        //        Response.Write("</feature>");
        //    }
        //dbAdmin.sqlClose();
        //dbAdmin.dbClose();

        Response.Write("</jqm_features>");
    }
}
