using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using PulseTFG.FirebaseService;
using PulseTFG.Models;

namespace PulseTFG.ViewModel
{
    public class InicioViewModel
    {
        readonly FirebaseFirestoreService _firestore;
        readonly string _uid, _rutinaId;

        // Lista de IDs de días (entrenamientos) de la rutina activa
        List<string> _listaDias = new();

        // Para cada día, la lista de ejercicios preparada
        List<List<EjercicioRutinaViewModel>> _masterDiasEjercicios = new();

        // Observable sólo con los ejercicios del día actual
        public ObservableCollection<EjercicioRutinaViewModel> ListaEjercicios { get; }
            = new ObservableCollection<EjercicioRutinaViewModel>();

        public int DiaActualIndex { get; private set; }

        public InicioViewModel(string uid, string rutinaId)
        {
            _uid = uid;
            _rutinaId = rutinaId;
            _firestore = new FirebaseFirestoreService();
        }

        /// <summary>
        /// Carga todos los días (entrenamientos) y para cada uno, su lista de ejercicios.
        /// Luego muestra el primer día.
        /// </summary>
        public async Task InicializarAsync(string primerDiaId)
        {
            DiaActualIndex = 0;
            _listaDias.Clear();
            _masterDiasEjercicios.Clear();
            ListaEjercicios.Clear();

            // 1) Obtén todos los entrenamientos (IDs y orden por fecha o nombre)
            var entrenamientos = await _firestore.ObtenerEntrenamientosAsync(_uid, _rutinaId);
            // Asegura orden deseado
            entrenamientos = entrenamientos.OrderBy(e => e.FechaCreacion).ToList();

            // 2) Para cada día, carga los ejercicios
            foreach (var ent in entrenamientos)
            {
                _listaDias.Add(ent.IdEntrenamiento);

                var trabajo = await _firestore.ObtenerTrabajoEsperadoAsync(
                    _uid, _rutinaId, ent.IdEntrenamiento);

                var listaVM = new List<EjercicioRutinaViewModel>();
                foreach (var t in trabajo)
                {
                    var vm = new EjercicioRutinaViewModel(t.NombreEjercicio)
                    {
                        IdTrabajoEsperado = t.IdTrabajoEsperado,
                        IdEjercicio = t.IdEjercicio,
                        SeriesActual = t.Series,
                        RepsActual = t.Repeticiones,
                        KgActual = 0,
                        IntensidadActual = 1,
                        HechoActual = false
                    };
                    var ultimo = await _firestore.ObtenerUltimoRegistroAsync(
                        _uid, t.IdTrabajoEsperado);
                    if (ultimo != null)
                    {
                        vm.SeriesAnterior = ultimo.Serie;
                        vm.RepsAnterior = ultimo.Repeticion;
                        vm.KgAnterior = ultimo.Peso;
                        vm.IntensidadAnterior = ultimo.Intensidad;
                    }
                    listaVM.Add(vm);
                }
                _masterDiasEjercicios.Add(listaVM);
            }

            // 3) Mostrar el día inicial (por índice 0)
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
        }

        /// <summary>
        /// Registra todos los ejercicios del día actual y avanza al siguiente día,
        /// ciclando al primero cuando se pasa del último.
        /// </summary>
        public async Task GuardarYAvanzarDiaAsync()
        {
            var ejercicios = _masterDiasEjercicios[DiaActualIndex];
            foreach (var actual in ejercicios)
            {
                var registro = new Registro
                {
                    IdTrabajo = actual.IdTrabajoEsperado,
                    IdEjercicio = actual.IdEjercicio,
                    NombreEjercicio = actual.Nombre,
                    Peso = actual.KgActual,
                    Repeticion = actual.RepsActual,
                    Serie = actual.SeriesActual,
                    Intensidad = actual.IntensidadActual,
                    Hecho = actual.HechoActual,
                    Notas = "",
                    Fecha = DateTime.Now
                };
                await _firestore.CrearRegistroAsync(_uid, registro);
            }

            // Avanza al siguiente día (cíclico)
            DiaActualIndex = (DiaActualIndex + 1) % _masterDiasEjercicios.Count;
            RefrescarDia();
        }
        public async Task CargarEjerciciosDelDiaAsync(string diaId)
        {
            ListaEjercicios.Clear();

            // traer trabajo esperado
            var trabajo = await _firestore.ObtenerTrabajoEsperadoAsync(_uid, _rutinaId, diaId);

            foreach (var t in trabajo)
            {
                var vm = new EjercicioRutinaViewModel(t.NombreEjercicio)
                {
                    IdTrabajoEsperado = t.IdTrabajoEsperado,
                    IdEjercicio = t.IdEjercicio,
                    SeriesActual = t.Series,
                    RepsActual = t.Repeticiones,
                    KgActual = 0,
                    IntensidadActual = 1,
                    HechoActual = false
                };

                // ← aquí el cambio: filtramos sólo registros de días anteriores
                var ultimo = await _firestore.ObtenerUltimoRegistroAnteriorAsync(_uid, t.IdEjercicio);
                if (ultimo != null)
                {
                    vm.SeriesAnterior = ultimo.Serie;
                    vm.RepsAnterior = ultimo.Repeticion;
                    vm.KgAnterior = ultimo.Peso;
                    vm.IntensidadAnterior = ultimo.Intensidad;
                }

                ListaEjercicios.Add(vm);
            }
        }

    }
}
