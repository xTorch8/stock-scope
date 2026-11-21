using System.Text.Json;
using System.Text.Json.Nodes;

namespace StockScope.Common
{
    public static class JsonPaginationHelper
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        public static string PaginateTimeSeries(
            string rawJson,
            int page,
            int limit,
            string? startDate = null,
            string? endDate = null)
        {
            if (string.IsNullOrWhiteSpace(rawJson))
            {
                return "{}";
            }

            JsonNode? rootNode;
            try
            {
                rootNode = JsonNode.Parse(rawJson);
            }
            catch
            {
                return rawJson;
            }

            if (rootNode is not JsonObject rootObj)
            {
                return rawJson;
            }

            if (rootObj.ContainsKey("Error Message") || rootObj.ContainsKey("Information") || rootObj.ContainsKey("Note"))
            {
                return rawJson;
            }

            KeyValuePair<string, JsonNode?>? targetEntry = null;
            foreach (var kvp in rootObj)
            {
                if (!kvp.Key.Equals("Meta Data", StringComparison.OrdinalIgnoreCase) && kvp.Value is JsonObject)
                {
                    targetEntry = kvp;
                    break;
                }
            }

            if (targetEntry == null || targetEntry.Value.Value is not JsonObject seriesObj)
            {
                return rawJson;
            }

            var allEntries = seriesObj.ToList();
            var filteredEntries = allEntries.Where(kvp =>
            {
                var dateKey = kvp.Key;
                var datePart = dateKey.Length >= 10 ? dateKey[..10] : dateKey;

                if (!string.IsNullOrEmpty(startDate) && string.CompareOrdinal(datePart, startDate) < 0)
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(endDate) && string.CompareOrdinal(datePart, endDate) > 0)
                {
                    return false;
                }

                return true;
            }).ToList();

