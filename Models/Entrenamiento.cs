using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PulseTFG.Models
{
    public class Entrenamiento : INotifyPropertyChanged
    {
        public Entrenamiento()
        {
            IdEntrenamiento = Guid.NewGuid().ToString();
            TrabajoEsperado = new ObservableCollection<TrabajoEsperado>();
        }

        public string IdEntrenamiento { get; set; }

        private string nombre;
        public string Nombre
        {
            get => nombre;
            set
            {
                if (nombre != value)
                {
                    nombre = value;
                    OnPropertyChanged(nameof(Nombre));
                }
            }
        }

        public DateTime FechaCreacion { get; set; }
        public DateTime Actualizado { get; set; }

        private ObservableCollection<TrabajoEsperado> trabajoEsperado;

        // Colección de trabajos esperados para este entrenamiento
        public ObservableCollection<TrabajoEsperado> TrabajoEsperado
        {
            get => trabajoEsperado;
            set
            {
                if (trabajoEsperado != value)
                {
                    trabajoEsperado = value;
                    OnPropertyChanged(nameof(TrabajoEsperado));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Método para notificar cambios en las propiedades
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


}
