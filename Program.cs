using Microsoft.EntityFrameworkCore;
using PastebinLite.Data;
using PastebinLite.Services;

var builder = WebApplication.CreateBuilder(args);

// Enable API + MVC (required for /p/{id})
builder.Services.AddControllersWithViews();

// PostgreSQL DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres")
    )
);

// Repository DI
builder.Services.AddScoped<IPasteRepository, PostgresPasteRepository>();

// Swagger (for local testing only)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
