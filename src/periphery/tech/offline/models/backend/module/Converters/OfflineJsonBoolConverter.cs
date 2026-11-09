namespace Tmobis.Offline.Converters;
using System;
using Newtonsoft.Json;

// TODO: Проверить необходимость?
/// <summary>
/// Специальный конвертер JSON для офлайн бэкапов с особой обработкой bool.
/// </summary>
public class OfflineJsonBoolConverter : JsonConverter
{
    /// <inheritdoc/>
    public override void WriteJson(
        JsonWriter writer,
        object? value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value != null && (bool)value ? "true" : "false");
    }

    /// <inheritdoc/>
    public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object? existingValue,
        JsonSerializer serializer)
    {
        return reader?.Value?.ToString() == "true";
    }

    /// <inheritdoc/>
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(bool);
    }
}
