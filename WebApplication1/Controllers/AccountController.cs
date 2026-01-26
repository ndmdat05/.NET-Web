using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using WebShop.Helpers;
using WebShop.Models;
using ZstdSharp.Unsafe;

namespace WebShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _connectionString =
           "Server=127.0.0.1;Database=DOCNET;User Id=root;Password=123456;Port=3306;";

        // 1. Đăng nhập
        // URL: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            if(HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(); // Tự động tìm Views/Account/Login.cshtml
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            // Xử lý đăng nhập ở đây (sau này)
            // Hiện tại chỉ giả lập thành công và chuyển hướng về trang chủ
            if(!ModelState.IsValid) return View(model);
            try
            {
                using var conn = new MySql.Data.MySqlClient.MySqlConnection(_connectionString);
                conn.Open();

                string sql = "SELECT id, email, password, lock, role FROM Users WHERE email = @email LIMIT 1";

                using var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@email", model.Email);

                using var reader = cmd.ExecuteReader();

                // 1. Email không tồn tại
                if (!reader.Read())
                {
                    ModelState.AddModelError("", "Sai email hoặc mật khẩu");
                    return View(model);
                }

                // 2. Tài khoản có bị khóa không
                bool isLock = Convert.ToBoolean(reader["lock"]);
                if (isLock)
                {
                    ModelState.AddModelError("", "Tài khoản đã bị khóa, vui lòng liên hệ quản trị viên");
                    return View(model);
                }

                string hashPassword = reader["password"].ToString();

                // 3. Mật khẩu sai
                if (!BCrypt.Net.BCrypt.Verify(model.Password, hashPassword))
                {
                    ModelState.AddModelError("", "Sai email hoặc mật khẩu");
                    return View(model);
                }

                // 4. Tạo session
                string userId = reader["id"].ToString();
                string role = reader["role"].ToString();

                HttpContext.Session.SetString("UserId", userId);
                HttpContext.Session.SetString("UserEmail", reader["email"].ToString());
                HttpContext.Session.SetString("UserRole", role);

                // 5. Phân quyền redirect
                if (role == "ADMIN")
                {
                    return RedirectToAction("Index", "Admin");
                }


                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                // 6. Lỗi hệ thống
                ModelState.AddModelError("", "Hệ thống đang bận, vui lòng thử lại sau");
                return View(model);
            }
        }

        // 2. Đăng ký
        // URL: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(); // Tự động tìm Views/Account/Register.cshtml
        }
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            // Xử lý đăng ký ở đây (sau này)
            // Hiện tại chỉ giả lập thành công và chuyển hướng về trang đăng nhập
            if(!ModelState.IsValid) return View(model);
            if (model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("Password", "Mật khẩu nhập lại không khớp với");
                return View(model);
            }
            if (!IsStrongPassword(model.Password))
            {
                ModelState.AddModelError("Password", "Mật khẩu phải có chữ hoa, chữ thường, số và ký tự đặc biệt");
                return View(model);
            }
            using var conn = new MySql.Data.MySqlClient.MySqlConnection(_connectionString);
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                // 1. Kiểm tra email đã tồn tại chưa
                string checkSql = "SELECT COUNT(*) FROM Users WHERE email = @email";
                using var checkCmd = new MySql.Data.MySqlClient.MySqlCommand(checkSql, conn);
                checkCmd.Parameters.AddWithValue("@email", model.Email);

                int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (exists > 0)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                    return View(model);
                }

                // 2. Hash mật khẩu
                string userId = Guid.NewGuid().ToString();
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // 3. Insert Users
                string insertUser = @"
            INSERT INTO Users(id, email, password, `lock`, role)
            VALUES(@id, @email, @password, 0, 'USER')";
                using var userCmd = new MySqlCommand(insertUser, conn, tran);
                userCmd.Parameters.AddWithValue("@id", userId);
                userCmd.Parameters.AddWithValue("@email", model.Email);
                userCmd.Parameters.AddWithValue("@password", hashedPassword);
                userCmd.ExecuteNonQuery();

                // 4. Insert user_infos
                string insertInfo = @"
            INSERT INTO user_infos(id, name)
            VALUES(@id, @name)";
                using var infoCmd = new MySqlCommand(insertInfo, conn, tran);
                infoCmd.Parameters.AddWithValue("@id", userId);
                infoCmd.Parameters.AddWithValue("@name", model.FullName);
                infoCmd.ExecuteNonQuery();
                tran.Commit();
                return RedirectToAction("Login");
            }
            catch (Exception)
            {
                // Lỗi hệ thống
                tran.Rollback();
                ModelState.AddModelError("", "Hệ thống đang bận, vui lòng thử lại sau");
                return View(model);
            }
        }
        private bool IsStrongPassword(string password)
        {
            if(string.IsNullOrEmpty(password)) return false;
            string regex = @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=!]).{8,}$";
            return Regex.IsMatch(password, regex);
        }

        // 3. Quên mật khẩu
        // URL: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(); // Tự động tìm Views/Account/ForgotPassword.cshtml
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            // Xử lý quên mật khẩu ở đây (sau này)
            // Hiện tại chỉ giả lập thành công và chuyển hướng về trang thông báo
            if(!ModelState.IsValid) return View(model);
            try
            {
                using var conn = new MySql.Data.MySqlClient.MySqlConnection(_connectionString);
                conn.Open();
                string sql = "SELECT id FROM Users WHERE email = @email LIMIT 1";
                using var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@email", model.Email);
                var userId = cmd.ExecuteScalar();
                // 1. Email không tồn tại
                if (userId == null)
                {
                    ModelState.AddModelError("", "Email không tồn tại trong hệ thống");
                    return View(model);
                }

                // 2. Tạo mật khẩu mới
                string newPassword = GenerateTempPassword();
                string hashPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                // 3. Update mật khẩu
                string updateSql = "UPDATE Users SET password = @password WHERE email = @email";
                using var updateCmd = new MySql.Data.MySqlClient.MySqlCommand(updateSql, conn);
                updateCmd.Parameters.AddWithValue("@password", hashPassword);
                updateCmd.Parameters.AddWithValue("@email", model.Email);
                updateCmd.ExecuteNonQuery();

                SendMail(model.Email, "WebShop - Mật khẩu mới",
                    $"Mật khẩu mới của bạn là: <strong>{newPassword}</strong><br/>Vui lòng đăng nhập và đổi lại mật khẩu.");

                return RedirectToAction("ForgotSuccess");

            }
            catch (Exception)
            {
                // 3. Lỗi hệ thống
                ModelState.AddModelError("", "Hệ thống đang bận, vui lòng thử lại sau");
                return View(model);
            }
        }
        private string GenerateTempPassword()
        {
            return "Tmp@0" + Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        public void SendMail(string toEmail, string subject, string body)
        {
            var fromEmail = "kurobaa123@gmail.com";
            var appPassword = "prxz feaa heyv ptcx".Replace(" ","");

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, appPassword),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail, "WebShop"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            smtp.Send(mail);
        }


        // 4. Thông báo gửi lại mật khẩu thành công
        // URL: /Account/ForgotSuccess
        public IActionResult ForgotSuccess()
        {

            return View(); // Tự động tìm Views/Account/ForgotSuccess.cshtml
        }

        // 5. Hồ sơ cá nhân
        // URL: /Account/Profile
        public IActionResult Profile()
        {
            return View(); // Tự động tìm Views/Account/Profile.cshtml
        }

        // 6. Danh sách yêu thích (Thêm mới cho đủ bộ)
        // URL: /Account/Wishlist
        public IActionResult Wishlist()
        {
            return View(); // Tự động tìm Views/Account/Wishlist.cshtml
        }

        
        
           
            // 7. DANH SÁCH ĐƠN HÀNG
            // URL: /Account/ProductStatus
           
            public IActionResult ProductStatus()
            {
            var orders = HttpContext.Session.Get<List<OrderViewModel>>("Orders");

            if (orders == null)
                orders = new List<OrderViewModel>(); 

            return View(orders); 
        }

        // 8. Chi tiết đơn hàng
        // URL: /Account/StatusDetails/5
        public IActionResult StatusDetails(string id)
        {
         var items = HttpContext.Session.Get<List<OrderItem>>("OrderItems_" + id)
                        ?? new List<OrderItem>();
                                                // Sau này sẽ dùng id để lấy dữ liệu đơn hàng từ DB
                                                     // Tự động tìm Views/Account/StatusDetails.cshtml
            return View(items); 
        }

        // 9. Đăng xuất
        public IActionResult Logout()
        {
            // Xử lý xóa session/cookie ở đây (sau này)
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home"); // Quay về trang chủ
        }
    }
}