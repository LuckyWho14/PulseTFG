using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using PulseTFG.FirebaseService;
using PulseTFG.Models;

namespace PulseTFG.ViewModel
{
    public class InicioViewModel : INotifyPropertyChanged
    {
        public string NombreRutina { get; set; }
        public string NombreEntrenamientoActual => 
            (_listaEntrenamientos.Count > DiaActualIndex) ? _listaEntrenamientos[DiaActualIndex].Nombre : "";

        private List<Entrenamiento> _listaEntrenamientos = new();

        readonly FirebaseFirestoreService _firestore;
        readonly string _uid, _rutinaId;

        List<string> _listaDias = new();
        List<List<EjercicioRutinaViewModel>> _masterDiasEjercicios = new();

        public ObservableCollection<EjercicioRutinaViewModel> ListaEjercicios { get; }
            = new ObservableCollection<EjercicioRutinaViewModel>();

        public int DiaActualIndex { get; private set; }

        public InicioViewModel(string uid, string rutinaId)
        {
            _uid = uid;
            _rutinaId = rutinaId;
            _firestore = new FirebaseFirestoreService();
        }

        public async Task InicializarAsync(string primerDiaId)
        {
            DiaActualIndex = 0;
            _listaDias.Clear();
            _masterDiasEjercicios.Clear();
            ListaEjercicios.Clear();

            // Obtener entrenamientos
            var entrenamientos = await _firestore.ObtenerEntrenamientosAsync(_uid, _rutinaId);
            _listaEntrenamientos = entrenamientos.OrderBy(e => e.FechaCreacion).ToList();

            foreach (var ent in _listaEntrenamientos)
            {
                _listaDias.Add(ent.IdEntrenamiento);

                var trabajo = await _firestore.ObtenerTrabajoEsperadoAsync(_uid, _rutinaId, ent.IdEntrenamiento);

                var listaVM = new List<EjercicioRutinaViewModel>();
                foreach (var t in trabajo)
                {
                    var vm = new EjercicioRutinaViewModel(t.NombreEjercicio)
                    {
                        IdTrabajoEsperado = t.IdTrabajoEsperado,
                        IdEjercicio = t.IdEjercicio,
                        Series = t.Series,
                        Repeticiones = t.Repeticiones,
                        Kg = 0,
                        Intensidad = 1,
                        Hecho = false
                    };



                    listaVM.Add(vm);
                }

                _masterDiasEjercicios.Add(listaVM);
            }

            if (_masterDiasEjercicios.Count > 0)
            {
                DiaActualIndex = 0;
                RefrescarDia();
            }
        }

        void RefrescarDia()
        {
            ListaEjercicios.Clear();
            foreach (var vm in _masterDiasEjercicios[DiaActualIndex])
                ListaEjercicios.Add(vm);

            OnPropertyChanged(nameof(NombreEntrenamientoActual));
        }

        public async Task GuardarDiaActualAsync()
        {
            var ejercicios = _masterDiasEjercicios[DiaActualIndex];
            foreach (var actual in ejercicios.Where(e => e.Hecho))
            {
                var registro = new Registro
                {
                    IdTrabajo = actual.IdTrabajoEsperado,
                    IdEjercicio = actual.IdEjercicio,
                    NombreEjercicio = actual.Nombre,
                    Repeticion = actual.Repeticiones,
                    Serie = actual.Series,
                    Peso = actual.Kg,
                    Intensidad = actual.Intensidad,
                    Hecho = true,
                    Notas = "",
                    Fecha = DateTime.Now
                };
                await _firestore.CrearRegistroAsync(_uid, registro);
            }
            foreach (var ejercicio in ejercicios)
            {

                ejercicio.Intensidad = 1;
                ejercicio.Hecho = false;
            }

        }

        public async Task CargarEjerciciosDelDiaAsync(string diaId)
        {
            ListaEjercicios.Clear();

            var trabajo = await _firestore.ObtenerTrabajoEsperadoAsync(_uid, _rutinaId, diaId);

            foreach (var t in trabajo)
            {
                var vm = new EjercicioRutinaViewModel(t.NombreEjercicio)
                {
                    IdTrabajoEsperado = t.IdTrabajoEsperado,
                    IdEjercicio = t.IdEjercicio
                };


                ListaEjercicios.Add(vm);
            }
        }

        public void MoverADiaAnterior()
        {
            if (_masterDiasEjercicios.Count == 0) return;

            DiaActualIndex = (DiaActualIndex - 1 + _masterDiasEjercicios.Count) % _masterDiasEjercicios.Count;
            RefrescarDia();
        }

        public void MoverADiaSiguiente()
        {
            if (_masterDiasEjercicios.Count == 0) return;

            DiaActualIndex = (DiaActualIndex + 1) % _masterDiasEjercicios.Count;
            RefrescarDia();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
