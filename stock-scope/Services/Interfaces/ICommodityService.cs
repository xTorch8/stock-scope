namespace StockScope.Services.Interfaces
{
    public interface ICommodityService
    {
        Task<string> GetGoldSilverHistoryAsync(
            string symbol,
            string interval = "daily",
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default
        );
    }
}

