using System.Security.Claims;
using JobAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS - permite Blazor WebAssembly pe porturile tale
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "https://localhost:7046", // Blazor HTTPS
                "http://localhost:5038"   // Blazor HTTP
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Aplica CORS înainte de Authorization
app.UseCors();

app.UseAuthorization();

// Map Controllers
app.MapControllers();

// Optional: redirect root la Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();