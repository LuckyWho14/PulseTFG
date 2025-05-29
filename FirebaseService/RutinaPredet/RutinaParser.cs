using PulseTFG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PulseTFG.FirebaseService.RutinaPredet
{
    /*
    public class RutinaParser : IRutinaParser
    {
        
        public async Task<Rutina> ParsearDesdeTxt(string filePath)
        {
            var contenido = await FileHelper.LeerArchivoAsync(filePath);
            var lineas = contenido.Split('\n');

            var rutina = new Rutina();
            DiaRutina diaActual = null;

            foreach (var linea in lineas)
            {
                if (linea.StartsWith("Rutina "))
                {
                    rutina.Nombre = linea.Trim();
                }
                else if (linea.Contains("Día") && linea.Contains("-"))
                {
                    diaActual = new DiaRutina
                    {
                        Nombre = linea.Split('-')[1].Trim(),
                        NumeroDia = int.Parse(linea.Split('-')[0].Replace("Día", "").Trim())
                    };
                    rutina.Dias.Add(diaActual);
                }
                // Lógica para parsear ejercicios...
            }

            return rutina;
        }

        public async Task<string> ConvertirAJson(Rutina rutina)
        {
            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Serialize(rutina, opciones);
            
        }
        
        
    }
        */
}
