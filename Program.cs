using Microsoft.EntityFrameworkCore;
using PastebinLite.Data;
using PastebinLite.Services;

var builder = WebApplication.CreateBuilder(args);

//  Controllers + MVC (required for /p/{id})
builder.Services.AddControllersWithViews();

// Enable CORS (for easy browser & Swagger testing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("OpenCors", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// PostgreSQL DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres")
    )
);

// Repository DI
builder.Services.AddScoped<IPasteRepository, PostgresPasteRepository>();

// Swagger (ENABLED for assignment review)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger UI (always enabled – reviewer friendly)
app.UseSwagger();
app.UseSwaggerUI();

// CORS must come BEFORE controllers
app.UseCors("OpenCors");

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
