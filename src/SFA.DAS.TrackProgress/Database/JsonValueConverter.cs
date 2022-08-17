using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace SFA.DAS.TrackProgress.Database;

public class JsonValueConverter<T> : ValueConverter<T, string> where T : class
{
    public JsonValueConverter()
        : base(v => JsonHelper.Serialize(v), v => JsonHelper.Deserialize(v))
    { }

    private static class JsonHelper
    {
        public static string Serialize(T obj)
            => JsonSerializer.Serialize(obj);

        public static T Deserialize(string json)
            => JsonSerializer.Deserialize<T>(json)
            ?? throw new ArgumentException("Cannot deserialise data");
    }
}