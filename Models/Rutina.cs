using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulseTFG.Models
{
    public class Rutina
    {
        [Key]
        public int IdRutina { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Descripcion { get; set; }

        // Claves foráneas
        public int IdRutina_entrenamiento { get; set; }
    }
}
