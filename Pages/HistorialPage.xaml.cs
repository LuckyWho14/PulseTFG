namespace PulseTFG.Pages;

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