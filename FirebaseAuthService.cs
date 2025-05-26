using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;

namespace PulseTFG;

public class FirebaseAuthService
{
    private readonly HttpClient _httpClient = new();
    private readonly string apiKey = "AIzaSyA6vA0XQcWhkIBjEA7Yrwo6wOZK-4-BPT4";

    public class AuthResponse
    {
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; }

        [JsonPropertyName("localId")]
        public string LocalId { get; set; }
    }

    public class EmailVerificationStatus
    {
        [JsonPropertyName("users")]
        public UserInfo[] Users { get; set; }
    }

    public class UserInfo
    {
        [JsonPropertyName("emailVerified")]
        public bool EmailVerified { get; set; }
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";

        var payload = new
        {
            email = email,
            password = password,
            returnSecureToken = true
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        var response = await client.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Error autenticando usuario");

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AuthResponse>(responseContent);

        // Comprobar si el email está verificado
        bool emailVerificado = await EstaVerificadoEmailAsync(result.IdToken);
        if (!emailVerificado)
            throw new Exception("Debes verificar tu email antes de iniciar sesión.");

        return result.IdToken;
    }


    public async Task<AuthResponse> RegisterAsync(string email, string password)
    {
        var requestUri = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={apiKey}";

        var payload = new
        {
            email = email,
            password = password,
            returnSecureToken = true
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(requestUri, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorJson = await response.Content.ReadAsStringAsync();
            // Maneja el error (parsea mensaje, etc.)
            throw new Exception($"Error al registrar: {errorJson}");
        }

        var responseJson = await response.Content.ReadAsStringAsync(); 
        var result = JsonSerializer.Deserialize<AuthResponse>(responseJson);

        return result; // token que puedes usar después
    }
    
    public async Task GuardarUsuarioAsync(Usuario usuario, string idToken)
    {
        var firestoreUrl = $"https://firestore.googleapis.com/v1/projects/pulsetfg-6642a/databases/(default)/documents/usuarios/{usuario.Uid}";


        var docData = new
        {
            fields = new Dictionary<string, object>
            {
                { "email", new { stringValue = usuario.Email } },
                { "nombreCompleto", new { stringValue = usuario.NombreCompleto } },
                { "fechaNacimiento", new { timestampValue = usuario.FechaNacimiento.ToUniversalTime().ToString("o") } },
                { "altura", new { doubleValue = usuario.Altura } },
                { "peso", new { doubleValue = usuario.Peso } }
            }       
        };

        var json = JsonSerializer.Serialize(docData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Patch, firestoreUrl);
        request.Content = content;
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al guardar datos del usuario: {errorContent}");
        }

    }
    public async Task EnviarEmailVerificacionAsync(string idToken)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={apiKey}";

        var payload = new
        {
            requestType = "VERIFY_EMAIL",
            idToken = idToken
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error enviando email de verificación: {error}");
        }
    }
    

    public async Task<bool> EstaVerificadoEmailAsync(string idToken)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:lookup?key={apiKey}";

        var payload = new
        {
            idToken = idToken
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error verificando estado del email: {error}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var userInfo = JsonSerializer.Deserialize<EmailVerificationStatus>(responseJson);

        return userInfo?.Users?[0].EmailVerified ?? false;
    }
   
    public async Task EnviarCorreoRecuperacionAsync(string email)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={apiKey}";

        var payload = new
        {
            requestType = "PASSWORD_RESET",
            email = email
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error enviando correo recuperación: {error}");
        }
    }


}
