using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseTFG.Models
{
    public class TrabajoEsperado
    {
        public string IdTrabajoEsperado { get; set; }
        public string IdEjercicio { get; set; }            
        public string NombreEjercicio { get; set; }        
        public int Series { get; set; }                    
        public int Repeticiones { get; set; }
        public int Orden { get; set; }
        public string FormatoTrabajo => $"{Series} x {Repeticiones}";


    }
}
