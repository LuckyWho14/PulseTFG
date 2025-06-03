using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PulseTFG.Converter
{
    public static class FirestoreHelper
    {
        public static T ConvertFromFirestore<T>(JsonElement doc) where T : new()
        {
            var fields = doc.GetProperty("fields");
            var obj = new T();

            foreach (var prop in typeof(T).GetProperties())
            {
                if (!fields.TryGetProperty(prop.Name, out var value)) continue;

                object parsedValue = null;

                switch (value.ValueKind)
                {
                    case JsonValueKind.Object:
                        if (value.TryGetProperty("stringValue", out var s))
                            parsedValue = s.GetString();
                        else if (value.TryGetProperty("booleanValue", out var b))
                            parsedValue = b.GetBoolean();
                        else if (value.TryGetProperty("integerValue", out var i))
                            parsedValue = int.Parse(i.GetString());
                        else if (value.TryGetProperty("doubleValue", out var d))
                            parsedValue = double.Parse(d.GetRawText(), CultureInfo.InvariantCulture);
                        else if (value.TryGetProperty("timestampValue", out var t))
                            parsedValue = DateTime.Parse(t.GetString(), null, DateTimeStyles.AdjustToUniversal);
                        break;
                }

                if (parsedValue != null)
                    prop.SetValue(obj, parsedValue);
            }

            // Extraer ID desde el nombre del documento
            if (doc.TryGetProperty("name", out var nameProp))
            {
                var name = nameProp.GetString();
                var id = name.Split('/').Last();
                var idProp = typeof(T).GetProperties()
                    .FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                                         p.Name.StartsWith("Id"));

                idProp?.SetValue(obj, id);
            }

            return obj;
        }
    }

}
