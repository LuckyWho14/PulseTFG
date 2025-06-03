using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PulseTFG.ViewModel
{
    public class EjercicioRutinaViewModel : INotifyPropertyChanged
    {
        public EjercicioRutinaViewModel(string nombre)
        {
            Nombre = nombre;
            Series = Repeticiones = Intensidad = 0;
            Kg = 0;
            Hecho = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public string IdTrabajoEsperado { get; set; }
        public string IdEjercicio { get; set; }
        public string Nombre { get; }

        private int _series;
        public int Series
        {
            get => _series;
            set { _series = value; OnPropertyChanged(); }
        }

        private int _repeticiones;
        public int Repeticiones
        {
            get => _repeticiones;
            set { _repeticiones = value; OnPropertyChanged(); }
        }

        private double _kg;
        public double Kg
        {
            get => _kg;
            set { _kg = value; OnPropertyChanged(); }
        }

        private int _intensidad;
        public int Intensidad
        {
            get => _intensidad;
            set { _intensidad = value; OnPropertyChanged(); }
        }

        private bool _hecho;
        public bool Hecho
        {
            get => _hecho;
            set { _hecho = value; OnPropertyChanged(); }
        }
    }
}
