using MySql.Data.MySqlClient;
using System;
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
    }
}
