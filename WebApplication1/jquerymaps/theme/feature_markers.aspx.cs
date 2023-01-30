using System;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Collections.Generic;
using System.Web;
using System.Collections;


public partial class jquerymaps_theme_feature_markers : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            using (SqlConnection db = new SqlConnection(Environment.GetEnvironmentVariable("SQLCONNSTR_CMDTAContext")))
            {

                string sqlstring = "SELECT l.LocationId, l.LocationType, isnull(l.MapLabel,'') as MapLabel, l.Latitude, l.Longitude, ISNULL(l.MapPopup,'') AS MapPopup"
                    + " FROM webhub.dbo.Locations l"
                    + " WHERE l.Latitude <> 0 AND l.Latitude IS NOT NULL";
                var locations = db.Query(sqlstring).ToList();


                Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n");
                Response.Write("<jqm_markers xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"../xsd/jqm_markers.xsd\">\n");

                foreach (var loc in locations)
                {
                    Response.Write("<marker id = \"" + loc.LocationId + "\" category = \"" + loc.LocationType + "\" label = \"" + loc.MapLabel + "\" lat = \"" + loc.Latitude + "\" lon = \"" + loc.Longitude + "\" popup = \"" + loc.MapPopup + "\" />\n");
                }
                Response.Write("</jqm_markers>\n");
            }
        }
        catch (Exception ex)
        {
            //throw ex;
        }
    }
}
