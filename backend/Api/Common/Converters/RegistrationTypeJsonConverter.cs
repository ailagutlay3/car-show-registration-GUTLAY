using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Entities;

namespace Api.Common.Converters;

public sealed class RegistrationTypeJsonConverter
    : JsonConverter<RegistrationType>
{
    public override RegistrationType Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();

            if (Enum.TryParse<RegistrationType>(
                value, true, out var result)
                && Enum.IsDefined(result))
            {
                return result;
            }

            return (RegistrationType)(-1);
        }

        if (reader.TokenType == JsonTokenType.Number &&
            reader.TryGetInt32(out var number))
        {
            var result = (RegistrationType)number;

            if (Enum.IsDefined(result))
                return result;

            return (RegistrationType)(-1);
        }

        throw new JsonException("Invalid registration type.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        RegistrationType value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}