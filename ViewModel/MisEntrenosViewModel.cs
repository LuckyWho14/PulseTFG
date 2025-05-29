using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using PulseTFG.FirebaseService;
using PulseTFG.Models;

namespace PulseTFG.ViewModel
{
    public class MisEntrenosViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Rutina> Entrenos { get; }
            = new ObservableCollection<Rutina>();

        private readonly FirebaseFirestoreService _firestore
            = new FirebaseFirestoreService();

        public MisEntrenosViewModel()
        {
            // No cargamos aquí para evitar duplicados
        }

        // Carga rutinas, su cuenta de días y notifica a la UI
        public async Task LoadEntrenosAsync()
        {
            Entrenos.Clear();

            var uid = Preferences.Get("firebase_user_uid", null);
            if (string.IsNullOrEmpty(uid)) return;

            var lista = await _firestore.ObtenerRutinasUsuarioAsync(uid);
            foreach (var r in lista)
            {
                // Obtenemos también cuántos entrenamientos/días tiene
                r.DiasEntrenamientoCount
                  = await _firestore.ObtenerEntrenamientosCountAsync(uid, r.IdRutina);

                Entrenos.Add(r);
            }
            OnPropertyChanged(nameof(Entrenos));
        }

        // Actualiza solo el campo "activo"
        public async Task UpdateRutinaAsync(Rutina rutina)
        {
            var uid = Preferences.Get("firebase_user_uid", null);
            if (string.IsNullOrEmpty(uid)) return;

            await _firestore.ActualizarCampoRutinaAsync(
                uid,
                rutina.IdRutina,
                "activo",
                rutina.Activo);
        }

        // Elimina la rutina de Firestore y de la colección local
        public async Task DeleteRutinaAsync(Rutina rutina)
        {
            var uid = Preferences.Get("firebase_user_uid", null);
            if (string.IsNullOrEmpty(uid)) return;

            // Petición DELETE a Firestore REST
            var urlBase = "https://firestore.googleapis.com/v1/projects/pulsetfg-6642a/databases/(default)/documents";
            var url = $"{urlBase}/usuarios/{uid}/rutinas/{rutina.IdRutina}";
            using var client = new HttpClient();
            var req = new HttpRequestMessage(HttpMethod.Delete, url);
            var token = Preferences.Get("firebase_id_token", null);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var resp = await client.SendAsync(req);
            resp.EnsureSuccessStatusCode();

            // Lo quitamos de la lista local
            Entrenos.Remove(rutina);
            OnPropertyChanged(nameof(Entrenos));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
