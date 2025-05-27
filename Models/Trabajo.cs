using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseTFG.Models
{
    public class Trabajo
    {
        public int IdTrabajo { get; set; }
        public int IdEjercicio { get; set; }
        public string NombreEjercicio { get; set; }

        public int Peso { get; set; }
        public int Repeticion { get; set; }
        public int Serie { get; set; }
        public int Intensidad { get; set; }
        public bool Hecho { get; set; }
        public string Notas { get; set; }

        public DateTime Fecha { get; set; }
    }
}
