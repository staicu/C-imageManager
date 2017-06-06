using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;



namespace MediaManager
{
    class Database
    {
        SqlConnection conn;

        public Database()
        {
            conn = new SqlConnection(@"Server=tcp:alireza-abbott.database.windows.net,1433;Initial Catalog=MediaManager;
Persist Security Info=False;User ID=dbadmin;Password=Alirezal2017;MultipleActiveResultSets=False;
Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            conn.Open();
        }


    }


}
