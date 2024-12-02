using APIwebmoi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
// Thêm dịch vụ CORS và cấu hình
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()  // Cho phép tất cả các nguồn gốc (Android, Web, v.v.)
                   .AllowAnyMethod()  // Cho phép mọi phương thức HTTP (GET, POST, PUT, DELETE)
                   .AllowAnyHeader(); // Cho phép mọi header
        });
});


// Thêm các dịch vụ khác
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IConfigurationManager, ConfigurationManager>();
builder.Services.AddDbContext<WebNangCaoQlda2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connection")));

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

