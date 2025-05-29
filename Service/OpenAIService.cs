using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PulseTFG.Service
{
    public class OpenAIService
    {

        // atributo para almacenar la instancia de HttpClient
        private readonly HttpClient _httpClient;
        private readonly string _ApiKey;

        public OpenAIService(string apiKey)
        {
            _ApiKey = apiKey.Trim(); // Limpia espacios en blanco al inicio y al final de la clave
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _ApiKey);
        }

        // Método para obtener una frase motivacional corta y única
        /*public async Task<string> ObtenerFraseMotivacionalYConsejoAsync()
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "Eres un coach personal motivador y experto en salud." },
                    new { role = "user", content = "Dame una frase motivacional breve y un consejo de salud útil. Sé claro y breve." }
                },
                max_tokens = 150,
                temperature = 0.8
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            var resultado = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return resultado;
        }*/

        public async Task<string> ObtenerFraseMotivacionalYConsejoAsync()
        {
            try
            {
                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = "Eres un coach personal motivador y experto en salud." },
                        new { role = "user", content = "Dame una frase motivacional breve y un consejo de salud útil. Sé claro y breve." }
            },
                    max_tokens = 150,
                    temperature = 0.8
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return $"Error en la API: {response.StatusCode} - {responseString}";
                }

                using var doc = JsonDocument.Parse(responseString);

                // Manejo más seguro del JSON
                if (doc.RootElement.TryGetProperty("choices", out var choices) &&
                    choices.GetArrayLength() > 0 &&
                    choices[0].TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var contentProp))
                {
                    return contentProp.GetString() ?? "No se recibió contenido";
                }

                return "No se pudo procesar la respuesta de la API";
            }
            catch (Exception ex)
            {
                return $"Error al conectar con OpenAI: {ex.Message}";
            }
        }

        
    }
}
