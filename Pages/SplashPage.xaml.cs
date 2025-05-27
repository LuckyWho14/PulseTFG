using PulseTFG.FirebaseService;

namespace PulseTFG.Pages;

public partial class SplashPage : ContentPage
{
    readonly FirebaseAuthService _auth = new();
    public SplashPage()
	{
		InitializeComponent();
	}

}