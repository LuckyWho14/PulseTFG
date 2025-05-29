using PulseTFG.Models;
using PulseTFG.ViewModel;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PulseTFG.FirebaseService;
using System.Linq;
using System.Collections.ObjectModel;

namespace PulseTFG.Pages;

[QueryProperty(nameof(Entrenamiento), "Entrenamiento")]
public partial class CrearRutinaCrearEntrenoPage : ContentPage
{
    private RoutineCreatorViewModel _vm;

    public CrearRutinaCrearEntrenoPage()
    {
        InitializeComponent();
        _vm = new RoutineCreatorViewModel();
        BindingContext = _vm;
    }

    public Entrenamiento Entrenamiento
    {
        get => _vm.EntrenamientoActual;
        set => _vm.EntrenamientoActual = value;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.InitializeAsync();
    }

    private async void Volver_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
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
        // Carga desde Firestore
        todosLosEjercicios = await _firestore.ObtenerEjerciciosFiltradosAsync(false, "");


        GrupoMuscularPicker.ItemsSource = _vm.GruposMusculares;
        GrupoMuscularPicker.SelectedIndex = 0;

        EjercicioPicker.ItemsSource = todosLosEjercicios;
        EjercicioPicker.SelectedIndex = 0;

        FavoritosSwitch.IsToggled = false;
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

    private void AceptarPopup_Clicked(object sender, EventArgs e)
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

            _vm.EntrenamientoActual.TrabajoEsperado.Add(trabajo);
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

}
