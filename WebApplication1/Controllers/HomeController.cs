using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Collections;
using Dapper;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public void Index()
        {
            using (SqlConnection db = new SqlConnection(Environment.GetEnvironmentVariable("SQLCONNSTR_CMDTAContext")))
            {

                //ROUTING TO THE MVS's /HOME/INDEX PAGE TO TAKE ADVANTAGE OF MVC's CAPTURING OF QUERY STRING PARAMETERS INSTEAD OF GOING DIRECTLY TO THE JQUERYMAPS INDEX PAGE

                //DO NOT RIGHT KEY ON THE INDEX.HTML PAGE IN THE ROOT OF THIS APPLICATION WHEN DEBUGGING - LET THE APP ROUTE TO ITS DEFAULT SETTINGS IN ROUTECONFIG.CS
                //TO PATH OUT IN THE BROWSER URL THE FORMAT IS
                //Sample URL for non polyon      https://localhost:44307/?Dashboard=Blended Fert&FromMonth=2022/01&ToMonth=2022/12&Filter1=&Filter2=&Groups=~Turf^Florida~Turf^Southeast~Turf^West~Turf^Coastal Plains~Turf^Midatlantic~Turf^Midwest~Turf^Northeast~Turf^Couth Central~Nursery^North Hort~Nursery^West Hort~Nursery^South Hort~Specialty Ag^Spec. Ag.~
                //sample URL for polyon    https://localhost:44307/?Dashboard=Polyon&FromMonth=2022/01&ToMonth=2022/12&Filter1=&Filter2=&Groups=~Turf^Florida~Turf^Southeast~Turf^West~Turf^Coastal Plains~Turf^Midatlantic~Turf^Midwest~Turf^Northeast~Turf^Couth Central~Nursery^North Hort~Nursery^West Hort~Nursery^South Hort~Specialty Ag^Spec. Ag.~
                //NOT https://localhost:44307/INDEX.HTML?Groups=~name1^value1~name2^value2~
                //https://harmapstest.azurewebsites.net/?Dashboard=Blended Fert&FromMonth=2022/01&ToMonth=2022/12&Filter1=&Filter2=&Groups=~Turf^Florida~Turf^Southeast~Turf^West~Turf^Coastal Plains~Turf^Midatlantic~Turf^Midwest~Turf^Northeast~Turf^Couth Central~Nursery^North Hort~Nursery^West Hort~Nursery^South Hort~Specialty Ag^Spec. Ag.~
                //getting qs paramerters here - putting them into the model so they are accessible to the rest of the app
                Models.Globals.Dashboard = Request.QueryString["Dashboard"];
                Models.Globals.FromMonth = Request.QueryString["FromMonth"];
                Models.Globals.ToMonth = Request.QueryString["ToMonth"];
                Models.Globals.Filter1 = Request.QueryString["Filter1"];
                Models.Globals.Filter2 = Request.QueryString["Filter2"];


                string groupsList = "";
                if (Request.QueryString["Groups"] != null)
                {
                    string[] groupNVPairs = Request.QueryString["Groups"].Split('~');
                    foreach (var group in groupNVPairs)
                    {
                        if (group != "")
                        {
                            var nvGroup = group.Split('^');
                            string repgroup = nvGroup[1];
                            string actrepgroup = nvGroup[1];

                            // need to get actual group name from db
                            string sqlstring = "SELECT eg.Name"
                                + " FROM webhub.dbo.EmployeeGroups eg"
                                + " WHERE eg.Description = @group";
                            var thisgroup = db.Query(sqlstring, new { @group = repgroup }).FirstOrDefault();
                            if (thisgroup != null)
                            {
                                actrepgroup = thisgroup.Name;
                                if (groupsList != "")
                                    groupsList = groupsList + ",";
                                groupsList = groupsList + "'" + actrepgroup + "'";
                            }

                        }
                    }
                    Models.Globals.Groups = groupsList;


                    DashboardCriteria criteria = new DashboardCriteria();
                    criteria.Dashboard = Models.Globals.Dashboard;
                    criteria.FromMonth = Models.Globals.FromMonth;
                    criteria.ToMonth = Models.Globals.ToMonth;
                    criteria.Filter1 = Models.Globals.Filter1;
                    criteria.Filter2 = Models.Globals.Filter2;
                    criteria.Groups = Models.Globals.Groups;

                    // build a global list of results by county

                    Models.Globals.Results= WebApplication1.Business_Rules.MapDataMethods.GetCountyResults(criteria);



                }


                //calling jquerymaps third party page - their page calls the appropriate cs (feature_catogories.aspx.cs for example) file to display the correct map
                Response.Redirect("/index.html");
            }
        }

    }
}