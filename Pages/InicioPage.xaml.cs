using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using PulseTFG.FirebaseService;
using PulseTFG.ViewModel;
using PulseTFG.Models;

namespace PulseTFG.Pages
{
    public partial class InicioPage : ContentPage
    {
        readonly FirebaseFirestoreService _firestore = new();
        InicioViewModel _vm;

        public InicioPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var uid = Preferences.Get("firebase_user_uid", "");
            if (string.IsNullOrEmpty(uid))
            {
                await DisplayAlert("Error", "Usuario no autenticado.", "OK");
                return;
            }

            // Rutina y día  inicial
            var rutinas = await _firestore.ObtenerRutinasUsuarioAsync(uid);
            var activa = rutinas.Find(r => r.Activo);
            if (activa == null)
            {
                await DisplayAlert("Aviso", "No tienes rutina activa.", "OK");
                return;
            }

            _vm = new InicioViewModel(uid, activa.IdRutina);
            BindingContext = _vm;

            await _vm.InicializarAsync(primerDiaId: null);
        }

        private async void OnGuardarRegistro_Clicked(object sender, EventArgs e)
        {
            bool ok = await DisplayAlert(
                "Confirmar",
                "Guardar registros de este día y pasar al siguiente?",
                "Sí", "No");
            if (!ok) return;

            try
            {
                await _vm.GuardarYAvanzarDiaAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
