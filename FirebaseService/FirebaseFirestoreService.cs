using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using PulseTFG.Models;
using System.Text.Json.Serialization;


namespace PulseTFG.FirebaseService
{
    internal class FirebaseFirestoreService
    {
        // Clave de ID del proyecto de Firebase
        private const string FirebaseProjectId = "pulsetfg-6642a";

        // Clave de API de Firebase
        private readonly string apiKey = "AIzaSyA6vA0XQcWhkIBjEA7Yrwo6wOZK-4-BPT4";

        // URL base para Firestore
        private const string FirestoreBaseUrl = $"https://firestore.googleapis.com/v1/projects/{FirebaseProjectId}/databases/(default)/documents";

        // Cliente HTTP para hacer peticiones a Firestore
        private readonly HttpClient _httpClient = new HttpClient();

        // Keys para almacenar tokens en Preferences
        private const string PrefsIdTokenKey = "firebase_id_token";

        // Key para almacenar el UID del usuario
        private const string PrefsUserUidKey = "firebase_user_uid";

        // Comprobar si el usuario tiene rutinas asociadas en Firestore.
        public async Task<bool> UsuarioTieneRutinasAsync(string uid)
        {
            using var client = new HttpClient();
            string url = $"{FirestoreBaseUrl}/usuarios/{uid}/rutinas?pageSize=1";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return false;

            var contenido = await response.Content.ReadAsStringAsync();
            return contenido.Contains("documents");
        }


        // Obtiene los datos del usuario desde Firestore.
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

        // Actualiza un único campo del documento de usuario en Firestore.
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

        // Obtiene las rutinas asociadas a un usuario específico desde Firestore.
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
            if (!resp.IsSuccessStatusCode)
                return new List<Rutina>();

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var list = new List<Rutina>();

            // Verifica si hay rutinas antes de acceder a "documents"
            if (!doc.RootElement.TryGetProperty("documents", out var documentos))
            {
                // No hay rutinas
                return new List<Rutina>();
            }

            foreach (var docElem in documentos.EnumerateArray())
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


        // Crea una nueva rutina asociada a un usuario específico en Firestore.
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

        // Obtiene una lista de ejercicios filtrados por favoritos y grupo muscular.
        public async Task<List<Ejercicio>> ObtenerEjerciciosFiltradosAsync(bool soloFavoritos, string grupoMuscular)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🔍 [FIRESTORE] Entrando en ObtenerEjerciciosFiltradosAsync");

                var token = Preferences.Get("firebase_id_token", null);
                var uid = Preferences.Get("firebase_user_uid", null);
                var listaEjercicios = new List<Ejercicio>();
                var favoritosIds = new HashSet<string>();

                // 🔸 Intentar obtener favoritos del usuario
                var favUrl = $"{FirestoreBaseUrl}/usuarios/{uid}/favoritos";
                var favReq = new HttpRequestMessage(HttpMethod.Get, favUrl);
                favReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var favResp = await _httpClient.SendAsync(favReq);
                if (favResp.IsSuccessStatusCode)
                {
                    var favJson = await favResp.Content.ReadAsStringAsync();
                    using var favDoc = JsonDocument.Parse(favJson);

                    if (favDoc.RootElement.TryGetProperty("documents", out var documentosFav))
                    {
                        foreach (var fav in documentosFav.EnumerateArray())
                        {
                            var favId = fav.GetProperty("name").GetString();
                            favoritosIds.Add(favId.Substring(favId.LastIndexOf('/') + 1));
                        }

                        System.Diagnostics.Debug.WriteLine("✅ Favoritos descargados correctamente");
                        System.Diagnostics.Debug.WriteLine($"🎯 Total favoritos encontrados: {favoritosIds.Count}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("ℹ️ El usuario no tiene favoritos todavía.");
                    }
                }

                // Obtener ejercicios
                var url = $"{FirestoreBaseUrl}/ejercicios";
                var req = new HttpRequestMessage(HttpMethod.Get, url);
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var resp = await _httpClient.SendAsync(req);
                if (!resp.IsSuccessStatusCode) return listaEjercicios;

                var json = await resp.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                System.Diagnostics.Debug.WriteLine("📥 Procesando ejercicios...");

                foreach (var ejercicioDoc in doc.RootElement.GetProperty("documents").EnumerateArray())
                {
                    var id = ejercicioDoc.GetProperty("name").GetString().Split('/').Last();
                    var f = ejercicioDoc.GetProperty("fields");

                    string grupo = f.TryGetProperty("GrupoMuscular", out var g)
                        ? g.GetProperty("stringValue").GetString()
                        : "";

                    if (!string.IsNullOrEmpty(grupoMuscular) && grupoMuscular != "Todos" && grupo != grupoMuscular)
                        continue;

                    if (soloFavoritos && !favoritosIds.Contains(id))
                        continue;

                    var nombre = f.TryGetProperty("Nombre", out var n) ? n.GetProperty("stringValue").GetString() : "(sin nombre)";
                    var descripcion = f.TryGetProperty("Descripcion", out var desc) ? desc.GetProperty("stringValue").GetString() : "";
                    var videoId = f.TryGetProperty("VideoId", out var v) ? v.GetProperty("stringValue").GetString() : "";

                    var ejercicio = new Ejercicio
                    {
                        IdEjercicio = id,
                        Nombre = nombre,
                        Descripcion = descripcion,
                        VideoId = videoId,
                        EsFavorito = favoritosIds.Contains(id)
                    };

                    System.Diagnostics.Debug.WriteLine($"✅ Añadido ejercicio: {ejercicio.Nombre}");
                    listaEjercicios.Add(ejercicio);
                }

                return listaEjercicios;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERROR EN FIRESTORE: {ex.Message}");
            }