            var totalItems = filteredEntries.Count;
            var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)limit);
            var skip = (page - 1) * limit;
            var pagedEntries = filteredEntries.Skip(skip).Take(limit).ToList();

            var pagedDataObj = new JsonObject();
            foreach (var entry in pagedEntries)
            {
                pagedDataObj[entry.Key] = entry.Value?.DeepClone();
            }

            var result = new JsonObject
            {
                ["metaData"] = rootObj["Meta Data"]?.DeepClone(),
                ["pagination"] = new JsonObject
                {
                    ["page"] = page,
                    ["limit"] = limit,
                    ["totalItems"] = totalItems,
                    ["totalPages"] = totalPages,
                    ["hasMore"] = page < totalPages
                },
                [targetEntry.Value.Key] = pagedDataObj
            };

            return result.ToJsonString(JsonOptions);
        }

        public static string PaginateArray(
            string rawJson,
            string arrayPropertyName,
            int page,
            int limit,
            string? datePropertyName = null,
            string? startDate = null,
            string? endDate = null)
        {
            if (string.IsNullOrWhiteSpace(rawJson))
            {
                return "{}";
            }

            JsonNode? rootNode;
            try
            {
                rootNode = JsonNode.Parse(rawJson);
            }
            catch
            {
                return rawJson;
            }

            if (rootNode is not JsonObject rootObj)
            {
                return rawJson;
            }

            if (rootObj.ContainsKey("Error Message") || rootObj.ContainsKey("Information") || rootObj.ContainsKey("Note"))
            {
                return rawJson;
            }

            var actualKey = rootObj.Select(k => k.Key).FirstOrDefault(k => k.Equals(arrayPropertyName, StringComparison.OrdinalIgnoreCase));
            if (actualKey == null || rootObj[actualKey] is not JsonArray arrayNode)
            {
                return rawJson;
            }

            var list = arrayNode.ToList();
            var dateKeys = !string.IsNullOrEmpty(datePropertyName)
                ? datePropertyName.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                : Array.Empty<string>();

            if (dateKeys.Length > 0 && (!string.IsNullOrEmpty(startDate) || !string.IsNullOrEmpty(endDate)))
            {
                list = list.Where(item =>
                {
                    if (item is JsonObject obj)
                    {
                        foreach (var prop in dateKeys)
                        {
                            var matchingKey = obj.Select(k => k.Key).FirstOrDefault(k => k.Equals(prop, StringComparison.OrdinalIgnoreCase));
                            if (matchingKey != null && obj.TryGetPropertyValue(matchingKey, out var dateNode) && dateNode != null)
                            {
                                var dateStr = dateNode.ToString();
                                var datePart = dateStr.Length >= 10 ? dateStr[..10] : dateStr;

                                if (!string.IsNullOrEmpty(startDate) && string.CompareOrdinal(datePart, startDate) < 0)
                                {
                                    return false;
                                }

                                if (!string.IsNullOrEmpty(endDate) && string.CompareOrdinal(datePart, endDate) > 0)
                                {
                                    return false;
                                }

                                return true;
                            }
                        }
                    }
                    return true;
                }).ToList();
            }

            var totalItems = list.Count;
            var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)limit);
            var skip = (page - 1) * limit;
            var pagedItems = list.Skip(skip).Take(limit).Select(item => item?.DeepClone()).ToArray();

            var pagedArray = new JsonArray(pagedItems);

            var result = new JsonObject();
            foreach (var kvp in rootObj)
            {
                if (kvp.Key.Equals(actualKey, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                result[kvp.Key] = kvp.Value?.DeepClone();
            }

            result["pagination"] = new JsonObject
            {
                ["page"] = page,
                ["limit"] = limit,
                ["totalItems"] = totalItems,
                ["totalPages"] = totalPages,
                ["hasMore"] = page < totalPages
            };
            result[actualKey] = pagedArray;

            return result.ToJsonString(JsonOptions);
        }

        public static string PaginateFinancialReports(string rawJson, string reportType, int limit)
        {
            if (string.IsNullOrWhiteSpace(rawJson))
            {
                return "{}";
            }

            JsonNode? rootNode;
            try
            {
                rootNode = JsonNode.Parse(rawJson);
            }
            catch
            {
                return rawJson;
            }

            if (rootNode is not JsonObject rootObj)
            {
                return rawJson;
            }

            if (rootObj.ContainsKey("Error Message") || rootObj.ContainsKey("Information") || rootObj.ContainsKey("Note"))
            {
                return rawJson;
            }

            var normalizedReportType = (reportType ?? "all").Trim().ToLowerInvariant();
            var validLimit = Math.Clamp(limit, 1, 50);

            var result = new JsonObject
            {
                ["symbol"] = rootObj["symbol"]?.DeepClone()
            };

            // Support both standard statements (annualReports/quarterlyReports) and earnings (annualEarnings/quarterlyEarnings)
            var annualKey = rootObj.ContainsKey("annualEarnings") ? "annualEarnings" : "annualReports";
            var quarterlyKey = rootObj.ContainsKey("quarterlyEarnings") ? "quarterlyEarnings" : "quarterlyReports";

            if (normalizedReportType is "all" or "annual")
            {
                if (rootObj[annualKey] is JsonArray annualArr)
                {
                    var sliced = annualArr.Take(validLimit).Select(x => x?.DeepClone()).ToArray();
                    result[annualKey] = new JsonArray(sliced);
                    result[$"{annualKey}Count"] = annualArr.Count;
                }
            }

            if (normalizedReportType is "all" or "quarterly")
            {
                if (rootObj[quarterlyKey] is JsonArray quarterlyArr)
                {
                    var sliced = quarterlyArr.Take(validLimit).Select(x => x?.DeepClone()).ToArray();
                    result[quarterlyKey] = new JsonArray(sliced);
                    result[$"{quarterlyKey}Count"] = quarterlyArr.Count;
                }
            }

            return result.ToJsonString(JsonOptions);
        }

        public static string PaginateCsv(string rawCsv, int page, int limit, string? searchKeyword = null)
        {
            if (string.IsNullOrWhiteSpace(rawCsv))
            {
                return "{}";
            }

            if (rawCsv.TrimStart().StartsWith('{'))
            {
                return rawCsv;
            }

            var lines = rawCsv.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0)
            {
                return "{}";
            }

            var headers = lines[0].Split(',');
            var dataLines = lines.Skip(1).ToList();

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                var term = searchKeyword.Trim();
                dataLines = dataLines.Where(line => line.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var totalItems = dataLines.Count;
            var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)limit);
            var skip = (page - 1) * limit;
            var pagedLines = dataLines.Skip(skip).Take(limit).ToList();

            var rowsArray = new JsonArray();
            foreach (var line in pagedLines)
            {
                var cols = line.Split(',');
                var rowObj = new JsonObject();
                for (int i = 0; i < headers.Length; i++)
                {
                    var header = headers[i].Trim();
                    var value = i < cols.Length ? cols[i].Trim() : string.Empty;
                    rowObj[header] = value;
                }
                rowsArray.Add(rowObj);
            }

            var result = new JsonObject
            {
                ["pagination"] = new JsonObject
                {
                    ["page"] = page,
                    ["limit"] = limit,
                    ["totalItems"] = totalItems,
                    ["totalPages"] = totalPages,
                    ["hasMore"] = page < totalPages
                },
                ["data"] = rowsArray
            };

            return result.ToJsonString(JsonOptions);
        }
    }
}
