using System.Globalization;
using System.Text.RegularExpressions;

namespace StockScope.Common
{
    public static partial class SecurityValidator
    {
        [GeneratedRegex(@"^[A-Za-z0-9.\-]{1,20}$", RegexOptions.Compiled)]
        private static partial Regex SymbolRegex();

        [GeneratedRegex(@"^[A-Z][0-9]{6}$", RegexOptions.Compiled)]
        private static partial Regex BioguideRegex();

        [GeneratedRegex(@"^[A-Za-z0-9_,().=\-\s]{1,200}$", RegexOptions.Compiled)]
        private static partial Regex CalculationsRegex();

        [GeneratedRegex(@"^[A-Za-z0-9\-]{1,50}$", RegexOptions.Compiled)]
        private static partial Regex AnalyticsRangeRegex();

        public static string ValidateSymbol(string? symbol, string paramName = "symbol")
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException($"Parameter '{paramName}' is required and cannot be empty.", paramName);
            }

            var trimmed = symbol.Trim().ToUpperInvariant();
            if (!SymbolRegex().IsMatch(trimmed))
            {
                throw new ArgumentException($"Parameter '{paramName}' contains invalid characters or exceeds 20 characters. Valid ticker format: e.g. AAPL, BRK.B.", paramName);
            }

            return trimmed;
        }

        public static string ValidateSymbols(string? symbols, string paramName = "symbols")
        {
            if (string.IsNullOrWhiteSpace(symbols))
            {
                throw new ArgumentException($"Parameter '{paramName}' is required and cannot be empty.", paramName);
            }

            var splitSymbols = symbols.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (splitSymbols.Length == 0)
            {
                throw new ArgumentException($"Parameter '{paramName}' must contain at least one valid ticker symbol.", paramName);
            }

            if (splitSymbols.Length > 20)
            {
                throw new ArgumentException($"Parameter '{paramName}' cannot contain more than 20 symbols per request.", paramName);
            }

            var validatedList = new List<string>(splitSymbols.Length);
            foreach (var s in splitSymbols)
            {
                validatedList.Add(ValidateSymbol(s, paramName));
            }

            return string.Join(",", validatedList);
        }

        public static string? ValidateDate(string? date, string paramName)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                return null;
            }

            var trimmed = date.Trim();
            if (!DateTime.TryParseExact(trimmed, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                throw new ArgumentException($"Parameter '{paramName}' must be a valid date in 'YYYY-MM-DD' format.", paramName);
            }

            return trimmed;
        }

        public static void ValidateDateRange(string? startDate, string? endDate)
        {
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                if (DateTime.Parse(startDate, CultureInfo.InvariantCulture) > DateTime.Parse(endDate, CultureInfo.InvariantCulture))
                {
                    throw new ArgumentException("startDate cannot be greater than endDate.");
                }
            }
        }

        public static (int Page, int Limit) ValidatePagination(int page, int limit)
        {
            var validPage = page < 1 ? 1 : page;
            var validLimit = Math.Clamp(limit, 1, 100);
            return (validPage, validLimit);
        }

        public static string ValidateInterval(string? interval, string[] allowedIntervals, string defaultInterval = "daily")
        {
            if (string.IsNullOrWhiteSpace(interval))
            {
                return defaultInterval;
            }

            var normalized = interval.Trim().ToLowerInvariant();
            if (!allowedIntervals.Contains(normalized, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"Interval '{interval}' is not valid. Allowed intervals: {string.Join(", ", allowedIntervals)}.", nameof(interval));
            }

            return normalized;
        }

        public static string ValidateSeriesType(string? seriesType, string defaultType = "close")
        {
            if (string.IsNullOrWhiteSpace(seriesType))
            {
                return defaultType;
            }

            var normalized = seriesType.Trim().ToLowerInvariant();
            var allowed = new[] { "close", "open", "high", "low" };
            if (!allowed.Contains(normalized))
            {
                throw new ArgumentException($"Series type '{seriesType}' is not valid. Allowed types: {string.Join(", ", allowed)}.", nameof(seriesType));
            }

            return normalized;
        }

        public static string ValidateCommoditySymbol(string? symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("Commodity symbol is required. Use GOLD (or XAU) or SILVER (or XAG).", nameof(symbol));
            }

            var normalized = symbol.Trim().ToUpperInvariant();
            var allowed = new[] { "GOLD", "SILVER", "XAU", "XAG" };
            if (!allowed.Contains(normalized))
            {
                throw new ArgumentException($"Commodity symbol '{symbol}' is not supported. Use GOLD, SILVER, XAU, or XAG.", nameof(symbol));
            }

            return normalized;
        }

        public static string SanitizeKeyword(string? keywords)
        {
            if (string.IsNullOrWhiteSpace(keywords))
            {
                throw new ArgumentException("Search keywords cannot be empty.", nameof(keywords));
            }

            var trimmed = keywords.Trim();
            if (trimmed.Length > 100)
            {
                trimmed = trimmed[..100];
            }

            // Strip dangerous control characters
            return new string(trimmed.Where(c => !char.IsControl(c)).ToArray());
        }

        public static string? ValidateBioguideId(string? bioguideId)
        {
            if (string.IsNullOrWhiteSpace(bioguideId))
            {
                return null;
            }

            var trimmed = bioguideId.Trim().ToUpperInvariant();
            if (!Regex.IsMatch(trimmed, @"^[A-Z][0-9]{6}$"))
            if (!BioguideRegex().IsMatch(trimmed))
            {
                throw new ArgumentException("Bioguide ID must be in format like 'S000510' (1 letter followed by 6 digits).", nameof(bioguideId));
            }

            return trimmed;
        }

        public static string ValidateCalculations(string? calculations, string paramName = "calculations")
        {
            if (string.IsNullOrWhiteSpace(calculations))
            {
                throw new ArgumentException("Calculations parameter is required (e.g. MEAN,STDDEV).", paramName);
            }

            var trimmed = calculations.Trim();
            if (!CalculationsRegex().IsMatch(trimmed))
            {
                throw new ArgumentException(
                    $"Parameter '{paramName}' contains invalid characters or exceeds 200 characters.",
                    paramName);
            }

            return trimmed;
        }

        public static string? ValidateAnalyticsRange(string? range, string paramName = "range")
        {
            if (string.IsNullOrWhiteSpace(range))
            {
                return null;
            }

            var trimmed = range.Trim();
            if (!AnalyticsRangeRegex().IsMatch(trimmed))
            {
                throw new ArgumentException(
                    $"Parameter '{paramName}' contains invalid characters or exceeds 50 characters (e.g. '1month', '1year', 'full').",
                    paramName);
            }

            return trimmed;
        }
    }
}

