using PulseTFG.ViewModel;
using PulseTFG.Models;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace PulseTFG.Pages
{
    public partial class CrearRutinaPersPage : ContentPage
    {
        RoutineCreatorViewModel _vm;

        public CrearRutinaPersPage()
        {
            InitializeComponent();
            _vm = new RoutineCreatorViewModel();
            BindingContext = _vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _vm.InitializeAsync();
        }

        private async void OnVolverClicked(object sender, EventArgs e)
        {
            _vm.Reset();

            await Shell.Current.GoToAsync("//CrearRutinaSelecTipoPage");
        }

        private async void CambiarNombre_Clicked(object sender, EventArgs e)
        {
            string nuevo = await DisplayPromptAsync("Nombre de rutina", "Introduce un nuevo nombre:", initialValue: _vm.NombreRutina);
            if (!string.IsNullOrWhiteSpace(nuevo))
                _vm.NombreRutina = nuevo;
        }

        private async void CambiarDescripcion_Clicked(object sender, EventArgs e)
        {
            string desc = await DisplayPromptAsync("Descripción", "Introduce una nueva descripción:", initialValue: _vm.DescripcionRutina);
            if (!string.IsNullOrWhiteSpace(desc))
                _vm.DescripcionRutina = desc;
        }

        private void Stepper_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            _vm.CambiarDiasCommand.Execute((int)e.NewValue);
        }

        private async void BorrarDia_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is Entrenamiento dia)
            {
                if (_vm.EntrenamientoTieneContenido(dia))
                {
                    bool ok = await DisplayAlert(
                        "Confirmar borrado",
                        $"¿Eliminar {dia.Nombre} con ejercicios asignados?",
                        "Sí", "No");
                    if (!ok) return;
                }
                _vm.DiasEntrenamientoLista.Remove(dia);
                _vm.DiasEntrenamiento = _vm.DiasEntrenamientoLista.Count;
            }
        }

        // ——————————————————————————————————————————————
        // Botón “Cambiar nombre” de cada día
        private async void OnCambiarNombreEntreno_clicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is Entrenamiento dia)
            {
                string nuevo = await DisplayPromptAsync(
                    "Cambiar nombre",
                    "Introduce un nuevo nombre para el día:",
                    initialValue: dia.Nombre);
                if (!string.IsNullOrWhiteSpace(nuevo))
                    dia.Nombre = nuevo;
            }
        }

        // ——————————————————————————————————————————————
        // Botón “Añadir ejercicio” (antes “Editar”) de cada día
        private async void EditarDia_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is Entrenamiento dia)
            {
                // 1) Señalamos el día sobre el que trabajamos
                _vm.EntrenamientoActual = dia;

                // 2) Reseteamos filtros del popup
                GrupoMuscularPicker.ItemsSource = _vm.GruposMusculares;
                GrupoMuscularPicker.SelectedIndex = 0;
                FavoritosSwitch.IsToggled = false;

                // 3) Cargamos ejercicios
                await _vm.CargarEjerciciosAsync(false, "");

                // 4) Mostramos popup
                PopupGrid.IsVisible = true;
            }
        }

        // ——————————————————————————————————————————————
        // Botón “Reset ejercicios” de cada día
        private async void OnResetEjercicios_clicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is Entrenamiento dia)
            {
                if (dia.TrabajoEsperado == null || dia.TrabajoEsperado.Count == 0)
                {
                    await DisplayAlert("Aviso", "No hay ejercicios que resetear.", "OK");
                    return;
                }

                bool ok = await DisplayAlert(
                    "Confirmar reset",
                    $"¿Eliminar todos los ejercicios de {dia.Nombre}?",
                    "Sí", "No");
                if (!ok) return;

                dia.TrabajoEsperado.Clear();
            }
        }

        // ——————————————————————————————————————————————
        // Botón “Borrar” dentro de la lista de ejercicios del popup
        private async void OnBorrarEjercicio_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is TrabajoEsperado te)
            {
                var dia = _vm.EntrenamientoActual;
                if (dia == null) return;

                bool ok = await DisplayAlert(
                    "Confirmar borrado",
                    $"¿Eliminar {te.NombreEjercicio} de {dia.Nombre}?",
                    "Sí", "No");
                if (!ok) return;

                dia.TrabajoEsperado.Remove(te);
            }
        }

        // ——————————————————————————————————————————————
        // Popup: Aceptar / Cancelar
        private async void AceptarPopup_Clicked(object sender, EventArgs e)
        {
            var dia = _vm.EntrenamientoActual;
            if (dia == null)
            {
                await DisplayAlert("Error", "No hay un día seleccionado.", "OK");
                return;
            }

            if (EjercicioPicker.SelectedItem is Ejercicio ejer)
            {
                dia.TrabajoEsperado.Add(new TrabajoEsperado
                {
                    IdEjercicio = ejer.IdEjercicio,
                    NombreEjercicio = ejer.Nombre,
                    Series = (int)SeriesStepper.Value,
                    Repeticiones = (int)RepeticionesStepper.Value
                });
            }
            else
            {
                await DisplayAlert("Error", "Selecciona un ejercicio.", "OK");
            }

            PopupGrid.IsVisible = false;
        }

        private void CancelarPopup_Clicked(object sender, EventArgs e)
            => PopupGrid.IsVisible = false;

        // Filtros del popup
        private async void GrupoMuscularPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var grupo = (string)GrupoMuscularPicker.SelectedItem;
            var filtro = grupo == "Todos" ? "" : grupo;
            await _vm.CargarEjerciciosAsync(FavoritosSwitch.IsToggled, filtro);
        }

        private async void FavoritosSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            var grupo = (string)GrupoMuscularPicker.SelectedItem ?? "Todos";
            var filtro = grupo == "Todos" ? "" : grupo;
            await _vm.CargarEjerciciosAsync(FavoritosSwitch.IsToggled, filtro);
        }
    }
}
