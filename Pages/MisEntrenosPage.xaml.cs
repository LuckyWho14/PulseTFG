using System;
using System.Linq;
using Microsoft.Maui.Controls;
using PulseTFG.Models;
using PulseTFG.ViewModel;

namespace PulseTFG.Pages
{
    public partial class MisEntrenosPage : ContentPage
    {
        private MisEntrenosViewModel vm;

        public MisEntrenosPage()
        {
            InitializeComponent();
            BindingContext = vm = new MisEntrenosViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Recargamos rutinas (y su cuenta de días) solo aquí
            await vm.LoadEntrenosAsync();
        }
        
        // Añadir
        private async void OnAñadirEntrenamiento_Clicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("///CrearRutinaSelectTipoPage");

        // Editar
        private async void EditarButton_Clicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is Rutina r)
                try
                {
                    await Shell.Current.GoToAsync($"///CrearRutinaPersPage?rutinaId={r.IdRutina}");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error de navegación", ex.Message, "OK");
                }
        }

        // Activar
        private async void ActivarButton_Clicked(object sender, EventArgs e)
        {
            if (!(sender is Button btn) || !(btn.CommandParameter is Rutina r))
                return;

            // 1) Si ya está activa, avisamos y salimos
            if (r.Activo)
            {
                await DisplayAlert(
                    "Información",
                    $"La rutina “{r.Nombre}” ya está activa.",
                    "OK");
                return;
            }

            // 2) Confirmamos cambio
            bool ok = await DisplayAlert(
                "Confirmar",
                $"¿Deseas activar “{r.Nombre}”?",
                "Sí", "No");
            if (!ok)
                return;

            // 3) Desactivar la anterior
            var anterior = vm.Entrenos.FirstOrDefault(x => x.Activo);
            if (anterior != null)
            {
                anterior.Activo = false;
                await vm.UpdateRutinaAsync(anterior);
            }

            // 4) Activar la nueva
            r.Activo = true;
            await vm.UpdateRutinaAsync(r);

            // 5) Avisar del cambio
            await DisplayAlert(
                "Rutina activada",
                $"Ahora la rutina activa es “{r.Nombre}”.",
                "OK");
        }

        // Borrar
        private async void BorrarButton_Clicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is Rutina r)
            {
                // No permitir borrar si es la única
                if (vm.Entrenos.Count <= 1)
                {
                    await DisplayAlert(
                        "Atención",
                        "Debe haber al menos una rutina.",
                        "OK");
                    return;
                }

                // Si estaba activa, confirmamos que se activará otra
                if (r.Activo)
                {
                    bool ok = await DisplayAlert(
                        "Confirmar",
                        $"La rutina “{r.Nombre}” está activa. Al borrarla se activará otra. ¿Continuar?",
                        "Sí", "No");
                    if (!ok) return;
                }

                // Borramos
                await vm.DeleteRutinaAsync(r);

                // Si borramos la activa, activamos la primera que quede
                if (r.Activo)
                {
                    var siguiente = vm.Entrenos.First();
                    siguiente.Activo = true;
                    await vm.UpdateRutinaAsync(siguiente);
                    await DisplayAlert(
                        "Rutina activada",
                        $"Se ha activado “{siguiente.Nombre}”.",
                        "OK");
                }
            }
        }
    }
}
