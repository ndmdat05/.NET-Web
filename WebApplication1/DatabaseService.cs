using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic; 

namespace WebShop
{
    public class DatabaseService
    {
        private readonly MySqlConnection _connection;

        public DatabaseService(MySqlConnection connection)
        {
            _connection = connection;
        }

        public bool TestConnection()
        {
            try
            {
                if (_connection.State != System.Data.ConnectionState.Open)
                    _connection.Open();

                Console.WriteLine("✅ Kết nối đến MySQL thành công!");
                _connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Kết nối đến MySQL thất bại: {ex.Message}");
                return false;
            }
        }

        public List<int> GetWishlistProductIds(string userId)
        {
            var ids = new List<int>();
            try
            {
                if (_connection.State != System.Data.ConnectionState.Open)
                    _connection.Open();

                string sql = "SELECT ProductId FROM Wishlists WHERE UserId = @userId";
                using var cmd = new MySqlCommand(sql, _connection);
                cmd.Parameters.AddWithValue("@userId", userId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ids.Add(reader.GetInt32("ProductId"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi GetWishlistProductIds: " + ex.Message);
            }
            finally { _connection.Close(); }
            return ids;
        }

        public List<dynamic> GetWishlistProducts(string userId)
        {
            var products = new List<dynamic>();
            try
            {
                if (_connection.State != System.Data.ConnectionState.Open)
                    _connection.Open();

                string sql = @"SELECT p.* FROM Products p 
                               JOIN Wishlists w ON p.Id = w.ProductId 
                               WHERE w.UserId = @userId";
                using var cmd = new MySqlCommand(sql, _connection);
                cmd.Parameters.AddWithValue("@userId", userId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new
                    {
                        Id = reader.GetInt32("Id"),
                        Name = reader.GetString("Name"),
                        Price = reader.GetDecimal("Price"),
                        Image = reader.GetString("Image") 
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi GetWishlistProducts: " + ex.Message);
            }
            finally { _connection.Close(); }
            return products;
        }

        
        public void ToggleWishlist(string userId, int productId)
        {
            try
            {
                if (_connection.State != System.Data.ConnectionState.Open)
                    _connection.Open();

                
                string checkSql = "SELECT COUNT(*) FROM Wishlists WHERE UserId = @u AND ProductId = @p";
                using var cmdCheck = new MySqlCommand(checkSql, _connection);
                cmdCheck.Parameters.AddWithValue("@u", userId);
                cmdCheck.Parameters.AddWithValue("@p", productId);
                int count = Convert.ToInt32(cmdCheck.ExecuteScalar());

                if (count > 0)
                {
                    string delSql = "DELETE FROM Wishlists WHERE UserId = @u AND ProductId = @p";
                    using var cmdDel = new MySqlCommand(delSql, _connection);
                    cmdDel.Parameters.AddWithValue("@u", userId);
                    cmdDel.Parameters.AddWithValue("@p", productId);
                    cmdDel.ExecuteNonQuery();
                }
                else
                {
                    string insSql = "INSERT INTO Wishlists (UserId, ProductId) VALUES (@u, @p)";
                    using var cmdIns = new MySqlCommand(insSql, _connection);
                    cmdIns.Parameters.AddWithValue("@u", userId);
                    cmdIns.Parameters.AddWithValue("@p", productId);
                    cmdIns.ExecuteNonQuery();
                }
            }
            finally { _connection.Close(); }
        }
    }
}