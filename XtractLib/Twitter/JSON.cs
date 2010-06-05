using System.IO;
using Newtonsoft.Json;

namespace XtractLib.Twitter
{
    public static class JSON
    {
        public static T Deserialize<T>(string toDeserialize)
        {
            T result;
            StringReader reader = new StringReader(toDeserialize);
            JsonSerializer serializer = new JsonSerializer();
            using (JsonReader reader2 = new JsonTextReader(reader))
            {
                result = (T)serializer.Deserialize(reader2, typeof(T));
                if (reader2.Read() && (reader2.TokenType != JsonToken.Comment))
                {
                    throw new JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");
                }
            }
            return result;
        }
    }
}