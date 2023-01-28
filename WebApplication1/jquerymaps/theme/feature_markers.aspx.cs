using System;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;



public partial class jquerymaps_theme_feature_markers : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //these are static so they are accessible
            string fromMonth = WebApplication1.Models.Globals.FromMonth;


            Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            Response.Write("<jqm_markers xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"../xsd/jqm_markers.xsd\">");

            Response.Write("< marker id = \"14792\" category = \"location\" label = \"New York\" lat = \"40.67\" lon = \" - 73.94\" popup = \"Population: 8.175.133\" />");
            Response.Write("</jqm_markers>");

        }
        catch (Exception ex)
        {
            //throw ex;
        }
    }
}
