namespace StockScope.Services.Interfaces
{
    public interface ITechnicalIndicatorService
    {
        Task<string> GetSmaAsync(
            string symbol,
            string interval = "daily",
            int timePeriod = 20,
            string seriesType = "close",
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default
        );

        Task<string> GetEmaAsync(
            string symbol,
            string interval = "daily",
            int timePeriod = 20,
            string seriesType = "close",
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default
        );

        Task<string> GetStochAsync(
            string symbol,
            string interval = "daily",
            int? fastkperiod = 5,
            int? slowkperiod = 3,
            int? slowdperiod = 3,
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default
        );

        Task<string> GetRsiAsync(
            string symbol,
            string interval = "daily",
            int timePeriod = 14,
            string seriesType = "close",
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default
        );
    }
}

