using System.Text.Json;

namespace StockScope.Clients
{
    public interface IAlphaVantageClient
    {
        Task<JsonDocument> GetAsync(
            string function,
            Dictionary<string, string>? parameters = null,
            CancellationToken cancellationToken = default
        );

        Task<string> GetRawAsync(
            string function,
            Dictionary<string, string>? parameters = null,
            CancellationToken cancellationToken = default
        );
    }
}
