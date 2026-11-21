namespace StockScope.Configurations
{
    public class AlphaVantageConfiguration
    {
        public const string SectionName = "ALPHA_VANTAGE";

        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
    }
}
