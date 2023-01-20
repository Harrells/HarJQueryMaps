using System;
using System.Data.SqlClient;

public class database
{
    public SqlConnection sqlConnection;
    public SqlCommand sqlCommand;
    public SqlDataReader sqlReader;

    public database()
    {
    }

    public string dbConnectStr()
    {
        return "Password=" + System.Configuration.ConfigurationManager.AppSettings.Get("password_str") + ";Persist Security Info=True;User ID=" + System.Configuration.ConfigurationManager.AppSettings.Get("user_str") + ";Initial Catalog=" + System.Configuration.ConfigurationManager.AppSettings.Get("database_str") + ";Data Source=" + System.Configuration.ConfigurationManager.AppSettings.Get("server_str");
    }

    public void dbConnect()
    {
        string connect_str = "Password=" + System.Configuration.ConfigurationManager.AppSettings.Get("password_str") + ";Persist Security Info=True;User ID=" + System.Configuration.ConfigurationManager.AppSettings.Get("user_str") + ";Initial Catalog=" + System.Configuration.ConfigurationManager.AppSettings.Get("database_str") + ";Data Source=" + System.Configuration.ConfigurationManager.AppSettings.Get("server_str");
        sqlConnection = new SqlConnection(connect_str);
        sqlConnection.Open();
    }

    public void dbClose()
    {
        sqlConnection.Close();
    }

    public SqlDataReader sqlOpen(string sql)
    {
        sqlCommand = new SqlCommand(sql, sqlConnection);
        sqlReader = sqlCommand.ExecuteReader();

        return sqlReader;
    }

    public SqlDataReader sqlOpen(SqlCommand cmd)
    {
        cmd.Connection = sqlConnection;
        sqlReader = cmd.ExecuteReader();

        return sqlReader;
    }

    public void sqlClose()
    {
        sqlReader.Close();
        sqlCommand.Dispose();
    }
}
