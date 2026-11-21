using StockScope.Clients;
using StockScope.Common;
using StockScope.Services.Interfaces;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace StockScope.Services
{
    public class StockService : IStockService
    {
        private readonly IAlphaVantageClient _alphaVantageClient;

        public StockService(IAlphaVantageClient alphaVantageClient)
        {
            _alphaVantageClient = alphaVantageClient;
        }

        public async Task<string> GetQuoteAsync(string symbol, CancellationToken cancellationToken = default)
        {
            var validSymbol = SecurityValidator.ValidateSymbol(symbol);
            var parameters = new Dictionary<string, string>
            {
                ["symbol"] = validSymbol
            };

            return await _alphaVantageClient.GetRawAsync("GLOBAL_QUOTE", parameters, cancellationToken);
        }

        public async Task<string> GetDailyTimeSeriesAsync(
            string symbol,
            string? outputSize = "compact",
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

            var normalizedOutputSize = (outputSize?.Trim().ToLowerInvariant() == "full") ? "full" : "compact";

            var parameters = new Dictionary<string, string>
            {
                ["symbol"] = validSymbol,
                ["outputsize"] = normalizedOutputSize
            };

            var rawJson = await _alphaVantageClient.GetRawAsync("TIME_SERIES_DAILY", parameters, cancellationToken);
            return JsonPaginationHelper.PaginateTimeSeries(rawJson, validPage, validLimit, validStart, validEnd);
        }

        public async Task<string> GetWeeklyTimeSeriesAsync(
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

            var rawJson = await _alphaVantageClient.GetRawAsync("TIME_SERIES_WEEKLY", parameters, cancellationToken);
            return JsonPaginationHelper.PaginateTimeSeries(rawJson, validPage, validLimit, validStart, validEnd);
        }

        public async Task<string> GetMonthlyTimeSeriesAsync(
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

            var rawJson = await _alphaVantageClient.GetRawAsync("TIME_SERIES_MONTHLY", parameters, cancellationToken);
            return JsonPaginationHelper.PaginateTimeSeries(rawJson, validPage, validLimit, validStart, validEnd);
        }

        public async Task<string> SearchSymbolAsync(
            string keywords,
            int page = 1,
            int limit = 20,
            CancellationToken cancellationToken = default)
        {
            var sanitizedKeywords = SecurityValidator.SanitizeKeyword(keywords);
            var (validPage, validLimit) = SecurityValidator.ValidatePagination(page, limit);

            var parameters = new Dictionary<string, string>
            {
                ["keywords"] = sanitizedKeywords
            };

            var rawJson = await _alphaVantageClient.GetRawAsync("SYMBOL_SEARCH", parameters, cancellationToken);
            return JsonPaginationHelper.PaginateArray(rawJson, "bestMatches", validPage, validLimit);
        }

        public async Task<string> GetMarketStatusAsync(CancellationToken cancellationToken = default)
        {
            return await _alphaVantageClient.GetRawAsync("MARKET_STATUS", null, cancellationToken);
        }

        public async Task<string> GetTopGainersLosersAsync(
            string? category = null,
            int limit = 20,
            CancellationToken cancellationToken = default)
        {
            var rawJson = await _alphaVantageClient.GetRawAsync("TOP_GAINERS_LOSERS", null, cancellationToken);
            var validLimit = Math.Clamp(limit, 1, 50);

            try
            {
                var rootNode = JsonNode.Parse(rawJson);
                if (rootNode is not JsonObject rootObj)
                {
                    return rawJson;
                }

                if (rootObj.ContainsKey("Error Message") || rootObj.ContainsKey("Information") || rootObj.ContainsKey("Note"))
                {
                    return rawJson;
                }

                var normalizedCat = category?.Trim().ToLowerInvariant();
                var result = new JsonObject
                {
                    ["metadata"] = rootObj["metadata"]?.DeepClone()
                };

                void AddCategory(string key)
                {
                    if (rootObj[key] is JsonArray arr)
                    {
                        var sliced = arr.Take(validLimit).Select(x => x?.DeepClone()).ToArray();
                        result[key] = new JsonArray(sliced);
                    }
                }

                if (string.IsNullOrEmpty(normalizedCat) || normalizedCat == "all")
                {
                    AddCategory("top_gainers");
                    AddCategory("top_losers");
                    AddCategory("most_actively_traded");
                }
                else if (normalizedCat is "top_gainers" or "gainers")
                {
                    AddCategory("top_gainers");
                }
                else if (normalizedCat is "top_losers" or "losers")
                {
                    AddCategory("top_losers");
                }
                else if (normalizedCat is "most_actively_traded" or "active")
                {
                    AddCategory("most_actively_traded");
                }
                else
                {
                    AddCategory("top_gainers");
                    AddCategory("top_losers");
                    AddCategory("most_actively_traded");
                }

                return result.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                return rawJson;
            }
        }

        public async Task<string> GetInsiderTransactionsAsync(
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

            var rawJson = await _alphaVantageClient.GetRawAsync("INSIDER_TRANSACTIONS", parameters, cancellationToken);
            return JsonPaginationHelper.PaginateArray(rawJson, "data", validPage, validLimit, "transaction_date", validStart, validEnd);
        }

        public async Task<string> GetCongressTradesAsync(
            string? symbol = null,
            string? bioguideId = null,
            string? startDate = null,
            string? endDate = null,
            int page = 1,
            int limit = 30,
            CancellationToken cancellationToken = default)
        {
            string? validSymbol = null;
            if (!string.IsNullOrWhiteSpace(symbol))
            {
                validSymbol = SecurityValidator.ValidateSymbol(symbol);
            }

            string? validBioguide = null;
            if (!string.IsNullOrWhiteSpace(bioguideId))
            {
                validBioguide = SecurityValidator.ValidateBioguideId(bioguideId);
            }

            var validStart = SecurityValidator.ValidateDate(startDate, nameof(startDate));
            var validEnd = SecurityValidator.ValidateDate(endDate, nameof(endDate));
            SecurityValidator.ValidateDateRange(validStart, validEnd);
            var (validPage, validLimit) = SecurityValidator.ValidatePagination(page, limit);

            var parameters = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(validSymbol))
            {
                parameters["symbol"] = validSymbol;
            }
            if (!string.IsNullOrEmpty(validBioguide))
            {
                parameters["bioguide_id"] = validBioguide;
            }

            var rawJson = await _alphaVantageClient.GetRawAsync("CONGRESS_TRADES", parameters, cancellationToken);
            return JsonPaginationHelper.PaginateArray(rawJson, "data", validPage, validLimit, "transaction_date", validStart, validEnd);
        }

        public async Task<string> GetAnalyticsFixedWindowAsync(
            string symbols,
            string calculations,
            string? startDate = null,
            string? endDate = null,
            string? range = null,
            string interval = "DAILY",
            string ohlc = "close",
            CancellationToken cancellationToken = default)
        {
            var validSymbols = SecurityValidator.ValidateSymbols(symbols);
            var validCalculations = SecurityValidator.ValidateCalculations(calculations);
            var validStart = SecurityValidator.ValidateDate(startDate, nameof(startDate));
            var validEnd = SecurityValidator.ValidateDate(endDate, nameof(endDate));
            SecurityValidator.ValidateDateRange(validStart, validEnd);
            var validRange = SecurityValidator.ValidateAnalyticsRange(range);

            var validInterval = SecurityValidator.ValidateInterval(interval, new[] { "daily", "weekly", "monthly" }, "daily").ToUpperInvariant();
            var validOhlc = SecurityValidator.ValidateSeriesType(ohlc, "close");

            var parameters = new Dictionary<string, string>
            {
                ["SYMBOLS"] = validSymbols,
                ["CALCULATIONS"] = validCalculations,
                ["INTERVAL"] = validInterval,
                ["OHLC"] = validOhlc
            };

            if (!string.IsNullOrEmpty(validStart))
            {
                parameters["START_DATE"] = validStart;
            }
            if (!string.IsNullOrEmpty(validEnd))
            {
                parameters["END_DATE"] = validEnd;
            }
            if (!string.IsNullOrEmpty(validRange))
            {
                parameters["RANGE"] = validRange;
            }

            return await _alphaVantageClient.GetRawAsync("ANALYTICS_FIXED_WINDOW", parameters, cancellationToken);
        }

        public async Task<string> GetAnalyticsSlidingWindowAsync(
            string symbols,
            string calculations,
            int windowSize = 20,
            string range = "1month",
            string interval = "DAILY",
            string ohlc = "close",
            CancellationToken cancellationToken = default)
        {
            var validSymbols = SecurityValidator.ValidateSymbols(symbols);
            var validCalculations = SecurityValidator.ValidateCalculations(calculations);
            var validRange = SecurityValidator.ValidateAnalyticsRange(range) ?? "1month";

            var validWindow = Math.Clamp(windowSize, 1, 500);
            var validInterval = SecurityValidator.ValidateInterval(interval, new[] { "daily", "weekly", "monthly" }, "daily").ToUpperInvariant();
            var validOhlc = SecurityValidator.ValidateSeriesType(ohlc, "close");

            var parameters = new Dictionary<string, string>
            {
                ["SYMBOLS"] = validSymbols,
                ["CALCULATIONS"] = validCalculations,
                ["WINDOW_SIZE"] = validWindow.ToString(),
                ["RANGE"] = validRange,
                ["INTERVAL"] = validInterval,
                ["OHLC"] = validOhlc
            };

            return await _alphaVantageClient.GetRawAsync("ANALYTICS_SLIDING_WINDOW", parameters, cancellationToken);
        }
    }
}
