using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public void Index()
        {
            //ROUTING TO THE MVS's /HOME/INDEX PAGE TO TAKE ADVANTAGE OF MVC's CAPTURING OF QUERY STRING PARAMETERS INSTEAD OF GOING DIRECTLY TO THE JQUERYMAPS INDEX PAGE

            //DO NOT RIGHT KEY ON THE INDEX.HTML PAGE IN THE ROOT OF THIS APPLICATION WHEN DEBUGGING - LET THE APP ROUTE TO ITS DEFAULT SETTINGS IN ROUTECONFIG.CS
            //TO PATH OUT IN THE BROWSER URL THE FORMAT IS https://localhost:44307/?Groups=~name1^value1~name2^value2~ NOT https://localhost:44307/INDEX.HTML?Groups=~name1^value1~name2^value2~

            //getting qs paramerters here - putting them into the model so they are accessible to the rest of the app
            Models.Globals.FromMonth = Request.QueryString["FromMonth"];
            Models.Globals.ToMonth = Request.QueryString["ToMonth"];
            Models.Globals.Filter1 = Request.QueryString["Filter1"];
            Models.Globals.Filter2 = Request.QueryString["Filter2"];


            List<Models.NameValue> groupsList = new List<Models.NameValue>();
            if (Request.QueryString["Groups"] != null)
            {
                string[] groupNVPairs = Request.QueryString["Groups"].Split('~');
                foreach (var group in groupNVPairs)
                {
                    if (group != "")
                    {
                        var nvGroup = group.Split('^');
                        groupsList.Add(new Models.NameValue { Name = nvGroup[0], Value = nvGroup[1] });

                    }
                }
                Models.Globals.Groups = groupsList;

            }


            //calling jquerymaps third party page - their page calls the appropriate cs (feature_catogories.aspx.cs for example) file to display the correct map
            Response.Redirect("/index.html");
        }


    }
}