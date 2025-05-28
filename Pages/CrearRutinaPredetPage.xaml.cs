using PulseTFG.ViewModel;
namespace PulseTFG.Pages;

public partial class CrearRutinaPredetPage : ContentPage
{
    
    RoutineCreatorViewModel _vm;

    public CrearRutinaPredetPage()
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