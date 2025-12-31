using Microsoft.AspNetCore.Mvc;
using PastebinLite.Services;
using System.Net;

namespace PastebinLite.Controllers;

[Route("p")]
public class PasteViewController : Controller
{
    private readonly IPasteRepository _repo;

    public PasteViewController(IPasteRepository repo)
    {
        _repo = repo;
    }

    // GET /p/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> ViewPaste(string id)
    {
        // 1️⃣ Fetch paste
        var paste = await _repo.GetAsync(id);
        if (paste == null)
            return NotFound();

        // 2️⃣ Get current time (supports TEST_MODE)
        var now = AppTimeProvider.GetNow(Request);

        // 3️⃣ TTL check
        if (paste.TtlSeconds.HasValue &&
            paste.CreatedAt.AddSeconds(paste.TtlSeconds.Value) <= now)
            return NotFound();

        // 4️⃣ View-count check
        if (paste.MaxViews.HasValue &&
            paste.Views >= paste.MaxViews.Value)
            return NotFound();

        // 5️⃣ Increment view count
        await _repo.IncrementViewsAsync(id);

        // 6️⃣ Safe HTML rendering
        return Content(
            $"<pre>{WebUtility.HtmlEncode(paste.Content)}</pre>",
            "text/html; charset=utf-8"
        );
    }
}
