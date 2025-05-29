using PulseTFG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseTFG.FirebaseService.RutinaPredet
{
    public interface IRutinaParser 
    {
        Task<Rutina> ParsearDesdeTxt(string filePath);
        Task<string> ConvertirAJson(Rutina rutina);
    }
}
