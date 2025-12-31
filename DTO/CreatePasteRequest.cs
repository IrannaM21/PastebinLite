namespace PastebinLite.DTO;

public class CreatePasteRequest
{
    public string Content { get; set; } = string.Empty;
    public int? Ttl_Seconds { get; set; }
    public int? Max_Views { get; set; }
}
