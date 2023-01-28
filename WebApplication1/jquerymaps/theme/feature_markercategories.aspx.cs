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

            Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            Response.Write("<jqm_markerCategories xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"../xsd/jqm_markerCategories.xsd\">");

            Response.Write("< category id = \"location\" enabled = \"true\" >");
            Response.Write("<markerStyle event=\"onMouseOut\" iconUrl=\".. / .. / images / icons / location.png\" scale=\"1\" opacity=\"1\" visible=\"true\"/>");
            Response.Write("</category>");
            Response.Write("</jqm_markerCategories>");

        }
        catch (Exception ex)
        {
            //throw ex;
        }
    }
}
