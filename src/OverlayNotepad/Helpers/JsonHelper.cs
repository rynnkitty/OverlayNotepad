using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using OverlayNotepad.Models;

namespace OverlayNotepad.Helpers
{
    public static class JsonHelper
    {
        private static readonly DataContractJsonSerializerSettings _settings =
            new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };

        public static string Serialize(AppSettings appSettings)
        {
            var serializer = new DataContractJsonSerializer(typeof(AppSettings), _settings);
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, appSettings);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static AppSettings Deserialize(string json)
        {
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(AppSettings), _settings);
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    return (AppSettings)serializer.ReadObject(ms);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
