
using System.Globalization;
using PulseTFG.ViewModel;

namespace PulseTFG.Pages;

public partial class InfoPersPage : ContentPage
{
    
	public InfoPersPage()
	{
		InitializeComponent();

        BindingContext = new ProfileViewModel();
    }
    
}