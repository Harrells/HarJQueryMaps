using System;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;



public partial class jquerymaps_theme_feature_markercategories : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            // not really using this.  Using the markercategories.xml since there are only 2 types in this world right now.  May need this if we add more/different markers

            Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n");
            Response.Write("<jqm_markerCategories xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"../xsd/jqm_markerCategories.xsd\">\n");

            Response.Write("<category id= \"location\" enabled=\"true\" >\n");
            Response.Write("<markerStyle event=\"onMouseOut\" iconUrl=\"../images/warehouseMarker.png\" scale=\".1\" opacity=\"1\" visible=\"true\" />\n");
            Response.Write("<action event=\"onMouseOver\" target=\"infowindow\" infoWindowDiv=\"jqm_popup\" align=\"mouse, 10, 10\" />\n");
            Response.Write("<action event=\"onClick\" target=\"js\" jsFunction=\"jqmDisplayMarkerPopup\" />\n");
            Response.Write("</category>\n");
            Response.Write("<category id=\"plant\" enabled=\"true\" >\n");
            Response.Write("<markerStyle event=\"onMouseOut\" iconUrl=\"../images/plantMarker.png\" scale=\".1\" opacity=\"1\" visible=\"true\" />\n");
            Response.Write("<action event=\"onMouseOver\" target=\"infowindow\" infoWindowDiv=\"jqm_popup\" align=\"mouse, 10, 10\" />\n");
            Response.Write("<action event=\"onClick\" target=\"js\" jsFunction=\"jqmDisplayMarkerPopup\" />\n");
            Response.Write("</category>\n");

            Response.Write("</jqm_markerCategories>\n");

        }
        catch (Exception ex)
        {
            //throw ex;
        }
    }
}
