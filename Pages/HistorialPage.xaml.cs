<<<<<<< HEAD:HistorialPage.xaml.cs
using System.Collections.ObjectModel;

namespace PulseTFG;
=======
namespace PulseTFG.Pages;
>>>>>>> origin/ClasesBBDD:Pages/HistorialPage.xaml.cs

public partial class HistorialPage : ContentPage
{
    public HistorialPage()
	{
		InitializeComponent();

        BindingContext = this;
        EstaActivo = false;

        ListaEjercicios = new ObservableCollection<Ejercicio>
        {
            new Ejercicio { Nombre = "Sentadillas", Intensidad = 5, Hecho = true, Repeticiones = 10, Series = 3, Kg = 60 },
            new Ejercicio { Nombre = "Press Banca", Intensidad = 10, Hecho = false, Repeticiones = 8, Series = 4, Kg = 40 },
            new Ejercicio { Nombre = "Press Banca", Intensidad = 10, Hecho = false, Repeticiones = 8, Series = 4, Kg = 40 },
            new Ejercicio { Nombre = "Press Banca", Intensidad = 10, Hecho = false, Repeticiones = 8, Series = 4, Kg = 40 },
            new Ejercicio { Nombre = "Press Banca", Intensidad = 10, Hecho = false, Repeticiones = 8, Series = 4, Kg = 40 },
            new Ejercicio { Nombre = "Press Banca", Intensidad = 10, Hecho = false, Repeticiones = 8, Series = 4, Kg = 40 },
            new Ejercicio { Nombre = "Press Banca", Intensidad = 10, Hecho = false, Repeticiones = 8, Series = 4, Kg = 40 },
            new Ejercicio { Nombre = "Press Banca", Intensidad = 10, Hecho = false, Repeticiones = 8, Series = 4, Kg = 40 },
            new Ejercicio { Nombre = "Press Banca", Intensidad = 10, Hecho = false, Repeticiones = 8, Series = 4, Kg = 40 },
            new Ejercicio { Nombre = "Press Banca", Intensidad = 10, Hecho = false, Repeticiones = 8, Series = 4, Kg = 40 },
            new Ejercicio { Nombre = "Press Banca", Intensidad = 10, Hecho = false, Repeticiones = 8, Series = 4, Kg = 40 },
        };

        EjerciciosList.ItemsSource = ListaEjercicios;

    }

    public ObservableCollection<Ejercicio> ListaEjercicios { get; set; }

    private bool _estaActivo;
    public bool EstaActivo
    {
        get => _estaActivo;
        set
        {
            if (_estaActivo != value)
            {
                _estaActivo = value;
                OnPropertyChanged(nameof(EstaActivo));
            }
        }
    }
    


}