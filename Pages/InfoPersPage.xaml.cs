
using System.Globalization;

namespace PulseTFG.Pages;

public partial class InfoPersPage : ContentPage
{
	public InfoPersPage()
	{
		InitializeComponent();
        BindingContext = this;
        Peso.Text = "80"; // Valor inicial

    }
    private string _pesoTexto;
    public string PesoTexto
    {
        get => _pesoTexto;
        set
        {
            if (_pesoTexto != value)
            {
                _pesoTexto = value;
                OnPropertyChanged(nameof(PesoTexto));
            }
        }
    }

    // Cuando aparece la pantalla
    protected override void OnAppearing()
    {
        base.OnAppearing();
        CalcularIMC();
    }

    // Cuando cambia el texto del campo de peso
    private void OnPesoChanged(object sender, TextChangedEventArgs e)
    {
        CalcularIMC();
    }

    private void DisminuirPeso(object sender, EventArgs e)
    {
        var culture = CultureInfo.InvariantCulture;
        if (double.TryParse(PesoTexto, NumberStyles.Any, culture, out double peso))
        {
            peso = Math.Max(peso - 0.05, 0);
            PesoTexto = peso.ToString("F2", culture);
        }
    }

    private void AumentarPeso(object sender, EventArgs e)
    {
        var culture = CultureInfo.InvariantCulture;
        if (double.TryParse(PesoTexto, NumberStyles.Any, culture, out double peso))
        {
            peso = Math.Min(peso + 0.05, 200);
            PesoTexto = peso.ToString("F2", culture);
        }
    }



    private void CalcularIMC()
    {
        var culture = CultureInfo.InvariantCulture;

        string alturaTexto = Altura.Text?.Trim();
        string pesoTexto = PesoTexto?.Trim();


        if (string.IsNullOrWhiteSpace(alturaTexto) || string.IsNullOrWhiteSpace(pesoTexto))
        {
            IMC.Text = "Faltan datos";
            return;
        }

        if (double.TryParse(pesoTexto, NumberStyles.Any, culture, out double peso) &&
            double.TryParse(alturaTexto, NumberStyles.Any, culture, out double altura))
        {
            if (peso > 0 && altura > 0)
            {
                double imc = peso / (altura * altura);
                IMC.Text = $"{imc:F2}";
            }
            else
            {
                IMC.Text = "Peso o altura no válidos";
            }
        }
        else
        {
            IMC.Text = "Error en entrada";
        }
    }

}