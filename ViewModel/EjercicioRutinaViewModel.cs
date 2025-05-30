// PulseTFG/ViewModel/EjercicioRutinaViewModel.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PulseTFG.ViewModel
{
    public class EjercicioRutinaViewModel : INotifyPropertyChanged
    {
        public EjercicioRutinaViewModel(string nombre)
        {
            Nombre = nombre;
            SeriesActual = RepsActual = KgActual = IntensidadActual = 0;
            HechoActual = false;
            SeriesAnterior = RepsAnterior = KgAnterior = IntensidadAnterior = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // — Identificadores —
        public string IdTrabajoEsperado { get; set; }
        public string IdEjercicio { get; set; }   // <-- string, no int

        // — Datos actuales —
        public string Nombre { get; }

        private int _seriesActual;
        public int SeriesActual
        {
            get => _seriesActual;
            set { _seriesActual = value; OnPropertyChanged(); }
        }

        private int _repsActual;
        public int RepsActual
        {
            get => _repsActual;
            set { _repsActual = value; OnPropertyChanged(); }
        }

        private int _kgActual;
        public int KgActual
        {
            get => _kgActual;
            set { _kgActual = value; OnPropertyChanged(); }
        }

        private int _intensidadActual;
        public int IntensidadActual
        {
            get => _intensidadActual;
            set { _intensidadActual = value; OnPropertyChanged(); }
        }

        private bool _hechoActual;
        public bool HechoActual
        {
            get => _hechoActual;
            set { _hechoActual = value; OnPropertyChanged(); }
        }

        // — Datos anteriores —
        private int _seriesAnterior;
        public int SeriesAnterior
        {
            get => _seriesAnterior;
            set
            {
                if (_seriesAnterior == value) return;
                _seriesAnterior = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TieneRegistroAnterior));
            }
        }

        private int _repsAnterior;
        public int RepsAnterior
        {
            get => _repsAnterior;
            set
            {
                if (_repsAnterior == value) return;
                _repsAnterior = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TieneRegistroAnterior));
            }
        }

        private int _kgAnterior;
        public int KgAnterior
        {
            get => _kgAnterior;
            set
            {
                if (_kgAnterior == value) return;
                _kgAnterior = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TieneRegistroAnterior));
            }
        }

        private int _intensidadAnterior;
        public int IntensidadAnterior
        {
            get => _intensidadAnterior;
            set
            {
                if (_intensidadAnterior == value) return;
                _intensidadAnterior = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IntensidadAnteriorNormalizada));
                OnPropertyChanged(nameof(TieneRegistroAnterior));
            }
        }

        public double IntensidadAnteriorNormalizada
            => (IntensidadAnterior - 1) / 9.0;

        public bool TieneRegistroAnterior
            => SeriesAnterior > 0
            || RepsAnterior > 0
            || KgAnterior > 0
            || IntensidadAnterior > 0;
    }
}
