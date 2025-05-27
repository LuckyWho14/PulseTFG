using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using PulseTFG.Models;
using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace PulseTFG.AuthService;

public class FirebaseAuthService
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly string apiKey = "AIzaSyA6vA0XQcWhkIBjEA7Yrwo6wOZK-4-BPT4";

    // Keys para almacenar tokens en Preferences
    private const string PrefsIdTokenKey = "firebase_id_token";
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

    /// <summary>
    /// Inicia sesión con email y contraseña.
    /// </summary>
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

    /// <summary>
    /// Registra un nuevo usuario en Firebase Auth.
    /// </summary>
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

        // Guardar tokens localmente
        Preferences.Set(PrefsIdTokenKey, result.IdToken);
        Preferences.Set(PrefsRefreshTokenKey, result.RefreshToken);

        // Guardar UID del usuario
        Preferences.Set(PrefsUserUidKey, result.LocalId);

        return result;
    }

    /// <summary>
    /// Comprueba si el token sigue siendo válido llamando al endpoint accounts:lookup.
    /// </summary>
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

    /// <summary>
    /// Guarda la información del usuario en Firestore, incluyendo fecha de creación.
    /// </summary>
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

    /// <summary>
    /// Envía email de verificación.
    /// </summary>
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

    /// <summary>
    /// Comprueba si el email ya está verificado.
    /// </summary>
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

    /// <summary>
    /// Envía correo de recuperación de contraseña.
    /// </summary>
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

    /// <summary>
    /// Cierra sesión borrando tokens locales y revocando refresh token.
    /// </summary>
    public async Task SignOutAsync()
    {
        // Revocar refresh token en servidor (opcional)
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

    /// <summary>
    /// Obtiene los datos del usuario desde Firestore.
    /// </summary>
    
    public async Task<Usuario> ObtenerUsuarioAsync(string uid)
    {
        var url =
          $"https://firestore.googleapis.com/v1/projects/pulsetfg-6642a/databases/(default)"
        + $"/documents/usuarios/{uid}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Usa el mismo idToken que guardaste en login
        var idToken = Preferences.Get(PrefsIdTokenKey, null);
        if (string.IsNullOrEmpty(idToken))
            throw new InvalidOperationException("No hay token válido.");

        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

        var resp = await _httpClient.SendAsync(request);
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var f = doc.RootElement.GetProperty("fields");

        return new Usuario
        {
            Uid = uid,
            NombreCompleto = f.GetProperty("nombreCompleto").GetProperty("stringValue").GetString(),
            Email = f.GetProperty("email").GetProperty("stringValue").GetString(),
            FechaNacimiento = DateTime.Parse(
                                  f.GetProperty("fechaNacimiento")
                                   .GetProperty("timestampValue")
                                   .GetString()
                              ),
            Altura = f.GetProperty("altura").GetProperty("doubleValue").GetDouble(),
            Peso = f.GetProperty("peso").GetProperty("doubleValue").GetDouble(),
            FechaCreacion = DateTime.Parse(
                                  f.GetProperty("fechaCreacion")
                                   .GetProperty("timestampValue")
                                   .GetString()
                              )
        };
    }

    /// <summary>
    /// Actualiza un único campo del documento de usuario en Firestore.
    /// </summary>
    public async Task ActualizarCampoUsuarioAsync(string uid, string campo, object valor)
    {
        // Endpoint con máscara para solo el campo que quieres tocar
        var url =
          $"https://firestore.googleapis.com/v1/projects/pulsetfg-6642a/databases/(default)" +
          $"/documents/usuarios/{uid}?updateMask.fieldPaths={campo}";

        var docData = new
        {
            fields = new Dictionary<string, object>
        {
            { campo, new { doubleValue = Convert.ToDouble(valor) } }
        }
        };
        var json = JsonSerializer.Serialize(docData);

        var req = new HttpRequestMessage(HttpMethod.Patch, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        var idToken = Preferences.Get("firebase_id_token", null);
        req.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

        var resp = await _httpClient.SendAsync(req);
        resp.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Obtiene las rutinas asociadas a un usuario específico desde Firestore.
    /// </summary>
    public async Task<List<Rutina>> ObtenerRutinasUsuarioAsync(string uid)
    {
        var url =
          $"https://firestore.googleapis.com/v1/projects/pulsetfg-6642a/databases/(default)" +
          $"/documents/usuarios/{uid}/rutinas";
        var req = new HttpRequestMessage(HttpMethod.Get, url);
        var idToken = Preferences.Get(PrefsIdTokenKey, null);
        req.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", idToken);

        var resp = await _httpClient.SendAsync(req);
        if (!resp.IsSuccessStatusCode) return new List<Rutina>();

        var json = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var list = new List<Rutina>();
        foreach (var docElem in doc.RootElement.GetProperty("documents").EnumerateArray())
        {
            var name = docElem.GetProperty("name").GetString();
            var fields = docElem.GetProperty("fields");
            list.Add(new Rutina
            {
                IdRutina = name.Substring(name.LastIndexOf('/') + 1),
                Nombre = fields.GetProperty("nombre").GetProperty("stringValue").GetString(),
                Descripcion = fields.GetProperty("descripcion").GetProperty("stringValue").GetString(),
                Activo = fields.GetProperty("activo").GetProperty("booleanValue").GetBoolean(),
                FechaCreacion = DateTime.Parse(fields.GetProperty("fechaCreacion").GetProperty("timestampValue").GetString()),
                Actualizado = DateTime.Parse(fields.GetProperty("actualizado").GetProperty("timestampValue").GetString())
            });
        }
        return list;
    }

    /// <summary>
    /// Crea una nueva rutina asociada a un usuario específico en Firestore.
    /// </summary>
    public async Task<Rutina> CrearRutinaAsync(string uid, Rutina r)
    {
        var url =
          $"https://firestore.googleapis.com/v1/projects/pulsetfg-6642a/databases/(default)" +
          $"/documents/usuarios/{uid}/rutinas?documentId={Guid.NewGuid()}";

        var docData = new
        {
            fields = new Dictionary<string, object>
        {
            { "nombre",        new { stringValue = r.Nombre } },
            { "descripcion",   new { stringValue = r.Descripcion } },
            { "activo",        new { booleanValue = r.Activo } },
            { "fechaCreacion", new { timestampValue = r.FechaCreacion.ToUniversalTime().ToString("o") } },
            { "actualizado",   new { timestampValue = r.Actualizado.ToUniversalTime().ToString("o") } }
        }
        };

        var json = JsonSerializer.Serialize(docData);
        var req = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        var token = Preferences.Get("firebase_id_token", null);
        req.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var resp = await _httpClient.SendAsync(req);
        resp.EnsureSuccessStatusCode();

        var respJson = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(respJson);
        var name = doc.RootElement.GetProperty("name").GetString();
        r.IdRutina = name[(name.LastIndexOf('/') + 1)..];
        return r;
    }
}
