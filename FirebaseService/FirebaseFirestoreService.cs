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
using Microsoft.Win32;
using PulseTFG.Converter;


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
                    { "Repeticiones",    new { integerValue = te.Repeticiones } },
                    { "Orden",           new { integerValue = te.Orden } }
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
        public async Task<int> ObtenerEntrenamientosCountAsync(string uid, string rutinaId)
        {
            var url = $"{FirestoreBaseUrl}/usuarios/{uid}/rutinas/{rutinaId}/entrenamientos";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            var token = Preferences.Get("firebase_id_token", null);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var resp = await _httpClient.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return 0;
            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            return doc.RootElement.TryGetProperty("documents", out var docs)
                ? docs.GetArrayLength()
                : 0;
        }

        // Obtiene la lista de TrabajoEsperado bajo un Entrenamiento específico.
        public async Task<List<TrabajoEsperado>> ObtenerTrabajoEsperadoAsync(
                string uid, string rutinaId, string diaId)
        {
            var url = $"{FirestoreBaseUrl}/usuarios/{uid}"
                    + $"/rutinas/{rutinaId}"
                    + $"/entrenamientos/{diaId}/trabajoEsperado";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            var token = Preferences.Get(PrefsIdTokenKey, null);
            req.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var resp = await _httpClient.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
                return new List<TrabajoEsperado>();

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var lista = new List<TrabajoEsperado>();
            if (!doc.RootElement.TryGetProperty("documents", out var docs))
                return lista;

            foreach (var d in docs.EnumerateArray())
            {
                var name = d.GetProperty("name").GetString();
                var id = name.Substring(name.LastIndexOf('/') + 1);
                var f = d.GetProperty("fields");

                // Lectura segura de 'Orden'
                int orden = 0;
                if (f.TryGetProperty("Orden", out var ordenProp) &&
                    ordenProp.TryGetProperty("integerValue", out var iv) &&
                    int.TryParse(iv.GetString(), out var parsed))
                {
                    orden = parsed;
                }

                lista.Add(new TrabajoEsperado
                {
                    IdTrabajoEsperado = id,
                    IdEjercicio = f.GetProperty("IdEjercicio")
                                           .GetProperty("stringValue").GetString(),
                    NombreEjercicio = f.GetProperty("NombreEjercicio")
                                           .GetProperty("stringValue").GetString(),
                    Series = int.Parse(f.GetProperty("Series")
                                           .GetProperty("integerValue").GetString()),
                    Repeticiones = int.Parse(f.GetProperty("Repeticiones")
                                           .GetProperty("integerValue").GetString()),
                    Orden = orden   // ya nunca falla aunque falte el campo
                });
            }

            // Ordena por 'Orden' (0 al final) y devuelve
            return lista.OrderBy(x => x.Orden).ToList();
        }

      
        // Obtiene la lista de Entrenamientos bajo una rutina dada.
        
        public async Task<List<Entrenamiento>> ObtenerEntrenamientosAsync(string uid, string rutinaId)
        {
            var url = $"{FirestoreBaseUrl}/usuarios/{uid}/rutinas/{rutinaId}/entrenamientos";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            var token = Preferences.Get(PrefsIdTokenKey, null);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await _httpClient.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
                return new List<Entrenamiento>();

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("documents", out var docs))
                return new List<Entrenamiento>();

            var lista = new List<Entrenamiento>();
            foreach (var d in docs.EnumerateArray())
            {
                var name = d.GetProperty("name").GetString();
                var id = name[(name.LastIndexOf('/') + 1)..];
                var f = d.GetProperty("fields");
                lista.Add(new Entrenamiento
                {
                    IdEntrenamiento = id,
                    Nombre = f.GetProperty("nombre").GetProperty("stringValue").GetString(),
                    FechaCreacion = DateTime.Parse(
                                          f.GetProperty("fechaCreacion")
                                           .GetProperty("timestampValue")
                                           .GetString()
                                      ),
                    Actualizado = DateTime.Parse(
                                          f.GetProperty("actualizado")
                                           .GetProperty("timestampValue")
                                           .GetString()
                                      )
                });
            }
            return lista;
        }

        // Obtiene el último registro de un ejercicio específico de un usuario.
        public async Task<Registro> ObtenerUltimoRegistroAsync(string uid, string idEjercicio)
        {
            var url = $"{FirestoreBaseUrl}:runQuery";
            var idToken = Preferences.Get(PrefsIdTokenKey, null);

            var structuredQuery = new
            {
                structuredQuery = new
                {
                    from = new[] { new { collectionId = "registros", allDescendants = false } },
                    where = new
                    {
                        fieldFilter = new
                        {
                            field = new { fieldPath = "IdEjercicio" },
                            op = "EQUAL",
                            value = new { stringValue = idEjercicio }
                        }
                    },
                    orderBy = new[]
                    {
                new { field = new { fieldPath = "Fecha" }, direction = "DESCENDING" }
            },
                    limit = 1
                }
            };
            var jsonBody = JsonSerializer.Serialize(structuredQuery);
            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

            var resp = await _httpClient.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;

            var respJson = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse($"[ {respJson.Trim()} ]");
            var elem = doc.RootElement.EnumerateArray().FirstOrDefault();
            if (!elem.TryGetProperty("document", out var document)) return null;

            var f = document.GetProperty("fields");
            return new Registro
            {
                IdTrabajo = f.GetProperty("IdTrabajo").GetProperty("stringValue").GetString(),
                IdEjercicio = f.GetProperty("IdEjercicio").GetProperty("stringValue").GetString(),
                NombreEjercicio = f.GetProperty("NombreEjercicio").GetProperty("stringValue").GetString(),
                Peso = int.Parse(f.GetProperty("Peso").GetProperty("integerValue").GetString()),
                Repeticion = int.Parse(f.GetProperty("Repeticion").GetProperty("integerValue").GetString()),
                Serie = int.Parse(f.GetProperty("Serie").GetProperty("integerValue").GetString()),
                Intensidad = int.Parse(f.GetProperty("Intensidad").GetProperty("integerValue").GetString()),
                Hecho = f.GetProperty("Hecho").GetProperty("booleanValue").GetBoolean(),
                Notas = f.TryGetProperty("Notas", out var n) ? n.GetProperty("stringValue").GetString() : "",
                Fecha = DateTime.Parse(f.GetProperty("Fecha").GetProperty("timestampValue").GetString())
            };
        }

        
        // Guarda un Registro en Firestore bajo /usuarios/{uid}/registros/{registroId}
        public async Task CrearRegistroAsync(string uid, Registro r)
        {
            var url = $"{FirestoreBaseUrl}/usuarios/{uid}/registros?documentId={Guid.NewGuid()}";
            var doc = new
            {
                fields = new Dictionary<string, object>
        {
            { "IdTrabajo",       new { stringValue  = r.IdTrabajo } },
            { "IdEjercicio",     new { stringValue  = r.IdEjercicio } },   // <-- stringValue
            { "NombreEjercicio", new { stringValue  = r.NombreEjercicio } },
            { "Peso",            new { doubleValue  = r.Peso } },
            { "Repeticion",      new { integerValue = r.Repeticion } },
            { "Serie",           new { integerValue = r.Serie } },
            { "Intensidad",      new { integerValue = r.Intensidad } },
            { "Hecho",           new { booleanValue = r.Hecho } },
            { "Notas",           new { stringValue  = r.Notas ?? "" } },
            { "Fecha",           new { timestampValue = r.Fecha.ToUniversalTime().ToString("o") } },
        }
            };
            var json = JsonSerializer.Serialize(doc);
            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            var token = Preferences.Get(PrefsIdTokenKey, null);
            req.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            var resp = await _httpClient.SendAsync(req);
            resp.EnsureSuccessStatusCode();
        }

        /// Obtiene el último registro anterior a hoy de un ejercicio específico de un usuario.
        public async Task<Registro> ObtenerUltimoRegistroAnteriorAsync(string uid, string idEjercicio)
        {
            var url = $"{FirestoreBaseUrl}:runQuery";
            var token = Preferences.Get(PrefsIdTokenKey, null);

            var hoyIso = DateTime.UtcNow.Date.ToString("o");
            var structuredQuery = new
            {
                structuredQuery = new
                {
                    from = new[] { new { collectionId = "registros", allDescendants = false } },
                    where = new
                    {
                        compositeFilter = new
                        {
                            op = "AND",
                            filters = new object[]
                            {
                        new {
                            fieldFilter = new {
                                field = new { fieldPath = "IdEjercicio" },
                                op = "EQUAL",
                                value = new { stringValue = idEjercicio }
                            }
                        },
                        new {
                            fieldFilter = new {
                                field = new { fieldPath = "Fecha" },
                                op = "LESS_THAN",
                                value = new { timestampValue = hoyIso }
                            }
                        }
                            }
                        }
                    },
                    orderBy = new[]
                    {
                new { field = new { fieldPath = "Fecha" }, direction = "DESCENDING" }
            },
                    limit = 1
                }
            };

            var json = JsonSerializer.Serialize(structuredQuery);
            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await _httpClient.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;

            var body = await resp.Content.ReadAsStringAsync();
            using var arr = JsonDocument.Parse($"[{body.Trim()}]");
            var elem = arr.RootElement.EnumerateArray().FirstOrDefault();
            if (!elem.TryGetProperty("document", out var docElem)) return null;

            var f = docElem.GetProperty("fields");
            return new Registro
            {
                IdTrabajo = f.GetProperty("IdTrabajo").GetProperty("stringValue").GetString(),
                IdEjercicio = f.GetProperty("IdEjercicio").GetProperty("stringValue").GetString(),
                NombreEjercicio = f.GetProperty("NombreEjercicio").GetProperty("stringValue").GetString(),
                Peso = int.Parse(f.GetProperty("Peso").GetProperty("integerValue").GetString()),
                Repeticion = int.Parse(f.GetProperty("Repeticion").GetProperty("integerValue").GetString()),
                Serie = int.Parse(f.GetProperty("Serie").GetProperty("integerValue").GetString()),
                Intensidad = int.Parse(f.GetProperty("Intensidad").GetProperty("integerValue").GetString()),
                Hecho = f.GetProperty("Hecho").GetProperty("booleanValue").GetBoolean(),
                Notas = f.TryGetProperty("Notas", out var n) ? n.GetProperty("stringValue").GetString() : "",
                Fecha = DateTime.Parse(f.GetProperty("Fecha").GetProperty("timestampValue").GetString())
            };
        }

       
        // Obtiene todos los registros de un usuario en Firestore (colección "/usuarios/{uid}/registros").
       
        public async Task<List<Registro>> ObtenerRegistrosUsuarioAsync(string uid)
        {
            var token = Preferences.Get(PrefsIdTokenKey, null);
            if (string.IsNullOrEmpty(token))
                return new List<Registro>();

            // Construimos la URL para GET /usuarios/{uid}/registros
            var url = $"{FirestoreBaseUrl}/usuarios/{uid}/registros";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await _httpClient.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
                return new List<Registro>();

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var lista = new List<Registro>();
            // Si no hay campo "documents", devolvemos lista vacía
            if (!doc.RootElement.TryGetProperty("documents", out var docs))
                return lista;

            foreach (var d in docs.EnumerateArray())
            {
                var fields = d.GetProperty("fields");

                // Mapear cada campo al modelo Registro (ver ModeloFirestore.txt :contentReference[oaicite:1]{index=1})
                lista.Add(new Registro
                {
                    IdTrabajo = fields.GetProperty("IdTrabajo").GetProperty("stringValue").GetString(),
                    NombreEjercicio = fields.GetProperty("NombreEjercicio").GetProperty("stringValue").GetString(),
                    Peso = fields.GetProperty("Peso").TryGetProperty("doubleValue", out var dv)
                            ? dv.GetDouble()
                            : double.Parse(fields.GetProperty("Peso").GetProperty("integerValue").GetString()),
                    Repeticion = int.Parse(fields.GetProperty("Repeticion").GetProperty("integerValue").GetString()),
                    Serie = int.Parse(fields.GetProperty("Serie").GetProperty("integerValue").GetString()),
                    Intensidad = int.Parse(fields.GetProperty("Intensidad").GetProperty("integerValue").GetString()),
                    Hecho = fields.GetProperty("Hecho").GetProperty("booleanValue").GetBoolean(),
                    Notas = fields.TryGetProperty("Notas", out var n)
                                        ? n.GetProperty("stringValue").GetString()
                                        : string.Empty,
                    Fecha = DateTime.Parse(fields.GetProperty("Fecha").GetProperty("timestampValue").GetString())
                });
            }

            return lista;
        }

        // Obtiene una rutina específica por su ID.
        public async Task<Rutina> ObtenerRutinaPorIdAsync(string uid, string rutinaId)
        {
            var url = $"{FirestoreBaseUrl}/usuarios/{uid}/rutinas/{rutinaId}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var fields = root.GetProperty("fields");

            return new Rutina
            {
                IdRutina = rutinaId,
                Nombre = fields.GetProperty("nombre").GetProperty("stringValue").GetString(),
                Descripcion = fields.GetProperty("descripcion").GetProperty("stringValue").GetString(),
                Activo = fields.GetProperty("activo").GetProperty("booleanValue").GetBoolean(),
                FechaCreacion = DateTime.Parse(fields.GetProperty("fechaCreacion").GetProperty("timestampValue").GetString()),
                Actualizado = DateTime.Parse(fields.GetProperty("actualizado").GetProperty("timestampValue").GetString())
            };
        }

        // Obtiene los entrenamientos de una rutina específica.
        public async Task<List<Entrenamiento>> ObtenerEntrenamientosDeRutinaAsync(string uid, string rutinaId)
        {
            var url = $"{FirestoreBaseUrl}/usuarios/{uid}/rutinas/{rutinaId}/entrenamientos";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            var token = Preferences.Get("firebase_id_token", null);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await _httpClient.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
                return new List<Entrenamiento>();

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("documents", out var docs))
                return new List<Entrenamiento>();

            var lista = new List<Entrenamiento>();
            foreach (var d in docs.EnumerateArray())
            {
                var name = d.GetProperty("name").GetString();
                var id = name.Substring(name.LastIndexOf('/') + 1);
                var f = d.GetProperty("fields");

                lista.Add(new Entrenamiento
                {
                    IdEntrenamiento = id,
                    Nombre = f.GetProperty("nombre").GetProperty("stringValue").GetString(),
                    FechaCreacion = DateTime.Parse(f.GetProperty("fechaCreacion").GetProperty("timestampValue").GetString()),
                    Actualizado = DateTime.Parse(f.GetProperty("actualizado").GetProperty("timestampValue").GetString()),
                    TrabajoEsperado = new System.Collections.ObjectModel.ObservableCollection<TrabajoEsperado>()
                });
            }

            return lista;
        }

        // Obtiene el trabajo esperado de un entrenamiento específico.
        public async Task<List<TrabajoEsperado>> ObtenerTrabajoEsperadoDeEntrenamientoAsync(string uid, string rutinaId, string entrenamientoId)
        {
            var url = $"{FirestoreBaseUrl}/usuarios/{uid}/rutinas/{rutinaId}/entrenamientos/{entrenamientoId}/trabajoEsperado";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            var token = Preferences.Get("firebase_id_token", null);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await _httpClient.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
                return new List<TrabajoEsperado>();

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var lista = new List<TrabajoEsperado>();
            if (!doc.RootElement.TryGetProperty("documents", out var docs))
                return lista;

            foreach (var d in docs.EnumerateArray())
            {
                var name = d.GetProperty("name").GetString();
                var id = name.Substring(name.LastIndexOf('/') + 1);
                var f = d.GetProperty("fields");

                int orden = 0;
                if (f.TryGetProperty("Orden", out var ordenProp) &&
                    ordenProp.TryGetProperty("integerValue", out var iv) &&
                    int.TryParse(iv.GetString(), out var parsed))
                {
                    orden = parsed;
                }

                lista.Add(new TrabajoEsperado
                {
                    IdTrabajoEsperado = id,
                    IdEjercicio = f.GetProperty("IdEjercicio").GetProperty("stringValue").GetString(),
                    NombreEjercicio = f.GetProperty("NombreEjercicio").GetProperty("stringValue").GetString(),
                    Series = int.Parse(f.GetProperty("Series").GetProperty("integerValue").GetString()),
                    Repeticiones = int.Parse(f.GetProperty("Repeticiones").GetProperty("integerValue").GetString()),
                    Orden = orden
                });
            }

            return lista.OrderBy(x => x.Orden).ToList();
        }



        // Borra un TrabajoEsperado específico de un Entrenamiento.
        public async Task BorrarTrabajoEsperadoAsync(string uid, string rutinaId, string diaId, string idTrabajo)
        {
            var url = $"{FirestoreBaseUrl}/usuarios/{uid}/rutinas/{rutinaId}/entrenamientos/{diaId}/trabajoEsperado/{idTrabajo}";
            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            var token = Preferences.Get("firebase_id_token", null);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var resp = await _httpClient.SendAsync(req);
            resp.EnsureSuccessStatusCode();
        }

        // Borra un Entrenamiento específico de una Rutina.
        public async Task BorrarEntrenamientoAsync(string uid, string rutinaId, string idEntrenamiento)
        {
            var url = $"{FirestoreBaseUrl}/usuarios/{uid}/rutinas/{rutinaId}/entrenamientos/{idEntrenamiento}";
            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            var token = Preferences.Get("firebase_id_token", null);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var resp = await _httpClient.SendAsync(req);
            resp.EnsureSuccessStatusCode();
        }

        // Actualiza un campo genérico de una rutina específica.
        public async Task ActualizarCampoRutinaGenericoAsync(string uid, string rutinaId, string campo, object valor)
        {
            string tipoFirestore;
            object firestoreValue;

            switch (valor)
            {
                case string s:
                    tipoFirestore = "stringValue";
                    firestoreValue = s;
                    break;
                case bool b:
                    tipoFirestore = "booleanValue";
                    firestoreValue = b;
                    break;
                case int i:
                    tipoFirestore = "integerValue";
                    firestoreValue = i;
                    break;
                case double d:
                    tipoFirestore = "doubleValue";
                    firestoreValue = d;
                    break;
                case DateTime dt:
                    tipoFirestore = "timestampValue";
                    firestoreValue = dt.ToUniversalTime().ToString("o");
                    break;
                default:
                    throw new ArgumentException("Tipo no soportado");
            }

            var url = $"{FirestoreBaseUrl}/usuarios/{uid}/rutinas/{rutinaId}?updateMask.fieldPaths={campo}";

            var doc = new
            {
                fields = new Dictionary<string, object>
        {
            { campo, new Dictionary<string, object> { { tipoFirestore, firestoreValue } } }
        }
            };

            var json = JsonSerializer.Serialize(doc);
            var req = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var token = Preferences.Get("firebase_id_token", null);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await _httpClient.SendAsync(req);
            resp.EnsureSuccessStatusCode();
        }

        // Elimina una rutina completa junto con todos sus entrenamientos y trabajos esperados.
        public async Task EliminarRutinaConEntrenamientosAsync(string uid, string rutinaId)
        {
            var entrenamientos = await ObtenerEntrenamientosAsync(uid, rutinaId);

            foreach (var entrenamiento in entrenamientos)
            {
                var trabajos = await ObtenerTrabajoEsperadoAsync(uid, rutinaId, entrenamiento.IdEntrenamiento);

                foreach (var trabajo in trabajos)
                {
                    await BorrarTrabajoEsperadoAsync(uid, rutinaId, entrenamiento.IdEntrenamiento, trabajo.IdTrabajoEsperado);
                }

                await BorrarEntrenamientoAsync(uid, rutinaId, entrenamiento.IdEntrenamiento);
            }

            // Por último, borrar la rutina en sí
            var url = $"https://firestore.googleapis.com/v1/projects/pulsetfg-6642a/databases/(default)/documents/usuarios/{uid}/rutinas/{rutinaId}";
            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            var token = Preferences.Get("firebase_id_token", null);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await _httpClient.SendAsync(req);
            resp.EnsureSuccessStatusCode();
        }

    }
}

