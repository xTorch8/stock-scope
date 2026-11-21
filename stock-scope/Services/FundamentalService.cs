using StockScope.Clients;
using StockScope.Common;
using StockScope.Services.Interfaces;

namespace StockScope.Services
{
    public class FundamentalService : IFundamentalService
    {
        private readonly IAlphaVantageClient _alphaVantageClient;

        public FundamentalService(IAlphaVantageClient alphaVantageClient)
        {
            _alphaVantageClient = alphaVantageClient;
        }

        public async Task<string> GetDividendsAsync(
            string symbol,
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default)
        {
            var validSymbol = SecurityValidator.ValidateSymbol(symbol);
            var validStart = SecurityValidator.ValidateDate(startDate, nameof(startDate));
            var validEnd = SecurityValidator.ValidateDate(endDate, nameof(endDate));
            SecurityValidator.ValidateDateRange(validStart, validEnd);
            var (validPage, validLimit) = SecurityValidator.ValidatePagination(page, limit);

            var parameters = new Dictionary<string, string>
            {
                ["symbol"] = validSymbol
            };

            var rawJson = await _alphaVantageClient.GetRawAsync("DIVIDENDS", parameters, cancellationToken);
            return JsonPaginationHelper.PaginateArray(rawJson, "data", validPage, validLimit, "ex_dividend_date,payment_date,record_date,declaration_date", validStart, validEnd);
        }

        public async Task<string> GetSplitsAsync(
            string symbol,
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default)
        {
            var validSymbol = SecurityValidator.ValidateSymbol(symbol);
            var validStart = SecurityValidator.ValidateDate(startDate, nameof(startDate));
            var validEnd = SecurityValidator.ValidateDate(endDate, nameof(endDate));
            SecurityValidator.ValidateDateRange(validStart, validEnd);
            var (validPage, validLimit) = SecurityValidator.ValidatePagination(page, limit);

            var parameters = new Dictionary<string, string>
            {
                ["symbol"] = validSymbol
            };

            var rawJson = await _alphaVantageClient.GetRawAsync("SPLITS", parameters, cancellationToken);
            return JsonPaginationHelper.PaginateArray(rawJson, "data", validPage, validLimit, "effective_date", validStart, validEnd);
        }

        public async Task<string> GetIncomeStatementAsync(
            string symbol,
            string reportType = "all",
            int limit = 5,
            CancellationToken cancellationToken = default)
        {
            var validSymbol = SecurityValidator.ValidateSymbol(symbol);
            var parameters = new Dictionary<string, string>
            {
                ["symbol"] = validSymbol
            };

            var rawJson = await _alphaVantageClient.GetRawAsync("INCOME_STATEMENT", parameters, cancellationToken);
            return JsonPaginationHelper.PaginateFinancialReports(rawJson, reportType, limit);
        }

        public async Task<string> GetBalanceSheetAsync(
            string symbol,
            string reportType = "all",
            int limit = 5,
            CancellationToken cancellationToken = default)
        {
            var validSymbol = SecurityValidator.ValidateSymbol(symbol);
            var parameters = new Dictionary<string, string>
            {
                ["symbol"] = validSymbol
            };

            var rawJson = await _alphaVantageClient.GetRawAsync("BALANCE_SHEET", parameters, cancellationToken);
            return JsonPaginationHelper.PaginateFinancialReports(rawJson, reportType, limit);
        }

        public async Task<string> GetCashFlowAsync(
            string symbol,
            string reportType = "all",
            int limit = 5,
            CancellationToken cancellationToken = default)
        {
            var validSymbol = SecurityValidator.ValidateSymbol(symbol);
            var parameters = new Dictionary<string, string>
            {
                ["symbol"] = validSymbol
            };

            var rawJson = await _alphaVantageClient.GetRawAsync("CASH_FLOW", parameters, cancellationToken);
            return JsonPaginationHelper.PaginateFinancialReports(rawJson, reportType, limit);
        }

        public async Task<string> GetSharesOutstandingAsync(
            string symbol,
            int page = 1,
            int limit = 20,
            CancellationToken cancellationToken = default)
        {
            var validSymbol = SecurityValidator.ValidateSymbol(symbol);
            var (validPage, validLimit) = SecurityValidator.ValidatePagination(page, limit);

            var parameters = new Dictionary<string, string>
            {
                ["symbol"] = validSymbol
            };

            var rawJson = await _alphaVantageClient.GetRawAsync("SHARES_OUTSTANDING", parameters, cancellationToken);
            return JsonPaginationHelper.PaginateArray(rawJson, "data", validPage, validLimit);
        }

        public async Task<string> GetEarningsAsync(
            string symbol,
            string reportType = "all",
            int limit = 10,
            CancellationToken cancellationToken = default)
        {
            var validSymbol = SecurityValidator.ValidateSymbol(symbol);
            var parameters = new Dictionary<string, string>
            {
                ["symbol"] = validSymbol
            };

            var rawJson = await _alphaVantageClient.GetRawAsync("EARNINGS", parameters, cancellationToken);
            return JsonPaginationHelper.PaginateFinancialReports(rawJson, reportType, limit);
        }

        public async Task<string> GetEarningsEstimatesAsync(
            string symbol,
            int page = 1,
            int limit = 20,
            CancellationToken cancellationToken = default)
        {
            var validSymbol = SecurityValidator.ValidateSymbol(symbol);
            var (validPage, validLimit) = SecurityValidator.ValidatePagination(page, limit);

            var parameters = new Dictionary<string, string>
            {
                ["symbol"] = validSymbol
            };

            var rawJson = await _alphaVantageClient.GetRawAsync("EARNINGS_ESTIMATES", parameters, cancellationToken);
            // Estimates usually return array under "estimates" or "data"
            var paginated = JsonPaginationHelper.PaginateArray(rawJson, "estimates", validPage, validLimit);
            if (paginated == rawJson)
            {
                paginated = JsonPaginationHelper.PaginateArray(rawJson, "data", validPage, validLimit);
            }
            return paginated;
        }

        public async Task<string> GetIpoCalendarAsync(
            string? search = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default)
        {
            var (validPage, validLimit) = SecurityValidator.ValidatePagination(page, limit);
            var rawCsv = await _alphaVantageClient.GetRawAsync("IPO_CALENDAR", null, cancellationToken);
            return JsonPaginationHelper.PaginateCsv(rawCsv, validPage, validLimit, search);
        }
    }
}

