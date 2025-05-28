using PulseTFG.FirebaseService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PulseTFG.Models;

namespace PulseTFG.ViewModel
{
    public class GlosarioEjerViewModel : INotifyPropertyChanged
    {
        private readonly FirebaseFirestoreService _firestoreService = new();
        public ObservableCollection<Ejercicio> ListaEjercicios { get; set; } = new();

        public ObservableCollection<string> GruposMusculares { get; set; } = new()
    {
        "Todos", "Pecho", "Espalda", "Pierna", "Hombro", "Bíceps", "Tríceps"
    };

        private string grupoSeleccionado = "Todos";
        public string GrupoSeleccionado
        {
            get => grupoSeleccionado;
            set
            {
                grupoSeleccionado = value;
                OnPropertyChanged(nameof(GrupoSeleccionado));
                _ = CargarEjercicios();
            }
        }

        private bool soloFavoritos;
        public bool SoloFavoritos
        {
            get => soloFavoritos;
            set
            {
                soloFavoritos = value;
                OnPropertyChanged(nameof(SoloFavoritos));
                _ = CargarEjercicios();
            }
        }

        public ICommand LimpiarFiltrosCommand { get; }

        private bool listaVacia;
        public bool ListaVacia
        {
            get => listaVacia;
            set
            {
                listaVacia = value;
                OnPropertyChanged(nameof(ListaVacia));
            }
        }

        public GlosarioEjerViewModel()
        {
            LimpiarFiltrosCommand = new Command(() =>
            {
                SoloFavoritos = false;
                GrupoSeleccionado = "Todos";
            });

            _ = CargarEjercicios();
        }

        private async Task CargarEjercicios()
        {
            System.Diagnostics.Debug.WriteLine("🚀 Cargando ejercicios...");

            var ejercicios = await _firestoreService.ObtenerEjerciciosFiltradosAsync(SoloFavoritos, GrupoSeleccionado);

            ListaEjercicios.Clear();

            foreach (var e in ejercicios)
            {
                // Asignar comando para alternar favorito
                e.ToggleFavoritoCommand = new Command(async () => await CambiarFavoritoAsync(e));
                ListaEjercicios.Add(e);
            }

            ListaVacia = ListaEjercicios.Count == 0;

            System.Diagnostics.Debug.WriteLine($"✅ Cargados {ListaEjercicios.Count} ejercicios");
        }


        private async Task CambiarFavoritoAsync(Ejercicio ejercicio)
        {
            var uid = Preferences.Get("firebase_user_uid", null);

            if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(ejercicio.IdEjercicio))
                return;

            if (ejercicio.EsFavorito)
            {
                // Quitar de favoritos
                await _firestoreService.EliminarFavoritoAsync(uid, ejercicio.IdEjercicio);
                ejercicio.EsFavorito = false;
            }
            else
            {
                // Añadir a favoritos
                await _firestoreService.AgregarFavoritoAsync(uid, ejercicio.IdEjercicio);
                ejercicio.EsFavorito = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
