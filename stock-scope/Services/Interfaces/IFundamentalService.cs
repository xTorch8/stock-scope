namespace StockScope.Services.Interfaces
{
    public interface IFundamentalService
    {
        Task<string> GetDividendsAsync(
            string symbol,
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default
        );

        Task<string> GetSplitsAsync(
            string symbol,
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default
        );

        Task<string> GetIncomeStatementAsync(
            string symbol,
            string reportType = "all",
            int limit = 5,
            CancellationToken cancellationToken = default
        );

        Task<string> GetBalanceSheetAsync(
            string symbol,
            string reportType = "all",
            int limit = 5,
            CancellationToken cancellationToken = default
        );

        Task<string> GetCashFlowAsync(
            string symbol,
            string reportType = "all",
            int limit = 5,
            CancellationToken cancellationToken = default
        );

        Task<string> GetSharesOutstandingAsync(
            string symbol,
            int page = 1,
            int limit = 20,
            CancellationToken cancellationToken = default
        );

        Task<string> GetEarningsAsync(
            string symbol,
            string reportType = "all",
            int limit = 10,
            CancellationToken cancellationToken = default
        );

        Task<string> GetEarningsEstimatesAsync(
            string symbol,
            int page = 1,
            int limit = 20,
            CancellationToken cancellationToken = default
        );

        Task<string> GetIpoCalendarAsync(
            string? search = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default
        );
    }
}

