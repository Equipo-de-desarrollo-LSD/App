namespace WayFinder.DestinosTuristicosDTOs
{
  public class CiudadDto
{
    // Mantenemos el "= string.Empty" que venía de Main (es mejor práctica)
    public string Nombre { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    
    public double Latitud { get; set; }
    public double Longitud { get; set; }

    // Mantenemos tu nueva propiedad que venía de la 3.2
    public double PaisPoblacion { get; set; } 
}