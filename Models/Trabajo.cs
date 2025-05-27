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
        [Key]
        public int IdTrabajo { get; set; }

        [Key]
        public string Nombre { get; set; }

        [Required]
        public int Peso { get; set; }

        [Required]
        public int Repeticion { get; set; }

        [Required]
        public int Serie { get; set; }

        [Required]
        public string Intensidad { get; set; }

        [Required]
        public bool Hecho { get; set; }

        // Claves foráneas
        public int? IdEntrenamiento { get; set; }

        public int? IdEjercicio { get; set; }
    }
}
