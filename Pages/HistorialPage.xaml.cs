namespace PulseTFG.Pages;
>>>>>>> modificacion de la estructura de la solucion; Creación de carpetas y corrección de errores:Pages/HistorialPage.xaml.cs

public partial class HistorialPage : ContentPage
{
    public HistorialPage()
	{
		InitializeComponent();

        BindingContext = this;
        EstaActivo = false;

    }

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