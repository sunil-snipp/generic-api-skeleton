namespace Generic.Api.Application.Logging.Models;

using System.Text.Json;
using System.Text.RegularExpressions;

/// <summary>
/// Normalizes log messages from various sources (especially Go/gRPC services).
/// Handles cross-platform log format variations to ensure consistent, parseable logs.
/// </summary>
public static class LogMessageNormalizer
{
    /// <summary>
    /// Normalizes a log message, handling platform-specific formats.
    /// </summary>
    /// <param name="message">The raw log message.</param>
    /// <param name="maxLength">Maximum length after normalization (default 3000 chars).</param>
    /// <returns>Normalized message.</returns>
    public static string Normalize(string message, int maxLength = 3000)
    {
        if (string.IsNullOrEmpty(message))
            return string.Empty;

        try
        {
            // Handle Go map format: map[key:value key2:value2]
            if (message.StartsWith("map["))
                message = ParseGoMapFormat(message);

            // Handle Go %!s format: %!s(type=value)
            message = ParseGoPercentSFormat(message);

            // Try JSON normalization if it appears JSON-like
            if (message.Contains("{") || message.Contains("["))
                message = TryFormatAsJson(message);
        }
        catch
        {
            // On any normalization error, return original message
        }

        // Truncate if needed
        return message.Length > maxLength ? message[..maxLength] : message;
    }

    /// <summary>
    /// Converts Go map format to JSON.
    /// Example: map[id:123 name:test] → {"id":"123","name":"test"}
    /// </summary>
    private static string ParseGoMapFormat(string input)
    {
        try
        {
            // Extract content between map[ and ]
            if (!input.StartsWith("map[") || !input.EndsWith("]"))
                return input;

            var content = input[4..^1];
            var dict = new Dictionary<string, string>();

            // Match key:value pairs
            var pairs = Regex.Matches(content, @"(\w+):([^\s]+)");
            foreach (Match pair in pairs)
            {
                dict[pair.Groups[1].Value] = pair.Groups[2].Value;
            }

            return dict.Count > 0 ? JsonSerializer.Serialize(dict) : input;
        }
        catch
        {
            return input;
        }
    }

    /// <summary>
    /// Converts Go's %!s format to readable values.
    /// Example: %!s(int64=123) → 123, %!s(bool=true) → true
    /// </summary>
    private static string ParseGoPercentSFormat(string input)
    {
        if (!input.Contains("%!s("))
            return input;

        try
        {
            return Regex.Replace(input, @"%!s\((\w+)=([^)]+)\)", match =>
            {
                var type = match.Groups[1].Value.ToLower();
                var value = match.Groups[2].Value;

                return type switch
                {
                    "bool" => bool.TryParse(value, out var b) ? b.ToString().ToLower() : value,
                    "int64" or "int32" or "int" => long.TryParse(value, out var l) ? l.ToString() : value,
                    "float64" or "float32" => double.TryParse(value, out var d) ? d.ToString() : value,
                    _ => value
                };
            });
        }
        catch
        {
            return input;
        }
    }

    /// <summary>
    /// Attempts to parse and pretty-format JSON, fixing common formatting issues.
    /// Handles single quotes, unquoted keys, and other common JSON mistakes.
    /// </summary>
    private static string TryFormatAsJson(string input)
    {
        try
        {
            // Check if already valid JSON
            using var doc = JsonDocument.Parse(input);
            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(doc.RootElement, options);
        }
        catch
        {
            // Try to fix common JSON issues
            try
            {
                // Replace single quotes with double quotes
                var fixed1 = Regex.Replace(input, @"(?<=([:,\[\{])\s*)'([^']*)'(?=\s*[,\}\]])", "\"$2\"");

                // Add quotes around unquoted property names
                var fixed2 = Regex.Replace(fixed1, @"(?<={|\s|,)(\w+)(?=\s*:)", "\"$1\"");

                using var doc = JsonDocument.Parse(fixed2);
                var options = new JsonSerializerOptions { WriteIndented = true };
                return JsonSerializer.Serialize(doc.RootElement, options);
            }
            catch
            {
                // If still not valid JSON, return original
                return input;
            }
        }
    }
}
