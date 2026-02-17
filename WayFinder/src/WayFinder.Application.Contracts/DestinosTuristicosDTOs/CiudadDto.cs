namespace WayFinder.DestinosTuristicosDTOs
{
    public class CiudadDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
    }
}