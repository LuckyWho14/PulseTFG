// Origen: PulseTFG/ViewModels/HistorialViewModel.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;    // Para la clase Command
using Microsoft.Maui.Storage;     // Para Preferences
using PulseTFG.FirebaseService;   // Tu servicio de Firestore REST
using PulseTFG.Models;            // Tu modelo Registro

namespace PulseTFG.ViewModel
{
    public class HistorialViewModel : INotifyPropertyChanged
    {
        // 1. Servicio REST de Firestore
        private readonly FirebaseFirestoreService _firestoreService = new FirebaseFirestoreService();

        // 2. Lista en memoria con TODOS los registros descargados
        private List<Registro> _allRegistros = new List<Registro>();

        // 3. Backing fields
        private bool _isBusy;
        private bool _mostrarTodosDias = true;
        private DateTime _fechaSeleccionada = DateTime.Today;
        private string _ejercicioSeleccionado;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string nombrePropiedad) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombrePropiedad));

        public ObservableCollection<Registro> ListaRegistros { get; }
        public ObservableCollection<string> ListaEjerciciosDisponibles { get; }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
                (CargarRegistrosCommand as Command)?.ChangeCanExecute();
                (RefrescarCommand as Command)?.ChangeCanExecute();
            }
        }

        public bool MostrarTodosDias
        {
            get => _mostrarTodosDias;
            set
            {
                if (_mostrarTodosDias == value) return;
                _mostrarTodosDias = value;
                OnPropertyChanged(nameof(MostrarTodosDias));
                AplicarFiltro();
            }
        }

        public DateTime FechaSeleccionada
        {
            get => _fechaSeleccionada;
            set
            {
                if (value > DateTime.Today) value = DateTime.Today;
                if (_fechaSeleccionada == value) return;
                _fechaSeleccionada = value;
                OnPropertyChanged(nameof(FechaSeleccionada));
                if (!MostrarTodosDias)
                    AplicarFiltro();
            }
        }

        public string EjercicioSeleccionado
        {
            get => _ejercicioSeleccionado;
            set
            {
                if (_ejercicioSeleccionado == value) return;
                _ejercicioSeleccionado = value;
                OnPropertyChanged(nameof(EjercicioSeleccionado));
                AplicarFiltro();
            }
        }

        public ICommand CargarRegistrosCommand { get; }
        public ICommand RefrescarCommand { get; }

        public HistorialViewModel()
        {
            ListaRegistros = new ObservableCollection<Registro>();
            ListaEjerciciosDisponibles = new ObservableCollection<string>();
            CargarRegistrosCommand = new Command(async () => await CargarRegistrosAsync(), () => !IsBusy);
            RefrescarCommand = new Command(async () => await CargarRegistrosAsync(), () => !IsBusy);
            EjercicioSeleccionado = "Todos";
        }

        private async Task CargarRegistrosAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                _allRegistros.Clear();
                ListaRegistros.Clear();
                ListaEjerciciosDisponibles.Clear();

                var uid = Preferences.Get("firebase_user_uid", null);
                if (string.IsNullOrEmpty(uid))
                    return;

                var registros = await _firestoreService.ObtenerRegistrosUsuarioAsync(uid);
                _allRegistros = registros.OrderByDescending(r => r.Fecha).ToList();

                ListaEjerciciosDisponibles.Add("Todos");
                var distinctEjercicios = _allRegistros
                                         .Select(r => r.NombreEjercicio)
                                         .Distinct()
                                         .OrderBy(n => n);
                foreach (var nombre in distinctEjercicios)
                    ListaEjerciciosDisponibles.Add(nombre);

                if (!ListaEjerciciosDisponibles.Contains(EjercicioSeleccionado))
                    EjercicioSeleccionado = "Todos";

                AplicarFiltro();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[HistorialVM] Error en CargarRegistrosAsync: {ex}");
            }
            finally
            {
                IsBusy = false;
                (RefrescarCommand as Command)?.ChangeCanExecute();
                (CargarRegistrosCommand as Command)?.ChangeCanExecute();
            }
        }

        private void AplicarFiltro()
        {
            if (_allRegistros == null) return;

            // 1. Partimos de todos los registros
            IEnumerable<Registro> filtrados = _allRegistros;

            // 2. Filtrar sólo los que están marcados como "Hecho == true"
            filtrados = filtrados.Where(r => r.Hecho);

            // 3. Filtrar por día si MostrarTodosDias == false
            if (!MostrarTodosDias)
            {
                filtrados = filtrados.Where(r => r.Fecha.Date == FechaSeleccionada.Date);
            }

            // 4. Filtrar por ejercicio si EjercicioSeleccionado != "Todos"
            if (!string.IsNullOrEmpty(EjercicioSeleccionado) && EjercicioSeleccionado != "Todos")
            {
                filtrados = filtrados.Where(r => r.NombreEjercicio == EjercicioSeleccionado);
            }

            // 5. Orden opcional: por fecha descendente
            filtrados = filtrados.OrderByDescending(r => r.Fecha);

            // 6. Actualizar la ObservableCollection (ListaRegistros)
            ListaRegistros.Clear();
            foreach (var reg in filtrados)
            {
                ListaRegistros.Add(reg);
            }
        }
    }
}
