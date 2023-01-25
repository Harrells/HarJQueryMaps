using System;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;



public partial class jquerymaps_theme_feature_categories : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //these are static so they are accessible
            string fromMonth = WebApplication1.Models.Globals.FromMonth;


            Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            Response.Write("<jqm_featureCategories xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"../xsd/jqm_featureCategories.xsd\">");

            String level = "county"; if (Request["l"] != null) { level = Request["l"].ToString(); }

            //MTW012023



            //String sql = "SELECT rangeID, range_color FROM bag_ranges WHERE range_level = '" + level + "' ORDER BY rangeID ";
            //var result = db.Query(sql).ToList();

            var result = WebApplication1.Models.Globals.MapRanges.Where(o => o.range_level == level).ToList();

            foreach (var rd in result)
            {
                Response.Write("<category id=\"feature_" + rd.rangeID.ToString() + "\" enabled=\"true\" zoom=\"true\" >");
                Response.Write("<style event=\"onMouseOut\" fillColor=\"" + rd.range_color.ToString() + "\" strokeColor=\"#999999\" strokeWidth=\"1\" />");
                Response.Write("<action event=\"onMouseOver\" target=\"infowindow\" infoWindowDiv=\"jqm_popup\" align=\"mouse,10,10\" />");
                Response.Write("</category>");

                Response.Write("<category id=\"county_" + rd.rangeID.ToString() + "\" enabled=\"true\" zoom=\"false\" >");
                Response.Write("<style event=\"onMouseOut\" fillColor=\"" + rd.range_color.ToString() + "\" strokeColor=\"#999999\" strokeWidth=\"1\" />");
                Response.Write("<action event=\"onMouseOver\" target=\"infowindow\" infoWindowDiv=\"jqm_popup\" align=\"mouse,10,10\" />");
                Response.Write("<action event=\"onClick\" target=\"js\" jsFunction=\"jqmLoadState\" />");
                Response.Write("</category>");
            }

            Response.Write("<category id=\"state\" enabled=\"false\" zoom=\"true\" >");
            Response.Write("<style event=\"onMouseOut\" fillColor=\"rgba(255,255,255,0.1)\" strokeColor=\"#999999\" strokeWidth=\"1\" />");
            Response.Write("</category>");

            Response.Write("</jqm_featureCategories>");
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
