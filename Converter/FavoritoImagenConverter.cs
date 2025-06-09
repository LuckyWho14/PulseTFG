using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseTFG.Converter;

public class FavoritoImagenConverter : IValueConverter
{
    // Este convertidor se utiliza para cambiar la imagen de un favorito en función de su estado (favorito o no favorito).
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool esFavorito = (bool)value;
        return esFavorito ? "favorito_on.png" : "favorito_off.png";
    }

    // El método ConvertBack no se utiliza en este caso, ya que no necesitamos convertir de vuelta el valor.
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}