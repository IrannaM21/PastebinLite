using PastebinLite.Models;

namespace PastebinLite.Services;

public interface IPasteRepository
{
    Task<Paste?> GetAsync(string id);
    Task CreateAsync(Paste paste);
    Task<bool> IncrementViewsAsync(string id);
}
