using System;

public partial class jquerymaps_theme_us : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        String theme_str = "";

        theme_str = "<jqm_theme xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"../xsd/jqm_theme.xsd\" ";
        theme_str += "id=\"us_msas\" shapesUrl=\"../maps/us_counties/fm-us.xml\" backgroundImageUrl=\"\" ";
        theme_str += "reloadInterval=\"\" reloadFeatures=\"false\" reloadFeatureCategories=\"false\" reloadMarkers=\"false\" reloadMarkerCategories=\"false\" ";
        theme_str += "featuresUrl=\"feature_states_counties.aspx\" featureCategoriesUrl=\"feature_categories.aspx?l=county\" ";
        theme_str += "markersUrl=\"\" markerCategoriesUrl=\"\" >";
//        theme_str += "markersUrl=\"Markers.xml\" markerCategoriesUrl=\"MarkerCategories.xml\" >";

        Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        Response.Write(theme_str);
        Response.Write("</jqm_theme>");
    }
}
