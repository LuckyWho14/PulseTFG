using PulseTFG.Models;
using PulseTFG.ViewModel;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PulseTFG.FirebaseService;
using System.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace PulseTFG.Pages;


public partial class CrearRutinaCrearEntrenoPage : ContentPage
{
    private RoutineCreatorViewModel _vm;


    public CrearRutinaCrearEntrenoPage()
    {
        InitializeComponent();
        _vm = AppState.RoutineCreatorVM;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_vm.EntrenamientoActual == null)
        {
            await DisplayAlert("Error", "Entrenamiento no definido.", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }

        await _vm.InitializeAsync();
    }


    public Entrenamiento Entrenamiento
    {
        get => _vm.EntrenamientoActual;
        set => _vm.EntrenamientoActual = value;
    }

    private async void Volver_Clicked(object sender, EventArgs e)
    {
        // Guardar el entrenamiento en la lista original si existe
        var entrenamiento = _vm.EntrenamientoActual;
        var lista = _vm.DiasEntrenamientoLista;

        var existente = lista.FirstOrDefault(e => e.IdEntrenamiento == entrenamiento.IdEntrenamiento);
        if (existente != null)
        {
            int index = lista.IndexOf(existente);
            lista[index] = entrenamiento;
        }

        // Opcional: refrescar UI
        _vm.OnPropertyChanged(nameof(_vm.DiasEntrenamientoLista));

        // Volver a la página de selección
        await Shell.Current.GoToAsync("//CrearRutinaPersPage");
    }


    private async void CambiarNombre_Clicked(object sender, EventArgs e)
    {
        string nuevoNombre = await DisplayPromptAsync("Editar nombre", "Introduce el nuevo nombre:", initialValue: Entrenamiento.Nombre);
        if (!string.IsNullOrWhiteSpace(nuevoNombre))
            Entrenamiento.Nombre = nuevoNombre;
    }

    private List<Ejercicio> todosLosEjercicios = new();
    readonly FirebaseFirestoreService _firestore = new();

    private async void MostrarPopup_Clicked(object sender, EventArgs e)
    {
        todosLosEjercicios = await _firestore.ObtenerEjerciciosFiltradosAsync(false, "");

        GrupoMuscularPicker.ItemsSource = _vm.GruposMusculares;
        GrupoMuscularPicker.SelectedIndex = 0;

        // ⚠️ Reiniciamos el ItemsSource y luego lo asignamos para evitar que esté vacío
        EjercicioPicker.ItemsSource = null;
        await Task.Delay(100); // Pequeña pausa para forzar el refresco visual
        EjercicioPicker.ItemsSource = todosLosEjercicios;
        EjercicioPicker.SelectedItem = todosLosEjercicios.FirstOrDefault();

        FavoritosSwitch.IsToggled = false;
        PopupGrid.BindingContext = _vm;
        PopupGrid.IsVisible = true;
    }



    private async void FavoritosSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        await AplicarFiltrosPopup();

    }

    private async void GrupoMuscularPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        await AplicarFiltrosPopup();
    }

    private async Task AplicarFiltrosPopup()
    {
        string grupo = GrupoMuscularPicker.SelectedItem?.ToString();
        bool favoritos = FavoritosSwitch.IsToggled;

        todosLosEjercicios = await _firestore.ObtenerEjerciciosFiltradosAsync(favoritos, grupo == "Todos" ? "" : grupo);
        EjercicioPicker.ItemsSource = todosLosEjercicios;

    }

    private async void AceptarPopup_Clicked(object sender, EventArgs e)
    {
        if (EjercicioPicker.SelectedItem is Ejercicio ejercicio)
        {
            var trabajo = new TrabajoEsperado
            {
                IdEjercicio = ejercicio.IdEjercicio,
                NombreEjercicio = ejercicio.Nombre,
                Series = (int)SeriesStepper.Value,
                Repeticiones = (int)RepeticionesStepper.Value
            };

            if (_vm.EntrenamientoActual != null)
            {
                // ✅ COMPRUEBA que la lista no sea null
                if (_vm.EntrenamientoActual.TrabajoEsperado == null)
                    _vm.EntrenamientoActual.TrabajoEsperado = new ObservableCollection<TrabajoEsperado>();

                _vm.EntrenamientoActual.TrabajoEsperado.Add(trabajo);
            }
            else
            {
                await DisplayAlert("Error", "No se ha seleccionado un entrenamiento válido.", "OK");
            }
        }

        PopupGrid.IsVisible = false;
    }




    private void CancelarPopup_Clicked(object sender, EventArgs e)
    {
        // Simplemente ocultamos el popup
        PopupGrid.IsVisible = false;

        // También puedes resetear valores si quieres:
        GrupoMuscularPicker.SelectedItem = null;
        EjercicioPicker.ItemsSource = null;
        FavoritosSwitch.IsToggled = false;
    }

    private async void BorrarTrabajoEsperado_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is TrabajoEsperado trabajo)
        {
            bool confirmar = await DisplayAlert("Confirmar", $"¿Eliminar {trabajo.NombreEjercicio}?", "Sí", "No");
            if (!confirmar) return;

            _vm.EntrenamientoActual.TrabajoEsperado.Remove(trabajo);
        }
    }

    private async void ResetearLista_Clicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Resetear lista",
            "¿Estás seguro de que quieres eliminar todos los ejercicios de este entrenamiento?",
            "Sí", "No");

        if (!confirmar) return;

        if (BindingContext is RoutineCreatorViewModel vm && vm.EntrenamientoActual != null)
        {
            vm.EntrenamientoActual.TrabajoEsperado.Clear();
        }
    }


}
