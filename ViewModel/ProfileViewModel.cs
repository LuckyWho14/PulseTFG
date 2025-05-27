using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using PulseTFG.FirebaseService;
using PulseTFG.Models;

namespace PulseTFG.ViewModel
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        readonly FirebaseAuthService _auth = new();
        readonly FirebaseFirestoreService _firestore = new();
        const string PrefsUserUidKey = "firebase_user_uid";

        public ProfileViewModel()
        {
            _ = LoadUserAsync();
        }

        private string nombre;
        public string Nombre
        {
            get => nombre;
            set => SetProperty(ref nombre, value);
        }

        private string email;
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        private DateTime fechaNacimiento;
        public DateTime FechaNacimiento
        {
            get => fechaNacimiento;
            set => SetProperty(ref fechaNacimiento, value);
        }

        private double altura;
        public double Altura
        {
            get => altura;
            set
            {
                if (SetProperty(ref altura, value))
                    OnPropertyChanged(nameof(IMC));
            }
        }

        private double peso;
        public double Peso
        {
            get => peso;
            set
            {
                if (SetProperty(ref peso, value))
                {
                    OnPropertyChanged(nameof(IMC));
                }
            }
        }

        // Propiedad calculada de IMC
        public double IMC => Altura > 0
            ? Math.Round(Peso / Math.Pow(Altura / 100.0, 2), 2)
            : 0;

        // Comandos para ajustar peso
        public ICommand IncreasePesoCommand => new Command(() => Peso += 0.25);
        public ICommand DecreasePesoCommand => new Command(() => Peso = Math.Max(0, Peso - 0.25));

        public async Task LoadUserAsync()
        {
            var uid = Preferences.Get("firebase_user_uid", null);
            if (string.IsNullOrEmpty(uid))
                return;

            var u = await _firestore.ObtenerUsuarioAsync(uid);

            Nombre = u.NombreCompleto;
            Email = u.Email;
            FechaNacimiento = u.FechaNacimiento;
            Altura = u.Altura;
            Peso = u.Peso;
        }

        /// <summary>
        /// Comando que, al pulsarlo, persiste el peso actual en Firestore.
        /// </summary>
        public ICommand GuardarPesoCommand => new Command(async () =>
        {
            await Application.Current.MainPage.DisplayAlert("Éxito", "Cambio de peso guardado", "OK");
            await UpdatePesoInDatabaseAsync();
        });

        // Método para actualizar el peso en Firestore
        private async Task UpdatePesoInDatabaseAsync()
        {
            var uid = Preferences.Get(PrefsUserUidKey, null);
            if (string.IsNullOrEmpty(uid))
                return;

            // Llamamos al servicio para actualizar solo el campo 'peso'
            await _firestore.ActualizarCampoUsuarioAsync(uid, "peso", Peso);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;
            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
