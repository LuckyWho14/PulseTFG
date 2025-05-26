using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulseTFG.Api.Models
{
    public class Entrenamiento
    {
        [Key]
        public int IdEntrenamiento { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public string Notas { get; set; }

        [Required]
        public int Intensidad_global { get; set; }

        // Claves foráneas
        public int? IdRutina_entrenamiento { get; set; }
    }
}
