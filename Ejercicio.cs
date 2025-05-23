using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseTFG
{
    public class Ejercicio : INotifyPropertyChanged
    {

        public string VideoId { get; set; }
        public string Descripcion { get; set; }
        public bool EsFavorito { get; set; }

        public string ThumbnailUrl
        {
            get
            {
                return $"https://img.youtube.com/vi/{VideoId}/0.jpg";
            }
        }


        public string Nombre { get; set; }

        private string intensidad;
        public string Intensidad
        {
            get => intensidad;
            set
            {
                intensidad = value;
                OnPropertyChanged(nameof(Intensidad));
            }
        }

        private bool hecho;
        public bool Hecho
        {
            get => hecho;
            set
            {
                hecho = value;
                OnPropertyChanged(nameof(Hecho));
            }
        }

        private int repeticiones;
        public int Repeticiones
        {
            get => repeticiones;
            set
            {
                repeticiones = value;
                OnPropertyChanged(nameof(Repeticiones));
            }
        }

        private int series;
        public int Series
        {
            get => series;
            set
            {
                series = value;
                OnPropertyChanged(nameof(Series));
            }
        }

        private int kg;
        public int Kg
        {
            get => kg;
            set
            {
                kg = value;
                OnPropertyChanged(nameof(Kg));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
