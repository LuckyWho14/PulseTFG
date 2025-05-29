using PulseTFG.ViewModel;
using PulseTFG.Models;

namespace PulseTFG.Pages;


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
        await _vm.InitializeAsync(); // Aquí sí esperas el resultado
    }
    private async void CambiarNombre_Clicked(object sender, EventArgs e)
    {
        string nuevoNombre = await DisplayPromptAsync("Nombre de rutina", "Introduce un nuevo nombre:", initialValue: _vm.NombreRutina);
        if (!string.IsNullOrWhiteSpace(nuevoNombre))
            _vm.NombreRutina = nuevoNombre;
    }

    private async void CambiarDescripcion_Clicked(object sender, EventArgs e)
    {
        string nuevaDescripcion = await DisplayPromptAsync("Descripción", "Introduce una nueva descripción:", initialValue: _vm.DescripcionRutina);
        if (!string.IsNullOrWhiteSpace(nuevaDescripcion))
            _vm.DescripcionRutina = nuevaDescripcion;
    }

    // Boton para volver a la pagina anterior
    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//CrearRutinaSelecTipoPage");
    }

    private void Stepper_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (BindingContext is RoutineCreatorViewModel vm)
        {
            vm.CambiarDiasCommand.Execute((int)e.NewValue);
        }
    }
    private async void BorrarDia_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is Entrenamiento entrenamiento)
        {
            var vm = BindingContext as RoutineCreatorViewModel;
            if (vm == null) return;

            int index = vm.DiasEntrenamientoLista.IndexOf(entrenamiento);
            if (index == 0)
            {
                await DisplayAlert("Aviso", "No puedes eliminar el Día 1.", "OK");
                return;
            }

            if (vm.EntrenamientoTieneContenido(entrenamiento))
            {
                bool confirmar = await DisplayAlert("Confirmar borrado",
                    $"¿Seguro que quieres borrar {entrenamiento.Nombre} con contenido?",
                    "Sí", "No");
                if (!confirmar) return;
            }

            vm.DiasEntrenamientoLista.Remove(entrenamiento);
            vm.DiasEntrenamiento = vm.DiasEntrenamientoLista.Count;
        }
    }

    private async void EditarDia_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is Entrenamiento entrenamiento)
        {
            var parametros = new Dictionary<string, object>
        {
            { "Entrenamiento", entrenamiento }
        };

            await Shell.Current.GoToAsync(nameof(CrearRutinaCrearEntrenoPage), parametros);

        }
    }

}