            return new List<Ejercicio>();
        }




        // Agrega un ejercicio a la lista de favoritos del usuario en Firestore.
        public async Task AgregarFavoritoAsync(string uid, string ejercicioId)
        {
            var url = $"{FirestoreBaseUrl}/usuarios/{uid}/favoritos?documentId={ejercicioId}";
            var data = new { fields = new Dictionary<string, object>() }; // Documento vacío
            var json = JsonSerializer.Serialize(data);

            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var token = Preferences.Get("firebase_id_token", null);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await _httpClient.SendAsync(req);
            resp.EnsureSuccessStatusCode();
        }

        // Elimina un ejercicio de la lista de favoritos del usuario en Firestore.
        public async Task EliminarFavoritoAsync(string uid, string ejercicioId)
        {
            var url = $"{FirestoreBaseUrl}/usuarios/{uid}/favoritos/{ejercicioId}";

            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            var token = Preferences.Get("firebase_id_token", null);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await _httpClient.SendAsync(req);
            resp.EnsureSuccessStatusCode();
        }

        // Actualiza un campo booleano de una rutina existente.
        public async Task ActualizarCampoRutinaAsync(string uid, string rutinaId, string campo, bool valor)
        {
            var url =
                $"{FirestoreBaseUrl}/usuarios/{uid}/rutinas/{rutinaId}"
                + $"?updateMask.fieldPaths={campo}";

            var doc = new
            {
                fields = new Dictionary<string, object>
                {
                    { campo, new { booleanValue = valor } }
                }
            };
            var json = JsonSerializer.Serialize(doc);
            var req = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            var token = Preferences.Get("firebase_id_token", null);
            req.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var resp = await _httpClient.SendAsync(req);
            resp.EnsureSuccessStatusCode();
        }

        // Agrega un Entrenamiento bajo una rutina.
        public async Task CrearEntrenamientoAsync(string uid, string rutinaId, Entrenamiento e)
        {
            var url =
              $"{FirestoreBaseUrl}/usuarios/{uid}/rutinas/{rutinaId}/entrenamientos"
              + $"?documentId={e.IdEntrenamiento}";

            var doc = new
            {
                fields = new Dictionary<string, object>
                {
                    { "nombre", new { stringValue = e.Nombre } },
                    { "fechaCreacion", new { timestampValue = e.FechaCreacion.ToUniversalTime().ToString("o") } },
                    { "actualizado",   new { timestampValue = DateTime.UtcNow.ToString("o") } }
                }
            };
            var json = JsonSerializer.Serialize(doc);
            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            var token = Preferences.Get("firebase_id_token", null);
            req.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var resp = await _httpClient.SendAsync(req);
            resp.EnsureSuccessStatusCode();
        }

        // Agrega un TrabajoEsperado bajo un Entrenamiento.
        public async Task CrearTrabajoEsperadoAsync(string uid, string rutinaId, string diaId, TrabajoEsperado te)
        {
            var url =
              $"{FirestoreBaseUrl}/usuarios/{uid}/rutinas/{rutinaId}/entrenamientos/{diaId}/trabajoEsperado"
              + $"?documentId={Guid.NewGuid()}";

            var doc = new
            {
                fields = new Dictionary<string, object>
                {
                    { "IdEjercicio",     new { stringValue = te.IdEjercicio } },
                    { "NombreEjercicio", new { stringValue = te.NombreEjercicio } },
                    { "Series",          new { integerValue = te.Series } },
                    { "Repeticiones",    new { integerValue = te.Repeticiones } }
                }
            };
            var json = JsonSerializer.Serialize(doc);
            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            var token = Preferences.Get("firebase_id_token", null);
            req.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var resp = await _httpClient.SendAsync(req);
            resp.EnsureSuccessStatusCode();
        }
    }

}

