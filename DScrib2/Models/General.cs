using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DScrib2.Models
{
    public class General
    {
        private string connectionString = "Data Source=(local);Initial Catalog=DScrib2;Integrated Security=true";

        /*
         * Ignores ID field of User if set.
         * 
         * Returns a User object if persistence succeeded.
         * Throws an exception if there is a problem.
         */
        public User CreateUser(User user)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO \"User\" (Email, VendorID) OUTPUT INSERTED.ID VALUES (@email, @vendorID)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@vendorID", user.VendorID);

                try
                {
                    connection.Open();
                    var newID = (int)command.ExecuteScalar();
                    user.ID = newID;
                }
                catch (Exception ex)
                {
                    if(ex.Message.StartsWith("Violation of UNIQUE KEY constraint"))
                    {
                        throw new Exception("Insertion would have violated unique key constraint.");
                        // propogate error?
                    } else
                    {
                        // Log error?
                        throw;
                    }
                }
            }

            return user;
        }

        public User GetUser(int ID)
        {
            User foundUser = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ID, Email, VendorID FROM \"User\" WHERE ID = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", ID);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        foundUser = new User()
                        {
                            ID = reader.GetInt32(0),
                            Email = reader.GetString(1),
                            VendorID = reader.GetString(2)
                        };
                    }
                }
                catch (Exception ex)
                {
                    // Log error?
                    throw;
                }
            }

            return foundUser;
        }
        /*
         * Returns null if user canot be found. Returns User otherwise.
         */
        public User GetUserByVendorID(string subject)
        {
            User foundUser = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ID, Email, VendorID FROM \"User\" WHERE VendorID = @vid";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@vid", subject);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if(reader.Read())
                    {
                        foundUser = new User()
                        {
                            ID = reader.GetInt32(0),
                            Email = reader.GetString(1),
                            VendorID = reader.GetString(2)
                        };
                    }
                } catch (Exception ex)
                {
                    // Log error?
                    throw;
                }
            }

            return foundUser;
        }
    }
}