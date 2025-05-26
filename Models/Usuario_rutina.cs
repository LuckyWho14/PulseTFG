using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseTFG.Api.Models
{
    public class Usuario_rutina
    {
        [Key]
        public int IdUsuario_rutina { get; set; }
        
        // Claves foráneas
        public int? IdUsuario { get; set; }
        public int? IdRutina { get; set; }
    }
}
