namespace StockScope.Services.Interfaces
{
    public interface IStockService
    {
        Task<string> GetQuoteAsync(
            string symbol,
            CancellationToken cancellationToken = default
        );

        Task<string> GetDailyTimeSeriesAsync(
            string symbol,
            string? outputSize = "compact",
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default
        );

        Task<string> GetWeeklyTimeSeriesAsync(
            string symbol,
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default
        );

        Task<string> GetMonthlyTimeSeriesAsync(
            string symbol,
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default
        );

        Task<string> SearchSymbolAsync(
            string keywords,
            int page = 1,
            int limit = 20,
            CancellationToken cancellationToken = default
        );

        Task<string> GetMarketStatusAsync(
            CancellationToken cancellationToken = default
        );

        Task<string> GetTopGainersLosersAsync(
            string? category = null,
            int limit = 20,
            CancellationToken cancellationToken = default
        );

        Task<string> GetInsiderTransactionsAsync(
            string symbol,
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default
        );

        Task<string> GetCongressTradesAsync(
            string? symbol = null,
            string? bioguideId = null,
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default
        );

        Task<string> GetAnalyticsFixedWindowAsync(
            string symbols,
            string calculations,
            string? startDate = null,
            string? endDate = null,
            string? range = null,
            string interval = "DAILY",
            string ohlc = "close",
            CancellationToken cancellationToken = default
        );

        Task<string> GetAnalyticsSlidingWindowAsync(
            string symbols,
            string calculations,
            int windowSize = 20,
            string range = "1month",
            string interval = "DAILY",
            string ohlc = "close",
            CancellationToken cancellationToken = default
        );
    }
}
