using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using PulseTFG.AuthService;
using PulseTFG.Models;

namespace PulseTFG.ViewModel
{
    public class RegisterViewModel
    {
        private readonly FirebaseAuthService _authService = new FirebaseAuthService();

        // Datos de registro
        public string Email { get; set; }
        public string Password { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaNacimiento { get; set; } = DateTime.Now;
        public double Altura { get; set; }
        public double Peso { get; set; }

        // Marca de creación para guardar en Firestore
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public ICommand RegisterCommand => new Command(async () =>
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(NombreCompleto))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Por favor, rellena todos los campos obligatorios.",
                    "OK");
                return;
            }

            try
            {
                // Registro en Firebase Auth
                var resultado = await _authService.RegisterAsync(Email, Password);
                await _authService.EnviarEmailVerificacionAsync(resultado.IdToken);

                // Crear la entidad Usuario con fecha de creación
                var usuario = new Usuario
                {
                    Uid = resultado.LocalId,
                    Email = Email,
                    NombreCompleto = NombreCompleto,
                    FechaNacimiento = FechaNacimiento,
                    Altura = Altura,
                    Peso = Peso,
                    FechaCreacion = FechaCreacion
                };

                // Guardar en Firestore
                await _authService.GuardarUsuarioAsync(usuario, resultado.IdToken);

                await Application.Current.MainPage.DisplayAlert(
                    "Éxito",
                    "Usuario registrado correctamente. Revisa tu correo para verificar la cuenta.",
                    "OK");

                // Navegar a login
                await Shell.Current.GoToAsync("///LoginPage"); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR Registro]: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    ex.Message,
                    "OK");
            }
        });
    }
}
