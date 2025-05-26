using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseTFG.Models
{
    public class Rutina_entrenamiento
    {
        [Key]
        public int IdRutina_entrenamiento { get; set; }

        // Claves foráneas
        public int? IdRutina { get; set; }

        public int? IdEntrenamiento { get; set; }

    }
}
