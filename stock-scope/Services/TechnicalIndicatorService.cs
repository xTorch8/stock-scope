using StockScope.Clients;
using StockScope.Common;
using StockScope.Services.Interfaces;

namespace StockScope.Services
{
    public class TechnicalIndicatorService : ITechnicalIndicatorService
    {
        private static readonly string[] AllowedIntervals =
        {
            "1min", "5min", "15min", "30min", "60min", "daily", "weekly", "monthly"
        };

        private readonly IAlphaVantageClient _alphaVantageClient;

        public TechnicalIndicatorService(IAlphaVantageClient alphaVantageClient)
        {
            _alphaVantageClient = alphaVantageClient;
        }

        public async Task<string> GetSmaAsync(
            string symbol,
            string interval = "daily",
            int timePeriod = 20,
            string seriesType = "close",
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default)
        {
            var validSymbol = SecurityValidator.ValidateSymbol(symbol);
            var validInterval = SecurityValidator.ValidateInterval(interval, AllowedIntervals, "daily");
            var validSeries = SecurityValidator.ValidateSeriesType(seriesType, "close");
            var validPeriod = Math.Clamp(timePeriod, 1, 2000);
            var validStart = SecurityValidator.ValidateDate(startDate, nameof(startDate));
            var validEnd = SecurityValidator.ValidateDate(endDate, nameof(endDate));
            SecurityValidator.ValidateDateRange(validStart, validEnd);
            var (validPage, validLimit) = SecurityValidator.ValidatePagination(page, limit);

            var parameters = new Dictionary<string, string>
            {
                ["symbol"] = validSymbol,
                ["interval"] = validInterval,
                ["time_period"] = validPeriod.ToString(),
                ["series_type"] = validSeries
            };

            var rawJson = await _alphaVantageClient.GetRawAsync("SMA", parameters, cancellationToken);
            return JsonPaginationHelper.PaginateTimeSeries(rawJson, validPage, validLimit, validStart, validEnd);
        }

        public async Task<string> GetEmaAsync(
            string symbol,
            string interval = "daily",
            int timePeriod = 20,
            string seriesType = "close",
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default)
        {
            var validSymbol = SecurityValidator.ValidateSymbol(symbol);
            var validInterval = SecurityValidator.ValidateInterval(interval, AllowedIntervals, "daily");
            var validSeries = SecurityValidator.ValidateSeriesType(seriesType, "close");
            var validPeriod = Math.Clamp(timePeriod, 1, 2000);
            var validStart = SecurityValidator.ValidateDate(startDate, nameof(startDate));
            var validEnd = SecurityValidator.ValidateDate(endDate, nameof(endDate));
            SecurityValidator.ValidateDateRange(validStart, validEnd);
            var (validPage, validLimit) = SecurityValidator.ValidatePagination(page, limit);

            var parameters = new Dictionary<string, string>
            {
                ["symbol"] = validSymbol,
                ["interval"] = validInterval,
                ["time_period"] = validPeriod.ToString(),
                ["series_type"] = validSeries
            };

            var rawJson = await _alphaVantageClient.GetRawAsync("EMA", parameters, cancellationToken);
            return JsonPaginationHelper.PaginateTimeSeries(rawJson, validPage, validLimit, validStart, validEnd);
        }

        public async Task<string> GetStochAsync(
            string symbol,
            string interval = "daily",
            int? fastkperiod = 5,
            int? slowkperiod = 3,
            int? slowdperiod = 3,
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default)
        {
            var validSymbol = SecurityValidator.ValidateSymbol(symbol);
            var validInterval = SecurityValidator.ValidateInterval(interval, AllowedIntervals, "daily");
            var validStart = SecurityValidator.ValidateDate(startDate, nameof(startDate));
            var validEnd = SecurityValidator.ValidateDate(endDate, nameof(endDate));
            SecurityValidator.ValidateDateRange(validStart, validEnd);
            var (validPage, validLimit) = SecurityValidator.ValidatePagination(page, limit);

            var parameters = new Dictionary<string, string>
            {
                ["symbol"] = validSymbol,
                ["interval"] = validInterval
            };

            if (fastkperiod.HasValue)
            {
                parameters["fastkperiod"] = Math.Clamp(fastkperiod.Value, 1, 2000).ToString();
            }
            if (slowkperiod.HasValue)
            {
                parameters["slowkperiod"] = Math.Clamp(slowkperiod.Value, 1, 2000).ToString();
            }
            if (slowdperiod.HasValue)
            {
                parameters["slowdperiod"] = Math.Clamp(slowdperiod.Value, 1, 2000).ToString();
            }

            var rawJson = await _alphaVantageClient.GetRawAsync("STOCH", parameters, cancellationToken);
            return JsonPaginationHelper.PaginateTimeSeries(rawJson, validPage, validLimit, validStart, validEnd);
        }

        public async Task<string> GetRsiAsync(
            string symbol,
            string interval = "daily",
            int timePeriod = 14,
            string seriesType = "close",
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default)
        {
            var validSymbol = SecurityValidator.ValidateSymbol(symbol);
            var validInterval = SecurityValidator.ValidateInterval(interval, AllowedIntervals, "daily");
            var validSeries = SecurityValidator.ValidateSeriesType(seriesType, "close");
            var validPeriod = Math.Clamp(timePeriod, 1, 2000);
            var validStart = SecurityValidator.ValidateDate(startDate, nameof(startDate));
            var validEnd = SecurityValidator.ValidateDate(endDate, nameof(endDate));
            SecurityValidator.ValidateDateRange(validStart, validEnd);
            var (validPage, validLimit) = SecurityValidator.ValidatePagination(page, limit);

            var parameters = new Dictionary<string, string>
            {
                ["symbol"] = validSymbol,
                ["interval"] = validInterval,
                ["time_period"] = validPeriod.ToString(),
                ["series_type"] = validSeries
            };

            var rawJson = await _alphaVantageClient.GetRawAsync("RSI", parameters, cancellationToken);
            return JsonPaginationHelper.PaginateTimeSeries(rawJson, validPage, validLimit, validStart, validEnd);
        }
    }
}

