using ModelContextProtocol.Server;
using StockScope.Services.Interfaces;
using System.ComponentModel;

namespace StockScope.Tools
{
    [McpServerToolType]
    public class TechnicalIndicatorTools
    {
        [McpServerTool]
        [Description("Calculates Simple Moving Average (SMA) values for a stock symbol with pagination and date filtering.")]
        public static async Task<string> GetSma(
            ITechnicalIndicatorService technicalIndicatorService,
            [Description("The stock ticker symbol, eg. IBM, AAPL.")]
            string symbol,
            [Description("Time interval between data points: 'daily', 'weekly', 'monthly', '1min', '5min', '15min', '30min', '60min'. Default is 'daily'.")]
            string interval = "daily",
            [Description("Number of data points used to calculate each SMA value, eg. 20, 50, 200. Default is 20.")]
            int timePeriod = 20,
            [Description("Price series type to evaluate: 'close', 'open', 'high', 'low'. Default is 'close'.")]
            string seriesType = "close",
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
            return await technicalIndicatorService.GetSmaAsync(symbol, interval, timePeriod, seriesType, startDate, endDate, page, limit, cancellationToken);
        }

        [McpServerTool]
        [Description("Calculates Exponential Moving Average (EMA) values for a stock symbol with pagination and date filtering.")]
        public static async Task<string> GetEma(
            ITechnicalIndicatorService technicalIndicatorService,
            [Description("The stock ticker symbol, eg. IBM, MSFT.")]
            string symbol,
            [Description("Time interval between data points: 'daily', 'weekly', 'monthly', '1min', '5min', '15min', '30min', '60min'. Default is 'daily'.")]
            string interval = "daily",
            [Description("Number of data points used to calculate each EMA value, eg. 20, 50, 200. Default is 20.")]
            int timePeriod = 20,
            [Description("Price series type to evaluate: 'close', 'open', 'high', 'low'. Default is 'close'.")]
            string seriesType = "close",
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
            return await technicalIndicatorService.GetEmaAsync(symbol, interval, timePeriod, seriesType, startDate, endDate, page, limit, cancellationToken);
        }

        [McpServerTool]
        [Description("Calculates Stochastic Oscillator (%K and %D lines) for a stock symbol with pagination and date filtering.")]
        public static async Task<string> GetStoch(
            ITechnicalIndicatorService technicalIndicatorService,
            [Description("The stock ticker symbol, eg. IBM, AAPL.")]
            string symbol,
            [Description("Time interval between data points: 'daily', 'weekly', 'monthly', '1min', '5min', '15min', '30min', '60min'. Default is 'daily'.")]
            string interval = "daily",
            [Description("Fast K period lookback. Default is 5.")]
            int? fastkperiod = 5,
            [Description("Slow K period lookback. Default is 3.")]
            int? slowkperiod = 3,
            [Description("Slow D period lookback. Default is 3.")]
            int? slowdperiod = 3,
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
            return await technicalIndicatorService.GetStochAsync(symbol, interval, fastkperiod, slowkperiod, slowdperiod, startDate, endDate, page, limit, cancellationToken);
        }

        [McpServerTool]
        [Description("Calculates Relative Strength Index (RSI momentum oscillator) for a stock symbol with pagination and date filtering.")]
        public static async Task<string> GetRsi(
            ITechnicalIndicatorService technicalIndicatorService,
            [Description("The stock ticker symbol, eg. IBM, AAPL.")]
            string symbol,
            [Description("Time interval between data points: 'daily', 'weekly', 'monthly', '1min', '5min', '15min', '30min', '60min'. Default is 'daily'.")]
            string interval = "daily",
            [Description("Time period window for RSI calculation, eg. 14. Default is 14.")]
            int timePeriod = 14,
            [Description("Price series type to evaluate: 'close', 'open', 'high', 'low'. Default is 'close'.")]
            string seriesType = "close",
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
            return await technicalIndicatorService.GetRsiAsync(symbol, interval, timePeriod, seriesType, startDate, endDate, page, limit, cancellationToken);
        }
    }
}

