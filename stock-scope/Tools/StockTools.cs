using ModelContextProtocol.Server;
using StockScope.Services.Interfaces;
using System.ComponentModel;

namespace StockScope.Tools
{
    [McpServerToolType]
    public class StockTools
    {
        [McpServerTool]
        [Description("Retrieves the latest stock quote for a given stock ticker symbol.")]
        public static async Task<string> GetStockQuote(
            IStockService stockService,
            [Description("The stock ticker symbol, eg. AAPL, MSFT, or GOOGL.")]
            string symbol,
            CancellationToken cancellationToken)
        {
            return await stockService.GetQuoteAsync(symbol, cancellationToken);
        }

        [McpServerTool]
        [Description("Retrieves daily time series stock data (date, daily open, high, low, close, volume) with pagination and date filtering.")]
        public static async Task<string> GetDailyTimeSeries(
            IStockService stockService,
            [Description("The stock ticker symbol, eg. AAPL, MSFT.")]
            string symbol,
            [Description("Output size: 'compact' returns the latest 100 data points; 'full' returns up to 20+ years of historical data. Default is 'compact'.")]
            string? outputSize = "compact",
            [Description("Optional start date filter in 'YYYY-MM-DD' format.")]
            string? startDate = null,
            [Description("Optional end date filter in 'YYYY-MM-DD' format.")]
            string? endDate = null,
            [Description("Page number for pagination (starts at 1). Default is 1.")]
            int page = 1,
            [Description("Maximum number of records to return per page (1-100). Default is 30.")]
            int limit = 30,
            CancellationToken cancellationToken = default)
        {
            return await stockService.GetDailyTimeSeriesAsync(symbol, outputSize, startDate, endDate, page, limit, cancellationToken);
        }

        [McpServerTool]
        [Description("Retrieves weekly time series stock data (weekly open, high, low, close, volume) with pagination and date filtering.")]
        public static async Task<string> GetWeeklyTimeSeries(
            IStockService stockService,
            [Description("The stock ticker symbol, eg. AAPL, MSFT.")]
            string symbol,
            [Description("Optional start date filter in 'YYYY-MM-DD' format.")]
            string? startDate = null,
            [Description("Optional end date filter in 'YYYY-MM-DD' format.")]
            string? endDate = null,
            [Description("Page number for pagination (starts at 1). Default is 1.")]
            int page = 1,
            [Description("Maximum number of records to return per page (1-100). Default is 30.")]
            int limit = 30,
            CancellationToken cancellationToken = default)
        {
            return await stockService.GetWeeklyTimeSeriesAsync(symbol, startDate, endDate, page, limit, cancellationToken);
        }

        [McpServerTool]
        [Description("Retrieves monthly time series stock data (monthly open, high, low, close, volume) with pagination and date filtering.")]
        public static async Task<string> GetMonthlyTimeSeries(
            IStockService stockService,
            [Description("The stock ticker symbol, eg. AAPL, MSFT.")]
            string symbol,
            [Description("Optional start date filter in 'YYYY-MM-DD' format.")]
            string? startDate = null,
            [Description("Optional end date filter in 'YYYY-MM-DD' format.")]
            string? endDate = null,
            [Description("Page number for pagination (starts at 1). Default is 1.")]
            int page = 1,
            [Description("Maximum number of records to return per page (1-100). Default is 30.")]
            int limit = 30,
            CancellationToken cancellationToken = default)
        {
            return await stockService.GetMonthlyTimeSeriesAsync(symbol, startDate, endDate, page, limit, cancellationToken);
        }

        [McpServerTool]
        [Description("Searches for stock symbols, companies, match scores, and currencies based on search keywords.")]
        public static async Task<string> SearchSymbol(
            IStockService stockService,
            [Description("Search query keywords, eg. 'apple' or 'microsoft'.")]
            string keywords,
            [Description("Page number for pagination (starts at 1). Default is 1.")]
            int page = 1,
            [Description("Maximum number of results to return (1-100). Default is 20.")]
            int limit = 20,
            CancellationToken cancellationToken = default)
        {
            return await stockService.SearchSymbolAsync(keywords, page, limit, cancellationToken);
        }

        [McpServerTool]
        [Description("Retrieves the current trading status (open, closed, pre/post-market) of major global equity and financial exchanges.")]
        public static async Task<string> GetMarketStatus(
            IStockService stockService,
            CancellationToken cancellationToken = default)
        {
            return await stockService.GetMarketStatusAsync(cancellationToken);
        }

