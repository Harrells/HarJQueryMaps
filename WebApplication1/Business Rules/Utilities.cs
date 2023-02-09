using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Dapper;

namespace WebApplication1.Business_Rules
{
    public class Utilities
    {
        public static void LogActivity(string username, string application, string activity)
        {
            try
            {
                using (SqlConnection db = new SqlConnection(Environment.GetEnvironmentVariable("SQLCONNSTR_BOConnection")))
                {
                    if (username == null)
                        username = "";

                    string sqlstring = "";
                    sqlstring = "Insert into AppActivity (username, application, activity) values (@username, @application, @activity)";
                    int rowsChanged = db.Execute(sqlstring, new { @username = username, @application = application, @activity = activity });
                }
            }
            catch (Exception ex)
            {

            }
        }


    }
}