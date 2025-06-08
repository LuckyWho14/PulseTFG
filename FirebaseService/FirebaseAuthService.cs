using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using PulseTFG.Models;
using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace PulseTFG.FirebaseService;

public class FirebaseAuthService
{
    // Cliente HTTP para hacer peticiones a Firebase Auth y Firestore
    private readonly HttpClient _httpClient = new HttpClient();

    // Clave de API de Firebase
    private readonly string apiKey = "AIzaSyA6vA0XQcWhkIBjEA7Yrwo6wOZK-4-BPT4";

    // Keys para almacenar tokens en Preferences
    private const string PrefsIdTokenKey = "firebase_id_token";

    // Key para almacenar el refresh token
    private const string PrefsRefreshTokenKey = "firebase_refresh_token";

    // Key para almacenar el UID del usuario
    private const string PrefsUserUidKey = "firebase_user_uid";

    public class AuthResponse
    {
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; }

        [JsonPropertyName("localId")]
        public string LocalId { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
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


    // Constructor que inicializa el servicio de autenticación de Firebase.    
    public async Task<string> LoginAsync(string email, string password)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";
        var payload = new { email, password, returnSecureToken = true };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Error autenticando usuario");

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AuthResponse>(responseContent);

        // Verificar email
        bool emailVerificado = await EstaVerificadoEmailAsync(result.IdToken);
        if (!emailVerificado)
            throw new Exception("Debes verificar tu email antes de iniciar sesión.");

        // Guardar tokens localmente
        Preferences.Set(PrefsIdTokenKey, result.IdToken);
        Preferences.Set(PrefsRefreshTokenKey, result.RefreshToken);

        // Guardar UID del usuario
        Preferences.Set(PrefsUserUidKey, result.LocalId);

        return result.IdToken;
    }

    // Registra un nuevo usuario en Firebase Auth.
    public async Task<AuthResponse> RegisterAsync(string email, string password)
    {
        var requestUri = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={apiKey}";
        var payload = new { email, password, returnSecureToken = true };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(requestUri, content);
        if (!response.IsSuccessStatusCode)
        {
            var errorJson = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al registrar: {errorJson}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AuthResponse>(responseJson);

        // Guarda tokens localmente
        Preferences.Set(PrefsIdTokenKey, result.IdToken);
        Preferences.Set(PrefsRefreshTokenKey, result.RefreshToken);

        // Guarda UID del usuario
        Preferences.Set(PrefsUserUidKey, result.LocalId);

        return result;
    }

    // Comprueba si el token sigue siendo válido llamando al endpoint accounts:lookup.
    public async Task<bool> TokenEsValidoAsync(string idToken)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:lookup?key={apiKey}";
        var payload = new { idToken };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        if (!response.IsSuccessStatusCode)
            return false;

        var responseJson = await response.Content.ReadAsStringAsync();
        var status = JsonSerializer.Deserialize<EmailVerificationStatus>(responseJson);
        return status?.Users != null && status.Users.Length > 0;
    }

    // Guarda la información del usuario en Firestore, incluyendo fecha de creación.
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
                { "peso", new { doubleValue = usuario.Peso } },
                { "fechaCreacion", new { timestampValue = usuario.FechaCreacion.ToUniversalTime().ToString("o") } }
            }
        };

        var json = JsonSerializer.Serialize(docData);
        var request = new HttpRequestMessage(HttpMethod.Patch, firestoreUrl)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al guardar datos del usuario: {errorContent}");
        }
    }

    // Envía email de verificación.
    public async Task EnviarEmailVerificacionAsync(string idToken)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={apiKey}";
        var payload = new { requestType = "VERIFY_EMAIL", idToken };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error enviando email de verificación: {error}");
        }
    }

    // Comprueba si el email ya está verificado.
    public async Task<bool> EstaVerificadoEmailAsync(string idToken)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:lookup?key={apiKey}";
        var payload = new { idToken };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error verificando estado del email: {await response.Content.ReadAsStringAsync()}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var userInfo = JsonSerializer.Deserialize<EmailVerificationStatus>(responseJson);
        return userInfo?.Users?[0].EmailVerified ?? false;
    }

    // Envía correo de recuperación de contraseña.
    public async Task EnviarCorreoRecuperacionAsync(string email)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={apiKey}";
        var payload = new { requestType = "PASSWORD_RESET", email };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error enviando correo recuperación: {await response.Content.ReadAsStringAsync()}");
        }
    }

    // Cierra sesión borrando tokens locales y revocando refresh token.
    public async Task SignOutAsync()
    {
        // Revocar refresh token en servidor
        var refreshToken = Preferences.Get(PrefsRefreshTokenKey, null);
        if (!string.IsNullOrEmpty(refreshToken))
        {
            var revokeUrl = $"https://securetoken.googleapis.com/v1/token?key={apiKey}";
            var body = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };
            var revokeContent = new FormUrlEncodedContent(body);
            await _httpClient.PostAsync(revokeUrl, revokeContent);
        }

        // Borrar datos locales
        Preferences.Remove(PrefsIdTokenKey);
        Preferences.Remove(PrefsRefreshTokenKey);
        Preferences.Remove(PrefsUserUidKey);
    }


}
