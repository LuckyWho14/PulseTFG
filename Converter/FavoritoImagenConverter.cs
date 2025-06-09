using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseTFG.Converter;

public class FavoritoImagenConverter : IValueConverter
{

    // Convierte un booleano a una imagen de favorito
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool esFavorito = (bool)value;
        return esFavorito ? "favorito_on.png" : "favorito_off.png";
    }

    // Convierte de vuelta, no implementado ya que no se usa
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}