using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using PulseTFG.FirebaseService;
using PulseTFG.Models;

namespace PulseTFG.ViewModel
{
    public class RoutineCreatorViewModel : INotifyPropertyChanged
    {
        readonly FirebaseAuthService _auth = new();
        readonly FirebaseFirestoreService _firestore = new();

        public RoutineCreatorViewModel()
        {
            CambiarDiasCommand = new Command<int>(async v => await ActualizarDiasEntrenamiento(v));

            // Día 1 con Id asignado automáticamente
            DiasEntrenamientoLista.Add(new Entrenamiento
            {
                IdEntrenamiento = Guid.NewGuid().ToString(),
                Nombre = "Día 1",
                FechaCreacion = DateTime.Now,
                TrabajoEsperado = new ObservableCollection<TrabajoEsperado>()
            });
        }

        private bool tieneRutinas;
        public bool TieneRutinas
        {
            get => tieneRutinas;
            set
            {
                if (SetProperty(ref tieneRutinas, value))
                    OnPropertyChanged(nameof(NoTieneRutinas));
            }
        }
        public bool NoTieneRutinas => TieneRutinas;

        public async Task InitializeAsync()
        {
            var uid = Preferences.Get("firebase_user_uid", null);
            if (string.IsNullOrEmpty(uid))
            {
                TieneRutinas = false;
                return;
            }

            var lista = await _firestore.ObtenerRutinasUsuarioAsync(uid);
            TieneRutinas = lista != null && lista.Count > 0;
        }

        private string nombreRutina = "Mi rutina personalizada";
        public string NombreRutina
        {
            get => nombreRutina;
            set { nombreRutina = value; OnPropertyChanged(); }
        }

        private string descripcionRutina = "Descripción personalizada";
        public string DescripcionRutina
        {
            get => descripcionRutina;
            set { descripcionRutina = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Entrenamiento> DiasEntrenamientoLista { get; set; } = new();

        private int diasEntrenamiento = 1;
        public int DiasEntrenamiento
        {
            get => diasEntrenamiento;
            set { if (SetProperty(ref diasEntrenamiento, value)) { } }
        }

        public ICommand CambiarDiasCommand { get; }

        private async Task ActualizarDiasEntrenamiento(int nuevoValor)
        {
            if (nuevoValor > DiasEntrenamientoLista.Count)
            {
                for (int i = DiasEntrenamientoLista.Count; i < nuevoValor; i++)
                {
                    DiasEntrenamientoLista.Add(new Entrenamiento
                    {
                        IdEntrenamiento = Guid.NewGuid().ToString(),
                        Nombre = $"Día {i + 1}",
                        FechaCreacion = DateTime.Now,
                        TrabajoEsperado = new ObservableCollection<TrabajoEsperado>()
                    });
                }
            }
            else if (nuevoValor < DiasEntrenamientoLista.Count)
            {
                for (int i = DiasEntrenamientoLista.Count - 1; i >= nuevoValor; i--)
                {
                    var entrenamiento = DiasEntrenamientoLista[i];
                    if (EntrenamientoTieneContenido(entrenamiento))
                    {
                        var confirmar = await Application.Current.MainPage.DisplayAlert(
                            "Confirmar borrado",
                            $"¿Eliminar {entrenamiento.Nombre} con ejercicios asignados?",
                            "Sí", "No");
                        if (!confirmar) return;
                    }
                    DiasEntrenamientoLista.RemoveAt(i);
                }
            }
            DiasEntrenamiento = nuevoValor;
        }

        public bool EntrenamientoTieneContenido(Entrenamiento e)
            => e?.TrabajoEsperado?.Count > 0;

        public ObservableCollection<string> GruposMusculares { get; set; } = new()
        {
            "Todos", "Pecho", "Espalda", "Pierna", "Hombro", "Bíceps", "Tríceps", "Abdomen"
        };

        private Entrenamiento _entrenamientoActual;
        public Entrenamiento EntrenamientoActual
        {
            get => _entrenamientoActual;
            set
            {
                _entrenamientoActual = value;
                // Garantiza lista inicializada
                if (_entrenamientoActual != null && _entrenamientoActual.TrabajoEsperado == null)
                    _entrenamientoActual.TrabajoEsperado = new ObservableCollection<TrabajoEsperado>();
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Ejercicio> ejerciciosFiltrados = new();
        public ObservableCollection<Ejercicio> EjerciciosFiltrados
        {
            get => ejerciciosFiltrados;
            set { SetProperty(ref ejerciciosFiltrados, value); }
        }

        public async Task CargarEjerciciosAsync(bool soloFav, string grupo)
        {
            var list = await _firestore.ObtenerEjerciciosFiltradosAsync(soloFav, grupo);
            EjerciciosFiltrados.Clear();
            foreach (var e in list)
                EjerciciosFiltrados.Add(e);
        }

        public void Reset()
        {
            // Restablece texto
            NombreRutina = "Mi rutina personalizada";
            DescripcionRutina = "Descripción personalizada";

            // Restablece días + lista
            diasEntrenamiento = 1;
            OnPropertyChanged(nameof(DiasEntrenamiento));
            DiasEntrenamientoLista.Clear();
            DiasEntrenamientoLista.Add(new Entrenamiento
            {
                IdEntrenamiento = Guid.NewGuid().ToString(),
                Nombre = "Día 1",
                FechaCreacion = DateTime.Now,
                TrabajoEsperado = new ObservableCollection<TrabajoEsperado>()
            });
        }

        public async Task CargarRutinaExistenteAsync(string uid, string rutinaId)
        {
            var rutina = await _firestore.ObtenerRutinaPorIdAsync(uid, rutinaId);
            if (rutina == null) return;

            NombreRutina = rutina.Nombre;
            DescripcionRutina = rutina.Descripcion;

            var entrenos = await _firestore.ObtenerEntrenamientosDeRutinaAsync(uid, rutinaId);

            DiasEntrenamientoLista.Clear();
            foreach (var ent in entrenos)
            {
                var trabajos = await _firestore.ObtenerTrabajoEsperadoDeEntrenamientoAsync(uid, rutinaId, ent.IdEntrenamiento);
                ent.TrabajoEsperado = new ObservableCollection<TrabajoEsperado>(trabajos);
                DiasEntrenamientoLista.Add(ent);
            }

            DiasEntrenamiento = DiasEntrenamientoLista.Count;
        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value)) return false;
            backingStore = value;
            OnPropertyChanged(propName);
            return true;
        }
        protected void OnPropertyChanged([CallerMemberName] string propName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        #endregion
    }
}
