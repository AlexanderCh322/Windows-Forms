using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.DataAccess
{
    public static class DataBaseHelper
    {
        private static string connectionString = @"Server=10K64-5-22-LEG\SQLEXPRESS03;Database=Предприятие_БД;Trusted_connection=true";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
