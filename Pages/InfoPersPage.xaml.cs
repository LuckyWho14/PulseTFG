
using System.Globalization;

namespace PulseTFG.Pages;

public partial class InfoPersPage : ContentPage
{
	public InfoPersPage()
	{
		InitializeComponent();

        
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        CalcularIMC();
    }

    private void CalcularIMC()
    {
        var culture = CultureInfo.InvariantCulture;

        string alturaTexto = Altura?.Text?.Trim();
        string pesoTexto = Peso?.Text?.Trim();

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