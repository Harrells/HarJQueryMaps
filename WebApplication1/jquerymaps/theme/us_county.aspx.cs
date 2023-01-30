using System;

public partial class jquerymaps_theme_us_county : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        String id = ""; if (Request["id"] != null) { id = Request["id"].ToString().Substring(6, 5); }
        String theme_str = "";

        theme_str = "<jqm_theme xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"../xsd/jqm_theme.xsd\" ";
        theme_str += "id=\"" + id + "\" shapesUrl=\"../maps/us_zipcodes_by_county/fm-us_" + id + ".xml\" backgroundImageUrl=\"\" ";
        theme_str += "reloadInterval=\"\" reloadFeatures=\"false\" reloadFeatureCategories=\"false\" reloadMarkers=\"false\" reloadMarkerCategories=\"false\" ";
        theme_str += "featuresUrl=\"feature_zipcodes.aspx?id=" + id + "\" featureCategoriesUrl=\"feature_categories.aspx?l=zip\" ";
        //theme_str += "markersUrl=\"\" markerCategoriesUrl=\"\" >";
        theme_str += "markersUrl=\"feature_markers.aspx\" markerCategoriesUrl=\"MarkerCategories.xml\" >";

        Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        Response.Write(theme_str);
        Response.Write("</jqm_theme>");
    }
}
