using Microsoft.AspNetCore.Mvc;
using PastebinLite.DTO;
using PastebinLite.Models;
using PastebinLite.Services;

namespace PastebinLite.Controllers;

[ApiController]
[Route("api/pastes")]
public class PastesApiController : ControllerBase
{
    private readonly IPasteRepository _repo;

    public PastesApiController(IPasteRepository repo)
    {
        _repo = repo;
    }

    // POST /api/pastes
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePasteRequest request)
    {
        // 1️⃣ Validation (PDF rules)
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest(new { error = "content is required" });

        if (request.Ttl_Seconds.HasValue && request.Ttl_Seconds < 1)
            return BadRequest(new { error = "ttl_seconds must be >= 1" });

        if (request.Max_Views.HasValue && request.Max_Views < 1)
            return BadRequest(new { error = "max_views must be >= 1" });

        // 2️⃣ Create entity
        var paste = new Paste
        {
            Id = Guid.NewGuid().ToString("N"),
            Content = request.Content,
            CreatedAt = DateTimeOffset.UtcNow,
            TtlSeconds = request.Ttl_Seconds,
            MaxViews = request.Max_Views,
            Views = 0
        };

        // 3️⃣ Save to DB
        await _repo.CreateAsync(paste);

        // 4️⃣ Response (PDF format)
        return Ok(new
        {
            id = paste.Id,
            url = $"{Request.Scheme}://{Request.Host}/p/{paste.Id}"
        });
    }

    // GET /api/pastes/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        // 1️⃣ Fetch
        var paste = await _repo.GetAsync(id);
        if (paste == null)
            return NotFound();

        // 2️⃣ Current time (TEST_MODE supported)
        var now = AppTimeProvider.GetNow(Request);

        // 3️⃣ TTL check
        if (paste.TtlSeconds.HasValue &&
            paste.CreatedAt.AddSeconds(paste.TtlSeconds.Value) <= now)
            return NotFound();

        // 4️⃣ View limit check
        if (paste.MaxViews.HasValue &&
            paste.Views >= paste.MaxViews.Value)
            return NotFound();

        // 5️⃣ Increment views
        await _repo.IncrementViewsAsync(id);

        int? remainingViews = paste.MaxViews.HasValue
            ? (int?)(paste.MaxViews.Value - paste.Views - 1)
            : null;

        DateTime? expiresAt = paste.TtlSeconds.HasValue
            ? paste.CreatedAt.AddSeconds(paste.TtlSeconds.Value).UtcDateTime
            : null;

        // 6️⃣ Response (PDF format)
        return Ok(new
        {
            content = paste.Content,
            remaining_views = remainingViews,
            expires_at = expiresAt
        });
    }
}
