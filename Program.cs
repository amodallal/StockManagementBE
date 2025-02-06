using Microsoft.EntityFrameworkCore;
using StockManagement.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddDbContext<StockManagementContext>();
var app = builder.Build();

app.MapControllers();
app.UseCors();
app.Run();


/*
 builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Allow only React app
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
}); 
 */