        [McpServerTool]
        [Description("Retrieves top 20 gainers, losers, and most actively traded tickers in the US market.")]
        public static async Task<string> GetTopGainersLosers(
            IStockService stockService,
            [Description("Category to filter: 'all', 'top_gainers', 'top_losers', or 'most_actively_traded'. Default is 'all'.")]
            string? category = "all",
            [Description("Maximum number of items per category (1-50). Default is 20.")]
            int limit = 20,
            CancellationToken cancellationToken = default)
        {
            return await stockService.GetTopGainersLosersAsync(category, limit, cancellationToken);
        }

        [McpServerTool]
        [Description("Retrieves insider trading transactions for a specified company symbol with pagination and date filtering.")]
        public static async Task<string> GetInsiderTransactions(
            IStockService stockService,
            [Description("The stock ticker symbol, eg. IBM, AAPL.")]
            string symbol,
            [Description("Optional start date filter in 'YYYY-MM-DD' format.")]
            string? startDate = null,
            [Description("Optional end date filter in 'YYYY-MM-DD' format.")]
            string? endDate = null,
            [Description("Page number for pagination (starts at 1). Default is 1.")]
            int page = 1,
            [Description("Maximum records to return per page (1-100). Default is 30.")]
            int limit = 30,
            CancellationToken cancellationToken = default)
        {
            return await stockService.GetInsiderTransactionsAsync(symbol, startDate, endDate, page, limit, cancellationToken);
        }

        [McpServerTool]
        [Description("Retrieves US Congressional representative stock trades disclosed under the STOCK Act, filtered by ticker or BioGuide ID.")]
        public static async Task<string> GetCongressTrades(
            IStockService stockService,
            [Description("Optional stock ticker symbol to filter trades, eg. AAPL.")]
            string? symbol = null,
            [Description("Optional Congressional member BioGuide ID, eg. S000510.")]
            string? bioguideId = null,
            [Description("Optional start date filter in 'YYYY-MM-DD' format.")]
            string? startDate = null,
            [Description("Optional end date filter in 'YYYY-MM-DD' format.")]
            string? endDate = null,
            [Description("Page number for pagination (starts at 1). Default is 1.")]
            int page = 1,
            [Description("Maximum records to return per page (1-100). Default is 30.")]
            int limit = 30,
            CancellationToken cancellationToken = default)
        {
            return await stockService.GetCongressTradesAsync(symbol, bioguideId, startDate, endDate, page, limit, cancellationToken);
        }

        [McpServerTool]
        [Description("Computes statistical calculations (e.g. MEAN, STDDEV, VARIANCE, SKEWNESS, KURTOSIS, CUMULATIVE_RETURN) over a fixed historical window.")]
        public static async Task<string> GetAnalyticsFixedWindow(
            IStockService stockService,
            [Description("Comma-separated list of ticker symbols, eg. 'AAPL,MSFT'. Up to 20 symbols.")]
            string symbols,
            [Description("Comma-separated list of metrics to calculate, eg. 'MEAN,STDDEV,CUMULATIVE_RETURN'.")]
            string calculations,
            [Description("Start date of the fixed window in 'YYYY-MM-DD' format.")]
            string? startDate = null,
            [Description("End date of the fixed window in 'YYYY-MM-DD' format.")]
            string? endDate = null,
            [Description("Alternative relative range string, eg. '1month', '1year'.")]
            string? range = null,
            [Description("Granularity of data: 'DAILY', 'WEEKLY', or 'MONTHLY'. Default is 'DAILY'.")]
            string interval = "DAILY",
            [Description("Target price field: 'open', 'high', 'low', 'close'. Default is 'close'.")]
            string ohlc = "close",
            CancellationToken cancellationToken = default)
        {
            return await stockService.GetAnalyticsFixedWindowAsync(symbols, calculations, startDate, endDate, range, interval, ohlc, cancellationToken);
        }

        [McpServerTool]
        [Description("Computes rolling statistical calculations (e.g. rolling MEAN, STDDEV) over a sliding window across a time range.")]
        public static async Task<string> GetAnalyticsSlidingWindow(
            IStockService stockService,
            [Description("Comma-separated list of ticker symbols, eg. 'AAPL,IBM'.")]
            string symbols,
            [Description("Calculations to perform, eg. 'MEAN,STDDEV'.")]
            string calculations,
            [Description("The integer sliding window period size, eg. 20 for a 20-period lookback. Default is 20.")]
            int windowSize = 20,
            [Description("Historical range lookback length, eg. '1month', '3month', '1year'. Default is '1month'.")]
            string range = "1month",
            [Description("Interval granularity: 'DAILY', 'WEEKLY'. Default is 'DAILY'.")]
            string interval = "DAILY",
            [Description("Target price field: 'open', 'high', 'low', 'close'. Default is 'close'.")]
            string ohlc = "close",
            CancellationToken cancellationToken = default)
        {
            return await stockService.GetAnalyticsSlidingWindowAsync(symbols, calculations, windowSize, range, interval, ohlc, cancellationToken);
        }
    }
}
