using ModelContextProtocol.Server;
using StockScope.Services.Interfaces;
using System.ComponentModel;

namespace StockScope.Tools
{
    [McpServerToolType]
    public class FundamentalTools
    {
        [McpServerTool]
        [Description("Retrieves historical and declared dividend distribution history for a company, with pagination and date filtering.")]
        public static async Task<string> GetDividends(
            IFundamentalService fundamentalService,
            [Description("The stock ticker symbol, eg. IBM, AAPL.")]
            string symbol,
            [Description("Optional start date filter in 'YYYY-MM-DD' format.")]
            string? startDate = null,
            [Description("Optional end date filter in 'YYYY-MM-DD' format.")]
            string? endDate = null,
            [Description("Page number for pagination (starts at 1). Default is 1.")]
            int page = 1,
            [Description("Maximum number of dividend events to return per page (1-100). Default is 30.")]
            int limit = 30,
            CancellationToken cancellationToken = default)
        {
            return await fundamentalService.GetDividendsAsync(symbol, startDate, endDate, page, limit, cancellationToken);
        }

        [McpServerTool]
        [Description("Retrieves historical stock split events for a company, with pagination and date filtering.")]
        public static async Task<string> GetSplits(
            IFundamentalService fundamentalService,
            [Description("The stock ticker symbol, eg. AAPL, TSLA.")]
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
            return await fundamentalService.GetSplitsAsync(symbol, startDate, endDate, page, limit, cancellationToken);
        }

        [McpServerTool]
        [Description("Retrieves annual and quarterly income statement financial reports for a company.")]
        public static async Task<string> GetIncomeStatement(
            IFundamentalService fundamentalService,
            [Description("The stock ticker symbol, eg. IBM, AAPL.")]
            string symbol,
            [Description("Report type to return: 'all', 'annual', or 'quarterly'. Default is 'all'.")]
            string reportType = "all",
            [Description("Maximum number of historical statements to return per period type (1-50). Default is 5.")]
            int limit = 5,
            CancellationToken cancellationToken = default)
        {
            return await fundamentalService.GetIncomeStatementAsync(symbol, reportType, limit, cancellationToken);
        }

        [McpServerTool]
        [Description("Retrieves annual and quarterly balance sheet financial reports for a company.")]
        public static async Task<string> GetBalanceSheet(
            IFundamentalService fundamentalService,
            [Description("The stock ticker symbol, eg. IBM, AAPL.")]
            string symbol,
            [Description("Report type to return: 'all', 'annual', or 'quarterly'. Default is 'all'.")]
            string reportType = "all",
            [Description("Maximum number of historical statements to return per period type (1-50). Default is 5.")]
            int limit = 5,
            CancellationToken cancellationToken = default)
        {
            return await fundamentalService.GetBalanceSheetAsync(symbol, reportType, limit, cancellationToken);
        }

        [McpServerTool]
        [Description("Retrieves annual and quarterly cash flow statements for a company.")]
        public static async Task<string> GetCashFlow(
            IFundamentalService fundamentalService,
            [Description("The stock ticker symbol, eg. IBM, AAPL.")]
            string symbol,
            [Description("Report type to return: 'all', 'annual', or 'quarterly'. Default is 'all'.")]
            string reportType = "all",
            [Description("Maximum number of historical statements to return per period type (1-50). Default is 5.")]
            int limit = 5,
            CancellationToken cancellationToken = default)
        {
            return await fundamentalService.GetCashFlowAsync(symbol, reportType, limit, cancellationToken);
        }

        [McpServerTool]
        [Description("Retrieves historical quarterly basic and diluted shares outstanding numbers for a company.")]
        public static async Task<string> GetSharesOutstanding(
            IFundamentalService fundamentalService,
            [Description("The stock ticker symbol, eg. MSFT, AAPL.")]
            string symbol,
            [Description("Page number for pagination (starts at 1). Default is 1.")]
            int page = 1,
            [Description("Maximum number of records to return (1-100). Default is 20.")]
            int limit = 20,
            CancellationToken cancellationToken = default)
        {
            return await fundamentalService.GetSharesOutstandingAsync(symbol, page, limit, cancellationToken);
        }

        [McpServerTool]
        [Description("Retrieves annual and quarterly earnings (EPS) performance history for a public company.")]
        public static async Task<string> GetEarnings(
            IFundamentalService fundamentalService,
            [Description("The stock ticker symbol, eg. IBM, AAPL.")]
            string symbol,
            [Description("Report period type: 'all', 'annual', or 'quarterly'. Default is 'all'.")]
            string reportType = "all",
            [Description("Maximum number of earnings entries to return (1-50). Default is 10.")]
            int limit = 10,
            CancellationToken cancellationToken = default)
        {
            return await fundamentalService.GetEarningsAsync(symbol, reportType, limit, cancellationToken);
        }

        [McpServerTool]
        [Description("Retrieves consensus earnings estimates and forward projections for a company.")]
        public static async Task<string> GetEarningsEstimates(
            IFundamentalService fundamentalService,
            [Description("The stock ticker symbol, eg. IBM, AAPL.")]
            string symbol,
            [Description("Page number for pagination (starts at 1). Default is 1.")]
            int page = 1,
            [Description("Maximum number of records to return (1-100). Default is 20.")]
            int limit = 20,
            CancellationToken cancellationToken = default)
        {
            return await fundamentalService.GetEarningsEstimatesAsync(symbol, page, limit, cancellationToken);
        }

        [McpServerTool]
        [Description("Retrieves upcoming Initial Public Offerings (IPOs) scheduled for the next 3 months with pagination and keyword filtering.")]
        public static async Task<string> GetIpoCalendar(
            IFundamentalService fundamentalService,
            [Description("Optional search filter for company name or ticker symbol.")]
            string? search = null,
            [Description("Page number for pagination (starts at 1). Default is 1.")]
            int page = 1,
            [Description("Maximum records to return per page (1-100). Default is 30.")]
            int limit = 30,
            CancellationToken cancellationToken = default)
        {
            return await fundamentalService.GetIpoCalendarAsync(search, page, limit, cancellationToken);
        }
    }
}

