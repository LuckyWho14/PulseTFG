namespace PulseTFG;

public partial class MenuBarra : ContentView
{
    
    public MenuBarra()
	{
		InitializeComponent();
	}
    private async void OnIcon1Tapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///InicioPage");
    }

    private async void OnIcon2Tapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///HistorialPage");
    }

    private async void OnIcon3Tapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///RelojPage");
    }
    private async void OnIcon4Tapped(object sender, EventArgs e)
    {
        if (Shell.Current is AppShell shell)
        {
            shell.FlyoutIsPresented = !shell.FlyoutIsPresented;
        }
    }
}