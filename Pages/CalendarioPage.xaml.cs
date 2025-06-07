using Syncfusion.Maui.Calendar;

namespace PulseTFG.Pages;

public partial class CalendarioPage : ContentPage
{
    private readonly List<DateTime> fechasEntrenadas = new();

    public CalendarioPage()
    {
        InitializeComponent();

        // 🚫 Comentado porque MonthCellLoaded no existe aún en MAUI (salvo versiones futuras)
        // CalendarioEntrenamientos.MonthCellLoaded += OnMonthCellLoaded;

        CalendarioEntrenamientos.SelectionChanged += OnSelectionChanged;
    }

    // Solo guarda la fecha (en memoria por ahora)
    private void OnSelectionChanged(object sender, CalendarSelectionChangedEventArgs e)
    {
        // e.NewValue es un solo DateTime, no una lista, así que no uses foreach
        if (e.NewValue is DateTime fecha)
        {
            if (!fechasEntrenadas.Contains(fecha.Date))
            {
                fechasEntrenadas.Add(fecha.Date);

                // Aquí guardarías en Firebase
                Console.WriteLine($"Día entrenado guardado: {fecha:yyyy-MM-dd}");
            }
        }
    }
}
