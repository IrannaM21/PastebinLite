using Microsoft.EntityFrameworkCore;
using PastebinLite.Data;
using PastebinLite.Services;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Controllers + MVC (required for /p/{id})
builder.Services.AddControllersWithViews();

// CORS (for browser testing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("OpenCors", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"))
);

// Repository
builder.Services.AddScoped<IPasteRepository, PostgresPasteRepository>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// REQUIRED for Render / reverse proxy
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// 🔥 REQUIRED FOR SWAGGER UI FILES
app.UseStaticFiles();

// 🔥 REQUIRED FOR ROUTING IN PROD
app.UseRouting();

// Swagger (always enabled for assignment)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PastebinLite API v1");
    c.RoutePrefix = "swagger"; // default but explicit
});

// CORS BEFORE endpoints
app.UseCors("OpenCors");

app.UseAuthorization();

// Map endpoints
app.MapControllers();

app.Run();
