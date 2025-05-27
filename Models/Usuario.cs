using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulseTFG.Models
{
    public class Usuario
    {
        [Key]
        public string Uid { get; set; }

        [Required]
        public string NombreCompleto { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        public double Altura { get; set; }

        [Required]
        public double Peso { get; set; }

        [Required]
        public float IMC { get; set; }


        // Claves foráneas
        public int? IdRutina { get; set; }

        public int? IdEntrenamiento { get; set; }


    }
}
