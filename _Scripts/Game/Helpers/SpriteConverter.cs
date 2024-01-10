using System;
using Newtonsoft.Json;
using UnityEngine;

public class SpriteConverter : JsonConverter<Sprite>
{
    public override void WriteJson(JsonWriter writer, Sprite value, JsonSerializer serializer)
    {
        writer.WriteNull();
    }

    public override Sprite ReadJson(JsonReader reader, Type objectType, Sprite existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanRead => false;
    public override bool CanWrite => true;
}
