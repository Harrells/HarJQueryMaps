using System;
using System.Data.SqlClient;
using System.Collections;
using Dapper;
using System.Linq;

public partial class jquerymaps_theme_feature_states_counties : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        Response.Write("<jqm_features xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"../../xsd/jqm_features.xsd\">\n");

        //MTW012023

        using (SqlConnection db = new SqlConnection("Data Source=harazuresql1.database.windows.net;Initial Catalog=HarMaps;User ID=HarMapsUserFull;Password=YouCanTunaAPianoButYouCantTunaFish!135"))
        {


            ArrayList ranges = new ArrayList();
            String sql = "SELECT rangeID, range_min, range_max FROM bag_ranges WHERE range_level = 'county' ORDER BY rangeID";
            var result = db.Query(sql).ToList();

            foreach (var rd in result)
            {
                String[] element = new String[3];
                element[0] = rd.rangeID.ToString();
                element[1] = rd.range_min.ToString();
                element[2] = rd.range_max.ToString();
                ranges.Add(element);
            }

            sql = "SELECT c.countryID, c.stateID, c.countyID, c.county, c.CurSalesTotal AS total ";
            //sql = "SELECT c.countryID, c.stateID, c.countyID, c.county AS total ";
            sql += "FROM jqm_us_countiesNew c ";
            //sql += "WHERE c.CurSalesTotal > 0";
            sql += "ORDER BY c.countyID";
            result = db.Query(sql).ToList();

            foreach (var rd in result)
            {
                String category_str = "";
                Double msa_value = Convert.ToDouble(rd.total);

                foreach (string[] element in ranges)
                {
                    if (msa_value >= Convert.ToDouble(element[1]) && msa_value <= Convert.ToDouble(element[2]))
                        category_str = "county_" + element[0];
                    else
                        if (msa_value >= Convert.ToDouble(element[1]) && Convert.ToDouble(element[2]) == 0)
                        category_str = "county_" + element[0];
                }

                Response.Write("<feature id=\"" + rd.countryID.ToString() + "_" + rd.stateID.ToString() + "_" + rd.countyID.ToString() + "\" category=\"" + category_str + "\" label=\"" + Server.HtmlEncode(rd.county.ToString()) + "\" popup=\"" + Convert.ToDouble(rd.total.ToString()).ToString("##,##0") + "\" />\n");

            }


            sql = "SELECT s.countryID, s.stateID, s.state FROM jqm_us_statesNew s ORDER BY s.stateID ";
            result = db.Query(sql).ToList();

            foreach (var rd in result)
            {
                Response.Write("<feature id=\"" + rd.countryID.ToString() + "_" + rd.stateID.ToString() + "\" category=\"state\" label=\"" + Server.HtmlEncode(rd.state.ToString()) + "\" popup=\"\" >\n");
                Response.Write("<action event=\"onClick\" target=\"loadChild\" url=\"us_state.aspx?id=##id##\" />");
                Response.Write("</feature>");
            }

        }

        Response.Write("</jqm_features>");
    }
}
