using PulseTFG.ViewModel;
namespace PulseTFG.Elementos;

public partial class CreadorRutina : ContentView
{
	public CreadorRutina()
	{
		InitializeComponent();

        BindingContext = new RoutineCreatorViewModel();
    }
}