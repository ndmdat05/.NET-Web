using MySql.Data.MySqlClient;
using WebShop; // Namespace chứa DatabaseService

var builder = WebApplication.CreateBuilder(args);


// 1. Thêm các dịch vụ vào container (Dependency Injection)
builder.Services.AddControllersWithViews();

// Cấu hình chuỗi kết nối MySQL
// Chuỗi kết nối
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped<MySqlConnection>(sp => new MySqlConnection(connectionString));
//builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DatabaseService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
}); 
builder.Services.AddHttpContextAccessor();
//builder.Services.AddTransient<MySqlConnection>(_ => new MySqlConnection(connectionString));
//builder.Services.AddTransient<DatabaseService>();

var app = builder.Build();

// 2. Cấu hình HTTP request pipeline (Middleware)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // app.UseHsts(); // Tắt HSTS trong môi trường không phải Dev để tránh lỗi chứng chỉ
}

// app.UseHttpsRedirection(); 
//app.UseSession();
app.UseStaticFiles(); // Cho phép load file css, js, images trong wwwroot
app.UseRouting();
app.UseSession();
app.UseAuthorization();


//app.UseAuthorization();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

// Route cho khu vực Admin (Areas)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

// Route mặc định cho người dùng (Trang chủ)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// 4. Chạy ứng dụng
app.Run();