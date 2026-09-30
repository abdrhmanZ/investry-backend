using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Investry.Application.Services.Helpers
{
    public static class DiditSignatureHelper
    {
        public static bool Verify(string rawJson, string signature, string secret)
        {
            var canonicalJson = Canonicalize(rawJson);
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(canonicalJson));
            var computedSignature = Convert.ToHexString(hash).ToLower();

            return computedSignature == signature.ToLower();
        }

        private static string Canonicalize(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var sorted = SortJsonElement(doc.RootElement);
            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            };
            return JsonSerializer.Serialize(sorted, options);
        }

        private static object SortJsonElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var dict = element.EnumerateObject()
                                      .OrderBy(p => p.Name)
                                      .ToDictionary(p => p.Name, p => SortJsonElement(p.Value));
                    return dict;

                case JsonValueKind.Array:
                    return element.EnumerateArray().Select(SortJsonElement).ToList();

                case JsonValueKind.Number:
                    if (element.TryGetInt64(out var l)) return l;
                    if (element.TryGetDouble(out var d)) return d;
                    return element.GetDecimal();

                case JsonValueKind.String:
                    return element.GetString();

                case JsonValueKind.True: return true;
                case JsonValueKind.False: return false;
                case JsonValueKind.Null: return null;

                default: return element.GetRawText();
            }
        }
    }
}
