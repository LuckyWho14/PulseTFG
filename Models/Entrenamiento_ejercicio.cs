using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseTFG.Api.Models
{
    public class Entrenamiento_ejercicio
    {
        [Key]
        public int IdEntrenamiento_ejercicio { get; set; }

        // Claves foráneas
        public int? IdEntrenamiento { get; set; }
        public int? IdEjercicio { get; set; }
    }
}
