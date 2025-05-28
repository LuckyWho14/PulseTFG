using PulseTFG.ViewModel;
namespace PulseTFG.Pages;

public partial class CrearRutinaCrearEntrenoPage : ContentPage
{
    RoutineCreatorViewModel _vm;

    public CrearRutinaCrearEntrenoPage()
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
}