using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace back.Converters
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly string[] Formats = {
            "yyyy-MM-dd",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ssZ"
        };

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value))
                throw new JsonException("Se esperaba una fecha no vacía.");

            if (DateTime.TryParseExact(value, Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return DateTime.SpecifyKind(date, DateTimeKind.Utc);
            }

            throw new JsonException($"No se pudo convertir '{value}' a DateTime. Formatos esperados: {string.Join(", ", Formats)}.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        }
    }
}
