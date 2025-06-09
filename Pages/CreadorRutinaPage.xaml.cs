using PulseTFG.ViewModel;

namespace PulseTFG.Pages;

public partial class CreadorRutinaPage : ContentPage
{
    RoutineCreatorViewModel _vm;
    public CreadorRutinaPage()
	{
		InitializeComponent();

        _vm = new RoutineCreatorViewModel();
        BindingContext = _vm;

        // Arranca la inicializaci�n sin bloquear el constructor
        _ = _vm.InitializeAsync();
    }

}