using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockScope.Configurations;
using System.Diagnostics;
using System.Text.Json;

namespace StockScope.Clients
{
    public class AlphaVantageClient : IAlphaVantageClient
    {
        private readonly HttpClient _httpClient;
        private readonly AlphaVantageConfiguration _configuration;
        private readonly ILogger<AlphaVantageClient> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AlphaVantageClient(
            HttpClient httpClient,
            IOptions<AlphaVantageConfiguration> configuration,
            ILogger<AlphaVantageClient> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration.Value;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

            if (string.IsNullOrWhiteSpace(_configuration.ApiKey))
            {
                _logger.LogWarning("Alpha Vantage API key is not configured. External requests will fail.");
            }
        }

        public async Task<string> GetRawAsync(
            string function,
            Dictionary<string, string>? parameters = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_configuration.ApiKey))
            {
                throw new InvalidOperationException("Alpha Vantage API Key is missing. Please configure ALPHA_VANTAGE:ApiKey.");
            }

            var stopwatch = Stopwatch.StartNew();
            var httpContext = _httpContextAccessor.HttpContext;
            var clientIp = httpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var clientAgent = SanitizeLogValue(httpContext?.Request.Headers.UserAgent.ToString());

            try
            {
                var queryParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        if (parameter.Key.Equals("function", StringComparison.OrdinalIgnoreCase) ||
                            parameter.Key.Equals("apikey", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        if (!string.IsNullOrWhiteSpace(parameter.Value))
                        {
                            queryParameters[parameter.Key] = parameter.Value;
                        }
                    }
                }

                // Set core parameters last so callers cannot override them
                queryParameters["function"] = function;
                queryParameters["apikey"] = _configuration.ApiKey;

                var query = string.Join("&",
                    queryParameters.Select(qp =>
                        $"{Uri.EscapeDataString(qp.Key)}={Uri.EscapeDataString(qp.Value)}"));

                _logger.LogInformation(
                    "Sending Alpha Vantage request. Function: {Function}, ClientIP: {ClientIP}, UserAgent: {UserAgent}",
                    function,
                    clientIp,
                    clientAgent
                );

                var response = await _httpClient.GetAsync(
                    $"?{query}",
                    cancellationToken
                );

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                stopwatch.Stop();

                _logger.LogInformation(
                    "Alpha Vantage response received. Function: {Function}, StatusCode: {StatusCode}, Duration: {Duration}ms",
                    function,
                    (int)response.StatusCode,
                    stopwatch.ElapsedMilliseconds
                );

                LogIfAlphaVantageErrorPayload(function, content);

                return content;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                // Prevent API key leakage in logs or exceptions
                var sanitizedMessage = SanitizeApiKey(ex.Message);

                _logger.LogError(
                    "Alpha Vantage request failed. Function: {Function}, ClientIP: {ClientIP}, Duration: {Duration}ms, ExceptionType: {ExceptionType}, Error: {Error}",
                    function,
                    clientIp,
                    stopwatch.ElapsedMilliseconds,
                    ex.GetType().Name,
                    sanitizedMessage
                );

                throw new ApplicationException(
                    $"Error occurred while fetching data from Alpha Vantage for function '{function}': {sanitizedMessage}"
                );
            }
        }

        public async Task<JsonDocument> GetAsync(
            string function,
            Dictionary<string, string>? parameters = null,
            CancellationToken cancellationToken = default)
        {
            var raw = await GetRawAsync(function, parameters, cancellationToken);
            return JsonDocument.Parse(raw);
        }

        private void LogIfAlphaVantageErrorPayload(string function, string content)
        {
            if (string.IsNullOrWhiteSpace(content) || !content.TrimStart().StartsWith('{'))
            {
                return;
            }

            try
            {
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;
                if (root.TryGetProperty("Error Message", out _))
                {
                    _logger.LogWarning("Alpha Vantage returned an error payload for function {Function}.", function);
                }
                else if (root.TryGetProperty("Note", out _) || root.TryGetProperty("Information", out _))
                {
                    _logger.LogWarning("Alpha Vantage returned a rate-limit or informational notice for function {Function}.", function);
                }
            }
            catch (JsonException)
            {
                // Ignore malformed JSON here; downstream callers handle raw payload
            }
        }

        private string SanitizeApiKey(string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(_configuration.ApiKey))
            {
                return text;
            }

            var sanitized = text.Replace(_configuration.ApiKey, "***REDACTED***", StringComparison.OrdinalIgnoreCase);
            var escapedKey = Uri.EscapeDataString(_configuration.ApiKey);
            if (!string.IsNullOrEmpty(escapedKey))
            {
                sanitized = sanitized.Replace(escapedKey, "***REDACTED***", StringComparison.OrdinalIgnoreCase);
            }

            return sanitized;
        }

        private static string SanitizeLogValue(string? value, int maxLength = 200)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Unknown";
            }

            var cleaned = new string(value.Where(c => !char.IsControl(c)).ToArray()).Trim();
            if (cleaned.Length == 0)
            {
                return "Unknown";
            }

            return cleaned.Length > maxLength ? cleaned[..maxLength] : cleaned;
        }
    }
}
