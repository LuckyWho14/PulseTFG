namespace PulseTFG;

public class Usuario
{
    public string Uid { get; set; }              // UID Firebase Auth, clave principal
    public string Email { get; set; }
    public string NombreCompleto { get; set; }
    public DateTime FechaNacimiento { get; set; }     
    public double Altura { get; set; }            
    public double Peso { get; set; }              
	
}