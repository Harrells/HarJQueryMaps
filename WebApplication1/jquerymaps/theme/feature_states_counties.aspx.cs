using System;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using Dapper;
using System.Linq;
using WebApplication1.Models;

public partial class jquerymaps_theme_feature_states_counties : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            Response.Write("<jqm_features xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"../../xsd/jqm_features.xsd\">\n");

            //MTW012023

            using (SqlConnection db = new SqlConnection("Data Source=harazuresql1.database.windows.net;Initial Catalog=HarMaps;User ID=HarMapsUserFull;Password=YouCanTunaAPianoButYouCantTunaFish!135"))
            {


                String sql = "SELECT rangeID, range_min, range_max FROM bag_ranges WHERE range_level = 'county' ORDER BY rangeID";
                List<Ranges> rangedata = db.Query<Ranges>(sql).ToList();

                sql = "SELECT c.countryID, c.stateID, c.countyID, c.county, c.CurSalesTotal AS total "
                    + "FROM jqm_us_countiesNew c "
                    + " Where c.CurSalesTotal>0 "
                    + "ORDER BY c.countyID";
                var result = db.Query(sql).ToList();
                String category_str = "";

                foreach (var rd in result)
                {
                    category_str = "";
                    Double msa_value = Convert.ToDouble(rd.total);
                    msa_value = Convert.ToInt32(msa_value);

                    var range = rangedata.Where(o => o.range_min <= msa_value && o.range_max >= msa_value).FirstOrDefault();
                    if (range != null)
                        category_str = "county_" + range.rangeID;

                    if (category_str != "")
                    {
                        Response.Write("<feature id=\"" + rd.countryID.ToString() + "_" + rd.stateID.ToString() + "_" + rd.countyID.ToString() + "\" category=\"" + category_str + "\" label=\"" + Server.HtmlEncode(rd.county.ToString()) + "\" popup=\"" + Convert.ToDouble(rd.total.ToString()).ToString("##,##0") + "\" />\n");
                    }
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
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
