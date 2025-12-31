namespace PastebinLite.Services
{
    public static class AppTimeProvider
    {
        public static DateTime GetNow(HttpRequest request)
        {
            if (Environment.GetEnvironmentVariable("TEST_MODE") == "1" &&
                request.Headers.TryGetValue("x-test-now-ms", out var value) &&
                long.TryParse(value, out var ms))
            {
                return DateTimeOffset
                    .FromUnixTimeMilliseconds(ms)
                    .UtcDateTime;
            }

            return DateTime.UtcNow;
        }
    }
}
