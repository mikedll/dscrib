using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DScrib2.Models
{
    public class General
    {
        public void Rock()
        {
            string connectionString = "Data Source=(local);Initial Catalog=DScrib2;Integrated Security=true";

            // Provide the query string with a parameter placeholder. 
            string queryString = "SELECT * from \"User\" WHERE Id = 1";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        Console.WriteLine($"-- {reader["ID"]} {reader["Email"]} {reader["VendorID"]}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }
    }
}