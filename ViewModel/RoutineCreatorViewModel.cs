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

        // NUEVO: Lista de entrenamientos por día
        public ObservableCollection<Entrenamiento> DiasEntrenamientoLista { get; set; } = new();

        private int diasEntrenamiento = 1;
        public int DiasEntrenamiento
        {
            get => diasEntrenamiento;
            set
            {
                if (SetProperty(ref diasEntrenamiento, value))
                {
                    // se puede usar si necesitas reaccionar directamente
                }
            }
        }

        public ICommand CambiarDiasCommand { get; }

        public RoutineCreatorViewModel()
        {
            CambiarDiasCommand = new Command<int>(async (nuevoValor) => await ActualizarDiasEntrenamiento(nuevoValor));

            // Siempre hay al menos un entrenamiento
            DiasEntrenamientoLista.Add(new Entrenamiento
            {
                Nombre = "Día 1",
                FechaCreacion = DateTime.Now
            });
        }

        private async Task ActualizarDiasEntrenamiento(int nuevoValor)
        {
            if (nuevoValor > DiasEntrenamientoLista.Count)
            {
                for (int i = DiasEntrenamientoLista.Count; i < nuevoValor; i++)
                {
                    DiasEntrenamientoLista.Add(new Entrenamiento
                    {
                        Nombre = $"Día {i + 1}",
                        FechaCreacion = DateTime.Now
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

                        if (!confirmar)
                            return;
                    }

                    DiasEntrenamientoLista.RemoveAt(i);
                }
            }

            DiasEntrenamiento = nuevoValor;
        }

        public bool EntrenamientoTieneContenido(Entrenamiento entrenamiento)
        {
            // Aquí va la lógica real para saber si tiene contenido
            // Por ahora devolvemos false (vacío)
            return false;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
