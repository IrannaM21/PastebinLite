using Microsoft.AspNetCore.Mvc;
using PastebinLite.Data;

namespace PastebinLite.Controllers;

[ApiController]
[Route("api/healthz")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _db;

    public HealthController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        await _db.Database.CanConnectAsync();
        return Ok(new { ok = true });
    }
}
