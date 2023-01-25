using System;
using System.Data.SqlClient;
using Dapper;
using System.Configuration;
using Newtonsoft.Json;
using System.Linq;

public partial class jquerymaps_info_legend : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            String level = "county"; if (Request["l"] != null) { level = Request["l"].ToString(); }
            String list = "<table class='legend'>";


            var result = WebApplication1.Models.Globals.MapRanges.Where(o => o.range_level == level).ToList();

            foreach (var rd in result)
            {
                list += "<tr><td width='20'><div class='legend_box' style='background: " + rd.range_color.ToString() + "'></div></td><td><div class='legend_label'>" + rd.range.ToString() + "</div></td></tr>";
            }

            list += "</table>";

            Response.Write(list);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //    String level = "county"; if (Request["l"] != null) { level = Request["l"].ToString(); }
    //String list = "<table class='legend'>";

    //String sql = "SELECT rangeID, range, range_color FROM bag_ranges WHERE range_level = '" + level + "' ORDER BY rangeID DESC ";
    //SqlDataReader rd = dbAdmin.sqlOpen(sql);

    //if (rd.HasRows)
    //    while (rd.Read())
    //    {
    //        list += "<tr><td width='20'><div class='legend_box' style='background: " + rd["range_color"].ToString() + "'></div></td><td><div class='legend_label'>" + rd["range"].ToString() + "</div></td></tr>";
    //    }
    //dbAdmin.sqlClose();
    //dbAdmin.dbClose();

    //list += "</table>";

    //Response.Write(list);
}
