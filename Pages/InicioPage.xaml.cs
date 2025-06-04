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
        private string rutinaActivaAnteriorId;
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

            // Obtener rutina activa actual
            var rutinas = await _firestore.ObtenerRutinasUsuarioAsync(uid);
            var activa = rutinas.Find(r => r.Activo);
            if (activa == null)
            {
                await DisplayAlert("Aviso", "No tienes rutina activa.", "OK");
                return;
            }

            if (_vm == null || activa.IdRutina != rutinaActivaAnteriorId)
            {
                rutinaActivaAnteriorId = activa.IdRutina;
                _vm = new InicioViewModel(uid, activa.IdRutina);
                BindingContext = _vm;
                await _vm.InicializarAsync(primerDiaId: null);
            }
        }


        private async void OnGuardarRegistro_Clicked(object sender, EventArgs e)
        {
            try
            {
                await _vm.GuardarDiaActualAsync();
                await DisplayAlert("Guardado", "Registros guardados correctamente.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void OnAnteriorDia_Clicked(object sender, EventArgs e)
        {
            _vm.MoverADiaAnterior();
        }

        private void OnSiguienteDia_Clicked(object sender, EventArgs e)
        {
            _vm.MoverADiaSiguiente();
        }

    }
}
