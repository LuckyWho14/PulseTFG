using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PulseTFG.ViewModel;

namespace PulseTFG
{
    public static class AppState
    {
        public static RoutineCreatorViewModel RoutineCreatorVM { get; set; } = new();
    }
}
