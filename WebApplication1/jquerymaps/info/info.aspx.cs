using System;
using System.Data.SqlClient;
using System.Collections;
using Dapper;
using System.Linq;
using WebApplication1.Models;
using System.Collections.Generic;


public partial class jquerymaps_info_info : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            using (SqlConnection db = new SqlConnection("Data Source=harazuresql1.database.windows.net;Initial Catalog=HarMaps;User ID=HarMapsUserFull;Password=YouCanTunaAPianoButYouCantTunaFish!135"))
            {
                String id = ""; if (Request["id"] != null) { id = Request["id"].ToString(); }
        String info = "";
        String displaytotal = "";
        String sql = "";

                //MTW012023

                switch (id.Length)
                {
                    case 5:
                        sql = "SELECT state AS label, CurSalesTotal FROM jqm_us_statesNew WHERE stateID = '" + id.Substring(3, 2) + "'";
                        break;
                    case 11:
                        sql = "SELECT county AS label, CurSalesTotal FROM jqm_us_countiesNew WHERE countyID = '" + id.Substring(6, 5) + "'";
                        break;
                    case 18:
                        sql = "SELECT zipID+' - '+zip AS label, CurSalesTotal FROM jqm_us_zipcodesNew WHERE zipID = '" + id.Substring(13, 5) + "'";
                        break;
                }

                

                if (sql != "")
                {
                    var rd = db.Query(sql).FirstOrDefault();
                    if (rd!=null)
                    {
                        rd.Read();
                        displaytotal = rd.label.ToString() + ": " + Convert.ToDouble(rd.CurSalesTotal.ToString()).ToString("##,##0");
                        //rep = rd["name"].ToString();
                    }
                }

                //this appears under the legend box
                info += "<div class='top'>Total Sales</div>";
        info += "<div class='cnt'>" + displaytotal + "</div>";
        //info += "<div class='cnt'>" + rep + "</div>";
        

        Response.Write(info);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
}
