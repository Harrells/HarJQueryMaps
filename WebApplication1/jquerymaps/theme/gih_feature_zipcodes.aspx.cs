using System;
using System.Data.SqlClient;
using System.Collections;

public partial class jquerymaps_theme_gih_feature_zipcodes : System.Web.UI.Page
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
        //String sql = "SELECT rangeID, range_min, range_max FROM bag_ranges WHERE range_level = 'zip' ORDER BY rangeID";
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

        //sql = "SELECT z.countryID, z.countyID, z.zipID, z.zip, z.CurSalesTotal AS total ";
        //sql += "FROM jqm_us_zipcodesNew z ";
        //sql += "WHERE z.countyID = '" + id + "' ";
        ////sql += "AND z.courses > 0";
        //sql += "ORDER BY z.zipID";
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

        //        Response.Write("<feature id=\"us_zip_" + rd["countyID"].ToString() + "_" + rd["zipID"].ToString() + "\" category=\"" + category_str + "\" label=\"" + Server.HtmlEncode(rd["zipID"].ToString() + " - " + rd["zip"].ToString()) + "\" popup=\"" + Convert.ToDouble(rd["total"].ToString()).ToString("##,##0") + "\" />\n");

        //    }
        //dbAdmin.sqlClose();
        //dbAdmin.dbClose();

        Response.Write("</jqm_features>");
    }
}