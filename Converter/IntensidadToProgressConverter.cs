// Origen: PulseTFG/Converters/IntensidadToProgressConverter.cs
using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace PulseTFG.Converter
{
    public class IntensidadToProgressConverter : IValueConverter
    {
        // Convierte un int (1–10) a double (0.0–1.0)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intensidad)
            {
                // Nos aseguramos de que intensidad esté en [1,10]
                intensidad = Math.Clamp(intensidad, 1, 10);
                return (intensidad - 1) / 9.0;
            }
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // No necesitamos conversión inversa para el ProgressBar
            throw new NotImplementedException();
        }
    }
}
