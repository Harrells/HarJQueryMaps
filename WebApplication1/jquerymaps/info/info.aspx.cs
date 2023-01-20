using System;
using System.Data.SqlClient;

public partial class jquerymaps_info_info : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        String id = ""; if (Request["id"] != null) { id = Request["id"].ToString(); }
        String info = "";
        String population = "";
        //String course = "";
        String sql = "";
        //String rep = "";

        //MTW012023

        //database dbAdmin = new database();
        //dbAdmin.dbConnect();

        //switch (id.Length)
        //{
        //    case 5:
        //        sql = "SELECT state AS label, CurSalesTotal FROM jqm_us_statesNew WHERE stateID = '" + id.Substring(3, 2) + "'";
        //        //sql = "SELECT state AS label, CurSalesTotal, rep AS name FROM jqm_us_statesNew WHERE stateID = '" + id.Substring(3, 2) + "'";
        //        break;
        //    case 11:
        //        sql = "SELECT county AS label, CurSalesTotal FROM jqm_us_countiesNew WHERE countyID = '" + id.Substring(6, 5) + "'";
        //        break;
        //    case 18:
        //        sql = "SELECT zipID+' - '+zip AS label, CurSalesTotal FROM jqm_us_zipcodesNew WHERE zipID = '" + id.Substring(13, 5) + "'";
        //        break;
        //}

        //if (sql != "")
        //{
        //    SqlDataReader rd = dbAdmin.sqlOpen(sql);

        //    if (rd.HasRows)
        //    {
        //        rd.Read();
        //        population = rd["label"].ToString() + ": " + Convert.ToDouble(rd["CurSalesTotal"].ToString()).ToString("##,##0");
        //        //rep = rd["name"].ToString();
        //    }
        //    dbAdmin.sqlClose();
        //}
        //dbAdmin.dbClose();

        //this appears under the legend box
        info += "<div class='top'>Total Sales</div>";
        info += "<div class='cnt'>" + population + "</div>";
        //info += "<div class='cnt'>" + rep + "</div>";
        

        Response.Write(info);
    }
}
