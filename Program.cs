using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using PastebinLite.Data;
using PastebinLite.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers + MVC
builder.Services.AddControllersWithViews();

// CORS (browser + Swagger)
builder.Services.AddCors(options =>
{
    options.AddPolicy("OpenCors", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// PostgreSQL (from Render environment variable)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"))
);

// Repository DI
builder.Services.AddScoped<IPasteRepository, PostgresPasteRepository>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Required for Render reverse proxy
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto
});

app.UseRouting();

// Swagger (always enabled for reviewer)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PastebinLite API v1");
    c.RoutePrefix = "swagger";
});

// CORS
app.UseCors("OpenCors");

// Authorization (future-proof)
app.UseAuthorization();

// Apply migrations BEFORE handling requests
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapControllers();

app.Run();
