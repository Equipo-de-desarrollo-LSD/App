using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace WayFinder.DestinosTuristicosDTOs;

public class GuardarDestinos
{
      [Required]
      [StringLength(128)] 
      public string Nombre { get; set; } = string.Empty;

    //      [Range(1, int.MaxValue, ErrorMessage = "El id debe ser mayor que cero.")]
      // public Guid Id { get; set; } 

      [Required]
      [StringLength(256)]
      public string Foto { get; set; } = string.Empty;
      public DateTime UltimaActualizacion { get; set; } = DateTime.Now;

    // gemini
    [Required]
    public string PaisNombre { get; set; }
    public double PaisPoblacion { get; set; }

    public double CoordenadasLatitud { get; set; }
    public double CoordenadasLongitud { get; set; }

    /* [Required]
   public PaisDto pais { get; set; } = new PaisDto();

   [Required]
   public CoordenadasDto coordenadas { get; set; } = new CoordenadasDto();
 */
}




