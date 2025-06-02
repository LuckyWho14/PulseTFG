// En tu clase MensajePopup.xaml.cs
using CommunityToolkit.Maui.Views;
using System;

namespace PulseTFG.Pages;

public partial class MensajePopup : Popup
{
    // Arreglo local de frases motivacionales (ejemplos en espa�ol)
    private static readonly string[] FrasesMotivacionales = new string[] {
        "Hoy es un buen d�a para comenzar de nuevo.",
        "Cree en ti. Cada paso cuenta.",
        "Tu esfuerzo vale m�s de lo que imaginas.",
        "Nunca subestimes el poder de la constancia.",
        "Est�s m�s cerca de lo que crees.",
        "Los logros nacen del compromiso diario.",
        "Cada intento es una victoria sobre la inacci�n.",
        "El verdadero progreso es mejorar un poco cada d�a.",
        "T� controlas tu actitud. Hoy eliges avanzar.",
        "El �xito empieza por decidir intentarlo.",
        "Conf�a en el proceso. Tu trabajo est� dando frutos.",
        "Un peque�o paso hoy vale m�s que nada ma�ana.",
        "La disciplina vence al talento cuando el talento no trabaja.",
        "Tu esfuerzo de hoy es el resultado de ma�ana.",
        "Aprende, adapta, contin�a.",
        "Cada d�a trae una nueva oportunidad de mejorar.",
        "Hazlo por ti. Tu mejor versi�n lo merece.",
        "La clave est� en no rendirse.",
        "Incluso los d�as dif�ciles construyen fuerza.",
        "No es perfecto, pero es progreso.",
        "Hoy te lo mereces m�s que ayer.",
        "T� eres tu competencia m�s importante.",
        "Todo gran cambio comienza con un paso peque�o.",
        "El progreso lento sigue siendo progreso.",
        "Tu constancia es tu superpoder.",
        "Haz que tu yo futuro se sienta orgulloso.",
        "No est�s empezando de cero. Est�s comenzando con experiencia.",
        "El cambio comienza cuando decides actuar.",
        "La motivaci�n te enciende. La disciplina te mantiene.",
        "No te detengas. Est�s construyendo algo grande.",
        "Eres capaz de m�s de lo que crees.",
        "Hoy es tu oportunidad para mejorar un 1%.",
        "Tus l�mites est�n donde t� los pongas.",
        "La perseverancia es el camino silencioso al �xito.",
        "Incluso el progreso invisible cuenta.",
        "Aplaude tu esfuerzo. Siempre.",
        "No tienes que ser perfecto, solo constante.",
        "Valora cada paso que das, no importa el tama�o.",
        "T� escribes tu historia, d�a a d�a.",
        "Hazlo con miedo, pero hazlo.",
        "Las excusas apagan, el compromiso enciende.",
        "Ser constante es un acto de valent�a diaria.",
        "El �xito se construye cuando nadie te ve.",
        "Hoy te acercas un poco m�s a tus metas.",
        "Sigue empujando. Est�s creciendo.",
        "Haz de tu esfuerzo un h�bito.",
        "El cambio no es inmediato, pero s� inevitable si no paras.",
        "No necesitas suerte. Solo decisi�n y constancia.",
        "T� eliges si hoy es el comienzo o una excusa m�s.",
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