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
        public string Uid { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public double Altura { get; set; }
        public double Peso { get; set; }
        public double IMC => Peso / Math.Pow(Altura / 100, 2);

        public DateTime FechaCreacion { get; set; }
    }
}
