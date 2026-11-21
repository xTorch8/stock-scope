using StockScope.Clients;
using StockScope.Common;
using StockScope.Services.Interfaces;

namespace StockScope.Services
{
    public class CommodityService : ICommodityService
    {
        private readonly IAlphaVantageClient _alphaVantageClient;

        public CommodityService(IAlphaVantageClient alphaVantageClient)
        {
            _alphaVantageClient = alphaVantageClient;
        }

        public async Task<string> GetGoldSilverHistoryAsync(
            string symbol,
            string interval = "daily",
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default)
        {
            var validSymbol = SecurityValidator.ValidateCommoditySymbol(symbol);
            var validInterval = SecurityValidator.ValidateInterval(interval, new[] { "daily", "weekly", "monthly" }, "daily");
            var validStart = SecurityValidator.ValidateDate(startDate, nameof(startDate));
            var validEnd = SecurityValidator.ValidateDate(endDate, nameof(endDate));
            SecurityValidator.ValidateDateRange(validStart, validEnd);
            var (validPage, validLimit) = SecurityValidator.ValidatePagination(page, limit);

            var parameters = new Dictionary<string, string>
            {
                ["symbol"] = validSymbol,
                ["interval"] = validInterval
            };

            var rawJson = await _alphaVantageClient.GetRawAsync("GOLD_SILVER_HISTORY", parameters, cancellationToken);
            return JsonPaginationHelper.PaginateArray(rawJson, "data", validPage, validLimit, "date", validStart, validEnd);
        }
    }
}

