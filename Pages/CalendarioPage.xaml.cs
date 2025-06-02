using Syncfusion.Maui.Calendar;

namespace PulseTFG.Pages;

public partial class CalendarioPage : ContentPage
{
	public CalendarioPage()
	{
		InitializeComponent();
	}

    private void OnSelectionChanged(object sender, CalendarSelectionChangedEventArgs e)
    {
        if (e.NewValue is IList<DateTime> fechasSeleccionadas)
        {
            foreach (var fecha in fechasSeleccionadas)
            {
                Console.WriteLine($"📅 Día marcado como entrenado: {fecha:dd/MM/yyyy}");
            }
        }
    }
}