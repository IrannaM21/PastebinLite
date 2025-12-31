using Microsoft.EntityFrameworkCore;
using PastebinLite.Data;
using PastebinLite.Models;

namespace PastebinLite.Services;

public class PostgresPasteRepository : IPasteRepository
{
    private readonly AppDbContext _context;

    public PostgresPasteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Paste paste)
    {
        _context.Pastes.Add(paste);
        await _context.SaveChangesAsync();
    }

    public async Task<Paste?> GetAsync(string id)
    {
        return await _context.Pastes.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> IncrementViewsAsync(string id)
    {
        var paste = await GetAsync(id);
        if (paste == null) return false;

        paste.Views++;
        await _context.SaveChangesAsync();
        return true;
    }
}
