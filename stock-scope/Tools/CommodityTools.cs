using ModelContextProtocol.Server;
using StockScope.Services.Interfaces;
using System.ComponentModel;

namespace StockScope.Tools
{
    [McpServerToolType]
    public class CommodityTools
    {
        [McpServerTool]
        [Description("Retrieves historical price data for gold or silver across daily, weekly, or monthly horizons with pagination and date filtering.")]
        public static async Task<string> GetGoldSilverHistory(
            ICommodityService commodityService,
            [Description("Commodity symbol: 'GOLD' (or 'XAU') for gold, 'SILVER' (or 'XAG') for silver.")]
            string symbol,
            [Description("Interval horizon: 'daily', 'weekly', or 'monthly'. Default is 'daily'.")]
            string interval = "daily",
            [Description("Optional start date filter in 'YYYY-MM-DD' format.")]
            string? startDate = null,
            [Description("Optional end date filter in 'YYYY-MM-DD' format.")]
            string? endDate = null,
            [Description("Page number for pagination (starts at 1). Default is 1.")]
            int page = 1,
            [Description("Maximum number of historical records to return per page (1-100). Default is 30.")]
            int limit = 30,
            CancellationToken cancellationToken = default)
        {
            return await commodityService.GetGoldSilverHistoryAsync(symbol, interval, startDate, endDate, page, limit, cancellationToken);
        }
    }
}

