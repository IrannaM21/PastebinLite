using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using PastebinLite.Data;
using PastebinLite.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers + MVC
builder.Services.AddControllersWithViews();

// CORS (for browser & Swagger testing)
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

// REQUIRED for Render reverse proxy
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


app.UseRouting();

// Swagger AFTER routing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PastebinLite API v1");
    c.RoutePrefix = "swagger";
});

// CORS
app.UseCors("OpenCors");


// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
