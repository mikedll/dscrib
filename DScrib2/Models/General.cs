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
         * Ignores ID field of reference if set.
         * 
         * Assumes sql is remainder of INSERT INTO statement.
         * 
         * Return ID resulting from INSERT statement.
         */
        private int SaveOne(string sql, Action<SqlCommand> paramsAdder)
        {
            int newID = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("INSERT INTO " + sql, connection);
                paramsAdder(command);
                try
                {
                    connection.Open();
                    newID = (int)command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("Violation of UNIQUE KEY constraint"))
                    {
                        throw new Exception("Insertion would have violated unique key constraint.");
                        // propogate error?
                    }
                    else
                    {
                        // Log error?
                        throw;
                    }
                }
            }

            return newID;
        }

        private T GetOne<T>(string sql, Action<SqlCommand> paramsAdder, Func<SqlDataReader, T> builder)
        {
            T found = default(T);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                paramsAdder(command);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read()) found = builder(reader);
                }
                catch (Exception)
                {
                    // Log error?
                    throw;
                }
            }

            return found;
        }

        public User CreateUser(User user)
        {
            var newID = SaveOne("\"User\" (Email, VendorID) OUTPUT INSERTED.ID VALUES (@email, @vendorID)", (cmd) =>
            {
                cmd.Parameters.AddWithValue("@email", user.Email);
                cmd.Parameters.AddWithValue("@vendorID", user.VendorID);
            });
            user.ID = newID;
            return user;
        }

        public Review SaveReview(Review review)
        {
            var newID = SaveOne("Review (Text, Date, Slug, AmazonID, UserID) OUTPUT INSERTED.ID VALUES (@text, @date, @slug, @amazonID, @userID)", (cmd) =>
            {
                cmd.Parameters.AddWithValue("@text", review.Text);
                cmd.Parameters.AddWithValue("@date", review.Date);
                cmd.Parameters.AddWithValue("@slug", review.Slug);
                cmd.Parameters.AddWithValue("@amazonID", review.AmazonID);
                cmd.Parameters.AddWithValue("@userID", review.UserID);
            });
            review.ID = newID;
            return review;
        }

        public Review GetReview(string linkSlug, string productID)
        {
            return GetOne("SELECT ID, Text, Date, Slug, AmazonID, UserID FROM Review WHERE Slug = @slug AND AmazonID = @amazonID", (cmd) =>
            {
                cmd.Parameters.AddWithValue("@slug", linkSlug);
                cmd.Parameters.AddWithValue("@amazonID", productID);
            }, (reader) =>
            {
                return new Review
                {
                    ID = reader.GetInt32(0),
                    Text = reader.GetString(1),
                    Date = reader.GetDateTime(2),
                    Slug = reader.GetString(3),
                    AmazonID = reader.GetString(4),
                    UserID = reader.GetInt32(5)
                };
            });
        }

        public User GetUser(int ID)
        {
            return GetOne("SELECT ID, Email, VendorID FROM \"User\" WHERE ID = @id", (cmd) => cmd.Parameters.AddWithValue("@id", ID), (reader) => {
                return new User()
                {
                    ID = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    VendorID = reader.GetString(2)
                };
            });
        }
        /*
         * Returns null if user canot be found. Returns User otherwise.
         */
        public User GetUserByVendorID(string subject)
        {
            return GetOne("SELECT ID, Email, VendorID FROM \"User\" WHERE VendorID = @vid", (cmd) => cmd.Parameters.AddWithValue("@vid", subject), (reader) =>
            {
                return new User()
                {
                    ID = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    VendorID = reader.GetString(2)
                };
            });
        }
    }
}