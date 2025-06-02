// En tu clase MensajePopup.xaml.cs
using CommunityToolkit.Maui.Views;
using System;

namespace PulseTFG.Pages;

public partial class MensajePopup : Popup
{
    // Arreglo local de frases motivacionales (ejemplos en español)
    private static readonly string[] FrasesMotivacionales = new string[] {
        "Hoy es un buen día para comenzar de nuevo.",
        "Cree en ti. Cada paso cuenta.",
        "Tu esfuerzo vale más de lo que imaginas.",
        "Nunca subestimes el poder de la constancia.",
        "Estás más cerca de lo que crees.",
        "Los logros nacen del compromiso diario.",
        "Cada intento es una victoria sobre la inacción.",
        "El verdadero progreso es mejorar un poco cada día.",
        "Tú controlas tu actitud. Hoy eliges avanzar.",
        "El éxito empieza por decidir intentarlo.",
        "Confía en el proceso. Tu trabajo está dando frutos.",
        "Un pequeño paso hoy vale más que nada mañana.",
        "La disciplina vence al talento cuando el talento no trabaja.",
        "Tu esfuerzo de hoy es el resultado de mañana.",
        "Aprende, adapta, continúa.",
        "Cada día trae una nueva oportunidad de mejorar.",
        "Hazlo por ti. Tu mejor versión lo merece.",
        "La clave está en no rendirse.",
        "Incluso los días difíciles construyen fuerza.",
        "No es perfecto, pero es progreso.",
        "Hoy te lo mereces más que ayer.",
        "Tú eres tu competencia más importante.",
        "Todo gran cambio comienza con un paso pequeño.",
        "El progreso lento sigue siendo progreso.",
        "Tu constancia es tu superpoder.",
        "Haz que tu yo futuro se sienta orgulloso.",
        "No estás empezando de cero. Estás comenzando con experiencia.",
        "El cambio comienza cuando decides actuar.",
        "La motivación te enciende. La disciplina te mantiene.",
        "No te detengas. Estás construyendo algo grande.",
        "Eres capaz de más de lo que crees.",
        "Hoy es tu oportunidad para mejorar un 1%.",
        "Tus límites están donde tú los pongas.",
        "La perseverancia es el camino silencioso al éxito.",
        "Incluso el progreso invisible cuenta.",
        "Aplaude tu esfuerzo. Siempre.",
        "No tienes que ser perfecto, solo constante.",
        "Valora cada paso que das, no importa el tamaño.",
        "Tú escribes tu historia, día a día.",
        "Hazlo con miedo, pero hazlo.",
        "Las excusas apagan, el compromiso enciende.",
        "Ser constante es un acto de valentía diaria.",
        "El éxito se construye cuando nadie te ve.",
        "Hoy te acercas un poco más a tus metas.",
        "Sigue empujando. Estás creciendo.",
        "Haz de tu esfuerzo un hábito.",
        "El cambio no es inmediato, pero sí inevitable si no paras.",
        "No necesitas suerte. Solo decisión y constancia.",
        "Tú eliges si hoy es el comienzo o una excusa más.",
        "Siempre es buen momento para comenzar de nuevo.",
        "Nadie progresa sin incomodidad. Agradece el esfuerzo."
        };

    // Generador de n\u00fameros aleatorios para escoger frases
    private static readonly Random Azar = new Random();

    public MensajePopup()
    {
        InitializeComponent();
        // Seleccionar aleatoriamente una frase del arreglo y mostrarla en el Label
        int indice = Azar.Next(FrasesMotivacionales.Length);
        string fraseAleatoria = FrasesMotivacionales[indice];
        MessageLabel.Text = fraseAleatoria;
    }

    // Manejador del bot\u00f3n "Cerrar" para cerrar el popup
    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        Close(); // Cierra el popup program\u00e1ticamente
    }
}