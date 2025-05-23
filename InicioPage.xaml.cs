using PulseTFG;
using System.Collections.ObjectModel;

namespace PulseTFG;

public partial class InicioPage : ContentPage
{
    public ObservableCollection<Ejercicio> ListaEjercicios { get; set; }
    public InicioPage()
	{
		InitializeComponent();

        ListaEjercicios = new ObservableCollection<Ejercicio>
        {
            new Ejercicio { Nombre = "Sentadillas", Intensidad = "Alta", Hecho = false, Repeticiones = 10, Series = 3, Kg = 60 },
            new Ejercicio { Nombre = "Press Banca", Intensidad = "Media", Hecho = false, Repeticiones = 8, Series = 4, Kg = 40 },
        };

        EjerciciosList.ItemsSource = ListaEjercicios;

    }
   
}