using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using PulseTFG.AuthService;
using PulseTFG.Models;

namespace PulseTFG.ViewModel
{
    public class RoutineCreatorViewModel : INotifyPropertyChanged
    {
        readonly FirebaseAuthService _auth = new();
        public RoutineCreatorViewModel()
        {
            FechaCreacion = DateTime.UtcNow;
            Actualizado = DateTime.UtcNow;
        }
        
        bool tieneRutinas;
        public bool TieneRutinas
        {
            get => tieneRutinas;
            set => SetProperty(ref tieneRutinas, value);
        }
        
        private string nombre;
        public string Nombre
        {
            get => nombre;
            set => SetProperty(ref nombre, value);
        }

        private string descripcion;
        public string Descripcion
        {
            get => descripcion;
            set => SetProperty(ref descripcion, value);
        }

        private bool activo = true;
        public bool Activo
        {
            get => activo;
            set => SetProperty(ref activo, value);
        }

        public DateTime FechaCreacion { get; }
        public DateTime Actualizado { get; }

        private string mensaje;
        public string Mensaje
        {
            get => mensaje;
            set => SetProperty(ref mensaje, value);
        }

        public ICommand CreateRoutineCommand => new Command(async () =>
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                Mensaje = "El nombre es obligatorio";
                return;
            }

            var uid = Preferences.Get("firebase_user_uid", null);
            if (uid == null)
            {
                Mensaje = "Usuario no autenticado";
                return;
            }

            var r = new Rutina
            {
                Nombre = Nombre,
                Descripcion = Descripcion ?? "",
                Activo = Activo,
                FechaCreacion = FechaCreacion,
                Actualizado = Actualizado
            };

            try
            {
                var creada = await _auth.CrearRutinaAsync(uid, r);
                Mensaje = "Rutina creada: " + creada.Nombre;
                // mensaje de éxito, puedes navegar a otra página o actualizar la UI
                await Application.Current.MainPage.DisplayAlert("Éxito", "Rutina creada correctamente", "OK");

                // lleva a la pagina inicial
                await Shell.Current.GoToAsync("//InicioPage");
            }
            catch (Exception ex)
            {
                Mensaje = "Error: " + ex.Message;
            }
        });

        /// <summary>
        /// Inicializa el ViewModel comprobando si el usuario tiene rutinas.
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            var uid = Preferences.Get("firebase_user_uid", null);
            if (string.IsNullOrEmpty(uid))
            {
                TieneRutinas = false;
                return;
            }

            // Llama a tu servicio para recuperar la lista
            var lista = await _auth.ObtenerRutinasUsuarioAsync(uid);
            TieneRutinas = lista != null && lista.Count > 0;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        bool SetProperty<T>(ref T backing, T value, [CallerMemberName] string p = null)
        {
            if (Equals(backing, value)) return false;
            backing = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
            return true;
        }
        #endregion
    }
}
