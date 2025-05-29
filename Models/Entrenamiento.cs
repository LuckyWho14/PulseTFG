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

        public ObservableCollection<TrabajoEsperado> TrabajoEsperado { get; set; } = new();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
