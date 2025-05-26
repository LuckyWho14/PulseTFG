using PulseTFG.Models;
using System.Collections.ObjectModel;

namespace PulseTFG.Pages;

public partial class InicioPage : ContentPage
{
    public ObservableCollection<Trabajo> ListaTrabajo { get; set; }

    public InicioPage()
	{
		InitializeComponent();

        ListaTrabajo = new ObservableCollection<Trabajo>
        {
            new Trabajo { Nombre = "Sentadillas", Intensidad = "Alta", Hecho = false, Repeticion = 10, Serie = 3, Peso = 60 },
            new Trabajo { Nombre = "Press Banca", Intensidad = "Media", Hecho = false, Repeticion = 8, Serie = 4, Peso = 40 },
        };

        TrabajoList.ItemsSource = ListaTrabajo;

    }
   
}