using PulseTFG.ViewModel;

namespace PulseTFG.Pages;

public partial class CrearRutinaSelecTipoPage : ContentPage
{
    RoutineCreatorViewModel _vm;
    public CrearRutinaSelecTipoPage()
	{
		InitializeComponent();

        _vm = new RoutineCreatorViewModel();
        BindingContext = _vm;

        // Arranca la inicializaci�n sin bloquear el constructor
        _ = _vm.InitializeAsync();
    }

}