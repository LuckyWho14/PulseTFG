using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PulseTFG.Models
{
    public class Ejercicio : INotifyPropertyChanged
    {
        public string IdEjercicio { get; set; }

        private string nombre;
        public string Nombre
        {
            get => nombre;
            set
            {
                nombre = value;
                OnPropertyChanged(nameof(Nombre));
            }
        }

        private string descripcion;
        public string Descripcion
        {
            get => descripcion;
            set
            {
                descripcion = value;
                OnPropertyChanged(nameof(Descripcion));
            }
        }

        private string videoId;
        public string VideoId
        {
            get => videoId;
            set
            {
                videoId = value;
                OnPropertyChanged(nameof(VideoId));
                OnPropertyChanged(nameof(ThumbnailUrl));
            }
        }

        private bool esFavorito;
        public bool EsFavorito
        {
            get => esFavorito;
            set
            {
                esFavorito = value;
                OnPropertyChanged(nameof(EsFavorito));
            }
        }

        public ICommand ToggleFavoritoCommand => new Command(() =>
        {
            EsFavorito = !EsFavorito;
        });

        public string ThumbnailUrl => $"https://img.youtube.com/vi/{VideoId}/0.jpg";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
