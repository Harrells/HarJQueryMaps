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
                string label = "";

                //MTW012023
                var results = WebApplication1.Models.Globals.Results;
                switch (id.Length)
                {
                    case 5:
                        results = results.Where(o => o.stateID == id.Substring(3, 2) && o.total != "0").ToList();
                        label = "state";
                        break;
                    case 11:
                        results = results.Where(o => o.countyID == id.Substring(6, 5) && o.total != "0").ToList();
                        label = "county";
                        break;
                    case 18:
                        // we aren't ever going down to zip
                        results = results.Where(o => o.zipID == id.Substring(13, 5) && o.total != "0").ToList();
                        label = "zip";
                        break;
                }
                double calctotal = 0;
                foreach (var rec in results)
                {
                    calctotal = calctotal + Convert.ToDouble(rec.realtotal);
                    if (label == "state")
                        label = rec.state;
                    else if (label == "county") 
                        label = rec.county;
                }

                calctotal = Math.Truncate(calctotal +.5);
                displaytotal =label + ": " + Convert.ToDouble(calctotal.ToString()).ToString("##,##0");

                //this appears under the legend box
                info += "<div class='top'>Total " + WebApplication1.Models.Globals.DashboardUM + "</div>";
                info += "<div class='cnt'>" + displaytotal + "</div>";
                //info += "<div class='cnt'>" + rep + "</div>";


                Response.Write(info);
            }
        }
        catch (Exception ex)
        {
            //throw ex;
        }

    }
}
